using SmartWait.Core.Async;
using SmartWait.Results.Extension;
using System;
using System.Threading.Tasks;

namespace SmartWait.Core
{
    public static partial class WaitFor
    {
        /// <summary>
        ///     Wait for some event. Throws exception if event did not appear.
        ///     Default wait time is 30 seconds
        /// </summary>
        /// <param name="waitCondition">Method that will return true if event appeared. Wait in stops in case of true</param>
        /// <param name="timeoutMessage">Error message for exception</param>
        public static async Task Condition(Func<Task<bool>> waitCondition, string timeoutMessage) => await Condition(waitCondition,
                builder => builder.SetMaxWaitTime(TimeSpan.FromSeconds(30)).Build(), timeoutMessage);

        /// <summary>
        ///     Wait for some event. Throws exception if event did not appear
        /// </summary>
        /// <param name="waitCondition">Method that will return true if event appeared. Wait in stops in case of true</param>
        /// <param name="maxWaitTime">Max wait time. Exception will be thrown if event will not appear after this time</param>
        /// <param name="timeoutMessage">Error message for exception</param>
        /// <param name="callback"></param>
        public static async Task Condition(Func<Task<bool>> waitCondition, string timeoutMessage, TimeSpan maxWaitTime,
            Action<int, TimeSpan> callback) => await Condition(waitCondition,
                builder => builder.SetMaxWaitTime(maxWaitTime)
                    .SetCallbackForSuccessful(callback)
                    .Build(), timeoutMessage);

        /// <summary>
        ///     Wait for some event. Throws exception if event did not appear
        /// </summary>
        /// <param name="waitCondition">Method that will return true if event appeared. Wait in stops in case of true</param>
        /// <param name="maxWaitTime">Max wait time. Exception will be thrown if event will not appear after this time</param>
        /// <param name="timeoutMessage">Error message for exception</param>
        public static async Task Condition(Func<Task<bool>> waitCondition, string timeoutMessage, TimeSpan maxWaitTime) => await Condition(waitCondition,
                builder => builder.SetMaxWaitTime(maxWaitTime).Build(), timeoutMessage);

        public static async Task Condition(Func<Task<bool>> waitCondition,
            Func<WaitBuilderAsync<bool>, WaitAsync<bool>> buildWaiter,
            string timeoutMessage)
        {
            var waiter = WaitAsync<bool>.CreateBuilder(waitCondition);
            waiter.SetTimeOutMessage(timeoutMessage);
            await buildWaiter(waiter)
                .For(x => x).OnFailureThrowException();
        }

        public static BuilderAsync<T> ForAsync<T>(Func<Task<T>> func) => new(func);

        public static BuilderAsync<T> ForAsync<T>(
                Func<Task<T>> func,
                Func<WaitBuilderAsync<T>,
                WaitAsync<T>> buildWaiter)
                => new(buildWaiter(WaitAsync<T>.CreateBuilder(func)));
    }
}