#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using System;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace PackTool
{
    public class SceneRoot : MonoBehaviour
    {
#if !USE_ABL
        [PackTool.Pack]
        public Material skybox;

        [PackTool.Pack]
        public Cubemap customReflection;

        [PackTool.Pack]
        public LightProbes lightProbes;

        [PackTool.Pack]
        public Texture2D[] lightmapLights;

        [PackTool.Pack]
        public Texture2D[] lightmapDirs;
#endif
        [Serializable]
        public class StaticList
        {
            public StaticList()
            {

            }

            public StaticList(GameObject[] gs)
            {
                gos = gs;
            }

            [SerializeField]
            public GameObject[] gos;
        }

        [SerializeField]
        StaticList[] StaticBatchings;

        [System.Serializable]
        class ObjLMD
        {
            public Renderer renderer = null;
            public int lightmapIndex;
            public Vector4 lightmapScaleOffset;

            public void Save()
            {
                lightmapIndex = renderer.lightmapIndex;
                lightmapScaleOffset = renderer.lightmapScaleOffset;
            }

            public void Set()
            {
                renderer.lightmapIndex = lightmapIndex;
                renderer.lightmapScaleOffset = lightmapScaleOffset;

                //Logger.LogDebug("{0} lightmapIndex:{1} lightmapScaleOffset:{2}", renderer.name, renderer.lightmapIndex, renderer.lightmapScaleOffset);
            }
        }

        [SerializeField]
        List<ObjLMD> ObjLMDs = new List<ObjLMD>();

        [SerializeField]
        LightmapsMode lightmapsMode;

        void SetObjLMD()
        {
            for (int i = 0; i < ObjLMDs.Count; ++i)
            {
                ObjLMDs[i].Set();
            }
        }

#if UNITY_EDITOR
        void SaveObjLMD()
        {
            ObjLMDs.Clear();
            Renderer[] renderers = transform.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; ++i)
            {
                Renderer r = renderers[i];
                if (r.lightmapIndex == -1)
                    continue;

                ObjLMD d = new ObjLMD();
                d.renderer = r;
                d.Save();

                ObjLMDs.Add(d);
            }
        }

        // 是否可以合并
        public static bool IsCanStaticBatch(Mesh mesh)
        {
            int vertexCount = mesh.vertexCount;
            if (vertexCount >= 5000)
                return false;

            return true;
        }

        static bool IsCanStaticBatch(GameObject go)
        {
            var flags = UnityEditor.GameObjectUtility.GetStaticEditorFlags(go);
            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                return false;

            if (go.GetComponent<ScrollingUVs>() != null)
                return false;

            flags = flags & (~UnityEditor.StaticEditorFlags.BatchingStatic);
            UnityEditor.GameObjectUtility.SetStaticEditorFlags(go, flags);
            return true;
        }

        [UnityEditor.MenuItem("PackTool/SetBatching")]
        public static void SetCurrentBatching()
        {
            List<StaticList> sls = new List<StaticList>();
            List<GameObject> roots = new List<GameObject>();
            GetCurrentSceneBatching(sls, roots);

            SetStaticBatching(sls.ToArray());
        }

        public static void GetCurrentSceneBatching(List<StaticList> sls, List<GameObject> roots)
        {
            if (roots == null)
                roots = new List<GameObject>();

            UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            roots.AddRange(scene.GetRootGameObjects());
            GetStaticBatchings(roots.ToArray(), sls);
        }

        static int Meshkey(Mesh mesh)
        {
            IntBit ib = new IntBit();
            ib.Set(0, mesh.uv.Length != 0);
            ib.Set(1, mesh.uv2.Length != 0);
            ib.Set(2, mesh.uv3.Length != 0);
            ib.Set(3, mesh.uv4.Length != 0);

            ib.Set(4, mesh.colors.Length != 0);
            ib.Set(5, mesh.normals.Length != 0);
            ib.Set(6, mesh.triangles.Length != 0);

            return ib.value;
        }

        public static void GetStaticBatchings(GameObject[] roots, List<StaticList> slList)
        {
            Dictionary<int, List<GameObject>> gos = new Dictionary<int, List<GameObject>>();
            List<MeshFilter> meshFilters = new List<MeshFilter>();

            for (int m = 0; m < roots.Length; ++m)
            {
                roots[m].GetComponentsInChildren(true, meshFilters);
                for (int i = 0; i < meshFilters.Count; ++i)
                {
                    if (meshFilters[i] == null)
                        continue;

                    if (!IsCanStaticBatch(meshFilters[i].gameObject))
                        continue;

                    Mesh mesh = meshFilters[i].sharedMesh;
                    if (mesh == null)
                        continue;

                    if (!IsCanStaticBatch(mesh))
                        continue;

                    int meshkey = Meshkey(mesh);
                    List<GameObject> gs = null;
                    if (!gos.TryGetValue(meshkey, out gs))
                    {
                        gs = new List<GameObject>();
                        gos.Add(meshkey, gs);
                    }

                    gs.Add(meshFilters[i].gameObject);
                }
            }

            foreach (var itor in gos)
            {
                slList.Add(new StaticList(itor.Value.ToArray()));
            }
        }

        void SetStaticBatchings()
        {
            List<StaticList> sls = new List<StaticList>();
            GetStaticBatchings(new GameObject[] { gameObject }, sls);
            StaticBatchings = sls.ToArray();
        }

        public void SaveScene()
        {
            SetStaticBatchings();

            SaveObjLMD();

#if !USE_ABL
            customReflection = RenderSettings.customReflection;
            skybox = RenderSettings.skybox;

            RenderSettings.skybox = null;
            RenderSettings.customReflection = null;

            rsConfig.Save();

            lightProbes = LightmapSettings.lightProbes;

            lightmapsMode = LightmapSettings.lightmapsMode;

            LightmapData[] lds = LightmapSettings.lightmaps;
            int count = lds == null ? 0 : lds.Length;
            lightmapDirs = new Texture2D[count];
            lightmapLights = new Texture2D[count];
            for (int i = 0; i < lds.Length; ++i)
            {
                LightmapData ld = lds[i];
                lightmapDirs[i] = ld.lightmapDir;
                lightmapLights[i] = ld.lightmapColor;
            }

            LightmapSettings.lightProbes = null;
#endif
        }
#endif

        [SerializeField]
        RSConfig rsConfig;

        [Serializable]
        struct RSConfig
        {
            public Color ambientEquatorColor;
            public Color ambientGroundColor;
            public float ambientIntensity;
            public Color ambientLight;
            public AmbientMode ambientMode;
            public SphericalHarmonicsL2 ambientProbe;
            public Color ambientSkyColor;
            public DefaultReflectionMode defaultReflectionMode;
            public int defaultReflectionResolution;
            public float flareFadeSpeed;
            public float flareStrength;
            public bool fog;
            public Color fogColor;
            public float fogDensity;
            public float fogEndDistance;
            public FogMode fogMode;
            public float fogStartDistance;
            public float haloStrength;
            public int reflectionBounces;
            public float reflectionIntensity;
            public Light sun;

            public void Save()
            {
                ambientEquatorColor = RenderSettings.ambientEquatorColor;
                ambientGroundColor = RenderSettings.ambientGroundColor;
                ambientIntensity = RenderSettings.ambientIntensity;
                ambientLight = RenderSettings.ambientLight;
                ambientMode = RenderSettings.ambientMode;
                ambientProbe = RenderSettings.ambientProbe;
                ambientSkyColor = RenderSettings.ambientSkyColor;
                defaultReflectionMode = RenderSettings.defaultReflectionMode;
                defaultReflectionResolution = RenderSettings.defaultReflectionResolution;
                flareFadeSpeed = RenderSettings.flareFadeSpeed;
                flareStrength = RenderSettings.flareStrength;
                fog = RenderSettings.fog;
                fogColor = RenderSettings.fogColor;
                fogDensity = RenderSettings.fogDensity;
                fogEndDistance = RenderSettings.fogEndDistance;
                fogMode = RenderSettings.fogMode;
                fogStartDistance = RenderSettings.fogStartDistance;
                haloStrength = RenderSettings.haloStrength;
                reflectionBounces = RenderSettings.reflectionBounces;
                reflectionIntensity = RenderSettings.reflectionIntensity;
                sun = RenderSettings.sun;
            }

            public void Restore()
            {
                RenderSettings.ambientEquatorColor = ambientEquatorColor;
                RenderSettings.ambientGroundColor = ambientGroundColor;
                RenderSettings.ambientIntensity = ambientIntensity;
                RenderSettings.ambientLight = ambientLight;
                RenderSettings.ambientMode = ambientMode;
                RenderSettings.ambientProbe = ambientProbe;
                RenderSettings.ambientSkyColor = ambientSkyColor;
                RenderSettings.defaultReflectionMode = defaultReflectionMode;
                RenderSettings.defaultReflectionResolution = defaultReflectionResolution;
                RenderSettings.flareFadeSpeed = flareFadeSpeed;
                RenderSettings.flareStrength = flareStrength;
                RenderSettings.fog = fog;
                RenderSettings.fogColor = fogColor;
                RenderSettings.fogDensity = fogDensity;
                RenderSettings.fogEndDistance = fogEndDistance;
                RenderSettings.fogMode = fogMode;
                RenderSettings.fogStartDistance = fogStartDistance;
                RenderSettings.haloStrength = haloStrength;
                RenderSettings.reflectionBounces = reflectionBounces;
                RenderSettings.reflectionIntensity = reflectionIntensity;
                RenderSettings.sun = sun;
            }
        }

        public static void SetStaticBatching(StaticList[] sls)
        {
            if (sls == null || sls.Length == 0)
                return ;

            List<GameObject> rs = new List<GameObject>();
            foreach (StaticList gos in sls)
            {
                rs.Clear();
                foreach (GameObject g in gos.gos)
                {
                    if (g != null)
                        rs.Add(g);
                }

                if (rs.Count != 0)
                {
                    GameObject root = GetRoot(rs[0]);
                    GameObject[] coms = rs.Count == gos.gos.Length ? gos.gos : rs.ToArray();
                    StaticBatchingUtility.Combine(coms, root);
                }
            }
        }

        static GameObject GetRoot(GameObject go)
        {
            Transform this_tran = go.transform.root;
            Transform current = go.transform;
            while ((current.parent != null) && (current.parent != this_tran))
                current = current.parent;

            return current.gameObject;
        }

        public void RestoreScene()
        {
#if !USE_ABL
            RenderSettings.skybox = skybox;
            RenderSettings.customReflection = customReflection;
            LightmapSettings.lightProbes = lightProbes;
            LightmapSettings.lightmapsMode = lightmapsMode;

            int count = lightmapDirs == null ? 0 : lightmapDirs.Length;
            LightmapData[] lds = new LightmapData[count];
            for (int i = 0; i < count; ++i)
            {
                LightmapData ld = new LightmapData();
                lds[i] = ld;
                ld.lightmapDir = lightmapDirs[i];
                ld.lightmapColor = lightmapLights[i];
            }

            LightmapSettings.lightmaps = lds;
#endif
            SetObjLMD();
            SetStaticBatching(StaticBatchings);

            rsConfig.Restore();
            Debug.LogFormat("name:{0} scene:{1}", LightmapSettings.lightProbes == null ? "null" : LightmapSettings.lightProbes.name, name);
        }
    }
}


