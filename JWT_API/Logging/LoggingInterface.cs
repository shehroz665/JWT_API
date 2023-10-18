namespace JWT_API.Logging
{
    public interface LoggingInterface
    {
        public string Success(string message, int statuscode, object? data);
        public string Failure(string message, int statuscode, object? data);
    }
}
