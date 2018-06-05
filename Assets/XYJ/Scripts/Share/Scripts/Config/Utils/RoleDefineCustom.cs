using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using xys;
using xys.battle;
using NetProto;

namespace Config
{
    public partial class RoleDefine
    {
        public enum LogicType
        {
            Normal,
            Obstruct,   //阻挡
        }

        public bool IsHideName()
        {
            if (hideName == -1)// 如果字段值为-1,则隐藏名字
                return true;

            return false;
        }

        //动作时间
        Dictionary<string, float> aniData = new Dictionary<string, float>();

        //角色初始化
        public void InitByCfg(IObject obj)
        {
            if (isBati == 1)
                obj.battle.m_buffMgr.AddFlag(BuffManager.Flag.Bati);
        }

        //获得动画时长
        public float GetAniTime(string name)
        {
            float time;
            if (aniData.TryGetValue(name, out time))
                return time;
            else
            {
                XYJLogger.LogError("角色动作时间没有配 name={0} roleid={1}", name, id);
                return 0;
            }
        }

        //获得思考间隔
        public float GetThinkInterval()
        {
            if (thinkInterval.Length == 1)
                return thinkInterval[0];
            else if (thinkInterval.Length == 2)
            {
                if (thinkInterval[0] == thinkInterval[1])
                    return thinkInterval[0];
                else
                    return BattleHelp.Rand((int)(thinkInterval[0] * 100), (int)(thinkInterval[1] * 100)) / 100.0f;
            }
            return 1.0f;
        }

        static void OnLoadEnd()
        {
            foreach (var p in DataList)
            {
                foreach (var ani in p.Value.serverAni.Split(';'))
                {
                    string[] para = ani.Split('|');
                    if (para.Length == 2)
                    {
                        if (p.Value.aniData.ContainsKey(para[0]))
                            XYJLogger.LogError("角色动作表重复roleid={0} ani={1}", p.Value.id, para[0]);
                        else
                            p.Value.aniData.Add(para[0], float.Parse(para[1]) / AniConst.AnimationFrameRate);
                    }

                    if (p.Value.battleCamp == BattleCamp.SameAsCfg)
                    {
                        p.Value.battleCamp = BattleCamp.NeutralCamp;
                        XYJLogger.LogError("角色阵营配置出错 roleid={0}", p.Value.id);
                    }

                    if (p.Value.thinkInterval.Length == 2 && p.Value.thinkInterval[0] > p.Value.thinkInterval[1])
                        XYJLogger.LogError("思考时间出错 role={0}", p.Value.id);
                }
            }
            CsvLoadAdapter.AddCallAfterAllLoad(OnLoadAllCsv);
        }

        static void OnLoadAllCsv()
        {

        }

    }
}
