using UnityEngine;
using System.Runtime.InteropServices;

public class CameraPhotoNative : MonoBehaviour
{
#if !UNITY_EDITOR && UNITY_IPHONE
    [DllImport("__Internal")]
    extern static void CameraPhoto_ShowCamera(string unityObj, string filepath, int width, int height);

    [DllImport("__Internal")]
    extern static void CameraPhoto_ShowPhoto(string unityObj, string filepath, int width, int height);

    [DllImport("__Internal")]
    extern static void CameraPhoto_ShowLibrary(string unityObj, string filepath, int width, int height);
#endif

    static string UnityObject = "";

    static CameraPhotoNative instance_;

    static CameraPhotoNative instance
    {
        get
        {
            if (instance_ == null)
                Init();

            return instance_;
        }
    }

    public delegate void OnEndFun(string error);

    static void Init()
    {
        instance_ = (new GameObject("CameraPhoto")).AddComponent<CameraPhotoNative>();
        UnityObject = instance_.name;
    }

	static void CheckDir(ref string path)
	{
		path = path.Replace ('\\', '/');
		var p = path.Substring (0, path.LastIndexOf ('/'));
		if (!System.IO.Directory.Exists(p))
		{
			System.IO.Directory.CreateDirectory (p);
		}
	}

    // OnEndFun操作的回调error为空则操作成功，不为空则失败
    // 返回值为false则表示操作失败，不会回调
    public static bool ShowCamera(string filepath, int width, int height, OnEndFun end)
    {
		CheckDir (ref filepath);

#if !UNITY_EDITOR && UNITY_ANDROID
        Show("ShowCamera", filepath, width, height, end);
        return true;
#elif !UNITY_EDITOR && UNITY_IPHONE
        instance.onEnd = end;
        CameraPhoto_ShowCamera(UnityObject, filepath, width, height);
        return true;
#else
        return false;
#endif
    }

    // OnEndFun操作的回调error为空则操作成功，不为空则失败
    // 返回值为false则表示操作失败，不会回调
    public static bool ShowPhoto(string filepath, int width, int height, OnEndFun end)
    {
		CheckDir (ref filepath);

#if !UNITY_EDITOR && UNITY_ANDROID
        Show("ShowPhoto", filepath, width, height, end);
        return true;
#elif !UNITY_EDITOR && UNITY_IPHONE
        instance.onEnd = end;
        CameraPhoto_ShowPhoto(UnityObject, filepath, width, height);
        return true;
#else
        return false;
#endif
    }

    // OnEndFun操作的回调error为空则操作成功，不为空则失败
    // 返回值为false则表示操作失败，不会回调
    public static bool ShowPhotoLib(string filepath, int width, int height, OnEndFun end)
    {
		CheckDir (ref filepath);

#if !UNITY_EDITOR && UNITY_ANDROID
        Show("ShowPhoto", filepath, width, height, end);
        return true;
#elif !UNITY_EDITOR && UNITY_IPHONE
        instance.onEnd = end;
        CameraPhoto_ShowLibrary(UnityObject, filepath, width, height);
        return true;
#else
        return false;
#endif
    }

#if !UNITY_EDITOR && UNITY_ANDROID
    // OnEndFun操作的回调error为空则操作成功，不为空则失败
    static void Show(string fun, string filepath, int width, int height, OnEndFun end)
    {
        AndroidJavaClass java = new AndroidJavaClass("com.wxb.Camera.ImageSelect");
        java.CallStatic(fun, UnityObject, filepath, width, height);
        instance.onEnd = end;
    }
#endif

    OnEndFun onEnd;

    void OnEnd(string error)
    {
        if (onEnd != null)
        {
            onEnd(error);
        }
    }
}