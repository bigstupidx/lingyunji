using UnityEngine;
using System.Collections;


namespace xys.battle
{
    public class TargetManager : IBattleComponent
    {
        protected IObject m_target;
        protected IObject m_obj;
        //不激活时仇恨值和目标都不能设置
        protected bool m_active = true;
        public IObject target
        {
            get
            {
                if (m_target != null && m_target.isAlive)
                    return m_target;
                else
                    return null;
            }
        }

        public virtual void SetTarget(IObject obj)
        {
            if (!m_active)
                return;
            m_target = obj;
        }

        public virtual void AddValue(IObject obj,int value )
        {

        }

        public virtual void SetActive(bool active)
        {
            m_active = active;
        }

        public void OnAwake(IObject obj)
        {
            m_obj = obj;
        }
        public void OnStart()
        {

        }
        public virtual void OnDestroy()
        {
            m_obj = null;
        }

        public virtual void OnEnterScene()
        {

        }
        public void OnExitScene()
        {
            m_target = null;
        }

        public virtual void Damage()
        {

        }
    }
}
