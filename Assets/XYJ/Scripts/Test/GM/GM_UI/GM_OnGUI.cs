using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif


//OnGUI封装类,项目其他地方需要使用OnGUI的都通过该模块提供的接口来显示，发布项目时可以统一取消掉OnGUI
namespace xys.gm
{
    public partial class GM_UI
    {
        public delegate void OnGUICall();
        class GUIInfo
        {
            public GameObject go;
            public OnGUICall fun;
            public bool needRemove = true;
        }
        static List<GUIInfo> s_guiList = new List<GUIInfo>();
        static List<GUIInfo> s_guiListTem = new List<GUIInfo>();

        void OnGUIList()
        {
            s_guiListTem.Clear();
            for (int i = 0; i < s_guiList.Count; i++)
            {
                if (s_guiList[i].go == null && s_guiList[i].needRemove)
                    s_guiListTem.Add(s_guiList[i]);
                else if (s_guiList[i].go == null || s_guiList[i].go.activeSelf)
                    s_guiList[i].fun();
            }
            for (int i = 0; i < s_guiListTem.Count; i++)
                s_guiList.Remove(s_guiListTem[i]);

            OnGUIAllRoleInfo();
        }

        //参数go不为null时，表示当go销毁时，gui回调也会销毁
        [Conditional("COM_DEBUG")]
        static public void AddOnGUI(OnGUICall guifun, GameObject go = null)
        {
            ImplAddOnGUI(guifun, go, false);
        }

        //只有在编辑器才显示
        [Conditional("COM_DEBUG")]
        static public void AddOnGUIEditor(OnGUICall guifun, GameObject go = null)
        {
            ImplAddOnGUI(guifun, go, true);
        }

        static void ImplAddOnGUI(OnGUICall guifun, GameObject go = null, bool isEditor = false)
        {
#if !UNITY_EDITOR
        if(isEditor)
            return;
#endif
            GUIInfo info = new GUIInfo();
            info.go = go;
            if (info.go == null)
                info.needRemove = false;
            info.fun = guifun;
            s_guiList.Add(info);
        }
    }

}
