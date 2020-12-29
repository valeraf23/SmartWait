namespace SmartWait.Results
{
    public class Failure<TSuccess, TFailure> : Result<TSuccess, TFailure>
    {
        private readonly TFailure _value;

        public Failure(TFailure value) => _value = value;

        public static implicit operator TFailure(Failure<TSuccess, TFailure> obj) => obj._value;
    }
}