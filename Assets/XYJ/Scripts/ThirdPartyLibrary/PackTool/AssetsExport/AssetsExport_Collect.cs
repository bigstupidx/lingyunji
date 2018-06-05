#if UNITY_EDITOR && USE_RESOURCESEXPORT
#define Instantiate_Prefab

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace PackTool
{
    public partial class AssetsExport
    {
        static void GetMaterialProperty(Material mat, MaterialProperty[] props, Material etc_mat, string propname, AssetsExport mgr, BinaryWriter writer, out ETCType type)
        {
            type = ETCType.Null;
            if (props == null)
                props = MaterialEditor.GetMaterialProperties(new Object[1] { mat });
            writer.Write((byte)(props.Length + (etc_mat == null ? 0 : 1)));
            foreach (MaterialProperty prop in props)
            {
                writer.Write((byte)prop.type);
                writer.Write(prop.name);
                switch (prop.type)
                {
                case MaterialProperty.PropType.Color:
                    {
                        Color32 color = prop.colorValue;
                        writer.Write(color.r);
                        writer.Write(color.g);
                        writer.Write(color.b);
                        writer.Write(color.a);
                    }
                    break;
                case MaterialProperty.PropType.Vector:
                    {
                        writer.Write(prop.vectorValue.x);
                        writer.Write(prop.vectorValue.y);
                        writer.Write(prop.vectorValue.z);
                        writer.Write(prop.vectorValue.w);
                    }
                    break;
                case MaterialProperty.PropType.Float:
                case MaterialProperty.PropType.Range:
                    {
                        writer.Write(prop.floatValue);
                    }
                    break;
                case MaterialProperty.PropType.Texture:
                    {
                        Texture texture = mat.GetTexture(prop.name);
                        if (etc_mat != null && prop.name == propname)
                        {
                            // 需要转换下
                            Texture2D src = (Texture2D)texture;
                            Texture2D etct2d;
                            Texture2D alpha2d;
                            GenETC(src, out etct2d, out alpha2d, out type);

                            Vector2 offset = mat.GetTextureOffset(prop.name);
                            Vector2 scale = mat.GetTextureScale(prop.name);
                            WriteTextureValue(writer, mgr, etct2d, offset, scale);

                            writer.Write((byte)prop.type);
                            writer.Write(prop.name + "Alpha");
                            WriteTextureValue(writer, mgr, alpha2d, offset, scale);
                        }
                        else
                        {
                            WriteTextureValue(writer, mgr, texture, mat.GetTextureOffset(prop.name), mat.GetTextureScale(prop.name));
                        }
                    }
                    break;
                }
            }
        }

        static void WriteTextureValue(BinaryWriter writer, AssetsExport mgr, Texture texture, Vector2 offset, Vector2 scale)
        {
            ComponentData.__CollectTexture__<Texture>(ref texture, writer, mgr);

            writer.Write(offset.x);
            writer.Write(offset.y);

            writer.Write(scale.x);
            writer.Write(scale.y);
        }

        // 收集依赖
        static void CollectMaterial(Material mat, AssetsExport mgr)
        {
            if (mgr.isCollect(mat))
                return;

            mgr.AddCollect(mat);
            mgr.AddMaterial(mat);
        }

        static void WriteShader(Shader shader, AssetsExport mgr, System.IO.BinaryWriter writer)
        {
            ComponentData.__CollectShader__(ref shader, writer, mgr);
        }

        static void WriteStreamToFile(Stream stream, string file)
        {
            if (File.Exists(file))
                File.Delete(file);
            else
                Directory.CreateDirectory(file.Substring(0, file.LastIndexOf('/')));

            stream.Position = 0;
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);
            //stream.Close();
            File.WriteAllBytes(file, bytes);
        }

        internal static GameObject CopyPrefab(GameObject obj)
        {
            string prefabPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(prefabPath))
            {
                Debuger.LogError("string.IsNullOrEmpty(prefabPath):" + obj.name);
            }
            string newPath = string.Format("{0}/__copy__/{1}", Application.dataPath, prefabPath.Substring(7));
            string path = newPath.Substring(0, newPath.LastIndexOf('/'));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            newPath = "Assets/__copy__/" + prefabPath.Substring(7);
            if (AssetDatabase.DeleteAsset(newPath))
            {
                //Logger.LogDebug("del:" + newPath);
                //AssetDatabase.Refresh();
                //AssetDatabase.SaveAssets();
            }

            AssetDatabase.CopyAsset(prefabPath, newPath);
            GameObject copy = AssetDatabase.LoadAssetAtPath<GameObject>(newPath);
            ExtraNodes en = copy.GetComponent<ExtraNodes>();
            if (en != null)
            {
                Object.DestroyImmediate(en, true);
                EditorUtility.SetDirty(copy);
            }

            return copy;
        }

        static void CollectPrefab(GameObject obj, AssetsExport mgr)
        {
            if (mgr.isCollect(obj))
                return;

            mgr.AddCollect(obj);
            mgr.AddPrefab(obj);

            //CollectPrefabImp(CopyPrefab(obj), mgr);
        }

        static void CollectTMPFont(TMPro.TMP_FontAsset fontasset, AssetsExport mgr)
        {
            if (mgr.isCollect(fontasset))
                return;

            mgr.AddCollect(fontasset);
            mgr.AddTMPFontlib(fontasset);
        }

        static void CollectTexture2DAsset(Texture2DAsset asset, AssetsExport mgr)
        {
            if (mgr.isCollect(asset))
                return;

            mgr.AddCollect(asset);
            mgr.AddT2DAsset(asset);
        }

        static void CollectScene(Object obj, AssetsExport mgr)
        {
            if (mgr.isCollect(obj))
                return;
            mgr.AddCollect(obj);

            string path = UnityEditor.AssetDatabase.GetAssetPath(obj);
            Scene currentScene = EditorSceneManager.OpenScene(path);

            GameObject root = currentScene.GetRootGameObjects()[0];
            PackTool.ComponentSave comSave = new PackTool.ComponentSave();
            comSave.Save(root, mgr);

            SceneResRecords rrs = root.GetComponent<SceneResRecords>();
            if (rrs == null)
                rrs = root.AddComponent<SceneResRecords>();

            rrs.components = comSave.CompList.ToArray();
            string filename = path.Substring(7);

            string scene_path = Application.dataPath + "/" + filename;
            string file_data_bytes = scene_path + Suffix.SceneDataByte;
            string file_pos_bytes = scene_path + Suffix.ScenePosByte;

            File.WriteAllBytes(file_data_bytes, comSave.GetSaveData());

            MemoryStream stream = new MemoryStream(comSave.PositionList.Count * 4);
            BinaryWriter writer = new BinaryWriter(stream);
            foreach (int d in comSave.PositionList)
                writer.Write(d);

            stream.Position = 0;
            File.WriteAllBytes(file_pos_bytes, stream.ToArray());
            stream.Close();

            string prefabpath = path.Substring(0, path.LastIndexOf('.')) + ".scene.prefab";
            GameObject go = AssetDatabase.LoadAssetAtPath(prefabpath, typeof(GameObject)) as GameObject;
            if (go == null)
            {
                go = PrefabUtility.CreatePrefab(prefabpath, root);
            }
            else
            {
                go = PrefabUtility.ReplacePrefab(root, go);
            }

            ImportAsset(prefabpath);

            Object.DestroyImmediate(root);
            Object.Instantiate(go).name = go.name;

            Lightmapping.lightingDataAsset = null;
            EditorSceneManager.SaveScene(currentScene);
            ImportAsset(path);

            mgr.AddScene(obj);
        }

        public static void ImportAsset(string path)
        {
            if (path.Contains("."))
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.Default | ImportAssetOptions.ForceUncompressedImport);
            else
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ImportRecursive | ImportAssetOptions.Default | ImportAssetOptions.ForceUncompressedImport);

            AssetDatabase.SaveAssets();
        }

        static List<ComponentSave> EmptyList = new List<ComponentSave>();

        //[MenuItem("Assets/SetAlawyIncludeShader")]
        static void SetAlawyIncludeShader()
        {
            HashSet<string> guids = new HashSet<string>(AssetDatabase.FindAssets("t:shader", new string[] { "Assets" }));
            List<Shader> shaders = new List<Shader>();
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (assetPath.Contains("/Resources/"))
                {
                    shaders.Add(AssetDatabase.LoadAssetAtPath<Shader>(assetPath));
                }
            }

            Dictionary<Shader, int> currents = new Dictionary<Shader, int>(); // 当前设置的
            List<int> emptys = new List<int>();
            SerializedObject gs = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
            SerializedProperty alwaysIncludedShaders = gs.FindProperty("m_AlwaysIncludedShaders");

            int arraySize = alwaysIncludedShaders.arraySize;
            for (int i = 0; i < arraySize; ++i)
            {
                SerializedProperty sp = alwaysIncludedShaders.GetArrayElementAtIndex(i);
                if (sp.objectReferenceValue == null)
                {
                    emptys.Add(i);
                }
                else
                {
                    currents.Add(sp.objectReferenceValue as Shader, i);
                }
            }

            bool isDirty = false;
            for (int i = 0; i < shaders.Count; ++i)
            {
                if (currents.ContainsKey(shaders[i]))
                {
                    // 当前已经存在了
                }
                else
                {
                    isDirty = true;

                    if (emptys.Count != 0)
                    {
                        alwaysIncludedShaders.GetArrayElementAtIndex(emptys[0]).objectReferenceValue = shaders[i];
                        emptys.RemoveAt(0);
                    }
                    else
                    {
                        arraySize = alwaysIncludedShaders.arraySize;
                        alwaysIncludedShaders.InsertArrayElementAtIndex(arraySize);
                        alwaysIncludedShaders.GetArrayElementAtIndex(arraySize).objectReferenceValue = shaders[i];
                    }
                }
            }

            if (isDirty)
            {
                gs.ApplyModifiedProperties();
            }
        }

        static ComponentSave Get()
        {
            if (EmptyList.Count == 0)
                return new ComponentSave();

            ComponentSave c = EmptyList[EmptyList.Count - 1];
            EmptyList.RemoveAt(EmptyList.Count - 1);
            return c;
        }

        static void Free(ComponentSave c)
        {
            c.Release();
            EmptyList.Add(c);
        }

        public static System.Func<WellFired.USSequencer, List<Object>> _save_ussequencer_prefab_ = null;

        [MenuItem("Assets/PackTool/TestPrefab")]
        static void TestPrefab()
        {
            //GameObject go = CollectPrefabImp(CopyPrefab(Selection.activeGameObject), new AssetsExport());
            ExportPrefab(Selection.activeGameObject);
        }

        static GameObject CollectPrefabImp(GameObject prefab, AssetsExport mgr)
        {
#if Instantiate_Prefab
            bool bIsActive = prefab.activeSelf;
            if (bIsActive)
                prefab.SetActive(false);
#endif
            string prefabpath = AssetDatabase.GetAssetPath(prefab);

#if Instantiate_Prefab
            GameObject instance = (GameObject)GameObject.Instantiate(prefab);
            instance.name = prefab.name;
#else
            GameObject instance = prefab;
#endif
            ComponentSave comSave = Get();
            comSave.Save(instance, mgr);

            {
                byte[] bytes = comSave.GetSaveData();
                string file_data_bytes = "";
                {
                    string file_prefabpath = Application.dataPath + "/" + prefabpath.Substring(7);
                    file_data_bytes = file_prefabpath + Suffix.PrefabDataByte;

                    Directory.CreateDirectory(file_data_bytes.Substring(0, file_data_bytes.LastIndexOf('/')));
                    if (bytes.Length == 0)
                    {
                        if (File.Exists(file_data_bytes))
                            File.Delete(file_data_bytes);
                    }
                    else
                    {
                        File.WriteAllBytes(file_data_bytes, bytes);
                    }
                }

                string path_data = GetSrcPath(prefab) + Suffix.PrefabDataByte;
                string fs = PackPath + path_data;
                if (File.Exists(fs))
                    File.Delete(fs);

                if (File.Exists(file_data_bytes))
                {
                    Directory.CreateDirectory(fs.Substring(0, fs.LastIndexOf('/')));
                    File.Copy(file_data_bytes, fs);
                }
            }

            bool isDirty = false;
            if (comSave.CompList == null || comSave.CompList.Count == 0)
            {
                ResourcesRecords rrs = instance.GetComponent<ResourcesRecords>();
                if (rrs != null)
                {
                    Object.DestroyImmediate(rrs, true);
                    isDirty = true;
                }
            }
            else
            {
                ResourcesRecords rrs = instance.GetComponent<ResourcesRecords>();
                if (rrs == null)
                    rrs = instance.AddComponent<ResourcesRecords>();

                rrs.components = comSave.CompList.ToArray();
                isDirty = true;
            }

            Free(comSave);

#if Instantiate_Prefab
            if (isDirty)
            {
                List<Object> objs = null;
                if (instance.GetComponent<WellFired.USSequencer>() != null)
                {
                    objs = _save_ussequencer_prefab_(instance.GetComponent<WellFired.USSequencer>());
                    for (int i = 0; i < objs.Count; ++i)
                        objs[i].name = i.ToString();
                }
                prefab = PrefabUtility.CreatePrefab(prefabpath, instance);
                if (objs != null)
                {
                    foreach (Object obj in objs)
                        AssetDatabase.AddObjectToAsset(obj, prefab);

                    EditorUtility.SetDirty(prefab);
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();

                    EditorUtility.SetDirty(prefab = PrefabUtility.ReplacePrefab(instance, prefab));

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    AssetDatabase.SaveAssets();
                }
            }

            if (bIsActive)
                prefab.SetActive(true);

            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();

            Object.DestroyImmediate(instance);
            prefab = AssetDatabase.LoadAssetAtPath(prefabpath, typeof(GameObject)) as GameObject;
#else
            if (isDirty)
            {
                EditorUtility.SetDirty(prefab);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
#endif
            return prefab;
        }
    }
}
#endif