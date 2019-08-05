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
        /// <param name="type"></param>
        public static void Condition<TException>(Func<bool> waitCondition, string timeoutMessage)
            where TException : Exception =>
            Condition(waitCondition, timeoutMessage, TimeSpan.FromSeconds(30), ExceptionHandling.ThrowPredefined, null,
                typeof(TException));

        /// <summary>
        /// Wait for some event. Throws exception if event did not appear.
        /// Default wait time is 30 seconds
        /// </summary>
        /// <param name="waitCondition">Method that will return true if event appeared. Wait in stops in case of true</param>
        /// <param name="timeoutMessage">Error message for exception</param>
        /// <param name="exceptionHandling">If true it will ignore all exceptions during waiting</param>
        public static void Condition(Func<bool> waitCondition, string timeoutMessage,
            ExceptionHandling exceptionHandling) => Condition(waitCondition, timeoutMessage, TimeSpan.FromSeconds(30),
            exceptionHandling, null);

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
            var maxWaitTime = (retryInterval ?? TimeSpan.FromMilliseconds(100)).TotalMilliseconds * retryCount;
            Condition(waitCondition, timeoutMessage, TimeSpan.FromMilliseconds(maxWaitTime), ExceptionHandling.ThrowPredefined, null);
        }

        /// <summary>
        /// Wait for some event. Throws exception if event did not appear
        /// </summary>
        /// <param name="waitCondition">Method that will return true if event appeared. Wait in stops in case of true</param>
        /// <param name="maxWaitTime">Max wait time. Exception will be thrown if event will not appear after this time</param>
        /// <param name="timeoutMessage">Error message for exception</param>
        public static void Condition(Func<bool> waitCondition, string timeoutMessage, TimeSpan maxWaitTime) =>
            Condition(waitCondition, timeoutMessage, maxWaitTime, ExceptionHandling.ThrowPredefined, null);

        /// <summary>
        /// Wait for some event. Throws exception if event did not appear
        /// </summary>
        /// <param name="waitCondition">Method that will return true if event appeared. Wait in stops in case of true</param>
        /// <param name="maxWaitTime">Max wait time. Exception will be thrown if event will not appear after this time</param>
        /// <param name="timeoutMessage">Error message for exception</param>
        /// <param name="callback"></param>
        public static void Condition(Func<bool> waitCondition, string timeoutMessage, TimeSpan maxWaitTime,
            Action<TimeSpan> callback) => Condition(waitCondition, timeoutMessage, maxWaitTime,
            ExceptionHandling.ThrowPredefined, callback);

        /// <summary>
        /// Wait for some event. Throws exception if event did not appear
        /// </summary>
        /// <param name="waitCondition">Method that will return true if event appeared. Wait in stops in case of true</param>
        /// <param name="maxWaitTime">Max wait time. Exception will be thrown if event will not appear after this time</param>
        /// <param name="timeoutMessage">Error message for exception</param>
        /// <param name="exceptionHandling">If true it will ignore all exceptions during waiting</param>
        public static void Condition(Func<bool> waitCondition, string timeoutMessage, TimeSpan maxWaitTime,
            ExceptionHandling exceptionHandling) =>
            Condition(waitCondition, timeoutMessage, maxWaitTime, exceptionHandling, null);

        /// <summary>
        /// Wait for some event. Throws exception if event did not appear
        /// </summary>
        /// <param name="waitCondition">Method that will return true if event appeared. Wait in stops in case of true</param>
        /// <param name="maxWaitTime">Max wait time. Exception will be thrown if event will not appear after this time</param>
        /// <param name="timeoutMessage">Error message for exception</param>
        /// <param name="callback"></param>
        /// <param name="exceptionHandling">If true it will ignore all exceptions during waiting</param>
        /// <param name="notIgnoredExceptionType"></param>
        public static void Condition(Func<bool> waitCondition, string timeoutMessage, TimeSpan maxWaitTime,
            ExceptionHandling exceptionHandling, Action<TimeSpan> callback, params Type[] notIgnoredExceptionType) =>
            new Wait<bool>(() => true).SetMaxWaitTime(maxWaitTime).SetExceptionHandling(exceptionHandling)
                .SetCallbackForSuccessful(callback).SetNotIgnoredExceptionType(notIgnoredExceptionType)
                .For(x => waitCondition() == x, timeoutMessage);

        public static Builder<T> For<T>(Func<T> func) => new Builder<T>(func);

        public static Builder<T> For<T>(Func<T> func, Func<Wait<T>, Wait<T>> buildWaiter) =>
            new Builder<T>(buildWaiter(new Wait<T>(func)));
    }
}