using xys.battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;
using Config;
using System;

namespace xys.battle
{
    /// <summary>
    /// 角色表现相关，与逻辑没有关系
    /// </summary>
    public class ActorComponent : IBattleComponent, IBattleUpdate
    {
        //根节点，不管有没有加载都会创建出来,只有obj销毁才会销毁
        public Transform m_rootTrans { get; private set; }
        //模型transwrap，切换模型也不会修改
        public Transform m_modelTrans { get; private set; }
        //瞄准跟随
        public Transform m_lookAtTrans { get; private set; }
        //伤害特效挂点
        public Transform hurtFxTrans { get; private set; }
        //处理浮空
        public ModelOffSet m_modelOffset { get; private set; }
        public ModelPartManage m_partManager { get; private set; }
        // 角色头顶UI挂点
        public ActorHangPointHandler m_hangPoint { get; private set; }
        public Transform m_titile { get { return m_partManager.TitleObject; } }

        public ObjectMono mono { get; private set; }
        ObjectEventAgent m_event;
        ObjectBase m_obj;
        int m_hideModel = 0;
        public bool isFinishLoad { get; private set; }

        CharacterController m_characterCtrl;

        public AnimationWrap rootAni
        {
            get
            {
                if (m_partManager != null)
                    return m_partManager.GetRootAni();
                else
                    return null;
            }
        }


        public void OnAwake(IObject obj)
        {
            m_obj = obj as ObjectBase;
        }
        public void OnStart()
        {

        }
        public void OnDestroy()
        {
            m_obj = null;
        }

        public void OnEnterScene()
        {
            m_event = new ObjectEventAgent(m_obj.eventSet);
            isFinishLoad = false;

            m_event.Subscribe(NetProto.AttType.AT_State, OnBattleState);
            m_event.Subscribe(NetProto.AttType.AT_Posture, OnSetPosture);

            m_rootTrans = (new GameObject(string.Format("{0} {1}", m_obj.cfgInfo.model, m_obj.charSceneId))).transform;
            m_rootTrans.gameObject.tag = ComLayer.TAG_ROLE;
            m_rootTrans.position = m_obj.position;
            m_rootTrans.rotation = Quaternion.Euler(0, m_obj.rotateAngle, 0);
            m_partManager = new ModelPartManage(m_obj is LocalPlayer);
            m_modelOffset = new ModelOffSet();
            m_hangPoint = new ActorHangPointHandler();

            // 创建模型
            if (m_obj is LocalPlayer)
            {
                LocalPlayer player = (LocalPlayer)m_obj;
                NetProto.AppearanceData appearance = player.GetModule<AppearanceModule>().GetAppearanceData();
                RoleDisguiseHandle disguiseHandle = new RoleDisguiseHandle();
                disguiseHandle.SetRoleAppearance(m_obj.cfgInfo.id, player.carrerValue, player.sexValue, appearance);
                m_partManager.LoadModelWithAppearance(disguiseHandle, OnFinishCreate);             
            }
            else
            {
                m_partManager.LoadModel(m_obj.cfgInfo.model, OnFinishCreate, (this.m_obj.cfgInfo.modelScale == 0f ? 1.0f : this.m_obj.cfgInfo.modelScale));
            }

            mono = m_rootTrans.AddComponentIfNoExist<ObjectMono>();
            mono.Init(m_obj);
        }
        public void OnExitScene()
        {
            isFinishLoad = false;
            if (m_event != null)
                m_event.Release();
            m_hangPoint.Destory();
            m_partManager.Destroy();
            GameObject.Destroy(m_rootTrans.gameObject);
        }


        public void OnUpdate()
        {
            m_modelOffset.UpdateMove(m_obj);
        }

        public CollisionFlags CCMove(Vector3 move)
        {
            if (m_characterCtrl != null)
            {
                //隐身调用cc的move不起效
                if (m_obj.battle.actor.IsHide())
                    m_obj.SetPosition(m_obj.position + move);
                else
                {
                    CollisionFlags flg = m_characterCtrl.Move(move);
                    m_obj.SetPositionExceptRoot(m_characterCtrl.transform.position);
                    return flg;
                }
            }
            return CollisionFlags.None;
        }

        //隐藏模型
        public void SetHide(bool isHide)
        {
            if (isHide)
            {
                if (++m_hideModel == 1)
                    HideImpl(true);
            }
            else
            {
                if (--m_hideModel == 0)
                    HideImpl(false);
            }
        }

        public bool IsHide()
        {
            return m_hideModel > 0;
        }

        void HideImpl(bool isHide)
        {
            EventTriggerLogic.HideModel(isHide, m_rootTrans.gameObject);
            DrawTargetObjectManage.ShowShadow(m_rootTrans, !isHide);
        }

        void OnFinishCreate(GameObject go)
        {
            if (m_rootTrans == null)
                return;
            if (isFinishLoad)
                Debug.LogError("模型加载回调重复调用 ");
            isFinishLoad = true;
            if (m_partManager.m_aniEffect != null)
                m_partManager.m_aniEffect.m_role = m_obj;
            BattleHelp.SetParent(go.transform, m_rootTrans);
            m_modelOffset.SetTrans(go.transform);
            m_characterCtrl = m_rootTrans.AddComponentIfNoExist<CharacterController>();
            m_characterCtrl.radius = m_obj.cfgInfo.behitRaidus < 1 ? 1 : m_obj.cfgInfo.behitRaidus;
            m_characterCtrl.skinWidth = m_characterCtrl.radius * 0.1f;
            m_characterCtrl.center = new Vector3(0, m_characterCtrl.height / 2 + m_characterCtrl.skinWidth, 0);
            m_rootTrans.gameObject.layer = ComLayer.Layer_NoColliderRole;

            //设置关键挂点
            m_modelTrans = go.transform;
            Transform Chest_M = BoneManage.GetBone(go.transform, "Chest_M");
            hurtFxTrans = BoneManage.GetBone(go.transform, "hurtFx");

            // 初始化挂点
            if (m_hangPoint != null)
                m_hangPoint.Init(m_partManager.TitleObject, (ObjectBase)this.m_obj);
            else
            {
                Debug.LogErrorFormat("m_hangPoint null!, prefab:{0}", go.name);
            }

            //伤害特效挂点和瞄准跟随
            if (Chest_M != null)
                m_lookAtTrans = Chest_M;
            else
                m_lookAtTrans = go.transform;

            //受击点如果没有配置则手动创建出来，因为特效表需要找这个骨骼
            if (hurtFxTrans == null)
                hurtFxTrans = CreateTrans("hurtFx", m_lookAtTrans);

            //设置摄像机
            if (m_obj is LocalPlayer)
                App.my.cameraMgr.FinishCreateMe(m_modelTrans);

            //避免除了屏幕就不播放动画了
            if( m_partManager.GetRootAni()!=null )
            {
                NewAnimation ani = m_partManager.GetRootAni() as NewAnimation;
                ani.m_animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }

            //贴地
            if (m_obj is NpcBase)
            {
                NpcBase npc = m_obj as NpcBase;
                if (null != npc)
                {
                    int refreshId = npc.m_refreshId;
                    bool toGround = App.my.localPlayer.GetModule<LevelModule>().SpawnNeedToGround(refreshId);
                    if (toGround)
                        BattleHelp.SetGround(m_obj);

                    //设置模型大小
                    Vector3 modelScale = App.my.localPlayer.GetModule<LevelModule>().GetSpawnScale(refreshId);
                    m_rootTrans.localScale = modelScale;
                }
            }
            else
                BattleHelp.SetGround(m_obj);


            //添加动态阴影
            DrawTargetObjectManage.AddShadow(m_rootTrans);


            if (m_obj.stateValue == (int)ObjectState.Born)
                m_obj.battle.State_PlayAni(m_obj.battle.m_attrMgr.postureCfg.bornAni, 0, LoadLoopEffect);
            else
            {
                //模型加载完播放动画
                //m_obj.battle.m_aniMgr.PlayStateAni();
                m_obj.battle.m_stateMgr.ChangeState(StateType.Idle);
                LoadLoopEffect(null);
            }
        }

        //加载循环动画
        void LoadLoopEffect(object para)
        {
            if (!string.IsNullOrEmpty(m_obj.cfgInfo.loopEffect) && m_obj.isAlive)
                m_obj.battle.m_effectMgr.AddEffect(m_obj.cfgInfo.loopEffect);
        }

        ///创建一个节点
        Transform CreateTrans(string name, Transform parent)
        {
            Transform trans = new GameObject(name).transform;
            //改动父节点
            trans.parent = parent;
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            return trans;
        }



        public void OnFinishSkill()
        {
            if (m_partManager != null && m_partManager.m_aniEffect != null)
                m_partManager.m_aniEffect.FinishSkill();
        }

        void OnBattleState(AttributeChange args)
        {
            if (!isFinishLoad)
                return;
            string inserAni;
            //播放动画
            switch((ObjectState)args.currentValue.intValue)
            {
                case ObjectState.EnterBattle: 
                        m_obj.battle.m_aniMgr.PlayAni(m_obj.battle.m_attrMgr.postureCfg.enterBattle);
                        m_obj.battle.m_aniMgr.PlayQueued(m_obj.battle.m_attrMgr.postureCfg.battleIdle);
                        return;
                case ObjectState.Battle: inserAni = m_obj.battle.m_attrMgr.postureCfg.normalToBattleIdle; break;
                case ObjectState.Idle: inserAni = m_obj.battle.m_attrMgr.postureCfg.battleToNormalIdle; break;
                default:
                    return;
            }
            m_obj.battle.m_aniMgr.PlayStateAni(inserAni);
        }
        PostureConfig m_oldPostureCfg;
        void OnSetPosture(AttributeChange args)
        {
            if (!isFinishLoad)
                return;
            PostureConfig newCfg = PostureConfig.Get(args.currentValue.intValue);
            if (newCfg == null)
                return;
            if (m_oldPostureCfg == null)
                m_oldPostureCfg = PostureConfig.Get(0);

            //移除循环特效
            m_obj.battle.m_effectMgr.RemoveEffect(m_oldPostureCfg.logic.effect);

            //添加离开特效
            m_obj.battle.m_effectMgr.AddEffect(m_oldPostureCfg.logic.destroyEffect);

            //添加开始特效
            m_obj.battle.m_effectMgr.AddEffect(newCfg.logic.enterEffect);

            //添加循环特效
            m_obj.battle.m_effectMgr.AddEffect(newCfg.logic.effect);

            //播放动画
            m_obj.battle.actor.m_partManager.m_aniEffect.SetPosture(newCfg.id);

            m_obj.battle.m_aniMgr.PlayStateAni(m_oldPostureCfg.battleChangePosture);
            m_oldPostureCfg = newCfg;
        }
    }

}
