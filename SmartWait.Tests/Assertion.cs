using FluentAssertions;
using FluentAssertions.Execution;
using SmartWait.Results;
using SmartWait.Results.FailureTypeResults;
using System;

namespace SmartWait.Tests
{
    public static class Assertion
    {
        public static void For(Action act)
        {
            using (new AssertionScope())
            {
                act();
            }
        }

        public static TFailureResult AssertFailure<T, TFailureResult>(Result<T, FailureResult> result)
            where TFailureResult : FailureResult
        {
            result.Should().BeOfType<Failure<T, FailureResult>>();
            Func<TFailureResult> act = () => (TFailureResult)(Failure<T, FailureResult>)result;
            act.Should().NotThrow();
            return act();
        }
    }
}