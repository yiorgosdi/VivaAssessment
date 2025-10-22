namespace VivaAssessment.Infrastructure.Exceptions;

public class ExternalServiceException : Exception
{
    public int? StatusCode { get; }
    public string ServiceName { get; }

    public ExternalServiceException(string serviceName, string message, int? statusCode = null, Exception? inner = null)
        : base(message, inner)
    {
        ServiceName = serviceName;
        StatusCode = statusCode;
    }
}