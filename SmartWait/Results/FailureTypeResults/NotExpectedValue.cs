using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using VF.ExpressionParser;
using TypeExtension = SmartWait.Helpers.TypeExtension;

namespace SmartWait.Results.FailureTypeResults
{
    public sealed class NotExpectedValue<T> : FailureResult
    {
        private readonly Expression<Func<T, bool>> _waitCondition;
        public readonly T ActuallyValue;

        public NotExpectedValue(int retryAttempt, TimeSpan maxWaitTime, TimeSpan stopwatchElapsed,
            string timeoutMessage, T actuallyValue,
            Expression<Func<T, bool>> waitCondition) : base(retryAttempt, maxWaitTime, stopwatchElapsed, timeoutMessage)
        {
            ActuallyValue = actuallyValue;
            _waitCondition = waitCondition;
        }

        public void Deconstruct(out T result) => result = ActuallyValue;

        public override string ToString()
        {
            var exceptionMsg = base.ToString();
            if (typeof(T) == typeof(bool)) return exceptionMsg;
            var msg =
                $"{exceptionMsg}{Environment.NewLine}Expected: {ExpressionExtension.ConvertToString(_waitCondition)}";
            if (ActuallyValue is not null && TypeExtension.IsPrimitiveOrString(ActuallyValue.GetType()))
            {
                return $"{msg}, but parameter \'{_waitCondition.Parameters.First().Name}\': {ActuallyValue}";
            }

            try
            {
                var options = new JsonSerializerOptions {WriteIndented = true};
                string json = JsonSerializer.Serialize(ActuallyValue, options);
                return $"{msg}, but parameter \'{_waitCondition.Parameters.First().Name}\':{Environment.NewLine} {json}";
            }
            catch (Exception)
            {
                // ignored
            }

            return msg;

        }
    }
}