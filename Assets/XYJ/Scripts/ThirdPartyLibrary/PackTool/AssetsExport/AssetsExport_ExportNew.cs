#if UNITY_EDITOR && USE_RESOURCESEXPORT
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public partial class AssetsExport
    {
        IEnumerator ExportMaterial(Material mat)
        {
            string key = AssetDatabase.GetAssetPath(mat).Substring(7);
            string path = "Assets/__copy__/" + key;
            Material copy_mat = null;
            bool hasExists = File.Exists(path);
            if (hasExists && !IsNeedUpdate(path))
            {
                copy_mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (copy_mat != null)
                {
                    CollectBuiltinResource(mat.shader);
                    CollectBuiltinMaterial(copy_mat);
                    yield break;
                }
            }

            // 文件不存在，或者需要更新
            if (hasExists)
            {
                File.Delete(path);
            }
            else
            {
                Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('/')));
            }

            AssetDatabase.CreateAsset(new Material(mat), path);
            AssetDatabase.Refresh();

            copy_mat = AssetDatabase.LoadAssetAtPath<Material>(path);            MaterialProperty[] mps = MaterialEditor.GetMaterialProperties(new Object[] { copy_mat });

            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write((byte)0);
            CollectBuiltinResource(copy_mat.shader);
            
            int total = 0;
            foreach (MaterialProperty mp in mps)
            {
                if (mp.type == MaterialProperty.PropType.Texture)
                {
                    Texture texture = copy_mat.GetTexture(mp.name);
                    if (texture != null)
                    {
                        writer.Write(mp.name);
                        ComponentData.__CollectTexture__<Texture>(ref texture, writer, this);
                        copy_mat.SetTexture(mp.name, texture);
                        ++total;
                    }
                }
            }

            EditorUtility.SetDirty(copy_mat);
            CollectBuiltinMaterial(copy_mat);

            string filePath = PackPath + key;
            if (File.Exists(filePath))
                File.Delete(filePath);
            else
            {
                Directory.CreateDirectory(filePath.Substring(0, filePath.LastIndexOf('/')));
            }

            if (total > 0)
            {
                var pos = stream.Position;
                writer.BaseStream.Position = 0;
                writer.Write((byte)total);
                writer.BaseStream.Position = pos;
                File.WriteAllBytes(filePath, stream.ToArray());
            }
            else
            {
                File.WriteAllBytes(filePath, new byte[0] { });
            }

            yield break;
        }

        static bool IsNeedExport(Object obj)
        {
            string prefabpath = AssetDatabase.GetAssetPath(obj).Substring(7);
            string file = PackPath + prefabpath;

            // 不需要更新，prefabpath
            if (!IsNeedUpdate(prefabpath) && System.IO.File.Exists(file))
                return false;

            return true;
        }

        List<string> ExportPrefabs = new List<string>();

        IEnumerator PrefabExport(GameObject prefab)
        {
            // 不需要更新
            if (!IsNeedExport(prefab))
            {
                string newPath = string.Format("{0}/__copy__/{1}", Application.dataPath, AssetDatabase.GetAssetPath(prefab).Substring(7));
                if (File.Exists(newPath))
                    yield break;
            }

            GameObject copy = CopyPrefab(prefab);
            GameObject go = CollectPrefabImp(copy, this);
            ExportPrefabs.Add(AssetDatabase.GetAssetPath(go));
        }

        public IEnumerator MeshExport(Mesh mesh)
        {
            string path = AssetDatabase.GetAssetPath(mesh);
            if (!path.ToLower().EndsWith(".asset"))
            {
                mesh = CopyObjectClip<Mesh>(mesh, ".asset");
            }

            ExportMesh(mesh);
            yield break;
        }

        public IEnumerator AnimationClipExport(AnimationClip clip)
        {
            string path = AssetDatabase.GetAssetPath(clip);
            if (!path.ToLower().EndsWith(".anim"))
                clip = CopyObjectClip<AnimationClip>(clip, ".anim");

            ExportAnim(clip);
            yield break;
        }
    }
}
#endif