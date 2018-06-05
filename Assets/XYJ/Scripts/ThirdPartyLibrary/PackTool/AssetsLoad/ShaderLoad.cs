#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XTools;

// 加载一份材质
namespace PackTool
{
    public class ShaderLoad
    {
#if UNITY_EDITOR // 当前项目下所有的shader
        static Dictionary<string, Shader> AllShaderList = new Dictionary<string, Shader>();

        internal static void Init()
        {
            AllShaderList.Clear();
            //TimeCheck tc = new TimeCheck(true);
            string[] objs = UnityEditor.AssetDatabase.FindAssets("*.shader", new string[] { "Assets" });
            for (int i = 0; i < objs.Length; ++i)
            {
                Shader s = UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(objs[i]), typeof(Shader)) as Shader;
                try
                {
                    AllShaderList.Add(s.name, s);
                }
                catch(System.Exception ex)
                {
                    Debuger.LogException(ex);
                }
            }
        }

        static Shader FindEditorShader(string name)
        {
            Shader s = Shader.Find(name);
            if (s != null)
            {
                //string assetPath = UnityEditor.AssetDatabase.GetAssetPath(s);
                return s;
            }

            if (AllShaderList.Count == 0)
            {
                Init();
            }

            AllShaderList.TryGetValue(name, out s);
            return s;
        }
#endif

        static public void Load(string name, ResourcesEnd<Shader> fun, object funp)
        {
            TimerMgrObj.Instance.addFrameLateUpdate(
                (object p) => 
                {
                    Shader shader = Find(name);
                    if (fun != null)
                    {
                        try
                        {
                            fun(shader, funp);
                        }
                        catch(System.Exception ex)
                        {
                            Debuger.LogException(ex);
                        }
                    }

                    return false;
                }, null);
        }

        static public Shader Find(string name)
        {
            Shader asset = null;
            if (string.IsNullOrEmpty(name))
            {

            }
            else if (name[0] == ':')
            {
                asset = Resources.Load(name.Substring(1, name.LastIndexOf('.') - 1), typeof(Shader)) as Shader;
                if (asset == null)
                {
                    Debuger.ErrorLog("ShaderLoad:{0} not find!", name);
                }
            }
            else
            {
                asset = BuiltinResource.Instance.GetShader(name);
                if (asset == null)
                    asset = Shader.Find(name);

                if (asset == null)
                {
                    Debuger.ErrorLog("ShaderLoad:{0} not find!", name);
                }
            }

#if UNITY_EDITOR
            if (asset != null)
            {
                //string src = UnityEditor.AssetDatabase.GetAssetPath(asset);
                Shader s = FindEditorShader(asset.name);
                //string dst = UnityEditor.AssetDatabase.GetAssetPath(s);
                if (s != null)
                {
                    asset = s;
                }
            }
#endif
            return asset;
        }
    }
}
#endif