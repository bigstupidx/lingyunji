using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace xys.GameStory
{
    /// <summary>
    /// 控制角色移动
    /// </summary>
    public class MoveComponent : IObjectComponent
    {

        StoryObjectBase m_obj;

        Transform m_rootObject;// 根节点，不管有没有加载都会创建出来,只有obj销毁才会销毁
        CharacterController m_characterCtrl;// 移动控制器，控制root对象移动

        public Transform rootObject
        {
            get { return m_rootObject; }
        }

        /// <summary>
        /// 每帧移动的增量值（movement deltas）
        /// </summary>
        /// <param name="motion"></param>
        public void CCMove(Vector3 motion)
        {
            if (m_characterCtrl != null)
            {
                m_characterCtrl.Move(motion);
            }
        }

        public void LookDirection(Vector3 direction)
        {
            if (m_rootObject!=null)
            {
                float turnAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                m_rootObject.localEulerAngles = Vector3.up * turnAngle;// xys.battle.BattleHelp.Vector2Angle(direction);
            }
        }

        /// <summary>
        /// 移动速度向量值
        /// </summary>
        /// <param name="speed"></param>
        public void CCSMove(Vector3 speed)
        {
            if (m_characterCtrl != null)
            {
                m_characterCtrl.SimpleMove(speed);
            }
        }

        public void OnAwake(StoryObjectBase obj)
        {
            m_obj = obj;
            m_rootObject = (new GameObject(string.Format("{0}({1})-{2}", m_obj.model, m_obj.name, m_obj.objectStoryID))).transform;
            m_rootObject.position = m_obj.bornPosition;

            m_characterCtrl = m_rootObject.AddComponentIfNoExist<CharacterController>();
            m_characterCtrl.center = new Vector3(0, m_characterCtrl.height / 2, 0);
            m_rootObject.gameObject.layer = ComLayer.Layer_NoColliderRole;
        }

        public void OnStart()
        {

        }

        public void OnDestroy()
        {

            if (m_rootObject != null)
                GameObject.Destroy(m_rootObject.gameObject);
        }

        public void OnUpdate()
        {

        }
    }

}
