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
        //单向的，a是b敌人，但反过来不一定成立
        public static bool IsEnemy(IObject source, IObject target)
        {
            return GetRelation(source, target) == BattleRelation.Enemy;
        }

        public static void Subscribe(IObject obj, NetProto.AttType e2, System.Action<CommonBase.IAttribute<NetProto.AttType>> action)
        {
#if COM_SERVER
            obj.eventSet.Subscribe<CommonBase.IAttribute<NetProto.AttType>>(GameServer.EventID.AttChange, e2, action);
#else
            obj.eventSet.Subscribe<AttributeChange>(e2, (p) => { action(p.currentValue); });
#endif
        }

        //先简单判断
        public static BattleRelation GetRelation(IObject source, IObject target)
        {
            if (source.battleCamp == target.battleCamp)
                return BattleRelation.Friend;
            else
            {
                if (source.battleCamp == BattleCamp.NeutralCamp
                    || target.battleCamp == BattleCamp.NeutralCamp)
                    return BattleRelation.Neutral;
                return BattleRelation.Enemy;
            }
        }


        //获得角色间距离，忽略y轴
        public static float GetDistance(IObject a, IObject b)
        {
            return GetDistance(a.position, b.position);
        }



        //获得角色间距离，忽略y轴
        public static float GetDistance(Vector3 a, Vector3 b, bool ingoreY = true)
        {
            if (ingoreY)
                a.y = b.y;
            return Vector3.Distance(a, b);
        }

        //获得攻击距离
        public static float GetAttackDistance(Vector3 pos, IObject target)
        {
            float dis = GetDistance(pos, target.position) - target.cfgInfo.behitRaidus;
            if (dis < 0)
                return 0;
            else
                return dis;
        }


        public static int HashCode(string value)
        {
            int h = 0;
            if (value.Length > 0)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    h = 31 * h + value[i];
                }
            }
            return h;
        }

        public static IObject GetAOIObj(IObject obj, int charid)
        {
            IObject target;
            if (GetAOIObj(obj).TryGetValue(charid, out target))
                return target;
            else
                return null;
        }

        //如果charid为0则返回obj
        public static IObject GetObjByID(IObject obj, int charid)
        {
            if (charid == 0)
                return obj;
            else
                return GetAOIObj(obj, charid);
        }

        //是否命中
        public static bool RandPercent(float rate)
        {
            int t = (int)(rate * 100);
            return Rand(0, 100) < t;
        }

        //根据比重随机选择id，如果rates为null或者全为0，则相同比重随机
        public static int RandRates(List<int> rates, int idcnt)
        {
            int total = 0;
            if (rates != null)
            {
                foreach (var p in rates)
                    total += p;
            }
            if (total == 0)
                return BattleHelp.Rand(0, idcnt);

            int randInt = BattleHelp.Rand(0, total);
            int curSumRand = 0;
            for (int i = 0; i < rates.Count; i++)
            {
                curSumRand += rates[i];
                if (curSumRand > randInt)
                    return i;
            }
            return 0;
        }

        //保证值在合适范围内
        public static void CheckValue(ref float v, float min, float max)
        {
            if (v < min)
                v = min;
            else if (v > max)
                v = max;
        }

        #region Vector3计算
        //旋转,后面再优化,可以缓存结果
        public static Vector3 RotateAngle(Vector3 v, float angle)
        {
            float rad = angle * Mathf.Deg2Rad;
            Vector3 r;
            float cosV = Mathf.Cos(rad);
            float sinV = Mathf.Sin(rad);
            r.x = v.x * cosV + v.z * sinV;
            r.z = -v.x * sinV + v.z * cosV;
            r.y = v.y;
            return r;
        }

        //根据向量获得y角度
        public static float Vector2Angle(Vector3 v)
        {
            int angle = 0;

            if (v.z == 0)
            {
                if (v.x > 0)
                    angle = 90;
                else if (v.x < 0)
                    angle = 270;
            }
            else
            {
                angle = (int)(Mathf.Atan(v.x / v.z) * Mathf.Rad2Deg);
                //2，3象限
                if (v.z < 0)
                    angle += 180;
                else if (v.z > 0 && v.x < 0)
                    angle += 360;
            }
            return angle;
        }

        public static float Vector2Angle(Vector3 toPos, Vector3 fromPos)
        {
            return Vector2Angle(toPos - fromPos);
        }


        public static Vector3 Normalize(Vector3 from, Vector3 to)
        {
            Vector3 tem = to - from;
            tem.y = 0;
            if (tem == Vector3.zero)
            {
                XYJLogger.LogError("移动目标点和开始点相同");
                return Vector3.zero;
            }
            return tem.normalized;
        }

        public static Vector3 Angle2Vector(float angle)
        {
            Vector3 v;
            float radian = angle * Mathf.Deg2Rad;
            v.x = Mathf.Sin(radian);
            v.y = 0;
            v.z = Mathf.Cos(radian);
            return v;
        }

        //判断pos是否在矩形内
        public static bool PosInRect(float w, float h, Vector3 posoff, float rectAngle)
        {
            Vector3 pos = RotateAngle(posoff, -rectAngle);
            return pos.x > -w && pos.z > 0 && pos.x < w && pos.z < h;
        }


        //取消y轴的正朝向
        public static Vector3 GetForward(Vector3 toPos, Vector3 fromPos)
        {
            Vector3 v = toPos - fromPos;
            v.y = 0;
            return v;
        }

        // 获取自己与目标之间的角度
        public static float GetAngle(Vector3 forward, Vector3 myPos, Vector3 targetPos)
        {
            forward.y = 0;
            float angle = Vector3.Angle(forward, GetForward(targetPos, myPos));
            return angle;
        }


        //跟正朝向的夹角
        public static float GetForwardAngle(float sourceAngle, Vector3 way)
        {
            float angle = Vector2Angle(way);
            return Math.Abs(sourceAngle - angle);
        }
        //是否正面
        public static bool IsForward(float sourceAngle, Vector3 way)
        {
            return GetForwardAngle(sourceAngle, way) < 90;
        }

        public static void SetLookAt(IObject obj, Vector3 toPos)
        {
            if (obj.position.x != toPos.x || obj.position.z != toPos.z)
                obj.SetRotate(Vector2Angle(toPos, obj.position));
        }

        [Conditional("UNITY_EDITOR")]
        public static void CheckFloatLegal(float value)
        {
            if (float.IsNaN(value))
                XYJLogger.LogError("数值出错");
        }

        #endregion
#if !COM_SERVER
        //游戏开始时间，不能改为关卡时间
        public static float timePass { get { return Time.time; } }
        //获取可视对象
        public static Dictionary<int, IObject> GetAOIObj(IObject obj)
        {
            return App.my.sceneMgr.GetObjs();
        }
        public static Dictionary<int, IObject> GetAOIObjByPos(IObject obj, Vector3 pos)
        {
            return App.my.sceneMgr.GetObjs();
        }
        public static int Rand(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }
#else

        //游戏开始时间，不能改为关卡时间
        public static float timePass { get { return GameServer.Time.realtimeSinceStartup; } }
        //获取可视对象
        public static Dictionary<int, IObject> GetAOIObj(IObject obj)
        {
            return obj.zone.GetObjs();
        }
        public static Dictionary<int, IObject> GetAOIObjByPos(IObject obj,Vector3 pos)
        {
            return obj.zone.GetObjs();
        }
        public static int Rand(int min, int max)
        {
            return GameApp.my.random.Next(min, max);
        }
        //是否本地驱动
        public static bool IsRunBattleLogic()
        {
            return true;
        } 
#endif

    }


    static class ClassExtend
    {
        public static void ToPoint3(this Vector3 frompos, Point3 toPos)
        {
            toPos.x = (int)(frompos.x * 1000);
            toPos.y = (int)(frompos.y * 1000);
            toPos.z = (int)(frompos.z * 1000);
        }

        public static Vector3 ToVector3(this Point3 toPos)
        {
            Vector3 p;
            p.x = toPos.x / 1000.0f;
            p.y = toPos.y / 1000.0f;
            p.z = toPos.z / 1000.0f;
            return p;
        }
    }

    public class LineMoveHelp
    {
        //移动方向
        Vector3 m_moveWay;
        //移动距离
        public float m_moveLenght { get; private set; }
        //移动速度
        public float m_moveSpeed { get; private set; }

        public void BeginMove(Vector3 fromPos, Vector3 toPos, float moveLenght, float moveSpeed)
        {
            //距离很多就不用移动
            if (moveLenght <= 0.01f)
            {
                m_moveLenght = 0;
                return;
            }

            if (moveSpeed > 100000)
            {
                XYJLogger.LogError("速度太大，出错");
                moveSpeed = 100000;
            }

            BattleHelp.CheckFloatLegal(moveLenght);
            BattleHelp.CheckFloatLegal(moveSpeed);


            m_moveWay = BattleHelp.Normalize(fromPos, toPos);
            m_moveLenght = moveLenght;
            m_moveSpeed = moveSpeed;
            //运动时其实只计算平面距离，不过需要同步y高度
            if (m_moveSpeed != 0)
                m_moveWay.y = moveLenght == 0 ? 0 : (toPos.y - fromPos.y) / moveLenght;

        }

        public void BeginMoveByTime(Vector3 fromPos, Vector3 toPos, float time)
        {
            if (time <= 0)
            {
                XYJLogger.LogError("移动时间小于0");
                time = 0.01f;
            }
            float moveLenght = BattleHelp.GetDistance(fromPos, toPos);
            float moveSpeed = moveLenght / time;
            BeginMove(fromPos, toPos, moveLenght, moveSpeed);
        }

        //继续移动
        public void MoveContinue(float lenght)
        {
            //速度减慢
            m_moveLenght = lenght;
        }

        public void BeginMoveBySpeed(Vector3 fromPos, Vector3 toPos, float speed)
        {
            float moveLenght = BattleHelp.GetDistance(fromPos, toPos);
            BeginMove(fromPos, toPos, moveLenght, speed);
        }

        public Vector3 UpdateMove(float deltaTime)
        {
            if (m_moveLenght <= 0)
                return Vector3.zero;
            float movelen = m_moveSpeed * deltaTime;
            if (m_moveLenght < movelen)
                movelen = m_moveLenght;
            m_moveLenght -= movelen;
            return m_moveWay * movelen;
        }

        public void SetMoveSpeed(float speed)
        {
            m_moveSpeed = speed;
        }

        public bool IsFinish()
        {
            return m_moveLenght <= 0;
        }
    }

    public class MoveToTargetHelp
    {
        IObject m_target;
        float m_finishDistance;
        bool m_isFinish = true;
        float m_speed;

        public void PlayMove(IObject target, float speed, float finishDistance)
        {
            m_speed = speed;
            m_finishDistance = finishDistance;
            m_target = target;
            m_isFinish = false;
        }

        public void Stop()
        {
            m_isFinish = true;
            m_target = null;
        }


        public bool IsFinish()
        {
            return m_isFinish;
        }
        //返回true表示结束了
        public Vector3 UpdateMove(Vector3 curpos, float delta)
        {
            if (m_isFinish)
                return Vector3.zero;

            //找不到目标自动结束
            if (m_target == null || !m_target.isAlive)
            {
                Stop();
                return Vector3.zero;
            }

            Vector3 toPos = m_target.position;
            toPos.y = curpos.y;
            float distance = BattleHelp.GetDistance(toPos, curpos) - m_finishDistance;
            float moveLen = delta * m_speed;

            //移动结束后是直接施放技能的，所以不同步坐标了，不然服务器和客户端坐标位置差距大
            if (distance <= 0)
            {
                Stop();
                return Vector3.zero;
            }

            //移动距离超过了
            if (distance < moveLen)
            {
                //移动增加一点点距离，避免误差导致移动不到
                moveLen = distance + 0.001f;
                //避免移动距离太小而一直到达不了
                if (moveLen < 0.01f)
                    moveLen = 0.01f;
                Stop();
            }

            Vector3 moveDir = (toPos - curpos).normalized * moveLen;
            return moveDir;
        }
    }


    public class CheckInterval
    {
        float m_time;

        //每隔interval返回true,会自动重置时间
        public bool Check(float interval)
        {
            if (BattleHelp.timePass - m_time > interval)
            {
                m_time = BattleHelp.timePass;
                return true;
            }
            else
                return false;
        }

        //重置时间
        public void ResetCheck()
        {
            m_time = BattleHelp.timePass;
        }

    }
}


//避免前端编译报错
namespace GameServer
{
}
namespace GameServer.Battle
{
}