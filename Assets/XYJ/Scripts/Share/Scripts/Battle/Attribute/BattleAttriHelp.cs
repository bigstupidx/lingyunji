using Config;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace xys.battle
{
    #region 系统模块接口
    //模块提供给战斗的数据
    public class ModuleBattleData
    {
        //模块提供给战斗的属性
        public BattleAttri attri = new BattleAttri();
        //模块提供给战斗的buff,一般是永久buff
        public List<int> buffids = new List<int>();
        //提供给玩家的技能
        public List<int> skillids = new List<int>();

        public ModuleBattleData()
        {

        }

        public ModuleBattleData(ModuleBattleData data)
        {
            if(data!=null)
            {
                attri.Copy(data.attri);
                if (data.buffids != null && data.buffids.Count > 0)
                    buffids.AddRange(data.buffids);
            }
        }
    }

    /// <summary>
    /// 战斗模块，需要继承该接口
    /// </summary>
    public interface IBattleModule
    {
        //重新计算属性，需要手动调用刷新接口 obj.battle.m_attriLogic.RefreshAttri(this);
        //角色使用
        ModuleBattleData CalculateBattleData();


    }
    #endregion
    #region 配置表相关
    //配置表战斗属性，不能修改
    public class BattleAttriCfg : BattleAttri
    {
        bool m_forbitModify = false;
        public void ForbitModify()
        {
            m_forbitModify = true;
        }
        public override void Set(int index, double value)
        {
            if (m_forbitModify)
            {
                XYJLogger.LogError("配置表战斗属性禁止修改");
                return;
            }
            base.Set(index, value);
        }

        public override void Add(int index, double value)
        {
            if (m_forbitModify)
            {
                XYJLogger.LogError("配置表战斗属性禁止修改");
                return;
            }
            base.Add(index, value);
        }

        public override void Mul(int index, double value)
        {
            if (m_forbitModify)
            {
                XYJLogger.LogError("配置表战斗属性禁止修改");
                return;
            }
            base.Mul(index, value);
        }
    }
    //继承该类的配置表自动解释属性
    public class CsvLineAttri
    {
        #region 配置表解释使用
        static CsvCommon.ICsvLoad s_reader;
        static Dictionary<int, int> s_toIndexs = new Dictionary<int, int>();

        //加载完以后,清除数据
        public static void ClearCache()
        {
            s_reader = null;
            s_toIndexs.Clear();
        }

        //解释一行
        public static BattleAttri GenBattleAttri(CsvCommon.ICsvLoad reader, int y)
        {
            int xcount = reader.GetXCount(y);
            if (s_reader != reader)
            {
                s_reader = reader;
                s_toIndexs.Clear();
                for (int x = 0; x < xcount; x++)
                {
                    int index = AttributeDefine.GetIndexByName(reader.getStr(0, x));
                    if (index >= 0)
                        s_toIndexs[x] = index;
                }
            }

            BattleAttriCfg battleAttri = new BattleAttriCfg();
            foreach (var p in s_toIndexs)
            {
                if (p.Key < xcount)
                {
                    float v = reader.getFloat(y, p.Key, 0);
                    if(v!=0)
                        battleAttri.Set(p.Value, v);
                }
            }
            //禁止配置表修改
            battleAttri.ForbitModify();
            return battleAttri;
        }
        #endregion
    }
    #endregion
}