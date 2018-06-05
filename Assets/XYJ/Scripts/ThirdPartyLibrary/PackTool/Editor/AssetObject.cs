#define ASSETPATH // 使用资源路径

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public interface AssetObjectinterface
    {
        Object Get();
    }

    public class AssetObject<T> : AssetObjectinterface where T : Object
    {
        public AssetObject(T obj)
        {
#if ASSETPATH
            assetPath = AssetDatabase.GetAssetPath(obj);
#else
            asset = obj;
#endif
        }

        public AssetObject(string path)
        {
#if ASSETPATH
            assetPath = path;
#else
            asset = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
#endif
        }

#if ASSETPATH
        public string assetPath { get; protected set; } // 资源路径
#else
        public T asset;
        public string assetPath { get { return AssetDatabase.GetAssetPath(asset); } } // 资源路径
#endif

        public Object Get()
        {
#if ASSETPATH
            return AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
#else
            return asset;
#endif
        }
    }

    public class ComponentObject<T> : AssetObjectinterface where T : MonoBehaviour
    {
        public ComponentObject(T mono)
        {
#if ASSETPATH
            assetPath = AssetDatabase.GetAssetPath(mono.gameObject);
#else
            asset = mono;
#endif
        }

        public ComponentObject(GameObject obj)
        {
#if ASSETPATH
            assetPath = AssetDatabase.GetAssetPath(obj);
#else
            asset = obj.GetComponent<T>();
#endif
        }

#if ASSETPATH
        public string assetPath { get; protected set; } // 资源路径
#else
        public T asset;
#endif

        public Object Get()
        {
            return GetValue();
        }

        public T GetValue()
        {
#if ASSETPATH
            GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
            return obj.GetComponent<T>();
#else
            return asset;
#endif
        }

        public static List<ComponentObject<T>> SwitchList(List<T> list)
        {
            List<ComponentObject<T>> rs = new List<ComponentObject<T>>();
            foreach (T o in list)
            {
                rs.Add(new ComponentObject<T>(o));
            }

            return rs;
        }
    }
}

#endif