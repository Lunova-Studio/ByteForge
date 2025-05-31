namespace ByteForge.Exceptions;

public class ReturnValueException : Exception
{
    public object Value { get; }

    public ReturnValueException(object value)
    {
        Value = value;
    }
}
