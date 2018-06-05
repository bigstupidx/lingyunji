#if !USE_HOT
using Config;
using NetProto;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI.ItemObtainShow;

namespace xys.hot.UI
{
    class HotObtainItemShowMgr
    {
        public HotObtainItemShowMgr() { }

        public HotObtainItemShowMgr(xys.UI.ObtainItemShowMgr parent)
        {
            this.parent = parent;
        }

        public xys.UI.ObtainItemShowMgr parent { get; private set; }

        bool isInit = false;
        static int petExpId = 5;

        [SerializeField]
        GameObject m_WaveTextPerfabPrefab;
        [SerializeField]
        GameObject m_AniItemPrefab;
        [SerializeField]
        GameObject m_UseTipPrefab;
        [SerializeField]
        private float m_MinCutTime = 0.5f;
        [SerializeField]
        private float m_DelayDestroyTime = 3f;

        EmptyGraphic m_ModalGraphic;
        EmptyGraphic modalGraphic
        {
            get
            {
                if (m_ModalGraphic == null)
                    m_ModalGraphic = parent.GetComponent<EmptyGraphic>();

                return m_ModalGraphic;
            }
        }

        RectTransform rectTransform;
        Queue<WaveText> waveTextQue;
        Queue<int> itemAniQue;

        bool isShowText = false;
        bool isShowItem = false;

        public bool isModalGraphicOpen
        {
            get { return modalGraphic.isActiveAndEnabled; }
            set { modalGraphic.enabled = value; }
        }

        public void Init()
        {
            rectTransform = parent.GetComponent<RectTransform>();
            waveTextQue = new Queue<WaveText>();
            itemAniQue = new Queue<int>();

            m_WaveTextPerfabPrefab.SetActive(false);
            m_AniItemPrefab.SetActive(false);
            m_UseTipPrefab.SetActive(false);

            isShowText = false;
            isShowItem = false;
            isModalGraphicOpen = false;
        }

        public static void ShowObtain(List<xys.UI.Obtain> obtains)
        {
            xys.UI.ObtainItemShowMgr.ShowObtain(obtains);
        }

        private void ShowRewards(List<xys.UI.Obtain> obtainList)
        {
            List<xys.UI.ObtainShow> itemShowList = new List<xys.UI.ObtainShow>();
            int panelId = App.my.uiSystem.ContainsBlurPanel() ? 2 : 1;    //当前界面否打开了背景模糊
            for (int i = 0; i < obtainList.Count; i++)
            {
                xys.UI.Obtain data = obtainList[i];
                ItemBase item = ItemBaseAll.Get(data.id);
                if (item == null) continue;

                ItemObtainShowRule itemShow = ItemObtainShowRule.GetRuleByItemId(panelId, data.id);
                if (itemShow == null)
                    itemShow = ItemObtainShowRule.GetRuleByItemType(panelId, (int)item.type);

                if (itemShow != null)
                {
                    xys.UI.ObtainShow obtainShow = new xys.UI.ObtainShow();
                    obtainShow.id = data.id;
                    obtainShow.value = data.count;
                    obtainShow.rule = itemShow;
                    itemShowList.Add(obtainShow);
                }
            }

            ShowRewardItem(itemShowList);
        }

        void ShowRewardItem(List<xys.UI.ObtainShow> itemShowList)
        {
            itemShowList.Sort((x, y) => x.rule.waveWordPriority.CompareTo(y.rule.waveWordPriority));

            for (int i = 0; i < itemShowList.Count; i++)
            {
                xys.UI.ObtainShow show = itemShowList[i];
                ItemBase item = ItemBaseAll.Get(show.id);
                string chatBoxText = "";

                if (show.value <= 0) continue;

                //飘字
                if (show.rule.waveWord == 1)
                {
                    string waveText = item.name + "+" + show.value;
                    chatBoxText = string.Format(show.rule.systemHintDesc, show.value, item.name);

                    waveTextQue.Enqueue(new WaveText(waveText, show.rule.colorState));
                    if (!isShowText)
                        TiggerText();
                }

                //系统提示框
                if (show.rule.systemHint == 1)
                {
                    string t = "";
                    if (show.id == petExpId)
                    {
                        string petName = "宠物";
                        string color = "61e171";

                        PetsModule petModule = App.my.localPlayer.GetModule<PetsModule>();
                        if (petModule != null)
                        {
                            petName = petModule.GetCurrentPetNickName();
                            color = QualitySourceConfig.Get((ItemQuality)PetAttribute.Get(petModule.GetCurrentPetId()).type).color + "FF";
                        }
                        petName = string.Format("#c{1}{0}#n", petName, color);

                        t = string.Format(show.rule.systemHintDesc, petName, show.value);
                    }
                    else
                    {
                        string itemName = item.name;
                        if (item.type != ItemType.money)
                            itemName = string.Format("#c{1}{0}#n", item.name, QualitySourceConfig.Get(item.quality).color + "FF");
                        t = string.Format(show.rule.systemHintDesc, show.value, itemName);
                    }
                    chatBoxText = t;

                    xys.UI.SystemHintMgr.ShowHint(t);
                }

                //动画
                if (show.rule.Ani == 1)
                {
                    itemAniQue.Enqueue(show.id);
                    if (!isShowItem)
                        TiggerItem();
                }

                //聊天框
                ChatUtil.ChatMgr.AddSystemMsg(ChannelType.Channel_System, chatBoxText);

                //道具使用提示
                Item itemUse = Item.Get(show.id);
                if (itemUse != null && itemUse.tipUse == true && itemUse.useLevel <= App.my.localPlayer.levelValue
                                                              && (itemUse.job.value == -1 || itemUse.job.value == (int)App.my.localPlayer.job))
                {
                    ShowUseTip(show);
                }
            }
        }

        void ShowWaveWord(WaveText waveText)
        {
            if (m_WaveTextPerfabPrefab != null)
            {
                GameObject go = GameObject.Instantiate(m_WaveTextPerfabPrefab);
                go.GetComponent<UIAwardTipsText>().OnShow(waveText);
                SetGameObjPos(go);
                Object.Destroy(go, m_DelayDestroyTime);
            }
        }

        void ShowAniItem(int id)
        {
            if (m_AniItemPrefab != null)
            {
                GameObject go = GameObject.Instantiate(m_AniItemPrefab);
                go.GetComponent<UIAwardTipsItem>().OnShow(id);
                SetGameObjPos(go);
                Object.Destroy(go, m_DelayDestroyTime);
            }
        }

        void ShowUseTip(xys.UI.ObtainShow show)
        {
            if (m_UseTipPrefab != null)
            {
                GameObject go = GameObject.Instantiate(m_UseTipPrefab);
                go.GetComponent<UIItemUseTip>().OnShow(show.id, show.value);
                SetGameObjPos(go);
                go.transform.SetAsFirstSibling();
            }
        }

        void SetGameObjPos(GameObject go)
        {
            go.transform.SetParent(rectTransform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.SetActive(true);
        }

        void TiggerText()
        {
            if (isShowText)
                waveTextQue.Dequeue();
            isShowText = true;

            if (waveTextQue.Count > 0)
            {
                ShowWaveWord(waveTextQue.Peek());
                App.my.mainTimer.Register(m_MinCutTime, 1, TiggerText);
            }
            else
                isShowText = false;
        }

        void TiggerItem()
        {
            if (isShowItem)
                itemAniQue.Dequeue();
            isShowItem = true;
            if (itemAniQue.Count > 0)
            {
                ShowAniItem(itemAniQue.Peek());
                App.my.mainTimer.Register(m_MinCutTime, 1, TiggerItem);
            }
            else
                isShowItem = false;
        }
    }
}
#endif