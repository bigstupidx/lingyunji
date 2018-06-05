/*----------------------------------------------------------------
// 创建者：
// 创建日期:
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Battle;
using xys;

public class TestPathToPos : MonoBehaviour
{
    public GameObject p1;
    public GameObject p2;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (App.my != null && App.my.localPlayer.battle!=null)
                App.my.localPlayer.battle.State_PathToPos(this.transform.position);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AstarPath.StartPath(p1.transform.position, p2.transform.position, (list) => { });
        }
    }

}
