using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


namespace Eyesblack.EditorTools {
    public class WndSceneChecker : EditorWindow {
        static bool _checkMemories = true;
        static bool _checkHiddenObjects = true;
        static bool _checkComponents = true;
        static bool _checkMeshData = true;

        MemoryStat _memoryStat;

        [MenuItem("Eyesblack/检查场景...", false, 0)]
        private static void Init() {
            WndSceneChecker wnd = ScriptableObject.CreateInstance<WndSceneChecker>();
            wnd.Show();
        }

        void OnEnable() {
            titleContent.text = "检查场景";
        }

        void OnDestroy() {
            if (_memoryStat != null)
                _memoryStat.ClearAllCaches();
        }

        void OnGUI() {
            _checkMemories = EditorGUILayout.Toggle("检查内存", _checkMemories);
            _checkHiddenObjects = EditorGUILayout.Toggle("查找隐藏对象", _checkHiddenObjects);
            _checkComponents = EditorGUILayout.Toggle("检查组件数据", _checkComponents);
            _checkMeshData = EditorGUILayout.Toggle("检查Mesh数据", _checkMeshData);

            if (GUILayout.Button("检查")) {
                CheckScene();
            }
        }

        private void CheckScene() {
            EditorToolUtility.ClearConsoleLogs();


            List<GameObject> allGameObjects = GetAllGameObjectsInScene();

            if (_checkMemories)
                CheckMemories();
            if (_checkHiddenObjects)
                CheckHiddenGameObjects(allGameObjects);
            if (_checkComponents)
                CheckComponents(allGameObjects);
            if (_checkMeshData)
                CheckMeshes();
        }


        void CheckHiddenGameObjects(List<GameObject> allGameObjects) {
            foreach (GameObject go in allGameObjects) {
                if (!go.activeInHierarchy && !go.CompareTag("EditorOnly") && !IsLightObject(go)) {
                    Debug.Log(go.name + "：隐藏对象建议标记为EditorOnly或直接删除", go);
                }
            }
        }

        void CheckComponents(List<GameObject> allGameObjects) {
            foreach (GameObject go in allGameObjects) {
                MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
                bool hasRenderer = meshRenderer != null && meshRenderer.enabled;
                if (meshRenderer != null && !meshRenderer.enabled) {
                    Debug.Log(go.name + "：这是个不可见的对象，建议删除多余的MeshRenderer组件", go);
                }

                if (!hasRenderer) {
                    MeshFilter meshFilter = go.GetComponent<MeshFilter>();
                    if (meshFilter != null) {
                        Debug.Log(go.name + "：这是个不可见的对象，建议删除多余的MeshFilter组件", go);
                    }
                }


                Animator animator = go.GetComponent<Animator>();
                if (animator != null && animator.runtimeAnimatorController == null) {
                    Debug.Log(go.name + "：此对象不包含动画，建议删除Animator组件", go);
                }
            }
        }


        void CheckMeshes() {
            List<Mesh> meshesChecked = new List<Mesh>();
            foreach (MeshRenderer meshRenderer in Object.FindObjectsOfType<MeshRenderer>()) {
                MeshFilter meshFilter = meshRenderer.GetComponent<MeshFilter>();
                if (meshFilter != null) {
                    Mesh mesh = meshFilter.sharedMesh;
                    if (mesh.tangents.Length > 0 && !meshesChecked.Contains(mesh)) {
                        meshesChecked.Add(mesh);
                        bool useBumpTexture = false;
                        var dependencies = EditorUtility.CollectDependencies(new UnityEngine.Object[] { meshRenderer });
                        foreach (Object obj in dependencies) {
                            Texture2D texture = obj as Texture2D;
                            if (texture != null) {
                                if (IsBumpTexture(texture)) {
                                    useBumpTexture = true;
                                    break;
                                }
                            }
                        }

                        if (!useBumpTexture) {
                            Debug.Log(mesh.name + "：不需要导入切线数据", mesh);
                        }
                    }
                }
            }

            foreach (SkinnedMeshRenderer skinnedRenderer in Object.FindObjectsOfType<SkinnedMeshRenderer>()) {
                Mesh mesh = skinnedRenderer.sharedMesh;
                if (mesh.tangents.Length > 0 && !meshesChecked.Contains(mesh)) {
                    meshesChecked.Add(mesh);
                    bool useBumpTexture = false;
                    var dependencies = EditorUtility.CollectDependencies(new UnityEngine.Object[] { skinnedRenderer });
                    foreach (Object obj in dependencies) {
                        Texture2D texture = obj as Texture2D;
                        if (texture != null) {
                            if (IsBumpTexture(texture)) {
                                useBumpTexture = true;
                                break;
                            }
                        }
                    }

                    if (!useBumpTexture) {
                        Debug.Log(mesh.name + "：不需要导入切线数据", mesh);
                    }
                }       
            }

            foreach (Mesh mesh in meshesChecked) {
                if (mesh.colors.Length > 0) {
                    Debug.Log(mesh.name + "：含有顶点颜色，如果不需要可以使用“删除FBX顶点颜色”功能删除！", mesh);
                }
            }
        }

        void CheckMemories() {
            if (_memoryStat == null)
                _memoryStat = new MemoryStat();

            _memoryStat.DoStat(true);
            long texturesMemories = _memoryStat.CalcTextureMemories();
            long meshesCombinedMemories = _memoryStat.CalcMeshesCombinedMemories();
            long meshesUncombinedMemories = _memoryStat.CalcMeshesUncombinedMemories();
            long totalMemories = _memoryStat.CalcTotalMemories(texturesMemories, meshesCombinedMemories, meshesUncombinedMemories, 0);
            Debug.Log("场景总内存/Mesh占用/贴图占用: " + MemoryStat.GetMemoriesString(totalMemories) + "/" + MemoryStat.GetMemoriesString(meshesCombinedMemories + meshesUncombinedMemories) + "/" + MemoryStat.GetMemoriesString(texturesMemories) + "。 详情请运行场景内存统计工具查看！");
        }

        bool IsLightObject(GameObject go) {
            return go.GetComponent<Light>() != null;
        }

        bool IsBumpTexture(Texture2D texture) {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;
            return importer.normalmap;
        }

        List<GameObject> GetAllGameObjectsInScene() {
            List<GameObject> allGameObjects = new List<GameObject>();
            foreach (GameObject root in MemoryStat.GetAllRootGamObjects()) {
                GetAllChildren(allGameObjects, root);
            }
            return allGameObjects;
        }

        void GetAllChildren(List<GameObject> allGameObjects, GameObject parent) {
            allGameObjects.Add(parent);
            for (int i = 0; i < parent.transform.childCount; i++) {
                Transform child = parent.transform.GetChild(i);
                GetAllChildren(allGameObjects, child.gameObject);
            }
        }
    }
}