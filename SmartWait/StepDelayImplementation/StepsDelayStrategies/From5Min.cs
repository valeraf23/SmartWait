using System;

namespace SmartWait.StepDelayImplementation.StepsDelayStrategies
{
    internal sealed class From5Min : IWaitStep
    {
        private readonly TimeSpan _maxWaitTime;

        public From5Min(TimeSpan maxWaitTime) => _maxWaitTime = maxWaitTime;

        public bool IsValid() => _maxWaitTime >= TimeSpan.FromMinutes(5);

        public TimeSpan GetStep() => TimeSpan.FromSeconds(10);
    }
}