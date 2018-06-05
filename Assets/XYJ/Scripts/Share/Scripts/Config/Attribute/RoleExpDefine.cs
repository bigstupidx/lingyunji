// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public struct ExpData
    {
        public int lv;
        public int exp;
    }

    public partial class RoleExp
    {
        /// <summary>
        /// 增加经验值接口
        /// </summary>
        /// <param name="job">职业</param>
        /// <param name="lv">当前级数</param>
        /// <param name="exp">当前经验值</param>
        /// <param name="addexp">增加经验值</param>
        /// <returns></returns>
        public static bool CalculateRoleExp(RoleJob.Job job,int lv,uint exp,out ExpData data)
        {
            data = new ExpData();
            int tempexp = (int)exp;
            int templv = lv;
            int nexp = 0;
            while (true)
            {
                if (RoleExp.Get(templv) == null && lv < RoleExp.GetAll().Count)
                    return false;
                RoleExp expData = RoleExp.Get(templv);
                switch(job)
                {
                    case RoleJob.Job.Pet:
                        nexp = expData.pet_exp;
                        break;
                    default:
                        nexp = expData.player_exp;
                        break;
                }
                if (tempexp < nexp)
                    break;
                templv += 1;
                tempexp -= nexp;
            }
            data.exp = tempexp;
            data.lv = templv;
            return true;
        }

        public static bool RecalculateLv(RoleJob.Job job,uint exp ,out ushort lv)
        {
            lv = 1;
            uint tempExp = 0;
            while(true)
            {
                if (RoleExp.Get(lv) == null && lv < RoleExp.GetAll().Count)
                    return false;//未满级的数据为空时
                tempExp += (uint)RoleExp.Get(lv).player_exp;
                if (tempExp > exp)
                    break;
                lv += 1;
            }
            return true;
        }
    }
}
