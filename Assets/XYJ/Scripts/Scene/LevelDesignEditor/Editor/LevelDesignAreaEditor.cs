using UnityEngine;
using UnityEditor;
using EditorExtensions;

[CustomEditor(typeof(LevelDesignArea))]
[CanEditMultipleObjects]
public class LevelDesignAreaEditor : EditorX<LevelDesignArea>
{
    static string[] GetAreaType()
    {
        string[] areaType = new string[]
        {
            "矩形",
            "圆形",
        };

        return areaType;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //target.m_data.m_areaSetId = gameobject.name;

        //Add();
        //LevelDesignEditor.Instance.Repaint();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        LevelDesignEditor.Instance.Repaint();
    }

    protected override void MultipleEnable(LevelDesignArea target, string name)
    {
        if (gameobject.transform.parent != null && gameobject.transform.parent.parent != null)
        {
            LevelDesignLogic logic = gameobject.transform.parent.parent.GetComponent<LevelDesignLogic>();
            if (logic != null)
            {
                target.m_data.m_areaSetId = name;
                //if (LevelDesignEditor.Instance.AddArea(target.m_data, logic.m_data))
                LevelDesignEditor.Instance.SetAreaData();
                LevelDesignEditor.Instance.SortArea();
            }
        }
    }

    protected override void DestroyTargetFunction(LevelDesignArea target)
    {
        //LevelDesignEditor.Instance.MinusArea(target.m_data);
        LevelDesignEditor.Instance.SetAreaData();
        LevelDesignEditor.Instance.SortArea();
    }

    public override void OnInspectorGUI()
    {
        using (new AutoEditorDisabledGroup(true))
        {
            target.m_data.m_areaSetId = EditorGUILayout.TextField("区域id:", target.m_data.m_areaSetId);
        }
        //target.m_data.m_saveToGround = EditorGUILayout.Toggle(target.m_data.m_saveToGround ? "保存贴地" : "保存不贴地", target.m_data.m_saveToGround);
        if (GUILayout.Button("增加区域", GUILayout.Height(50)))
        {
            target.AddAreaToSet();
        }

        //绘制所有的区域
        for (int i = 0; i < gameobject.transform.childCount; ++i)
        {
            GUI.color = SetColor(i % 2 != 0);
            GameObject child = gameobject.transform.GetChild(i).gameObject;
            using (new AutoEditorHorizontal())
            {
                EditorGUILayout.LabelField("区域 : " + child.name + "         ", GUILayout.Width(50f));
                EditorGUILayout.ObjectField(child, typeof(GameObject), true, GUILayout.Width(100));

                DrawPopupLayout((int)target.GetAreaType(child), GetAreaType(), value => target.SetColliderType(child, value), "区域类型");
            }
        }

        if (target.m_data.m_areaSetId != gameobject.name)
        {
            target.m_data.m_areaSetId = gameobject.name;
            LevelDesignEditor.Instance.SortArea();
            LevelDesignEditor.Instance.Repaint();
        }
    }

    void Add()
    {
        if (gameobject.transform.parent != null && gameobject.transform.parent.parent != null)
        {
            LevelDesignLogic logic = gameobject.transform.parent.parent.GetComponent<LevelDesignLogic>();
            if (logic != null)
            {
                LevelDesignEditor.Instance.AddArea(target.m_data, logic.m_data);
                return;
            }
        }
    }
}
