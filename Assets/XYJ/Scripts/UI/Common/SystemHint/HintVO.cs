using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.UI
{
    public class HintGo
    {
        public GameObject m_Go;
        public float m_Height;

        public HintGo(GameObject go, float height)
        {
            m_Go = go;
            m_Height = height;
        }
    }
}