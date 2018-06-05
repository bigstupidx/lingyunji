#if MEMORY_CHECK
using PackTool;
using GUIEditor;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public partial class RunningAssetsShow
{
    class PrefabShow : AssetsShow<PrefabBeh.Data>
    {
        enum SortType
        {
            创建次数,
            存活个数,
            加载次数, 
        }

        SortType sortType = SortType.存活个数;
        bool isReverse = true;

        public PrefabShow()
        {
            infoText = (PrefabBeh.Data d) => { return d.Text; };

            beginList = (List<PrefabBeh.Data> items, ParamList pl) =>
            {
                GuiTools.HorizontalField(2,
                    () =>
                    {
                        sortType = (SortType)EditorGUILayout.EnumPopup("排序", sortType);
                        isReverse = EditorGUILayout.Toggle("升序", isReverse);
                    });

                switch (sortType)
                {
                case SortType.创建次数:
                    items.Sort((PrefabBeh.Data x, PrefabBeh.Data y)=>{ return x.create_num.CompareTo(y.create_num); });
                    break;
                case SortType.存活个数:
                    items.Sort((PrefabBeh.Data x, PrefabBeh.Data y) => { return x.lift_num.CompareTo(y.lift_num); });
                    break;
                case SortType.加载次数:
                    items.Sort((PrefabBeh.Data x, PrefabBeh.Data y) => { return x.load_num.CompareTo(y.load_num); });
                    break;
                }

                if (isReverse)
                    items.Reverse();
            };
        }
    }
}
#endif