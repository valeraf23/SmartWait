using System;

namespace SmartWait.StepDelayImplementation.StepsDelayStrategies
{
    internal sealed class LessThanOneMinute : IWaitStep
    {
        private readonly TimeSpan _maxWaitTime;

        public LessThanOneMinute(TimeSpan maxWaitTime) => _maxWaitTime = maxWaitTime;

        public bool IsValid() => _maxWaitTime <= TimeSpan.FromMinutes(1);

        public TimeSpan GetStep() => TimeSpan.FromMilliseconds(_maxWaitTime.TotalMilliseconds / 60);
    }
}