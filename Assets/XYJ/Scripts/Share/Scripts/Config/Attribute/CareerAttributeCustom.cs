using System.Collections;
using System.Collections.Generic;
using xys.battle;

namespace Config
{
    public partial class CareerAttribute
    {
        static void OnLoadEnd()
        {
            CsvLoadAdapter.AddCallAfterAllLoad(OnLoadAllCsv);
        }

        static void OnLoadAllCsv()
        {
            foreach(var carrer in DataList_key.Values)
            {
                for(int i=1;i<kvCommon.levelMax;i++)
                {
                    if(i> kvCommon.levelMax || carrer[i-1].level!=i)
                    {
                        XYJLogger.LogError(string.Format("职业属性不对 职业={0} 等级={1}", carrer[i].key, i));
                        break;
                    }
                }
            }
        }

        public static BattleAttri GetLevelAttribute( RoleJob.Job job, int level )
        {
            List<CareerAttribute> list = CareerAttribute.GetGroupBykey((int)job);

            //测试代码
            if (level == 0)
            {
                level = 1;
                XYJLogger.LogError(string.Format("角色等级小于1"));
            }

            if (list == null || level > list.Count || level <= 0)
            {
                XYJLogger.LogError(string.Format("找不到职业={0} 等级={1} 的属性", job, level));
                return new BattleAttri();
            }
            return list[level - 1].battleAttri;
        }
    }
}
