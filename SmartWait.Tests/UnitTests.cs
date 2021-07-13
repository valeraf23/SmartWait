using System;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;
using SmartWait.Core.Sync;
using SmartWait.WaitSteps;

namespace SmartWait.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    internal class UnitTests
    {
        [TestCase(WaitSteps.Time.FromHours, 8)]
        [TestCase(WaitSteps.Time.FromMilliseconds, 8)]
        [TestCase(WaitSteps.Time.FromMinutes, 8)]
        [TestCase(WaitSteps.Time.FromSeconds, 8)]
        public void Time(Time time, int step)
        {
            var methodName = time.ToString().Replace("From", "");
            var methodInfo = typeof(FluentTimeSpanExtensions).GetMethod(methodName, new[] {step.GetType()});
            var expectedTimeSpan = (TimeSpan) methodInfo.Invoke(null, new object[] {step});
            time.ToSpan(step).Should().Be(expectedTimeSpan);
        }

        [Test]
        public void LogarithmStep_Argument_Should_be_higher_than_0()
        {
            Action act = () => new LogarithmStep().Invoke(0);
            act.Should().Throw<ArgumentException>().And.Message.Should().Contain("Should be higher than 0");
        }

        [Test]
        public void ParabolaStep_Argument_Should_be_higher_than_0()
        {
            Action act = () => new ParabolaStep().Invoke(0);
            act.Should().Throw<ArgumentException>().And.Message.Should().Contain("Should be higher than 0");
        }

        [Test]
        public void WaitBuilder_Should_Throw_Exception_For_duplicate_step()
        {
            Action act = () =>
                new WaitBuilder<int>(() => 3).SetTimeBetweenStep(TimeSpan.FromSeconds(3))
                    .SetTimeBetweenStep(TimeSpan.FromSeconds(4)).Build();
            act.Should().Throw<ArgumentException>().WithMessage("This step has already added (Parameter 'Step')");
        }
    }
}