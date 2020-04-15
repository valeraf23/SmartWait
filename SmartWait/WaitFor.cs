using System;
using SmartWait.ExceptionHandler;

namespace SmartWait
{
    public static class WaitFor
    {
        /// <summary>
        /// Wait for some event. Throws exception if event did not appear.
        /// Default wait time is 30 seconds
        /// </summary>
        /// <param name="waitCondition">Method that will return true if event appeared. Wait in stops in case of true</param>
        /// <param name="timeoutMessage">Error message for exception</param>
        public static void Condition(Func<bool> waitCondition, string timeoutMessage) =>
            Condition(waitCondition, timeoutMessage, ExceptionHandling.ThrowPredefined);

        /// <summary>
        /// Wait for some event. Throws exception if event did not appear.
        /// Default wait time is 30 seconds
        /// </summary>
        /// <param name="waitCondition">Method that will return true if event appeared. Wait in stops in case of true</param>
        /// <param name="timeoutMessage">Error message for exception</param>
        public static void Condition<TException>(Func<bool> waitCondition, string timeoutMessage)
            where TException : Exception => Condition(waitCondition,
            builder => builder.SetMaxWaitTime(TimeSpan.FromSeconds(30))
                .SetExceptionHandling(ExceptionHandling.ThrowPredefined).SetNotIgnoredExceptionType(typeof(TException))
                .Build(), timeoutMessage);


        /// <summary>
        /// Wait for some event. Throws exception if event did not appear.
        /// Default wait time is 30 seconds
        /// </summary>
        /// <param name="waitCondition">Method that will return true if event appeared. Wait in stops in case of true</param>
        /// <param name="timeoutMessage">Error message for exception</param>
        /// <param name="exceptionHandling">If true it will ignore all exceptions during waiting</param>
        public static void Condition(Func<bool> waitCondition, string timeoutMessage,
            ExceptionHandling exceptionHandling) =>
            Condition(waitCondition,
                builder => builder.SetMaxWaitTime(TimeSpan.FromSeconds(30))
                    .SetExceptionHandling(exceptionHandling)
                    .Build(), timeoutMessage);

        /// <summary>
        /// Wait for some event. Throws exception if event did not appear
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
                .SetTimeBetweenStep(TimeSpan.FromMilliseconds(interval)).Build()
                .For(x => waitCondition() == x, timeoutMessage);
        }

        /// <summary>
        /// Wait for some event. Throws exception if event did not appear
        /// </summary>
        /// <param name="waitCondition">Method that will return true if event appeared. Wait in stops in case of true</param>
        /// <param name="maxWaitTime">Max wait time. Exception will be thrown if event will not appear after this time</param>
        /// <param name="timeoutMessage">Error message for exception</param>
        public static void Condition(Func<bool> waitCondition, string timeoutMessage, TimeSpan maxWaitTime) =>
            Condition(waitCondition,
                builder => builder.SetMaxWaitTime(maxWaitTime)
                    .SetExceptionHandling(ExceptionHandling.ThrowPredefined)
                    .Build(), timeoutMessage);


        /// <summary>
        /// Wait for some event. Throws exception if event did not appear
        /// </summary>
        /// <param name="waitCondition">Method that will return true if event appeared. Wait in stops in case of true</param>
        /// <param name="maxWaitTime">Max wait time. Exception will be thrown if event will not appear after this time</param>
        /// <param name="timeoutMessage">Error message for exception</param>
        /// <param name="callback"></param>
        public static void Condition(Func<bool> waitCondition, string timeoutMessage, TimeSpan maxWaitTime,
            Action<TimeSpan> callback) =>
            Condition(waitCondition,
                builder => builder.SetMaxWaitTime(maxWaitTime)
                    .SetExceptionHandling(ExceptionHandling.ThrowPredefined).SetCallbackForSuccessful(callback)
                    .Build(), timeoutMessage);

        /// <summary>
        /// Wait for some event. Throws exception if event did not appear
        /// </summary>
        /// <param name="waitCondition">Method that will return true if event appeared. Wait in stops in case of true</param>
        /// <param name="maxWaitTime">Max wait time. Exception will be thrown if event will not appear after this time</param>
        /// <param name="timeoutMessage">Error message for exception</param>
        /// <param name="exceptionHandling">If true it will ignore all exceptions during waiting</param>
        public static void Condition(Func<bool> waitCondition, string timeoutMessage, TimeSpan maxWaitTime,
            ExceptionHandling exceptionHandling) => Condition(waitCondition,
            builder => builder.SetMaxWaitTime(maxWaitTime)
                .SetExceptionHandling(exceptionHandling)
                .Build(), timeoutMessage);

        /// <summary>
        /// Wait for some event. Throws exception if event did not appear
        /// </summary>
        /// <param name="waitCondition">Method that will return true if event appeared. Wait in stops in case of true</param>
        /// <param name="maxWaitTime">Max wait time. Exception will be thrown if event will not appear after this time</param>
        /// <param name="timeoutMessage">Error message for exception</param>
        /// <param name="callback"></param>
        /// <param name="exceptionHandling">If true it will ignore all exceptions during waiting</param>
        /// <param name="notIgnoredExceptionType"></param>
        [Obsolete("Use Condition with builder")]
        public static void Condition(Func<bool> waitCondition, string timeoutMessage, TimeSpan maxWaitTime,
            ExceptionHandling exceptionHandling, Action<TimeSpan> callback, params Type[] notIgnoredExceptionType) =>
            Wait<bool>.CreateBuilder(() => true).SetMaxWaitTime(maxWaitTime).SetExceptionHandling(exceptionHandling)
                .SetCallbackForSuccessful(callback).SetNotIgnoredExceptionType(notIgnoredExceptionType).Build()
                .For(x => waitCondition() == x, timeoutMessage);

        public static void Condition(Func<bool> waitCondition, Func<WaitBuilder<bool>, Wait<bool>> buildWaiter,
            string timeoutMessage) => buildWaiter(Wait<bool>.CreateBuilder(() => true))
            .For(x => waitCondition() == x, timeoutMessage);

        public static Builder<T> For<T>(Func<T> func) => new Builder<T>(func);

        public static Builder<T> For<T>(Func<T> func, Func<WaitBuilder<T>, Wait<T>> buildWaiter) =>
            new Builder<T>(buildWaiter(Wait<T>.CreateBuilder(func)));
    }
}