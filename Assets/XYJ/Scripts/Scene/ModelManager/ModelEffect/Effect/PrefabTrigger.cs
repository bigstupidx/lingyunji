using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//特效触发
[System.Serializable]
public class PrefabTrigger : BaseEffectTrigger
{
    //根据不同类型特效做一些特殊处理
    public enum Type
    {
        Normal,         //普通特效
        Materail        //材质特效类型
    };
    public string m_prefabs;                //预制体             
    public string m_boneName;               //骨骼名字
    public bool m_followPos = false;        //是否跟随
    public bool m_followRotate = false;     //是否跟着旋转
    public int m_beginFrame;                //开始帧
    public int m_stopFrame = -1;              //停止侦
    public bool m_destroyByAniFinish = false;	   //动作结束会自动销毁
    public bool m_destroyByObjDestroy = true;      //对象销毁时会自动销毁
    public bool m_destroyBySkill = false;      //技能结束时会自动销毁
    public bool m_stopFollowByAniFinish = false;   //动作结束后就不跟随
    public int m_stopFollowFrame = 0;      //停止跟随的时间
    public bool m_isScale = true;	        //创建时根据父节点缩放
    public bool m_loop = false;		        //是否循环创建
    public bool m_useSelfRotation = false;  //使用世界角度
    public bool m_isPlayOnlyMe = false;     //只有本地玩家才生效
    public Type m_type;                     //特效类型
    public bool m_mustCreate;               //必须创建的特效，有时候动作被中断了或者触发帧被跳过了也必须要触发
    public bool m_onlyOneInstance = false;  //只有一个实例
}

public class PrefabTriggerLogic : BaseEffectTriggerLogic
{
    public PrefabTrigger m_data;
    bool m_isPlay;//是否正在播放
    int m_loopCnt;//循环次数
    [System.NonSerialized]
    public GameObject m_obj;//创建的特效

    public PrefabTriggerLogic( PrefabTrigger data)
    {
        m_data = data;
    }

    public override void Play(AnimationEffectManage triggerManage)
    {
        m_isPlay = true;
        m_loopCnt = -1;

        //保证不删除的特效只有一个
        if (m_obj != null && (m_data.m_destroyByObjDestroy || m_data.m_destroyBySkill))
            GameObject.Destroy(m_obj);
    }

    public override void Update(AnimationEffectManage triggerManage, int frame, int loopCnt, string ani)
    {
        if (string.IsNullOrEmpty(m_data.m_prefabs))
            return;
        //设置了本地玩家才有效
        if (m_data.m_isPlayOnlyMe && !triggerManage.m_isMe)
            return;
        //到了结束帧
        if (m_data.m_stopFrame != -1 && frame >= m_data.m_stopFrame)
        {
            DestroyObj();
            return;
        }

        //到了指定帧数,判断是否需要跟随
        if (m_data.m_stopFollowFrame > 0 && frame >= m_data.m_stopFollowFrame)
        {
            if (m_obj != null && null != m_obj.GetComponent<EffectFollowBone>())
            {
                m_obj.GetComponent<EffectFollowBone>().m_trans = null;
            }
        }

        //非循环不会创建多次
        if (loopCnt >= 1 && !m_data.m_loop)
            return;

        bool isFrame = (m_data.m_beginFrame <= frame);
        if (m_loopCnt != loopCnt)
        {
            if (isFrame)
            {
                m_loopCnt = loopCnt;
                if (m_data.m_destroyByAniFinish && m_loopCnt >= 1)
                    DestroyObj();

                //只允许播放一个实例
                if (m_data.m_onlyOneInstance)
                {
                    if (triggerManage.IsEffectExist(m_data.m_prefabs))
                        return;
                    if (!m_data.m_destroyByObjDestroy)
                    {
                        Debuger.LogError("只允许播放一个实例的特效销毁方式必须是 m_destroyByObjDestroy");
                        return;
                    }
                }
                XYJObjectPool.LoadPrefab(m_data.m_prefabs, CreateGoFinish, triggerManage);
            }
        }
    }

    void DestroyObj()
    {
        if (m_obj != null)
        {
            XYJObjectPool.Destroy(m_obj);
            m_obj = null;
        }
    }

    void CreateGoFinish(GameObject go, object p)
    {
        //创建过程已经结束了
        if (!m_isPlay && !m_data.m_mustCreate)
        {
            XYJObjectPool.Destroy(go);
            return;
        }

        AnimationEffectManage triggerManage = (AnimationEffectManage)p;
        //Vector3 pos = Vector3.zero;
        //Quaternion rot = Quaternion.identity;
        go.name = m_data.m_prefabs;

        //播放特效事件
        IEffectBaseEvent effectEvent = go.GetComponent<IEffectBaseEvent>();
        if (effectEvent != null)
            effectEvent.PlayEvent(triggerManage);

        //部分特效漏勾选了IsPlayOnlyMe
#if UNITY_EDITOR
        BaseRoleCameraEffect cameraEffect = go.GetComponent<BaseRoleCameraEffect>();
        if (cameraEffect != null && cameraEffect.testIsOnlyMe != m_data.m_isPlayOnlyMe)
        {
            Debuger.LogError( string.Format("该特效漏了勾选IsPlayOnlyMe go={0} ani={1} fx={2}",triggerManage.gameObject.name,triggerManage.m_stName,m_data.m_prefabs));
        }          
#endif

        Transform bone = triggerManage.m_boneManage.GetBone(m_data.m_boneName);
        if (bone != null)
        {
            //pos = bone.transform.position;
            if (!m_data.m_useSelfRotation)
            {
                //rot = bone.transform.rotation;
            }
        }
        if (!go.activeSelf)
            go.SetActive(true);
        if (m_data.m_destroyByObjDestroy || m_data.m_destroyBySkill)
            triggerManage.SetEffectNeedDestroy(this);
        if (!m_data.m_destroyByAniFinish && m_data.m_stopFrame == -1 && !m_data.m_destroyByObjDestroy)
            XYJObjectPool.CheckDestroy(go, "动作");//不导出中文

        //triggerManage.triggerDataImpl.m_goList.Add(go); 
        m_obj = go;

        //处理跟随
        //材质特效直接挂在骨骼点下面
        if (m_data.m_type == PrefabTrigger.Type.Materail)
        {
            if (bone != null)
                go.transform.parent = bone;
            else
                go.transform.parent = triggerManage.gameObject.transform;
            MaterialAnimation2 effect = go.GetComponent<MaterialAnimation2>();
            if (effect != null)
                effect.Play();
        }
        //非材质的通过脚本来跟随
        else
        {
            if (bone != null)
            {
                go.transform.position = bone.transform.position;
                go.transform.rotation = bone.transform.rotation;
            }

            //跟随模型放缩
            if (m_data.m_isScale)
                go.transform.localScale = triggerManage.gameObject.transform.lossyScale;
            //特效放缩
            else
                go.transform.localScale = Vector3.one;

            if (m_data.m_followPos)
            {
                if (bone != null)
                    EffectFollowBone.AddFollow(go, bone.transform, m_data.m_followRotate, Vector3.zero, false);
            }

            if (go.transform.parent == null)
                go.transform.parent = AnimationEffectManage.GetEffectRoot();
        }

    }

    //动作结束
    public override void Finish(AnimationEffectManage triggerManage)
    {
        if (m_data.m_destroyByAniFinish || m_data.m_stopFrame != -1)
        {
            DestroyObj();

            ////设置摄像机跟随
            //if (null != m_obj && null != m_obj.GetComponent<SkillCamera>())
            //{
            //    m_obj.GetComponent<SkillCamera>().Finish();
            //}
        }

        //保证触发
        if (m_data.m_mustCreate && m_obj == null)
            Update(triggerManage, int.MaxValue, 0, "");

        //设置不跟随
        if (m_data.m_stopFollowByAniFinish && m_data.m_followPos)
        {
            if (m_obj != null && null != m_obj.GetComponent<EffectFollowBone>())
            {
                m_obj.GetComponent<EffectFollowBone>().m_trans = null;
            }
        }
        m_isPlay = false;
    }
}