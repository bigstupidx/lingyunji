using UnityEngine;

namespace xys
{
    public class Main : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);

#if USER_ALLRESOURCES
            Resources.Load<PackTool.AllResources>("AllResources").Init();
#endif
            App app = new App(this);
            StartCoroutine(app.Init());
        }

        void Update()
        {
            App.my.Update();
        }

        private void LateUpdate()
        {
            App.my.LateUpdate();
        }

        private void OnApplicationQuit()
        {
            App.my.OnApplicationQuit();
        }
    }
}
