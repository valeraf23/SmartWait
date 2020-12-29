namespace SmartWait.Results
{
    public class Success<TSuccess, TFailure> : Result<TSuccess, TFailure>
    {
        private readonly TSuccess _value;

        public Success(TSuccess value) => _value = value;

        public static implicit operator TSuccess(Success<TSuccess, TFailure> obj) => obj._value;
    }
}