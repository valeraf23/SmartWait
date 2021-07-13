using System;
using System.Threading.Tasks;
using SmartWait.Results.FailureTypeResults;

namespace SmartWait.Results.Extension
{
    public static partial class FailureResultAdapter
    {
        public static async Task<Result<TSuccess, FailureResult>> WhenWasExceptions<TSuccess>(
            this Task<Result<TSuccess, FailureResult>> result,
            Func<ExceptionsHappened, TSuccess> func)
        {
            var res = await result;
            return res.WhenWasExceptions(func);
        }

        public static async Task<Result<TSuccess, FailureResult>> WhenNotExpectedValue<TSuccess>(
            this Task<Result<TSuccess, FailureResult>> result,
            Func<NotExpectedValue<TSuccess>, TSuccess> func)
        {
            var res = await result;
            return res.WhenNotExpectedValue(func);
        }

        public static async Task<Result<TSuccess, FailureResult>> DoWhenWasExceptions<TSuccess>(
            this Task<Result<TSuccess, FailureResult>> result,
            Action<ExceptionsHappened> map)
        {
            var res = await result;
            return res.DoWhenWasExceptions(map);
        }

        public static async Task<Result<TSuccess, FailureResult>> DoWhenNotExpectedValue<TSuccess>(
            this Task<Result<TSuccess, FailureResult>> result,
            Action<NotExpectedValue<TSuccess>> map)
        {
            var res = await result;
            return res.DoWhenNotExpectedValue(map);
        }

        public static async Task<TSuccess> OnFailureThrowException<TSuccess>(this Task<Result<TSuccess, FailureResult>> result)
        {
            var res = await result;
            return res.OnFailureThrowException();
        }
    }
}