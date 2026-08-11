namespace RushTodo.Api.Helpers;

public class ConcurrencyException : Exception
{
    public ConcurrencyException() : base("This record has been changed since you opened it. Reload and try again.") { }
}


[Serializable]
public class NotFoundException : Exception
{
    public NotFoundException() { }
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
}


public class UniqueConstraintException : Exception
{
    public UniqueConstraintException(Exception innerException)
        : base("A record with the same unique value already exists.", innerException) { }
}


public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}
