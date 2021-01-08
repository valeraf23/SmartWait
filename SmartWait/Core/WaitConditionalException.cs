using System;

namespace SmartWait.Core
{
    public sealed class WaitConditionalException : Exception
    {
        public WaitConditionalException()
        {
        }

        public WaitConditionalException(string message) : base(message)
        {
        }

        public WaitConditionalException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}