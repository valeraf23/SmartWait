using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SmartWait.Results.FailureTypeResults
{
    public sealed class ExceptionsHappened : FailureResult
    {
        private readonly List<ExceptionCounter> _exceptions;

        public ExceptionsHappened(double totalSeconds, string timeoutMessage, List<Exception> exceptions) :
            base(totalSeconds, timeoutMessage) => _exceptions = ToExceptionCounter(exceptions);

        public IReadOnlyCollection<ExceptionCounter> Exceptions => _exceptions;

        private static List<ExceptionCounter> ToExceptionCounter(IEnumerable<Exception> exceptions)
        {
            List<ExceptionCounter> exceptionCounters = new List<ExceptionCounter>();
            foreach (var exception in exceptions)
            {
                ExceptionContent content = new ExceptionContent(exception);
                var exist = exceptionCounters.FirstOrDefault(x => x.Content == content);
                if (exist != null)
                    exist.Counter++;
                else
                    exceptionCounters.Add(new ExceptionCounter
                    {
                        Content = content
                    });
            }

            return exceptionCounters;
        }

        public override string ToString()
        {
            var exceptionMsg = base.ToString();
            return JsonSerializer.Serialize(new ExceptionResult(_exceptions, exceptionMsg),
                new JsonSerializerOptions { WriteIndented = true });
        }
    }
}