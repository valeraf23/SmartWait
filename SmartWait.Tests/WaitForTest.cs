using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SmartWait.ExceptionHandler;

namespace SmartWait.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    internal class WaitForTest
    {
        [Test]
        public void WaitTrue()
        {
            //Arrange
            bool Expected() => 3 > 1;
            var actual = false;
            //Act
            Task.Run(() => { actual = Do(Expected); });
            //Assert
            WaitFor.Condition(() => actual, "Fail");
        }

        [Test]
        public void WaitTrueWithRetryCount()
        {
            //Arrange
            bool Expected() => 3 > 1;
            var actual = false;
            //Act
            Task.Run(() => { actual = Do(Expected); });
            //Assert
            WaitFor.Condition(() => actual, "Fail", 100);
        }

        [Test]
        public void WaitException()
        {
            //Arrange
            bool Expected() => 3 > 4;
            var actual = false;
            //Act
            Task.Run(() => { actual = Do(Expected, TimeSpan.FromSeconds(2)); });
            //Assert
            Assert.That(() => WaitFor.Condition(() => actual, "Fail", TimeSpan.FromSeconds(5)),
                Throws.TypeOf<WaitConditionalException>());
        }

        [Test]
        public void WaitProperlyValueForValueType()
        {
            //Arrange
            int Expected() => 3;
            var actual = 0;
            //Act
            Task.Run(() => { actual = Do(Expected, TimeSpan.FromSeconds(2)); });
            //Assert
            var res = WaitFor.For(() => actual).Become(a => a == 3, "Fail");
            Assert.AreEqual(3, res);
        }

        [Test]
        public void IfNotReturnActual()
        {
            //Arrange
            int Expected() => 3;
            var actual = 0;
            //Act
            Task.Run(() => { actual = Do(Expected, TimeSpan.FromSeconds(2)); });
            //Assert
            var res = WaitFor.For(() => actual, w => w.SetExceptionHandling(ExceptionHandling.Ignore).Build())
                .Become(a => a == 5, "Fail");
            Assert.AreEqual(3, res);
        }

        [Test]
        public void WaitProperlyValueForRefType()
        {
            //Arrange
            SomeClass Expected() => new SomeClass
            {
                A = 3,
                SomeClassA = new SomeClassA
                {
                    A1 = 1
                }
            };

            SomeClass actual = null;

            //Act
            Task.Run(() => { actual = Do(Expected, TimeSpan.FromSeconds(2)); });
            //Assert
            var res = WaitFor.For(() => actual).Become(a => a.SomeClassA.A1 == 1 && a.A == 3, "Fail");
            Assert.AreEqual(3, res.A);
        }

        [Test]
        public void WaitExceptionValueForRefType()
        {
            //Arrange
            SomeClass Expected() => new SomeClass
            {
                A = 3,
                SomeClassA = new SomeClassA
                {
                    A1 = 5
                }
            };

            SomeClass actual = null;
            //Act
            Task.Run(() => { actual = Do(Expected, TimeSpan.FromSeconds(2)); });
            //Assert
            Assert.That(() => WaitFor.For(() => actual).Become(a => a.SomeClassA.A1 == 1 && a.A == 3, "Fail"),
                Throws.TypeOf<WaitConditionalException>());
        }

        [Test]
        public void CatchNotIgnoredException()
        {
            //Arrange
            bool Expected() => throw new ArgumentException("ArgumentException");

            //Assert
            Assert.That(() => WaitFor.Condition<ArgumentException>(Expected, ""),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void CatchIgnoredException()
        {
            //Arrange
            bool Expected() => throw new ArgumentException("ArgumentException");

            Assert.That(() => WaitFor.Condition(Expected, ""),
                Throws.TypeOf<WaitConditionalException>());
        }

        [Test]
        public void WaitExceptionHandling()
        {
            //Arrange
            var i = 0;
            const int timeWaitInSec = 5;

            //Act
            string Expected()
            {
                if (i == 0)
                {
                    i++;
                    throw new Exception("Exception: 1");
                }

                if (i != 1) return null;
                i++;
                throw new Exception("Exception: 2");
            }

            //Assert
            var actualErrorMsg = string.Empty;
            try
            {
                WaitFor.Condition(() => Expected() != null, "Fail", TimeSpan.FromSeconds(timeWaitInSec),
                    ExceptionHandling.Collect);
            }
            catch (Exception e)
            {
                actualErrorMsg = e.Message;
            }

            var expectedErrorMsg =
                $"Timeout after {timeWaitInSec} seconds: Fail. Exceptions During Wait: ( Exception: 1\r\nException: 2\r\n )";
            Assert.AreEqual(actualErrorMsg, expectedErrorMsg);
        }

        #region Methods and Test DTO

        private static T Do<T>(Func<T> act, TimeSpan time)
        {
            Thread.Sleep(time);
            Console.WriteLine(act());
            return act();
        }

        private static T Do<T>(Func<T> act)
        {
            return Do(act, TimeSpan.FromSeconds(3));
        }

        private class SomeClass
        {
            public int A { get; set; }
            public SomeClassA SomeClassA { get; set; }
        }

        private class SomeClassA
        {
            public int A1 { get; set; }
        }

        #endregion
    }
}