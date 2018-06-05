using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys
{
    public interface IModule
    {
        IModuleOwner owner { get; }

        string moduleType { get; } // 模块类型
        int id { get; } // 模块的ID

        void SetModuleType(string type, int id);

        void Awake(IModuleOwner owner);

        void Deserialize(wProtobuf.IReadStream output);

        // 模块重置
        void Release();
    }
}