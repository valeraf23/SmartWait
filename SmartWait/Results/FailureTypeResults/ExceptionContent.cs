using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace SmartWait.Results.FailureTypeResults
{
    public class ExceptionContent : IEquatable<ExceptionContent>
    {
        public ExceptionContent(Exception exception)
        {
            Exception = exception;
            CallStack = exception.ToStringDemystified();
        }

        [JsonIgnore] public Exception Exception { get; }

        public string? CallStack { get; }

        public bool Equals(ExceptionContent? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return CallStack == other.CallStack;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ExceptionContent)obj);
        }

        public static bool operator ==(ExceptionContent? a, ExceptionContent? b) => a is null && b is null ||
                   a?.Equals(b) == true;

        public static bool operator !=(ExceptionContent? a, ExceptionContent? b) => !(a == b);

        public override int GetHashCode() => CallStack is not null ? CallStack.GetHashCode() : 0;
    }
}