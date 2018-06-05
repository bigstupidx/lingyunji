using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace xys.UI
{
    public partial class ColorUT
    {
        public static Color ToColor(string colorName)
        {
            if (string.IsNullOrEmpty(colorName))
            {
                return Color.white;
            }
            if (colorName.StartsWith("#"))
                colorName = colorName.Replace("#", string.Empty);
            int v;
            if (!int.TryParse(colorName, System.Globalization.NumberStyles.HexNumber, null, out v))
            {
                Debuger.LogError("Error" + colorName);
            }
            return new Color()
            {
                //a = Convert.ToByte((v >> 24) & 255) / 255f,
                a = 1,
                r = Convert.ToByte((v >> 16) & 255) / 255f,
                g = Convert.ToByte((v >> 8) & 255) / 255f,
                b = Convert.ToByte((v >> 0) & 255) / 255f
            };
        }
    }
}
