#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XTools;

// 加载基础资源
namespace PackTool
{
    public class TextureLoad : AssetLoad<Texture>
    {
        static public void Load(string name, ResourcesEnd<Texture> fun, object funp)
        {
            TextureLoad.LoadImp(name, fun, funp, Create);
        }

        static public TextureLoad.Data LoadAsync(string name, ResourcesEnd<Texture> fun, object funp)
        {
            TextureLoad.Data td = TextureLoad.LoadImp(name, fun, funp, Create);
            td.AddRef();
            return td; 
        }

        static public void LoadEx(string name, string resGroup, ResourcesEnd<Texture> fun, object p)
        {
#if USE_RESOURCESEXPORT
            // 使用打包加载模式
            name = ResourcesGroup.GetFullPath(resGroup, name);
            TextureLoad.LoadImp(name, fun, p, Create).load.AddRef();
#else
            TextureLoad load = new TextureLoad();
            load.Reset(name);
            /*TextureLoad.Data loaddata = */load.Add(fun, p);
#if USER_ALLRESOURCES
            load.asset = (Texture)AllResources.Instance.GetObject(ResourcesGroup.GetFullPath(resGroup, name));
#else
#if UNITY_EDITOR
            string path = "Assets/" + ResourcesGroup.GetFullPath(resGroup, name);
            load.asset = (Texture)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Texture));
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

        static TextureLoad Create(string name)
        {
            return CreateAsset<TextureLoad>(name);
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
                if (name.LastIndexOf('.') != -1)
                    AssetBundleLoad.Load(name, LoadAssetEnd);
                else
                {
                    AddRef(); // 内置资源不卸载
                    asset = BuiltinResource.Instance.GetTexture(name);
                    NextFrame();
                }
            }
            else
            {
                asset = Resources.Load(name.Substring(1, name.LastIndexOf('.') - 1), typeof(Texture)) as Texture;
                if (asset == null)
                {
                    Debuger.ErrorLog("TextureLoad url:{0} Resources.Load null!", url);
                }

                TimerMgrObj.Instance.addFrameLateUpdate((object p) => { OnEnd(); return false; }, null);
            }
        }

        void LoadAssetEnd(AssetBundle assetBundle)
        {
            isDone = true;
            if (assetBundle == null)
            {
                Debuger.LogError(string.Format("LoadAssetEnd: {0} assetBundle == null!", url));
            }
            else
            {
                asset = assetBundle.mainAsset as Texture;
                assetBundle.Unload(false);
            }

            OnEnd();
        }

        // 回收自身
        protected override void FreeSelf()
        {
            FreeAsset(this);
        }
    }
}
#endif