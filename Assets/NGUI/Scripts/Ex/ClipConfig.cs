using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClipConfig
{
    // 裁圆
    public static readonly Vector2[] CIRCLE_CLIP_NODES;
    const int CIRCLE_ACCURACY = 64;

    static ClipConfig()
    {
        List<Vector2> list = new List<Vector2>();
        for (int i = 0; i < CIRCLE_ACCURACY; ++i)
        {
            float rad = Mathf.Deg2Rad*i*360/(float) CIRCLE_ACCURACY;
            Vector2 v = new Vector2(0.5f + Mathf.Sin(rad)*0.5f, 0.5f + Mathf.Cos(rad)*0.5f);
            list.Add(v);
        }
        CIRCLE_CLIP_NODES = list.ToArray();
    }
}
