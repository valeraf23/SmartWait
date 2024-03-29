using SmartWait.Results;
using SmartWait.Results.FailureTypeResults;
using System;
using System.Linq.Expressions;

namespace SmartWait.Core.Sync
{
    public readonly struct Builder<T>
    {
        private readonly Wait<T> _wait;

        public Builder(Func<T> factory) => _wait = new Wait<T>(factory);

        public Builder(Wait<T> wait) => _wait = wait;

        public Result<T, FailureResult> Become(Expression<Func<T, bool>> predicate) => _wait.For(predicate);
    }
}