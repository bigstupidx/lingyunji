using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RewardDefine 
    {
        public static ItemCount[] InitItems(string t, char c)
        {
            string[] items = t.Split(c);
            List<ItemCount> datas = new List<ItemCount>(items.Length);
            for (int i = 0; i < items.Length; ++i)
            {
                if (string.IsNullOrEmpty(items[i]))
                    continue;

                datas.Add(ItemCount.InitConfig(items[i]));
            }

            return datas.ToArray();
        }

        static void InitConfig(string text, out RoleJob.Job job, out ItemCount[] items)
        {
            try
            {
                int job_pos = text.IndexOf('(');
                string job_str = text.Substring(0, job_pos);
                job = (RoleJob.Job)int.Parse(job_str);

                string t = text.Substring(job_pos + 1, text.Length - job_pos - 2);
                items = InitItems(t, ',');
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public class JobItems
        {
            public Dictionary<RoleJob.Job, ItemCount[]> datas { get; private set; }
            public ItemCount[] items { get; private set; }

            public static JobItems InitConfig(string text)
            {
                if (string.IsNullOrEmpty(text))
                    return null;

                JobItems data = new JobItems();
                if (text.IndexOf('(') != -1)
                {
                    data.datas = new Dictionary<RoleJob.Job, ItemCount[]>();

                    // 有职业
                    string[] t = text.Split(';');
                    for (int i = 0; i < t.Length; ++i)
                    {
                        RoleJob.Job job;
                        ItemCount[] items;
                        RewardDefine.InitConfig(t[i], out job, out items);
                        data.datas.Add(job, items);
                    }
                }
                else
                {
                    data.items = RewardDefine.InitItems(text, ';');
                }

                return data;
            }

            public ItemCount[] Get(RoleJob.Job job)
            {
                if (datas == null)
                    return items;

                ItemCount[] d = null;
                if (datas.TryGetValue(job, out d))
                    return d;

                return null;
            }
        }
    }
}


