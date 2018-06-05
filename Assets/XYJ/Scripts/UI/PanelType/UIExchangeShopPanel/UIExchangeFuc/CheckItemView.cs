using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckItemView : MonoBehaviour {

    public Action ItemView;
    private float sumTime;

    void Update()
    {
        sumTime += Time.deltaTime;
        if (sumTime > 1)
        {
            ItemView();
            sumTime = 0;
        }
    }
}
