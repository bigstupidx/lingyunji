using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class DeviceModel
{
    static int check_type = -1;

#if UNITY_IOS
    public static void InitNoHdConfig()
    {
        check_type = 0;
        switch(Device.generation)
        {
        case DeviceGeneration.iPadAir1:
        case DeviceGeneration.iPadAir2:
        case DeviceGeneration.iPhone6:
        case DeviceGeneration.iPhone6Plus:
        case DeviceGeneration.iPhone6S:
        case DeviceGeneration.iPhone6SPlus:
        case DeviceGeneration.iPadPro1Gen:
        case DeviceGeneration.iPadMini4Gen:
        case DeviceGeneration.iPhoneSE1Gen:
        case DeviceGeneration.iPadPro10Inch1Gen:
        case DeviceGeneration.iPhone7:
        case DeviceGeneration.iPhone7Plus:
        case DeviceGeneration.iPhoneUnknown:
        case DeviceGeneration.iPadUnknown:
            check_type = 1;
            break;
        }
    }
#endif

    static bool isHdDevice()
    {
#if UNITY_EDITOR
        return true;
#elif UNITY_ANDROID
        if (check_type == -1)
        {
            ulong total_memory = (ulong)SystemInfo.systemMemorySize;
            if (total_memory >= 1024 * 3)
            {
                check_type = 1;
            }
            else
            {
                check_type = 0;
            }
        }

        return check_type == 1 ? true : false;
#else
        return check_type == 1 ? true : false;
#endif
    }

    // 是否高级设备
    static public bool isHight
    {
        get
        {
#if USE_RESOURCESEXPORT
#if UNITY_ANDROID || UNITY_IPHONE
            return isHdDevice();
#else
            return false;
#endif
#else
            return true;
#endif
        }
    }
}