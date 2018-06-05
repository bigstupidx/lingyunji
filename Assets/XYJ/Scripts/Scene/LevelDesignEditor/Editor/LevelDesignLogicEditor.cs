using UnityEngine;
using UnityEditor;
using EditorExtensions;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(LevelDesignLogic))]
[CanEditMultipleObjects]
public class LevelDesignLogicEditor : EditorX<LevelDesignLogic>
{
    protected override void OnEnable()
    {
        base.OnEnable();
        //target.m_data.m_name = gameobject.name;

        Show();
        //Add();
        //LevelDesignEditor.Instance.Repaint();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        LevelDesignEditor.Instance.Repaint();
    }

    protected override void MultipleEnable(LevelDesignLogic target, string name)
    {
        Transform parent = gameobject.transform.parent;
        if (parent != null)
        {
            GameObject logicParent = LevelDesignEditor.Instance.RootFind(LevelDesignEditor.Instance.GetCurEditConfigName());
            if (parent == logicParent.transform)
            {
                target.m_data.m_name = name;
                //LevelDesignEditor.Instance.AddLogic(target.m_data);
                LevelDesignEditor.Instance.SetLogicData();
                LevelDesignEditor.Instance.SortLogic();
            }
        }
    }

    protected override void DestroyTargetFunction(LevelDesignLogic target)
    {
        //LevelDesignEditor.Instance.MinusLogic(target.m_data);
        LevelDesignEditor.Instance.SetLogicData();
        LevelDesignEditor.Instance.SortLogic();
    }

    public override void OnInspectorGUI()
    {
        //是否是主逻辑
        bool isStart = LevelDesignEditor.Instance.IsStartLogic(target.m_data.m_name);
        using (new AutoEditorHorizontal())
        {
            EditorGUILayout.Toggle("是否初始逻辑", isStart);
            if(GUILayout.Button(isStart ? "取消" : "选择"))
            {
                if(isStart)
                {
                    //取消
                    LevelDesignEditor.Instance.SetStartLogic(null);
                }
                else
                {
                    //选择
                    LevelDesignEditor.Instance.SetStartLogic(target.m_data.m_name);
                }
            }
        }

        //显示场景
        DrawObjectLayout(target.m_scene, typeof(Object), value => target.SetScene(value), "场景");
        //显示风格
        DrawTextLayout(target.m_data.m_sceneStyle, value => target.SetSceneStyle(value), "场景风格");
        //显示事件集
        //DrawObjectLayout(target.m_eventSet, typeof(GameObject), value => target.SetEventSet(value), "事件集");

        using (new AutoEditorHorizontal())
        {
            if(GUILayout.Button("打开场景"))
            {
                string path = LevelDesignEditor.Instance.GetScenePath(target.m_data.m_scene);
                UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                LevelDesignEditor.Instance.Reload();
            }
        }

        if (target.m_data.m_name != gameobject.name)
        {
            if(LevelDesignEditor.Instance.IsCurData(target.m_data))
            {
                LevelDesignEditor.Instance.SetSelect(gameobject.name);
            }
            target.m_data.m_name = gameobject.name;
            LevelDesignEditor.Instance.SortLogic();
            LevelDesignEditor.Instance.Repaint();
        }
    }

    void Show()
    {
        //显示场景
        string path = LevelDesignEditor.Instance.GetScenePath(target.m_data.m_scene);
        if(!string.IsNullOrEmpty(path))
        {
            target.SetScene(AssetDatabase.LoadAssetAtPath(path, typeof(object)));
        }

        target.SetSceneStyle(target.m_data.m_sceneStyle);
    }

    void Add()
    {
        Transform parent = gameobject.transform.parent;
        if(parent != null)
        {
            GameObject logicParent = LevelDesignEditor.Instance.RootFind(LevelDesignEditor.Instance.GetCurEditConfigName());
            if(parent == logicParent.transform)
            {
                LevelDesignEditor.Instance.AddLogic(target.m_data);
            }
        }
    }
}
