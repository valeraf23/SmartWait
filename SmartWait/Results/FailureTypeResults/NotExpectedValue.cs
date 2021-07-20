using SmartWait.Helpers;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using ExpressionExtension = VF.ExpressionParser.ExpressionExtension;

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
            var msg = $"{exceptionMsg}{Environment.NewLine}Expected: {ExpressionExtension.ConvertToString(_waitCondition)}";
            if (ActuallyValue is not null && ActuallyValue.GetType().IsPrimitiveOrString())
            {
                return $"{msg}, but parameter \'{_waitCondition.Parameters.First().Name}\': {ActuallyValue}";
            }
            return ReplaceParameters(msg);
        }

        private string ReplaceParameters(string msg)
        {
            var getters = MemberExpressionHelper.GetMembersFunctions<T>(_waitCondition);

            foreach (var getter in getters)
            {
                var value = getter.Getter(ActuallyValue);
                var pattern = getter.Key;
                var target = $"{pattern}({GetValuePattern(value)})";
                Regex regex = new(pattern);
                var result = regex.Replace(msg, target);
                msg = result;
            }

            return msg;
        }

        private static string GetValuePattern(object? obj) =>
            obj switch
            {
                string => $"\"{obj}\"",
                _ => $"{obj ?? "null"}"
            };
    }
}