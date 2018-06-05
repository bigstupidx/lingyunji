#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

public class ExtraNodes : MonoBehaviour
{
    [SerializeField]
    public GameObject[] nodes;

    public string[] paths
    {
        get
        {
            if (nodes == null)
                return new string[0];

            string[] ps = new string[nodes.Length];
            for (int i = 0; i < nodes.Length; ++i)
            {
                ps[i] = GetTranPath(nodes[i], transform);
            }

            return ps;
        }
    }

    public void SetPath(GameObject[] templates, string[] paths)
    {
        nodes = new GameObject[paths.Length];
        for (int i = 0; i < paths.Length; ++i)
        {
            string root;
            Transform child = transform;
            int last = paths[i].LastIndexOf('/');
            if (last != -1)
            {
                root = paths[i].Substring(0, last);
                child = transform.Find(root);
                if (child == null)
                {
                    Debug.LogErrorFormat("骨骼点:{0}查找失败!", root);
                    continue;
                }
            }

            GameObject goc = new GameObject();
            goc.name = templates[i].name;
            goc.transform.SetParent(child);

            goc.transform.localPosition = templates[i].transform.localPosition;
            goc.transform.localEulerAngles = templates[i].transform.localEulerAngles;
            goc.transform.localScale = templates[i].transform.localScale;
        }
    }

    public static string GetTranPath(GameObject go, Transform sr)
    {
        Transform root = sr;
        Transform current = go.transform;
        List<Transform> parents = new List<Transform>();
        Transform t = current;
        parents.Add(t);

        t = current.parent;
        while (t != null && t != root)
        {
            parents.Add(t);
            t = t.parent;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(parents[parents.Count - 1].name);
        for (int i = parents.Count - 2; i >= 0; --i)
            sb.AppendFormat("/{0}", parents[i].name);

        return sb.ToString();
    }
}
#endif