using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace xys
{
    using NetProto;

    public class PackageModule : HotModule
    {
        public PackageModule() : base("xys.hot.HotPackageModule")
        {

        }

        public bool UseItemById(int index, int count)
        {
            return (bool)refType.InvokeMethodReturn("UserItemById", index, count);
        }

        public int GetItemCount(int id)
        {
            return (int)refType.InvokeMethodReturn("GetItemCount", id);
        }

        //public C2APackageRequest request { get { return refType.GetProperty<C2APackageRequest>("request"); } }

        // 包裹类
        //public PackageMgr packageMgr { get { return refType.GetProperty<PackageMgr>("packageMgr"); } } 
    }
}
