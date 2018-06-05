using UnityEngine;
using UnityEditor;
using xys;

//圆形滑动列表
[CustomEditor(typeof(CircularController))]
[ExecuteInEditMode]
public class CircularEditor : EditorX<CircularController>
{
    void OnEnable()
    {
        if (null == target.m_circleCenter)
        {
            GameObject go = target.CreateObj(target.transform, "CircelCenter");
            target.SetCircleCenter(go.transform);
        }

        if (null == target.m_layout)
        {
            GameObject go = target.CreateObj(target.transform, "Layout");
            target.SetLayout(go.AddMissingComponent<RectTransform>());
        }

        if(null == target.m_defaultItem)
        {
            GameObject go = target.CreateObj(target.m_layout, "DefaultItem");
            target.SetDefaultItem(go);
        }

        target.SetRadius(target.Radius);
        target.InitComponent();
    }

    void OnDisable()
    {
        Tools.hidden = false;
    }

    public override void OnInspectorGUI()
    {
        //圆心
        DrawObjectLayout(target.m_circleCenter, typeof(Transform), value => target.SetCircleCenter(value), "圆心");
        //layout
        DrawObjectLayout(target.m_layout, typeof(RectTransform), value => target.SetLayout(value), "Layout");
        //默认物体
        DrawObjectLayout(target.m_defaultItem, typeof(GameObject), value => target.SetDefaultItem(value), "DefaultItem");
        //半径
        DrawFloatFieldLayout(target.Radius, value => target.SetRadius(value), "半径");
        //类型
        DrawEnumPopupLayout(target.m_ciraularType, value => target.SetCircularType(value), "类型");
    }
}
