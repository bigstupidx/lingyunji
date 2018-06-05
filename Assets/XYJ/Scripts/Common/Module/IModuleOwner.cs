using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xys
{
    public interface IModuleOwner
    {
        IModule GetModule(NetProto.ModuleType type);
        T GetModule<T>() where T : IModule;
    }
}
