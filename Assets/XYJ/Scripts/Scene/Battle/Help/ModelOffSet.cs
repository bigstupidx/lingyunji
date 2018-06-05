using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    public class ModelOffSet 
    {
        public enum MoveType
        {
            Ground,
            JiFei,  //击飞时需要处理锁定镜头
        }
        AccelerationMove m_move = new AccelerationMove();
        //角色击飞是要
        MoveType moveType = MoveType.Ground;
        // 模型创建的默认转角
        Vector3 m_defaultRotation = Vector3.zero;
        Transform m_bodyTrans;

        public void SetTrans(Transform trans)
        {
            m_bodyTrans = trans;
        }

        //获取移动状态
        public MoveType getMoveType()
        {
            return moveType;
        }

        public float HeightOff()
        {
            return m_bodyTrans.localPosition.y;
        }
        //浮空
        public void HitToSky(  IObject obj, float moveTime )
        {
            float toHeight = kvBattle.toSkyHeight;
            float fromSpeed = 30;

            //模型放缩了，但是浮空高度不要放缩
            toHeight = toHeight / obj.root.localScale.y;
            ChangeHeight( toHeight, fromSpeed, moveTime);
            moveType = MoveType.JiFei;
        }

        //自由落地
        public void FallToGround(float moveTime)
        {
            ChangeHeight(0, 0, moveTime);
            moveType = MoveType.JiFei;
        }

        //浮空击倒到地
        public void HitToGround(float moveTime)
        {
            float fromSpeed = 30;
            ChangeHeight(0, fromSpeed, moveTime);
            moveType = MoveType.JiFei;
        }


        //变速运动
        //如果fromspeed和tospeed填的不合理，会导致运动达不到预期
        void ChangeHeight( float toHeight,float fromSpeed,float moveTime )
        {
            float fromHeight = m_bodyTrans.localPosition.y;
            m_move.PlaySpeed(fromHeight, toHeight, fromSpeed, moveTime);
        }

        public void ResetHeight(IObject obj)
        {
            m_bodyTrans.localPosition = Vector3.zero;
            m_move.Reset();

            //击飞镜头处理
            if (moveType == MoveType.JiFei && App.my.cameraMgr!= null)
            {
                //玩家被挑飞的时候修改玩家镜头高度
                if (obj == App.my.localPlayer )
                    App.my.cameraMgr.ResetPlayerHeight();
                else if (obj == App.my.localPlayer.GetUIChooseTarget())
                {
                    App.my.cameraMgr.ResetTargetHeight();
                }
            }
        }

        public void UpdateMove( IObject obj)
        {
            if (m_bodyTrans == null)
                return;
            ////跟随动作改变坐标
            //if (m_createRoot_M != null && m_trueRoot_M != null && m_createRoot_M.transform != null && m_trueRoot_M.transform!=null)
            //    m_createRoot_M.position = m_trueRoot_M.position;

            if (!m_move.IsFinish())
            {
                Vector3 pos = Vector3.zero;
                pos.y = m_move.UpdateMove();
                m_bodyTrans.localPosition = pos;           
    
                //击飞镜头处理
                if (moveType == MoveType.JiFei)
                {
                    //玩家被挑飞的时候修改玩家镜头高度
                    if (obj == App.my.localPlayer)
                    {
                        float playerHeight = (1 - m_bodyTrans.localPosition.y / kvBattle.toSkyHeight) * App.my.cameraMgr.m_defaultPlayerHeight;
                        App.my.cameraMgr.SetPlayerHeight(playerHeight);
                    }
                    //摄像机锁定的目标被挑飞
                    else if (obj == App.my.localPlayer.GetUIChooseTarget())
                    {
                        float playerHeight = (1 - m_bodyTrans.localPosition.y / kvBattle.toSkyHeight) * App.my.cameraMgr.m_defaultTargetHeight;
                        App.my.cameraMgr.SetTargetHeight(playerHeight);
                    }
                }
                //else if (moveType == MoveType.Fly)
                //{
                //    if (m_bodyTrans.localPosition.y > 0)
                //        m_role.SetLayer(ComLayer.Layer_Terrains);
                //    else
                //    {
                //        m_role.ResetLayer();
                //        moveType = MoveType.Ground;
                //        m_role.m_buffManage.RemoveFlag(BuffManage.Flag.NoGravity);
                //    }
                //}
            }
        }
    }
}
    