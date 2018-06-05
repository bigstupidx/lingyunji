using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys;
using xys.UI;


namespace xys.battle
{
    public class ObjectMono : MonoBehaviour
    {
        public IObject m_obj { get; private set; }
        public bool m_testAni = false;
        public bool m_testSkill = false;

        public void Init(IObject obj)
        {
            this.m_obj = obj;
        }

        void OnDestory()
        {
            m_obj = null;
        }
    }
}


