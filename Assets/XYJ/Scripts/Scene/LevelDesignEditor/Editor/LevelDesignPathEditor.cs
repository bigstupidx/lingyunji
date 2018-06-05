using UnityEngine;
using UnityEditor;
using EditorExtensions;

[CustomEditor(typeof(LevelDesignPath))]
[CanEditMultipleObjects]
public class LevelDesignPathEditor : EditorX<LevelDesignPath>
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        LevelDesignEditor.Instance.Repaint();
    }

    protected override void MultipleEnable(LevelDesignPath target, string name)
    {
        if (gameobject.transform.parent != null && gameobject.transform.parent.parent != null)
        {
            LevelDesignLogic logic = gameobject.transform.parent.parent.GetComponent<LevelDesignLogic>();
            if (logic != null)
            {
                target.m_data.m_pathId = name;
                LevelDesignEditor.Instance.SetPathData();
                LevelDesignEditor.Instance.SortPath();
            }
        }
    }

    protected override void DestroyTargetFunction(LevelDesignPath target)
    {
        LevelDesignEditor.Instance.SetPathData();
        LevelDesignEditor.Instance.SortPath();
    }

    public override void OnInspectorGUI()
    {
        using (new AutoEditorDisabledGroup(true))
        {
            target.m_data.m_pathId = EditorGUILayout.TextField("路径id:", target.m_data.m_pathId);
        }
        target.m_data.m_saveToGround = EditorGUILayout.Toggle(target.m_data.m_saveToGround ? "保存贴地" : "保存不贴地", target.m_data.m_saveToGround);
        if (GUILayout.Button("增加路径", GUILayout.Height(50)))
        {
            target.AddPathToSet();
        }

        //时间
        DrawFloatFieldLayout(target.m_data.m_speed, value => target.SetDefaultSpeed(value), "默认速度");
        target.color = EditorGUILayout.ColorField("颜色", target.color);
        target.radius = EditorGUILayout.FloatField("半径:", target.radius);

        //绘制所有的区域
        for (int i = 0; i < gameobject.transform.childCount; ++i)
        {
            GUI.color = SetColor(i % 2 != 0);
            GameObject child = gameobject.transform.GetChild(i).gameObject;
            using (new AutoEditorVertical(EditorStylesEx.BoxArea))
            {
                using (new AutoEditorHorizontal())
                {
                    EditorGUILayout.LabelField("路径点 : " + child.name + "         ", GUILayout.Width(50f));
                    EditorGUILayout.ObjectField(child, typeof(GameObject), true, GUILayout.Width(100));
                }
                //关键点事件
                DrawTextLayout(child.GetComponent<LevelDesignPathItem>().m_event, value => target.SetEvent(child, value), "关键点事件");
                //停留时间
                DrawFloatFieldLayout(child.GetComponent<LevelDesignPathItem>().m_stayTime, value => target.SetStayTime(child, value), "停留时间");
                //去该点的速度
                DrawFloatFieldLayout(child.GetComponent<LevelDesignPathItem>().m_speed, value => target.SetSpeed(child, value), "速度");
            }
        }

        if (target.m_data.m_pathId != gameobject.name)
        {
            target.m_data.m_pathId = gameobject.name;
            LevelDesignEditor.Instance.SortPath();
            LevelDesignEditor.Instance.Repaint();
        }
    }
}
