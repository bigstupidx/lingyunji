#if USE_ABL
using UnityEngine;

namespace ABL
{
    class PrefabAsset : MonoBehaviour
    {
        static public bool isAutoAdd = false; // 是否主动添加
        static public int wait_total = 0; // 等待计数的个数

        public string url;

        IAsset asset;

        PrefabAsset()
        {
            if (isAutoAdd)
                return;

            ++wait_total;
            XTools.TimerMgrObj.Instance.addFrameLateUpdate(CheckURL, this);
        }

        static bool CheckURL(object p)
        {
            --wait_total;
            PrefabAsset pb = p as PrefabAsset;
            if (pb == null)
                return false;

            pb.AddRef();
            return false;
        }

        void AddRef()
        {
            asset = AssetsMgr.Get(url);
            if (asset != null)
                asset.AddRef();
            else
            {
                Debug.LogErrorFormat("url:{0} not find Prefab Asset!", url);
                Object.DestroyImmediate(this);
            }
        }

        private void OnDestroy()
        {
            if (asset != null)
                asset.SubRef();
        }
    }
}
#endif