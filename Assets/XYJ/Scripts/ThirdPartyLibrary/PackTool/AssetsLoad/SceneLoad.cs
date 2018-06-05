using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XTools;

namespace PackTool
{
    public class SceneLoad
#if USE_RESOURCESEXPORT
        : AssetLoad<Object>
#endif
    {
        // 加载的状态
        public enum State
        {
            LoadAssetBundle, // 加载场景LoadAssetBundle中
            LoadDep,         // 加载场景资源依赖中
            End,             // 加载完成
        }

        public State CurrentState { get; protected set; }

#if !USE_RESOURCESEXPORT
        public bool isDone { get; private set; }
#endif

        // 场景加载的进度
        public float progress
        {
            get 
            {
                if (isDone)
                    return 1.0f;

#if USE_RESOURCESEXPORT
                return (dependence_progress == 0.0f ? 0.1f : dependence_progress);
#else
                return lla == null ? 0f : lla.progress;
#endif
            }
        }

#if !USE_RESOURCESEXPORT
        AsyncOperation lla = null;
#endif

        static string ScenePath(string sceneid)
        {
#if USE_ABL || USE_RESOURCESEXPORT || UNITY_EDITOR
            
            #if SCENE_OPT
            string fullpath = ResourcesGroup.GetPath(ResourcesGroup.Scene, sceneid + "_Opt.unity");
            if (!string.IsNullOrEmpty(fullpath))
                return fullpath;
            #endif

            return ResourcesGroup.GetFullPath(ResourcesGroup.Scene, sceneid + ".unity");
#else

#if SCENE_OPT
            string s = sceneid + "_Opt";
            if (SceneVersion.IsExist(s))
                return s;
#endif
            return sceneid;
#endif
        }

        public delegate bool OnSceneAssetEnd(SceneResRecords srrs, object p);

        OnSceneAssetEnd mSAE = null;
        object mSAEp = null;

        public static void DestroyScene()
        {
#if USE_RESOURCESEXPORT || USE_ABL
            if (Current != null)
            {
                Current.DestroySelf();
                Current = null;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene("Empty");
#endif
        }

#if USE_RESOURCESEXPORT
        static SceneLoad Create(string url)
        {
            SceneLoad sl = new SceneLoad();
            sl.Reset(url);
            return sl;
        }
#endif

        // show加载完成之后，是否立即显示
        public static SceneLoad Load(string name, OnSceneAssetEnd sae, object saep = null)
        {
            DestroyScene();
#if USE_RESOURCESEXPORT
            // 先卸载所有的资源
            Data sld = LoadImp(ScenePath(name), null, null, Create);
            Current = sld.load as SceneLoad;
            Current.AddRef();
            return Current;
#elif USE_ABL
            if (!name.EndsWith(".unity"))
                name += ".unity";

            Current = new SceneLoad();
            Current.Load(name);
            return Current;
#else
            Current = new SceneLoad();
#if UNITY_EDITOR
            string path = ScenePath(name);
#if USE_SIMPLESCENE
            if (File.Exists("Assets/__copy__/" + path))
            {
                path = "__copy__/" + path;
            }
#endif
            Current.lla = UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode("Assets/" + path);
#else
            Current.lla = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(ScenePath(name));
#endif
            MagicThread.Instance.StartCoroutine(Current.CheckEnd());
            return Current;
#endif
        }

#if USE_ABL
        string url;

        public ABL.IAsset current { get; private set; }

        void Load(string url)
        {
            this.url = url;
            ABL.AssetsMgr.LoadScene(ResourcesGroup.GetFullPath(ResourcesGroup.Scene, url.ToLower()), LoadEnd);
        }

        void LoadEnd(ABL.IAsset obj)
        {
            current = obj;
            current.AddRef();
            MagicThread.Instance.StartCoroutine(Load());
        }

        IEnumerator Load()
        {
            lla = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(Path.GetFileNameWithoutExtension(url), UnityEngine.SceneManagement.LoadSceneMode.Single);
            yield return lla;

            Debug.LogFormat("t:{0}", lla.GetType().Name);

            UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            SceneRoot sr = null;
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                sr = go.GetComponent<SceneRoot>();
                if (sr != null)
                    break;
            }

            if (sr != null)
            {
                sr.RestoreScene();

                sr.transform.DetachChildren();
                Object.Destroy(sr.gameObject);
            }

            isDone = true;
        }

        void DestroySelf()
        {
            current.SubRef();
        }

        public void AddRef()
        {
            current.AddRef();
        }
#endif

#if !USE_RESOURCESEXPORT && !USE_ABL
        IEnumerator CheckEnd()
        {
#if UNITY_EDITOR
            UnityEditor.Lightmapping.bakedGI = false;
#endif
            CurrentState = State.LoadDep;
            var la = lla;
            while (!la.isDone)
            {
                yield return 0;
            }

            yield return 0;
            if (mSAE != null)
            {
                mSAE(null, mSAEp);
            }

            //UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            //CheckOptimize(scene);

            isDone = true;

#if UNITY_EDITOR
            UnityEditor.Lightmapping.bakedGI = false;
#endif
        }
#endif

#if ASSET_DEBUG
        static TimeTrack sceneload_show = TimeTrackMgr.Instance.Get("SceneLoad.Show");
        static TimeTrack sceneload_RestoreScene = TimeTrackMgr.Instance.Get("SceneLoad.RestoreScene");
        static TimeTrack sceneload_loadlevelAsync = TimeTrackMgr.Instance.Get("Application.LoadLevelAsync");
#endif

        // 当前场景
        public static SceneLoad Current { get; protected set; }

#if USE_RESOURCESEXPORT
        protected override void FreeSelf()
        {

        }

        protected override void ReleaseImp()
        {
            base.ReleaseImp();

#if USE_RESOURCESEXPORT
            dependence_progress = 0f;
#endif
        }
#endif
        // 开始显示
        void Show()
        {
#if USE_RESOURCESEXPORT
            if (mRoot != null)
            {
                SceneRoot sr = mRoot.GetComponent<SceneRoot>();
                if (sr != null)
                {
#if ASSET_DEBUG
                    sceneload_RestoreScene.Execution(
                        url,
                        () =>
                        {
#endif
                            sr.RestoreScene();
#if ASSET_DEBUG
                        });
#endif
                }

#if ASSET_DEBUG
                sceneload_show.Execution(url,
                    () => 
                    {
#endif
                        Transform node = mRoot.transform.Find("MapScene");
                        if (node != null)
                            node.AddComponentIfNoExist<SceneOptimize>();
                        mRoot.transform.DetachChildren();
                        
                        Object.Destroy(mRoot);
#if ASSET_DEBUG
                    });
#endif

#if UNITY_EDITOR
                Shader shader = null;
                if (RenderSettings.skybox != null && ((shader = RenderSettings.skybox.shader) != null))
                {
                    if ((shader = Shader.Find(shader.name)) != null)
                        RenderSettings.skybox.shader = shader;
                }
#endif
            }
#endif
        }

#if USE_RESOURCESEXPORT
        protected override void LoadAsset(string name)
        {
            AssetBundleLoad.Load(name, OnLoadAssetEnd);
        }

        protected void OnLoadAssetEnd(AssetBundle assetBundle)
        {
            if (assetBundle == null)
            {
                OnEnd();
                dependence_progress = 1f;
                CurrentState = State.End;
                Debuger.LogError(string.Format("scene:{0} LoadAssetEnd obj = null!", url));
                return;
            }

            MagicThread.Instance.StartCoroutine(LoadDependence(assetBundle));
        }

        float dependence_progress = 0f;

        GameObject mRoot = null;

        IEnumerator LoadDependence(AssetBundle assetBundle)
        {
            Component[] components = null;
            dependence_progress = 0.6f;

            string scene_name = Path.GetFileNameWithoutExtension(url);

#if RESOURCES_DEBUG
            OnceStep step = new OnceStep();
            yield return step.Next(string.Format("LoadSceneAsync:{0}!", scene_name));
#endif
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene_name);
            while (!ao.isDone)
                yield return 0;

#if RESOURCES_DEBUG
            yield return step.Next("场景AB加载完毕!");
#endif
            assetBundle.Unload(false);

#if RESOURCES_DEBUG
            yield return step.Next("释放AB!");
#endif

            UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            //CheckOptimize(scene);

            SceneResRecords srrs = null;
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                srrs = go.GetComponent<SceneResRecords>();
                if (srrs != null)
                    mRoot = go;
            }
            if (srrs != null)
            {
                if (mSAE != null)
                {
                    if (mSAE(srrs, mSAEp))
                        yield return 0;
                }
                components = srrs.components;
                Object.Destroy(srrs);

                //SceneRoot srs = mRoot.GetComponent<SceneRoot>();

                if (components != null)
                {
                    ComponentSave cps = Buff<ComponentSave>.Get();
                    Stream stream_data = ResourcesPack.FindBaseStream(string.Format("{0}{1}", url, Suffix.SceneDataByte));
                    Stream stream_pos = ResourcesPack.FindBaseStream(string.Format("{0}{1}", url, Suffix.ScenePosByte));

#if RESOURCES_DEBUG
                    yield return step.Next("开始加载依赖!");
#endif

#if ASSET_DEBUG
                    ComponentSave.LoadResources_timetrack.Execution(
                        url, 
                        () => 
                        {
#endif
                    cps.LoadResources(stream_data, components, stream_pos, null,
                                (string dep) =>
                                {
                                    GetDependences().Add(dep);
                                });
#if ASSET_DEBUG
                    });
#endif

#if RESOURCES_DEBUG
                    yield return step.Next("依赖加载结束!");
#endif
                    Current = this;
                    AssetsUnLoad.UnloadUnusedAssets(false);
                    while (!AssetsUnLoad.isDone)
                        yield return 0;

                    components = null;
                    CurrentState = State.LoadDep;
                    while (!cps.isDone)
                    {
                        dependence_progress = 0.6f + 0.4f * cps.progress;
                        yield return 0;
                    }
                    yield return 0;

                    AddDepRef();

                    cps.Release();
                    Buff<ComponentSave>.Free(cps);
                    cps = null;
                }
            }
#if UNITY_EDITOR
            else
            {
                List<Renderer> renderers = new List<Renderer>();
                foreach (GameObject go in scene.GetRootGameObjects())
                    go.GetComponentsInChildren(true, renderers);

                for (int i = 0; i < renderers.Count; ++i)
                {
                    Material[] materials = renderers[i].sharedMaterials;
                    for (int j = 0; j < materials.Length; ++j)
                    {
                        Material mat = materials[j];
                        if (mat == null)
                            continue;

                        Shader s = mat.shader;
                        if (s != null && (s = Shader.Find(s.name)) != null)
                            mat.shader = s;
                    }
                }
            }
#endif
            Show();

            AssetsUnLoad.ClearMeshByStaticBinding();
            while (!AssetsUnLoad.isDone)
                yield return 0;

            OnEnd();
            dependence_progress = 1.0f;
            CurrentState = State.End;

            yield return 0;
            UnityEngine.DynamicGI.UpdateEnvironment();
        }
#endif
    }
}
