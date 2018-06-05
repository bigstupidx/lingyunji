using UnityEngine;
using UnityEditor;
using EditorExtensions;
using System.Collections.Generic;

[CustomEditor(typeof(LevelDesignPoint))]
[CanEditMultipleObjects]
public class LevelDesignPointEditor : EditorX<LevelDesignPoint>
{
    protected override void OnEnable()
    {
        base.OnEnable();
        //target.m_data.m_pointSetId = gameobject.name;

        //Add();
        //LevelDesignEditor.Instance.Repaint();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        LevelDesignEditor.Instance.Repaint();
    }

    protected override void MultipleEnable(LevelDesignPoint target, string name)
    {
        if (gameobject.transform.parent != null && gameobject.transform.parent.parent != null)
        {
            LevelDesignLogic logic = gameobject.transform.parent.parent.GetComponent<LevelDesignLogic>();
            if (logic != null)
            {
                target.m_data.m_pointSetId = name;
                LevelDesignEditor.Instance.SetPointData();
                //if (LevelDesignEditor.Instance.AddPoint(target.m_data, logic.m_data))
                LevelDesignEditor.Instance.SortPoint();
            }
        }
    }

    protected override void DestroyTargetFunction(LevelDesignPoint target)
    {
        //LevelDesignEditor.Instance.MinusPoint(target.m_data);

        LevelDesignEditor.Instance.SetPointData();
        LevelDesignEditor.Instance.SortPoint();
    }

    public override void OnInspectorGUI()
    {
        using (new AutoEditorDisabledGroup(true))
        {
            target.m_data.m_pointSetId = EditorGUILayout.TextField("点集id:", target.m_data.m_pointSetId);
        }
        target.m_data.m_saveToGround = EditorGUILayout.Toggle(target.m_data.m_saveToGround ? "保存贴地" : "不贴地", target.m_data.m_saveToGround);

        if (GUILayout.Button("增加点", GUILayout.Height(50)))
        {
            target.AddPointToSet();
        }

        //绘制所有的点
        for(int i = 0; i < gameobject.transform.childCount; ++i)
        {
            GUI.color = SetColor(i % 2 != 0);
            GameObject child = gameobject.transform.GetChild(i).gameObject;
            using (new AutoEditorHorizontal())
            {
                EditorGUILayout.LabelField("点 : " + child.name + "         ", GUILayout.Width(50f));
                using (new AutoEditorDisabledGroup(true))
                {
                    EditorGUILayout.ObjectField(child, typeof(GameObject), true, GUILayout.Width(150));
                }
            }
        }

        if (target.m_data.m_pointSetId != gameobject.name)
        {
            target.m_data.m_pointSetId = gameobject.name;
            LevelDesignEditor.Instance.SortPoint();
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
                LevelDesignEditor.Instance.AddPoint(target.m_data, logic.m_data);
            }
        }
    }
}
