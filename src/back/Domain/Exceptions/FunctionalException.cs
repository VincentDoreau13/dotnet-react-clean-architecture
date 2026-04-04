namespace ShopApi.Domain.Exceptions;

public class FunctionalException : Exception
{
    public FunctionalException()
    {
    }

    public FunctionalException(string code, string message) : base(message)
    {
        Code = code;
    }

    public FunctionalException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public FunctionalException(string message) : base(message)
    {
    }

    public string? Code { get; set; }
}
