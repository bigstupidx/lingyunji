#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Geometry2D
{
    public static class Math2D
    {
        public static bool Equals(float x, float y)
        {
            return Mathf.Abs(x - y) < 0.001f ? true : false;
        }
    }
}
#endif