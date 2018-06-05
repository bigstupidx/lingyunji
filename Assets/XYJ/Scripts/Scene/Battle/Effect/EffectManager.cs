using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;

namespace xys.battle
{
    //角色特效管理
    public class ObjectEffectManager : IBattleComponent, IBattleUpdate
    {
        public ILoadReference AddEffect(string fxid, float timeLenght = -1)
        {
            if (string.IsNullOrEmpty(fxid))
                return null;
                
            EffectConfig fx = EffectConfig.Get(fxid);
            ILoadReference loadRef = null;
            if (fx != null)
            {
                if (string.IsNullOrEmpty(fx.material))
                    loadRef = fx.CreateEffectReference(m_obj.root, null, null);
                else
                    loadRef = fx.PlayMaterialEffect(m_obj);
            }
            AddLoadReference(fxid, loadRef, timeLenght);
            return loadRef;
        }

        public void AddEffect(string fxid, ILoadReference loadRef, float timeLenght)
        {
            if (string.IsNullOrEmpty(fxid))
            {
                if (loadRef != null)
                    Debuger.LogError("特效管理添加的fxid为null");
                return;    
            }

            AddLoadReference(fxid, loadRef, timeLenght);
        }

        void AddLoadReference(string fxid, ILoadReference loadRef, float timeLenght)
        {
            //一种效果只能添加一个,先移除旧的
            RemoveEffect(fxid);
            if (loadRef != null)
            {
                Info info = new Info();
                info.fxid = fxid;
                info.loadRef = loadRef;
                if (timeLenght <= 0)
                    info.finishTime = float.MaxValue;
                else
                    info.finishTime = BattleHelp.timePass + timeLenght;
                m_list.Add(info);
            }
        }

        public void RemoveEffect(string fxid)
        {
            if (string.IsNullOrEmpty(fxid))
                return;
            //一种效果只能添加一个
            for (int i = m_list.Count - 1; i >= 0; i--)
            {
                if (m_list[i].fxid == fxid)
                {
                    RemoveEffectImpl(m_list[i]);
                    m_list.RemoveAt(i);
                    return;
                }
            }
        }


        class Info
        {
            public string fxid;
            public ILoadReference loadRef;
            public float finishTime;
        }
        List<Info> m_list = new List<Info>();
        IObject m_obj;
        public void OnAwake(IObject obj) { m_obj = obj; }
        public void OnStart() { }
        public void OnDestroy() { m_obj = null; }
        public void OnEnterScene() { }
        public void OnExitScene() 
        {
            for (int i = m_list.Count - 1; i >= 0; i--)
            {
                 m_list[i].loadRef.SetDestroy();
            }
            m_list.Clear();
        }

        void RemoveEffectImpl( Info info )
        {
            //移除时重置材质
            if (info.loadRef is MaterialLoadReference)
                m_obj.battle.actor.m_partManager.ResetMaterial();
            info.loadRef.SetDestroy();
        }

        //避免特效被移除了没有通知到这里
        public void OnUpdate()
        {
            for (int i = m_list.Count - 1; i >= 0; i--)
            {
                if (m_list[i].loadRef.IsDestroy() || BattleHelp.timePass > m_list[i].finishTime)
                {
                    RemoveEffectImpl(m_list[i]);
                    m_list.RemoveAt(i);
                    return;
                }
            }
        }
    }


    //静态函数，一般都需要美术保证销毁
    public static class EffectManager
    {
        //给角色挂一个特效,这种特效必定是有美术控制销毁的
        public static void PlayEffect(IObject target, string fxid, Action<GameObject, object> call = null, object para = null)
        {
            if (string.IsNullOrEmpty(fxid) || target == null)
                return;
            EffectConfig fx = EffectConfig.Get(fxid);
            if (fx != null)
                fx.CreateEffect(target.root, call, para);
        }

        //受击特效
        public static void PlayHurtFx(IObject source, IObject target, AttackActionConfig cfg)
        {
            string fxid = cfg.hitEffect;
            Vector3 rotate = source.root.eulerAngles;
            string hitEffectAngle = cfg.hitEffectAngle;

            if (string.IsNullOrEmpty(fxid))
                return;
            EffectConfig fx = EffectConfig.Get(fxid);
            if (fx != null)
                fx.CreateEffect(target.root, OnLoadHurtFxComplete, new object[] { rotate, hitEffectAngle, source, target });
        }

        //受击特效加载完成
        static void OnLoadHurtFxComplete(GameObject obj, object info)
        {
            GameObject go = obj;
            object[] paraList = (object[])info;
            string hitEffectAngle = paraList[1] as string;
            IObject source = paraList[2] as IObject;
            IObject target = paraList[3] as IObject;

            //BaseAttackEffect effect = obj.GetComponent<BaseAttackEffect>();
            //if (effect != null)
            //    effect.BeginEffect(source, target, m_role.transform, source.transform);

            //没有配置角度则不要修改
            if (hitEffectAngle.Length == 0)
                return;

            BattleHelp.CheckEffectDestroy(obj);

            Vector3 roleForward = (Vector3)paraList[0];
            go.transform.eulerAngles = roleForward;
            float angle = UnityEngine.Random.Range(-30f, 30f);
            Vector3 rotate = Vector3.zero;
            if (hitEffectAngle.Length == 0 || hitEffectAngle == "左")
            {
                rotate.y = 270 - angle;
            }
            else if (hitEffectAngle == "右")
            {
                rotate.y = 90 - angle;
            }
            else if (hitEffectAngle == "上")
            {
                rotate.x = 270 - angle;
            }
            else if (hitEffectAngle == "下")
            {
                rotate.x = 90 - angle;
            }
            else if (hitEffectAngle == "外")
            {
                rotate.y = angle;
            }
            go.transform.Rotate(rotate);
        }

        #region 预警
        //预警
        public static PrefabsLoadReference PlayWarning(IObject source, IObject searchTarget, SearchActionConfig cfg, float timeLen)
        {
            if (cfg == null)
                return null;

            //特效位置
            Vector3 pos = SearchAction.GetSearchPos(searchTarget.position, source.rotateAngle, cfg);
            //特效名字
            string fxName = null; ;
            bool isEnemy = BattleHelp.IsEnemy(App.my.localPlayer, source);
            Vector3 size = Vector3.one;

            pos.y += 0.2f;
            size.y = 0.1f;

            if (cfg.searchType == SearchActionConfig.SearchSharp.Rect)
            {
                if (isEnemy)
                    fxName = "fx_warning_line_3_enemy";
                else
                    fxName = "warning_line_player";
                size.x = cfg.searchPara[0];
                size.z = cfg.searchPara[1];
            }
            else if (cfg.searchType == SearchActionConfig.SearchSharp.Angle)
            {
                float angle = cfg.searchPara[1];
                int id = 0;
                if (angle <= 180)
                    id = 0;
                else
                    id = 1;
                string[] selfPrefabs = { "fx_warning_120_player", "fx_warning_360_player" };
                string[] enemyPrefabs = { "fx_warning_180_enemy", "fx_warning_360_enemy" };
                if (isEnemy)
                    fxName = enemyPrefabs[id];
                else
                    fxName = selfPrefabs[id];
                size.z = cfg.searchPara[0];
                size.x = cfg.searchPara[0];
            }
            else
            {
                Debug.LogError("找不到预警类型 id=" + cfg.id);
                return null;
            }

            PrefabsLoadReference load = new PrefabsLoadReference(true);
            load.Load(fxName, OnLoadWarning, new object[] { timeLen, size }, pos, source.root.rotation);
            return load;
        }

        static void OnLoadWarning(GameObject go, object para)
        {
            object[] pList = (object[])para;
            go.transform.localScale = (Vector3)pList[1];
            //调整动画时间
            float time = (float)pList[0];
            Animation anim1 = go.GetComponentInChildren<Animation>();
            Animator anim2 = go.GetComponentInChildren<Animator>();
            if (anim1 != null)
            {
                foreach (AnimationState aniState in anim1)
                {
                    if (!aniState.enabled)
                        continue;
                    if (time == 0)
                        aniState.speed = 1;
                    else
                        aniState.speed = aniState.length / time;
                }
            }
            if (anim2 != null)
            {
                AnimatorClipInfo[] clips = anim2.GetCurrentAnimatorClipInfo(0);
                if (clips.Length > 0 && clips[0].clip.length != 0)
                {
                    if (time == 0)
                        anim2.speed = 1;
                    else
                        anim2.speed = clips[0].clip.length / time;
                }

                else
                {
                    clips = anim2.GetNextAnimatorClipInfo(0);
                    if (clips.Length > 0 && clips[0].clip.length != 0)
                    {
                        if (time == 0)
                            anim2.speed = 1;
                        else
                            anim2.speed = clips[0].clip.length / time;
                    }
                }
            }
        }
        #endregion
    }



}

