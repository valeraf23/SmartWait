using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace SmartWait.Results.FailureTypeResults
{
    [DebuggerDisplay("{" + nameof(ToString) + "()}")]
    public abstract class FailureResult
    {
        private readonly string _timeoutMessage;
        private readonly double _totalSeconds;

        protected FailureResult(double totalSeconds, string timeoutMessage)
        {
            _totalSeconds = totalSeconds;
            _timeoutMessage = timeoutMessage;
        }

        public static FailureResult Create(double totalSeconds, string timeoutMessage, List<Exception> exceptions) => new ExceptionsHappened(totalSeconds, timeoutMessage, exceptions);

        public static FailureResult Create<T>(double totalSeconds, string timeoutMessage, T actuallyValue,
            Expression<Func<T, bool>> waitCondition) => new NotExpectedValue<T>(totalSeconds, timeoutMessage, actuallyValue, waitCondition);

        public override string ToString()
        {
            var msg = _timeoutMessage;
            if (!string.IsNullOrEmpty(_timeoutMessage)) msg = $": {_timeoutMessage}";

            return $"Timeout after {_totalSeconds} second(s){msg}";
        }
    }
}