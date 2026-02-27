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
            bool continueOnCapturedContext = false,
            CancellationToken cancellationToken = default)
        {
            var retryAttempt = 0;
            TSuccessResult? value = default;
            List<Exception> ex = new();
            var wc = waitCondition.Compile();
            var stopwatch = Stopwatch.StartNew();
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
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

                var stopwatchElapsed = stopwatch.Elapsed;
                var canRetry = stopwatchElapsed < maxWaitTime;
                if (!canRetry)
                {
                    var baseFailureResult =
                        FailureResult.Create(retryAttempt, maxWaitTime, stopwatchElapsed, timeoutMessage);
                    return ex.Any()
                        ? baseFailureResult.WhenExceptions(ex)
                        : baseFailureResult.WhenNotExpectedValue(value, waitCondition!);
                }

                var remaining = maxWaitTime - stopwatchElapsed;
                if (remaining <= TimeSpan.Zero) continue;

                var sleep = stepEngine.Invoke(retryAttempt);
                if (sleep < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException(nameof(stepEngine), sleep, "Step engine must not return a negative sleep duration.");
                var delay = sleep <= remaining ? sleep : remaining;
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(continueOnCapturedContext);
                }
            } while (true);
        }
    }
}
