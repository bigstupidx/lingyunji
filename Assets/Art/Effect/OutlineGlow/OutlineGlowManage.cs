using UnityEngine;
using System.Collections;

/// <summary>
/// 避免添加多个描边效果，相互影响
/// </summary>
public class OutlineGlowManage : MonoBehaviour 
{
    //public OutlineGlowRenderer m_outline;

    ////一次只能有一个描边
    //public static void AddOutLine( GameObject go, OutlineGlowRenderer outline)
    //{
    //    if (go == null)
    //    {
    //        ObjectPool.DestroyImpl(outline.gameObject);
    //        return;
    //    }

    //    OutlineGlowManage outlineManage = go.GetComponent<OutlineGlowManage>();
    //    if (outlineManage == null)
    //        outlineManage = go.AddComponent<OutlineGlowManage>();

    //    if (outlineManage.m_outline != null)
    //    {
    //        ObjectPool.DestroyImpl(outlineManage.m_outline.gameObject);
    //    }
    //    outlineManage.m_outline = outline;
    //}
}
