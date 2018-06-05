using System;
using XTools;
using UnityEngine;
using System.Collections.Generic;

namespace xys.UI
{
    // UIPanelBase深度范围在0-100之间，
    // UIPanelHint系统提示为101
    // 新手引导提示在102
    public class PanelDepth
#if MEMORY_CHECK
        : MemoryObject
#endif
    {
        // 初始默认的面板深度
        [HideInInspector]
        protected Pair<Canvas, int>[] defaultPanelDepth = null;

        // 当前的深度
        public float currentDepth { get; protected set; }

        public int GetMinDepth()
        {
            return defaultPanelDepth[0].first.sortingOrder;
        }

        public int GetMaxDepth()
        {
            return defaultPanelDepth[defaultPanelDepth.Length - 1].first.sortingOrder;
        }

        public int GetStartMinDepth()
        {
            return defaultPanelDepth[0].second;
        }

        public int GetStartMaxDepth()
        {
            return defaultPanelDepth[defaultPanelDepth.Length - 1].second;
        }

        public void EachPanel(System.Action<Canvas> action)
        {
            foreach (Pair<Canvas, int> p in defaultPanelDepth)
            {
                action(p.first);
            }
        }

        // 增加面板的深度
        public void AddDepth(float depth)
        {
            if (defaultPanelDepth == null)
                return;

            currentDepth = depth;
            depth *= 100;
            foreach (Pair<Canvas, int> itor in defaultPanelDepth)
                itor.first.sortingOrder = (int)(itor.second + depth);
        }

        public void SetPanelDepth(int depth)
        {
            foreach (Pair<Canvas, int> itor in defaultPanelDepth)
                itor.first.sortingOrder = depth + itor.second;
        }

        // 默认深度
        public void DefaultDepth()
        {
            AddDepth(0);
        }

        static public void GetOrResetCanvas(GameObject go, List<Canvas> canvas)
        {
            go.GetComponentsInChildren(true, canvas);
            canvas.Sort((Canvas x, Canvas y) => { return x.sortingOrder.CompareTo(y.sortingOrder); });
        }

        public void InitDefaultDepth(GameObject go)
        {
            currentDepth = 0;
            List<Canvas> canvas = Pool.Const.CanvasList;

            GetOrResetCanvas(go, canvas);

            int count = canvas.Count;
            for (int i = 0; i < count; ++i)
                canvas[i].sortingOrder = i + 1;

            defaultPanelDepth = new Pair<Canvas, int>[count];
            for (int i = 0; i < count; ++i)
                defaultPanelDepth[i] = new Pair<Canvas, int>(canvas[i], canvas[i].sortingOrder);

            canvas.Clear();
        }
    }
}