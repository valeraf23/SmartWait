using SmartWait.WaitSteps;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartWait.Core
{
    public class WaitBuilder<T>
    {
        private readonly Dictionary<string, Action> _actions = new();
        private readonly Wait<T> _wait;

        public WaitBuilder(Func<T> factory) => _wait = new Wait<T>(factory);

        private void AddAction(string key, Action act)
        {
            if (_actions.ContainsKey(key)) throw new ArgumentException("This step has already added", key);

            _actions.Add(key, act);
        }

        public WaitBuilder<T> SetTimeBetweenStep(TimeSpan step)
        {
            AddAction(nameof(_wait.Step), () => _wait.Step = _ => step);
            return this;
        }

        public WaitBuilder<T> SetTimeOutMessage(string timeOutMessage)
        {
            AddAction(nameof(_wait.TimeoutMessage), () => _wait.TimeoutMessage = timeOutMessage);
            return this;
        }

        public WaitBuilder<T> SetTimeBetweenStep(Func<int, TimeSpan> step)
        {
            AddAction(nameof(_wait.Step), () => _wait.Step = step);
            return this;
        }

        public WaitBuilder<T> SetTimeBetweenStep(IStep<int> step)
        {
            AddAction(nameof(_wait.Step), () => _wait.Step = step.Invoke);
            return this;
        }

        public WaitBuilder<T> SetLogarithmStep(Time time)
        {
            AddAction(nameof(_wait.Step), () => _wait.Step = new LogarithmStep(time).Invoke);
            return this;
        }

        public WaitBuilder<T> SetParabolaStep(Time time)
        {
            AddAction(nameof(_wait.Step), () => _wait.Step = new ParabolaStep(time).Invoke);
            return this;
        }

        public WaitBuilder<T> SetMaxWaitTime(TimeSpan maxWaitTime)
        {
            AddAction(nameof(_wait.MaxWaitTime), () => _wait.MaxWaitTime = maxWaitTime);
            return this;
        }

        public WaitBuilder<T> SetCallbackForSuccessful(Action<int, TimeSpan> callbackIfWaitSuccessful)
        {
            AddAction(nameof(_wait.CallbackIfWaitSuccessful),
                () => _wait.CallbackIfWaitSuccessful += callbackIfWaitSuccessful);
            return this;
        }

        public WaitBuilder<T> SetNotIgnoredExceptionType(IEnumerable<Type> types)
        {
            var notIgnoredExceptionType = types as Type[] ?? types.ToArray();
            var isExceptionsTypes = notIgnoredExceptionType.All(x => x.IsAssignableFrom(typeof(Exception)));
            if (!isExceptionsTypes) throw new ArgumentException("Should be Exception types");

            _wait.NotIgnoredExceptionType.AddRange(notIgnoredExceptionType);
            return this;
        }

        public WaitBuilder<T> SetNotIgnoredExceptionType(Type type, params Type[] types)
        {
            var typesList = new List<Type>(types) { type };
            return SetNotIgnoredExceptionType(typesList);
        }

        public WaitBuilder<T> SetNotIgnoredExceptionType<TException>() => SetNotIgnoredExceptionType(typeof(TException), Array.Empty<Type>());

        public Wait<T> Build()
        {
            foreach (var act in _actions.Values) act();

            return _wait;
        }
    }
}