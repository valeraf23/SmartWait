using System;
using System.Runtime.CompilerServices;

namespace SmartWait.WaitSteps
{
    public readonly struct LogarithmStep : IStep<int>
    {
        private readonly Time _time;
        private readonly int _start;

        public LogarithmStep(Time time)
        {
            _time = time;
            _start = 0;
        }

        public LogarithmStep(Time time, int start) : this(time) => _start = start;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TimeSpan Invoke(int step)
        {
            if (step <= 0) throw new ArgumentException("Should be higher than 0", nameof(step));
            var s = Math.Log(step) + _start;
            return _time.ToSpan(s);
        }
    }
}