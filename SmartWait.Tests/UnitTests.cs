using System;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;
using SmartWait.Core.Sync;
using SmartWait.WaitSteps;

namespace SmartWait.Tests
{
    public class UnitTests
    {
        [Theory]
        [InlineData("FromHours", 8)]
        [InlineData("FromMilliseconds", 8)]
        [InlineData("FromMinutes", 8)]
        [InlineData("FromSeconds", 8)]
        public void Time(string methodName, int step)
        {
            var extensionMethodName = methodName.Replace("From", "");
            var methodInfo = typeof(FluentTimeSpanExtensions).GetMethod(extensionMethodName, new[] {step.GetType()});
            var expectedTimeSpan = (TimeSpan) methodInfo.Invoke(null, new object[] {step});
            var time = (Time)Enum.Parse(typeof(Time), methodName);
            time.ToSpan(step).Should().Be(expectedTimeSpan);
        }

        [Fact]
        public void LogarithmStep_Argument_Should_be_higher_than_0()
        {
            Action act = () => new LogarithmStep().Invoke(0);
            act.Should().Throw<ArgumentException>().And.Message.Should().Contain("Should be higher than 0");
        }

        [Fact]
        public void ParabolaStep_Argument_Should_be_higher_than_0()
        {
            Action act = () => new ParabolaStep().Invoke(0);
            act.Should().Throw<ArgumentException>().And.Message.Should().Contain("Should be higher than 0");
        }

        [Fact]
        public void WaitBuilder_Should_Throw_Exception_For_duplicate_step()
        {
            Action act = () =>
                new WaitBuilder<int>(() => 3).SetTimeBetweenStep(TimeSpan.FromSeconds(3))
                    .SetTimeBetweenStep(TimeSpan.FromSeconds(4)).Build();
            act.Should().Throw<ArgumentException>().WithMessage("This step has already added (Parameter 'Step')");
        }
    }
}