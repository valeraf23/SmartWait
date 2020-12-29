using System;
using System.Linq.Expressions;

namespace SmartWait.Results.FailureTypeResults
{
    public sealed class NotExpectedValue<T> : FailureResult
    {
        private readonly Expression<Func<T, bool>> _waitCondition;
        public readonly T ActuallyValue;

        public NotExpectedValue(double totalSeconds, string timeoutMessage, T actuallyValue,
            Expression<Func<T, bool>> waitCondition) : base(totalSeconds,
            timeoutMessage)
        {
            ActuallyValue = actuallyValue;
            _waitCondition = waitCondition;
        }

        public void Deconstruct(out T result) => result = ActuallyValue;

        public override string ToString()
        {
            var exceptionMsg = base.ToString();

            return
                $"{exceptionMsg}{Environment.NewLine}Expected: {ExpressionExtension.Get(_waitCondition)}, but was {ActuallyValue}";
        }
    }
}