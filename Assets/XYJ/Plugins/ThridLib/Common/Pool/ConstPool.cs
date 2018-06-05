using UnityEngine;
using System.Collections.Generic;

namespace Pool
{
    public class Const
    {
        static List<Canvas> _CanvasList = new List<Canvas>();

        public static List<Canvas> CanvasList
        {
            get
            {
#if COM_DEBUG
                if (_CanvasList.Count != 0)
                {
                    Debug.LogFormat("_CanvasList.Count != 0");
                    return new List<Canvas>();
                }
#endif
                return _CanvasList;
            }
        }
    }
}