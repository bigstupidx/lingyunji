namespace xys.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    public class InitItemTipsData
    {
        public enum Type
        {
            Package, // ��������
            EquipTrain, // װ������
            Shopping, // �̳ǽ���
            Welfare, // ��������
            Develop, // ���ɽ���
            GangDepot, // ���ֿ�
            CommonTips,//ͨ��tips
            CommonEquipTips,
            EquipPrototypeTips,
            Mail, // �ʼ�
            Compound, // �ϳ�
        }

        public Type type; // ��ʾ������
        public NetProto.ItemGrid itemData; // ��Ʒ����
        public int index; // �����±�
        public Config.BagType m_BagType; // ��������
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