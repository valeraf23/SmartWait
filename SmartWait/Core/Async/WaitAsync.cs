using SmartWait.Results;
using SmartWait.Results.FailureTypeResults;
using System;
using System.Linq.Expressions;
using System.Threading;
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

        public Task<Result<T, FailureResult>> For(Expression<Func<T, bool>> waitCondition) => For(waitCondition, CancellationToken.None);

        public Task<Result<T, FailureResult>> For(Expression<Func<T, bool>> waitCondition, CancellationToken cancellationToken) => WaitEngineAsync.ExecuteAsync(
                Factory,
                waitCondition,
                MaxWaitTime,
                Step,
                TimeoutMessage,
                NotIgnoredExceptionType,
                CallbackIfWaitSuccessful,
                cancellationToken: cancellationToken);

        public static WaitBuilderAsync<T> CreateBuilder(Func<Task<T>> factory) => new(factory);
    }
}
