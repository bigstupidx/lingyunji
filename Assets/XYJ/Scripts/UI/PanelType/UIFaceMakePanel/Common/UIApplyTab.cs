#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;
using Config;
namespace xys.hot.UI
{
    [AutoILMono]
    class UIApplyTab
    {
        [SerializeField]
        Button m_saveBtn;
        [SerializeField]
        Button m_applyBtn;
        [SerializeField]
        Button m_materialIcon;
        [SerializeField]
        Text m_ownNeedText;
        [SerializeField]
        public StateRoot m_saveAsPreset;
        private System.Action m_clickSaveEvent = null;
        private System.Action m_clickApplyEvent = null;

        void Awake()
        {
            if (m_saveBtn != null)
            {
                m_saveBtn.onClick.AddListener(OnClickSaveBtn);
                m_applyBtn.onClick.AddListener(OnClickApplyBtn);
            }
            if(m_materialIcon!=null)
            {
                m_materialIcon.onClick.AddListener(OnClickIcon);
            }
        }

        public void Set(System.Action clickSaveEvent,System.Action clickApplyEvent)
        {
            kvCommon item = kvCommon.Get("FaceChange");
            int id = 0;
            int num = 0;
            ClothItem.StrToTwoInt(item.value, out id, out num);
            int curOwn = App.my.localPlayer.GetModule<PackageModule>().GetItemCount(id);
            if(curOwn<num)
            {
                m_ownNeedText.color = Color.red;
            }
            else
            {
                m_ownNeedText.color = Color.green;
            }
            m_ownNeedText.text = curOwn.ToString() + "/" + num.ToString();

            Item applyItem = Item.Get(id);
            if(applyItem!=null)
            {
                Helper.SetSprite(m_materialIcon.GetComponent<Image>(), applyItem.icon);
            }
            m_clickSaveEvent = clickSaveEvent;
            m_clickApplyEvent = clickApplyEvent;
        }
        void OnClickSaveBtn()
        {
            if(m_clickSaveEvent!=null)
            {
                m_clickSaveEvent();
            }
        }
        void OnClickApplyBtn()
        {
            if(m_clickApplyEvent!=null)
            {
                m_clickApplyEvent();
            }
        }
        void OnClickIcon()
        {
            kvCommon item = kvCommon.Get("FaceChange");
            int id = 0;
            int num = 0;
            ClothItem.StrToTwoInt(item.value, out id, out num);

            ItemTipsPanel.Param param = new ItemTipsPanel.Param();
            param.itemId = id;
            App.my.uiSystem.ShowPanel(PanelType.UIItemTipsPanel, param, true);
        }
    }
}
#endif