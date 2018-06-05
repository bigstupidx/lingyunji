#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace EditorExtensions
{
    public class EditorUtilityEx
    {
        // Json文件后缀
        public const string JsonFileSuffix = ".json";

        /// <summary>
        /// 保存json 对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="filePath"></param>
        public static void SaveJson<T>(T obj, string filePath)
        {
            string fullPath = filePath;
            if (!fullPath.EndsWith(JsonFileSuffix))
                fullPath += JsonFileSuffix;
            string json = EditorJsonUtility.ToJson(obj);
            File.WriteAllText(fullPath, json, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 加在json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T LoadJson<T> (string filePath)
        {
            string fullPath = filePath;
            if (!fullPath.EndsWith(JsonFileSuffix))
                fullPath += JsonFileSuffix;
            if (!File.Exists(fullPath))
            {
                Debug.LogError("加载Json文件失败！路径：" + fullPath);
                return default(T);
            }
            string json = File.ReadAllText(fullPath, System.Text.Encoding.UTF8);
            return JsonUtility.FromJson<T>(json);
        }

        // Overwrite data in an object by reading from its JSON representation.
        // This is similar to [JsonUtility.FromJsonOverwrite], but it supports any engine object. 
        public static void LoadJsonOverwrite(string filePath, object objectOverwrite)
        {
            string fullPath = filePath;
            if (!fullPath.EndsWith(JsonFileSuffix))
                fullPath += JsonFileSuffix;
            if (!File.Exists(fullPath))
            {
                Debug.LogError("加载Json文件失败！路径：" + fullPath);
                return;
            }
            string json = File.ReadAllText(fullPath, System.Text.Encoding.UTF8);
            EditorJsonUtility.FromJsonOverwrite(json, objectOverwrite);
        }

        /// <summary>
        /// 设置对象被修改
        /// </summary>
        /// <param name="obj"></param>
        public static void SetDirty(UnityEngine.Object obj)
        {
            if (obj == null)
                return;
            //如果是预制体或者其他资源
            if (EditorUtility.IsPersistent(obj))
            {
                EditorUtility.SetDirty(obj);
                //AssetDatabase.SaveAssets();
            }
            //如果是场景中的对象
            else if (!Application.isPlaying)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();///FIX. 5.3以后要加这一句，不然修改场景中的对象无效
            }
        }

        /// <summary>
        /// 获取枚举列表的描述
        /// 命名空间：using System.ComponentModel;
        /// 枚举项添加属性：[Description("描述")]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] GetEnumDescArray<T>()
        {
            System.Type type = typeof(T);

            if (!type.IsEnum)
            {
                Debug.LogError("模板参数必须是枚举");
                return new string[] { };
            }

            string[] names = System.Enum.GetNames(type);
            string[] descs = new string[names.Length];

            for (int i = 0; i < names.Length; ++i)
            {
                string name = names[i];
                System.Reflection.FieldInfo fi = type.GetField(name);
                System.ComponentModel.DescriptionAttribute[] arrDesc = (System.ComponentModel.DescriptionAttribute[])fi.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                descs[i] = arrDesc != null && arrDesc.Length > 0 ? arrDesc[0].Description : "";
            }

            return descs;
        }

        // 获取icon图片
        public static Texture2D GetIconForFile(string fileName)
        {
            int num = fileName.LastIndexOf('.');
            string text = (num != -1) ? fileName.Substring(num + 1).ToLower() : "";
            return GetIconBySuffix(text);
        }
        // 获取icon图片
        public static Texture2D GetIconBySuffix(string suffix)
        {
            Texture2D result;
            switch (suffix)
            {
                case "boo":
                    result = EditorGUIUtility.FindTexture("boo Script Icon");
                    return result;
                case "cginc":
                    result = EditorGUIUtility.FindTexture("CGProgram Icon");
                    return result;
                case "cs":
                    result = EditorGUIUtility.FindTexture("cs Script Icon");
                    return result;
                case "guiskin":
                    result = EditorGUIUtility.FindTexture("GUISkin Icon");
                    return result;
                case "js":
                    result = EditorGUIUtility.FindTexture("Js Script Icon");
                    return result;
                case "mat":
                    result = EditorGUIUtility.FindTexture("Material Icon");
                    return result;
                case "physicmaterial":
                    result = EditorGUIUtility.FindTexture("PhysicMaterial Icon");
                    return result;
                case "prefab":
                    result = EditorGUIUtility.FindTexture("PrefabNormal Icon");
                    return result;
                case "shader":
                    result = EditorGUIUtility.FindTexture("Shader Icon");
                    return result;
                case "txt":
                    result = EditorGUIUtility.FindTexture("TextAsset Icon");
                    return result;
                case "unity":
                    result = EditorGUIUtility.FindTexture("SceneAsset Icon");
                    return result;
                case "asset":
                case "prefs":
                    result = EditorGUIUtility.FindTexture("GameManager Icon");
                    return result;
                case "anim":
                    result = EditorGUIUtility.FindTexture("Animation Icon");
                    return result;
                case "meta":
                    result = EditorGUIUtility.FindTexture("MetaFile Icon");
                    return result;
                case "mixer":
                    result = EditorGUIUtility.FindTexture("AudioMixerController Icon");
                    return result;
                case "ttf":
                case "otf":
                case "fon":
                case "fnt":
                    result = EditorGUIUtility.FindTexture("Font Icon");
                    return result;
                case "aac":
                case "aif":
                case "aiff":
                case "au":
                case "mid":
                case "midi":
                case "mp3":
                case "mpa":
                case "ra":
                case "ram":
                case "wma":
                case "wav":
                case "wave":
                case "ogg":
                    result = EditorGUIUtility.FindTexture("AudioClip Icon");
                    return result;
                case "ai":
                case "apng":
                case "png":
                case "bmp":
                case "cdr":
                case "dib":
                case "eps":
                case "exif":
                case "gif":
                case "ico":
                case "icon":
                case "j":
                case "j2c":
                case "j2k":
                case "jas":
                case "jiff":
                case "jng":
                case "jp2":
                case "jpc":
                case "jpe":
                case "jpeg":
                case "jpf":
                case "jpg":
                case "jpw":
                case "jpx":
                case "jtf":
                case "mac":
                case "omf":
                case "qif":
                case "qti":
                case "qtif":
                case "tex":
                case "tfw":
                case "tga":
                case "tif":
                case "tiff":
                case "wmf":
                case "psd":
                case "exr":
                case "hdr":
                    result = EditorGUIUtility.FindTexture("Texture Icon");
                    return result;
                case "3df":
                case "3dm":
                case "3dmf":
                case "3ds":
                case "3dv":
                case "3dx":
                case "blend":
                case "c4d":
                case "lwo":
                case "lws":
                case "ma":
                case "max":
                case "mb":
                case "mesh":
                case "obj":
                case "vrl":
                case "wrl":
                case "wrz":
                case "fbx":
                    result = EditorGUIUtility.FindTexture("Mesh Icon");
                    return result;
                case "asf":
                case "asx":
                case "avi":
                case "dat":
                case "divx":
                case "dvx":
                case "mlv":
                case "m2l":
                case "m2t":
                case "m2ts":
                case "m2v":
                case "m4e":
                case "m4v":
                case "mjp":
                case "mov":
                case "movie":
                case "mp21":
                case "mp4":
                case "mpe":
                case "mpeg":
                case "mpg":
                case "mpv2":
                case "ogm":
                case "qt":
                case "rm":
                case "rmvb":
                case "wmw":
                case "xvid":
                    result = EditorGUIUtility.FindTexture("MovieTexture Icon");
                    return result;
                case "colors":
                case "gradients":
                case "curves":
                case "curvesnormalized":
                case "particlecurves":
                case "particlecurvessigned":
                case "particledoublecurves":
                case "particledoublecurvessigned":
                    result = EditorGUIUtility.FindTexture("ScriptableObject Icon");
                    return result;
            }
            result = EditorGUIUtility.FindTexture("DefaultAsset Icon");
            return result;
        }

    }
}
#endif