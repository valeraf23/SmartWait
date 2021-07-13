using SmartWait.WaitSteps;
using System;
using System.Collections.Generic;

namespace SmartWait.Core.Sync
{
    public class WaitBuilder<T>
    {
        private readonly Dictionary<string, Action> _actions = new();
        private readonly Wait<T> _wait;
        private readonly WaitBuilderBase _waitBuilderBase;

        public WaitBuilder(Func<T> factory)
        {
            _wait = new Wait<T>(factory);
            _waitBuilderBase = new WaitBuilderBase(_wait, _actions);
        }

        public WaitBuilder<T> SetTimeBetweenStep(TimeSpan step)
        {
            _waitBuilderBase.SetTimeBetweenStep(step);
            return this;
        }

        public WaitBuilder<T> SetTimeOutMessage(string timeOutMessage)
        {
            _waitBuilderBase.SetTimeOutMessage(timeOutMessage);
            return this;
        }

        public WaitBuilder<T> SetTimeBetweenStep(Func<int, TimeSpan> step)
        {
            _waitBuilderBase.SetTimeBetweenStep(step);
            return this;
        }

        public WaitBuilder<T> SetTimeBetweenStep(IStep<int> step)
        {
            _waitBuilderBase.SetTimeBetweenStep(step);
            return this;
        }

        public WaitBuilder<T> SetLogarithmStep(Time time)
        {
            _waitBuilderBase.SetLogarithmStep(time);
            return this;
        }

        public WaitBuilder<T> SetParabolaStep(Time time)
        {
            _waitBuilderBase.SetParabolaStep(time);
            return this;
        }

        public WaitBuilder<T> SetMaxWaitTime(TimeSpan maxWaitTime)
        {
            _waitBuilderBase.SetMaxWaitTime(maxWaitTime);
            return this;
        }

        public WaitBuilder<T> SetCallbackForSuccessful(Action<int, TimeSpan> callbackIfWaitSuccessful)
        {
            _waitBuilderBase.SetCallbackForSuccessful(callbackIfWaitSuccessful);
            return this;
        }

        public WaitBuilder<T> SetNotIgnoredExceptionType(IEnumerable<Type> types)
        {
            _waitBuilderBase.SetNotIgnoredExceptionType(types);
            return this;
        }

        public WaitBuilder<T> SetNotIgnoredExceptionType(Type type, params Type[] types)
        {
            _waitBuilderBase.SetNotIgnoredExceptionType(type, types);
            return this;
        }

        public WaitBuilder<T> SetNotIgnoredExceptionType<TException>()
        {
            _waitBuilderBase.SetNotIgnoredExceptionType<TException>();
            return this;
        }

        public Wait<T> Build()
        {
            foreach (var act in _actions.Values) act();

            return _wait;
        }
    }
}