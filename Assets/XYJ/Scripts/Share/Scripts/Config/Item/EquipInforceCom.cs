// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class EquipInforceTab : EquipInforceBase
    {
        static List<int> beginIndexList = new List<int>();  //开始索引。加速查找
        public static EquipInforceTab Get(int enforceLv, int subType)
        {
            if (DataList.Count > 0)
            {
                for (int i = 0, j = beginIndexList[i]; j < DataList.Count;)
                {
                    if (DataList[j].types.IndexOf(subType.ToString()) != -1)
                    {
                        if (DataList[j].level == enforceLv)
                            return DataList[j];
                    }
                    else
                    {
                        j = beginIndexList[i++];   //jump to next index
                        continue;
                    }
                    j++;
                }
                //var itr = DataList.GetEnumerator();
                //while (itr.MoveNext())
                //{
                //    if ((itr.Current.level == enforceLv) && itr.Current.types.IndexOf(subType.ToString()) != -1)
                //        return itr.Current;
                //}
            }

            return null;
        }
        static void OnLoadEnd()
        {
            CsvLoadAdapter.AddCallAfterAllLoad(EquipInforceTab.ManageAfterAllFinish);
        }
        static void ManageAfterAllFinish()
        {
            if (DataList != null)
            {
                string currentEquipType = DataList[0].types;
                beginIndexList.Add(0);
                var itr = DataList.GetEnumerator();
                int index = 0;
                while (itr.MoveNext())
                {
                    if (itr.Current.types != currentEquipType)
                    {
                        currentEquipType = itr.Current.types;
                        beginIndexList.Add(index);
                    }
                    index++;
                }
            }
        }
    }
}


