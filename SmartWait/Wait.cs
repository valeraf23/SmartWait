using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SmartWait.Results;
using SmartWait.Results.FailureTypeResults;
using SmartWait.WaitSteps;

namespace SmartWait
{
    public sealed class Wait<T>
    {
        public readonly Func<T> Factory;
        public TimeSpan MaxWaitTime;
        public List<Type> NotIgnoredExceptionType = new();

        public Wait(Func<T> factory)
        {
            Factory = factory;
            MaxWaitTime = TimeSpan.FromSeconds(30);
            CallbackIfWaitSuccessful = (_, _) => { };
            TimeoutMessage = string.Empty;
        }

        public string TimeoutMessage { get; set; }

        public Func<int, TimeSpan> Step { get; set; } = new LogarithmStep(Time.FromSeconds).Invoke;
        public event Action<int, TimeSpan> CallbackIfWaitSuccessful;

        public Result<T, FailureResult> For(Expression<Func<T, bool>> waitCondition) => WaitEngine.Execute(
                Factory,
                waitCondition,
                MaxWaitTime,
                Step,
                TimeoutMessage,
                NotIgnoredExceptionType,
                CallbackIfWaitSuccessful);

        public static WaitBuilder<T> CreateBuilder(Func<T> factory) => new(factory);
    }
}