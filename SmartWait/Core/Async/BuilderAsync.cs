using SmartWait.Results;
using SmartWait.Results.FailureTypeResults;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmartWait.Core.Async
{
    public readonly struct BuilderAsync<T>
    {
        private readonly WaitAsync<T> _wait;

        public BuilderAsync(Func<Task<T>> factory) => _wait = new WaitAsync<T>(factory);

        public BuilderAsync(WaitAsync<T> wait) => _wait = wait;

        public Task<Result<T, FailureResult>> Become(Expression<Func<T, bool>> predicate) => _wait.For(predicate);
    }
}