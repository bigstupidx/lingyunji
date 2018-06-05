#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ShowAllMono : MonoBehaviour
{
    [SerializeField]
    public GameObject[] roots;

    [System.Serializable]
    public class Data
    {
        public string Name;
        public System.Type type;
#if UNITY_EDITOR
        public UnityEditor.MonoScript script;
#endif

        public List<Component> monos;
    }

    [SerializeField]
    public List<Data> Datas = new List<Data>();

    [SerializeField]
    public List<GameObject> nullMonos = new List<GameObject>();
}

