using SmartWait.Results;
using SmartWait.Results.FailureTypeResults;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWait.Core.Async
{
    internal static class WaitEngineAsync
    {
        public static async Task<Result<TSuccessResult, FailureResult>> ExecuteAsync<TSuccessResult>(
            Func<Task<TSuccessResult>> action,
            Expression<Func<TSuccessResult, bool>> waitCondition,
            TimeSpan maxWaitTime,
            Func<int, TimeSpan> stepEngine,
            string timeoutMessage,
            IList<Type> notIgnoredExceptionType,
            Action<int, TimeSpan> callbackIfWaitSuccessful,
            bool continueOnCapturedContext = false)
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
                    value = await action().ConfigureAwait(continueOnCapturedContext);
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
                    var baseFailureResult = FailureResult.Create(retryAttempt, maxWaitTime, stopwatchElapsed, timeoutMessage);
                    return ex.Any()
                        ? baseFailureResult.WhenExceptions(ex)
                        : baseFailureResult.WhenNotExpectedValue(value, waitCondition!);
                }

                Thread.Sleep(sleep);
            } while (true);
        }
    }
}