using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using xys.battle;

namespace Config
{
    public partial class RoleChangeAttr
    {
        static Dictionary<RoleJob.Job, Dictionary<int, BattleAttri>> s_jobChangeAttrimul = new Dictionary<RoleJob.Job, Dictionary<int, BattleAttri>>();

        static void OnLoadEnd()
        {
            s_jobChangeAttrimul.Clear();
            foreach (var p in DataList_key)
            {
                Dictionary<int, BattleAttri> jobMap = new Dictionary<int, BattleAttri>();
                s_jobChangeAttrimul.Add((RoleJob.Job)p.Key, jobMap);

                //收集职业每行属性转化
                foreach (var a in p.Value)
                {
                    int attriIndex = AttributeDefine.GetIndexByName(a.name);
                    jobMap.Add(attriIndex, a.battleAttri);
                }
            }
        }


        //获得职业属性转化表
        public static Dictionary<int, BattleAttri> GetAttriChange(RoleJob.Job job)
        {
            Dictionary<int, BattleAttri> p;
            if (s_jobChangeAttrimul.TryGetValue(job, out p))
                return p;
            else
                return null;
        }

        //属性转换
        public static BattleAttri ChangeAttribute(BattleAttri baseAttri, RoleJob.Job job)
        {
            Dictionary<int, BattleAttri> map = GetAttriChange(job);
            if (map == null)
            {
                XYJLogger.LogError("属性转化出错，找不到对应职业 " + job);
                return new BattleAttri();
            }

            BattleAttri toAttri = new BattleAttri();

            foreach (var p in map)
            {
                //需要转化的属性值
                double value = baseAttri.Get(p.Key);
                if (value == 0)
                    continue;
                BattleAttri changeAttri = p.Value;
                foreach (var index in changeAttri.GetKeys())
                {
                    double toValue = changeAttri.Get(index) * value;
                    toAttri.Add(index, toValue);
                }
            }

            return toAttri;
        }

        //通过属性名字获取对应组转化率数据
        public static bool GetChangeAttrByNameInGroup(int index, string name, out RoleChangeAttr attr)
        {
            attr = null;
            List<RoleChangeAttr> list = RoleChangeAttr.GetGroupBykey(index);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].name == name)
                {
                    attr = list[i];
                    return true;
                }
            }
            return false;
        }

        //获取主属性ID
        public static int GetMainAttriId(int jobId)
        {
            List<RoleChangeAttr> list = RoleChangeAttr.GetGroupBykey(jobId);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].mainAttri == 1)
                    return AttributeDefine.GetIndexByName(list[i].name);
            }
            return 0;
        }

#if !COM_SERVER
        //获取主属性描述
        public static string GetMainAttriDesc(int jobId)
        {
            List<RoleChangeAttr> list = RoleChangeAttr.GetGroupBykey(jobId);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].mainAttri == 1)
                    return GlobalSymbol.ToUT(list[i].mainAttriDesc);
            }
            return "";
        }
#endif
    }
}