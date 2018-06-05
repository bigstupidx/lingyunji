using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys;
using xys.UI;

namespace xys.battle
{
    /// <summary>
    /// 输入管理
    /// </summary>
    public class InputManager
    {
        //移动方向
        Vector3 m_moveWay;
        bool m_isMoving;
        //正在使用摇杆移动
        public bool IsMoving()
        {
            return m_isMoving;
        }

        //获得输入的移动方向
        public Vector3 GetMoveWay()
        {
            return m_moveWay;
        }


        //设置移动方向(screenPos为摇杆偏移百分比)
        public void SetMoveWay(Vector2 screenPos)
        {
            //移动摇杆太小，则不算移动
            if (screenPos.magnitude < 0.1f)
            {
                m_isMoving = false;
                m_moveWay = Vector3.zero;
                App.my.cameraMgr.LerpToNormal(false);
            }
            else
            {
                //

                m_isMoving = true;
                Camera camera = App.my.cameraMgr.m_mainCamera;
                if (camera == null)
                    return;
                m_moveWay.y = 0;
                m_moveWay.x = screenPos.x;
                m_moveWay.z = screenPos.y;
                Quaternion rot = Quaternion.Euler(0, camera.transform.rotation.eulerAngles.y, 0);
                m_moveWay.Normalize();
                m_moveWay = rot * m_moveWay;

                App.my.cameraMgr.LerpToNormal(true);
            }
        }

        #region 测试代码
        bool m_iskeyboardHolding;
        //强制移动，不用手动按
        Vector3 m_forcemove = Vector3.zero;
        //模拟按键
        public void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            {
                Vector2 pos = Vector2.zero;
                if (Input.GetKey(KeyCode.W))
                    pos.y += 1;
                if (Input.GetKey(KeyCode.S))
                    pos.y -= 1;
                if (Input.GetKey(KeyCode.A))
                    pos.x -= 1;
                if (Input.GetKey(KeyCode.D))
                    pos.x += 1;

                //强制移动，不需要手动按
                if (Input.GetKey(KeyCode.F2))
                    m_forcemove = pos;
                if (m_forcemove != Vector3.zero)
                    pos = m_forcemove;

                if (pos != Vector2.zero)
                {
                    if (!m_iskeyboardHolding)
                        BeginInput();
                    SetMoveWay(pos);
                    m_iskeyboardHolding = true;
                }
                //松开按键
                else if (m_iskeyboardHolding)
                {
                    m_iskeyboardHolding = false;
                    SetMoveWay(pos);
                }

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    //App.my.eventSet.FireEvent<int>(EventID.MainPanel_FireSkill, 0);
                    //App.my.localPlayer.battle.m_skillMgr.PlaySkillImpl(SkillConfig.Get(1001),null,null);
                    //App.my.uiSystem.progressMgr.PlaySkillCasting(new ProgressData() { timeLenght = 3, titleName="测试"});
                    //if (App.my.appStateMgr.curState == AppStateType.GameIn)
                    //    App.my.localPlayer.battle.m_attrLogic.SetBattleState(!App.my.localPlayer.battle.m_attrMgr.battleState);
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    //App.my.eventSet.FireEvent<int>(EventID.MainPanel_FireSkill, 3);
                    //App.my.localPlayer.battle.m_jumpMgr.PlayJump();
                    //if (App.my.appStateMgr.curState == AppStateType.GameIn)
                    //    App.my.localPlayer.battle.m_effectMgr.AddEffect("test",3.0f);
                }
            }
#endif
        }
        #endregion


        #region 管理摇杆再次按下
        public enum InputFlg
        {
            //避免按着移动时打断施法动作,需要再次按才能打断
            SkillFlg,
            //玩家这时候需要重新再按方向键才能打断移动
            MoveToSkillFlg,
            Cnt
        };
        int[] m_touchCntFlg = new int[(int)InputFlg.Cnt];
        int m_curTouchCnt = 0;

        //按下摇杆
        public void BeginInput()
        {
            m_curTouchCnt++;
        }
        public void ResetJoyInputCnt(InputFlg flg)
        {
            m_touchCntFlg[(int)flg] = m_curTouchCnt;
        }

        //是否有输入移动指令.要抬起手一次
        //先调用ResetJoyInputCnt,那下次要抬手一次该回调才会返回true
        public bool IsReInput(InputFlg flg)
        {
            return m_isMoving && m_touchCntFlg[(int)flg] != m_curTouchCnt;
        }
        #endregion
    }

}
