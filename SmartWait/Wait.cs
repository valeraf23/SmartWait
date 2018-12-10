﻿using System;
using System.Diagnostics;
using System.Threading;
using SmartWait.ExceptionHandler;
using SmartWait.StepDelayImplementation;

namespace SmartWait
{
    public sealed class Wait<T>
    {
        public ExceptionHandling ExceptionHandling { get; set; }
        public event Action<TimeSpan> CallbackIfWaitSuccessful;
        private TimeSpan _step = default(TimeSpan);
        private readonly Func<T> _factory;
        private TimeSpan _maxWaitTime;

        public TimeSpan Step
        {
            get
            {
                if (_step == default(TimeSpan))
                {
                    _step = new CalculateStepDelay(_maxWaitTime).CalculateDefaultStepWaiter();
                }

                return _step;
            }
            set => _step = value;
        }

        public Wait(Func<T> factory)
        {
            _factory = factory;
            _maxWaitTime = TimeSpan.FromSeconds(30);
            ExceptionHandling = ExceptionHandling.ThrowPredefined;
            CallbackIfWaitSuccessful = null;
        }

        public Wait<T> SetTimeBetweenStep(TimeSpan step)
        {
            _step = step;
            return this;
        }

        public Wait<T> SetMaxWaitTime(TimeSpan maxWaitTime)
        {
            _maxWaitTime = maxWaitTime;
            return this;
        }

        public Wait<T> SetExceptionHandling(ExceptionHandling exceptionHandling)
        {
            ExceptionHandling = exceptionHandling;
            return this;
        }

        public Wait<T> SetCallbackForSuccessful(Action<TimeSpan> callbackIfWaitSuccessful)
        {
            CallbackIfWaitSuccessful = callbackIfWaitSuccessful;
            return this;
        }

        public T For(Func<T, bool> waitCondition, string timeoutMessage)
        {
            var waitExceptionHandler = new WaitExceptionHandler(_maxWaitTime, ExceptionHandling);
            var stopwatch = Stopwatch.StartNew();
            do
            {
                try
                {
                    var value = _factory();
                    if (waitCondition(value))
                    {
                        CallbackIfWaitSuccessful?.Invoke(stopwatch.Elapsed);
                        return value;
                    }
                }
                catch (Exception e)
                {
                    waitExceptionHandler.CreateExceptionMessage(e.Demystify());
                }

                Thread.Sleep(Step);
            } while (stopwatch.Elapsed < _maxWaitTime);

            waitExceptionHandler.ThrowException(timeoutMessage);
            return _factory();
        }
    }
}