using System;

namespace SmartWait.WaitSteps
{
    public interface IStep<in T> where T : struct
    {
        TimeSpan Invoke(T step);
    }
}