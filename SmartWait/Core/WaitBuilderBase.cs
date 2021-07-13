using SmartWait.WaitSteps;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartWait.Core
{
    internal class WaitBuilderBase
    {
        private readonly Dictionary<string, Action> _actions;
        private readonly WaitBase _wait;

        public WaitBuilderBase(WaitBase wait, Dictionary<string, Action> actions)
        {
            _wait = wait;
            _actions = actions;
        }

        private void AddAction(string key, Action act)
        {
            if (_actions.ContainsKey(key)) throw new ArgumentException("This step has already added", key);

            _actions.Add(key, act);
        }

        public void SetTimeBetweenStep(TimeSpan step) => AddAction(nameof(_wait.Step), () => _wait.Step = _ => step);

        public void SetTimeOutMessage(string timeOutMessage) => AddAction(nameof(_wait.TimeoutMessage), () => _wait.TimeoutMessage = timeOutMessage);

        public void SetTimeBetweenStep(Func<int, TimeSpan> step) => AddAction(nameof(_wait.Step), () => _wait.Step = step);

        public void SetTimeBetweenStep(IStep<int> step) => AddAction(nameof(_wait.Step), () => _wait.Step = step.Invoke);

        public void SetLogarithmStep(Time time) => AddAction(nameof(_wait.Step), () => _wait.Step = new LogarithmStep(time).Invoke);

        public void SetParabolaStep(Time time) => AddAction(nameof(_wait.Step), () => _wait.Step = new ParabolaStep(time).Invoke);

        public void SetMaxWaitTime(TimeSpan maxWaitTime) => AddAction(nameof(_wait.MaxWaitTime), () => _wait.MaxWaitTime = maxWaitTime);

        public void SetCallbackForSuccessful(Action<int, TimeSpan> callbackIfWaitSuccessful) => AddAction(nameof(_wait.CallbackIfWaitSuccessful),
                () => _wait.CallbackIfWaitSuccessful += callbackIfWaitSuccessful);

        public void SetNotIgnoredExceptionType(IEnumerable<Type> types)
        {
            var notIgnoredExceptionType = types as Type[] ?? types.ToArray();
            var isExceptionsTypes = notIgnoredExceptionType.All(x => x.IsAssignableFrom(typeof(Exception)));
            if (!isExceptionsTypes) throw new ArgumentException("Should be Exception types");

            _wait.NotIgnoredExceptionType.AddRange(notIgnoredExceptionType);
        }

        public void SetNotIgnoredExceptionType(Type type, params Type[] types)
        {
            var typesList = new List<Type>(types) { type };
            SetNotIgnoredExceptionType(typesList);
        }

        public void SetNotIgnoredExceptionType<TException>() => SetNotIgnoredExceptionType(typeof(TException), Array.Empty<Type>());
    }
}