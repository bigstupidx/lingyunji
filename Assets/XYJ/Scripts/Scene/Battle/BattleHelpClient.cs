using Config;
using GameServer;
using NetProto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace xys.battle
{
    public partial class BattleHelp
    {
        [Conditional("UNITY_EDITOR")]
        public static void CheckEffectDestroy(GameObject go)
        {
            EffectDestroy p = go.GetComponent<EffectDestroy>();
            if (p == null)
                Debuger.LogError("特效没有挂销毁脚本 name=" + go.name);
        }

        //是否本地跑战斗逻辑
        public static bool IsRunBattleLogic()
        {
            return BattleProtocol.testCtrlByLocal;
        }

        public static bool IsMe(IObject obj)
        {
            return obj == App.my.localPlayer;
        }
        public static void SetLookAt(Transform trans, Vector3 toPos)
        {
            toPos.y = trans.position.y;
            if (trans.position.x != toPos.x || trans.position.z != toPos.z)
                trans.LookAt(toPos);
        }

        //获得角色间距离，忽略y轴
        public static float GetDistance(Transform a, Transform b)
        {
            Vector3 pos1 = a.position;
            Vector3 pos2 = b.position;
            pos2.y = pos1.y;
            return Vector3.Distance(pos1, pos2);
        }

        //获得攻击距离和角度是否满足
        static public bool CheckAttackDistanceAndAngle(IObject source, IObject target, float disMin, float disMax, float angleLimit)
        {
            float dis = GetAttackDistance(source.position, target);
            if (dis > disMax || dis < disMin)
                return false;

            if (angleLimit >= 180)
                return true;

            Vector3 targetDir = target.position - source.position;
            targetDir.y = 0;
            float angle = Vector3.Angle(source.battle.m_root.forward, targetDir);
            return angle <= angleLimit;
        }

        //忽略y轴
        public static void SetParent(Transform trans, Transform parent)
        {
            trans.parent = parent;
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
        }

        public static string GetFullPath( Transform trans )
        {
            string path = "";
            while(trans!=null)
            {
                path = string.Format("{0}/{1}",trans.name,path);
                trans = trans.parent;
            }
            return path;
        }
        //获得地表坐标 
        public static bool GetGroundPos(ref Vector3 pos)
        {
            return GetColliderPos(ref pos, ComLayer.Layer_RoleFallDownMask);
        }

        //设置贴地
        public static void SetGround(IObject obj)
        {
            Vector3 pos = obj.position;
            if (GetGroundPos(ref pos))
                obj.SetPosition(pos);
        }

        static RaycastHit[] s_hits = new RaycastHit[10];
        static bool GetColliderPos( ref Vector3 pos, int layerMask)
        {
            //拔高5米，搜索一段范围内最接近当前位置的碰撞
            Vector3 tempPos = pos;
            tempPos.y += 10.0f;
            int minIndex = 0;
            float minDis = 1000;
            
            int count = Physics.RaycastNonAlloc(tempPos, Vector3.down,s_hits, 20, layerMask);
            if (count > 0)
            {
                //找到最近的点
                for (int i = 0; i < count; ++i)
                {
                    float dis = Mathf.Abs(pos.y - s_hits[i].point.y);
                    if (minDis > dis)
                    {
                        minDis = dis;
                        minIndex = i;
                    }
                }

                if (minIndex >= 0 )
                {
                    pos = s_hits[minIndex].point;
                    return true;
                }
            }
            return false;
        }
    }
}