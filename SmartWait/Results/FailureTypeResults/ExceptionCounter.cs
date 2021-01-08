namespace SmartWait.Results.FailureTypeResults
{
    public class ExceptionCounter
    {
        public int Counter { get; set; } = 0;
        public ExceptionContent Content { get; init; }
        public ExceptionCounter(ExceptionContent content) => Content = content;
    }
}