#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XTools;

// 加载基础资源
namespace PackTool
{
    public class AudioClipLoad : AssetLoad<AudioClip>
    {
        static public void Load(string name, ResourcesEnd<AudioClip> fun, object funp)
        {
            AudioClipLoad.LoadImp(name, fun, funp, Create);
        }

        static public AudioClipLoad.Data LoadAsync(string name, ResourcesEnd<AudioClip> fun, object funp)
        {
            AudioClipLoad.Data td = AudioClipLoad.LoadImp(name, fun, funp, Create);
            td.AddRef();
            return td; 
        }

        static public void LoadEx(string name, ResourcesEnd<AudioClip> fun, object p)
        {
#if USE_RESOURCESEXPORT
            // 使用打包加载模式
            name = ResourcesGroup.GetFullPath("AudioClipExport", name);
            AudioClipLoad.LoadImp(name, fun, p, Create).load.AddRef();
#else
            AudioClipLoad load = new AudioClipLoad();
            load.Reset(name);
            /*AudioClipLoad.Data loaddata =*/ load.Add(fun, p);

#if USE_RESOURCES
            int pos = name.LastIndexOf('.');
            if (pos != -1)
                name = name.Substring(0, pos);
            load.asset = AssetResoruce.Load<AudioClip>(name.Substring(0));
#elif USER_ALLRESOURCES
            load.asset = (AudioClip)AllResources.Instance.GetObject(ResourcesGroup.GetFullPath("AudioClipExport", name));
#else
#if UNITY_EDITOR
            string path = "Assets/" + ResourcesGroup.GetFullPath("AudioClipExport", name);
            load.asset = (AudioClip)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(AudioClip));
#endif
#endif
            if (load.asset == null)
            {
                Debuger.LogError("name:" + name + " Resources.LoadAssetAtPath Not Find!!");
            }

            load.isDone = true;
            load.NextFrame();
#endif
        }

        static AudioClipLoad Create(string name)
        {
            return CreateAsset<AudioClipLoad>(name);
        }

        protected override void LoadAsset(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                NextFrame();
                return;
            }

            if (name[0] != ':')
            {
                AssetBundleLoad.Load(name, LoadAssetEnd);
            }
            else
            {
                asset = Resources.Load(name.Substring(1, name.LastIndexOf('.') - 1), typeof(AudioClip)) as AudioClip;
                if (asset == null)
                {
                    Debuger.ErrorLog("AudioClipLoad url:{0} Resources.Load null!", url);
                }

                TimerMgrObj.Instance.addFrameLateUpdate((object p) => { OnEnd(); return false; }, null);
            }
        }

        AssetBundle m_AssetBundle;

        void LoadAssetEnd(AssetBundle assetBundle)
        {
            isDone = true;
            if (assetBundle == null)
            {
                Debuger.LogError(string.Format("LoadAssetEnd: {0} assetBundle == null!", url));
            }
            else
            {
                m_AssetBundle = assetBundle;
                asset = assetBundle.mainAsset as AudioClip;
            }

            OnEnd();
        }


        protected override void DestroyAssetAsset()
        {
            if (m_AssetBundle != null)
            {
                m_AssetBundle.Unload(true);
                m_AssetBundle = null;
            }

            if (asset != null)
            {
                Object.DestroyImmediate(asset, true);
            }
        }

        // 回收自身
        protected override void FreeSelf()
        {
            FreeAsset<AudioClipLoad>(this);
        }
    }
}
#endif