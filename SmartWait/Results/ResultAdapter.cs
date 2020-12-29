using System;
using SmartWait.Results.FailureTypeResults;

namespace SmartWait.Results
{
    public static class ResultAdapter
    {
        public static Result<TNewSuccess, TFailure> OnSuccess<TSuccess, TFailure, TNewSuccess>(
            this Result<TSuccess, TFailure> result, Func<TSuccess, TNewSuccess> map) => result is Success<TSuccess, TFailure> right
                ? (Result<TNewSuccess, TFailure>)map(right)
                : (TFailure)(Failure<TSuccess, TFailure>)result;

        public static TSuccess OnFailure<TSuccess, TFailure>(
            this Result<TSuccess, TFailure> result, Func<TFailure, TSuccess> func)
        {
            if (result is Failure<TSuccess, TFailure> failure) return func(failure);

            return (Success<TSuccess, TFailure>)result;
        }

        public static Result<TSuccess, TFailure> OnFailure<TSuccess, TFailure>(
            this Result<TSuccess, TFailure> result, Func<TFailure, TSuccess> func, Func<TFailure, bool> when)
        {
            if (result is Failure<TSuccess, TFailure> failure && when(failure)) return func(failure);

            return result;
        }

        public static Result<TSuccess, TFailure> OnSuccess<TSuccess, TFailure>(this Result<TSuccess, TFailure> result,
            Action<TSuccess> map)
        {
            if (result is Success<TSuccess, TFailure> success)
            {
                map(success);
                return success;
            }

            return result;
        }

        public static Result<TSuccess, TFailure> OnFailure<TSuccess, TFailure>(this Result<TSuccess, TFailure> result,
            Action<TFailure> map)
        {
            if (!(result is Failure<TSuccess, TFailure> failure)) return result;
            map(failure);
            return failure;
        }

        public static TSuccess OnFailureThrowException<TSuccess>(this Result<TSuccess, FailureResult> result) => result.OnFailure(fr => throw new WaitConditionalException(fr.ToString()));
    }
}