using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Config
{
    public partial class EffectLibrary
    {
        public Dictionary<int, EffectTable> effectTables = new Dictionary<int, EffectTable>();
        static void OnLoadEnd()
        {
            CsvLoadAdapter.AddCallAfterAllLoad(EffectLibrary.ManageAfterAllFinish);
        }

        static void ManageAfterAllFinish()
        {
            List<int> ids = EffectLibrary.GetAll().Keys.ToList();
            foreach (int id in ids)
            {
                EffectLibrary effectLib = EffectLibrary.Get(id);
                if (null == effectLib)
                    continue;

                foreach (System.Reflection.PropertyInfo pInfo in effectLib.GetType().GetProperties())
                {
                    object val = pInfo.GetValue(effectLib, null);
                    if (null != val && val is Config.EffectTable)
                    {
                        Config.EffectNameTable nameTable = Config.EffectNameTable.Get(pInfo.Name);
                        if (null == nameTable)
                            continue;

                        Config.EffectTable effectTable = val as Config.EffectTable;
                        effectLib.effectTables[nameTable.id] = effectTable;
                    }
                }
            }
        }
    }
}
