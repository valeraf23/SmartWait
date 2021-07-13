using SmartWait.Core.Sync;
using SmartWait.Results.Extension;
using System;

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
        public static void Condition(Func<bool> waitCondition, string timeoutMessage) => Condition(waitCondition,
                builder => builder.SetMaxWaitTime(TimeSpan.FromSeconds(30)).Build(), timeoutMessage);

        /// <summary>
        ///     Wait for some event. Throws exception if event did not appear
        /// </summary>
        /// <param name="waitCondition">Method that will return true if event appeared. Wait in stops in case of true</param>
        /// <param name="maxWaitTime">Max wait time. Exception will be thrown if event will not appear after this time</param>
        /// <param name="timeoutMessage">Error message for exception</param>
        /// <param name="callback"></param>
        public static void Condition(Func<bool> waitCondition, string timeoutMessage, TimeSpan maxWaitTime,
            Action<int, TimeSpan> callback) => Condition(waitCondition,
                builder => builder.SetMaxWaitTime(maxWaitTime)
                    .SetCallbackForSuccessful(callback)
                    .Build(), timeoutMessage);

        /// <summary>
        ///     Wait for some event. Throws exception if event did not appear
        /// </summary>
        /// <param name="waitCondition">Method that will return true if event appeared. Wait in stops in case of true</param>
        /// <param name="maxWaitTime">Max wait time. Exception will be thrown if event will not appear after this time</param>
        /// <param name="timeoutMessage">Error message for exception</param>
        public static void Condition(Func<bool> waitCondition, string timeoutMessage, TimeSpan maxWaitTime) => Condition(waitCondition,
                builder => builder.SetMaxWaitTime(maxWaitTime)
                    .Build(), timeoutMessage);

        public static void Condition(Func<bool> waitCondition, Func<WaitBuilder<bool>, Wait<bool>> buildWaiter,
            string timeoutMessage)
        {
            var waiter = Wait<bool>.CreateBuilder(waitCondition);
            waiter.SetTimeOutMessage(timeoutMessage);
            buildWaiter(waiter)
                .For(x => x).OnFailureThrowException();
        }

        public static Builder<T> For<T>(Func<T> func) => new(func);

        public static Builder<T> For<T>(Func<T> func, Func<WaitBuilder<T>, Wait<T>> buildWaiter) => new(buildWaiter(Wait<T>.CreateBuilder(func)));
    }
}