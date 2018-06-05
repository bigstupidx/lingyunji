#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XTools;

namespace PackTool
{
    public partial class PrefabLoad : AssetLoad<GameObject>
    {
        void OnDepLoadEnd(ComponentSave cos)
        {
            PrefabBeh.isAutoAdd = true;
            PrefabBeh pb = asset.AddComponent<PrefabBeh>();
            PrefabBeh.isAutoAdd = false;
            pb.url = url;

#if UNITY_EDITOR && COM_DEBUG
            if (cos != null)
            {
                DependenceList = cos.GetDepList();
            }
            pb.Get().load_num++;
#endif
            if (com_save != null)
            {
                //com_save.GetDepList(GetDependences());
                com_save.Release();
                Buff<ComponentSave>.Free(com_save);
                com_save = null;
            }
            AddDepRef(); // 增加依赖的计数

            isDone = true;
            --CurrentLoading;

            OnEnd();
            OnLoadEnd();
        }

        bool AutoDestroy(object p)
        {
            if (destroyType == DestroyType.Auto)
            {
                if (DestroyImp())
                {
                    destroyType = DestroyType.Destroy;
                    Debuger.DebugLog("OnAutoDestroy:{0}", url);
                    PrefabBeh.Add(this);
                    return false;
                }
                else
                {
                    Debuger.DebugLog("AutoDestro:{0}y unsuc!", url);
                }
            }

            return true;
        }

        public void OnAutoDestroy()
        {
            if (destroyType == DestroyType.Auto)
            {
                TimerMgrObj.Instance.addFrameLateUpdate(AutoDestroy, null);
            }
        }

        void OnLoadEnd()
        {
            //OnAutoDestroy();
        }

        void OnLoadAssetEnd(AssetBundle assetBundle)
        {
            if (assetBundle == null)
            {
                Debuger.LogError("LoadPrefabEnd url:" + url);
                isDone = true;
                --CurrentLoading;
                OnEnd();
                OnLoadEnd();
            }
            else
            {
                try
                {
                    asset = (GameObject)assetBundle.mainAsset;
                    assetBundle.Unload(false);
                }
                catch (System.Exception ex)
                {
                    Debuger.LogException(ex);
                }

                if (asset == null)
                {
#if UNITY_EDITOR && COM_DEBUG
                    DependenceList = new Object[0];
#endif
                    isDone = true;
                    --CurrentLoading;
                    OnEnd();
                    OnLoadEnd();
                }
                else
                {
                    ResourcesRecords rrs = asset.GetComponent<ResourcesRecords>();
                    if (rrs != null)
                    {
                        Component[] components = rrs.components;
                        Object.DestroyImmediate(rrs, true);

                        com_save = Buff<ComponentSave>.Get();
                        Stream stream = ResourcesPack.FindBaseStream(string.Format("{0}{1}", url, Suffix.PrefabDataByte));
                        if (
#if ASSET_DEBUG
                            (int)ComponentSave.LoadResources_timetrack.Execution(url, () =>
                            {
                                return 
#endif
                            com_save.LoadResources(stream, components, null, OnDepLoadEnd, 
                            (string dep) => 
                            {
                                GetDependences().Add(dep);
                                //Logger.LogDebug("url:{0} dep:{1}", url, dep);
                            })
#if ASSET_DEBUG
                            ;})
#endif
                            != 0)
                            return;
                    }

                    OnDepLoadEnd(null);
                }
            }
        }
    }
}
#endif