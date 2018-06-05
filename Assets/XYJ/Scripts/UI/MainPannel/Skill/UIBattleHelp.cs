using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using xys.UI;
using xys.battle;
using Config;
using WXB;

namespace xys.UI
{
    public static class UIBattleHelp
    {
        //代码设置点击事件
        static public void AddBattleEvent(Transform root, string path,UIBattleFunType type)
        {
            Transform trans = root.Find(path);
            if (trans == null)
            {
                Debug.LogError("战斗功能对象找不到 "+path);
                return;
            }

            EventMono p = trans.AddComponentIfNoExist<EventMono>();
            p.type = EventMono.ParamType.Int;
            p.intParam = (int)type;
            p.eventName = EventID.MainPanel_BattleFuntion.ToString();
        }
    }
}