#if !USE_HOT
namespace xys.hot.UI
{
    using UIWidgets;
    using UnityEngine;
    using System.Collections.Generic;

    public class HotTComponentBase
    {
        public HotTComponentBase(xys.UI.HotTComponent parent)
        {
            this.parent = parent;
        }

        public xys.UI.HotTComponent parent { get; private set; }

        public int Index { get { return parent.Index; } }
        public Transform transform { get { return parent.transform; } }
    }
}
#endif