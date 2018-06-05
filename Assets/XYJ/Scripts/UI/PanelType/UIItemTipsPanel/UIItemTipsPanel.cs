namespace xys.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    public class InitItemTipsData
    {
        public enum Type
        {
            Package, // 包裹界面
            EquipTrain, // 装备培养
            Shopping, // 商城界面
            Welfare, // 福利界面
            Develop, // 养成界面
            GangDepot, // 帮会仓库
            CommonTips,//通用tips
            CommonEquipTips,
            EquipPrototypeTips,
            Mail, // 邮件
            Compound, // 合成
        }

        public Type type; // 显示的类型
        public NetProto.ItemGrid itemData; // 物品数据
        public int index; // 格子下标
        public Config.BagType m_BagType; // 背包类型
    }

#if UNITY_EDITOR
    [SinglePanelType("xys.hot.UI.ItemTipsPanel")]
#endif
    public class UIItemTipsPanel : UIHotPanel
    {
        int m_EventHanderId = -1;
        protected override void OnShow(object args)
        {
            base.OnShow(args);
            m_EventHanderId = EventHandler.pointerClickHandler.Add(CloseTips);
        }

        protected override void OnHide()
        {
            base.OnHide();
            EventHandler.pointerClickHandler.Remove(m_EventHanderId);
        }

        bool CloseTips(GameObject obj, BaseEventData bed)
        {
            if (obj == null || !obj.transform.IsChildOf(this.transform))
            {
                App.my.uiSystem.HidePanel(PanelType.UIItemTipsPanel, false);
                App.my.eventSet.fireEvent(EventID.Package_TipsClose);
                return false;
            }
            return true;
        }

    }
}