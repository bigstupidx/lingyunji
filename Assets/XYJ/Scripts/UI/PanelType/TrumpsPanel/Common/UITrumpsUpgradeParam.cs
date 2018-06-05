#if !USE_HOT
namespace xys.hot.UI
{
    using Config;
    using NetProto;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using xys.UI;
    using xys.UI.State;

    partial class UITrumpsUpgrade
    {
        //tips
        [SerializeField]
        GameObject m_UpgradeTips;
        [SerializeField]
        Text m_SoulLv;
        [SerializeField]
        Text m_SoulLevelMax;
        [SerializeField]
        Text m_Exp;
        [SerializeField]
        Image m_ExpBar;
        [SerializeField]
        float m_MaxLen = 494.0f;
        [SerializeField]
        Transform m_ItemGrid;
        [SerializeField]
        Button m_CloseBtn;
        [SerializeField]
        Button m_UpgradeBtn;

        List<Item> m_SoulItems;

        int m_SelectedIndex;

        void OnInitTips()
        {
            m_SoulItems = new List<Item>();
            foreach (Item item in Item.GetAll().Values)
            {
                if (item.sonType == (int)ItemChildType.treasureTrainMaterial)
                    m_SoulItems.Add(item);
                else if (item.sonType == (int)ItemChildType.treasureItem && item.actWeapon == m_TrumpId)
                    m_SoulItems.Add(item);
            }
            //m_SoulItems.Sort();
            for (int i = 0; i < m_ItemGrid.childCount; i++)
            {
                Transform root = m_ItemGrid.GetChild(i);
                if (i >= m_SoulItems.Count)
                {
                    root.gameObject.SetActive(false);
                    continue;
                }
                root.gameObject.SetActive(true);
                Helper.SetSprite(root.Find("Group/Icon").GetComponent<Image>(), m_SoulItems[i].icon);
                Helper.SetSprite(root.Find("Group/Quality").GetComponent<Image>(), QualitySourceConfig.Get(m_SoulItems[i].quality).icon);

                root.Find("Name").GetComponent<Text>().text = m_SoulItems[i].name;
                root.Find("Value").GetComponent<Text>().text = m_SoulItems[i].weaponExp.ToString();

                int count = App.my.localPlayer.GetModule<PackageModule>().GetItemCount(m_SoulItems[i].id);
                root.Find("Group/Num").GetComponent<Text>().text = count == 0 ? GlobalSymbol.ToUT(string.Format("#[R1]{0}#n", count)).ToString() : count.ToString();

                int index = i;
                root.Find("Group").GetComponent<Button>().onClick.AddListener(() => { this.ShowItemTips(m_SoulItems[index].id,index); });
                root.GetComponent<Button>().onClick.AddListener(() => { this.OnSelectedItem(index); });
            }
//             m_UpgradeBtn.onClick.AddListener(this.AddExpEvent);
//             m_CloseBtn.onClick.AddListener(() => { this.ShowTips(false); });
        }

        void ShowSoulTipsEvent()
        {
            this.ResetTips();
            this.OnSelectedItem(-1);
        }

        void ResetTips()
        {
            TrumpAttribute attribute = m_TrumpMgr.GetTrumpAttribute(m_TrumpId);
            if (!TrumpCultivateExp.GetAll().ContainsKey(attribute.souldata.lv))
                return;
            TrumpCultivateExp expData = TrumpCultivateExp.Get(attribute.souldata.lv);
            this.ShowTips(true);
            m_SoulLv.text = attribute.souldata.lv.ToString();
            m_SoulLevelMax.text = m_TrumpMgr.GetMaxSoulLv(attribute.id).ToString();
            m_Exp.text = attribute.souldata.exp + "/" + expData.exp;
            m_ExpBar.rectTransform.sizeDelta = new Vector2((float)attribute.souldata.exp / (float)expData.exp * m_MaxLen, m_ExpBar.rectTransform.sizeDelta.y);

            this.OnInitTips();
            for (int i = 0; i < m_SoulItems.Count; i++)
            {
                Transform root = m_ItemGrid.GetChild(i);
                int count = App.my.localPlayer.GetModule<PackageModule>().GetItemCount(m_SoulItems[i].id);
                root.Find("Group/Num").GetComponent<Text>().text = count == 0 ? GlobalSymbol.ToUT(string.Format("#[R1]{0}#n", count)).ToString() : count.ToString();
            }
        }

        void OnSelectedItem(int index)
        {
            if(m_SelectedIndex != index)
            {
                if (m_SelectedIndex != -1)
                    m_ItemGrid.GetChild(m_SelectedIndex).GetComponent<StateRoot>().CurrentState = 0;

                m_SelectedIndex = index;

                if (m_SelectedIndex != -1)
                    m_ItemGrid.GetChild(m_SelectedIndex).GetComponent<StateRoot>().CurrentState = 1;
            }
        }

        void ShowItemTips(int itemId,int index)
        {
            if (m_SelectedIndex != index)
                this.OnSelectedItem(index);
            else
                UICommon.ShowItemTips(itemId);
        }

        void AddExpEvent()
        {
            if (m_SelectedIndex == -1)
            {
                SystemHintMgr.ShowHint(TipsContent.GetByName("Trump_Soul_Select").des);
                return;
            }
            TrumpAttribute attribute = m_TrumpMgr.GetTrumpAttribute(m_TrumpId);
            int lv = attribute.souldata.lv;
            int exp = attribute.souldata.exp;
            //能否升级
            if(!m_TrumpMgr.CanLvUp(attribute.id,lv,exp))
            {
                SystemHintMgr.ShowHint(TipsContent.GetByName("Trump_Soul_LvLimit").des);
                return;
            }
            //是否满级
            if(lv >= TrumpCultivateExp.Get(TrumpCultivateExp.GetAll().Count - 1).id && exp>= TrumpCultivateExp.Get(TrumpCultivateExp.GetAll().Count - 1).exp)
            {
                SystemHintMgr.ShowHint(TipsContent.GetByName("Trump_Soul_LvMax").des);
                return;
            }
            //
            TrumpsExpRequest request = new TrumpsExpRequest();
            request.trumpid = m_TrumpId;
            request.itemid = m_SoulItems[m_SelectedIndex].id;
            m_Page.Event.FireEvent(EventID.Trumps_AddExp, request);
        }

        void ShowTips(bool show)
        {
            m_UpgradeTips.gameObject.SetActive(show);
        }
    }
}
#endif