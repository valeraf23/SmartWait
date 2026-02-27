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
            var timeoutMessage = base.ToString();
            if (typeof(T) == typeof(bool)) return timeoutMessage;

            var expectedExpression = ExpressionExtension.ConvertToString(_waitCondition);
            if (ActuallyValue is not null && ActuallyValue.GetType().IsPrimitiveOrString())
            {
                var parameterName = _waitCondition.Parameters.First().Name;
                return $"{timeoutMessage}{Environment.NewLine}Expected: {expectedExpression}{Environment.NewLine}Actual {parameterName}: {GetValuePattern(ActuallyValue)}";
            }

            var expectedWithValues = ReplaceParameters(expectedExpression);
            return $"{timeoutMessage}{Environment.NewLine}Expected: {expectedWithValues}";
        }

        private string ReplaceParameters(string expression)
        {
            var getters = MemberExpressionHelper.GetMembersFunctions<T>(_waitCondition);

            foreach (var getter in getters)
            {
                var value = getter.Getter(ActuallyValue);
                var pattern = getter.Key;
                var target = $"{pattern}({GetValuePattern(value)})";
                var escapedPattern = Regex.Escape(pattern);
                var replacePattern = $"{escapedPattern}(?!\\()";
                Regex regex = new(replacePattern);
                expression = regex.Replace(expression, target, 1);
            }

            return expression;
        }

        private static string GetValuePattern(object? obj) =>
            obj switch
            {
                string => $"\"{obj}\"",
                _ => $"{obj ?? "null"}"
            };
    }
}
