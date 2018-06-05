using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RoleAttriFx
    {

        static public double GetDefenseFx(int job,int lv)
        {
            int index = job;
            if (job != (int)RoleJob.Job.Pet && job != (int)RoleJob.Job.Monster)
                index = 0;

            RoleAttriFx data = RoleAttriFx.Get(index);
            if (data != null)
                return data.DefenseA + data.DefenseB * (lv - 1);
            return 0;
        }

        //等级函数,已经做了修正的
        static public double GetLevelFx(int job,int lv)
        {
            int index = job;
            if (job != (int)RoleJob.Job.Pet && job != (int)RoleJob.Job.Monster)
                index = 0;

            RoleAttriFx data = RoleAttriFx.Get(index);
            if (data != null)
                return 5/(data.LvA * lv * lv + data.LvB * lv + data.LvC)*0.01f;
            return 0;
        }
    }
}