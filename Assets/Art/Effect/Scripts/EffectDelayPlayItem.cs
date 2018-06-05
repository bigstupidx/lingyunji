/********************************************************************************
 *	文件名：	EffectDelayPlayItem.cs
 *	创建人：	weipeng
 *	创建时间：  2016.05.31
 *
 *	功能说明： 特效延迟播放子类
 *           
 *	
 *	修改记录：
 *********************************************************************************/
using UnityEngine;
using System.Collections;

[System.Serializable]
public class EffectDelayPlayItem
{
    public GameObject m_effectObj;
    public float m_delay;
    int m_timerId;
    public void PlayEffect()
    {
        m_effectObj.SetActive(false);
        //m_timerId = Utils.TimerManage.AddTimer(DelayFunc, m_delay);
    }
    void DelayFunc()
    {
        m_effectObj.SetActive(true);
    }
    public void StopEffect()
    {
        m_effectObj.SetActive(false);
        //Utils.TimerManage.DelTimer(m_timerId);
    }
}
