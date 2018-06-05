#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys
{
    [System.Serializable]
    class TestSEr
    {
        public string name;
    }

    [AutoILMono]
    [System.Serializable]
    class TestIL
    {
        [SerializeField]
        int[] intValue;
        // 
        //         [SerializeField]
        //         uint[] uintValue;

        //[SerializeField]
        //short[] shortValue;

        //[SerializeField]
        //ushort[] ushortValue;

        //[SerializeField]
        //sbyte[] sbyteValue;

        //[SerializeField]
        //byte[] byteValue;

        //[SerializeField]
        //float[] floatValue;

        //[SerializeField]
        //double[] doubleValue;

        //[SerializeField]
        //long[] longValue;

        //[SerializeField]
        //ulong[] ulongValue;

        //[SerializeField]
        //string[] stringValue;

        [SerializeField]
        GameObject go;

        [SerializeField]
        Texture2D texture2D;

        //public TestSEr[] testSers = new TestSEr[10];
        //public TestXYS[] testXYSs;


        void Awake()
        {
            //Debug.LogFormat("texture2D:{0} stringValue:{1}", texture2D, stringValue);
        }
    }
}
#endif