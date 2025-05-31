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

public class RewriteFactory : IPatchFactory
{
    public IEnumerable<IPatch> SearchAll(Type mixin)
    {
        return mixin.GetMethods().Where(x => x.GetCustomAttribute<RewriteAttribute>() != null).Select(x => new RewritePatch(x));
    }
}
