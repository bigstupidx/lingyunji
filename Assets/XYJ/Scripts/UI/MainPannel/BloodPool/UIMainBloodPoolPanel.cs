#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using System;
using xys.UI;

namespace xys.hot.UI
{
    [Serializable]
    public class UIMainBloodPoolPanel
    {
        [SerializeField]
        Button m_AddBloodBtn;

        Coroutine disableCoroutine;

        xys.UI.UIHotPanel m_Parent;

        public void OnInit(xys.UI.UIHotPanel parent)
        {
            m_Parent = parent;

            SetBloodBottle(App.my.localPlayer.bloodBottleValue);
            m_AddBloodBtn.onClick.AddListener(AddBloodOnClick);
            m_AddBloodBtn.GetComponent<xys.UI.ButtonPress>().onPress.AddListener(() =>
            {
                App.my.uiSystem.ShowPanel(PanelType.UIBloodTipsPanel, null, true);
            });
        }

        public void OnShow()
        {
            App.my.eventSet.Subscribe<int>(EventID.BloodPoolWearHp, SetBloodBottle);
            App.my.eventSet.Subscribe(EventID.Package_UpdatePackage, UpdateItemCount);
        }

        public void OnHide()
        {
        }

        void UpdateItemCount()
        {
            int itemId = App.my.localPlayer.bloodBottleValue;
            Config.ItemBase config = Config.ItemBaseAll.Get(itemId);
            if (config == null)
                return;

            Text num = m_AddBloodBtn.transform.Find("num").GetComponent<Text>();
            int count = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(itemId);
            if (count > 0)
                num.text = count.ToString();
            else
                num.text = GlobalSymbol.ToUT(string.Format("#[R]{0}#n", count));
        }

        // 设置血瓶
        void SetBloodBottle(int itemId)
        {
            Image icon = m_AddBloodBtn.transform.Find("icon").GetComponent<Image>();
            Text num = m_AddBloodBtn.transform.Find("num").GetComponent<Text>();
            Image cdImage = m_AddBloodBtn.transform.Find("cd").GetComponent<Image>();

            Config.ItemBase config = Config.ItemBaseAll.Get(itemId);
            if (config != null)
            {
                int count = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(itemId);
                xys.UI.Helper.SetSprite(icon, config.icon);

                if (count > 0)
                    num.text = count.ToString();
                else
                    num.text = GlobalSymbol.ToUT(string.Format("#[R]{0}#n", count));

                if (count > 0)
                    cdImage.gameObject.SetActive(false);
                else
                    cdImage.gameObject.SetActive(true);

                if (disableCoroutine == null)
                    disableCoroutine = m_Parent.StartCoroutine(TimeLimitItem(itemId));
            }
            else
            {
                Config.kvClient clientCfg = Config.kvClient.Get("DefaultDrugIcon");
                cdImage.gameObject.SetActive(true);
                xys.UI.Helper.SetSprite(icon, clientCfg.value);
                num.text = GlobalSymbol.ToUT(string.Format("#[R]{0}#n", 0));

                cdImage.fillAmount = 1;
                if (disableCoroutine != null)
                {
                    m_Parent.StopCoroutine(disableCoroutine);
                    disableCoroutine = null;
                }
            }
        }

        void AddBloodOnClick()
        {
            // 血瓶是否已经装备
            Config.ItemBase itemcConfig = Config.ItemBaseAll.Get(App.my.localPlayer.bloodBottleValue);
            if (itemcConfig == null)
            {
                // 如果未装备就打开tips界面
                App.my.uiSystem.ShowPanel(PanelType.UIBloodTipsPanel, null, true);
                return;
            }

            // 是否冷却中
            if (!App.my.localPlayer.cdMgr.isEnd(CDType.Item, (short)itemcConfig.sonType))
            {
                Config.TipsContent config = Config.TipsContent.Get(3104);
                if (config == null)
                    return;

                xys.UI.SystemHintMgr.ShowHint(config.des);
                return;
            }

            // 是否满血
            if (App.my.localPlayer.hpValue == App.my.localPlayer.maxHpValue)
            {
                Config.TipsContent config = Config.TipsContent.Get(3100);
                if (config == null)
                    return;

                xys.UI.SystemHintMgr.ShowHint(config.des);
                return;
            }

            PackageMgr mgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            if (mgr == null)
                return;

            if (!mgr.UseItemById(App.my.localPlayer.bloodBottleValue, 1))
                return;

            if (disableCoroutine != null)
            {
                m_Parent.StopCoroutine(disableCoroutine);
                disableCoroutine = null;
            }

            disableCoroutine = m_Parent.StartCoroutine(TimeLimitItem(App.my.localPlayer.bloodBottleValue));

            Config.Item cfg = Config.Item.Get(itemcConfig.id);
            Config.TipsContent tipsConfig = Config.TipsContent.Get(3105);
            if (tipsConfig == null)
                return;

            xys.UI.SystemHintMgr.ShowHint(string.Format(tipsConfig.des, cfg.addHp));
        }

        System.Collections.IEnumerator TimeLimitItem(int itemId)
        {
            while (true)
            {
                yield return 10;
                SetCdImage(itemId);
            }
        }

        // 设置遮罩cd
        public void SetCdImage(int itemId)
        {
            Image cdImage = m_AddBloodBtn.transform.Find("cd").GetComponent<Image>();

            if (itemId == 0)
            {
                m_Parent.StopCoroutine(disableCoroutine);
                disableCoroutine = null;
                return;
            }

            Config.ItemBase config = Config.ItemBaseAll.Get(itemId);
            if (config == null || config.type == Config.ItemType.equip)
            {
                cdImage.gameObject.SetActive(false);
                return;
            }

            Config.Item itemConfig = Config.Item.Get(itemId);
            if (itemConfig == null || itemConfig.cooling == 0)
            {
                cdImage.gameObject.SetActive(false);
                return;
            }

            if (App.my.localPlayer.cdMgr.isEnd(CDType.Item, (short)config.sonType))
            {
                cdImage.gameObject.SetActive(false);
                return;
            }

            CDEventData data = App.my.localPlayer.cdMgr.GetData(CDType.Item, (short)config.sonType);
            float cdTime = (data.total - data.elapse) / data.total;
            cdImage.fillAmount = cdTime;
            cdImage.gameObject.SetActive(true);

            if (App.my.localPlayer.cdMgr.isEnd(CDType.Item, (short)config.sonType))
            {
                cdImage.gameObject.SetActive(false);
                m_Parent.StopCoroutine(disableCoroutine);
                disableCoroutine = null;
                return;
            }
        }

    }
}
#endif
