using System;
using System.Runtime.CompilerServices;

namespace SmartWait.WaitSteps
{
    public readonly struct ParabolaStep : IStep<int>
    {
        private readonly Time _time;

        public ParabolaStep(Time time) => _time = time;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TimeSpan Invoke(int step)
        {
            if (step <= 0) throw new ArgumentException("Should be higher than 0", nameof(step));
            var s = Math.Pow(2, step);
            return _time.ToSpan(s);
        }
    }
}