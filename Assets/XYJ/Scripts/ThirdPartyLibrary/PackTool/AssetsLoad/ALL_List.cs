#if USE_RESOURCESEXPORT
using System.Collections.Generic;

namespace PackTool
{
    public partial class AllAsset
    {
        // 加载一份材质
        public static Dictionary<string, AssetLoadObject> AlreadLoadList = new Dictionary<string, AssetLoadObject>();

#if MEMORY_CHECK
        static HashSet<string> LastList = new HashSet<string>();

        public static void GetAllLoadRes(System.Text.StringBuilder sb)
        {
            Dictionary<System.Type, List<AssetLoadObject>> typesList = new Dictionary<System.Type, List<AssetLoadObject>>();
            foreach (KeyValuePair<string, AssetLoadObject> itor in AlreadLoadList)
            {
                if (itor.Value.GetAssetObj() != null)
                {
                    System.Type type = itor.Value.GetAssetObj().GetType();
                    List<AssetLoadObject> objs = null;
                    if (!typesList.TryGetValue(type, out objs))
                    {
                        objs = new List<AssetLoadObject>();
                        typesList.Add(type, objs);
                    }

                    objs.Add(itor.Value);
                }
            }

            sb.AppendLine(string.Format("AllAsset:{0}", AlreadLoadList.Count));
            foreach (KeyValuePair<System.Type, List<AssetLoadObject>> itor in typesList)
            {
                sb.AppendLine(string.Format("   {0} num:{1}", itor.Key.Name, itor.Value.Count));
                for (int i = 0; i < itor.Value.Count; ++i)
                    sb.AppendLine(string.Format("       url:{0}", itor.Value[i].url));
            }


            if (LastList.Count != 0)
            {
                XTools.Utility.SetAddOrSub(LastList, AlreadLoadList,
                    (string key, AssetLoadObject alo) =>
                    {
                        sb.AppendLine(string.Format("   type:{0} add:{1}", alo.GetAssetObj().GetType().Name, key));
                    },
                    (string key) =>
                    {
                        sb.AppendLine(string.Format("   sub:{0}", key));
                    });
            }

            foreach (KeyValuePair<string, AssetLoadObject> itor in AlreadLoadList)
                LastList.Add(itor.Key);
        }
#endif
    }
}

#endif