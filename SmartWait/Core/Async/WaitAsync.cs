using SmartWait.Results;
using SmartWait.Results.FailureTypeResults;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmartWait.Core.Async
{
    public sealed class WaitAsync<T> : WaitBase
    {
        public readonly Func<Task<T>> Factory;

        public WaitAsync(Func<Task<T>> factory)
        {
            Factory = factory;
        }

        public Task<Result<T, FailureResult>> For(Expression<Func<T, bool>> waitCondition) => WaitEngineAsync.ExecuteAsync(
                Factory,
                waitCondition,
                MaxWaitTime,
                Step,
                TimeoutMessage,
                NotIgnoredExceptionType,
                CallbackIfWaitSuccessful);

        public static WaitBuilderAsync<T> CreateBuilder(Func<Task<T>> factory) => new(factory);
    }
}