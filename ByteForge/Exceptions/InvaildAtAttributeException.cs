using ByteForge.Attributes;
using ByteForge.Interfaces;

namespace ByteForge.Exceptions;

public class InvaildAtAttributeException : Exception
{
    public IPatch? Patch { get; }
    public AtAttribute? AtAttribute { get; }


    public InvaildAtAttributeException(IPatch patch, AtAttribute? atAttribute)
    {
        Patch = patch;
        AtAttribute = atAttribute;
    }
}
