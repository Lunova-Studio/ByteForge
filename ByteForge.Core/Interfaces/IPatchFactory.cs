using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteForge.Core.Interfaces;

public interface IPatchFactory
{
    public IEnumerable<IPatch> SearchAll(Type mixin);
}
