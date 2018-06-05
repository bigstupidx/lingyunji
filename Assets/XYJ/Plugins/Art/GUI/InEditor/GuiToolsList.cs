#if UNITY_EDITOR
using UnityEngine;

using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace GUIEditor
{
    public class EmptyObject : Object
    {

    }

    public partial class GuiTools
    {
        public static void AnyObjectField<T>(bool depth, T item, System.Func<T, Object> fun, System.Action<T> begin, System.Action<T> end, params GUILayoutOption[] options)
        {
            Object o = fun(item);
            if (o is Texture)
            {
                TextureField<T>(depth, "", item, (T t) => { return (Texture)fun(t); }, begin, end, options);
            }
            else if (o is Material)
            {
                MaterialField(o as Material, depth, 
                    (Material m) =>
                    { 
                        if (begin != null) 
                        { 
                            begin(item);
                        }
                    },

                    (Material m) => 
                    {
                        if (end != null) 
                        {
                            end(item); 
                        } 
                    });
            }
            else if (o is Shader)
            {
                ShaderField(o as Shader, depth,
                    (Shader m) =>
                    {
                        if (begin != null)
                        {
                            begin(item);
                        }
                    },
                    (Shader m) =>
                    {
                        if (end != null)
                        {
                            end(item);
                        }
                    }, options);
            }
            else if (o is Mesh)
            {
                MeshField(o as Mesh, depth,
                    (Mesh m) =>
                    {
                        if (begin != null)
                        {
                            begin(item);
                        }
                    },
                    (Mesh m) =>
                    {
                        if (end != null)
                        {
                            end(item);
                        }
                    }, options);
            }
            else if (o is GameObject)
            {
                GameObjectField(o as GameObject, depth, 
                    (GameObject go) => 
                    { 
                        if (begin != null) 
                            begin(item); 
                    },
                    (GameObject go) => 
                    { 
                        if (end != null) 
                            end(item); 
                    }, options);
            }
            else if (o is Renderer)
            {
                RendererObjectField(o as Renderer, depth,
                    (Renderer r) =>
                    {
                        if (begin != null)
                            begin(item);
                    },
                    (Renderer r) =>
                    {
                        if (end != null)
                            end(item);
                    },
                    options);
            }
            else if (o is EmptyObject)
            {
                using (new GUIIndent(depth))
                {
                    EditorGUILayout.BeginHorizontal();
                    if (begin != null)
                    {
                        begin(item);
                    }

                    if (end != null)
                    {
                        end(item);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                using (new GUIIndent(depth))
                {
                    EditorGUILayout.BeginHorizontal();
                    if (begin != null)
                    {
                        begin(item);
                    }

                    EditorGUILayout.ObjectField(o, o == null ? typeof(Object) : o.GetType(), true, options);
                    EditorGUILayout.LabelField("内存:" + XTools.Utility.ToMb(GetMemoryObject(o)), GUILayout.Width(120f));
                    string path = ObjectPath(o);
                    if (!string.IsNullOrEmpty(path))
                        EditorGUILayout.LabelField(path);
                    if (GUILayout.Button("select", GUILayout.Width(60f)))
                        Selection.activeObject = o;

                    if (end != null)
                    {
                        end(item);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        static Dictionary<int, long> ObjectMemorys = new Dictionary<int, long>();
        static long GetMemoryObject(Object obj)
        {
            if (obj == null)
                return 0;

            long v;
            if (ObjectMemorys.TryGetValue(obj.GetInstanceID(), out v))
            {
                return v;
            }

            v = UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(obj);

            if (obj is Texture2D)
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                if (assetPath.StartsWith("Assets/"))
                    v /= 2;
            }

            ObjectMemorys.Add(obj.GetInstanceID(), v);
            return v;
        }

        static string ObjectPath(Object o)
        {
            if (o == null)
                return "";

            GameObject go = null;
            if (o is Component)
            {
                go = ((Component)o).gameObject;
            }
            else if (o is GameObject)
                go = (GameObject)o;

            if (go == null)
                return "";

            List<GameObject> gos = new List<GameObject>();
            gos.Add(go);
            Transform tran = go.transform.parent;
            while (tran != null)
            {
                gos.Add(tran.gameObject);
                tran = tran.parent;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = gos.Count - 1; i >= 0; --i)
                sb.AppendFormat(gos[i].name + "/");

            string s = sb.ToString();
            return s.Substring(0, s.Length - 1);
        }

        public static void AnyObjectField(bool depth, Object o, System.Action<Object> begin, System.Action<Object> end)
        {
            AnyObjectField<Object>(depth, o, (Object t) => { return t; }, begin, end);
        }

        public static void ObjectFieldList<T>(ParamList paramlist, List<T> objs, bool depth, System.Action<T> end = null, params GUILayoutOption[] options) where T : Object
        {
            if (isInherited(typeof(T), typeof(Texture)))
            {
                List<Texture> texList = new List<Texture>();
                foreach (T obj in objs)
                    texList.Add((Texture)((Object)obj));
                TextureListField(paramlist, depth, "", texList, null, null, 
                    (Texture tex) => 
                    {
                        if (end != null)
                        {
                            T t = tex as T;
                            end(t);
                        }
                    },null, options);
            }
            else
            {
                List<Object> tobjs = new List<Object>();
                foreach (T obj in objs)
                    tobjs.Add(obj);
                ObjectFieldList(
                    paramlist, 
                    tobjs, 
                    (Object o) => { return o; }, 
                    depth, 
                    null,
                    null,
                    (Object o) => 
                    {
                        if (end != null)
                        {
                            end(o as T);
                        }
                    }, options);
            }
        }

        static System.Type GetType(Object o)
        {
            if (o == null)
                return null;

            return o.GetType();
        }

        static string GetTypeName(Object o)
        {
            System.Type t = GetType(o);
            if (t == null)
                return "null";

            return t.Name;
        }

        public static void ObjectFieldList<T>(
            ParamList pl, 
            List<T> items, 
            System.Func<T, Object> fun, 
            bool depth, 
            System.Action<List<T>, ParamList> beginList, 
            System.Action<T> begin, 
            System.Action<T> end, 
            params GUILayoutOption[] options)
        {
            using (new GUIIndent(depth))
            {
                List<T> showList = items;
                int beginindex = 0;
                int endindex = items.Count;

                XTools.Multimap<string, T> typeList = new XTools.Multimap<string, T>();
                List<string> keys = new List<string>();
                keys.Add("all");
                int texsize = 0;
                int texcount = 0;
                foreach (T o in items)
                {
                    Object obj = fun(o);
                    //System.Type type = GetType(obj);
                    string typeName = GetTypeName(obj);

                    typeList.Add(typeName, o);
                    if (!keys.Contains(typeName))
                        keys.Add(typeName);

                    if (obj is Texture)
                    {
                        Texture t = obj as Texture;
                        texsize += (t.width * t.height);
                        ++texcount;
                    }
                }

                if (texcount != 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(string.Format("texcount:{0} allsize:{1}*{1}", texcount, Mathf.Sqrt((float)texsize)));
                    EditorGUILayout.EndHorizontal();
                }

                string select = pl.GetString("select");

                bool sd = false;
                if (keys.Count > 2)
                {
                    //                Debug.Log("keys.Count" + keys.Count);
                    select = StringPopup(depth, select, keys);
                    pl.Set("select", select);
                    sd = true;
                }
                else if (keys.Count == 2)
                {
                    //select = "all";
                    select = keys[1];
                }
                else
                {
                    select = "all";
                }

                if (select == "all")
                {
                    showList = items;
                }
                else
                {
                    typeList.TryGetValue(select, out showList);
                }

                if (select == typeof(Texture2D).Name)
                {
                    TextureListField<T>(pl.Get<ParamList>("TextureListField"), sd, "", showList, (T tt) => { return fun(tt) as Texture2D; }, null, begin, end, options);
                }
                else if (select == typeof(Material).Name)
                {
                    MaterialListField<T>(pl.Get<ParamList>("MaterialListField"), sd, "", showList, (T tt) => { return fun(tt) as Material; }, beginList, null, begin, end, options);
                }
                else if (select == typeof(Mesh).Name)
                {
                    MeshListField<T>(pl.Get<ParamList>("MeshListField"), sd, "", showList, (T tt) => { return fun(tt) as Mesh; }, beginList, null, begin, end, options);
                }
                else
                {
                    if (beginList != null)
                        beginList(showList, pl.Get<ParamList>("Object_beginList"));

                    EditorPageBtn epb = pl.Get<EditorPageBtn>("epb");
                    epb.total = (showList == null ? 0 : showList.Count);
                    epb.pageNum = 20;
                    if (epb.TotalPage > 1)
                        epb.OnRender(sd);

                    beginindex = epb.beginIndex;
                    endindex = epb.endIndex;

                    for (int i = beginindex; i < endindex; ++i)
                    {
                        AnyObjectField<T>(sd, showList[i], fun, begin, end, options);
                    }
                }
            }
        }

//         public static void ObjectFieldList(ParamList pl, List<Object> objs, int depth, System.Action<Object> end = null)
//         {
//             ObjectFieldList<Object>(pl, objs, (Object o) => { return o; }, depth, null, null, end);
//         }
    }
}

#endif