#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace XTools
{
    public class GlobalCoroutine
    {
        static List<IEnumerator> CoroutineList = new List<IEnumerator>();

        static bool IsInit = false;
        public static void StartCoroutine(IEnumerator routine)
        {
            Init();
            if (routine.MoveNext())
            {
                CoroutineList.Add(routine);
            }
        }

        static void Init()
        {
            if (!IsInit || UnityEditor.EditorApplication.update == null)
            {
                IsInit = true;
                UnityEditor.EditorApplication.update += Update;
            }
        }

        static void UnInit()
        {
            if (IsInit)
            {
                IsInit = false;
                UnityEditor.EditorApplication.update -= Update;
//                 lastTimeSinceStartup = -1;
//                 frameCount = 0;
            }
        }

//         static double lastTimeSinceStartup = -1;
//         public static float deltaTime { get; private set; }
//         public static int frameCount { get; private set; }

        static void Update()
        {
//             ++frameCount;
//             double current = UnityEditor.EditorApplication.timeSinceStartup;
//             if (lastTimeSinceStartup == -1)
//             {
//                 lastTimeSinceStartup = current;
//             }

            if (CoroutineList.Count != 0)
            {
                List<IEnumerator> list = new List<IEnumerator>(CoroutineList);
                CoroutineList.Clear();
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].MoveNext())
                        CoroutineList.Add(list[i]);
                }
            }
            else
            {
                UnInit();
            }

            //deltaTime = (float)(current - lastTimeSinceStartup);
            //Debug.Log("deltaTime:" + deltaTime);
            //lastTimeSinceStartup = current;
        }
    }
}

#endif