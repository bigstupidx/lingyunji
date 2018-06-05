using UnityEngine;

namespace xys.UI
{
    [ExecuteInEditMode]
    public class ScreenEvent : MonoBehaviour
    {
        int mWidth;
        int mHeight;

        void Awake()
        {
            GetScreenSize(out mWidth, out mHeight);
        }

        static void GetScreenSize(out int w, out int h)
        {
#if UNITY_EDITOR
            Vector2 size = XTools.Utility.screenSize;
            w = (int)size.x;
            h = (int)size.y;
#else
            w = Screen.width;
            h = Screen.height;
#endif
        }

        void LateUpdate()
        {
            int w;
            int h;
            GetScreenSize(out w, out h);
            if (w != mWidth || h != mHeight)
            {
                mWidth = w;
                mHeight = h;

                BroadcastMessage("OnScreenResize", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}