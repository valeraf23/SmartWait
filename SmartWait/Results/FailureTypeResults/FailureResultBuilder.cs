using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SmartWait.Results.FailureTypeResults
{
    public class FailureResultBuilder
    {
        private readonly TimeSpan _maxWaitTime;
        private readonly int _retryAttempt;
        private readonly TimeSpan _stopwatchElapsed;
        private readonly string _timeoutMessage;

        public FailureResultBuilder(int retryAttempt, TimeSpan maxWaitTime, TimeSpan stopwatchElapsed,
            string timeoutMessage)
        {
            _retryAttempt = retryAttempt;
            _maxWaitTime = maxWaitTime;
            _stopwatchElapsed = stopwatchElapsed;
            _timeoutMessage = timeoutMessage;
        }

        public FailureResult WhenExceptions(List<Exception> exceptions) => new ExceptionsHappened(_retryAttempt, _maxWaitTime, _stopwatchElapsed, _timeoutMessage, exceptions);

        public FailureResult WhenNotExpectedValue<TSuccessResult>(TSuccessResult actuallyValue,
              Expression<Func<TSuccessResult, bool>> waitCondition) => new NotExpectedValue<TSuccessResult>(_retryAttempt, _maxWaitTime, _stopwatchElapsed, _timeoutMessage,
            actuallyValue, waitCondition);

    }
}