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
        public Func<int,TimeSpan> _step;
        public readonly Func<T> Factory;
        public TimeSpan MaxWaitTime;
        public IList<Type> NotIgnoredExceptionType;

        public Func<int,TimeSpan> Step
        {
            get { return _step ?? (_step = _ => new CalculateStepDelay(MaxWaitTime).CalculateDefaultStepWaiter()); }
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
            var retryAttempt = 0;
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

                retryAttempt++;
                Thread.Sleep(Step(retryAttempt));
            } while (stopwatch.Elapsed < MaxWaitTime);

            waitExceptionHandler.ThrowException(timeoutMessage);
            return Factory();
        }

        public static WaitBuilder<T> CreateBuilder(Func<T> factory) => new WaitBuilder<T>(factory);
    }
}