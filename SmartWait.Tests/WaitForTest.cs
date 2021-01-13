using FluentAssertions;
using NUnit.Framework;
using SmartWait.Core;
using SmartWait.Results;
using SmartWait.Results.Extension;
using SmartWait.Results.FailureTypeResults;
using SmartWait.WaitSteps;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWait.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    internal class WaitForTest
    {
        private const string DefaultTimeOutMessage = "Fail";

        [Test]
        public void Condition_Success()
        {
            //Arrange
            static bool Expected() => 3 > 1;

            var actual = false;

            //Act
            Task.Run(() => { actual = Do(Expected); });

            //Assert
            WaitFor.Condition(() => actual, DefaultTimeOutMessage);
            Assert.Pass();
        }

        [Test]
        public void Condition_WaitConditionalException_Rise()
        {
            //Arrange
            var timeLimit = TimeSpan.FromSeconds(5);

            static bool Expected() => 3 > 4;

            var actual = false;

            //Act
            Task.Run(() => { actual = Do(Expected, TimeSpan.FromSeconds(2)); });
            Action act = () => WaitFor.Condition(() => actual, DefaultTimeOutMessage, timeLimit);

            //Assert
            act.Should().Throw<WaitConditionalException>().And.Message.Should()
                .Contain(DefaultTimeOutMessage).And
                .Contain("Expected: x => (False == x), but was True");
        }

        [Test]
        public void Catch_NotIgnored_Exception()
        {
            //Arrange
            static bool Expected() => throw new ArgumentException("ArgumentException");

            //Act
            Action act = () => WaitFor.Condition(Expected,
                buildWaiter => buildWaiter.SetNotIgnoredExceptionType<ArgumentException>().Build(),
                DefaultTimeOutMessage);

            //Assert
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Catch_Ignored_Exception()
        {
            //Arrange
            static bool Expected() => throw new ArgumentException("ArgumentException");

            Action act = () => WaitFor.Condition(Expected, DefaultTimeOutMessage);

            //Assert
            act.Should().Throw<WaitConditionalException>().And.Message.Should().Contain(nameof(ArgumentException)).And
                .Contain(DefaultTimeOutMessage);
        }

        [Test]
        public void For_Success()
        {
            //Arrange
            static int Expected() => 3;

            var actual = 0;

            //Act
            Task.Run(() => { actual = Do(Expected, TimeSpan.FromSeconds(2)); });

            //Assert
            var res = WaitFor.For(() => actual).Become(a => a == 3).OnFailure(_ => 0);
            res.Should().Be(3);
        }

        [Test]
        public void For_Failure()
        {
            //Arrange
            static int Expected() => 3;

            var actual = 0;

            //Act
            Task.Run(() => { actual = Do(Expected, TimeSpan.FromSeconds(1)); });

            //Assert
            var res = WaitFor.For(() => actual)
                .Become(a => a == 4)
                .OnFailure(_ => 0);

            res.Should().Be(0);
        }

        [Test]
        public void For_Failure_OnFailureWhenNotExpectedValue()
        {
            //Arrange
            static int Expected() => 3;

            var actual = 0;

            //Act
            Task.Run(() => { actual = Do(Expected, TimeSpan.FromSeconds(1)); });

            //Assert
            var res = WaitFor.For(() => actual)
                .Become(a => a == 4)
                .WhenNotExpectedValue(x => x.ActuallyValue)
                .OnFailure(_ => 0);

            res.Should().Be(3);
        }

        [Test]
        public void For_Failure_DoWhenNotExpectedValue()
        {
            //Arrange
            static int Expected() => 3;

            var actual = 0;
            var callbackExpected = 0;
            //Act
            Task.Run(() => { actual = Do(Expected, TimeSpan.FromSeconds(1)); });

            //Assert
            WaitFor.For(() => actual)
                .Become(a => a == 4)
                .DoNotExpectedValue(x => callbackExpected = x.ActuallyValue)
                .OnFailure(_ => 0);

            callbackExpected.Should().Be(3);
        }

        [Test]
        public void For_Success_Return_New_Type()
        {
            //Arrange
            static int Expected() => 3;

            var actual = 0;

            //Act
            Task.Run(() => { actual = Do(Expected, TimeSpan.FromSeconds(2)); });
            var res = WaitFor.For(() => actual).Become(a => a == 3)
                .OnSuccess(x => x.ToString())
                .OnFailureThrowException();

            //Assert
            res.Should().BeEquivalentTo(3.ToString());
        }

        [Test]
        public void For_Failure_With_Condition()
        {
            //Arrange
            static int Expected() => 3;

            var actual = 0;

            //Act
            Task.Run(() => { actual = Do(Expected, TimeSpan.FromSeconds(2)); });
            var res = WaitFor.For(() => actual)
                .Become(a => a == 5)
                .OnFailure(_ => 1, fail => fail is NotExpectedValue<int>)
                .OnFailure(_ => -2);

            //Assert
            res.Should().Be(1);
        }

        [Test]
        public void For_Failure_Return_ActuallyValue()
        {
            //Arrange
            static int Expected() => 3;

            var actual = 0;

            //Act
            Task.Run(() => { actual = Do(Expected, TimeSpan.FromSeconds(2)); });
            var res = WaitFor.For(() => actual,
                    w => w
                        .SetTimeBetweenStep(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))).Build())
                .Become(a => a == 5);
            var notExpectedResult = Assertion.AssertFailure<int, NotExpectedValue<int>>(res);

            //Assert
            notExpectedResult.ActuallyValue.Should().Be(3);
        }

        [Test]
        public void For_Success_Return_ActuallyValue()
        {
            //Arrange
            static int Expected() => 3;

            var actual = 0;

            //Act
            Task.Run(() => { actual = Do(Expected, TimeSpan.FromSeconds(2)); });
            var res = WaitFor.For(() => actual,
                    w => w.SetLogarithmStep(Time.FromSeconds).Build())
                .Become(a => a == 3).OnFailureThrowException();

            //Assert
            res.Should().Be(3);
        }

        [Test]
        public void For_Success_For_Classes()
        {
            //Arrange
            static SomeClass Expected() => new()
            {
                SomeNumber = 3,
                Child = new OtherClass
                {
                    SomeNumber = 1
                }
            };

            SomeClass actual = default;

            //Act
            Task.Run(() => { actual = Do(Expected, TimeSpan.FromSeconds(2)); });
            var res = WaitFor.For(() => actual).Become(a => a.Child.SomeNumber == 1 && a.SomeNumber == 3)
                .OnFailureThrowException();

            //Assert
            res.SomeNumber.Should().Be(3);
            res.Child.SomeNumber.Should().Be(1);
        }

        [Test]
        public void For_Rise_Exception_For_FailureResult()
        {
            //Arrange
            static SomeClass Expected() => new()
            {
                SomeNumber = 3,
                Child = new OtherClass
                {
                    SomeNumber = 5
                }
            };

            SomeClass actual = default;

            //Act
            Task.Run(() => { actual = Do(Expected, TimeSpan.FromSeconds(2)); });

            Result<SomeClass, FailureResult> Func() => WaitFor.For(() => actual)
                    .Become(a => a.Child.SomeNumber == 1 && a.SomeNumber == 3)
                    .OnFailureThrowException();

            //Assert
            Assert.That(Func, Throws.TypeOf<WaitConditionalException>());
        }

        [Test]
        public void Exceptions_Should_Has_Json_View()
        {
            //Arrange
            const int exceptionRiseCount = 4;
            var interaction = 0;
            const int timeWaitInSec = 10;

            //Act
            string Sut()
            {
                if (interaction > exceptionRiseCount) throw new Exception("Exception: 2");
                interaction++;
                throw new ArgumentException("ArgumentException: 1");
            }

            var result = WaitFor.For(Sut, b => b.SetMaxWaitTime(TimeSpan.FromSeconds(timeWaitInSec)).Build())
                .Become(x => x != null);

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
        public void For_OnFailureWhenWasExceptions()
        {
            //Arrange
            const int exceptionRiseCount = 4;
            var interaction = 0;
            const int timeWaitInSec = 10;

            //Act
            string Sut()
            {
                if (interaction > exceptionRiseCount) throw new Exception("Exception: 2");
                interaction++;
                throw new ArgumentException("ArgumentException: 1");
            }

            var failureResult = WaitFor.For(Sut, b => b.SetMaxWaitTime(TimeSpan.FromSeconds(timeWaitInSec)).Build())
                .Become(x => x != null).WhenWasExceptions(x => x.ToString()).OnFailure(_ => "Finish");

            //Assert
            var actualErrorMsg = failureResult;
            ValidateJson(actualErrorMsg).Should().BeTrue($"Expected Json representation {actualErrorMsg}");
        }

        [Test]
        public void NotExpectedValue_Should_Contain_ActuallyValue()
        {
            //Arrange
            const int timeWaitInSec = 1;
            const int act = 3;
            var timeOutMsg = $"Fail {nameof(NotExpectedValue_Should_Contain_ActuallyValue)}";
            Expression<Func<int, bool>> predicate = x => x == 4;

            //Act
            static int Sut() => act;

            var result = WaitFor.For(Sut,
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
        public void MaxTime_Should_be_Close_To_Actual(int sec)
        {
            var maxTime = TimeSpan.FromSeconds(sec);
            var stopwatch = Stopwatch.StartNew();
            WaitFor.For(() => 5, b => b.SetMaxWaitTime(maxTime).Build()).Become(x => x == 6);
            stopwatch.Elapsed.Should().BeGreaterOrEqualTo(maxTime);
        }

        private static T Do<T>(Func<T> act, TimeSpan time)
        {
            Thread.Sleep(time);
            Console.WriteLine(act());
            return act();
        }

        private static T Do<T>(Func<T> act) => Do(act, TimeSpan.FromSeconds(3));

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