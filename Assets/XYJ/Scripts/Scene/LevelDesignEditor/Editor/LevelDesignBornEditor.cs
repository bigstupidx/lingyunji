using UnityEngine;
using UnityEditor;
using EditorExtensions;

[CustomEditor(typeof(LevelDesignBorn))]
[CanEditMultipleObjects]
public class LevelDesignBornEditor : EditorX<LevelDesignBorn>
{
    protected override void OnEnable()
    {
        base.OnEnable();
        //target.m_data.m_bornId = gameobject.name;
        
        //Add();
        //LevelDesignEditor.Instance.Repaint();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        LevelDesignEditor.Instance.Repaint();
    }

    protected override void MultipleEnable(LevelDesignBorn target, string name)
    {
        if (gameobject.transform.parent != null && gameobject.transform.parent.parent != null)
        {
            LevelDesignLogic logic = gameobject.transform.parent.parent.GetComponent<LevelDesignLogic>();
            if (logic != null)
            {
                target.m_data.m_bornId = name;
                //if(LevelDesignEditor.Instance.AddBorn(target.m_data, logic.m_data))
                //    LevelDesignEditor.Instance.SortBorn();
                LevelDesignEditor.Instance.SetBornData();
                LevelDesignEditor.Instance.SortBorn();
            }
        }
    }

    protected override void DestroyTargetFunction(LevelDesignBorn target)
    {
        //LevelDesignEditor.Instance.MinusBorn(target.m_data);
        LevelDesignEditor.Instance.SetBornData();
        LevelDesignEditor.Instance.SortBorn();
    }

    public override void OnInspectorGUI()
    {
        using (new AutoEditorDisabledGroup(true))
        {
            DrawVector3FieldLayout(target.m_data.m_pos, value => target.SetPos(value), "坐标");
            DrawVector3FieldLayout(target.m_data.m_dir, value => target.SetDir(value), "旋转");
        }

        if (target.m_data.m_bornId != gameobject.name)
        {
            target.m_data.m_bornId = gameobject.name;
            //对名字进行排序
            LevelDesignEditor.Instance.SortBorn();
            LevelDesignEditor.Instance.Repaint();
        }
        if(target.m_data.m_pos != gameobject.transform.position)
        {
            target.m_data.m_pos = gameobject.transform.position;
        }
        if (target.m_data.m_dir != gameobject.transform.eulerAngles)
        {
            target.m_data.m_dir = gameobject.transform.eulerAngles;
        }
    }
}
