#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [AutoILMono]
    class UITexts
    {
        //名称
        [SerializeField]
        Text m_name;
        //门派
        [SerializeField]
        Text m_type;
        //描述
        [SerializeField]
        Text m_description;
        //有效期
        [SerializeField]
        Text m_validTime;

        public void Set(string name,int type,string discription,string time)
        {
            if(!string.IsNullOrEmpty(name))
            {
                m_name.text = name;
            }
            if(type==1)
            {
                m_type.text = "【时装】";
            }
            else if(type==2)
            {
                m_type.text = "【门派】";
            }
            else
            {
                m_type.text = "";
            }
            if(!string.IsNullOrEmpty(discription))
            {
                m_description.text = discription;
            }
            if (!string.IsNullOrEmpty(time))
            {
                m_validTime.text = "有效期：" + time;
            }
            
        }
    }
}
#endif