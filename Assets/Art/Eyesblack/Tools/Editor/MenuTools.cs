using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;


namespace Eyesblack.EditorTools {
    public class MenuTools {
        [MenuItem("Eyesblack/查找使用此Shader的材质")]
        private static void FindShaderReferencesInProject() {
            Shader shader = Selection.activeObject as Shader;
            if (shader == null) {
                Debug.LogError("请选择一个Shader文件！");
                return;
            }


            string[] guids = AssetDatabase.FindAssets("t:Material", new string[] { "Assets" });
            int index = 0;
            foreach (string guid in guids) {
                if (!EditorUtility.DisplayCancelableProgressBar("查找材质", "Processing", index++ / (float)guids.Length)) {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
                    if (material.shader == shader) {
                        Debug.Log(material.name, material);
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }




        [DllImport("FBXExporter", EntryPoint="RemoveVertexColors")]
        private static extern int RemoveFBXVertexColors(string[] fbxFilePaths, int fileCount);


        [MenuItem("Eyesblack/删除FBX顶点颜色")]
        private static void RemoveFBXMeshVertexColors() {
            List<string> fbxFiles = new List<string>();
            foreach (Object obj in Selection.objects) {
                string path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path)) {
                    string ext = Path.GetExtension(path).ToLower();
                    if (ext == ".fbx") {
                        fbxFiles.Add(path);
                    }
                }
            }

            if (fbxFiles.Count > 0) {
                RemoveFBXVertexColors(fbxFiles.ToArray(), fbxFiles.Count);

                foreach (string fbxPath in fbxFiles) {
                    AssetDatabase.ImportAsset(fbxPath, ImportAssetOptions.ForceUpdate);
                }


                Debug.Log("已完成删除顶点颜色！");
            } else {
                Debug.LogError("没有选中任何有效FBX文件！");
            }

        }


        /*
        [MenuItem("Eyesblack/统计场景Mesh资源内存")]
        private static void StatMeshAssetsMem() {
            List<Mesh> meshes = new List<Mesh>();

            int mem = 0;
            foreach (MeshFilter meshFilter in Object.FindObjectsOfType<MeshFilter>()) {
                if (!meshes.Contains(meshFilter.sharedMesh)) {
                    mem += MemoryStat.CalcMeshSizeBytes(meshFilter.sharedMesh);
                    meshes.Add(meshFilter.sharedMesh);
                }                  
            }

            foreach (SkinnedMeshRenderer skinnedRenderer in Object.FindObjectsOfType<SkinnedMeshRenderer>()) {
                if (!meshes.Contains(skinnedRenderer.sharedMesh)) {
                    mem += MemoryStat.CalcMeshSizeBytes(skinnedRenderer.sharedMesh);
                    meshes.Add(skinnedRenderer.sharedMesh);
                }
            }

            Debug.Log(mem / 1024.0f / 1024.0f);
        }
        */
    }

}
