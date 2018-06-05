#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using System;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace PackTool
{
    // 保存有用到的内置资源，贴图，shader之类的
    public partial class BuiltinResource : MonoBehaviour
    {
        [Serializable]
        class Data
        {
            [SerializeField]
            List<Object> objs; // 资源列表

#if UNITY_EDITOR
            public bool Add(Object o)
            {
                if (objs == null)
                    objs = new List<Object>();

                if (objs.Contains(o))
                    return false;

                while (true)
                {
                    if (objs.RemoveAll((Object x) => { return o == null ? true : false; }) == 0)
                        break;
                }

                objs.Add(o);;
                objs.Sort(
                    (Object x, Object y) => 
                    {
                        if (x == null)
                            return -1;

                        if (y == null)
                            return 1;

                        string[] names = new string[] { x.name, y.name };
                        System.Array.Sort<string>(names);
                        if (names[0] == x.name)
                            return -1;

                        return 1;
                    });

                return true;
            }

            public void Clear()
            {
                objs = null;
                maps = null;
            }

            public T[] GetT<T>() where T : Object
            {
                int l = objs == null ? 0 : objs.Count;
                T[] tos = new T[l];
                for (int i = 0; i < l; ++i)
                {
                    tos[i] = objs[i] as T;
                }

                return tos;
            }
#endif
            public Dictionary<string, Object> maps;

            public Object Get(string name)
            {
                if (maps == null)
                    return null;

                Object o = null;
                if (maps.TryGetValue(name, out o))
                {
                    return o;
                }

                return null;
            }

            public void Init()
            {
                maps = new Dictionary<string, Object>();
                if (objs == null)
                    return;

                string name;
                foreach (Object o in objs)
                {
                    if (o != null)
                    {
                        name = o.name;
                        if (!maps.ContainsKey(name))
                            maps.Add(name, o);
                        else
                            Debug.LogErrorFormat("type:{0} name:{1}", o.GetType().Name, name);
                        StringHashCode.Add(name);
                    }
                }

                objs = null;
            }
        }

        enum AssetType
        {
            Texture,
            Material,
            Shader,
            Mesh,
            Font,
            Sprite,

            Max
        }

        [SerializeField]
        Data Textures = new Data();

        [SerializeField]
        Data Materials = new Data();

        [SerializeField]
        Data Shaders = new Data();

        [SerializeField]
        Data Meshs = new Data();

        [SerializeField]
        Data Fonts = new Data();

        [SerializeField]
        Data Sprites = new Data();

        Data Get(AssetType type)
        {
            switch (type)
            {
            case AssetType.Texture: return Textures;
            case AssetType.Material: return Materials;
            case AssetType.Shader: return Shaders;
            case AssetType.Mesh: return Meshs;
            case AssetType.Font: return Fonts;
            case AssetType.Sprite: return Sprites;
            }

            return null;
        }

        public Material GetMat(string name)
        {
            return Get(AssetType.Material).Get(name) as Material;
        }

        public Shader GetShader(string name)
        {
            return Get(AssetType.Shader).Get(name) as Shader;
        }

        public Mesh GetMesh(string name)
        {
            return Get(AssetType.Mesh).Get(name) as Mesh;
        }

        public Texture GetTexture(string name)
        {
            return Get(AssetType.Texture).Get(name) as Texture;
        }

        public Font GetFont(string name)
        {
            return Get(AssetType.Font).Get(name) as Font;
        }

        [Serializable]
        public class MaterialData
        {
            public int key; // key值
            public Material mat;
        }

        [SerializeField]
        List<MaterialData> SelfMaterialsData;

        Dictionary<int, MaterialData> SelfMaterials;

        public MaterialData GetMaterial(int key)
        {
            if (SelfMaterials == null)
                return null; ;

            MaterialData md = null;
            if (SelfMaterials.TryGetValue(key, out md))
                return md;

            return null;
        }

        void InitMaterialData()
        {
            if (SelfMaterials == null)
                SelfMaterials = new Dictionary<int, MaterialData>();
            else
                SelfMaterials.Clear();

            if (SelfMaterialsData != null)
            {
                MaterialData md;
                for (int i = 0; i < SelfMaterialsData.Count; ++i)
                {
                    md = SelfMaterialsData[i];
                    SelfMaterials.Add(md.key, md);
                }
            }
        }

#if UNITY_EDITOR
        public Shader[] GetShaders()
        {
            Data d = Get(AssetType.Shader);
            if (d == null)
                return null;

            return d.GetT<Shader>();
        }

        public Material[] GetMaterials()
        {
            Dictionary<int, string> dic = null;
            List<Material> mats = new List<Material>(SelfMaterialsData.Count);
            for (int i = 0; i < SelfMaterialsData.Count; ++i)
            {
                if (SelfMaterialsData[i] != null)
                {
                    if (SelfMaterialsData[i].mat != null)
                        mats.Add(SelfMaterialsData[i].mat);
                    else
                    {
                        int key = SelfMaterialsData[i].key;
                        if (dic == null)
                            dic = StringHashCode.PathToHash();

                        string assetPath;
                        if (dic.TryGetValue(key, out assetPath))
                        {
                            var m = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/" + assetPath);
                            if (m != null)
                                mats.Add(m);
                        }
                    }
                }
            }

            return mats.ToArray();
        }
#endif

        public Sprite GetSprite(string name)
        {
            return Get(AssetType.Sprite).Get(name) as Sprite;
        }

        void InitMap()
        {
            for (int i = 0; i < (int)AssetType.Max; ++i)
            {
                Get((AssetType)i).Init();
            }

            InitMaterialData();
            SelfMaterialsData = null;

#if UNITY_EDITOR
            Dictionary<string, Object> newlist = new Dictionary<string, Object>();
            Data shaderList = Get(AssetType.Shader);
            Dictionary<string, Object>.Enumerator itor = shaderList.maps.GetEnumerator();
            Shader shader = null;
            while (itor.MoveNext())
            {
                shader = Shader.Find(itor.Current.Value.name);
                if (shader == null)
                    shader = itor.Current.Value as Shader;

                newlist.Add(itor.Current.Key, shader);
            }

            shaderList.maps = newlist;
            itor = Get(AssetType.Material).maps.GetEnumerator();
            while (itor.MoveNext())
            {
                Material mat = itor.Current.Value as Material;
                shader = Shader.Find(mat.shader.name);
                if (shader != null)
                    mat.shader = shader;
            }

            foreach (var v in SelfMaterials)
            {
                if (v.Value.mat == null || v.Value.mat.shader == null)
                    continue;

                shader = Shader.Find(v.Value.mat.shader.name);
                if (shader != null)
                {
                    v.Value.mat.shader = shader;
                }
            }
#endif
        }

#if UNITY_EDITOR
        AssetType GetAssetType(Object o)
        {
            if (o is Texture)
                return AssetType.Texture;
            if (o is Material)
                return AssetType.Material;
            if (o is Shader)
                return AssetType.Shader;
            if (o is Mesh)
                return AssetType.Mesh;
            if (o is Font)
                return AssetType.Font;
            if (o is Sprite)
                return AssetType.Sprite;

            return AssetType.Max;
        }

        public bool Add(Object o)
        {
            AssetType at = GetAssetType(o);
            if (at == AssetType.Max)
            {
                if (o == null)
                {
                    XYJLogger.LogError("BuiltinResource o:null");
                }
                else
                {
                    XYJLogger.LogError("BuiltinResource o:{0} type:{1}", o.name, o.GetType().Name);
                }

                return false;
            }

            return Get(at).Add(o);
        }

        public void AddMaterial(Material mat)
        {
            if (SelfMaterials == null)
                InitMaterialData();

            string key = UnityEditor.AssetDatabase.GetAssetPath(mat).Substring(16);
            MaterialData md = null;
            int code = key.GetHashCode();
            if (!SelfMaterials.TryGetValue(code, out md))
            {
                md = new MaterialData();
                md.key = code;
                SelfMaterials.Add(code, md);

                if (SelfMaterialsData == null)
                    SelfMaterialsData = new List<MaterialData>();

                SelfMaterialsData.Add(md);
            }

            md.mat = mat;
            md.key = code;
        }
#endif
    }
}
