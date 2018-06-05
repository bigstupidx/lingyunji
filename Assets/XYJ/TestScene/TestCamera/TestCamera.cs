using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace xys.Test
{
    /// <summary>
    /// 测试代码
    /// </summary>
    public class TestCamera : MonoBehaviour
    {
        EventAgent events = new EventAgent();
        void Start()
        {
            events.Subscribe(EventID.FinishAppInit, OnFinishGameInit);
            events.Subscribe(EventID.FinishLoadScene, OnFinishChangeScene);

        }

        void OnDestroy()
        {
            events.Release();
        }


        public IEnumerator LoadScene(string name)
        {
            SceneManager.LoadScene(name);
            yield return 0;
            App.my.eventSet.fireEvent(EventID.FinishLoadScene);

        }

        //游戏初始化结束后加载场景
        void OnFinishGameInit()
        {
            StartCoroutine( LoadScene("Level_bangpai") );
            //打开主面板        
            App.my.uiSystem.HidePanel("UILoginPanel");

            App.my.uiSystem.ShowPanel("UIMainPanel");
        }
        //进入场景后加载角色
        void OnFinishChangeScene()
        {
            //Actor player = new Actor();
            //ObjectManager.instance.AddObject(0, player);
        }

        void Update()
        {
            //ObjectManager.instance.Update();
        }
    }

}
