/********************************************************************************
 *	文件名：	EffectDelayPlay.cs
 *	创建人：	weipeng
 *	创建时间：  2016.05.31
 *
 *	功能说明： 特效延迟播放类
 *           
 *	
 *	修改记录：
 *********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectDelayPlay : MonoBehaviour 
{

    public List<EffectDelayPlayItem> m_list;

    void OnEnable()
    {
        if(m_list != null)
        {
            for(int i = 0; i < m_list.Count; i ++)
            {
                EffectDelayPlayItem item = m_list[i];
                item.PlayEffect();
            }
        }
    }

    void OnDisable()
    {
        if (m_list != null)
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                EffectDelayPlayItem item = m_list[i];
                item.StopEffect();
            }
        }
    }
}
