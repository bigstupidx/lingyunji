#if UNITY_EDITOR && USE_RESOURCESEXPORT
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XTools;
using System.IO;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace PackTool
{
    public partial class AssetsExport : IAssetsExport
    {
        public AssetsExport(bool isExportAtlas = false)
        {
            isDone = true;
            AssetList = new Dictionary<string, int>[(int)ResourceType.Max];
            for (int i = 0; i < AssetList.Length; ++i)
                AssetList[i] = new Dictionary<string, int>();

            this.isExportAtlas = isExportAtlas;
        }

        public bool isEmpty
        {
            get
            {
                for (int i = 0; i < AssetList.Length; ++i)
                {
                    if (AssetList[i] != null && AssetList[i].Count > 0)
                        return false;
                }

                if (mBuiltinResources.Count != 0)
                    return false;

                return true;
            }
        }

        public bool isPause = false;
        public bool isStop = false;

        //public void CollectAtlas(UI.Atlas atlas)
        //{
        //    AddObject(ResourceType.Atlas, atlas);
        //}

        //public void CollectAtlas(ATLAS atlas)
        //{
        //    AddObject(ResourceType.Atlas, atlas);
        //}

        public void CollectMaterial(Material mat)
        {
            CollectMaterial(mat, this);
        }

        public void CollectPrefab(GameObject prefab)
        {
            CollectPrefab(prefab, this);
        }

        public void CollectScene(Object scene)
        {
            CollectScene(scene, this);
        }

        public List<Object> mBuiltinResources = new List<Object>();
        public List<Material> mMaterialResources = new List<Material>();

        // 收集u3d的内置资源
        public void CollectBuiltinResource(Object obj)
        {
            if (obj == null)
                return;

            if (!mBuiltinResources.Contains(obj))
            {
                mBuiltinResources.Add(obj);
            }
        }

        public void CollectBuiltinMaterial(Material obj)
        {
            if (obj == null)
                return;

            mMaterialResources.Add(obj);
        }

        public void CollectTexture(Texture t)
        {
            AddTexture(t);
        }

        public void CollectTMPFont(TMPro.TMP_FontAsset fontasset)
        {
            CollectTMPFont(fontasset, this);
        }

        public void CollectTexture2DAsset(Texture2DAsset asset)
        {
            CollectTexture2DAsset(asset, this);
        }

        public void CollectMesh(Mesh mesh)
        {
            CollectMesh(mesh, this);
        }

        public void CollectSprite(Sprite sprite)
        {

        }

        public Component current { get; set; }

        static T CopyObjectClip<T>(T obj, string suffix) where T : Object
        {
            string path = GetObjectCopyPath(obj, suffix);

            Object copy = AssetDatabase.LoadAssetAtPath(path, typeof(T));
            if (copy != null && !IsNeedUpdate(AssetDatabase.GetAssetPath(obj).Substring(7)))
            {
                // 不为空,并且不需要更新
                return copy as T;
            }

            copy = Object.Instantiate(obj);
            System.IO.Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('/')));

            if (copy is Mesh)
                ((Mesh)copy).colors = new Color[0];

            AssetDatabase.CreateAsset(copy, path);
            return AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
        }

        public static Object GetSrcAnim(AnimationClip clip)
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(clip);
            if (path.ToLower().EndsWith(".anim"))
                return clip;

            return UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
        }

        void AddAnimator(RuntimeAnimatorController anim)
        {
            AddObject(ResourceType.Animator, anim);
        }

        void AddAvatar(Avatar avatar)
        {
            AddObject(ResourceType.Avatar, avatar);
        }

        void AddLightProbes(LightProbes lightProbes)
        {
            AddObject(ResourceType.LightProbes, lightProbes);
        }

        public void CollectLightProbes(LightProbes lightProbes)
        {
            if (isCollect(lightProbes))
                return;
            AddCollect(lightProbes);

            //AddLightProbes(lightProbes);
            ExportLightProbes(lightProbes);
        }

        public void CollectAvatar(Avatar avatar)
        {
            CollectAvatar(avatar, this);
        }

        public static void CollectAvatar(Avatar avatar, AssetsExport mgr)
        {
            if (mgr.isCollect(avatar))
                return;
            mgr.AddCollect(avatar);

            avatar = CreateAvatar(avatar);
            mgr.AddAvatar(avatar);
        }

        static void CollectAnimator(RuntimeAnimatorController rac, AssetsExport mgr)
        {
            if (mgr.isCollect(rac))
                return;

            mgr.AddCollect(rac);
            //rac = CreateAnimatorController(rac, mgr);
            mgr.AddAnimator(rac);
        }

        public void CollectAnimator(RuntimeAnimatorController rac)
        {
            CollectAnimator(rac, this);
        }

        public void CollectAnimation(AnimationClip clip)
        {
            CollectAnimation(clip, this);
        }

        static void CollectAnimation(AnimationClip clip, AssetsExport mgr)
        {
            if (mgr.isCollect(clip))
                return;

            mgr.AddCollect(clip);
            //             string path = AssetDatabase.GetAssetPath(clip);
            //             if (!path.ToLower().EndsWith(".anim"))
            //                 clip = CopyObjectClip<AnimationClip>(clip, ".anim");

            mgr.AddAnimation(clip);
        }

        static void CollectMesh(Mesh mesh, AssetsExport mgr)
        {
            if (mgr.isCollect(mesh))
                return;

            mgr.AddCollect(mesh);
            //             string path = AssetDatabase.GetAssetPath(mesh);
            //             if (!path.ToLower().EndsWith(".asset"))
            //                 mesh = CopyObjectClip<Mesh>(mesh, ".asset");

            mgr.AddMesh(mesh);
        }

        public void CollectSound(AudioClip clip)
        {
            AddSound(clip);
        }

        public void CollectFontlib(Font font)
        {
            AddFontlib(font);
        }

        // 收集要打包的资源
        Dictionary<string, int>[] AssetList;

        Dictionary<string, int> GetObjList(ResourceType type)
        {
            return AssetList[(int)type];
        }

        // 添加场景
        public void AddScene(Object scene)
        {
            AddObject(ResourceType.Scene, scene);
        }

        List<Mesh> MeshList = new List<Mesh>();
        List<AnimationClip> AnimationClipList = new List<AnimationClip>();

        void AddObject(ResourceType type, Object obj)
        {
            Dictionary<string, int> objs = GetObjList(type);
            string assetPath = GetAssetPath(obj);
            int num = 0;
            if (objs.TryGetValue(assetPath, out num))
                objs[assetPath] = num + 1;
            else
                objs[assetPath] = 1;
        }

        void AddMaterial(Material mat)
        {
            AddObject(ResourceType.Material, mat);
        }

        void AddPrefab(GameObject go)
        {
            AddObject(ResourceType.Prefab, go);
        }

        void AddTMPFontlib(TMPro.TMP_FontAsset font)
        {
            AddObject(ResourceType.TMPFont, font);
        }

        void AddT2DAsset(Texture2DAsset asset)
        {
            AddObject(ResourceType.T2dAsset, asset);
        }

        void AddTexture(Texture texture)
        {
            AddObject(ResourceType.Texture, texture);
        }

        void AddMesh(Mesh mesh)
        {
            MeshList.Add(mesh);
            AddObject(ResourceType.Mesh, mesh);
        }

        void AddAnimation(AnimationClip anim)
        {
            AnimationClipList.Add(anim);
            AddObject(ResourceType.Animation, anim);
        }

        void AddSound(AudioClip clip)
        {
            AddObject(ResourceType.Sound, clip);
        }

        void AddFontlib(Font font)
        {
            AddObject(ResourceType.Fontlib, font);
        }

        // 已经加载的资源
        Dictionary<string, int> AlreadCollectList = new Dictionary<string, int>();

        static string GetAssetPath(Object obj)
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path))
            {
                Debuger.LogError("path == null");
            }

            return path.Substring(7);
        }

        static string GetObjectKey(Object obj)
        {
            string assetPath = GetAssetPath(obj);
            if (obj is Mesh || obj is Avatar)
            {
                return string.Format("{0}:{1}:{2}{3}", assetPath, obj.GetType().Name, obj.name, GetObjArraySuffix("Assets/" + assetPath, obj));
            }
            else
            {
                return GetAssetPath(obj) + ":" + obj.GetType().Name + obj.name;
            }
        }

        // 是否已经收集了此资源的依赖
        bool isCollect(Object obj)
        {
            if (AlreadCollectList.ContainsKey(GetObjectKey(obj)))
                return true;

            return false;
        }

        void AddCollect(Object obj)
        {
            AlreadCollectList.Add(GetObjectKey(obj), 1);
        }

        // 是否显示日志
        public bool ShowLog = true;

        // 开始打包
        public bool isDone { get; protected set; }

        [MenuItem("Assets/ExportAllAtlas")]
        static void ExportAllAtlas()
        {
            //             string[] policies = UnityEditor.Sprites.Packer.Policies;
            //             Debug.LogFormat("policies:{0}", policies.Length);
            //             for (int i = 0; i < policies.Length; ++i)
            //                 Debug.LogFormat("{0}):{1}", i, policies[i]);

            AssetsExport ae = new AssetsExport(true);
            GlobalCoroutine.StartCoroutine(ae.BeginPack());
        }

        static void CheckFile(string file)
        {
            if (!File.Exists(file))
            {
                Debug.LogErrorFormat("not find file:{0}", file);
                return;
            }

            Debug.LogFormat("file:{0}", file);
            AssetBundle ab = AssetBundle.LoadFromFile(file);
            try
            {
                xys.UI.Atlas atlas = ab.mainAsset as xys.UI.Atlas;
                List<TextureFormat> tf = new List<TextureFormat>();
                foreach (Sprite s in atlas.Sprites)
                {
                    if (!tf.Contains(s.texture.format))
                        tf.Add(s.texture.format);
                }

                for (int i = 0; i < tf.Count; ++i)
                    Debug.LogFormat("{0}):{1}", i, tf[i]);
            }
            finally
            {
                ab.Unload(true);
            }
        }

        [MenuItem("Assets/CheckFormat")]
        static void CheckFormat()
        {
            CheckFile(ResourcesPath.LocalDataPath + "Art/UIData/UData/Atlas/Common.prefab");
            CheckFile(ResourcesPath.LocalDataPath + "Art/UIData/UData/Atlas/Common_astc.prefab");
        }

        IEnumerator ExportPrefabItor()
        {
            var itor = ExportAsset<GameObject>(GetObjList(ResourceType.Prefab), PrefabExport);
            while (itor.MoveNext())
                yield return 0;

            yield return 0;
            for (int i = 0; i < ExportPrefabs.Count; ++i)
            {
                ExportPrefab(AssetDatabase.LoadAssetAtPath<GameObject>(ExportPrefabs[i]));
                yield return 0;
            }
            ExportPrefabs.Clear();
        }

        IEnumerator ExportAtlasItor()
        {
            if (!isExportAtlas)
                yield break;

            // Atlas资源
            List<string> normal_atlas = new List<string>();
#if UNITY_IOS
            List<string> normal_astc_atlas = new List<string>();
#endif
            Utility.ForEach(xys.UI.CheckAllPanel.atlas_root, (AssetImporter ai) => { }, (string assetPath, string root) =>
            {
                if (!assetPath.EndsWith(".prefab", true, null))
                    return false;

#if UNITY_IOS
                if (assetPath.EndsWith("_astc.prefab", true, null))
                    normal_astc_atlas.Add(assetPath);
                else
#endif
                    normal_atlas.Add(assetPath);

                return false;
            });

            var ator = ExportAtlasByAssetPaths(normal_atlas, "SpritePacker");
            while (ator.MoveNext())
                yield return 0;

#if UNITY_IOS
            ator = ExportAtlasByAssetPaths(normal_astc_atlas, "SpritePackerAstc");
            while (ator.MoveNext())
                yield return 0;
#endif
        }

        static IEnumerator ExportAtlasByAssetPaths(List<string> atlas, string policy)
        {
            using (new SetSpritePackerMode(SpritePackerMode.AlwaysOn))
            {
                using (new SetSelectedPolicy(policy))
                {
                    yield return 0;
                    Debug.LogFormat("ExportAtlas:{0}", UnityEditor.Sprites.Packer.SelectedPolicy);
                    yield return 0;
                    EditorSettings.spritePackerPaddingPower = 3;
                    UnityEditor.Sprites.Packer.RebuildAtlasCacheIfNeeded(GetBuildTarget(), false, UnityEditor.Sprites.Packer.Execution.ForceRegroup);

                    int i = 0;
                    foreach (var itor in atlas)
                    {
                        Debug.LogFormat("{0}):{1}", ++i, itor);
                        var ator = ExportAtlas(AssetDatabase.LoadAssetAtPath<xys.UI.Atlas>(itor));
                        while (ator.MoveNext())
                            yield return 0;
                    }
                }
            }

            yield return 0;
        }

        static void GenAtlasConfig()
        {
            xys.UI.CheckAllPanel.StartCheckAllPanel().CreateAtlas();
            List<string> paths = new List<string>();
            Utility.ForEach(xys.UI.CheckAllPanel.atlas_root, (AssetImporter ai) => { }, (string assetPath, string root) =>
            {
#if UNITY_IOS
                if (assetPath.EndsWith("_astc.prefab", true, null))
                    return false;
#endif
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                xys.UI.Atlas atlas = null;
                if (obj != null && ((atlas = obj.GetComponent<xys.UI.Atlas>()) != null))
                {
                    paths.Add(assetPath.Substring(root.Length + 1));
                }

                return false;
            });

            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            writer.Write(paths.Count);
            for (int i = 0; i < paths.Count; ++i)
            {
                writer.Write(paths[i]);
                AssetDatabase.LoadAssetAtPath<xys.UI.Atlas>(xys.UI.CheckAllPanel.atlas_root + paths[i]).Write(writer);
            }

            Directory.CreateDirectory(PackPath);
            File.WriteAllBytes(PackPath + "sprites_atlas.b", ms.ToArray());
            ms.Close();
            writer.Close();
        }

        // 是否导出图集
        public bool isExportAtlas { get; private set; }

        public IEnumerator BeginPack()
        {
            var spritePackerMode = EditorSettings.spritePackerMode;
            if (spritePackerMode != SpritePackerMode.Disabled)
                EditorSettings.spritePackerMode = SpritePackerMode.Disabled;

            if (isExportAtlas)
                GenAtlasConfig();

            CacheList.Clear();
            string currentScene = EditorSceneManager.GetActiveScene().path;

            TimeCheck timeCheck = new TimeCheck();
            timeCheck.begin();
            isStop = false;
            isDone = false;

            System.Func<IEnumerator>[] allfuns = new System.Func<IEnumerator>[] 
            {
                ExportPrefabItor, // 导预置体
                ()=> { return ExportAsset<TMPro.TMP_FontAsset>(GetObjList(ResourceType.TMPFont), ExportTMPFont); },
                ()=> { return ExportAsset<Texture2DAsset>(GetObjList(ResourceType.T2dAsset), ExportTexture2DAsset); }, // 可读纹理资源
                ExportAtlasItor, // 导图集
                ()=> { return ExportAsset<Material>(GetObjList(ResourceType.Material), ExportMaterial); }, // 材质
                ()=> { return ExportAsset<Mesh>(MeshList, MeshExport); }, // 网格
                ()=> { return ExportAsset<AnimationClip>(AnimationClipList, AnimationClipExport); },
                ()=> { return ExportAsset<Font>(GetObjList(ResourceType.Fontlib), ExportFontLib); },
                ()=> { return ExportAsset<Texture>(GetObjList(ResourceType.Texture), ExportTexture); },
                ()=> { return ExportAsset<AudioClip>(GetObjList(ResourceType.Sound), ExportSound); },
                ()=> { return ExportAsset<RuntimeAnimatorController>(GetObjList(ResourceType.Animator), ExportAnimator); },
                ()=> { return ExportAsset<Avatar>(GetObjList(ResourceType.Avatar), ExportAvatar); },
                ()=> { return ExportAsset<Object>(GetObjList(ResourceType.Scene), ExportScene); },
/*                ()=> { return ExportAsset<LightProbes>(GetObjList(ResourceType.LightProbes), ExportLightProbes); },*/
            };

            foreach (var fun in allfuns)
            {
                var ator = fun();
                while (ator.MoveNext())
                    yield return 0;
            }

            yield return 0;

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

            yield return 0;

            // 内置资源
            ExportBuiltinResources(mBuiltinResources, mMaterialResources);

            CodeCheckAtler.SaveToFile();

            Release();

            isDone = true;
            Debuger.Log("资源打包完成! 总耗时:" + timeCheck.delay);
            Logger.LogDebug("组件依赖信息:{0} ", ComponentSave.ToInfo());

            if (!string.IsNullOrEmpty(currentScene))
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(currentScene);
            }

            if (spritePackerMode != SpritePackerMode.Disabled)
                EditorSettings.spritePackerMode = spritePackerMode;

            yield return 0;
        }

        public void Release()
        {
            for (int i = 0; i < AssetList.Length; ++i)
                AssetList[i].Clear();

            AlreadCollectList.Clear();
            mBuiltinResources.Clear();
        }

        public delegate IEnumerator ExportObject<T>(T obj);

        IEnumerator ExportAsset<T>(List<T> objs, ExportObject<T> fun) where T : Object
        {
            string typename = typeof(T).Name;
            List<T> newobjs = new List<T>(objs);
            objs.Clear();
            int index = 1;
            int total = 0;
            TimeCheck tc = new TimeCheck(true);
            while (true)
            {
                total += newobjs.Count;
                int num = 0;
                foreach (T obj in newobjs)
                {
                    if (obj == null)
                    {
                        Debuger.DebugLog("第{0}轮: {1}:{2} {3}:{4}", index, typename, "(null)", num, newobjs.Count);
                        continue;
                    }

                    if (ShowLog)
                    {
                        Debuger.DebugLog("第{0}轮: {1}:{2} {3}:{4}", index, typename, obj.name, num, newobjs.Count);
                    }

                    bool has = false;
                    IEnumerator itor = fun(obj);

                    try
                    {
                        has = itor.MoveNext();
                    }
                    catch (System.Exception e)
                    {
                        isError = true;
                        Debuger.LogError("ExportAsset:" + obj.name);
                        Debuger.LogException(e);
                    }

                    while (has)
                    {
                        try
                        {
                            has = itor.MoveNext();
                        }
                        catch (System.Exception e)
                        {
                            isError = true;
                            Debuger.LogError("ExportAsset:" + obj.name);
                            Debuger.LogException(e);
                        }

                        if (has)
                        {
                            yield return 0;
                        }
                    }

                    num++;
                    if ((num % 50) == 0)
                        yield return 0;
                }

                if (objs.Count == 0)
                    break;

                ++index;
                newobjs = new List<T>(objs);
                objs.Clear();
            }

            Debuger.DebugLog("{0}: total:{1} totaltime:{2}", typename, total, tc.delay);
        }

        bool isError_ = false;

        public bool isError { get { return isError_; } protected set { isError_ = value; } }

        IEnumerator ExportAsset<T>(Dictionary<string, int> objs, ExportObject<T> fun) where T : Object
        {
            string typename = typeof(T).Name;
            T asset;
            Dictionary<string, int> newobjs = new Dictionary<string, int>(objs);
            objs.Clear();
            int index = 1;
            int total = 0;
            TimeCheck tc = new TimeCheck(true);
            while (true)
            {
                total += newobjs.Count;
                int num = 0;
                foreach (KeyValuePair<string, int> itor in newobjs)
                {
                    //                     while (isPause)
                    //                         yield return 0;
                    // 
                    //                     if (isStop)
                    //                         yield break;

                    if (ShowLog)
                    {
                        Debuger.DebugLog("第{0}轮: {1}:{2} {3}:{4}", index, typename, itor.Key, num, newobjs.Count);
                    }

                    string assetPath = "Assets/" + itor.Key;
                    asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                    if (asset == null)
                    {
                        if (!File.Exists(assetPath))
                        {
                            Debuger.ErrorLog("file {0} not find!", assetPath);
                            isError = true;
                        }
                        else
                        {
                            Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                            Debuger.ErrorLog("type:{0}->{2} path:{1} null!", typeof(T).Name, assetPath, obj.GetType().Name);
                        }
                    }
                    else
                    {
                        bool has = false;
                        IEnumerator ator = fun(asset);

                        try
                        {
                            has = ator.MoveNext();
                        }
                        catch (System.Exception e)
                        {
                            isError = true;
                            Debuger.LogError("ExportAsset:" + asset.name);
                            Debuger.LogException(e);
                        }

                        while (has)
                        {
                            try
                            {
                                has = ator.MoveNext();
                            }
                            catch (System.Exception e)
                            {
                                isError = true;
                                Debuger.LogError("ExportAsset:" + asset.name);
                                Debuger.LogException(e);
                            }

                            if (has)
                            {
                                yield return 0;
                            }
                        }
                    }

                    num++;
                    if ((num % 50) == 0)
                        yield return 0;

                    //                     if (ShowLog)
                    //                     {
                    //                         Debuger.Log(asset == null ? itor.Key : asset.name);
                    //                         yield return 0;
                    //                     }
                }

                if (objs.Count == 0)
                    break;

                ++index;
                newobjs = new Dictionary<string, int>(objs);
                objs.Clear();
            }

            Debuger.DebugLog("{0}: total:{1} totaltime:{2}", typename, total, tc.delay);
        }
    }
}

#endif