#if !USE_HOT
namespace xys.hot
{
    using UnityEngine;
    using NetProto.Hot;
    using System.Collections.Generic;
    using Config;

    /// <summary>
    /// 处理关卡相关的事件
    /// </summary>
    public partial class LevelEditorLogic
    {
        delegate void OnTriggerActionDelegate(LevelDesignConfig.LevelEventAction action, long charId);
        Dictionary<LevelDesignConfig.ActionType, OnTriggerActionDelegate> m_triggerActionDic;
        const string OWN_SIGN = "-2";

        /// <summary>
        /// 初始化事件
        /// </summary>
        public void InitEvent()
        {
            if(null == m_triggerActionDic)
            {
                m_triggerActionDic = new Dictionary<LevelDesignConfig.ActionType, OnTriggerActionDelegate>();
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.RoleAction, OnRoleAction);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.LockMonster, OnLockMonster);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.UnLockMonster, OnUnLockMonster);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.SetToPoint, OnSetToPoint);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.ActiveArea, OnActiveArea);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.UnActiveArea, OnUnActiveArea);

                //表现相关
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.PlayEffect, OnPlayEffect);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.PlayHangEffect, OnPlayHangEffect);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.LogicPause, OnLogicPause);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.LogicContinue, OnLogicContinue);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.RoleMotion, OnRoleMotion);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.RoleMotionLastFrame, OnRoleMotionLastFrame);
                //m_triggerActionDic.Add(LevelDesignConfig.ActionType.RoleMove, OnRoleMove);
                //m_triggerActionDic.Add(LevelDesignConfig.ActionType.Patrol, OnPatrol);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.RoleRotate, OnRoleRotate);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.ChangePosture, OnChangePosture);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.Bubbling, OnBubbling);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.RandomBubbling, OnRandomBubbling);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.Dialogue, OnDialogue);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.BubblingOnOff, OnBubblingOnOff);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.PlayCg, OnPlayCg);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.Interaction, OnInteraction);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.PlayCamera, OnPlayCamera);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.ClickOnOff, OnClickOnOff);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.RandomEvent, OnRandomEvent);
                m_triggerActionDic.Add(LevelDesignConfig.ActionType.PersonalityEvent, OnPersonalityEvent);
            }
            
            m_conditionVoDic = new Dictionary<string, EventMonitorVo>();
        }

        /// <summary>
        /// 触发了某个事件
        /// </summary>
        /// <param name="data"></param>
        public void LevelTriggerEvent(LevelTriggerEvent data)
        {
            string eventId = data.eventId;
            bool overAll = data.isOverall;
            LevelDesignConfig.LevelEventObjData eventData;
            if ((overAll && m_oaEventDic.TryGetValue(eventId, out eventData)) ||
                (!overAll && m_eventDic.TryGetValue(eventId, out eventData)))
            {
                TriggerAction(eventData, data.charid, overAll);
            }
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="charId"></param>
        void TriggerAction(LevelDesignConfig.LevelEventObjData eventData, long charId, bool overAll = false)
        {
            List<LevelDesignConfig.LevelEventAction> actionList = eventData.m_actions;
            for (int i = 0; i < actionList.Count; ++i)
            {
                LevelDesignConfig.LevelEventAction action = actionList[i];
                if (m_triggerActionDic.ContainsKey(action.m_actionType))
                {
                    float delay = action.m_delay;
                    if (delay > 0)
                    {
                        m_timer.Register(delay, 1, (index) =>
                        {
                            DoAction(action, eventData.m_eventId, charId, overAll, index);
                        }, i);
                    }
                    else
                    {
                        DoAction(action, eventData.m_eventId, charId, overAll, i);
                    }
                }
            }
        }

        /// <summary>
        /// 事件的条件达成通知
        /// </summary>
        /// <param name="data"></param>
        public void LevelEventNotice(LevelEventNotice data)
        {
            EventMonitorVo vo;
            if (!m_conditionVoDic.TryGetValue(data.eventId, out vo))
            {
                vo = new EventMonitorVo();
                vo.eventId = data.eventId;
                vo.overAll = data.overall;
                vo.conditionIndexList = new List<int>();
                vo.actionIndexList = new List<int>();
                m_conditionVoDic.Add(data.eventId, vo);
            }
            if(data.noticeType == 1)
            {
                if (!vo.conditionIndexList.Contains(data.index))
                {
                    if(data.charId != -1)
                    {
                        if(data.charId == App.my.localPlayer.charid)
                        {
                            vo.conditionIndexList.Add(data.index);
                        }
                    }
                    else
                    {
                        vo.conditionIndexList.Add(data.index);
                    }
                }
            }
            else
            {
                if (!vo.actionIndexList.Contains(data.index))
                {
                    if(data.charId != -1)
                    {
                        if (data.charId == App.my.localPlayer.charid)
                        {
                            vo.actionIndexList.Add(data.index);
                        }
                    }
                    else
                    {
                        vo.actionIndexList.Add(data.index);
                    }
                }
            }

#if UNITY_EDITOR
            if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (LevelMonitorEditor.Instance != null)
                    LevelMonitorEditor.Instance.Repaint();
            }
#endif
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="action"></param>
        void DoAction(LevelDesignConfig.LevelEventAction action, string eventId, long charId, bool overAll, int index)
        {
            //触发前端的事件
            m_triggerActionDic[action.m_actionType](action, charId);

            LevelEventNotice notice = new LevelEventNotice();
            notice.eventId = eventId;
            notice.index = index;
            notice.overall = overAll;
            notice.noticeType = 2;
            notice.charId = charId;
            LevelEventNotice(notice);
        }

        /// <summary>
        /// 播放交互动画
        /// </summary>
        /// <param name="action"></param>
        void OnInteraction(LevelDesignConfig.LevelEventAction action, long charId)
        {
            string interactionName = action.m_param1;
            string animName = action.m_param2;
            GameObject Interaction = GameObject.Find("MapScene/[Interaction]");
            if (!string.IsNullOrEmpty(interactionName) && !string.IsNullOrEmpty(animName))
            {
                Transform interaction = Interaction.transform.Find(interactionName);

                AnimationWrap animWrap = AnimationWrap.Create(interaction.gameObject);
                animWrap.PlayAni(animName);
            }
        }

        /// <summary>
        /// 角色表现
        /// </summary>
        /// <param name="action"></param>
        void OnRoleAction(LevelDesignConfig.LevelEventAction action, long charId)
        {

        }

        /// <summary>
        /// 锁定怪物
        /// </summary>
        /// <param name="action"></param>
        void OnLockMonster(LevelDesignConfig.LevelEventAction action, long charId)
        {
            Debuger.Log("锁定怪物");
            string spawnId = action.m_param1;
            int index = GetSpawnIndex(spawnId);
            List<NpcBase> list = App.my.sceneMgr.GetNpcsBySpawnId(index);
            if(list != null && list.Count == 1)
            {
                App.my.cameraMgr.SetOldLock(true, list[0]);
            }
        }

        /// <summary>
        /// 设置角色到点
        /// </summary>
        void OnSetToPoint(LevelDesignConfig.LevelEventAction action, long charId)
        {
            App.my.cameraMgr.SetExcessiveCamera(true);
        }

        /// <summary>
        /// 解锁怪物
        /// </summary>
        /// <param name="action"></param>
        void OnUnLockMonster(LevelDesignConfig.LevelEventAction action, long charId)
        {
            Debuger.Log("解锁怪物");
            App.my.cameraMgr.SetOldLock(false, null);
        }

        /// <summary>
        /// 播放过场动画
        /// </summary>
        /// <param name="action"></param>
        void OnPlayCg(LevelDesignConfig.LevelEventAction action, long charId)
        {
            Debuger.Log("播放过场动画");
            string name = action.m_param1;
            USPlayCG.PlayCG(name, null, "", false);
        }

        /// <summary>
        /// 播放特效
        /// </summary>
        /// <param name="action"></param>
        void OnPlayEffect(LevelDesignConfig.LevelEventAction action, long charId)
        {
            Debuger.Log("播放特效");
            string name = action.m_param1;
            string timeParam = action.m_param2;
            float time;
            if(float.TryParse(timeParam, out time))
            {
                XYJObjectPool.LoadPrefab(name, (GameObject go, object p) =>
                {
                    if(go != null)
                    {
                        go.SetActive(true);
                        if(time > 0)
                        {
                            EffectDestroy destroy = go.GetComponent<EffectDestroy>();
                            if (destroy == null)
                                destroy = go.AddComponent<EffectDestroy>();
                            destroy.m_destroyTime = time;
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 逻辑暂停
        /// </summary>
        /// <param name="action"></param>
        void OnLogicPause(LevelDesignConfig.LevelEventAction action, long charId)
        {
            App.my.uiSystem.ShowMaskPanel(true);
        }

        /// <summary>
        /// 逻辑恢复
        /// </summary>
        /// <param name="action"></param>
        void OnLogicContinue(LevelDesignConfig.LevelEventAction action, long charId)
        {
            App.my.uiSystem.ShowMaskPanel(false);
        }

        /// <summary>
        /// 播放挂点特效
        /// </summary>
        /// <param name="action"></param>
        void OnPlayHangEffect(LevelDesignConfig.LevelEventAction action, long charId)
        {
            string name = action.m_param1;
            string hangPoint = action.m_param2;
            string timeParam = action.m_param3;
            float time;
            if (float.TryParse(timeParam, out time))
            {
                if("-1" == hangPoint)
                {
                    //挂载到主角身上
                    LoadHangEffect(name, time, (go) =>
                    {
                        //特效需要跟随人物
                        EffectFollowBone.AddFollow(go, App.my.localPlayer.battle.actor.m_rootTrans, false, Vector3.zero);
                    });
                }
                else
                {
                    //挂载到点集,点集下每一个点下都要加载特效
                    LevelDesignConfig.LevelPointData pointSet = GetPointById(hangPoint);
                    if(null != pointSet)
                    {
                        for(int i = 0; i < pointSet.m_postions.Count; ++i)
                        {
                            Vector3 pos = pointSet.m_postions[i];
                            LoadHangEffect(name, time, (go) =>
                            {
                                //特效需要跟随人物
                                go.transform.position = pos;
                            });
                        }
                    }
                }
            }
        }

        void LoadHangEffect(string name, float time, System.Action<GameObject> action = null)
        {
            XYJObjectPool.LoadPrefab(name, (GameObject go, object p) =>
            {
                if (go != null)
                {
                    go.SetActive(true);
                    if (time > 0)
                    {
                        EffectDestroy destroy = go.GetComponent<EffectDestroy>();
                        if (destroy == null)
                            destroy = go.AddComponent<EffectDestroy>();
                        destroy.m_destroyTime = time;
                    }

                    if (null != action)
                        action(go);
                }
            });
        }

        /// <summary>
        /// 激活一个区域
        /// </summary>
        /// <param name="action"></param>
        void OnActiveArea(LevelDesignConfig.LevelEventAction action, long charId)
        {
            string areaId = action.m_param1;
            LevelLogicArea area = GetAreaById(areaId);
            if(null != area)
            {
                area.SetActive(true);
            }
        }

        /// <summary>
        /// 关闭一个区域
        /// </summary>
        /// <param name="action"></param>
        void OnUnActiveArea(LevelDesignConfig.LevelEventAction action, long charId)
        {
            string areaId = action.m_param1;
            LevelLogicArea area = GetAreaById(areaId);
            if (null != area)
            {
                area.SetActive(true);
            }
        }

        /// <summary>
        /// 动作
        /// </summary>
        /// <param name="action"></param>
        void OnRoleMotion(LevelDesignConfig.LevelEventAction action, long charId)
        {
            string spawnId = action.m_param1;
            string motionName = action.m_param2;
            string loopTime = action.m_param3;
            float time;
            if(float.TryParse(loopTime, out time))
            {
                if (-1 == time)
                    time = 0;

                if(OWN_SIGN == spawnId)
                {
                    if(!IsOwnEvent(charId))
                    {
                        return;
                    }

                    //自己
                    LocalPlayer player = App.my.localPlayer;
                    if(null != player)
                    {
                        player.battle.State_PlayAni(motionName, time);
                    }
                }
                else
                {
                    //刷新点的对象
                    int index = GetSpawnIndex(spawnId);
                    List<NpcBase> npcList = App.my.sceneMgr.GetNpcsBySpawnId(index);
                    foreach(var npc in npcList)
                    {
                        npc.battle.State_PlayAni(motionName, time);
                    }
                }
            }
        }

        /// <summary>
        /// 动作无后续
        /// </summary>
        /// <param name="action"></param>
        void OnRoleMotionLastFrame(LevelDesignConfig.LevelEventAction action, long charId)
        {
            string spawnId = action.m_param1;
            string motionName = action.m_param2;
            if (OWN_SIGN == spawnId)
            {
                if (!IsOwnEvent(charId))
                {
                    return;
                }

                //自己
                LocalPlayer player = App.my.localPlayer;
                if (null != player)
                {
                    player.battle.m_aniMgr.PlayAni(motionName);
                }
            }
            else
            {
                //刷新点的对象
                int index = GetSpawnIndex(spawnId);
                List<NpcBase> npcList = App.my.sceneMgr.GetNpcsBySpawnId(index);
                foreach (var npc in npcList)
                {
                    npc.battle.m_aniMgr.PlayAni(motionName);
                }
            }
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="action"></param>
        void OnRoleMove(LevelDesignConfig.LevelEventAction action, long charId)
        {
            string spawnId = action.m_param1;
            string pointSetId = action.m_param2;
            float time = float.Parse(action.m_param3);

            LocalPlayer player = App.my.localPlayer;
            if(null == player)
            {
                return;
            }
            LevelDesignConfig.LevelPointData pointData = GetPointById(pointSetId);
            if(null == pointData)
            {
                Debuger.LogError("取不到点集数据");
                return;
            }

            //寻路的点集列表
            List<Vector3> pointList = pointData.m_postions;
            float speed = CaculateSpeed(pointList, time);
            if (OWN_SIGN == spawnId)
            {
                //自己移动
                if (!IsOwnEvent(charId))
                {
                    return;
                }
                MoveToPos(pointList, speed, player.battle);
            }
            else
            {
                //刷新点的对象移动
                int index = GetSpawnIndex(spawnId);
                List<NpcBase> npcList = App.my.sceneMgr.GetNpcsBySpawnId(index);
                if (null != npcList && npcList.Count > 0)
                {
                    for (int i = 0; i < npcList.Count; ++i)
                    {
                        MoveToPos(pointList, speed, npcList[i].battle);
                    }
                }
                else
                {
                    Debuger.LogError("找不到刷新点信息 " + spawnId);
                }
            }
        }

        /// <summary>
        /// 计算速度
        /// </summary>
        /// <returns></returns>
        float CaculateSpeed(List<Vector3> pointList, float time)
        {
            float speed = 1;
            float dis = 0;
            for(int i = 0; i < pointList.Count - 1; ++i)
            {
                Vector3 source = pointList[i];
                Vector3 target = pointList[i + 1];
                dis += Vector3.Distance(source, target);
            }
            speed = dis / time;

            return speed;
        }

        void MoveToPos(List<Vector3> pointList, float speed, battle.BattleManagerBase battle)
        {
            if (pointList.Count > 0)
            {
                Vector3 target = pointList[0];
                pointList.RemoveAt(0);
                battle.State_MoveToPos(target,
                (list) =>
                {
                    MoveToPos((List<Vector3>)list, speed, battle);
                },
                pointList,
                speed);
            }
            else
            {
                //移动到终点
            }
        }

        /// <summary>
        /// 巡逻
        /// </summary>
        /// <param name="action"></param>
        /// <param name="charId"></param>
        void OnPatrol(LevelDesignConfig.LevelEventAction action, long charId)
        {
            string spawnId = action.m_param1;
            string patrolId = action.m_param2;//巡逻id就是关卡编辑器的路径点的id
            bool loopType = bool.Parse(action.m_param3);

            //List<PathPointVo> voList = GetPathById(patrolId);
            //if(null != voList && voList.Count > 0)
            {
                int index = GetSpawnIndex(spawnId);
                List<NpcBase> npcList = App.my.sceneMgr.GetNpcsBySpawnId(index);
                if (null != npcList && npcList.Count > 0)
                {
                    for (int i = 0; i < npcList.Count; ++i)
                    {
                        if(npcList[i].SetPatrolData(patrolId, loopType))
                        {
                            //有巡逻数据
                            PatrolMove(npcList[i]);
                        }
                    }
                }
                else
                {
                    Debuger.LogError("找不到刷新点信息 " + spawnId);
                }
            }
            //else
            //{
            //    Debuger.LogError("巡逻路径点信息找不到");
            //}
        }

        //巡逻
        void PatrolMove(NpcBase npc)
        {
            NpcPatrolData data = npc.m_patrolData;
            if(null != data)
            {
                if(data.pathList != null)
                {
                    //若当前的巡逻队列为空，则重新初始化
                    if(data.pathList.Count == 0 && data.tempList.Count > 0)
                    {
                        data.pathList = new List<PathPointVo>(data.tempList.ToArray());
                        data.tempList = new List<PathPointVo>();
                        if(data.loopType)
                        {
                            //若是正序，则设置npc坐标至第一个点
                            npc.SetPosition(data.pathList[0].pos);
                        }
                    }

                    PathPointVo vo = data.pathList[0];
                    data.pathList.RemoveAt(0);
                    if (data.loopType)
                    {
                        //正序
                        data.tempList.Add(vo);
                    }
                    else
                    {
                        //倒叙，插入到最前
                        data.tempList.Insert(0, vo);
                    }
                    //先把旋转清空
                    iTween.Stop(npc.battle.m_root.gameObject);
                    npc.battle.State_MoveToPos(vo.pos, OnPatrolPoint, new object[] { npc, vo }, vo.speed);
                }
            }
        }

        //巡逻到点
        void OnPatrolPoint(object obj)
        {
            object[] pList = (object[])obj;
            NpcBase npc = (NpcBase)pList[0];
            PathPointVo vo = (PathPointVo)pList[1];
            if (null != vo)
            {
                float stayTime = vo.stayTime;
                string eventId = vo.eventId;
                
                if (!string.IsNullOrEmpty(eventId))
                {
                    //执行一个事件
                    LevelDesignConfig.LevelEventObjData eventData;
                    if (m_eventDic.TryGetValue(eventId, out eventData) || m_oaEventDic.TryGetValue(eventId, out eventData))
                    {
                        TriggerAction(eventData, App.my.localPlayer.charid);
                    }
                }
                if (stayTime == 0)
                {
                    stayTime = 0.1f;
                }
                else
                {
                    npc.battle.m_stateMgr.ChangeState(battle.StateType.Idle);
                }

                //在寻路点上等待一段时间
                m_timer.Register<NpcBase>(stayTime, 1, PatrolMove, npc);
            }
        }

        /// <summary>
        /// 转向
        /// </summary>
        /// <param name="action"></param>
        void OnRoleRotate(LevelDesignConfig.LevelEventAction action, long charId)
        {
            string spawnId = action.m_param1;
            int type;
            string param = action.m_param3;
            float speed = float.Parse(action.m_param4);
            if(int.TryParse(action.m_param2, out type))
            {
                if(OWN_SIGN == spawnId)
                {
                    if (!IsOwnEvent(charId))
                    {
                        return;
                    }

                    //自己转向
                    //GameObject go = App.my.localPlayer.battle.actor.m_rootTrans.gameObject;
                    DoRotate(App.my.localPlayer, type, param, speed);
                }
                else
                {
                    //刷新点转向
                    int index = GetSpawnIndex(spawnId);
                    List<NpcBase> npcList = App.my.sceneMgr.GetNpcsBySpawnId(index);
                    foreach (var npc in npcList)
                    {
                        //GameObject go = npc.battle.actor.m_rootTrans.gameObject;
                        DoRotate(npc, type, param, speed);
                    }
                }
            }
        }

        //3种类型， 1表示转某个角度，负数向右  2表示朝向某个刷新点  3归位
        void DoRotate(ObjectBase obj, int type, string param, float speed)
        {
            GameObject go = obj.battle.actor.m_rootTrans.gameObject;
            float angle = 0;
            float targetAngle = 0;
            switch(type)
            {
                case 1:
                    {
                        string[] temp = param.Split('|');
                        if(float.TryParse(temp[0], out angle))
                        {
                            targetAngle = go.transform.eulerAngles.y + angle;
                        }
                    }
                    break;
                case 2:
                    {
                        string[] temp = param.Split('|');
                        string spawnId = temp[0];
                        Vector3 targetPos = Vector3.zero;
                        if(spawnId == OWN_SIGN)
                        {
                            targetPos = App.my.localPlayer.battle.actor.m_rootTrans.position;
                        }
                        else
                        {
                            int index = GetSpawnIndex(spawnId);
                            List<NpcBase> npcList = App.my.sceneMgr.GetNpcsBySpawnId(index);
                            if(null != npcList && npcList.Count > 0)
                            {
                                targetPos = npcList[0].battle.actor.m_rootTrans.position;
                            }
                            else
                            {
                                Debuger.LogError("转向  找不到npc : " + spawnId);
                            }
                        }

                        Vector3 lookDir = (targetPos - go.transform.position).normalized;
                        lookDir.y = 0;
                        Quaternion look = Quaternion.LookRotation(lookDir);
                        angle = Mathf.Acos(Vector3.Dot(lookDir, go.transform.forward)) * 180.0f / Mathf.PI;
                        targetAngle = look.eulerAngles.y;
                    }
                    break;
                case 3:
                    {
                        NpcBase npc = obj as NpcBase;
                        if(null != npc)
                        {
                            angle = Mathf.Acos(Vector3.Dot(npc.bornRot, go.transform.forward)) * 180.0f / Mathf.PI;
                            targetAngle = npc.bornRot.y;
                        }
                    }
                    break;
            }
            //Debuger.Log("type : " + type + " targetAngle : " + targetAngle + " angle : " + angle);
            float time = angle / speed;
            iTween.RotateTo(go, new Vector3(0, targetAngle), time);
        }

        /// <summary>
        /// 切换姿态
        /// </summary>
        void OnChangePosture(LevelDesignConfig.LevelEventAction action, long charId)
        {
            //后端处理
        }

        /// <summary>
        /// 冒泡
        /// </summary>
        /// <param name="action"></param>
        void OnBubbling(LevelDesignConfig.LevelEventAction action, long charId)
        {
            string spawnId = action.m_param1;
            string bubbId = action.m_param2;
            float time = float.Parse(action.m_param3);

            BubbleContents content = BubbleContents.Get(bubbId);
            if(content == null)
            {
                Debuger.LogError("找不到冒泡数据 " + bubbId);
                return;
            }
            if (OWN_SIGN == spawnId)
            {
                if (!IsOwnEvent(charId))
                {
                    return;
                }

                //自己冒泡
                ShowBubbling(App.my.localPlayer.battle.actor.m_hangPoint, content.content, time);
            }
            else
            {
                //刷新点冒泡
                if(m_spawnBubb.ContainsKey(spawnId))
                {
                    if (!m_spawnBubb[spawnId])
                        return;
                }
                int index = GetSpawnIndex(spawnId);
                List<NpcBase> npcList = App.my.sceneMgr.GetNpcsBySpawnId(index);
                foreach (var npc in npcList)
                {
                    ShowBubbling(npc.battle.actor.m_hangPoint, content.content, time);
                }
            }
        }

        void ShowBubbling(xys.UI.ActorHangPointHandler handler, string content, float time)
        {
            handler.ShowBubbling(content, time);
        }

        /// <summary>
        /// 随机冒泡
        /// </summary>
        /// <param name="action"></param>
        void OnRandomBubbling(LevelDesignConfig.LevelEventAction action, long charId)
        {
            string spawnId = action.m_param1;
            string bubblingIds = action.m_param2;
            float prop = float.Parse(action.m_param3);
            float time = float.Parse(action.m_param4);

            if (OWN_SIGN == spawnId)
            {
                if (!IsOwnEvent(charId))
                {
                    return;
                }

                //自己
                ShowRandomBubbling(App.my.localPlayer.battle.actor.m_hangPoint, bubblingIds, prop, time);
            }
            else
            {
                //刷新点
                if (m_spawnBubb.ContainsKey(spawnId))
                {
                    if (!m_spawnBubb[spawnId])
                        return;
                }
                int index = GetSpawnIndex(spawnId);
                List<NpcBase> npcList = App.my.sceneMgr.GetNpcsBySpawnId(index);
                foreach (var npc in npcList)
                {
                    ShowRandomBubbling(npc.battle.actor.m_hangPoint, bubblingIds, prop, time);
                }
            }
        }

        void ShowRandomBubbling(xys.UI.ActorHangPointHandler handler, string bubblingIds, float prop, float time)
        {
            string[] temp = bubblingIds.Split(';');
            List<string> contents = new List<string>();
            foreach(var itor in temp)
            {
                BubbleContents content = BubbleContents.Get(itor);
                contents.Add(content.content);
            }
            handler.SetRandomBubbling(contents.ToArray(), time, 5);
        }

        /// <summary>
        /// 对白
        /// </summary>
        /// <param name="action"></param>
        void OnDialogue(LevelDesignConfig.LevelEventAction action, long charId)
        {
            //todo   等小烨
        }

        /// <summary>
        /// 冒泡开关
        /// </summary>
        /// <param name="action"></param>
        void OnBubblingOnOff(LevelDesignConfig.LevelEventAction action, long charId)
        {
            string spawnId = action.m_param1;
            bool onOff = action.m_param2 == "0" ? false : true;
            if(m_spawnBubb.ContainsKey(spawnId))
            {
                m_spawnBubb[spawnId] = onOff;
            }
        }

        /// <summary>
        /// 镜头动画
        /// </summary>
        /// <param name="action"></param>
        void OnPlayCamera(LevelDesignConfig.LevelEventAction action, long charId)
        {
            string name = action.m_param1;
            if(!string.IsNullOrEmpty(name))
            {
                XYJObjectPool.LoadPrefab(name, null);
            }
        }

        /// <summary>
        /// 点击响应开关
        /// </summary>
        /// <param name="action"></param>
        void OnClickOnOff(LevelDesignConfig.LevelEventAction action, long charId)
        {
            string spawnId = action.m_param1;
            bool can = action.m_param2 == "0" ? false : true;
            int index = GetSpawnIndex(spawnId);
            List<NpcBase> npcList = App.my.sceneMgr.GetNpcsBySpawnId(index);
            foreach(var itor in npcList)
            {
                itor.canClick = can;
            }
        }

        /// <summary>
        /// 随机表现
        /// </summary>
        /// <param name="action"></param>
        void OnRandomEvent(LevelDesignConfig.LevelEventAction action, long charId)
        {
            string eventId1 = action.m_param1;
            string eventId2 = action.m_param2;
            string eventId3 = action.m_param3;
            string eventId4 = action.m_param4;
            string eventId5 = action.m_param5;

            List<string> randomList = new List<string>();
            if (!string.IsNullOrEmpty(eventId1))
                randomList.Add(eventId1);
            if (!string.IsNullOrEmpty(eventId2))
                randomList.Add(eventId2);
            if (!string.IsNullOrEmpty(eventId3))
                randomList.Add(eventId3);
            if (!string.IsNullOrEmpty(eventId4))
                randomList.Add(eventId4);
            if (!string.IsNullOrEmpty(eventId5))
                randomList.Add(eventId5);

            if(randomList.Count > 0)
            {
                int index = Random.Range(0, randomList.Count - 1);
                string eventId = randomList[index];
                LevelDesignConfig.LevelEventObjData eventData;
                if(m_eventDic.TryGetValue(eventId, out eventData) || m_oaEventDic.TryGetValue(eventId, out eventData))
                {
                    TriggerAction(eventData, charId);
                }
            } 
        }

        /// <summary>
        /// 个性事件
        /// </summary>
        /// <param name="action"></param>
        void OnPersonalityEvent(LevelDesignConfig.LevelEventAction action, long charId)
        {

        }

        /// <summary>
        /// 判断是否是自己才要生效的事件
        /// </summary>
        /// <param name="charId"></param>
        /// <returns></returns>
        bool IsOwnEvent(long charId)
        {
            if(charId != -1)
            {
                if (charId == App.my.localPlayer.charid)
                    return true;
            }
            return false;
        }
    }
}
#endif