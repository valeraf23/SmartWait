using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using SmartWait.Results;
using SmartWait.Results.FailureTypeResults;

namespace SmartWait
{
    internal static class WaitEngine
    {
        public static Result<TSuccessResult, FailureResult> Execute<TSuccessResult>(
            Func<TSuccessResult> action,
            [NotNull] Expression<Func<TSuccessResult, bool>> waitCondition,
            TimeSpan maxWaitTime,
            Func<int, TimeSpan> stepEngine,
            string timeoutMessage,
            IList<Type> notIgnoredExceptionType,
            Action<int, TimeSpan> callbackIfWaitSuccessful)
        {
            var stopwatch = Stopwatch.StartNew();
            var retryAttempt = 0;

            TSuccessResult value = default;
            List<Exception> ex = new();
            var wc = waitCondition.Compile();

            do
            {
                try
                {
                    value = action();
                    if (wc(value))
                    {
                        callbackIfWaitSuccessful?.Invoke(retryAttempt, stopwatch.Elapsed);
                        return value;
                    }
                }
                catch (Exception e) when (notIgnoredExceptionType.Any(x => x == e.GetType()))
                {
                    throw;
                }

                catch (Exception e)
                {
                    ex.Add(e);
                }

                retryAttempt++;
                Thread.Sleep(stepEngine.Invoke(retryAttempt));
            } while (stopwatch.Elapsed < maxWaitTime);

            return ex.Any()
                ? FailureResult.Create(maxWaitTime.TotalSeconds, timeoutMessage, ex)
                : FailureResult.Create(maxWaitTime.TotalSeconds, timeoutMessage, value, waitCondition);
        }
    }
}