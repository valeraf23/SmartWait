using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SmartWait.Results.FailureTypeResults
{
    public class ExceptionResult
    {
        public ExceptionResult(List<ExceptionCounter> exception, string exceptionMsg)
        {
            Exceptions = exception;
            ExceptionMsg = exceptionMsg;
        }

        [JsonPropertyName("Exception Message")] public string ExceptionMsg { get; }

        [JsonPropertyName("Exceptions During Wait")]
        public List<ExceptionCounter> Exceptions { get; }
    }
}