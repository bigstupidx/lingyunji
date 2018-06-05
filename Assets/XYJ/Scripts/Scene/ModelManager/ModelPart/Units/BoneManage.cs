/*----------------------------------------------------------------
// 创建者：
// 创建日期:
// 模块描述：骨骼管理
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class BoneManage
#if MEMORY_CHECK
        : MemoryObject
#endif
{
    Dictionary<string, Transform> m_list = new Dictionary<string,Transform>();
    Transform m_root;

    public BoneManage( GameObject go )
    {
        m_root = go.transform;
    }

    public void Clear()
    {
        m_root = null;
        m_list.Clear();
    }

    public Transform GetRoot()
    {
        return m_root;
    }

    // 查找骨骼
    public Transform GetBone(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        Transform ret;
        if(m_list.TryGetValue(name,out ret))
            return ret;
        else
        {
            ret = FindBoneImpl(m_root, ref name);
            m_list.Add(name, ret);
            return ret;
        }
    }

    // 查找骨骼
    public static Transform GetBone(Transform parent, string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        return FindBoneImpl(parent, ref name);
    }

    // 查找骨骼
    static Transform FindBoneImpl(Transform parent, ref string name)
    {
        Transform t = parent.Find(name);
        if (t != null)
            return t;

        int count = parent.childCount;
        for (int i = 0; i < count; ++i)
        {
            t = FindBoneImpl(parent.GetChild(i), ref name);
            if (t != null)
                return t;
        }

        return null;
    } 

    //获得完整名字,打印信息用
    static public string GetFullGoPath( Transform trans )
    {
        StringBuilder path = new StringBuilder();
        path.Insert(0, trans.name);
        trans = trans.parent;
        while(trans!=null)
        {
            path.Insert(0,trans.name+"\\");
            trans = trans.parent;
        }
        return path.ToString();
    }
}
