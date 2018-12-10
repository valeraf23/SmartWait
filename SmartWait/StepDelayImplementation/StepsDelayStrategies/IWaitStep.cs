using System;

namespace SmartWait.StepDelayImplementation.StepsDelayStrategies
{
    public interface IWaitStep
    {
        bool IsValid();
        TimeSpan GetStep();
    }
}