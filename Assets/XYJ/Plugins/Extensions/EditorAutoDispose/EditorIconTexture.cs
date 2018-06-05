#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EditorExtensions
{
    public enum IconTextureType
    {
        Folder_Icon,
        AudioSource_Icon,
        Camera_Icon,
        Windzone_Icon,
        GameObject_Icon,

        //_file_suffix_icon
        boo_Script_Icon,
        CGProgram_Icon,
        cs_Script_Icon,
        GUISkin_Icon,
        Js_Script_Icon,
        Material_Icon,
        PhysicMaterial_Icon,
        PrefabNormal_Icon,
        Shader_Icon,
        TextAsset_Icon,
        SceneAsset_Icon,
        ameManager_Icon,
        Animation_Icon,
        MetaFile_Icon,
        AudioMixerController_Icon,
        Font_Icon,
        AudioClip_Icon,
        Texture_Icon,
        Mesh_Icon,
        MovieTexture_Icon,
        ScriptableObject_Icon,
        DefaultAsset_Icon,

        AudioMixerSnapshot_Icon,
        AudioMixerGroup_Icon,
        AudioMixerView_Icon,
        AudioListener_Icon,

        Search_Icon,
        Favorite_Icon,
        FolderFavorite_Icon,

        Favorite,
        VisibilityOn,
        CustomSorting,
        DefaultSorting,

        SceneLoadIn,
        SceneLoadOut,
        SceneSave,

        LookDevLight,
        LookDevShadowFrame,
        LookDevShadow,
        LookDevObjRotation,
        LookDevEnvRotation,
        LookDevSingle1,
        LookDevSingle2,
        LookDevSideBySide,
        LookDevSplit,
        LookDevZone,

        FilterByLabel,
        FilterByType,
        UnityEditor_HierarchyWindow,
        console_warnicon_sml,
    }
    
    /// <summary>
    /// 图标贴图
    /// </summary>
    public class EditorIconTexture
    {
        // Texture2D，key为图片名
        static Dictionary<string, Texture2D> _iconTextureCache = new Dictionary<string, Texture2D>();

        /// <summary>
        /// 数量
        /// </summary>
        public static int Count
        {
            get { return _iconTextureCache.Count; }
        }

        /// <summary>
        /// This function will look in Assets/Editor Default Resources/ + path for the resource. If not there, it will try the built-in editor resources by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Texture2D Get(string name)
        {
            if (_iconTextureCache.ContainsKey(name))
                return _iconTextureCache[name];

            Texture2D tex = (Texture2D)EditorGUIUtility.Load(name);
            if (!tex)
                return null;
            _iconTextureCache.Add(name, tex);
            return tex;
        }

        public static Texture2D GetCustom(string name)
        {
            if (_iconTextureCache.ContainsKey(name))
                return _iconTextureCache[name];

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(string.Format("Assets/Editor/EditorExtensions/EditorTextures/{0}.png", name));
            if (!tex)
                return null;
            _iconTextureCache.Add(name, tex);
            return tex;
        }

        public static Texture2D GetSystem(IconTextureType type)
        {
            string name = IconTextures[(int)type];
            if (_iconTextureCache.ContainsKey(name))
                return _iconTextureCache[name];

            Texture2D tex = EditorGUIUtility.FindTexture(name);
            if (!tex)
                return null;
            _iconTextureCache.Add(name, tex);
            return tex;
        }

#region System Icon Textures

        static string[] IconTextures =
        {
            "Folder Icon",
            "AudioSource Icon",
            "Camera Icon",
            "Windzone Icon",
            "GameObject Icon",

            // file suffix icon
            "boo Script Icon",
            "CGProgram Icon",
            "cs Script Icon",
            "GUISkin Icon",
            "Js Script Icon",
            "Material Icon",
            "PhysicMaterial Icon",
            "PrefabNormal Icon",
            "Shader Icon",
            "TextAsset Icon",
            "SceneAsset Icon",
            "GameManager Icon",
            "Animation Icon",
            "MetaFile Icon",
            "AudioMixerController Icon",
            "Font Icon",
            "AudioClip Icon",
            "Texture Icon",
            "Mesh Icon",
            "MovieTexture Icon",
            "ScriptableObject Icon",
            "DefaultAsset Icon",

            "AudioMixerSnapshot Icon",
            "AudioMixerGroup Icon",
            "AudioMixerView Icon",
            "AudioListener Icon",

            
            "Search Icon",
            "Favorite Icon",
            "FolderFavorite Icon",

            "Favorite",
            "VisibilityOn",
            "CustomSorting",
            "DefaultSorting",
            
            "SceneLoadIn",
            "SceneLoadOut",
            "SceneSave",

            "LookDevLight",
            "LookDevShadowFrame",
            "LookDevShadow",
            "LookDevObjRotation",
            "LookDevEnvRotation",
            "LookDevSingle1",
            "LookDevSingle2",
            "LookDevSideBySide",
            "LookDevSplit",
            "LookDevZone",

            "FilterByLabel",
            "FilterByType",
            "UnityEditor.HierarchyWindow",
            "console.warnicon.sml",
        };

#endregion

    }
}
#endif