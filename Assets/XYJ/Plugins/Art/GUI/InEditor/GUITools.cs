#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEditor;

namespace GUIEditor
{
    public partial class GuiTools
    {
        // 是否有继承关系
        public static bool isInherited(System.Type type, System.Type baseType)
        {
            if (type == null)
                return false;

            if (type == baseType)
                return true;

            if (type.BaseType == null)
                return false;

            if (type.BaseType == baseType)
                return true;

            return isInherited(type.BaseType, baseType);
        }

        static float Pixel(int depth)
        {
            return depth * 30f;
        }

        static bool IsSelected(Object obj)
        {
            Object[] objs = Selection.objects;
            foreach (Object o in objs)
            {
                if (o == obj)
                    return true;

                if (obj as Component)
                {
                    if (o == ((Component)obj).gameObject)
                        return true;
                }
            }

            return false;
        }

        static Color GetObjectColor(Object obj, Color col)
        {
            if (IsSelected(obj))
                return Color.green;

            return col;
        }

        public static void ObjectField(Object obj, bool depth, System.Action<Object> begin, System.Action<Object> end)
        {
            ObjectField<Object>(obj, depth, begin, end);
        }

        public static void IndentLevel()
        {
            if (EditorGUI.indentLevel != 0)
            {
                GUILayout.Space(EditorGUI.indentLevel * 15);
            }
        }

        public static void ObjectField<T>(T obj, bool depth, System.Action<T> begin, System.Action<T> end, params GUILayoutOption[] objs) where T : Object
        {
            Color col = GUI.color;
            GUI.color = GetObjectColor(obj, col);

            EditorGUILayout.BeginHorizontal();
            using (new GUIIndent(depth))
            {
                IndentLevel();
                if (begin != null)
                    begin(obj);
                if (obj as Component && typeof(T) == typeof(Object))
                {
                    Component com = obj as Component;
                    EditorGUILayout.ObjectField(com.gameObject, typeof(GameObject), true, objs);
                }
                else
                    EditorGUILayout.ObjectField(obj, typeof(T), true, objs);

                if (GUILayout.Button("选中", GUILayout.Width(100)))
                    Selection.activeObject = obj;

                try
                {
                    if (end != null)
                        end(obj);
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
                GUI.color = col;
            }
            EditorGUILayout.EndHorizontal();
        }

        public static void LabelFieldFun(bool depth, string label, params System.Action[] begins)
        {
            using (new GUIIndent(depth))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(label);
                for (int i = 0; i < begins.Length; ++i)
                {
                    if (begins[i] != null)
                        begins[i]();
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        public static void TextFieldFun(bool depth, string label, params System.Action[] begins)
        {
            using (new GUIIndent(depth))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.TextField(label);
                for (int i = 0; i < begins.Length; ++i)
                {
                    if (begins[i] != null)
                        begins[i]();
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        public static bool[] LabelField(bool depth, string label, params string[] buttons)
        {
            using (new GUIIndent(depth))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(label);
                //bool b = false;
                bool[] res = new bool[buttons.Length];
                for (int i = 0; i < buttons.Length; ++i)
                {
                    res[i] = false;
                    if (!string.IsNullOrEmpty(buttons[i]))
                        res[i] = GUILayout.Button(buttons[i]);
                }

                EditorGUILayout.EndHorizontal();
                return res;
            }
        }

        public static void HorizontalField(bool depth, params System.Action[] actions)
        {
            EditorGUILayout.BeginHorizontal();
            using (new GUIIndent(depth))
            {
                foreach (System.Action action in actions)
                    action();
            }
            EditorGUILayout.EndHorizontal();
        }

        public static void HorizontalFieldAsTextArea(bool depth, System.Action action, params GUILayoutOption[] objs)
        {
            using (new GUIIndent(depth))
            {
                IndentLevel();
                List<GUILayoutOption> p = new List<GUILayoutOption>();
                p.Add(GUILayout.MinHeight(20f));
                if (objs != null)
                    p.AddRange(objs);

                EditorGUILayout.BeginHorizontal("AS TextArea", p.ToArray());
                action();
                EditorGUILayout.EndHorizontal();
            }
        }

        public delegate void ObjField<T>(T obj, bool depth, System.Action<T> begin, System.Action<T> end);

        static ParamList GlobalPL = new ParamList();

        // 材质显示
        public static void MaterialField(Material mat, bool depth, System.Action<Material> begin, System.Action<Material> end)
        {
            using (new GUIIndent(depth))
            {
                ParamList pl = GlobalPL.Get("mat:" + mat.GetHashCode(), () => { return new ParamList(); });
                bool isshow = pl.Get("isshow", false);
                ObjectField<Material>(mat, depth, (Material m) =>
                {
                    if (GUILayout.Button(isshow ? "隐藏纹理" : "显示纹理", GUILayout.Width(80)))
                    {
                        isshow = !isshow;
                        pl.Set("isshow", isshow);
                    }
                    if (begin != null)
                        begin(m);
                },
                (Material m) =>
                {
                    long totalMemory = 0;
                    foreach (var t in XTools.Utility.GetMaterialTexture(m))
                        totalMemory += GuiTools.TextureMemorySize(t);

                    GUILayout.Label(string.Format("内存占用:{0}", XTools.Utility.ToMb(totalMemory)), GUILayout.Width(200f));
                    if (end != null)
                        end(m);
                }, GUILayout.Width(350f));

                if (isshow)
                {
                    using (new GUIIndent(true))
                    {
                        List<Texture> texList = XTools.Utility.GetMaterialTexture(mat);
                        GuiTools.TextureListField(pl.Get<ParamList>("TextureList"), true, "", texList);
                    }
                }
            }
        }

        // shader显示
        public static void ShaderField(Shader shader, bool depth, System.Action<Shader> begin, System.Action<Shader> end, params GUILayoutOption[] objs)
        {
            ObjectField<Shader>(shader, depth, begin, end, ((objs == null || objs.Length == 0) ? new GUILayoutOption[] { GUILayout.Width(250f) } : objs));
        }

        static Dictionary<int, MeshInfo> MeshInfos = new Dictionary<int, MeshInfo>();

        public class MeshInfo
        {
            public Mesh mesh;
            public int VertexCount;
            public int faceCount;
            public int totalMemory;

            public void Init(Mesh m)
            {
                mesh = m;
                VertexCount = mesh.vertexCount;
                faceCount = GetMeshFace(mesh);
                totalMemory = GetMeshSize(mesh);
            }
        }

        public static MeshInfo GetMeshInfo(Mesh mesh)
        {
            MeshInfo info = null;
            if (MeshInfos.TryGetValue(mesh.GetInstanceID(), out info))
                return info;

            info = new MeshInfo();
            info.Init(mesh);

            MeshInfos.Add(mesh.GetInstanceID(), info);
            return info;
        }

        public static void MeshField(Mesh obj, bool depth, System.Action<Mesh> begin, System.Action<Mesh> end, params GUILayoutOption[] objs)
        {
            ObjectField<Mesh>(
                obj,
                depth,
                (Mesh mesh) =>
                {
                    if (mesh == null)
                        return;

                    if (begin != null)
                        begin(mesh);

                    MeshInfo info = GetMeshInfo(obj);

                    // 顶点数量，
                    EditorGUILayout.LabelField("顶点:" + info.VertexCount, GUILayout.Width(90f));
                    EditorGUILayout.LabelField("面数:" + info.faceCount, GUILayout.Width(90f));
                    EditorGUILayout.LabelField("内存:" + XTools.Utility.ToMb(info.totalMemory), GUILayout.Width(90f));
                },
                (Mesh mesh) =>
                {
                    if (GUILayout.Button("保存", GUILayout.Width(50f)))
                    {
                        string file = UnityEditor.EditorUtility.SaveFilePanelInProject("保存网格", obj.name.Replace(":", ""), "asset", "保存的路径");
                        if (!string.IsNullOrEmpty(file))
                        {
                            AssetDatabase.CreateAsset(obj, file);
                        }
                    }

                    if (GUILayout.Button(mesh.isReadable ? "可读写" : "不可读", GUILayout.Width(60f)))
                    {
                        SetMeshReadable(mesh, !mesh.isReadable);
                    }

                    if (end != null)
                        end(mesh);
                }, 
                objs);
        }

        public static void SetMeshReadable(Mesh mesh, bool value)
        {
            if (mesh.isReadable == value)
                return;

            var assetPath = AssetDatabase.GetAssetPath(mesh);
            ModelImporter mi = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (mi != null)
            {
                mi.isReadable = value;
                EditorUtility.SetDirty(mi);
                AssetDatabase.ImportAsset(assetPath);
            }
        }

        public static void GameObjectField(GameObject go, bool depth, System.Action<GameObject> begin, System.Action<GameObject> end, params GUILayoutOption[] objs)
        {
            ObjectField<GameObject>(
                go,
                depth,
                (GameObject obj) =>
                {
                    if (begin != null)
                        begin(obj);
                },
                (GameObject obj) =>
                {
                    if (end != null)
                        end(obj);
                }, objs);
        }

        public static void RendererObjectField(Renderer renderer, bool depth, System.Action<Renderer> begin, System.Action<Renderer> end, params GUILayoutOption[] objs)
        {
            ObjectField<Renderer>(
                renderer,
                depth,
                (Renderer obj) =>
                {
                    if (begin != null)
                        begin(obj);
                },
                (Renderer obj) =>
                {
                    if (end != null)
                        end(obj);
                }, objs);
        }

        public static int GetMeshsMemory(List<Mesh> meshs)
        {
            int total = 0;
            for (int i = 0; i < meshs.Count; ++i)
                total += GetMeshInfo(meshs[i]).totalMemory;

            return total;
        }

        public static int GetMeshsVertexCount(List<Mesh> meshs)
        {
            int index = 0;
            meshs.RemoveAll((Mesh m) => { return m == null ? true : false; });
            for (int i = 0; i < meshs.Count; ++i)
                index += meshs[i].vertexCount;

            return index;
        }

        public static int GetMeshsFace(List<Mesh> meshs)
        {
            int index = 0;
            if (meshs == null)
                return index; 
            
            for (int i = 0; i < meshs.Count; ++i)
                index += GetMeshFace(meshs[i]);

            return index;
        }

        public static int GetMeshFace(Mesh mesh)
        {
            int index = 0;
            if (mesh == null)
                return index;

            for (int i = 0; i < mesh.subMeshCount; ++i)
                index += (mesh.GetTriangles(i).Length/ 3);

            return index;
        }

        public static int GetMeshSize(Mesh mesh)
        {
            int count = mesh.vertexCount;
            int pointSize = 0; // 一个顶点的大小
            if (mesh.colors.Length != 0)
                pointSize += 4; // 颜色，4个字节

            if (mesh.normals.Length != 0)
                pointSize += 12; // 法线位置，12个字节

            if (mesh.tangents.Length != 0)
                pointSize += 12; // 切线位置，12个字节

            if (mesh.uv.Length != 0)
                pointSize += 8; // UV坐标，8个字节

            if (mesh.uv2.Length != 0)
                pointSize += 8; // UV坐标，8个字节
            if (mesh.uv3.Length != 0)
                pointSize += 8; // UV坐标，8个字节
            if (mesh.uv4.Length != 0)
                pointSize += 8; // UV坐标，8个字节

            pointSize += 12; // 位置信息，12个字节

            int total = pointSize * count;
            for (int i = 0; i < mesh.subMeshCount; ++i)
                total += mesh.GetIndices(i).Length * 2;

            return total;
        }

        public static string GetTextureInfo(Texture tex)
        {
            return string.Format("w:{0} h:{1} m:{2}", tex.width, tex.height, XTools.Utility.ToMb(TextureMemorySize(tex)));
        }

        public static void TextureField(Texture tex, bool depth, System.Action<Texture> begin, System.Action<Texture> end, params GUILayoutOption[] options)
        {
            ObjectField<Texture>(
                tex,
                depth,
                (Texture t) =>
                {
                    if (t == null)
                        return;

                    if (begin != null)
                        begin(tex);

                    EditorGUILayout.LabelField(GetTextureInfo(tex), GUILayout.Width(200f));
                },
                (Texture t) =>
                {
                    if (t == null)
                        return;

                    string path = AssetDatabase.GetAssetPath(tex);
                    if (!string.IsNullOrEmpty(path))
                    {
                        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                        if (GUILayout.Button("缩小", GUILayout.Width(60f)))
                        {
                            ti.maxTextureSize /= 2;
                            AssetDatabase.ImportAsset(path);
                            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                        }

                        if (GUILayout.Button("放大", GUILayout.Width(60f)))
                        {
                            ti.maxTextureSize *= 2;
                            AssetDatabase.ImportAsset(path);
                            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                        }

                        if (GUILayout.Button("恢复", GUILayout.Width(60f)))
                        {
                            int maxSize = 0;
                            {
                                string file = Application.dataPath + "/" + path.Substring(7);
                                string copy = file.Insert(file.LastIndexOf('.'), "_copy");
                                System.IO.File.Copy(file, copy);
                                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                                string copy_asset = path.Insert(path.LastIndexOf('.'), "_copy");
                                Texture copy_tex = AssetDatabase.LoadAssetAtPath(copy_asset, typeof(Texture)) as Texture;
                                maxSize = Mathf.Max(copy_tex.width, copy_tex.height);
                                System.IO.File.Delete(copy);
                                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                            }

                            ti.maxTextureSize = (int)Mathf.Max(maxSize, maxSize);
                            AssetDatabase.ImportAsset(path);
                            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                        }
                    }

                    if (end != null)
                        end(tex);
                },
                (options == null || options.Length == 0) ? new GUILayoutOption[] { GUILayout.Width(350f) } : options);
        }
    }
}

#endif