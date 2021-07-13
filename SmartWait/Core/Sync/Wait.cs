using SmartWait.Results;
using SmartWait.Results.FailureTypeResults;
using System;
using System.Linq.Expressions;

namespace SmartWait.Core.Sync
{
    public sealed class Wait<T> : WaitBase
    {
        public readonly Func<T> Factory;

        public Wait(Func<T> factory)
        {
            Factory = factory;
            MaxWaitTime = TimeSpan.FromSeconds(30);
            TimeoutMessage = string.Empty;
        }

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