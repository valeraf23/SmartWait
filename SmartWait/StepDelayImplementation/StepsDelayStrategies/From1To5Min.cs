using System;

namespace SmartWait.StepDelayImplementation.StepsDelayStrategies
{
    internal sealed class From1To5Min : IWaitStep
    {
        private readonly TimeSpan _maxWaitTime;

        public From1To5Min(TimeSpan maxWaitTime) => _maxWaitTime = maxWaitTime;

        public bool IsValid() => TimeSpan.FromMinutes(1) < _maxWaitTime && _maxWaitTime < TimeSpan.FromMinutes(5);

        public TimeSpan GetStep() => TimeSpan.FromMilliseconds(_maxWaitTime.TotalMilliseconds / 40);
    }
}