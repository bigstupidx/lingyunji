using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//特效触发
[System.Serializable]
public class SoundTrigger : BaseEffectTrigger
{
    public string m_sound;                    //预制体
    //public AudioClip m_soundClip;               //音源
    public int m_beginFrame;                    //开始帧
    public bool m_stopByAniFinish = false;	    //动作结束会自动销毁
    public bool m_is3DSound = false;            //是否3d音效
#if !SCENE_DEBUG
    public FMOD.Studio.EventInstance m_soundInstance = null; //声音实例 
#endif
}


public class SoundTriggerLogic : BaseEffectTriggerLogic
{
    public SoundTrigger m_data;
    //bool m_isPlay;                              //是否正在播放   
    public bool m_loop = false;		            //是否循环创建
    int m_loopCnt;                              //循环次数

    public SoundTriggerLogic(SoundTrigger data)
    {
        m_data = data;
    }

    public override void Play(AnimationEffectManage triggerManage)
    {
        //m_isPlay = true;
        m_loopCnt = -1;
    }

    public override void Update(AnimationEffectManage triggerManage, int frame, int loopCnt, string ani)
    {
        if (string.IsNullOrEmpty(m_data.m_sound))
            return;

        //非循环不会创建多次
        if (loopCnt >= 1 && !m_loop)
            return;

        bool isFrame = (m_data.m_beginFrame <= frame);
        if (m_loopCnt != loopCnt)
        {
            if (isFrame)
            {
                m_loopCnt = loopCnt;

                if (m_data.m_stopByAniFinish && m_loopCnt >= 1)
                {
                    Finish(triggerManage);
                }
                else
                {
//                    GameMusicManage.Play3DSoundEffectByPath(m_data.m_sound, triggerManage.transform);
#if !SCENE_DEBUG
                    if(m_data.m_is3DSound)
                    {
                        m_data.m_soundInstance = xys.SoundMgr.PlayOneShotAttached(m_data.m_sound, triggerManage.transform.gameObject);
                    }
                    else
                    {
                        m_data.m_soundInstance = xys.SoundMgr.PlayOneShot(m_data.m_sound);
                    }
#endif
                }
            }
        }
    }

    public override void Finish(AnimationEffectManage triggerManage)
    {
        //m_isPlay = false;
        if (m_data.m_stopByAniFinish)
        {
            //这里调用音乐结束的接口
//            GameMusicManage.StopSoundByPath(m_data.m_sound);
#if !SCENE_DEBUG
            if(m_data.m_soundInstance != null)
            {
                m_data.m_soundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }
#endif
        }
    }
}