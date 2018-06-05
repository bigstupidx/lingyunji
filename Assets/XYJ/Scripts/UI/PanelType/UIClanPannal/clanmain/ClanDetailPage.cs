#if !USE_HOT
using Config;
using NetProto;
using NetProto.Hot;
using System;
using System.Collections.Generic;
using System.Text;
using UIWidgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using xys.UI;
using xys.UI.Clan;
using xys.UI.State;

namespace xys.hot.UI
{
    class ClanDetailPage : HotTablePageBase
    {
        [SerializeField]
        Text m_noticeTxt;
        [SerializeField]
        Text m_flagName;
        [SerializeField]
        Text m_clanName;
        [SerializeField]
        Button m_exchangeNameBtn;
        [SerializeField]
        Text m_clanLeaderName;
        [SerializeField]
        Text m_clanLevel;
        [SerializeField]
        Text m_clanID;
        [SerializeField]
        Transform m_clanMsg;
        [SerializeField]
        Button m_clanBuildBtn;
        [SerializeField]
        Button m_clanPostBtn;
        [SerializeField]
        Button m_clanProcessBtn;
        [SerializeField]
        Button m_clanGoBackBtn;
        [SerializeField]
        Button m_changeNoticeBtn;
        [SerializeField]
        Transform m_changeClanName;
        [SerializeField]
        Transform m_changeNotice;

        [SerializeField]
        Transform m_clanPostTransform;
        [SerializeField]
        Transform m_clanBuildTansform;

        [SerializeField]
        Transform m_clanHistoryTansform;

        [SerializeField]
        ClanHistoryView m_historyView;

        //氏族建筑相关
        [SerializeField]
        ClanBuildView m_buildView;

        [SerializeField]
        Text m_buildName;
        [SerializeField]
        Text m_buildLevel;
        [SerializeField]
        Text m_buildDec;
        [SerializeField]
        Text m_curEffect;
        [SerializeField]
        Text m_buildNeedGold;
        [SerializeField]
        Text m_otherNeed;
        [SerializeField]
        Text m_buildNeedTime;
        [SerializeField]
        Text m_buildUseGold;
        [SerializeField]
        Text m_buildOwnGold;
        [SerializeField]
        Button m_buildLvUpBtn;

        private ClanDbData m_curData = null;

        private ClanBuildViewData m_curBuildData = null;

        ClanDetailPage() : base(null) { }
        public ClanDetailPage(HotTablePage page) : base(page)
        {

        }

        protected override void OnInit()
        {
            m_buildView.SelectItemHandler += ListView_SelectItemHandler;
            m_exchangeNameBtn.onClick.AddListenerIfNoExist(() => { OnClickExchangeName(); });
            m_clanBuildBtn.onClick.AddListenerIfNoExist(() => { OnClickClanBuild(); });
            m_clanPostBtn.onClick.AddListenerIfNoExist(() => { OnClickSetClanPost(); });
            m_clanProcessBtn.onClick.AddListenerIfNoExist(() => { OnClickProcess(); });
            m_clanGoBackBtn.onClick.AddListenerIfNoExist(() => { OnClickGoBackClan(); });
            m_changeNoticeBtn.onClick.AddListenerIfNoExist(() => { OnClickChangeNotice(); });
        }

        protected override void OnShow(object args)
        {
            Event.Subscribe<ClanDbData>(EventID.Clan_RecvSelfClan, this.RefreshUI);
            Event.fireEvent(EventID.Clan_GetSelfClan);
        }

        protected override void OnHide()
        {

        }

        public void RefreshUI(ClanDbData data)
        {
            if (data != null)
            {
                m_curData = null;
                m_curData = data;

                m_noticeTxt.text = data.notice;
                m_flagName.text = data.flag;
                m_clanName.text = data.name;


                m_clanLeaderName.text = GetLeaderName(data);
                m_clanLevel.text = string.Format("{0}级", data.clanlevel.ToString());
                m_clanID.text = data.clanid.ToString();

                SetTextColor(data.colorid);

                //详细信息
                m_clanMsg.Find("GuildMemberCount").GetComponent<Text>().text = string.Format("{0}/{1}/{2}", 5, data.membercount, 100);  //成员

                m_clanMsg.Find("GuildCreateName").GetComponent<Text>().text = data.creater; //创始人

                m_clanMsg.Find("GuildLearnCount").GetComponent<Text>().text = GetClanPostCount(data, ClanPost.Clan_Apprentice).ToString();  //学徒数量

                m_clanMsg.Find("ShiliPinFen").GetComponent<Text>().text = data.combatpower.ToString(); //实力评分

                m_clanMsg.Find("HuoYueDu").GetComponent<Text>().text = "0";   //活跃度

                m_clanMsg.Find("GuildCoin").GetComponent<Text>().text = data.gold.ToString();   //氏族资金

                m_clanMsg.Find("GuildWeiHuCoin").GetComponent<Text>().text = "10000";   //维护资金

                m_clanMsg.Find("JiXianLevel").GetComponent<Text>().text = data.build.allBuild[2].level.ToString();      //集贤阁等级

                m_clanMsg.Find("JinKuLevel").GetComponent<Text>().text = data.build.allBuild[3].level.ToString();      //金库等级

                m_clanMsg.Find("JiNengLevel").GetComponent<Text>().text = data.build.allBuild[4].level.ToString();     //技能坊等级

                m_clanMsg.Find("BaiBaoLevel").GetComponent<Text>().text = data.build.allBuild[5].level.ToString();     //百宝阁


                //更新建筑界面
                if (m_clanBuildTansform.gameObject.activeSelf)
                {
                    RefreshBuildUI();
                    if (m_curBuildData != null)
                    {
                        RefreshBuildDetail(m_curBuildData);
                    }
                }

                //更新职务界面
                if (m_clanPostTransform.gameObject.activeSelf)
                {
                    RefreshClanPostUI();
                }
                //更新历程界面
                if (m_clanHistoryTansform.gameObject.activeSelf)
                {
                    RefreshHistory();
                }
            }
        }

        public string GetLeaderName(ClanDbData data)
        {
            if (data == null)
                return null;

            foreach (var item in data.member.membermap)
            {
                if (item.Value.charId == data.leaderid)
                {
                    return item.Value.name;
                }
            }
            return "";
        }

        public int GetClanPostCount(ClanDbData data, ClanPost post)
        {
            int count = 0;

            foreach (var item in data.member.membermap)
            {
                if (item.Value.post == post)
                {
                    count++;
                }
            }
            return count;
        }
        public void OnClickExchangeName(object args = null)
        {
            if (m_changeClanName != null)
            {
                m_changeClanName.gameObject.SetActive(true);
                xys.UI.EventHandler.pointerClickHandler.Add(OnClickHideChangeName);

                m_changeClanName.Find("Button").GetComponent<Button>().onClick.AddListenerIfNoExist(() =>
                {

                    if (m_changeClanName.Find("GuildName/Text").GetComponent<Text>().text != "" && m_changeClanName.Find("GuildName/Text").GetComponent<Text>().text != null)
                    {
                        ClanDbData newData = new ClanDbData();
                        newData.clanid = App.my.localPlayer.clanIdValue;
                        newData.name = m_changeClanName.Find("GuildName/Text").GetComponent<Text>().text;
                        App.my.eventSet.FireEvent<ClanDbData>(EventID.Clan_UpdataInfo, newData);
                        m_changeClanName.gameObject.SetActive(false);
                    }

                });
            }
        }

        //氏族建筑相关
        public void OnClickClanBuild(object args = null)
        {
            if (m_clanBuildTansform != null)
            {
                m_clanBuildTansform.gameObject.SetActive(true);
                xys.UI.EventHandler.pointerClickHandler.Add(OnClickHideBuildUI);

                m_clanBuildTansform.Find("close_button").GetComponent<Button>().onClick.AddListenerIfNoExist(() => { m_clanBuildTansform.gameObject.SetActive(false); });

                m_buildLvUpBtn.onClick.AddListenerIfNoExist(()=> {

                    Event.FireEvent<int>(EventID.Clan_Build_LevelUp, m_curBuildData.id);
                });

                RefreshBuildUI();
            }

        }

        public void RefreshBuildUI()
        {
            if (m_curData != null && m_buildView != null)
            {
                List<ClanBuildViewData> clanItems = new List<ClanBuildViewData>();
                clanItems.Clear();
                if (m_curData != null)
                {
                    int idx = 0;
                    foreach (var item in m_curData.build.allBuild)
                    {
                        ClanBuildViewData clanBuildData = new ClanBuildViewData();
                        clanBuildData.data = item.Value;
                        if (idx == 0)
                        {
                            clanBuildData.isSelected = true;
                            m_curBuildData = clanBuildData;
                            RefreshBuildDetail(clanBuildData);
                        }
                        clanItems.Add(clanBuildData);
                        idx++;
                    }
                    m_buildView.DataSource = new UIWidgets.ObservableList<ClanBuildViewData>(clanItems);
                }

            }
        }
        //刷新右侧的UI
        public void RefreshBuildDetail(ClanBuildViewData buildInfo)
        {

            m_buildName.text = ClanCommon.GetBuildName(buildInfo.id);

            m_buildLevel.text = string.Format("{0}级", buildInfo.data.level);

            m_buildOwnGold.text = m_curData.gold.ToString();

            switch ((ClanBuildType)buildInfo.id)
            {
                case ClanBuildType.Clan_Build_Begin:
                    break;
                case ClanBuildType.Clan_Main:

                    ClanMainBuildConfig config = ClanMainBuildConfig.Get(buildInfo.data.level);

                    m_buildDec.text = config.tips1;

                    m_curEffect.text = string.Format("当前集贤馆等级上限{0}级，金库等级上限{1}级，技坊等级上限{2}级，百宝阁等级上限{3}级", config.jixianMax, config.jinkuMax, config.jifangMax, config.baibaoMax);

                    m_buildNeedGold.text = config.needGold.ToString();

                    m_otherNeed.text = string.Format("需要氏族活跃度达到{0}，建筑等级需求{1}级", config.needAct, config.buildLvNeed);

                    m_buildNeedTime.text = string.Format("{0}",config.useTime);

                    m_buildUseGold.text = config.levelUpNeedUse.ToString();
                
                    break;
                case ClanBuildType.Clan_JiXian:

                    ClanJiXianConfig jixianConfig = ClanJiXianConfig.Get(buildInfo.data.level);

                    m_buildDec.text = jixianConfig.tips1;


                    m_curEffect.text = string.Format("当前氏族总人数{0}", jixianConfig.clanMemerMax);

                    m_buildNeedGold.text = jixianConfig.needGold.ToString();

                    m_otherNeed.text = string.Format("需要氏族建筑达到{0}级", jixianConfig.needClanLv);

                    m_buildNeedTime.text = string.Format("{0}", jixianConfig.useTime);

                    m_buildUseGold.text = jixianConfig.useGold.ToString();

                    break;
                case ClanBuildType.Clan_JinKu:
                    ClanJinKuConfig jinkuConfig = ClanJinKuConfig.Get(buildInfo.data.level);

                    m_buildDec.text = jinkuConfig.tips1;
                  
                    m_curEffect.text = string.Format("氏族存储资金上限{0}", jinkuConfig.goldMax);

                    m_buildNeedGold.text = jinkuConfig.needGold.ToString();

                    m_otherNeed.text = string.Format("需要氏族建筑达到{0}级", jinkuConfig.needClanLv);

                    m_buildNeedTime.text = string.Format("{0}", jinkuConfig.time);

                    m_buildUseGold.text = jinkuConfig.useGold.ToString();
                    break;
                case ClanBuildType.Clan_JiFang:
                    ClanSkillBuildConfig jifangConfig = ClanSkillBuildConfig.Get(buildInfo.data.level);

                    m_buildDec.text = jifangConfig.tips1;

                    m_curEffect.text = string.Format("开启技能个数{0}，开启技能等级上限{1}", jifangConfig.openSkillMax, jifangConfig.openSkillLvMax);

                    m_buildNeedGold.text = jifangConfig.needGold.ToString();

                    m_otherNeed.text = string.Format("需要氏族建筑达到{0}级", jifangConfig.needClanLv);

                    m_buildNeedTime.text = string.Format("{0}", jifangConfig.time);

                    m_buildUseGold.text = jifangConfig.useGold.ToString();
                    break;
                case ClanBuildType.Clan_BaiBao:
                    ClanBaiBaoConfig baibaoConfig = ClanBaiBaoConfig.Get(buildInfo.data.level);

                    m_buildDec.text = baibaoConfig.tips1;

                    m_curEffect.text = string.Format("每周抽奖次数上限{0}，帮会宝物掉率{1}%", baibaoConfig.lotteryTime, baibaoConfig.rate*100);

                    m_buildNeedGold.text = baibaoConfig.needGold.ToString();

                    m_otherNeed.text = string.Format("需要氏族建筑达到{0}级", baibaoConfig.clanLv);

                    m_buildNeedTime.text = string.Format("{0}", baibaoConfig.time);

                    m_buildUseGold.text = baibaoConfig.needUseGold.ToString();
                    break;
                default:
                    break;
            }


        }

        public bool OnClickHideBuildUI(GameObject obj, BaseEventData bed)
        {
            if (m_clanBuildTansform.gameObject.activeSelf)
            {
                if (obj == null || obj == m_clanBuildTansform.Find("Bg2").gameObject || !obj.transform.IsChildOf(m_clanBuildTansform))
                {
                    m_clanBuildTansform.gameObject.SetActive(false);
                    return false;
                }
            }
            return true;
        }

        private void ListView_SelectItemHandler(ClanBuildView listView, ClanBuildViewItem item)
        {
            ClanBuildViewData itemData = item.itemdData;
            if (null == itemData)
                return;

            if (itemData.isSelected)
            {
                m_curBuildData = itemData;
                RefreshBuildDetail(itemData);
            }
        }


        public void OnClickSetClanPost(object args = null)
        {
            if (m_clanPostTransform != null)
            {
                m_clanPostTransform.gameObject.SetActive(true);

                xys.UI.EventHandler.pointerClickHandler.Add(OnClickHidePost);

                m_clanPostTransform.Find("close_button").GetComponent<Button>().onClick.AddListenerIfNoExist(() => { m_clanPostTransform.gameObject.SetActive(false); });

                RefreshClanPostUI();
            }
        }

        public bool OnClickHidePost(GameObject obj, BaseEventData bed)
        {
            if (m_clanPostTransform.gameObject.activeSelf)
            {
                if (obj == null || obj == m_clanPostTransform.Find("Bg2").gameObject || !obj.transform.IsChildOf(m_clanPostTransform))
                {
                    m_clanPostTransform.gameObject.SetActive(false);
                    return false;
                }
            }
            return true;
        }

        public void RefreshClanPostUI()
        {
            if (m_curData != null)
            {

                List<ClanUser> tempUserList = new List<ClanUser>();
                tempUserList.Clear();

                //族长
                tempUserList = ClanCommon.GetMemberName(m_curData, ClanPost.Clan_Leader);
                if (tempUserList.Count == 1)
                {
                    m_clanPostTransform.Find("1/Name").GetComponent<Text>().text = tempUserList[0].name;
                }

                //副族长
                tempUserList.Clear();
                tempUserList = ClanCommon.GetMemberName(m_curData, ClanPost.Clan_Subleader);
                for (int i = 0; i < 2; i++)
                {
                    if (tempUserList.Count > i)
                    {
                        m_clanPostTransform.Find(string.Format("{0}/Name", i + 3)).GetComponent<Text>().text = tempUserList[i].name;
                        m_clanPostTransform.Find(string.Format("{0}/Button", i + 3)).GetComponent<Button>().onClick.AddListenerIfNoExist(() => { this.OnClickPost(tempUserList[i]); });
                    }
                    else
                    {
                        m_clanPostTransform.Find(string.Format("{0}/Name", i + 3)).GetComponent<Text>().text = "暂无";
                    }
                }

                //长老
                tempUserList.Clear();
                tempUserList = ClanCommon.GetMemberName(m_curData, ClanPost.Clan_Elder);
                for (int i = 0; i < 4; i++)
                {
                    if (tempUserList.Count > i)
                    {
                        m_clanPostTransform.Find(string.Format("{0}/Name", i + 5)).GetComponent<Text>().text = tempUserList[i].name;
                        m_clanPostTransform.Find(string.Format("{0}/Button", i + 5)).GetComponent<Button>().onClick.AddListenerIfNoExist(() => { this.OnClickPost(tempUserList[i]); });
                    }
                    else
                    {
                        m_clanPostTransform.Find(string.Format("{0}/Name", i + 5)).GetComponent<Text>().text = "暂无";
                    }
                }

            }
        }

        public bool OnClickHidePostUI(GameObject obj, BaseEventData bed)
        {
            if (m_clanPostTransform.gameObject.activeSelf)
            {
                if (obj == null || obj == m_clanPostTransform.Find("Bg2").gameObject || !obj.transform.IsChildOf(m_clanPostTransform))
                {
                    m_clanPostTransform.gameObject.SetActive(false);
                    return false;
                }
            }
            return true;
        }


        public void OnClickPost(ClanUser userInfo)
        {

        }

        //氏族历程相关
        public void OnClickProcess(object args = null)
        {
            if (m_clanHistoryTansform != null)
            {
                m_clanHistoryTansform.gameObject.SetActive(true);

                xys.UI.EventHandler.pointerClickHandler.Add(OnClickHideHistory);

                RefreshHistory();

                m_clanHistoryTansform.Find("close_button").GetComponent<Button>().onClick.AddListenerIfNoExist(() => { m_clanHistoryTansform.gameObject.SetActive(false); });


            }
        }

        public bool OnClickHideHistory(GameObject obj, BaseEventData bed)
        {
            if (m_clanHistoryTansform.gameObject.activeSelf)
            {
                if (obj == null || obj == m_clanHistoryTansform.Find("Bg2").gameObject || !obj.transform.IsChildOf(m_clanHistoryTansform))
                {
                    m_clanHistoryTansform.gameObject.SetActive(false);
                    return false;
                }
            }
            return true;

        }

        public void RefreshHistory()
        {
            if (m_curData != null && m_historyView != null)
            {
                List<ClanHistoryViewData> clanItems = new List<ClanHistoryViewData>();
                clanItems.Clear();
                if (m_curData != null)
                {
                    foreach (var item in m_curData.history.allData)
                    {
                        ClanHistoryViewData clanUserData = new ClanHistoryViewData();
                        clanUserData.data = item.Value;
                        clanItems.Add(clanUserData);
                    }

                    m_historyView.DataSource = new UIWidgets.ObservableList<ClanHistoryViewData>(clanItems);
                }

            }
        }
        //返回氏族
        public void OnClickGoBackClan(object args = null)
        {

        }

        public void OnClickChangeNotice(object args = null)
        {
            if (m_changeNotice != null)
            {
                m_changeNotice.gameObject.SetActive(true);
                //m_changeNotice.Find("GuildNotice/Text").GetComponent<Text>().text = "";

                xys.UI.EventHandler.pointerClickHandler.Add(OnClickHideChangeNotice);

                m_changeNotice.Find("Button").GetComponent<Button>().onClick.AddListenerIfNoExist(() =>
                {

                    if (m_changeNotice.Find("GuildNotice/Text").GetComponent<Text>().text != "" && m_changeNotice.Find("GuildNotice/Text").GetComponent<Text>().text != null)
                    {
                        ClanDbData newData = new ClanDbData();
                        newData.clanid = App.my.localPlayer.clanIdValue;
                        newData.notice = m_changeNotice.Find("GuildNotice/Text").GetComponent<Text>().text;
                        App.my.eventSet.FireEvent<ClanDbData>(EventID.Clan_UpdataInfo, newData);
                        m_changeNotice.gameObject.SetActive(false);
                    }

                });
            }
        }

        public bool OnClickHideChangeName(GameObject obj, BaseEventData bed)
        {
            if (m_changeClanName.gameObject.activeSelf)
            {
                if (obj == null || obj == m_changeClanName.Find("mask").gameObject || !obj.transform.IsChildOf(m_changeClanName))
                {
                    m_changeClanName.gameObject.SetActive(false);
                    return false;
                }
            }
            return true;
        }


        public bool OnClickHideChangeNotice(GameObject obj, BaseEventData bed)
        {
            if (m_changeNotice.gameObject.activeSelf)
            {
                if (obj == null || obj == m_changeNotice.Find("mask").gameObject || !obj.transform.IsChildOf(m_changeNotice))
                {
                    m_changeNotice.gameObject.SetActive(false);
                    return false;
                }
            }
            return true;
        }


        public void SetTextColor(int colorId)
        {
            Dictionary<int, GameClanFlagColor> allConfig = GameClanFlagColor.GetAll();
            if (colorId >= allConfig.Count)
            {
                return;
            }
            if (allConfig[colorId].mainColor.Length == 6)
            {
                float r = 0;
                float g = 0;
                float b = 0;
                StringToRgb(allConfig[colorId].mainColor, out r, out g, out b);
                m_flagName.color = new Color(r / 255f, g / 255f, b / 255f);
            }

        }


        //必须是16进制颜色str  比如 FFFFFF
        public void StringToRgb(string Str, out float r, out float g, out float b)
        {
            string str1 = Str.Substring(0, 2);
            r = Convert.ToInt32(str1, 16);

            string str2 = Str.Substring(2, 2);
            g = Convert.ToInt32(str2, 16);

            string str3 = Str.Substring(4, 2);
            b = Convert.ToInt32(str3, 16);
        }
    }
}

#endif