using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using SmartWait.Core;
using SmartWait.Core.Async;
using SmartWait.Results.Extension;
using Xunit;

namespace SmartWait.Tests;

public class ExpressionDiagnosticsXunitTests
{
    [Fact]
    public async Task Duplicate_Member_Path_Should_Be_Formatted_For_Each_Occurrence()
    {
        static Task<DiagnosticState> Sut() => Task.FromResult(new DiagnosticState
        {
            Job = new DiagnosticJobWithSequence(null, string.Empty)
        });

        Func<Task> act = async () => await WaitFor.ForAsync(Sut,
                b => b.SetMaxWaitTime(TimeSpan.FromMilliseconds(100)).SetTimeOutMessage("Fail duplicate member path").Build())
            .Become(state => state.Job != null
                && state.Job.DeadLetterReason != null
                && state.Job.DeadLetterReason != string.Empty)
            .OnFailureThrowException();

        var exception = await Assert.ThrowsAsync<WaitConditionalException>(act);

        exception.Message.Should().Contain("Fail duplicate member path")
            .And.Contain("state.Job.DeadLetterReason(null)")
            .And.Contain("state.Job.DeadLetterReason(\"\")")
            .And.NotContain("state.Job.DeadLetterReason(null)(\"\")");
    }

    [Fact]
    public async Task Null_In_Member_Chain_Should_Not_Throw_NullReferenceException()
    {
        static Task<DiagnosticState> Sut() => Task.FromResult(new DiagnosticState
        {
            Saga = null,
            Job = null
        });

        Func<Task> act = async () => await WaitFor.ForAsync(Sut,
                b => b.SetMaxWaitTime(TimeSpan.FromMilliseconds(100)).SetTimeOutMessage("Fail null chain").Build())
            .Become(state => state.Saga != null
                && state.Saga.Stage == 3
                && state.Job != null
                && state.Job.State == 2)
            .OnFailureThrowException();

        var exception = await Assert.ThrowsAsync<WaitConditionalException>(act);

        exception.Message.Should().Contain("Fail null chain")
            .And.Contain("state.Saga(null)")
            .And.Contain("state.Job(null)");
    }


    [Fact]
    public async Task Nullable_Value_Member_Access_Should_Not_Throw_When_Null()
    {
        static Task<NullableValueState> Sut() => Task.FromResult(new NullableValueState
        {
            F = null
        });

        Func<Task> act = async () => await WaitFor.ForAsync(Sut,
                b => b.SetMaxWaitTime(TimeSpan.FromMilliseconds(100)).SetTimeOutMessage("Fail nullable value").Build())
            .Become(state => state.F.HasValue && state.F.Value == 1)
            .OnFailureThrowException();

        var exception = await Assert.ThrowsAsync<WaitConditionalException>(act);

        exception.Message.Should().Contain("Fail nullable value")
            .And.Contain("state.F.HasValue(False)")
            .And.Contain("state.F.Value(null)");
    }

    private sealed class DiagnosticJobWithSequence : DiagnosticJob
    {
        private readonly Queue<string> _values;

        public DiagnosticJobWithSequence(params string[] values)
        {
            _values = new Queue<string>(values);
        }

        public override string DeadLetterReason => _values.Count > 0 ? _values.Dequeue() : null;
    }


    private sealed class NullableValueState
    {
        public int? F { get; init; }
    }

    private sealed class DiagnosticState
    {
        public DiagnosticSaga Saga { get; init; }
        public DiagnosticJob Job { get; init; }
    }

    private sealed class DiagnosticSaga
    {
        public int Stage { get; init; }
    }

    private class DiagnosticJob
    {
        public virtual int State { get; init; }
        public virtual string DeadLetterReason { get; init; }
    }
}
