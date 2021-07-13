using SmartWait.Results;
using SmartWait.Results.FailureTypeResults;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace SmartWait.Core.Sync
{
    internal static class WaitEngine
    {
        public static Result<TSuccessResult, FailureResult> Execute<TSuccessResult>(
            Func<TSuccessResult> action,
            Expression<Func<TSuccessResult, bool>> waitCondition,
            TimeSpan maxWaitTime,
            Func<int, TimeSpan> stepEngine,
            string timeoutMessage,
            IList<Type> notIgnoredExceptionType,
            Action<int, TimeSpan> callbackIfWaitSuccessful)
        {
            var retryAttempt = 0;
            TSuccessResult? value = default;
            List<Exception> ex = new();
            var wc = waitCondition.Compile();
            var stopwatch = Stopwatch.StartNew();
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

                if (retryAttempt < int.MaxValue) retryAttempt++;

                var sleep = stepEngine.Invoke(retryAttempt);
                var stopwatchElapsed = stopwatch.Elapsed;
                var canRetry = stopwatch.Elapsed < maxWaitTime;
                if (!canRetry)
                {
                    var baseFailureResult =
                        FailureResult.Create(retryAttempt, maxWaitTime, stopwatchElapsed, timeoutMessage);
                    return ex.Any()
                        ? baseFailureResult.WhenExceptions(ex)
                        : baseFailureResult.WhenNotExpectedValue(value, waitCondition!);
                }

                Thread.Sleep(sleep);
            } while (true);
        }
    }
}