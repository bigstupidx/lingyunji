using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class DropedItem
    {
        public class RandomCount
        {
            public int min;
            public int max;

            public static RandomCount InitConfig(string text)
            {
                if (string.IsNullOrEmpty(text))
                    return null;

                string[] src = text.Split('|');
                RandomCount item = new RandomCount();
                item.min = int.Parse(src[0]);
                item.max = int.Parse(src[1]);
                return item;
            }
        }

        public static List<DropedItem> GetByGroupIdAndLevel(int group, int level)
        {
            var items = GetGroupBygroupId(group);
            if (items == null || items.Count == 0)
                return null;

            List<DropedItem> copys = new List<DropedItem>(items.Count);
            for (int i = 0; i < items.Count; ++i)
            {
                DropedItem item = items[i];
                if (level >= item.minLevel && level <= item.maxLevel)
                    copys.Add(item);
            }

            return copys;
        }
    }
}


