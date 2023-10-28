namespace Food_Delivery.Common;

public class AuthException : Exception
{
    public AuthException(string? message)
    {
        Message = message;
    }

    public string? Message { get; }
}