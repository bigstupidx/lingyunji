using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using NetProto;

namespace Config
{
    public struct JobMask
    {
        public int value { get; private set; }

        public bool Has(RoleJob.Job job)
        {
            return Has((int)job);
        }

        bool Has(int job)
        {
            return (value & (1 << (int)job)) == 0 ? false : true;
        }

        string info_;

        public string info
        {
            get
            {
                if (info_ != null)
                    return info_;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < 32; ++i)
                {
                    if (!Has(i))
                        continue;

                    RoleJob jobConfig = RoleJob.Get(i);
                    if (jobConfig == null)
                        continue;

                    if (sb.Length == 0)
                        sb.Append(jobConfig.name);
                    else
                        sb.AppendFormat(",{0}", jobConfig.name);
                }

                info_ = sb.ToString();
                return info_;
            }
        }

        public static JobMask InitConfig(string text)
        {
            JobMask mask = new JobMask();
            mask.info_ = null;
            if (string.IsNullOrEmpty(text) || text.Contains("-1"))
            {
                mask.value = -1;
                return mask;
            }

            string[] strs = text.Split(',');
            foreach (string v in strs)
            {
                try
                {
                    mask.value |= (1 << int.Parse(v));
                }
                catch (System.Exception e)
                {
                    mask.value = 0;
                    Log.Error(e.ToString());
                    break;
                }
            }
            return mask;
        }
    }

    public partial class RoleJob
    {
        //怪物，宠物使用职业主要是属性计算用
        public enum Job
        {
            All = -1,       //部分装备所有职业都能使用，配置为-1
            TianJian = 1,
            Mojia = 2,
            GuiGu = 3,
            Qiyao = 4,
            TuoBa = 5,
            FeiYu = 6,

            //主要是用来做属性计算用
            Monster = 100,
            Pet = 101,
        }

        public static int GetRoleID(int career, int sex)
        {
            foreach (var p in DataList)
            {
                if (p.Key == career)
                {
                    if (sex == 1)
                        return p.Value.maleId;
                    else
                        return p.Value.felmaleId;
                }
            }

            XYJLogger.LogError(string.Format("找不到对应职业 carrer={0} sex={1}", career, sex));
            return -1;
        }

        public static Job GetJobByRoleid(int roleid,ObjectType ot )
        {
            if (ot == ObjectType.Pet)
                return RoleJob.Job.Pet;
            else if (ot == ObjectType.Npc)
                return RoleJob.Job.Monster;
            else if (ot == ObjectType.Player)
            {
                foreach (var p in DataList)
                {
                    if (p.Value.maleId == roleid || p.Value.felmaleId == roleid)
                        return (Job)p.Key;
                }

                XYJLogger.LogError(string.Format("找不到对应职业 roleid={0}", roleid));
                return Job.Monster;
            }
            return Job.Monster;
        }
    }
}
