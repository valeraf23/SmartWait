using FluentAssertions;
using NUnit.Framework;
using SmartWait.WaitSteps;
using System;

namespace SmartWait.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    internal class UnitTests
    {
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