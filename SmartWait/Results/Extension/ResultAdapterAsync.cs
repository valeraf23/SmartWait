using System;
using System.Threading.Tasks;

namespace SmartWait.Results.Extension
{
    public static partial class ResultAdapter
    {
        public static async Task<Result<TNewSuccess, TFailure>> OnSuccess<TSuccess, TFailure, TNewSuccess>(
            this Task<Result<TSuccess, TFailure>> result, Func<TSuccess, TNewSuccess> map)
        {
            var res = await result;
            return res.OnSuccess(map);
        }

        public static async Task<TSuccess> OnFailure<TSuccess, TFailure>(
            this Task<Result<TSuccess, TFailure>> result, Func<TFailure, TSuccess> func)
        {
            var res = await result;
            return res.OnFailure(func);
        }

        public static async Task<Result<TSuccess, TFailure>> OnFailure<TSuccess, TFailure>(
            this Task<Result<TSuccess, TFailure>> result, Func<TFailure, TSuccess> func, Func<TFailure, bool> when)
        {
            var res = await result;
            return res.OnFailure(func, when);
        }

        public static async Task<Result<TSuccess, TFailure>> OnSuccess<TSuccess, TFailure>(this Task<Result<TSuccess, TFailure>> result,
            Action<TSuccess> map)
        {
            var res = await result;
            return res.OnSuccess(map);
        }

        public static async Task<Result<TSuccess, TFailure>> OnFailure<TSuccess, TFailure>(this Task<Result<TSuccess, TFailure>> result,
            Action<TFailure> map)
        {
            var res = await result;
            return res.OnFailure(map);
        }
    }
}