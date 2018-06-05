#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEditor;

namespace GUIEditor
{
    public class ColorQueue
    {
        public ColorQueue()
        {
            colors = new Color[] { GUI.color, Color.yellow };
        }

        public ColorQueue(params Color[] objs)
        {
            colors = objs;
        }

        Color[] colors = null;

        int index = 0;

        public void Next(bool isSet)
        {
            if (isSet)
                GUI.color = Color.green;
            else
                Next();
        }

        public void Next()
        {
            GUI.color = colors[(index++) % colors.Length];
        }

        public void Recover()
        {
            GUI.color = colors[0];
        }
    }
}

#endif