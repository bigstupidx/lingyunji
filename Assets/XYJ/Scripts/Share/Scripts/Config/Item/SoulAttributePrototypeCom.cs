using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Config
{
     public partial class SoulAttributePrototype
    {
        public Dictionary<int, SoulAttr> attrList = new Dictionary<int, SoulAttr>();
        static void OnLoadEnd()
        {
            CsvLoadAdapter.AddCallAfterAllLoad(SoulAttributePrototype.ManageAfterAllFinish);
        }
        static void ManageAfterAllFinish()
        {
            var idItr = SoulAttributePrototype.GetAll().Keys.GetEnumerator();
            while (idItr.MoveNext())
            {
                SoulAttributePrototype cfg = Config.SoulAttributePrototype.Get(idItr.Current);
                if (cfg!=null)
                {
                    foreach (System.Reflection.PropertyInfo pInfo in cfg.GetType().GetProperties(System.Reflection.BindingFlags.Public))
                    {
                        object val = pInfo.GetValue(cfg, null);
                        if (val != null && val is SoulAttr)
                            cfg.attrList.Add(idItr.Current, (SoulAttr)val);
                    }
                }
            }
        }
    }
}
