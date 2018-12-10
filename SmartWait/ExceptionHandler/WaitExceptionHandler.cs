using System;
using System.Collections.Generic;
using System.Text;

namespace SmartWait.ExceptionHandler
{
    internal sealed class WaitExceptionHandler
    {
        private readonly TimeSpan _maxWaitTime;
        private readonly ExceptionHandling _ignoreExceptionsDuringWait;
        private readonly StringBuilder _exceptionsDuringWait;

        public WaitExceptionHandler(TimeSpan maxWaitTime, ExceptionHandling ignoreExceptionsDuringWait)
        {
            _exceptionsDuringWait = new StringBuilder();
            _maxWaitTime = maxWaitTime;
            _ignoreExceptionsDuringWait = ignoreExceptionsDuringWait;
        }

        private readonly Dictionary<ExceptionHandling, Func<Exception, string>> _exceptionMessageHandler =
            new Dictionary
                <ExceptionHandling, Func<Exception, string>>
                {
                    [ExceptionHandling.Collect] = e => e.Message,
                    [ExceptionHandling.CollectWithStackTrace] = e => e.ToString(),
                    [ExceptionHandling.ThrowPredefined] = e => string.Empty,
                    [ExceptionHandling.Ignore] = e => string.Empty
                };

        public void CreateExceptionMessage(Exception e) =>
            _exceptionsDuringWait.AppendLine(_exceptionMessageHandler[_ignoreExceptionsDuringWait](e));

        public void ThrowException(string timeoutMessage)
        {
            if (_ignoreExceptionsDuringWait == ExceptionHandling.Ignore)
                return;

            var exceptionMsg = $"Timeout after {_maxWaitTime.TotalSeconds} seconds: {timeoutMessage}";
            if (IsExceptionHandlingCollect())
            {
                throw new WaitConditionalException(
                    $"{exceptionMsg}. Exceptions During Wait: ( {_exceptionsDuringWait} )");
            }

            throw new WaitConditionalException(exceptionMsg);
        }

        private bool IsExceptionHandlingCollect() =>
            _ignoreExceptionsDuringWait != ExceptionHandling.ThrowPredefined && !
                string.IsNullOrEmpty(_exceptionsDuringWait.ToString());
    }
}