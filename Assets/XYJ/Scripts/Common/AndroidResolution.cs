#if UNITY_ANDROID
using UnityEngine;

public class AndroidResolution
{
    //最大分辨率
    const float MUL = 1.0f;
    const float W1 = 1136 * MUL;
    const float H1 = 640 * MUL;

    public static void SetWindows()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            int w2 = Screen.currentResolution.width;
            int h2 = Screen.currentResolution.height;
            XYJLogger.LogDebug("currentResolution width:{0} height:{1}", w2, h2);
            ImplSetWindows(w2, h2);
        }
    }

    //分辨率被限制在W1，H1内，等比放缩
    static void ImplSetWindows(int w2, int h2)
    {
        float w, h;

        //分辨率合适，就不用设置
        if (w2 <= W1 && h2 <= H1)
            return;

        //w比例大le
        if (w2 / W1 < h2 / H1)
        {
            w = W1;
            h = h2 / (w2 / W1);
        }
        else
        {
            w = w2 / (h2 / H1);
            h = H1;
        }

        Screen.SetResolution((int)w, (int)h, true);
        XYJLogger.LogDebug("SetWindows width:{0} height:{1}", w, h);
    }
}
#endif