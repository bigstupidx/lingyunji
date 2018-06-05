using UnityEngine;
using System.Collections;

/// <summary>
/// 根据机器性能来适配
/// </summary>
public class ComSetting
{
    //质量等级
    
    /// <summary>
    /// 通过该标签来适配ios和android
    /// </summary>
    /// <returns></returns>
    static public bool IsIOS()
    {
        int qualityLevel = PlayerPrefs.GetInt("quality",-1);
        //默认
        if (qualityLevel == -1 )
        {
#if  UNITY_IPHONE || UNITY_EDITOR || UNITY_STANDALONE
            return true;
#else
        return false;
#endif
        }
        else if(qualityLevel == 1)
            return true;
        else 
            return false;
    }
}
