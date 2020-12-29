namespace SmartWait.Results
{
    public class Result<TSuccess, TFailure>
    {
        public static implicit operator Result<TSuccess, TFailure>(TSuccess obj) => new Success<TSuccess, TFailure>(obj);

        public static implicit operator Result<TSuccess, TFailure>(TFailure obj) => new Failure<TSuccess, TFailure>(obj);
    }
}