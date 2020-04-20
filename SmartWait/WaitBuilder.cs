using SmartWait.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartWait
{
    public class WaitBuilder<T>
    {
        private readonly Wait<T> _wait;

        public WaitBuilder(Func<T> factory) => _wait = new Wait<T>(factory);

        public WaitBuilder<T> SetTimeBetweenStep(TimeSpan step)
        {
            _wait.Step = _ => step;
            return this;
        }
        public WaitBuilder<T> SetTimeBetweenStep(Func<int, TimeSpan> step)
        {
            _wait.Step = step;
            return this;
        }

        public WaitBuilder<T> SetMaxWaitTime(TimeSpan maxWaitTime)
        {
            _wait.MaxWaitTime = maxWaitTime;
            return this;
        }

        public WaitBuilder<T> SetExceptionHandling(ExceptionHandling exceptionHandling)
        {
            _wait.ExceptionHandling = exceptionHandling;
            return this;
        }

        public WaitBuilder<T> SetCallbackForSuccessful(Action<TimeSpan> callbackIfWaitSuccessful)
        {
            _wait.CallbackIfWaitSuccessful += callbackIfWaitSuccessful;
            return this;
        }

        public WaitBuilder<T> SetNotIgnoredExceptionType(IEnumerable<Type> types)
        {
            var notIgnoredExceptionType = types as Type[] ?? types.ToArray();
            var isExceptionsTypes = notIgnoredExceptionType.All(x => x.IsAssignableFrom(typeof(Exception)));
            if (!isExceptionsTypes)
            {
                throw new ArgumentException("Should be Exception types");
            }

            _wait.NotIgnoredExceptionType = notIgnoredExceptionType;
            return this;
        }

        public WaitBuilder<T> SetNotIgnoredExceptionType(Type type, params Type[] types)
        {
            var typesList = new List<Type>(types) { type };
            return SetNotIgnoredExceptionType(typesList);
        }
        public Wait<T> Build() => _wait;
    }
}