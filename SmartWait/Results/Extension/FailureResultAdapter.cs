using SmartWait.Core;
using SmartWait.Results.FailureTypeResults;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SmartWait.Results.Extension
{
    public static partial class ResultAdapter
    {
        public static Result<TSuccess, TFailure> OnFailureWhenWasExceptions<TSuccess, TFailure>(
            this Result<TSuccess, TFailure> result,
            [NotNull] Func<ExceptionsHappened, TSuccess> func) => OnFailure(result, func, f => f as ExceptionsHappened);

        public static Result<TSuccess, TFailure> OnFailureWhenNotExpectedValue<TSuccess, TFailure>(
            this Result<TSuccess, TFailure> result,
            [NotNull] Func<NotExpectedValue<TSuccess>, TSuccess> func) =>
            OnFailure(result, func, f => f as NotExpectedValue<TSuccess>);

        private static Result<TSuccess, TFailure> OnFailure<TSuccess, TFailure, TNewFailure>(
            this Result<TSuccess, TFailure> result, [NotNull] Func<TNewFailure, TSuccess> func,
            Func<TFailure, TNewFailure> when)
        {
            if (result is not Failure<TSuccess, TFailure> failure) return result;
            var newFailure = when(failure);
            return newFailure != null ? func(newFailure) : result;
        }

        public static TSuccess OnFailureThrowException<TSuccess>(this Result<TSuccess, FailureResult> result) =>
            result.OnFailure(fr => throw new WaitConditionalException(fr.ToString()));
    }
}
