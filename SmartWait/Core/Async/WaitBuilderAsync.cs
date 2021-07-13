using SmartWait.WaitSteps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartWait.Core.Async
{
    public class WaitBuilderAsync<T>
    {
        private readonly Dictionary<string, Action> _actions = new();
        private readonly WaitAsync<T> _wait;
        private readonly WaitBuilderBase _waitBuilderBase;

        public WaitBuilderAsync(Func<Task<T>> factory)
        {
            _wait = new WaitAsync<T>(factory);
            _waitBuilderBase = new WaitBuilderBase(_wait, _actions);
        }

        public WaitBuilderAsync<T> SetTimeBetweenStep(TimeSpan step)
        {
            _waitBuilderBase.SetTimeBetweenStep(step);
            return this;
        }

        public WaitBuilderAsync<T> SetTimeOutMessage(string timeOutMessage)
        {
            _waitBuilderBase.SetTimeOutMessage(timeOutMessage);
            return this;
        }

        public WaitBuilderAsync<T> SetTimeBetweenStep(Func<int, TimeSpan> step)
        {
            _waitBuilderBase.SetTimeBetweenStep(step);
            return this;
        }

        public WaitBuilderAsync<T> SetTimeBetweenStep(IStep<int> step)
        {
            _waitBuilderBase.SetTimeBetweenStep(step);
            return this;
        }

        public WaitBuilderAsync<T> SetLogarithmStep(Time time)
        {
            _waitBuilderBase.SetLogarithmStep(time);
            return this;
        }

        public WaitBuilderAsync<T> SetParabolaStep(Time time)
        {
            _waitBuilderBase.SetParabolaStep(time);
            return this;
        }

        public WaitBuilderAsync<T> SetMaxWaitTime(TimeSpan maxWaitTime)
        {
            _waitBuilderBase.SetMaxWaitTime(maxWaitTime);
            return this;
        }

        public WaitBuilderAsync<T> SetCallbackForSuccessful(Action<int, TimeSpan> callbackIfWaitSuccessful)
        {
            _waitBuilderBase.SetCallbackForSuccessful(callbackIfWaitSuccessful);
            return this;
        }

        public WaitBuilderAsync<T> SetNotIgnoredExceptionType(IEnumerable<Type> types)
        {
            _waitBuilderBase.SetNotIgnoredExceptionType(types);
            return this;
        }

        public WaitBuilderAsync<T> SetNotIgnoredExceptionType(Type type, params Type[] types)
        {
            _waitBuilderBase.SetNotIgnoredExceptionType(type, types);
            return this;
        }

        public WaitBuilderAsync<T> SetNotIgnoredExceptionType<TException>()
        {
            _waitBuilderBase.SetNotIgnoredExceptionType<TException>();
            return this;
        }

        public WaitAsync<T> Build()
        {
            foreach (var act in _actions.Values) act();

            return _wait;
        }
    }
}