using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

namespace xys.UI
{
    public class PanelDepthMono : MonoBehaviour
    {
        [SerializeField]
        sbyte depth = 0;

        void Awake()
        {
            List<Canvas> canvas = Pool.Const.CanvasList;
            PanelDepth.GetOrResetCanvas(gameObject, canvas);
            int baseDepth = (depth * 100) + 1;
            for (int i = 0; i < canvas.Count; ++i)
                canvas[i].sortingOrder = i + baseDepth;
            canvas.Clear();

#if !UNITY_EDITOR
            Destroy(this);
#endif
        }
    }
}