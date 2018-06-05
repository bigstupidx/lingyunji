#if UNITY_EDITOR && USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using XTools;

// 加载一份材质
namespace PackTool
{
    public partial class AssetsExport
    {
        public class ETCShader
        {
            internal static SortedDictionary<string, Shader> AllETCShaders = new SortedDictionary<string, Shader>();

            [MenuItem("Assets/PackTool/TestETCLoad")]
            internal static void Init()
            {
                AllETCShaders.Clear();
                TimeCheck tc = new TimeCheck(true);
                string[] objs = AssetDatabase.FindAssets("*.*", new string[] { "Assets/Shader/ETC" });
                for (int i = 0; i < objs.Length; ++i)
                {
                    Shader s = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(objs[i]), typeof(Shader)) as Shader;
                    if (s != null)
                    {
                        if (s.name.Contains("_ETCR"))
                            AllETCShaders.Add(s.name, s);
                    }
                }

                Debug.Log(string.Format("time:{0} objs:{1} shaders:{2}", tc.renew, objs.Length, AllETCShaders.Count));
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (KeyValuePair<string, Shader> itor in AllETCShaders)
                    sb.AppendLine(itor.Key);
                Debug.Log(sb.ToString());
            }
            
            public static Shader GetETC(string name)
            {
                if (AllETCShaders.Count == 0)
                    Init();

                Shader s = null;
                if (AllETCShaders.TryGetValue(name, out s))
                {
                    if (s == null)
                    {
                        Init();
                        return GetETC(name);
                    }

                    return s;
                }

                return null;
            }

            public class ETC
            {
                public string proterty;
                public string etc_proterty;
            }

            public static Shader ToETC(Shader shader)
            {
                string name = string.Format("{0}_ETCR", shader.name);
                Shader etcs = GetETC(name);
                if (etcs == null)
                    return null;

                return etcs;
            }

            public static Shader ToETC(Shader shader, ETCType type)
            {
                switch (type)
                {
                case ETCType.R: return GetETC(shader.name + "_ETCR");
                case ETCType.G: return GetETC(shader.name + "_ETCG");
                case ETCType.B: return GetETC(shader.name + "_ETCB");
                }

                return null;
            }

            public static bool CheckMaterialETC(Material mat)
            {
                if (mat == null || mat.shader == null)
                    return false;

                if (ToETC(mat.shader) != null)
                    return false;

                MaterialProperty[] props = MaterialEditor.GetMaterialProperties(new Object[1] { mat });
                foreach (MaterialProperty prop in props)
                {
                    if (prop.type == MaterialProperty.PropType.Texture)
                    {
                        Texture2D t = mat.GetTexture(prop.name) as Texture2D;
                        if (t == null || !TextureExport.isARGB(t.format))
                            continue; // 空纹理或非ARGB格式，不需要转换

                        return true;
                    }
                }

                return false;
            }

            // 是否需要转换为ETC材质
            public static bool HasETCMaterial(Material mat, out Material etcMat, out string name, out MaterialProperty[] props)
            {
                name = string.Empty;
                etcMat = null;
                props = null;
                if (mat.shader == null)
                    return false;

                Shader shader = ToETC(mat.shader);
                if (shader == null)
                    return false;

                etcMat = new Material(shader);
                props = MaterialEditor.GetMaterialProperties(new Object[1] { mat });
                int total = 0;
                foreach (MaterialProperty prop in props)
                {
                    if (!etcMat.HasProperty(prop.name))
                    {
                        etcMat = null;
                        Logger.LogError("etc src:{0} etc:{1} {2}!", mat.shader.name, shader.name, prop.name);
                        return false; // 没有对应的属性，不能转换
                    }

                    if (prop.type == MaterialProperty.PropType.Texture)
                    {
                        Texture2D t = mat.GetTexture(prop.name) as Texture2D;
                        if (t == null || !TextureExport.isARGB(t.format))
                        {
                            continue; // 空纹理或非ARGB格式，不需要转换
                        }

                        ResourceStream rs = ResourceStream.GetResType(t);
                        if (rs.type != ResType.Asset)
                            continue;

                        if (!etcMat.HasProperty(prop.name))
                        {
                            //Logger.LogError("etc shader 没有对应的属性! src:{0} etc:{1} {2}!", mat.shader.name, shader.name, prop.name);
                            continue; // etc里没有对应的属性，不能转换
                        }

                        if (!etcMat.HasProperty(prop.name + "Alpha"))
                        {
                            //Logger.LogError("etc shader 没有对应的透贴属性! src:{0} etc:{1} {2}!", mat.shader.name, shader.name, prop.name);
                            continue; // etc里没有对应的透明纹理，不能转换
                        }

                        // 需要转换
                        ++total;
                        name = prop.name;
                    }
                }

                if (total == 1)
                {
                    return true;
                }
                else if (total >= 2)
                {
                    Logger.LogError("etc 多个属性，不能自动转换!{0}!", mat.shader.name);
                }

                etcMat = null;
                return false;
            }
        }
    }
}
#endif
