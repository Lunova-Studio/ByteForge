using System;
using System.Collections.Generic;
using System.Text;

namespace ByteForge.Utils;

public static class IEnumableUtils
{
    public static IEnumerable<T> Append<T>(IEnumerable<T> source, T item)
    {
        foreach (var element in source)
        {
            yield return element;
        }

        yield return item;
    }
}
