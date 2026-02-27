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

        public void SetTimeBetweenStep(TimeSpan step)
        {
            if (step < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(step), "Step must be non-negative.");
            AddAction(nameof(_wait.Step), () => _wait.Step = _ => step);
        }

        public void SetTimeOutMessage(string timeOutMessage)
        {
            if (timeOutMessage is null) throw new ArgumentNullException(nameof(timeOutMessage));
            AddAction(nameof(_wait.TimeoutMessage), () => _wait.TimeoutMessage = timeOutMessage);
        }

        public void SetTimeBetweenStep(Func<int, TimeSpan> step)
        {
            if (step is null) throw new ArgumentNullException(nameof(step));
            AddAction(nameof(_wait.Step), () => _wait.Step = step);
        }

        public void SetTimeBetweenStep(IStep<int> step)
        {
            if (step is null) throw new ArgumentNullException(nameof(step));
            AddAction(nameof(_wait.Step), () => _wait.Step = step.Invoke);
        }

        public void SetLogarithmStep(Time time) => AddAction(nameof(_wait.Step), () => _wait.Step = new LogarithmStep(time).Invoke);

        public void SetParabolaStep(Time time) => AddAction(nameof(_wait.Step), () => _wait.Step = new ParabolaStep(time).Invoke);

        public void SetMaxWaitTime(TimeSpan maxWaitTime)
        {
            if (maxWaitTime < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(maxWaitTime), "Max wait time must be non-negative.");
            AddAction(nameof(_wait.MaxWaitTime), () => _wait.MaxWaitTime = maxWaitTime);
        }

        public void SetCallbackForSuccessful(Action<int, TimeSpan> callbackIfWaitSuccessful)
        {
            if (callbackIfWaitSuccessful is null) throw new ArgumentNullException(nameof(callbackIfWaitSuccessful));
            AddAction(nameof(_wait.CallbackIfWaitSuccessful),
                () => _wait.CallbackIfWaitSuccessful += callbackIfWaitSuccessful);
        }

        public void SetNotIgnoredExceptionType(IEnumerable<Type> types)
        {
            if (types is null) throw new ArgumentNullException(nameof(types));

            var notIgnoredExceptionType = types as Type[] ?? types.ToArray();
            var isExceptionsTypes = notIgnoredExceptionType.All(x => x is not null && typeof(Exception).IsAssignableFrom(x));
            if (!isExceptionsTypes) throw new ArgumentException("Should be Exception types", nameof(types));

            _wait.NotIgnoredExceptionType.AddRange(notIgnoredExceptionType);
        }

        public void SetNotIgnoredExceptionType(Type type, params Type[] types)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (types is null) throw new ArgumentNullException(nameof(types));

            var typesList = new List<Type>(types) { type };
            SetNotIgnoredExceptionType(typesList);
        }

        public void SetNotIgnoredExceptionType<TException>() => SetNotIgnoredExceptionType(typeof(TException), Array.Empty<Type>());
    }
}
