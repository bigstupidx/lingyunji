using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    /// <summary>
    /// 杂项里面的keyvalue对
    /// </summary>
    public partial class kvBattle
    {
        public static float toSkyHeight { get; private set; }   //浮空高度
        public static float lockTargetMaxDistance { get; private set; } //锁定敌人距离

        public static float intoFastrunTime { get; private set; }      //普通跑步进入神行的时间
        public static float fastRunSpeedMultiple { get; private set; }//神行速度是基本速度倍数
        public static float fastRunFov { get; private set; } //神行相机fov
        public static string[] hutiImmuneState { get; private set; } //护体免疫的状态
        //脱离战斗时间
        public static float breakBattleTime { get; private set; }

        //回出生点速度
        public static float MoveToBornPosSpeedMul { get; private set; }

        //霸体击退距离百分比
        public static float batiBeatBackMoveMul { get; private set; }
        //超过一定距离就不用飘字了
        public static float ActiiveNameDistance { get; private set; }
        //bufftick间隔
        public static float buffTickInterval { get; private set; }
        //仇恨值切换百分比
        public static float hatredChangeMul { get; private set; }
        //首次进入视野的敌人仇恨值
        public static int hatredInitValue { get; private set; }

        #region 同步
        //服务器同步间隔
        public static float serverSysPosInterval { get; private set; }
        public static float clientSysPosInterval { get; private set; }
        //客户端和服务器位置差距多大就强拉
        public static float clientSysPosMaxOff { get; private set; }
        //客户端和服务器距离小于这个值就不需要移动
        public static float clientSysPosNormalOff { get; private set; }
        //加速完成
        public static float clientSysPosSpeedUpMul { get; private set; }
        //减速
        public static float clientSysPosSpeedDownMul { get; private set; }
        //距离多少的时候开始减速
        public static float clientSysPosSpeedDownByDistance { get; private set; }
        #endregion

        #region 属性
        //护体非护体状态回复加成
        public static float hutiRecoverSpeedWhenBreak { get; private set; }
        //护体非战斗状态时恢复速度
        public static float hutiRecoverSpeedWhenIdle { get; private set; }
        //护体战斗状态回复速度
        public static float hutiRecoverSpeedWhenBattle { get; private set; }
        //护体进入护体状态百分百
        public static float hutiEnterStatusMul { get; private set; }
        //护体打破时上的虚弱时间
        public static float hutiBreakWeakTime { get; private set; }
        //护体破体时免疫护体伤害时间 
        public static float hutiBreakNoDamageTime { get; private set; }

        //真气上限
        public static int zhenqiMax { get; private set; }
        //真气恢复速度
        public static int zhenqiRecoverSpeed { get; private set; }


        #endregion


        static void OnLoadEnd()
        {

            toSkyHeight = GetFloat("toSkyHeight", 3.0f);
            lockTargetMaxDistance = GetFloat("lockTargetMaxDistance", 30.0f);
            intoFastrunTime = GetFloat("intoFastrunTime", 2.0f);
            fastRunSpeedMultiple = GetFloat("fastRunSpeedMultiple", 2.0f);
            fastRunFov = GetFloat("fastRunFov", 2.0f);
            breakBattleTime = GetFloat("breakBattleTime", 6.0f);
            hutiImmuneState = GetStrs("hutiImmuneState");
            batiBeatBackMoveMul = GetFloat("batiBeatBackMoveMul", 0.5f);
            ActiiveNameDistance = GetFloat("ActiiveNameDistance", 20.0f);
            buffTickInterval = GetFloat("buffTickInterval", 1.0f);
            hatredChangeMul = GetFloat("hatredChangeMul", 0.2f);
            hatredInitValue = GetInt("hatredInitValue", 1);

            MoveToBornPosSpeedMul = GetInt("MoveToBornPosSpeedMul", 3);

            #region 同步
            serverSysPosInterval = GetFloat("serverSysPosInterval", 0.2f);
            clientSysPosInterval = GetFloat("clientSysPosInterval", 0.2f);
            clientSysPosMaxOff = GetFloat("clientSysPosMaxOff", 1);
            clientSysPosNormalOff = GetFloat("clientSysPosNormalOff", 0.5f);

            clientSysPosSpeedUpMul = GetFloat("clientSysPosSpeedUpMul", 1.5f);
            clientSysPosSpeedDownMul = GetFloat("clientSysPosSpeedDownMul", 0.5f);
            clientSysPosSpeedDownByDistance = GetFloat("clientSysPosSpeedDownByDistance", 0.8f);
            #endregion

            #region 数值
            hutiRecoverSpeedWhenBreak = GetFloat("hutiRecoverSpeedWhenBreak", 0);
            hutiRecoverSpeedWhenIdle = GetFloat("hutiRecoverSpeedWhenIdle", 0);
            hutiRecoverSpeedWhenBattle = GetFloat("hutiRecoverSpeedWhenBattle", 0);
            hutiEnterStatusMul = GetFloat("hutiEnterStatusMul", 0);
            hutiBreakWeakTime = GetFloat("hutiBreakWeakTime", 0);
            hutiBreakNoDamageTime = GetFloat("hutiBreakNoDamageTime", 0);

            zhenqiMax = GetInt("zhenqiMax", 10);
            zhenqiRecoverSpeed = GetInt("zhenqiRecoverSpeed", 1);
            #endregion
        }

        static int GetInt(string key, int defaultV)
        {
            kvBattle p = Get(key);
            if (p != null)
                return int.Parse(p.value);
            else
                return defaultV;
        }
        static float GetFloat(string key, float defaultV)
        {
            kvBattle p = Get(key);
            if (p != null)
                return float.Parse(p.value);
            else
                return defaultV;
        }
        static string[] GetStrs(string key)
        {
            kvBattle p = Get(key);
            if (p != null)
                return p.value.Split('|');
            else
                return new string[0];
        }
    }
}
