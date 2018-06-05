using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PackTool
{
    public partial class MonoScriptAutoGen
    {
        #region
        const string File =
@"#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{{
    public class {0}PackData : ComponentData
    {{
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {{
            bool has = false;
            {4} com = component as {4};
{1}
            return has;
        }}
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {{
            Data data = CreateData(pd);
{2}
            return data;
        }}
{3}
    }}
}}
#endif";
        #endregion

        class Data
        {
            public StringBuilder collect = new StringBuilder(1000);
            public StringBuilder loadtext = new StringBuilder(1000);
            public StringBuilder loadendfun = new StringBuilder(1000);

            public StringBuilder prefabloadend = new StringBuilder(1000);
            public StringBuilder textureloadend = new StringBuilder(1000);
            public StringBuilder materialloadend = new StringBuilder(1000);
            public StringBuilder shaderloadend = new StringBuilder(1000);
            public StringBuilder meshloadend = new StringBuilder(1000);
            public StringBuilder animcliploadend = new StringBuilder(1000);
            public StringBuilder tmpfontAssetloadend = new StringBuilder(1000);
            public StringBuilder audiocliploadend = new StringBuilder(1000);
            public StringBuilder fontloadend = new StringBuilder(1000);
            public StringBuilder uifontloadend = new StringBuilder(1000);
            public StringBuilder uiatlasloadend = new StringBuilder(1000);
            public StringBuilder avatarloadend = new StringBuilder(1000);
            public StringBuilder lightProbesloadend = new StringBuilder(1000);
            public StringBuilder runanimconloadend = new StringBuilder(1000);
            public StringBuilder spriteloadend = new StringBuilder(1000);

            public StringBuilder prefabloadendlist = new StringBuilder(1000);
            public StringBuilder materialloadendlist = new StringBuilder(1000);
            public StringBuilder textureloadendlist = new StringBuilder(1000);
            public StringBuilder shaderloadendlist = new StringBuilder(1000);
            public StringBuilder meshloadendlist = new StringBuilder(1000);
            public StringBuilder animcliploadlist = new StringBuilder(1000);
            public StringBuilder audiocliploadlist = new StringBuilder(1000);
            public StringBuilder fontloadlist = new StringBuilder(1000);
            public StringBuilder uifontloadlist = new StringBuilder(1000);
            public StringBuilder uiatlasloadlist = new StringBuilder(1000);
            public StringBuilder avatarloadlist = new StringBuilder(1000);
            public StringBuilder lightProbesloadlist = new StringBuilder(1000);
            public StringBuilder runanimconloadlist = new StringBuilder(1000);
            public StringBuilder spriteloadlist = new StringBuilder(1000);
            public StringBuilder tmpfontAssetloadlist = new StringBuilder(1000);
        }

        static bool isInherited(System.Type type, System.Type baseType)
        {
            if (type == null)
                return false;

            if (type == baseType)
                return true;

            if (type.BaseType == null)
                return false;

            if (type.BaseType == baseType)
                return true;

            return isInherited(type.BaseType, baseType);
        }

        static bool isArrayInherited(System.Type type, System.Type baseType)
        {
            if (type.IsGenericType)//判断是否是泛型
            {
                // 为模版的
                List<System.Type> genericTypes = new List<System.Type>(type.GetGenericArguments());
                foreach (System.Type t in genericTypes)
                {
                    if (isInherited(t, baseType))
                        return true;
                }
            }
            else
            {
                if (isInherited(type.GetElementType(), baseType))
                    return true;
            }

            return false;
        }

        public static void Gen(MonoScript s)
        {
            System.Type type = s.GetClass();
            if (type == null)
                return;

            FieldInfo[] infos = type.GetFields();
            //StringBuilder funstring = new StringBuilder();

            Data data = new Data();

            bool hasPrefab = false;
            bool hasPrefabList = false;

            bool hasTexture = false;
            bool hasTextureList = false;

            bool hasMaterial = false;
            bool hasMaterialList = false;

            bool hasshader = false;
            bool hasshaderList = false;

            bool hasMesh = false;
            bool hasMeshList = false;

            bool hasAvatar = false;
            bool hasAvatarList = false;

            bool hasAnimClip = false;
            bool hasAnimClipList = false;

            bool hasRunAnimCon = false;
            bool hasRunAnimConList = false;

            bool hasAudioClip = false;
            bool hasAudioClipList = false;

            bool hasfont = false;
            bool hasfontlist = false;

            bool hasuifont = false;
            bool hasuifontlist = false;

            bool hasatlas = false;
            bool hasatlaslist = false;

            bool hassprite = false;
            bool hasspritelist = false;

            bool hastmpfont = false;
            bool hastmpfontlist = false;

            bool haslightProbes = false;
            bool haslightProbeslist = false;

            int index = 0;
            foreach (FieldInfo info in infos)
            {
                if (!info.IsDefined(typeof(PackAttribute), false))
                    continue;

                // 定义了
                if (info.FieldType == typeof(GameObject))
                {
                    Gen_Prefab(info, data, index);
                    hasPrefab = true;
                }
                else if (isInherited(info.FieldType, typeof(Texture)))
                {
                    Gen_Texture(info, data, index);
                    hasTexture = true;
                }
                else if (info.FieldType == typeof(Material))
                {
                    Gen_Material(info, data, index);
                    hasMaterial = true;
                }
                else if (info.FieldType == typeof(Shader))
                {
                    Gen_Shader(info, data, index);
                    hasshader = true;
                }
                else if (info.FieldType == typeof(Mesh))
                {
                    Gen_Mesh(info, data, index);
                    hasMesh = true;
                }
                else if (info.FieldType == typeof(Avatar))
                {
                    Gen_Avatar(info, data, index);
                    hasAvatar = true;
                }
                else if (info.FieldType == typeof(AnimationClip))
                {
                    Gen_AnimClip(info, data, index);
                    hasAnimClip = true;
                }
                else if (info.FieldType == typeof(RuntimeAnimatorController))
                {
                    Gen_RunAnimCon(info, data, index);
                    hasRunAnimCon = true;
                }
                else if (info.FieldType == typeof(AudioClip))
                {
                    Gen_AudioClip(info, data, index);
                    hasAudioClip = true;
                }
                else if (info.FieldType == typeof(Font))
                {
                    Gen_Font(info, data, index);
                    hasfont = true;
                }
                else if (info.FieldType == typeof(Sprite))
                {
                    Gen_Sprite(info, data, index);
                    hassprite = true;
                }
                else if (info.FieldType == typeof(TMPro.TMP_FontAsset))
                {
                    Gen_TMPFont(info, data, index);
                    hastmpfont = true;
                }
                else if (info.FieldType == typeof(LightProbes))
                {
                    Gen_LightProbes(info, data, index);
                    haslightProbes = true;
                }
                else if (isInherited(info.FieldType, typeof(Component)))
                {
                    Gen_MonoPrefab(info, data, index);
                    hasPrefab = true;
                }
                else if (info.FieldType == typeof(GameObject[]) || info.FieldType == typeof(List<GameObject>))
                {
                    Gen_PrefabList(info, data, index);
                    hasPrefabList = true;
                }
                else if (info.FieldType == typeof(Material[]) || info.FieldType == typeof(List<Material>))
                {
                    Gen_MaterialList(info, data, index);
                    hasMaterialList = true;
                }
                else if (isArrayInherited(info.FieldType, typeof(Texture)))
                {
                    Gen_TextureList(info, data, index);
                    hasTextureList = true;
                }
                else if (info.FieldType == typeof(Shader[]) || info.FieldType == typeof(List<Shader>))
                {
                    Gen_ShaderList(info, data, index);
                    hasshaderList = true;
                }
                else if (info.FieldType == typeof(Mesh[]) || info.FieldType == typeof(List<Mesh>))
                {
                    Gen_MeshList(info, data, index);
                    hasMeshList = true;
                }
                else if (info.FieldType == typeof(Avatar[]) || info.FieldType == typeof(List<Avatar>))
                {
                    Gen_AvatarList(info, data, index);
                    hasAvatarList = true;
                }
                else if (info.FieldType == typeof(AnimationClip[]) || info.FieldType == typeof(List<AnimationClip>))
                {
                    Gen_AnimClipList(info, data, index);
                    hasAnimClipList = true;
                }
                else if (info.FieldType == typeof(RuntimeAnimatorController[]) || info.FieldType == typeof(List<RuntimeAnimatorController>))
                {
                    Gen_RunAnimConList(info, data, index);
                    hasRunAnimConList = true;
                }
                else if (info.FieldType == typeof(AudioClip[]) || info.FieldType == typeof(List<AudioClip>))
                {
                    Gen_AudioClipList(info, data, index);
                    hasAudioClipList = true;
                }
                else if (info.FieldType == typeof(Font[]) || info.FieldType == typeof(List<Font>))
                {
                    Gen_FontList(info, data, index);
                    hasfontlist = true;
                }
                else if (info.FieldType == typeof(Sprite[]) || info.FieldType == typeof(List<Sprite>))
                {
                    Gen_SpriteList(info, data, index);
                    hasspritelist = true;
                }
                else if (info.FieldType == typeof(TMP_FontAssets[]) || info.FieldType == typeof(List<TMP_FontAssets>))
                {
                    Gen_TMPFontList(info, data, index);
                    hastmpfontlist = true;
                }
                else if (info.FieldType == typeof(LightProbes[]) || info.FieldType == typeof(List<LightProbes>))
                {
                    Gen_lightProbesList(info, data, index);
                    haslightProbeslist = true;
                }
                else if (isArrayInherited(info.FieldType, typeof(Component)))
                {
                    Gen_MonoList(info, data, index);
                    hasPrefabList = true;
                }
                else
                {
                    Debug.LogWarning("pack type not supper! type:" + info.FieldType.Name);
                }

                ++index;
            }

            if (hasPrefab == true)
            {
                Gen_PrefabFun(data, type);
            }

            if (hasTexture == true)
            {
                Gen_TextureFun(data, type);
            }

            if (hasMaterial == true)
            {
                Gen_MaterialFun(data, type);
            }
            
            if (hasshader == true)
            {
                Gen_ShaderFun(data, type);
            }

            if (hasPrefabList == true)
            {
                Gen_PrefabFunList(data, type);
            }

            if (hasMaterialList == true)
            {
                Gen_MaterialListFun(data, type);
            }

            if (hasTextureList == true)
            {
                Gen_TextureListFun(data, type);
            }

            if (hasshaderList == true)
            {
                Gen_ShaderListFun(data, type);
            }

            if (hasMesh == true)
            {
                Gen_MeshFun(data, type);
            }

            if (hasMeshList == true)
            {
                Gen_MeshFunList(data, type);
            }

            if (hasAvatar == true)
            {
                Gen_AvatarFun(data, type);
            }

            if (hasAvatarList == true)
            {
                Gen_AvatarFunList(data, type);
            }

            if (hasAnimClip == true)
            {
                Gen_AnimClipFun(data, type);
            }

            if (hasAnimClipList == true)
            {
                Gen_AnimClipFunList(data, type);
            }

            if (hasRunAnimCon == true)
            {
                Gen_RunAnimConFun(data, type);
            }

            if (hasRunAnimConList == true)
            {
                Gen_RunAnimConListFunList(data, type);
            }

            if (hasAudioClip == true)
            {
                Gen_AudioClipFun(data, type);
            }

            if (hasAudioClipList == true)
            {
                Gen_AudioClipFunList(data, type);
            }

            if (hasfont == true)
            {
                Gen_FontFun(data, type);
            }

            if (hasfontlist == true)
            {
                Gen_FontFunList(data, type);
            }

            if (hasuifont == true)
            {
                Gen_UIFontFun(data, type);
            }

            if (hasuifontlist == true)
            {
                Gen_UIFontFunList(data, type);
            }

            if (hasatlas == true)
            {
                Gen_UIAtlasFun(data, type);
            }

            if (hasatlaslist == true)
            {
                Gen_UIAtlasFunList(data, type);
            }

            if (hassprite == true)
            {
                Gen_SpriteFun(data, type);
            }

            if (hasspritelist == true)
            {
                Gen_SpriteFunList(data, type);
            }

            if (hastmpfont == true)
            {
                Gen_TMPFontFun(data, type);
            }

            if (hastmpfontlist == true)
            {
                Gen_TMPFontFunList(data, type);
            }
            if (haslightProbes == true)
            {
                Gen_LightProbesFun(data, type);
            }

            if (haslightProbeslist == true)
            {
                Gen_lightProbesListFun(data, type);
            }

            if (index != 0)
            {
                string objfile = AssetDatabase.GetAssetPath(s);
                objfile = System.IO.Path.GetFileNameWithoutExtension(objfile) + "PackData.cs";
                //string assetfile = "Assets/Scripts/ThirdPartyLibrary/PackTool/Component/Auto/" + objfile;
                string dstfilename = Application.dataPath + "/Scripts/ThirdPartyLibrary/PackTool/Component/Auto/" + objfile;

                string contents = string.Format(File, type.Name, data.collect.ToString(), data.loadtext.ToString(), data.loadendfun.ToString(), GetTypeName(type));

                dstfilename = dstfilename.Replace('\\', '/');
                System.IO.Directory.CreateDirectory(dstfilename.Substring(0, dstfilename.LastIndexOf('/')));

                System.IO.File.WriteAllText(dstfilename, contents, System.Text.Encoding.UTF8);
            }
        }
    }
}