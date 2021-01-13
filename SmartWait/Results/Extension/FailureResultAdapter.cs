using SmartWait.Core;
using SmartWait.Results.FailureTypeResults;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SmartWait.Results.Extension
{
    public static partial class ResultAdapter
    {
        public static Result<TSuccess, FailureResult> WhenWasExceptions<TSuccess>(
            this Result<TSuccess, FailureResult> result,
            [NotNull] Func<ExceptionsHappened, TSuccess> func) => OnFailure(result, func, f => f as ExceptionsHappened);

        public static Result<TSuccess, FailureResult> WhenNotExpectedValue<TSuccess>(
            this Result<TSuccess, FailureResult> result,
            [NotNull] Func<NotExpectedValue<TSuccess>, TSuccess> func) =>
            OnFailure(result, func, f => f as NotExpectedValue<TSuccess>);

        public static Result<TSuccess, FailureResult> DoWhenWasExceptions<TSuccess>(
            this Result<TSuccess, FailureResult> result,
            [NotNull] Action<ExceptionsHappened> map) => DoOnFailure(result, map, f => f as ExceptionsHappened);

        public static Result<TSuccess, FailureResult> DoNotExpectedValue<TSuccess>(
            this Result<TSuccess, FailureResult> result,
            [NotNull] Action<NotExpectedValue<TSuccess>> map) =>
            DoOnFailure(result, map, f => f as NotExpectedValue<TSuccess>);

        private static Result<TSuccess, FailureResult> OnFailure<TSuccess, TNewFailure>(
            this Result<TSuccess, FailureResult> result,
            [NotNull] Func<TNewFailure, TSuccess> func,
            Func<FailureResult, TNewFailure?> when)
            where TNewFailure : FailureResult
        {
            if (result is not Failure<TSuccess, FailureResult> failure) return result;
            var newFailure = when(failure);
            if (newFailure == null) return failure;

            return func(newFailure);
        }

        private static Result<TSuccess, FailureResult> DoOnFailure<TSuccess, TNewFailure>(
            this Result<TSuccess, FailureResult> result,
            [NotNull] Action<TNewFailure> map,
            Func<FailureResult, TNewFailure?> when)
            where TNewFailure : FailureResult
        {
            if (result is not Failure<TSuccess, FailureResult> failure) return result;
            var newFailure = when(failure);
            if (newFailure != null) map(newFailure);

            return failure;
        }

        public static TSuccess OnFailureThrowException<TSuccess>(this Result<TSuccess, FailureResult> result) =>
            result.OnFailure(fr => throw new WaitConditionalException(fr.ToString()));
    }
}