using SmartWait.StepDelayImplementation.StepsDelayStrategies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartWait.StepDelayImplementation
{
    public sealed class DelayLocator
    {
        private List<IWaitStep> Mapping { get; } = new List<IWaitStep>();

        public void Register(IWaitStep waitStep) => Mapping.Add(waitStep);
        public void Register(ICollection<IWaitStep> waitStep) => Mapping.AddRange(waitStep);

        public void Register(IWaitStep waitStep, params IWaitStep[] waitSteps) =>
            Mapping.AddRange(new List<IWaitStep>(waitSteps) { waitStep });

        public TimeSpan GetStep() => Mapping.First(mapping => mapping.IsValid()).GetStep();
    }
}
