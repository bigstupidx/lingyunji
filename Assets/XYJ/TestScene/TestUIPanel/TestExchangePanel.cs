using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys;

public class TestExchangePanel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI() {

        if (GUILayout.Button("点击测试", GUILayout.Width(200), GUILayout.Height(100))){

            //App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIExchangeStorePanel, xys.hot.UI.ShopType.ExchangeShopNonIndependence);
        }
    }
}
