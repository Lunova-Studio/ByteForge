using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteForge.Core.Exceptions
{
    public class ReturnValueException : Exception
    {
        public object Value { get; }

        public ReturnValueException(object value)
        {
            Value = value;
        }
    }
}
