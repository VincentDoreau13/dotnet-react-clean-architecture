namespace ShopApi.Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException()
    {
    }

    public NotFoundException(string code, string message) : base(message)
    {
        Code = code;
    }

    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
        Code = string.Empty;
    }

    public NotFoundException(string code, string name, object key) : base($"Entity '{name}' ({key}) was not found.")
    {
        Code = code;
    }

    public NotFoundException(string message) : base(message)
    {
        Code = string.Empty;
    }

    public string? Code { get; init; }
}
