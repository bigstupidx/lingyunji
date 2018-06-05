using UnityEngine;
using UnityEditor;
using EditorExtensions;
using Config;

[CustomEditor(typeof(LevelDesignSpawn))]
[CanEditMultipleObjects]
public class LevelDesignSpawnEditor : EditorX<LevelDesignSpawn>
{
    static string[] GetConditionType()
    {
        string[] conditionType = new string[]
        {
            "无",
            "间隔时间刷新",
            "死亡后刷新",
        };

        return conditionType;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //target.m_data.m_spawnId = gameobject.name;
        
        //Add();
        //LevelDesignEditor.Instance.Repaint();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        LevelDesignEditor.Instance.Repaint();
    }

    protected override void MultipleEnable(LevelDesignSpawn target, string name)
    {
        if (gameobject.transform.parent != null && gameobject.transform.parent.parent != null)
        {
            LevelDesignLogic logic = gameobject.transform.parent.parent.GetComponent<LevelDesignLogic>();
            if (logic != null)
            {
                target.m_data.m_spawnId = name;
                LevelDesignEditor.Instance.SetSpawnData();
                //if (LevelDesignEditor.Instance.AddSpawn(target.m_data, logic.m_data))
                LevelDesignEditor.Instance.SortSpawn();
            }
        }
    }

    protected override void DestroyTargetFunction(LevelDesignSpawn target)
    {
        LevelDesignEditor.Instance.SetSpawnData();
        LevelDesignEditor.Instance.SortSpawn();
        //LevelDesignEditor.Instance.MinusSpawn(target.m_data);
    }

    public override void OnInspectorGUI()
    {
        //绘制ui
        EditorGUILayout.LabelField("刷新点id", target.m_data.m_spawnId);
        target.m_data.m_spawnGroupId = EditorGUILayout.TextField("组id", target.m_data.m_spawnGroupId);
        target.m_data.m_name = EditorGUILayout.TextField("刷新点名字", target.m_data.m_name);
        target.m_data.m_isRandomSpawn = EditorGUILayout.Toggle("是否随机刷新", target.m_data.m_isRandomSpawn);
        target.m_data.m_lookToPlayer = EditorGUILayout.Toggle("是否面向主角", target.m_data.m_lookToPlayer);
        target.m_data.m_isToGround = EditorGUILayout.Toggle("保存贴地", target.m_data.m_isToGround);
        target.m_data.m_isPassivity = EditorGUILayout.Toggle("是否被动怪", target.m_data.m_isPassivity);
        target.m_data.backToBorn = EditorGUILayout.Toggle("非战斗时回归出生点", target.m_data.backToBorn);
        target.m_data.fullHp = EditorGUILayout.Toggle("回归出生点是否满血", target.m_data.fullHp);
        target.m_data.m_fieldOfVision = EditorGUILayout.FloatField("视野", target.m_data.m_fieldOfVision);
        target.m_data.m_shareView = EditorGUILayout.Toggle("是否共享视野", target.m_data.m_shareView);
        target.m_data.m_startNum = EditorGUILayout.IntField("初始刷新的数量", target.m_data.m_startNum);
        target.m_data.m_startCd = EditorGUILayout.FloatField("初始刷新间隔", target.m_data.m_startCd);
        target.m_data.m_maxNum = EditorGUILayout.IntField("最大数量", target.m_data.m_maxNum);
        target.m_data.m_survivalLimit = EditorGUILayout.IntField("存活上限", target.m_data.m_survivalLimit);

        //刷新方式
        target.m_data.m_spawnType = (LevelDesignConfig.LevelSpawnData.SpawnType)EditorGUILayout.Popup("刷新方式", (int)target.m_data.m_spawnType, GetConditionType());
        switch (target.m_data.m_spawnType)
        {
            case LevelDesignConfig.LevelSpawnData.SpawnType.Time:
                target.m_data.m_spawnParam1 = EditorGUILayout.FloatField("时间", target.m_data.m_spawnParam1);
                target.m_data.m_spawnParam3 = EditorGUILayout.IntField("刷新数量", target.m_data.m_spawnParam3);
                break;
            case LevelDesignConfig.LevelSpawnData.SpawnType.Dead:
                target.m_data.m_spawnParam1 = EditorGUILayout.FloatField("时间", target.m_data.m_spawnParam1);
                target.m_data.m_spawnParam2 = EditorGUILayout.IntField("死亡数量", target.m_data.m_spawnParam2);
                target.m_data.m_spawnParam3 = EditorGUILayout.IntField("刷新数量", target.m_data.m_spawnParam3);
                break;
        }

        GUILayout.Space(15);
        //ai相关
        target.m_data.m_patrolId = EditorGUILayout.TextField("巡逻id", target.m_data.m_patrolId);
        target.m_data.m_bornAction = EditorGUILayout.TextField("出生行为", target.m_data.m_bornAction);
        target.m_data.m_enterBattleAction = EditorGUILayout.TextField("进入战斗行为", target.m_data.m_enterBattleAction);
        target.m_data.m_defaultIdle = EditorGUILayout.TextField("默认休闲待机动作", target.m_data.m_defaultIdle);
        target.m_data.m_bornBubb = EditorGUILayout.TextField("出生冒泡", target.m_data.m_bornBubb);
        target.m_data.m_enterBattleBubb = EditorGUILayout.TextField("进入战斗冒泡", target.m_data.m_enterBattleBubb);
        target.m_data.m_enterBattleSigh = EditorGUILayout.Toggle("进入战斗叹号", target.m_data.m_enterBattleSigh);
        target.m_data.m_randomIdle = EditorGUILayout.TextField("休闲随机待机", target.m_data.m_randomIdle);
        target.m_data.m_autoPatrol = EditorGUILayout.TextField("自动巡逻", target.m_data.m_autoPatrol);

        GUILayout.Space(15);

        //显示和隐藏模型
        using (new AutoEditorHorizontal())
        {
            if (GUILayout.Button("显示模型", GUILayout.Height(20f)))
            {
                ShowModel();
            }

            if (GUILayout.Button("隐藏模型", GUILayout.Height(20f)))
            {
                HideModel();
            }
        }
        //对象列表
        if (GUILayout.Button("增加对象", GUILayout.Height(30f)))
        {
            target.m_data.m_objs.Add(0);
        }
        for(int i = 0; i < target.m_data.m_objs.Count;)
        {
            using (new AutoEditorHorizontal())
            {
                target.m_data.m_objs[i] = EditorGUILayout.IntField(string.Format("角色ID [{0}]:", i + 1), target.m_data.m_objs[i]);
                //if (!EditorGUIUtility.editingTextField)
                {
                    int id = target.m_data.m_objs[i];
                    //if(int.TryParse(target.m_data.m_objs[i], out id))
                    {
                        Config.RoleDefine config = Config.RoleDefine.Get(id);
                        if(null != config)
                        {
                            string roleName = config.name;
                            GUILayout.Label("(" + roleName + ")");
                        }
                    }
                }

                if (GUILayout.Button("删除"))
                {
                    target.m_data.m_objs.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }
        }

        //增加点
        if (GUILayout.Button("增加点", GUILayout.Height(50)))
        {
            target.AddPointToSet();
        }

        //绘制所有的点
        for (int i = 0; i < gameobject.transform.childCount; ++i)
        {
            GUI.color = SetColor(i % 2 != 0);
            GameObject child = gameobject.transform.GetChild(i).gameObject;
            using (new AutoEditorHorizontal())
            {
                EditorGUILayout.LabelField("点 : " + child.name + "         ", GUILayout.Width(50f));
                EditorGUILayout.ObjectField(child, typeof(GameObject), true, GUILayout.Width(150));
            }
        }

        if (target.m_data.m_spawnId != gameobject.name)
        {
            target.m_data.m_spawnId = gameobject.name;
            LevelDesignEditor.Instance.SortSpawn();
            LevelDesignEditor.Instance.Repaint();
        }
    }

    //显示模型
    void ShowModel()
    {
        if(target.m_data.m_objs != null && target.m_data.m_objs.Count > 0)
        {
            int id = target.m_data.m_objs[0];
            RoleDefine config = RoleDefine.Get(id);
            if(null != config)
            {
                string model = config.model;
                //需要在每个点下面创建
                for(int i = 0; i < gameobject.transform.childCount; ++i)
                {
                    Transform child = gameobject.transform.GetChild(i);
                    ArtResLoad.LoadRes(model, LoadModelEnd, child);
                }
            }
            else
            {
                Debuger.LogError("该id配置表不存在");
            }
        }
        else
        {
            Debuger.LogError("当前没有填角色");
        }
    }

    //加载模型完成
    void LoadModelEnd(GameObject go, object param)
    {
        Transform parent = (Transform)param;
        go.transform.SetParent(parent);
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.localScale = Vector3.one;

        ModelPartTempObject.Create(go);
    }

    //隐藏模型
    void HideModel()
    {
        for (int i = 0; i < gameobject.transform.childCount; ++i)
        {
            Transform child = gameobject.transform.GetChild(i);
            child.DestroyChildren();
        }
    }

    void Add()
    {
        if(gameobject.transform.parent != null && gameobject.transform.parent.parent != null)
        {
            LevelDesignLogic logic = gameobject.transform.parent.parent.GetComponent<LevelDesignLogic>();
            if(logic != null)
            {
                LevelDesignEditor.Instance.AddSpawn(target.m_data, logic.m_data);
                return;
            }
        }
    }
}
