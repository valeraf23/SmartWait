using System;

namespace SmartWait
{
    public class Builder<T>
    {
        private readonly Wait<T> _wait;

        public Builder(Func<T> factory) => _wait = new Wait<T>(factory);
        public Builder(Wait<T> wait) => _wait = wait;

        public T Become(Func<T, bool> predicate, string timeoutMessage) => _wait.For(predicate, timeoutMessage);
    }
}