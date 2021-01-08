using System;
using System.Diagnostics;

namespace SmartWait.Results.FailureTypeResults
{
    [DebuggerDisplay("{" + nameof(ToString) + "()}")]
    public abstract class FailureResult
    {
        private readonly TimeSpan _maxWaitTime;
        private readonly int _retryAttempt;
        private readonly TimeSpan _stopwatchElapsed;
        private readonly string _timeoutMessage;

        protected FailureResult(int retryAttempt, TimeSpan maxWaitTime, TimeSpan stopwatchElapsed,
            string timeoutMessage)
        {
            _retryAttempt = retryAttempt;
            _maxWaitTime = maxWaitTime;
            _stopwatchElapsed = stopwatchElapsed;
            _timeoutMessage = timeoutMessage;
        }

        public static FailureResultBuilder Create(int retryAttempt, TimeSpan maxWaitTime, TimeSpan stopwatchElapsed,
            string timeoutMessage) => new(retryAttempt, maxWaitTime, stopwatchElapsed, timeoutMessage);

        public override string ToString()
        {
            var msg = _timeoutMessage;
            if (!string.IsNullOrEmpty(_timeoutMessage)) msg = $": {_timeoutMessage}";

            return
                $"Timeout after {_stopwatchElapsed.TotalSeconds} second(s) and {"number of attempts".ToUpper()} {_retryAttempt} {msg}";
        }
    }
}