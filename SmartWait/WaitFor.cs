using System;
using SmartWait.Results;

namespace SmartWait
{
    public static class WaitFor
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
        /// <param name="timeoutMessage">Error message for exception</param>
        /// <param name="retryCount"></param>
        /// <param name="retryInterval"></param>
        public static void Condition(Func<bool> waitCondition, string timeoutMessage, int retryCount = 5,
            TimeSpan? retryInterval = null)
        {
            if (retryCount <= 0) throw new ArgumentOutOfRangeException(nameof(retryCount));
            var interval = (retryInterval ?? TimeSpan.FromMilliseconds(100)).TotalMilliseconds;
            var maxWaitTime = interval * retryCount;

            Wait<bool>.CreateBuilder(() => true).SetMaxWaitTime(TimeSpan.FromMilliseconds(maxWaitTime))
                .SetTimeBetweenStep(TimeSpan.FromMilliseconds(interval)).SetTimeOutMessage(timeoutMessage).Build()
                .For(x => waitCondition() == x);
        }

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
            var waiter = Wait<bool>.CreateBuilder(() => true);
            waiter.SetTimeOutMessage(timeoutMessage);
            buildWaiter(waiter)
                .For(x => waitCondition() == x).OnFailureThrowException();
        }

        public static Builder<T> For<T>(Func<T> func) => new Builder<T>(func);

        public static Builder<T> For<T>(Func<T> func, Func<WaitBuilder<T>, Wait<T>> buildWaiter) => new Builder<T>(buildWaiter(Wait<T>.CreateBuilder(func)));
    }
}