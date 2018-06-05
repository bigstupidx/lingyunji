using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;

namespace Eyesblack.EditorTools {
    public class MemoryStat {
        public class MeshInfo {
            public long memorySize;
            public long verticesTotal;
            public int usedCount;
        }

        public GameObject _rootGo;

        static List<Transform> _allChildrenTransfroms = new List<Transform>();

        public Dictionary<Mesh, MeshInfo> _meshesCombined = new Dictionary<Mesh, MeshInfo>();
        public Dictionary<Mesh, int> _meshesUncombined = new Dictionary<Mesh, int>();
        public Dictionary<Texture, int> _activeTextures = new Dictionary<Texture, int>();
        public Dictionary<AnimationClip, int> _animationClips = new Dictionary<AnimationClip, int>();


        public long CalcTotalMemories() {
            long texturesMemories = CalcTextureMemories();
            long meshesCombinedMemories = CalcMeshesCombinedMemories();
            long meshesUncombinedMemories = CalcMeshesUncombinedMemories();
            long animationClipsMemories = CalcAnimationClipsMemories();
            long totalMemories = CalcTotalMemories(texturesMemories, meshesCombinedMemories, meshesUncombinedMemories, animationClipsMemories);
            return totalMemories;
        }

        public long CalcTotalMemories(long texturesMemories, long meshesCombinedMemories, long meshesUncombinedMemories, long animationClipsMemories) {
            long totalMemories = texturesMemories + (meshesCombinedMemories + meshesUncombinedMemories) + animationClipsMemories;
            return totalMemories;
        }

        public long CalcTextureMemories() {
            long memoryTotal = 0;
            foreach (KeyValuePair<Texture, int> kvp in _activeTextures)
                memoryTotal += kvp.Value;
            return memoryTotal;
        }

        public long CalcMeshesCombinedMemories() {
            long memoryTotal = 0;
            foreach (KeyValuePair<Mesh, MeshInfo> kvp in _meshesCombined)
                memoryTotal += kvp.Value.memorySize;
            return memoryTotal;
        }

        public long CalcMeshesUncombinedMemories() {
            long memoryTotal = 0;
            foreach (KeyValuePair<Mesh, int> kvp in _meshesUncombined)
                memoryTotal += kvp.Value;
            return memoryTotal;
        }

        public long CalcAnimationClipsMemories() {
            long memoryTotal = 0;
            foreach (KeyValuePair<AnimationClip, int> kvp in _animationClips)
                memoryTotal += kvp.Value;
            return memoryTotal;
        }


        public static string GetMemoriesString(long byteSize) {
            string memorySizeStr = byteSize >= 1048576 ? byteSize / 1048576.0f + "MB" : byteSize / 1024.0f + "KB";
            return memorySizeStr;
        }


        public void ClearAllCaches() {
            _allChildrenTransfroms.Clear();

            _meshesCombined.Clear();
            _meshesUncombined.Clear();
            _activeTextures.Clear();
            _animationClips.Clear();

            EditorUtility.UnloadUnusedAssetsImmediate();
        }

        public void DoStat(bool isScene) {
            if (EditorApplication.isPlaying) {
                Debug.LogError("运行时不能检查内存！");
                return;
            }

            ClearAllCaches();

            if (isScene) {
                foreach (var root in GetAllRootGamObjects())
                    GetAllChildren(root.transform);
            }
            if (_rootGo)
                GetAllChildren(_rootGo.transform);

            DoMeshesStat();
            DoTexturesStat();
            if (isScene)
                DoEnvStat();
            //DoAnimationClipsStat();
        }

        private void DoMeshesStat() {
            foreach (Transform tr in _allChildrenTransfroms) {
                if (GameObjectUtility.AreStaticEditorFlagsSet(tr.gameObject, StaticEditorFlags.BatchingStatic)) {
                    if (tr.gameObject.activeSelf) {
                        //if (tr.GetComponent<YangStudio.DetailInfo>()) {
                        //AddMeshCombined(_grassVirtualMesh);
                        //} else {
                        MeshFilter meshFilter = tr.GetComponent<MeshFilter>();
                        if (meshFilter) {
                            AddMeshCombined(meshFilter.sharedMesh);
                        } else {
                            SkinnedMeshRenderer renderer = tr.GetComponent<SkinnedMeshRenderer>();
                            if (renderer)
                                AddMeshUncombined(renderer.sharedMesh);
                        }
                        //}
                    }
                } else {  //不合并的mesh，无论是否激活都会占用内存
                    MeshFilter meshFilter = tr.GetComponent<MeshFilter>();
                    if (meshFilter) {
                        AddMeshUncombined(meshFilter.sharedMesh);
                    } else {
                        SkinnedMeshRenderer renderer = tr.GetComponent<SkinnedMeshRenderer>();
                        if (renderer)
                            AddMeshUncombined(renderer.sharedMesh);
                    }
                }
            }
        }



        private void DoTexturesStat() {
            /*
            List<Material> activeMaterials = new List<Material>();
            foreach (Transform tr in _allChildrenTransfroms) {
                Renderer renderer = tr.GetComponent<Renderer>();
                if (renderer) {  //无论是否激活贴图都会占用内存
                    foreach (Material material in renderer.sharedMaterials) {
                        if (!activeMaterials.Contains(material))
                            activeMaterials.Add(material);
                    }
                }
            }

            foreach (Material material in activeMaterials) {
                var dependencies = EditorUtility.CollectDependencies(new UnityEngine.Object[] { material });
                foreach (Object obj in dependencies) {
                    if (obj is Texture) {
                        Texture texture = obj as Texture;
                        int ret = 0;
                        if (!_activeTextures.TryGetValue(texture, out ret)) {
                            int memSize = CalculateTextureSizeBytes(texture);
                            _activeTextures.Add(texture, memSize);
                        }
                    }

                }
            }
            */
            foreach (Transform tr in _allChildrenTransfroms) {
                var dependencies = EditorUtility.CollectDependencies(new UnityEngine.Object[] { tr.gameObject });
                foreach (Object obj in dependencies) {
                    if (obj is Texture) {
                        Texture texture = obj as Texture;
                        int ret = 0;
                        if (!_activeTextures.TryGetValue(texture, out ret)) {
                            int memSize = CalculateTextureSizeBytes(texture);
                            _activeTextures.Add(texture, memSize);
                        }
                    }
                }
            }
        }

        void DoEnvStat() {
            if (RenderSettings.skybox != null) {
                var dependencies = EditorUtility.CollectDependencies(new UnityEngine.Object[] { RenderSettings.skybox });
                foreach (Object obj in dependencies) {
                    if (obj is Texture) {
                        Texture texture = obj as Texture;
                        int ret = 0;
                        if (!_activeTextures.TryGetValue(texture, out ret)) {
                            int memSize = CalculateTextureSizeBytes(texture);
                            _activeTextures.Add(texture, memSize);
                        }
                    }

                }
            }

            for (int i = 0; i < LightmapSettings.lightmaps.Length; i++) {
                Texture2D texture = LightmapSettings.lightmaps[i].lightmapColor;
                if (texture) {
                    int ret = 0;
                    if (!_activeTextures.TryGetValue(texture, out ret)) {
                        int memSize = CalculateTextureSizeBytes(texture);
                        _activeTextures.Add(texture, memSize);
                    }
                }
            }
        }


        private void DoAnimationClipsStat() {
            foreach (Transform tr in _allChildrenTransfroms) {
                Animation animation = tr.GetComponent<Animation>();
                if (animation != null) {
                    foreach (AnimationState state in animation) {
                        int ret = 0;
                        if (!_animationClips.TryGetValue(state.clip, out ret)) {
                            int memSize = Profiler.GetRuntimeMemorySize(state.clip);
                            _animationClips.Add(state.clip, memSize);
                        }
                    }
                } else {
                    Animator animator = tr.GetComponent<Animator>();
                    if (animator != null) {
                        UnityEditor.Animations.AnimatorController ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
                        if (ac != null) {
                            foreach (AnimationClip clip in ac.animationClips) {
                                int ret = 0;
                                if (!_animationClips.TryGetValue(clip, out ret)) {
                                    int memSize = Profiler.GetRuntimeMemorySize(clip);
                                    _animationClips.Add(clip, memSize);
                                }
                            }
                        }
                    }
                }
            }
        }


        //会返回未激活对象
        private void GetAllChildren(Transform parent) {
            // if (parent.gameObject.tag == "EditorOnly")
            //     return;
            _allChildrenTransfroms.Add(parent);

            for (int i = 0; i < parent.childCount; i++) {
                Transform child = parent.GetChild(i);
                GetAllChildren(child);
            }
        }

        public static GameObject[] GetAllRootGamObjects() {
            return EditorSceneManager.GetActiveScene().GetRootGameObjects();
            /*
            var prop = new HierarchyProperty(HierarchyType.GameObjects);
            var expanded = new int[0];
            while (prop.Next(expanded)) {
                yield return prop.pptrValue as GameObject;
            }
             */
        }


        private void AddMeshCombined(Mesh mesh) {
            if (mesh) {
                MeshInfo meshInfo;
                if (!_meshesCombined.TryGetValue(mesh, out meshInfo)) {
                    meshInfo = new MeshInfo();
                    _meshesCombined.Add(mesh, meshInfo);
                }

                // meshInfo.memorySize += Profiler.GetRuntimeMemorySize(mesh);
                meshInfo.memorySize += CalcMeshSizeBytes(mesh);
                meshInfo.verticesTotal += mesh.vertexCount;
                meshInfo.usedCount++;
            }
        }

        private void AddMeshUncombined(Mesh mesh) {
            if (mesh) {
                int memSize = 0;
                if (!_meshesUncombined.TryGetValue(mesh, out memSize)) {
                    _meshesUncombined.Add(mesh, CalcMeshSizeBytes(mesh));
                }
            }
        }


        public static int CalcMeshSizeBytes(Mesh mesh) {
            int size = 0;
            size += mesh.vertices.Length * 12;
            size += mesh.normals.Length * 12;
            size += mesh.tangents.Length * 12;
            size += mesh.colors.Length * 12;
            size += mesh.uv.Length * 8;
            size += mesh.uv2.Length * 8;

            size += mesh.triangles.Length * 2;

            return size * 2;
        }


        private int CalculateTextureSizeBytes(Texture tTexture) {

            int tWidth = tTexture.width;
            int tHeight = tTexture.height;
            if (tTexture is Texture2D) {
                Texture2D tTex2D = tTexture as Texture2D;
                int bitsPerPixel = GetBitsPerPixel(tTex2D.format);
                int mipMapCount = tTex2D.mipmapCount;
                int mipLevel = 1;
                int tSize = 0;
                while (mipLevel <= mipMapCount) {
                    tSize += tWidth * tHeight * bitsPerPixel / 8;
                    tWidth = tWidth / 2;
                    tHeight = tHeight / 2;
                    mipLevel++;
                }
                return tSize;
            }

            if (tTexture is Cubemap) {
                Cubemap tCubemap = tTexture as Cubemap;
                int bitsPerPixel = GetBitsPerPixel(tCubemap.format);
                int mipMapCount = tCubemap.mipmapCount;
                int mipLevel = 1;
                int tSize = 0;
                while (mipLevel <= mipMapCount) {
                    tSize += tWidth * tHeight * bitsPerPixel / 8;
                    tWidth = tWidth / 2;
                    tHeight = tHeight / 2;
                    mipLevel++;
                }
                tSize *= 6;
                return tSize;
            }
            return 0;
        }

        private int GetBitsPerPixel(TextureFormat format) {
            switch (format) {
                case TextureFormat.Alpha8: //	 Alpha-only texture format.
                    return 8;
                case TextureFormat.ARGB4444: //	 A 16 bits/pixel texture format. Texture stores color with an alpha channel.
                    return 16;
                case TextureFormat.RGBA4444: //	 A 16 bits/pixel texture format.
                    return 16;
                case TextureFormat.RGB24:   // A color texture format.
                    return 24;
                case TextureFormat.RGBA32:  //Color with an alpha channel texture format.
                    return 32;
                case TextureFormat.ARGB32:  //Color with an alpha channel texture format.
                    return 32;
                case TextureFormat.RGB565:  //	 A 16 bit color texture format.
                    return 16;
                case TextureFormat.DXT1:    // Compressed color texture format.
                    return 4;
                case TextureFormat.DXT5:    // Compressed color with alpha channel texture format.
                    return 8;
                /*
                case TextureFormat.WiiI4:	// Wii texture format.
                case TextureFormat.WiiI8:	// Wii texture format. Intensity 8 bit.
                case TextureFormat.WiiIA4:	// Wii texture format. Intensity + Alpha 8 bit (4 + 4).
                case TextureFormat.WiiIA8:	// Wii texture format. Intensity + Alpha 16 bit (8 + 8).
                case TextureFormat.WiiRGB565:	// Wii texture format. RGB 16 bit (565).
                case TextureFormat.WiiRGB5A3:	// Wii texture format. RGBA 16 bit (4443).
                case TextureFormat.WiiRGBA8:	// Wii texture format. RGBA 32 bit (8888).
                case TextureFormat.WiiCMPR:	//	 Compressed Wii texture format. 4 bits/texel, ~RGB8A1 (Outline alpha is not currently supported).
                    return 0;  //Not supported yet
                */
                case TextureFormat.PVRTC_RGB2://	 PowerVR (iOS) 2 bits/pixel compressed color texture format.
                    return 2;
                case TextureFormat.PVRTC_RGBA2://	 PowerVR (iOS) 2 bits/pixel compressed with alpha channel texture format
                    return 2;
                case TextureFormat.PVRTC_RGB4://	 PowerVR (iOS) 4 bits/pixel compressed color texture format.
                    return 4;
                case TextureFormat.PVRTC_RGBA4://	 PowerVR (iOS) 4 bits/pixel compressed with alpha channel texture format
                    return 4;
                case TextureFormat.ETC_RGB4://	 ETC (GLES2.0) 4 bits/pixel compressed RGB texture format.
                    return 4;
                case TextureFormat.ETC2_RGB:
                    return 4;
                case TextureFormat.ETC2_RGBA8:
                    return 8;
                case TextureFormat.ATC_RGB4://	 ATC (ATITC) 4 bits/pixel compressed RGB texture format.
                    return 4;
                case TextureFormat.ATC_RGBA8://	 ATC (ATITC) 8 bits/pixel compressed RGB texture format.
                    return 8;
                case TextureFormat.BGRA32://	 Format returned by iPhone camera
                    return 32;
#if !(UNITY_5 || UNITY_2017)
            case TextureFormat.ATF_RGB_DXT1://	 Flash-specific RGB DXT1 compressed color texture format.
                case TextureFormat.ATF_RGBA_JPG://	 Flash-specific RGBA JPG-compressed color texture format.
                case TextureFormat.ATF_RGB_JPG://	 Flash-specific RGB JPG-compressed color texture format.
                    return 0; //Not supported yet  
#endif
            }
            return 0;
        }
    }



    public class WndSceneMemoryStat : EditorWindow {
        private MemoryStat _memoryStat;

        Vector2 _textureListScrollPos = new Vector2(0, 0);
        Vector2 _meshListScrollPos = new Vector2(0, 0);
        Vector2 _animationClipsListScrollPos = new Vector2(0, 0);

        static string[] _inspectToolbarStrings = { "Textures", "Meshes"/*, "Animation"*/ };
        int _currentPage = 0;

        [MenuItem("Eyesblack/场景内存统计", false, 1)]
        private static void Init() {
            WndSceneMemoryStat window = (WndSceneMemoryStat)EditorWindow.GetWindow(typeof(WndSceneMemoryStat));
            window.InitData();
        }

        void OnEnable() {
            titleContent.text = "场景内存统计";
        }

        void OnDestroy() {
            _memoryStat.ClearAllCaches();
        }

        void InitData() {
            _memoryStat = new MemoryStat();
        }


        void OnGUI() {
            //_memoryStat._rootGo = EditorGUILayout.ObjectField("Root Object", _memoryStat._rootGo, typeof(GameObject), true) as GameObject;

            if (GUILayout.Button("Start")) {
                _memoryStat.DoStat(true);
            }


            long texturesMemories = _memoryStat.CalcTextureMemories();
            long meshesCombinedMemories = _memoryStat.CalcMeshesCombinedMemories();
            long meshesUncombinedMemories = _memoryStat.CalcMeshesUncombinedMemories();
            long animationClipsMemories = _memoryStat.CalcAnimationClipsMemories();
            long totalMemories = _memoryStat.CalcTotalMemories(texturesMemories, meshesCombinedMemories, meshesUncombinedMemories, animationClipsMemories);

            GUILayout.Label("估计总内存：" + MemoryStat.GetMemoriesString(totalMemories));
            GUILayout.Label("贴图占用：" + MemoryStat.GetMemoriesString(texturesMemories));
            GUILayout.Label("Mesh占用：" + MemoryStat.GetMemoriesString(meshesCombinedMemories + meshesUncombinedMemories) + " (其中未合并Mesh占用：" + MemoryStat.GetMemoriesString(meshesUncombinedMemories) + ")");
           // GUILayout.Label("动画占用：" + MemoryStat.GetMemoriesString(animationClipsMemories));


            _currentPage = GUILayout.Toolbar(_currentPage, _inspectToolbarStrings);
            switch (_currentPage) {
                case 0:
                    GUILayout.Label("(显示格式 —— 贴图名: 占用内存大小)");
                    ListTextures();
                    break;

                case 1:
                    GUILayout.Label("(以下为合并mesh信息        显示格式 —— Mesh名: 占用内存大小/总顶点数/使用次数)");
                    ListMeshes();
                    break;
                /*
                case 2:
                    GUILayout.Label("(显示格式 —— 动画名: 占用内存大小)");
                    ListAnimationClips();
                    break;
                */
            }
        }


        private void ListTextures() {
            _textureListScrollPos = EditorGUILayout.BeginScrollView(_textureListScrollPos);

            var dicSort = from objDic in _memoryStat._activeTextures orderby objDic.Value descending select objDic;
            foreach (KeyValuePair<Texture, int> kvp in dicSort) {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(kvp.Key.name, GUILayout.Width(300));
                GUILayout.Label(MemoryStat.GetMemoriesString(kvp.Value), GUILayout.Width(100));
                if (GUILayout.Button("定位", GUILayout.Width(50)))
                    Selection.objects = new Object[] { kvp.Key };
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        private void ListMeshes() {
            _meshListScrollPos = EditorGUILayout.BeginScrollView(_meshListScrollPos);


            var dicSort = from objDic in _memoryStat._meshesCombined orderby objDic.Value.memorySize descending select objDic;
            foreach (KeyValuePair<Mesh, MemoryStat.MeshInfo> kvp in dicSort) {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(kvp.Key.name, GUILayout.Width(300));
                GUILayout.Label(MemoryStat.GetMemoriesString(kvp.Value.memorySize) + "/" + kvp.Value.verticesTotal + "/" + kvp.Value.usedCount);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("定位", GUILayout.Width(50)))
                    Selection.objects = new Object[] { kvp.Key };
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }


        private void ListAnimationClips() {
            _animationClipsListScrollPos = EditorGUILayout.BeginScrollView(_animationClipsListScrollPos);

            var dicSort = from objDic in _memoryStat._animationClips orderby objDic.Value descending select objDic;
            foreach (KeyValuePair<AnimationClip, int> kvp in dicSort) {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(kvp.Key.name, GUILayout.Width(300));
                GUILayout.Label(MemoryStat.GetMemoriesString(kvp.Value), GUILayout.Width(100));
                if (GUILayout.Button("定位", GUILayout.Width(50)))
                    Selection.objects = new Object[] { kvp.Key };
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }





}


