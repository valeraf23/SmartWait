using SmartWait.WaitSteps;
using System;
using System.Collections.Generic;

namespace SmartWait.Core
{
    public abstract class WaitBase
    {
        public Action<int, TimeSpan> CallbackIfWaitSuccessful { get; set; } = (_, _) => { };
        public TimeSpan MaxWaitTime { get; set; }= TimeSpan.FromSeconds(30);
        public List<Type> NotIgnoredExceptionType { get; } = new();
        public string TimeoutMessage { get; set; } = string.Empty;
        public Func<int, TimeSpan> Step { get; set; } = new LogarithmStep(Time.FromSeconds).Invoke;
    }
}