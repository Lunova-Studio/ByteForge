using ByteForge.Core.Attributes;
using ByteForge.Core.Interfaces;
using ByteForge.Core.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ByteForge.Core.Factories;

public class PrefixFactory : IPatchFactory
{
    public IEnumerable<IPatch> SearchAll(Type mixin)
    {
        return mixin.GetMethods().Where(x => x.GetCustomAttribute<PrefixAttribute>() != null).Select(x => new PrefixPatch(x));
    }
}
