using System;
using System.Collections.Generic;
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
        public TimeSpan _step = default(TimeSpan);
        public readonly Func<T> Factory;
        public TimeSpan MaxWaitTime;
        public IList<Type> NotIgnoredExceptionType;


        public TimeSpan Step
        {
            get
            {
                if (_step == default(TimeSpan))
                {
                    _step = new CalculateStepDelay(MaxWaitTime).CalculateDefaultStepWaiter();
                }

                return _step;
            }
            set => _step = value;
        }

        public Wait(Func<T> factory)
        {
            Factory = factory;
            MaxWaitTime = TimeSpan.FromSeconds(30);
            ExceptionHandling = ExceptionHandling.ThrowPredefined;
            CallbackIfWaitSuccessful = null;
        }



        public T For(Func<T, bool> waitCondition, string timeoutMessage)
        {
            var waitExceptionHandler = new WaitExceptionHandler(MaxWaitTime, ExceptionHandling);
            var stopwatch = Stopwatch.StartNew();
            do
            {
                try
                {
                    var value = Factory();
                    if (waitCondition(value))
                    {
                        CallbackIfWaitSuccessful?.Invoke(stopwatch.Elapsed);
                        return value;
                    }
                }
                catch (Exception e) when (e.GetType() == NotIgnoredExceptionType?.GetType())
                {
                    throw;
                }

                catch (Exception e)
                {
                    waitExceptionHandler.CreateExceptionMessage(e.Demystify());
                }

                Thread.Sleep(Step);
            } while (stopwatch.Elapsed < MaxWaitTime);

            waitExceptionHandler.ThrowException(timeoutMessage);
            return Factory();
        }

        public static WaitBuilder<T> CreateBuilder(Func<T> factory) => new WaitBuilder<T>(factory);
    }
}