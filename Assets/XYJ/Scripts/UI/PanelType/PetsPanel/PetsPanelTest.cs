using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using xys;
using xys.UI;

public class PetsPanelTest : MonoBehaviour
{
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(400, 50, 200, 200));
        GUILayout.BeginVertical();

        if (GUILayout.Button("采集读条测试"))
        {
            //UICommon.ShowItemTips(1601);
            //UICommon.ShowTipsWithPromptKey("测试", "test", "左", "右");
            //Debuger.LogError(ReturnBef(123456789));
            //UICommon.ShowCalculator(Vector3.zero, 74,1,1,this.FinishEvent);
            App.my.uiSystem.ShowPanelRoot(false);
        }

        if (GUILayout.Button("打断测试"))
        {
            //ProgressMgr.Release();
            App.my.uiSystem.ShowPanelRoot(true);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    void FinishEvent(int value)
    {
        Debuger.Log(" ----------------- FinishEvent : " + value);
    }
    void BreakEvent()
    {
        Debuger.Log(" ----------------- BreakEvent");
    }

    public string ReturnBef(double value)
    {
        string valueStr = value.ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        return valueStr.Replace("$", string.Empty);
    }
}

