//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;

//namespace XTools
//{
//    public class SceneDestroy
//    {
//        static SceneDestroy destroyRoot = new SceneDestroy(); // 根结点
//        //static SceneDestroy dontDestroyRoot = new SceneDestroy(); // 根结点

//        bool isVaild()
//        {
//            if (cacheObj == null)
//                return false;
//            return true;
//        }

//        public Transform cacheTrans { get; protected set; }
//        public GameObject cacheObj { get; protected set; }

//        static Dictionary<string, SceneDestroy> SceneList = new Dictionary<string, SceneDestroy>();

//        public static void DestroyAllScene()
//        {
//            SceneList.Clear();
//            if (!destroyRoot.isVaild())
//                return;

//            Object.Destroy(destroyRoot.cacheObj);
//        }

//        public static SceneDestroy GetDefault()
//        {
//            if (destroyRoot.isVaild())
//                return destroyRoot;

//            destroyRoot.cacheObj = new GameObject("DestroyRoot");
//            destroyRoot.cacheTrans = destroyRoot.cacheObj.transform;
//            return destroyRoot;
//        }

//        static SceneDestroy Get(string name)
//        {
//            GetDefault();

//            SceneDestroy sd = null;
//            if (SceneList.TryGetValue(name, out sd))
//            {
//                //防止误删除
//                if(sd==null)
//                {
//                    GameObject goTem = new GameObject(name);
//                    sd.cacheObj = goTem;
//                    sd.cacheTrans = goTem.transform;
//                    sd.cacheTrans.parent = destroyRoot.cacheTrans;
//                }
//                return sd;
//            }


//            GameObject go = new GameObject(name);
//            sd = new SceneDestroy();
//            sd.cacheObj = go;
//            sd.cacheTrans = go.transform;
//            sd.cacheTrans.parent = destroyRoot.cacheTrans;
//            SceneList.Add(name, sd);

//            return sd;
//        }

//        public static GameObject GetObj(string name)
//        {
//            return Get(name).cacheObj;
//        }

//        public static Transform GetTran(string name)
//        {
//            return Get(name).cacheTrans;
//        }

//        public static void SetDontDestroyOnLoad( GameObject go )
//        {
////             if (!dontDestroyRoot.isVaild())
////             {
////                 dontDestroyRoot.cacheObj = new GameObject("DontDestroyOnLoad");
////                 dontDestroyRoot.cacheTrans = dontDestroyRoot.cacheObj.transform;

////#if !USE_RESOURCESEXPORT
//            Object.DontDestroyOnLoad(go);
////#endif
////            }

//            //go.transform.parent = dontDestroyRoot.cacheTrans;
//        }
//    }
//}
