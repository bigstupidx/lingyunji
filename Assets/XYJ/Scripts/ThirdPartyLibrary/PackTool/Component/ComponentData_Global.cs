using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
#if USE_RESOURCESEXPORT
    // 组件所需要保存的数据
    public abstract partial class ComponentData
    {
#if UNITY_EDITOR
        delegate bool CollectObject<T>(ref T t, BinaryWriter writer, IAssetsExport mgr);

        static bool __CollectTemplate__<T>(ref Object obj, byte type, BinaryWriter writer, IAssetsExport mgr, CollectObject<T> fun) where T : Object
        {
            writer.Write(type);
            T t = obj as T;
            if (fun(ref t, writer, mgr))
            {
                obj = t;
                return true;
            }

            return false;
        }

        static protected bool __CollectObjects__(List<Object> objs, BinaryWriter writer, IAssetsExport mgr)
        {
            if (objs.Count == 0)
                return false;

            long pos = writer.BaseStream.Position;
            bool isHit = false;
            writer.Write((ushort)objs.Count);
            for (int i = 0; i < objs.Count; ++i)
            {
                Object obj = objs[i];
                if (__CollectObject__(ref obj, writer, mgr))
                {
                    objs[i] = obj;
                    isHit = true;
                }
            }

            if (isHit)
                return true;

            writer.BaseStream.Position = pos;
            return false;
        }

        static protected bool __CollectObject__(ref Object obj, BinaryWriter writer, IAssetsExport mgr)
        {
            if (obj == null)
            {
                writer.Write((byte)0);
                return false;
            }

            if (obj is Sprite)
            {
                return __CollectTemplate__<Sprite>(ref obj, 1, writer, mgr, __CollectSprite__);
            }
            else if (obj is Texture)
            {
                return __CollectTemplate__<Texture>(ref obj, 2, writer, mgr, __CollectTexture__);
            }
            else if (obj is Material)
            {
                return __CollectTemplate__<Material>(ref obj, 3, writer, mgr, __CollectMaterial__);
            }
            else if (obj is AudioClip)
            {
                return __CollectTemplate__<AudioClip>(ref obj, 4, writer, mgr, __CollectAudioClip__);
            }
            else if (obj is Mesh)
            {
                return __CollectTemplate__<Mesh>(ref obj, 5, writer, mgr, __CollectMesh__);
            }
            else if (obj is Font)
            {
                return __CollectTemplate__<Font>(ref obj, 6, writer, mgr, __CollectFont__);
            }
            else if (obj is GameObject)
            {
                return __CollectTemplate__<GameObject>(ref obj, 7, writer, mgr, __CollectPrefab__);
            }
            else if (obj is MonoBehaviour)
            {
                return __CollectTemplate__<MonoBehaviour>(ref obj, 8, writer, mgr, __CollectMono__);
            }
            else
            {
                writer.Write((byte)0);
                return false;
            }
        }

        static protected bool __CollectTMPFont__(ref TMPro.TMP_FontAsset fontAsset, BinaryWriter writer, IAssetsExport mgr)
        {
            ResourceStream rs = ResourceStream.GetResType(fontAsset);
            switch (rs.type)
            {
            case ResType.Resources:
                break;
            case ResType.Asset:
                mgr.CollectTMPFont(fontAsset);
                break;
            default:
                break;
            }

            if (rs.GetRes())
                fontAsset = null;
            rs.Write(writer);

            return rs.GetRes();
        }

        static protected bool __CollectTexture2DAsset__(ref Texture2DAsset asset, BinaryWriter writer, IAssetsExport mgr)
        {
            ResourceStream rs = ResourceStream.GetResType(asset);
            switch (rs.type)
            {
            case ResType.Resources:
                break;
            case ResType.Asset:
                mgr.CollectTexture2DAsset(asset);
                break;
            default:
                break;
            }

            if (rs.GetRes())
                asset = null;
            rs.Write(writer);
            return rs.GetRes();
        }

        static protected bool __CollectMesh__(ref Mesh mesh, BinaryWriter writer, IAssetsExport mgr)
        {
            ResourceStream rs = ResourceStream.GetResType(mesh);
            switch (rs.type)
            {
            case ResType.Builtin:
                mgr.CollectBuiltinResource(mesh);
                break;
            case ResType.Resources:
                break;
            case ResType.Asset:
                mgr.CollectMesh(mesh);
                rs.path = AssetsExport.ExportMeshPath(mesh);
                rs.hashcode = rs.path.GetHashCode();
                break;
            default:
                break;
            }

            if (rs.GetRes())
                mesh = null;
            rs.Write(writer);

            return rs.GetRes();
        }

        static protected bool __CollectSprite__(ref Sprite sprite, BinaryWriter writer, IAssetsExport mgr)
        {
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(sprite);
            if (string.IsNullOrEmpty(assetPath) || sprite == null)
            {
                writer.Write("");
                return false;
            }

            if (assetPath == "Resources/unity_builtin_extra")
            {
                writer.Write(":"+ sprite.name);
                mgr.CollectBuiltinResource(sprite);
                sprite = null;
                return true;
            }

            //UnityEditor.TextureImporter textureImporter = (UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath(assetPath);

            //             string direct = assetPath.Substring(0, assetPath.LastIndexOf('/'));
            //             direct = direct.Substring(direct.LastIndexOf('/') + 1);

            mgr.CollectSprite(sprite);
            string asset = sprite.name;
            writer.Write(asset);
            sprite = null;

            return true;
        }

        static public bool __CollectMaterial__(ref Material mat, BinaryWriter writer, IAssetsExport mgr)
        {
            ResourceStream rs = ResourceStream.GetResType(mat);
            switch (rs.type)
            {
            case ResType.Builtin:
                mgr.CollectBuiltinResource(mat);
                break;
            case ResType.Resources:
                break;
            case ResType.Asset:
                {
                    if (rs.path.EndsWith(".mat", true, null))
                    {
                        mgr.CollectMaterial(mat);
                    }
                    else
                    {
                        rs.type = ResType.Empty;
                        mat = null;
                    }
                }
                break;
            default:
                break;
            }

            if (rs.GetRes())
                mat = null;
            rs.Write(writer);

            return rs.GetRes();
        }

        static protected bool __CollectAnimClip__(ref AnimationClip clip, BinaryWriter writer, IAssetsExport mgr)
        {
            ResourceStream rs = ResourceStream.GetResType(clip);
            switch (rs.type)
            {
            case ResType.Builtin:
                break;
            case ResType.Resources:
                break;
            case ResType.Asset:
                {
                    rs.path = AssetsExport.ExportAnimPath(clip);
                    rs.hashcode = rs.path.GetHashCode();
                    mgr.CollectAnimation(clip);
                }
                break;
            default:
                break;
            }

            if (rs.GetRes())
                clip = null;
            rs.Write(writer);

            return rs.GetRes();
        }

        static protected bool __CollectAnimatorController__(ref RuntimeAnimatorController controller, BinaryWriter writer, IAssetsExport mgr)
        {
            ResourceStream rs = ResourceStream.GetResType(controller);
            switch (rs.type)
            {
            case ResType.Builtin:
                break;
            case ResType.Resources:
                break;
            case ResType.Asset:
                {
                    mgr.CollectAnimator(controller);
                }
                break;
            default:
                break;
            }

            if (rs.GetRes())
                controller = null;

            rs.Write(writer);

            return rs.GetRes();
        }

        static protected bool __CollectAvatar__(ref Avatar avatar, BinaryWriter writer, IAssetsExport mgr)
        {
            ResourceStream rs = ResourceStream.GetResType(avatar);
            switch (rs.type)
            {
            case ResType.Builtin:
                break;
            case ResType.Resources:
                break;
            case ResType.Asset:
                {
                    rs.path = AssetsExport.ExportAvatarPath(avatar);
                    rs.hashcode = rs.path.GetHashCode();

                    mgr.CollectAvatar(avatar);
                }
                break;
            default:
                break;
            }

            if (rs.GetRes())
                avatar = null;

            rs.Write(writer);

            return rs.GetRes();
        }

        static protected bool __CollectAudioClip__(ref AudioClip audioClip, BinaryWriter writer, IAssetsExport mgr)
        {
            ResourceStream rs = ResourceStream.GetResType(audioClip);
            switch (rs.type)
            {
            case ResType.Builtin:
                break;
            case ResType.Resources:
                break;
            case ResType.Asset:
                {
                    mgr.CollectSound(audioClip);
                }
                break;
            default:
                break;
            }

            if (rs.GetRes())
                audioClip = null;

            rs.Write(writer);

            return rs.GetRes();
        }

        static public bool __CollectShader__(ref Shader shader, BinaryWriter writer, IAssetsExport mgr)
        {
            ResourceStream rs = ResourceStream.GetResType(shader);
            switch (rs.type)
            {
            case ResType.Resources:
                break;
            case ResType.Builtin:
            case ResType.Asset:
                {
                    mgr.CollectBuiltinResource(shader);

                    rs.path = shader.name;
                    rs.hashcode = rs.path.GetHashCode();
                }
                break;
            default:
                break;
            }

            rs.Write(writer);

            if (rs.GetRes())
                shader = null;

            return rs.GetRes();
        }
        static public bool __CollectLightProbes__(ref LightProbes lightProbes, BinaryWriter writer, IAssetsExport mgr)
        {
            ResourceStream rs = ResourceStream.GetResType(lightProbes);
            switch (rs.type)
            {
            case ResType.Resources:
                break;
            case ResType.Builtin:
                rs.type = ResType.Empty;
                rs.hashcode = 0;
                rs.path = null;
                break;
            case ResType.Asset:
                mgr.CollectLightProbes(lightProbes);
                break;
            default:
                break;
            }

            rs.Write(writer);

            if (rs.GetRes())
                lightProbes = null;

            return rs.GetRes();
        }

        static public bool __CollectTexture__<T>(ref T texture, BinaryWriter writer, IAssetsExport mgr) where T : Texture
        {
            ResourceStream rs = ResourceStream.GetResType(texture);
            switch (rs.type)
            {
            case ResType.Resources:
                break;
            case ResType.Builtin:
                mgr.CollectBuiltinResource(texture);
                break;
            case ResType.Asset:
                mgr.CollectTexture(texture);
                break;
            default:
                break;
            }

            rs.Write(writer);

            if (rs.GetRes())
                texture = null;

            return rs.GetRes();
        }

        static public bool __CollectFont__(ref Font font, BinaryWriter writer, IAssetsExport mgr)
        {
            ResourceStream rs = ResourceStream.GetResType(font);
            switch (rs.type)
            {
            case ResType.Resources:
                break;
            case ResType.Builtin:
                mgr.CollectBuiltinResource(font);
                break;
            case ResType.Asset:
                mgr.CollectFontlib(font);
                break;
            default:
                break;
            }

            rs.Write(writer);
            if (rs.GetRes())
                font = null;

            return rs.GetRes();
        }

        static protected bool __CollectPrefab__(ref GameObject prefab, BinaryWriter writer, IAssetsExport mgr)
        {
            ResourceStream rs = ResourceStream.GetResType(prefab);
            switch (rs.type)
            {
            case ResType.Resources:
                break;
            case ResType.Builtin:
                rs.type = ResType.Empty;
                rs.hashcode = 0;
                rs.path = null;
                break;
            case ResType.Asset:
                mgr.CollectPrefab(prefab);
                break;
            default:
                break;
            }

            rs.Write(writer);
            if (rs.GetRes())
                prefab = null;

            return rs.GetRes();
        }

        static protected bool __CollectMono__<T>(ref T mono, BinaryWriter writer, IAssetsExport mgr) where T : Component
        {
            ResourceStream rs = ResourceStream.GetResType(mono == null ? null : mono.gameObject);
            switch (rs.type)
            {
            case ResType.Resources:
                break;
            case ResType.Builtin:
                rs.type = ResType.Empty;
                rs.hashcode = 0;
                rs.path = null;
                break;
            case ResType.Asset:
                mgr.CollectPrefab(mono.gameObject);
                break;
            default:
                break;
            }

            rs.Write(writer);
            if (rs.GetRes())
                mono = null;

            return rs.GetRes();
        }

        protected delegate bool __CollectRes__<T>(ref T asset, BinaryWriter writer, IAssetsExport mgr) where T : Object;

        static protected bool __CollectList__<T>(List<T> assets, BinaryWriter writer, IAssetsExport mgr, __CollectRes__<T> fun) where T : Object
        {
            bool has = false;
            writer.Write((byte)(assets != null ? assets.Count : 0));
            for (int i = 0; i < assets.Count; ++i)
            {
                T asset = assets[i];
                if (fun(ref asset, writer, mgr))
                {
                    assets[i] = null;
                    has = true;
                }
            }

            return has;
        }

        static protected bool __CollectList__<T1, T2>(List<T1> assets, System.Func<T1, T2> getFun, System.Action<T1, T2> setFun, BinaryWriter writer, IAssetsExport mgr, __CollectRes__<T2> fun) where T2 : Object
        {
            bool has = false;
            writer.Write((byte)(assets != null ? assets.Count : 0));
            for (int i = 0; i < assets.Count; ++i)
            {
                T2 asset = getFun(assets[i]);
                if (fun(ref asset, writer, mgr))
                {
                    setFun(assets[i], asset);
                    has = true;
                }
            }

            return has;
        }

        static protected bool __CollectList__<T>(T[] assets, BinaryWriter writer, IAssetsExport mgr, __CollectRes__<T> fun) where T : Object
        {
            bool has = false;
            byte num = (byte)(assets == null ? 0 : assets.Length);
            writer.Write(num);
            for (int i = 0; i < num; ++i)
            {
                if (fun(ref assets[i], writer, mgr))
                {
                    has = true;
                }
            }

            return has;
        }
#endif

#if UNITY_EDITOR && COM_DEBUG
        static void OnResourcesEnd<T>(T obj, object p) where T : Object
        {
            object[] ps = p as object[];
            Data data = ps[0] as Data;
            ResourcesEnd<T> fun = ps[1] as ResourcesEnd<T>;
            object funp = ps[2];

            data.mParamData.parent.AddDep(obj);

            fun(obj, funp);
        }
#endif

        static ResourceStream s_rs = new ResourceStream();

        static protected void __LoadObjects__(Data data, BinaryReader reader, System.Action<Object, int> fun)
        {
            int count = reader.ReadUInt16();
            if (count == 0)
                return;

            for (int i = 0; i < count; ++i)
            {
                int index = i;
                __LoadObject__(data, reader, (Object obj, object p) => { if (fun != null) fun(obj, index); data.OnEnd(); }, null);
            }
        }

        static protected void __LoadObject__(Data data, BinaryReader reader, ResourcesEnd<Object> fun, object funp)
        {
            byte type = reader.ReadByte();
            if (type == 0)
                return;

            switch (type)
            {
            case 1: __LoadSprite__(data, reader, (Sprite s, object p)=> {  if (fun != null) fun(s, p); }, funp); break;
            case 2: __LoadTexture__(data, reader, (Texture t, object p) => { if (fun != null) fun(t, p); }, funp); break;
            case 3: __LoadMaterial__(data, reader, (Material mat, object p) => { if (fun != null) fun(mat, p); }, funp); break;
            case 4: __LoadAudioClip__(data, reader, (AudioClip clip, object p) => { if (fun != null) fun(clip, p); }, funp); break;
            case 5: __LoadMesh__(data, reader, (Mesh m, object p) => { if (fun != null) fun(m, p); }, funp); break;
            case 6: __LoadFontlib__(data, reader, (Font font, object p) => { if (fun != null) fun(font, p); }, funp); break;
            case 7: __LoadPrefab__(data, reader, (GameObject go, object p) => { if (fun != null) fun(go, p); }, funp); break;
            case 8: __LoadMono__(data, reader, (GameObject go, object p) => { if (fun != null) fun(go, p); }, funp); break;
            default: Debug.LogErrorFormat("__LoadObject__ type:{0} error!", type); break;
            }
        }


        static protected void __LoadTMPFont__(Data data, BinaryReader reader, ResourcesEnd<TMPro.TMP_FontAsset> fun, object funp)
        {
            s_rs.Reader(reader);
            if (s_rs.GetRes())
            {
                string path = s_rs.Key;
#if UNITY_EDITOR && COM_DEBUG
                object p = new object[] { data, fun, funp };
                fun = OnResourcesEnd;
                funp = p;
#endif
                ++data.mTotal;
                data.mParamData.parent.AddDepKey(path);
                TMPFontLoad.Load(path, fun, funp);
            }
        }

        static protected void __LoadT2DAsset__(Data data, BinaryReader reader, ResourcesEnd<Texture2DAsset> fun, object funp)
        {
            s_rs.Reader(reader);
            if (s_rs.GetRes())
            {
                string path = s_rs.Key;
#if UNITY_EDITOR && COM_DEBUG
                object p = new object[] { data, fun, funp };
                fun = OnResourcesEnd;
                funp = p;
#endif
                ++data.mTotal;
                data.mParamData.parent.AddDepKey(path);
                T2DAssetLoad.Load(path, fun, funp);
            }
        }

        static protected void __LoadMesh__(Data data, BinaryReader reader, ResourcesEnd<Mesh> fun, object funp)
        {
            s_rs.Reader(reader);
            if (s_rs.GetRes())
            {
                string ms = s_rs.Key;
                if (string.IsNullOrEmpty(ms))
                {
                    Debuger.ErrorLog("Mesh:{0}", data.mComponent.name);
                    return;
                }

                ++data.mTotal;

#if UNITY_EDITOR && COM_DEBUG
                object p = new object[] { data, fun, funp };
                fun = OnResourcesEnd;
                funp = p;
#endif
                data.mParamData.parent.AddDepKey(ms);
                MeshLoad.Load(ms, fun, funp);
            }
        }

        static public void __LoadMaterial__(Data data, BinaryReader reader, ResourcesEnd<Material> fun, object funp)
        {
            s_rs.Reader(reader);
            if (s_rs.GetRes())
            {
                string mat = s_rs.Key;
                if (string.IsNullOrEmpty(mat))
                {
                    Debuger.ErrorLog("__LoadMaterial__:{0} null Root:{1}!", data.mComponent.name, data.mComponent.gameObject.transform.root.name);
                    return;
                }

                ++data.mTotal;
#if UNITY_EDITOR && COM_DEBUG
                object p = new object[] { data, fun, funp };
                fun = OnResourcesEnd;
                funp = p;
#endif
                data.mParamData.parent.AddDepKey(mat);
                MaterialLoad.Load(mat, fun, funp);
            }
        }

        static protected void __LoadLightProbes__(Data data, BinaryReader reader, ResourcesEnd<LightProbes> fun, object funp)
        {
            s_rs.Reader(reader);
            if (s_rs.GetRes())
            {
                string path = s_rs.Key;

#if UNITY_EDITOR && COM_DEBUG
                object p = new object[] { data, fun, funp };
                fun = OnResourcesEnd;
                funp = p;
#endif
                ++data.mTotal;
                data.mParamData.parent.AddDepKey(path);
                ResourcesLoad<LightProbes>.Load(path, fun, funp);
            }
        }

        static protected void __LoadAnimationClip__(Data data, BinaryReader reader, ResourcesEnd<AnimationClip> fun, object funp)
        {
            s_rs.Reader(reader);
            if (s_rs.GetRes())
            {
                string clip = s_rs.Key;

#if UNITY_EDITOR && COM_DEBUG
                object p = new object[] { data, fun, funp };
                fun = OnResourcesEnd;
                funp = p;
#endif
                ++data.mTotal;
                data.mParamData.parent.AddDepKey(clip);
                ResourcesLoad<AnimationClip>.Load(clip, fun, funp);
            }
        }

        static protected void __LoadAnimatorController__(Data data, BinaryReader reader, ResourcesEnd<RuntimeAnimatorController> fun, object funp)
        {
            s_rs.Reader(reader);
            if (s_rs.GetRes())
            {
                string clip = s_rs.Key;

#if UNITY_EDITOR && COM_DEBUG
                object p = new object[] { data, fun, funp };
                fun = OnResourcesEnd;
                funp = p;
#endif
                ++data.mTotal;
                data.mParamData.parent.AddDepKey(clip);
                RunAnimContLoad.Load(clip, fun, funp);
            }
        }

        static protected void __LoadAvatar__(Data data, BinaryReader reader, ResourcesEnd<Avatar> fun, object funp)
        {
            s_rs.Reader(reader);
            if (s_rs.GetRes())
            {
                string avatar = s_rs.Key;

                if (string.IsNullOrEmpty(avatar))
                {
                    Debuger.ErrorLog("Avatar:{0}", data.mComponent.name);
                    return;
                }

#if UNITY_EDITOR && COM_DEBUG
                object p = new object[] { data, fun, funp };
                fun = OnResourcesEnd;
                funp = p;
#endif
                ++data.mTotal;
                data.mParamData.parent.AddDepKey(avatar);
                ResourcesLoad<Avatar>.Load(avatar, fun, funp);
            }
        }

        static protected void __LoadAudioClip__(Data data, BinaryReader reader, ResourcesEnd<AudioClip> fun, object funp)
        {
            s_rs.Reader(reader);
            if (s_rs.GetRes())
            {
                string sound = s_rs.Key;
#if UNITY_EDITOR && COM_DEBUG
                object p = new object[] { data, fun, funp };
                fun = OnResourcesEnd;
                funp = p;
#endif
                ++data.mTotal;
                data.mParamData.parent.AddDepKey(sound);
                AudioClipLoad.Load(sound, fun, funp);
            }
        }

        static protected void __LoadShader__(Data data, BinaryReader reader, ResourcesEnd<Shader> fun, object funp)
        {
            s_rs.Reader(reader);
            if (s_rs.GetRes())
            {
                string path = s_rs.Key;

#if UNITY_EDITOR && COM_DEBUG
                object p = new object[] { data, fun, funp };
                fun = OnResourcesEnd;
                funp = p;
#endif
                ++data.mTotal;
                ShaderLoad.Load(path, fun, funp);
            }
        }
        static public void __LoadTexture__(Data data, BinaryReader reader, ResourcesEnd<Texture> fun, object funp)
        {
            s_rs.Reader(reader);
            if (s_rs.GetRes())
            {
                string path = s_rs.Key;
#if UNITY_EDITOR && COM_DEBUG
                object p = new object[] { data, fun, funp };
                fun = OnResourcesEnd;
                funp = p;
#endif
                ++data.mTotal;
                data.mParamData.parent.AddDepKey(path);
                TextureLoad.Load(path, fun, funp);
            }
        }

        static public bool Reader(BinaryReader reader, out string path)
        {
            path = string.Empty;
            s_rs.Reader(reader);
            if (s_rs.GetRes())
            {
                path = s_rs.Key;
                return true;
            }
            return false;
        }

        static protected void __LoadSprite__(Data data, BinaryReader reader, ResourcesEnd<Sprite> fun, object funp)
        {
            string res = reader.ReadString();
            if (!string.IsNullOrEmpty(res))
            {
#if UNITY_EDITOR && COM_DEBUG
                object p = new object[] { data, fun, funp };
                fun = OnResourcesEnd;
                funp = p;
#endif
                ++data.mTotal;
                SpritesLoad.Load(res, fun, funp);
            }
        }

        static protected void __LoadFontlib__(Data data, BinaryReader reader, ResourcesEnd<Font> fun, object funp)
        {
            s_rs.Reader(reader);
            if (s_rs.GetRes())
            {
                string name = s_rs.Key;
#if UNITY_EDITOR && COM_DEBUG
                object p = new object[] { data, fun, funp };
                fun = OnResourcesEnd;
                funp = p;
#endif
                ++data.mTotal;
                FontLibLoad.Load(name, fun, funp);
            }
        }

        static protected void __LoadPrefab__(Data data, BinaryReader reader, ResourcesEnd<GameObject> fun, object funp)
        {
            s_rs.Reader(reader);
            if (s_rs.GetRes())
            {
                string path = s_rs.Key;
#if UNITY_EDITOR && COM_DEBUG
                object p = new object[] { data, fun, funp };
                fun = OnResourcesEnd;
                funp = p;
#endif
                ++data.mTotal;
                data.mParamData.parent.AddDepKey(path);
                PrefabLoad.LoadFullPath(path, fun, funp, false);
            }
        }

        static protected void __LoadMono__(Data data, BinaryReader reader, ResourcesEnd<GameObject> fun, object funp)
        {
            __LoadPrefab__(data, reader, fun, funp);
        }

        protected delegate void LoadAsset<T>(Data data, BinaryReader reader, ResourcesEnd<T> fun, object funp) where T : Object;

        static protected int __LoadAssetList__<T>(int type, Data data, BinaryReader reader, LoadAsset<T> assetfun, ResourcesEnd<T> loadfun) where T : Object
        {
            int num = reader.ReadByte();
            for (int i = 0; i < num; ++i)
            {
                int index = i;
                object p = new object[] { data, index, type };
                assetfun(data, reader, loadfun, p);
            }

            return num;
        }

        static protected void __LoadMonoList__(int type, Data data, BinaryReader reader, ResourcesEnd<GameObject> fun)
        {
            int num = reader.ReadByte();
            for (int i = 0; i < num; ++i)
            {
                int index = i;
                object p = new object[] { data, index, type };

                __LoadPrefab__(data, reader, fun, p);
            }
        }
    }
#endif
}