using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SmartWait.Core;
using SmartWait.Core.Async;
using SmartWait.Results.Extension;
using SmartWait.Results.FailureTypeResults;
using SmartWait.WaitSteps;

namespace SmartWait.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    internal class WaitForAsyncTest
    {
        private const string DefaultTimeOutMessage = "Fail";

        [Test]
        public async Task Condition_Success()
        {
            //Arrange
            static async Task<int> Expected()
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                return 4;
            }

            //Act
            var res = await WaitEngineAsync.ExecuteAsync(Expected, x => x > 3, TimeSpan.FromSeconds(3),
                i => TimeSpan.FromMilliseconds(i),
                DefaultTimeOutMessage,
                new List<Type>(),
                null!).OnFailure(_ => 0);

            //Assert
            res.Should().Be(4);
        }

        [Test]
        public void Condition_WaitConditionalException_Rise()
        {
            //Arrange
            var timeLimit = TimeSpan.FromSeconds(5);

            static async Task<bool> Expected()
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                return 3 > 4;
            }

            //Act
            Func<Task> act = async () => await WaitFor.Condition(Expected, DefaultTimeOutMessage, timeLimit);

            //Assert
            act.Should().Throw<WaitConditionalException>().And.Message.Should()
                .Contain(DefaultTimeOutMessage).And.NotContain("Expected()");
        }

        [Test]
        public void Catch_NotIgnored_Exception()
        {
            //Arrange
            static Task<bool> Expected()
            {
                throw new ArgumentException("ArgumentException");
            }

            //Act
            Func<Task> act = async () => await WaitFor.Condition(Expected,
                buildWaiter => buildWaiter.SetNotIgnoredExceptionType<ArgumentException>().Build(),
                DefaultTimeOutMessage);

            //Assert
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Catch_Ignored_Exception()
        {
            //Arrange
            static Task<bool> Expected()
            {
                throw new ArgumentException("ArgumentException");
            }

            //Act
            Func<Task> act = async () => await WaitFor.Condition(Expected, DefaultTimeOutMessage);

            //Assert
            act.Should().Throw<WaitConditionalException>().And.Message.Should().Contain(nameof(ArgumentException)).And
                .Contain(DefaultTimeOutMessage);
        }

        [Test]
        public async Task For_Success()
        {
            //Arrange
            static async Task<int> Expected()
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                return 3;
            }

            //Act
            var res = await WaitFor.ForAsync(Expected).Become(a => a == 3).OnFailure(_ => 0);

            //Assert
            res.Should().Be(3);
        }

        [Test]
        public async Task For_Failure()
        {
            //Arrange
            static async Task<int> Expected()
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                return 3;
            }

            //Act
            var res = await WaitFor.ForAsync(Expected).Become(a => a == 4).OnFailure(_ => 0);

            //Assert
            res.Should().Be(0);
        }

        [Test]
        public async Task For_Failure_OnFailureWhenNotExpectedValue()
        {
            //Arrange
            static Task<int> Expected()
            {
                return Task.FromResult(3);
            }

            //Act
            var res = await WaitFor.ForAsync(Expected)
                .Become(a => a == 4)
                .WhenNotExpectedValue(x => x.ActuallyValue)
                .OnFailure(_ => 0);

            //Assert
            res.Should().Be(3);
        }

        [Test]
        public async Task For_Failure_DoWhenNotExpectedValue()
        {
            //Arrange
            static Task<int> Expected()
            {
                return Task.FromResult(3);
            }

            var callbackExpected = 0;

            //Act
            await WaitFor.ForAsync(Expected)
                .Become(a => a == 4)
                .DoWhenNotExpectedValue(x => callbackExpected = x.ActuallyValue)
                .OnFailure(_ => 0);

            //Assert
            callbackExpected.Should().Be(3);
        }

        [Test]
        public async Task For_Success_Return_New_Type()
        {
            //Arrange
            static Task<int> Expected()
            {
                return Task.FromResult(3);
            }

            //Act
            var res = await WaitFor.ForAsync(Expected).Become(a => a == 3)
                .OnSuccess(x => x.ToString())
                .OnFailureThrowException();

            //Assert
            res.Should().BeEquivalentTo(3.ToString());
        }

        [Test]
        public async Task For_Failure_With_Condition()
        {
            //Arrange
            static Task<int> Expected()
            {
                return Task.FromResult(3);
            }

            //Act
            var res = await WaitFor.ForAsync(Expected)
                .Become(a => a == 5)
                .OnFailure(_ => 1, fail => fail is NotExpectedValue<int>)
                .OnFailure(_ => -2);

            //Assert
            res.Should().Be(1);
        }

        [Test]
        public async Task For_Failure_Return_ActuallyValue()
        {
            //Arrange
            static Task<int> Expected()
            {
                return Task.FromResult(3);
            }

            //Act
            var res = await WaitFor.ForAsync(Expected,
                    w => w
                        .SetTimeBetweenStep(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))).Build())
                .Become(a => a == 5);

            var notExpectedResult = Assertion.AssertFailure<int, NotExpectedValue<int>>(res);

            //Assert
            notExpectedResult.ActuallyValue.Should().Be(3);
        }

        [Test]
        public async Task For_Success_Return_ActuallyValue()
        {
            //Arrange
            static Task<int> Expected()
            {
                return Task.FromResult(3);
            }

            //Act
            var res = await WaitFor.ForAsync(Expected,
                    w => w.SetLogarithmStep(Time.FromSeconds).Build())
                .Become(a => a == 3).OnFailureThrowException();

            //Assert
            res.Should().Be(3);
        }

        [Test]
        public async Task For_Success_For_Classes()
        {
            //Arrange
            static async Task<SomeClass> Expected()
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                return new SomeClass
                {
                    SomeNumber = 3,
                    Child = new OtherClass
                    {
                        SomeNumber = 1
                    }
                };
            }

            //Act
            var res = await WaitFor.ForAsync(Expected)
                .Become(a => a.Child.SomeNumber == 1 && a.SomeNumber == 3)
                .OnFailureThrowException();

            //Assert
            res.SomeNumber.Should().Be(3);
            res.Child.SomeNumber.Should().Be(1);
        }

        [Test]
        public void For_Rise_Exception_For_FailureResult()
        {
            //Arrange
            static async Task<SomeClass> Expected()
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                return new SomeClass
                {
                    SomeNumber = 3,
                    Child = new OtherClass
                    {
                        SomeNumber = 5
                    }
                };
            }

            //Act
            Func<Task<SomeClass>> act = async () => await WaitFor.ForAsync(Expected)
                .Become(a => a.Child.SomeNumber == 1 && a.SomeNumber == 3)
                .OnFailureThrowException();

            //Assert
            act.Should().Throw<WaitConditionalException>().And.Message
                .Contains("Expected: (a) => a.Child.SomeNumber == 1 && a.SomeNumber == 3");
        }

        [Test]
        public async Task Exceptions_Should_Has_Json_View()
        {
            //Arrange
            const int exceptionRiseCount = 4;
            var interaction = 0;
            const int timeWaitInSec = 10;

            //Act
            async Task<string> Sut()
            {
                await Task.Delay(TimeSpan.FromMilliseconds(10));
                if (interaction > exceptionRiseCount) throw new Exception("Exception: 2");
                interaction++;
                throw new ArgumentException("ArgumentException: 1");
            }

            var result = await WaitFor.ForAsync(Sut, b => b.SetMaxWaitTime(TimeSpan.FromSeconds(timeWaitInSec)).Build())
                .Become(x => !string.IsNullOrEmpty(x));

            //Assert
            var failureResult = Assertion.AssertFailure<string, ExceptionsHappened>(result);

            Assertion.For(() =>
            {
                var actualErrorMsg = failureResult.ToString();
                ValidateJson(actualErrorMsg).Should().BeTrue($"Expected Json representation {actualErrorMsg}");
                failureResult.Exceptions.Should().HaveCount(2);
                var firstException = failureResult.Exceptions.First();
                firstException.Counter.Should().Be(exceptionRiseCount);
                var exceptionContent = firstException.Content;
                exceptionContent.Exception.Should().BeOfType<ArgumentException>();
                exceptionContent.CallStack.Should().NotBeNullOrEmpty();
            });
        }

        [Test]
        public async Task For_OnFailureWhenWasExceptions()
        {
            //Arrange
            const int exceptionRiseCount = 4;
            var interaction = 0;
            const int timeWaitInSec = 10;

            //Act
            async Task<string> Sut()
            {
                await Task.Delay(TimeSpan.FromMilliseconds(10));
                if (interaction > exceptionRiseCount) throw new Exception("Exception: 2");
                interaction++;
                throw new ArgumentException("ArgumentException: 1");
            }

            var failureResult = await WaitFor
                .ForAsync(Sut, b => b.SetMaxWaitTime(TimeSpan.FromSeconds(timeWaitInSec)).Build())
                .Become(x => x != null).WhenWasExceptions(x => x.ToString()).OnFailure(_ => "Finish");

            //Assert
            ValidateJson(failureResult).Should().BeTrue($"Expected Json representation {failureResult}");
        }

        [Test]
        public async Task NotExpectedValue_Should_Contain_ActuallyValue()
        {
            //Arrange
            const int timeWaitInSec = 1;
            var timeOutMsg = $"Fail {nameof(NotExpectedValue_Should_Contain_ActuallyValue)}";
            Expression<Func<int, bool>> predicate = x => x == 4;

            //Act
            static Task<int> Sut()
            {
                return Task.FromResult(3);
            }

            var result = await WaitFor.ForAsync(Sut,
                    b => b.SetMaxWaitTime(TimeSpan.FromSeconds(timeWaitInSec)).SetTimeOutMessage(timeOutMsg).Build())
                .Become(predicate);

            //Assert
            var failureResult = Assertion.AssertFailure<int, NotExpectedValue<int>>(result);

            failureResult.ActuallyValue.Should().Be(3);
        }

        [TestCase(30)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(60)]
        public async Task MaxTime_Should_be_Close_To_Actual(int sec)
        {
            var maxTime = TimeSpan.FromSeconds(sec);
            var stopwatch = Stopwatch.StartNew();

            static async Task<int> Func()
            {
                await Task.Delay(TimeSpan.FromMilliseconds(10));
                return 5;
            }

            await WaitFor.ForAsync(Func, b => b.SetMaxWaitTime(maxTime).Build()).Become(x => x == 6);
            stopwatch.Elapsed.Should().BeGreaterOrEqualTo(maxTime);
        }


        private class SomeClass
        {
            public int SomeNumber { get; init; }
            public OtherClass Child { get; init; }
        }

        private class OtherClass
        {
            public int SomeNumber { get; init; }
        }

        public static bool ValidateJson(string s)
        {
            try
            {
                JsonDocument.Parse(s);
                return true;
            }
            catch
            {
                // ignored
            }

            return false;
        }
    }
}