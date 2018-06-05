using System;
using UnityEngine;
using System.Collections.Generic;

#pragma warning disable 649

// ÑÓ³Ù¼¤»î
public class Delays : MonoBehaviour
{
    [Serializable]
    class Data
    {
        public GameObject obj;
        public float delay = 1f;
    }

    [SerializeField]
    List<Data> objs = new List<Data>();

    List<Data> setObjs;

    float last_time = -1;

    void OnEnable()
    {
        last_time = Time.realtimeSinceStartup;
        setObjs = new List<Data>();

        objs.ForEach((Data d) =>
        {
            if (d.obj != null)
            {
                d.obj.SetActive(false);
                setObjs.Add(d);
            }
        });

        setObjs.Sort((Data x, Data y) => { return y.delay.CompareTo(x.delay); });
    }

    void OnDisable()
    {
        setObjs = null;
        last_time = -1;
    }

    void Update()
    {
        if (setObjs == null || setObjs.Count == 0)
        {
            //enabled = false;
            return;
        }

        float delay = Time.realtimeSinceStartup - last_time;
        for (int i = setObjs.Count - 1; i >= 0; --i)
        {
            Data d = setObjs[i];
            if (d.obj == null || d.obj.activeSelf)
            {
                setObjs.RemoveAt(i);
                return;
            }

            if (delay >= d.delay)
            {
                d.obj.SetActive(true);
                setObjs.RemoveAt(i);
            }
            else
            {
                break;
            }
        }
    }
}
