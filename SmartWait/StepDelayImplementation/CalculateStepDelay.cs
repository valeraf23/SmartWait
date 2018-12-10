using System;
using SmartWait.StepDelayImplementation.StepsDelayStrategies;

namespace SmartWait.StepDelayImplementation
{
    internal sealed class CalculateStepDelay
    {
        private readonly DelayLocator _delayLocator = new DelayLocator();

        public CalculateStepDelay(TimeSpan maxWaitTime) => _delayLocator.Register(new From1To5Min(maxWaitTime),
            new From5Min(maxWaitTime),
            new LessThanOneMinute(maxWaitTime));

        public TimeSpan CalculateDefaultStepWaiter() => _delayLocator.GetStep();
    }

}
