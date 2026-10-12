namespace CutCal.Model.Exceptions;

/// <summary>A service the request depends on (for example the message broker) cannot be reached right now.</summary>
public class ServiceUnavailableException : Exception
{
    public ServiceUnavailableException(string message) : base(message)
    {
    }
}
