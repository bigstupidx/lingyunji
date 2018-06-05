#if !USE_HOT
using System;
using System.Collections.Generic;
using Config;
using NetProto;
using UIWidgets;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WXB;
using xys.hot.Event;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class UIEmotionPanel : HotPanelBase
    {
        [SerializeField]
        private Animator animator;                      // 面板动画
        [SerializeField]
        private ScrollRect scroll;                      // scroll面板
        [SerializeField]
        private StateToggle functionToggle;             // 功能按钮

        [SerializeField]
        private Transform emotionGrid;                  // 表情容器

        [SerializeField]
        private Transform itemGrid;                     // 物品容器
        [SerializeField]
        private GameObject itemCell;                    // 物品子物体

        [SerializeField]
        private Transform petsGrid;                     // 宠物容器
        [SerializeField]
        private GameObject petCell;                     // 宠物子物体

        [SerializeField]
        private Transform historyGrid;                 // 历史容器
        [SerializeField]
        private GameObject historyCell;                 // 历史子物体

        [SerializeField]
        private Transform inputGrid;                    // 便捷输入容器
        [SerializeField]
        private GameObject inputCell;                   // 便捷输入子物体

        // 面板动画
        private EmotionOpenType type;
        // 点击隐藏事件
        private int onClickHideEventId = -1;

        private HotObjectEventAgent eventAgent;
        private bool init = false;
        #region Impl
        public UIEmotionPanel() : base(null) { }

        public UIEmotionPanel(UIHotPanel parent) : base(parent) { }

        protected override void OnInit()
        {
            // 初始默认为表情窗口
            scroll.content = emotionGrid.GetComponent<RectTransform>();
            lastSelectedGo = emotionGrid.gameObject;
            functionToggle.OnSelectChange = OnSelectChange;
        }

        protected override void OnShow(object p)
        {
            // 开启时的动画
            type = (EmotionOpenType)p;
            switch(type)
            {
                case EmotionOpenType.None:
                    break;
                case EmotionOpenType.Chat:
                    //AnimationHelp.PlayAnimation(animator, "ui_TanKuang_Tips1");
                    hotApp.my.eventSet.FireEvent(EventID.ChatPanel_OnChatPanelHangUpState, "顶起");
                    break;
                case EmotionOpenType.Hero:
                    break;
                case EmotionOpenType.Friend:
                    break;
                case EmotionOpenType.Count:
                    break;
                default:
                    Debuger.Log("Unknow open type!".Color());
                    break;
            }

            OpenEvent();
            // 注册事件
            eventAgent = new HotObjectEventAgent(App.my.localPlayer.eventSet);
            eventAgent.Subscribe(EventID.Package_UpdatePackage, ShowItemScroll);
            eventAgent.Subscribe(EventID.ChatPanel_RefreshHistory, ShowHistoryScrollView);
            // 重新打开更新显示
            if(init)
            {
                OnSelectChange(functionToggle.SelectObj, functionToggle.Select); 
            }
            else
            {
                init = true;
            }
        }

        protected override void OnHide()
        {
            eventAgent.Release();
            eventAgent = null;
            // 关闭时的动画
            switch(type)
            {
                case EmotionOpenType.None:
                    break;
                case EmotionOpenType.Chat:
                    //AnimationHelp.PlayAnimation(animator, "ui_TanKuang_Tips_Close1");
                    hotApp.my.eventSet.FireEvent(EventID.ChatPanel_OnChatPanelHangUpState, "常态");
                    break;
                case EmotionOpenType.Hero:
                    break;
                case EmotionOpenType.Friend:
                    break;
                case EmotionOpenType.Count:
                    break;
                default:
                    Debuger.Log("Unknow open type!".Color());
                    break;
            }

            CloseEvent();
        }

        private void OpenEvent()
        {
            if(onClickHideEventId != -1)
            {
                xys.UI.EventHandler.pointerClickHandler.Remove(onClickHideEventId);
            }
            onClickHideEventId = xys.UI.EventHandler.pointerClickHandler.Add(OnGlobalClick);
        }

        private void CloseEvent()
        {
            if(onClickHideEventId != -1)
            {
                xys.UI.EventHandler.pointerClickHandler.Remove(onClickHideEventId);
                onClickHideEventId = -1;
            }
        }

        private bool OnGlobalClick(GameObject go, BaseEventData data)
        {
            if(null == go || !go.transform.IsChildOf(parent.transform))
            {
                CloseEvent();
                App.my.uiSystem.HidePanel("UIEmotionPanel");
                return false;
            }
            return true;
        }
        #endregion

        #region Toggle
        private void OnSelectChange(StateRoot sr, int index)
        {
            switch(index)
            {
                case 0:// 表情
                    ShowEmotionScrollView();
                    break;
                case 1:// 便捷用语
                    ShowInputSimpleScrollView();
                    break;
                case 2:// 法宝
                    SystemHintMgr.ShowHint("功能尚未开放");
                    ShowEmotionScrollView();
                    break;
                case 3:// 物品
                    ShowItemScroll();
                    break;
                case 4:// 输入历史
                    ShowHistoryScrollView();
                    break;
                case 5:// 红包
                    SystemHintMgr.ShowHint("功能尚未开放");
                    ShowEmotionScrollView();
                    break;
                case 6:// 灵兽
                    SystemHintMgr.ShowHint("功能尚未开放");
                    ShowPetsScrollView();
                    break;
                case 7:// 寄售物品
                    SystemHintMgr.ShowHint("功能尚未开放");
                    ShowEmotionScrollView();
                    break;
                case 8:// 暂无
                    SystemHintMgr.ShowHint("功能尚未开放");
                    ShowEmotionScrollView();
                    break;

            }
        }

        private GameObject lastSelectedGo;
        private void RefreshScrollViewShow(GameObject show)
        {
            scroll.content = show.GetComponent<RectTransform>();
            if(show != lastSelectedGo)
            {
                lastSelectedGo.SetActive(false);
                show.SetActive(true);
                lastSelectedGo = show;
            }
        }
        #endregion

        #region 表情

        private bool emotionInit;
        private void ShowEmotionScrollView()
        {
            RefreshScrollViewShow(emotionGrid.gameObject);
            // 初始化
            if(!emotionInit)
            {
                var num = ChatUtil.GetEmotionNum();
                for(int i = 1 ; i < num + 1 ; i++)
                {
                    GameObject cell = emotionGrid.GetChild(i - 1).gameObject;
                    cell.SetActive(true);
                    // 改名以便点击时获取信息
                    cell.name = i.ToString();
                    // 图片默认取第一个
                    cell.transform.GetChild(0).GetComponent<Image>().SetSprite(string.Format("fb_{0}_1", i));
                    // 按钮事件
                    var btn = cell.GetComponent<ButtonEx>();
                    // 清除匿名函数
                    btn.OnClickWithoutDrag.RemoveAllListeners();
                    btn.OnClickWithoutDrag.AddListenerIfNoExist(() =>
                    {
                        hotApp.my.eventSet.FireEvent(EventID.ChatInput_OnReceiveFaceData, cell.name);
                    });
                }

                for(int i = num ; i < emotionGrid.childCount ; i++)
                {
                    emotionGrid.GetChild(i).gameObject.SetActive(false);
                }
                emotionInit = true;
            }
        }

        #endregion

        #region 道具
        private void ShowItemScroll()
        {
            RefreshScrollViewShow(itemGrid.gameObject);

            var equipMgr = hotApp.my.localPlayer.GetModule<EquipModule>().equipMgr as EquipMgr;
            Assert.IsNotNull(equipMgr, "Cannot get equipmodule's mgr in UIEmotionPanel!");
            var equips = equipMgr.GetAllEquips();
            var package = hotApp.my.GetModule<HotPackageModule>().packageMgr.package;
            // 先初始化已装备的信息
            if(equips.Count > 0)
            {
                equips.ForEach((data, i) =>
                {
                    GameObject cell;
                    if (itemGrid.childCount > i)
                    {
                        cell = itemGrid.GetChild(i).gameObject;
                    }
                    else
                    {
                        cell = GameObject.Instantiate(itemCell);
                        cell.transform.SetParent(itemGrid, false);
                        cell.transform.localScale = Vector3.one;
                    }

                    cell.SetActive(true);
                    var il = cell.GetComponent<ILMonoBehaviour>();
                    Assert.IsNotNull(il, "itemscrollview 的cell丢失ILMonoBehavior组件");
                    var item = (GoodsCell)il.GetObject();
                    item.OnCellAdding(true, data.Value, -1);
                });
            }

            int index = equips.Count;
            // 初始道具信息
            if(package.Count > 0)
            {
                package.ForEach(_ =>
                   {
                       if(!_.isEmpty)
                       {
                           GameObject cell;
                           if(itemGrid.childCount > index)
                           {
                               cell = itemGrid.GetChild(index).gameObject;
                           }
                           else
                           {
                               cell = GameObject.Instantiate(itemCell);
                               cell.transform.SetParent(itemGrid, false);
                               cell.transform.localScale = Vector3.one;
                           }

                           cell.SetActive(true);
                           var il = cell.GetComponent<ILMonoBehaviour>();
                           Assert.IsNotNull(il, "itemscrollview 的 cell 丢失ILMonoBehavior->goodscell组件");
                           var item = (GoodsCell)il.GetObject();
                           item.OnCellAdding(false, _.data.data, _.pos);
                           ++index;
                       }
                   });
            }

            // 将多余的物体隐藏
            for(int i = equips.Count + package.Count ; i < itemGrid.childCount ; i++)
            {
                itemGrid.GetChild(i).gameObject.SetActive(false);
            }
        }
        #endregion

        #region 宠物

        private void ShowPetsScrollView()
        {
            RefreshScrollViewShow(petsGrid.gameObject);
            var petsDatas = hotApp.my.GetModule<HotPetsModule>().petsMgr.m_PetsTable.attribute;

            for(int i = 0 ; i < petsDatas.Count ; i++)
            {
                GameObject cell;
                if(petsGrid.childCount > i)
                {
                    cell = petsGrid.GetChild(i).gameObject;
                }
                else
                {
                    cell = GameObject.Instantiate(petCell);
                    cell.transform.SetParent(petsGrid, false);
                    cell.transform.localScale = Vector3.one;
                }

                cell.SetActive(true);
                var il = cell.GetComponent<ILMonoBehaviour>();
                Assert.IsNotNull(il, "petscrollview 的 cell 丢失ILMonoBehavior->petscell组件");
                var item = (PetsCell)il.GetObject();
                item.OnCellAdding(i);
            }

            // 将多余的物体隐藏
            for(int i = petsDatas.Count ; i < petsGrid.childCount ; i++)
            {
                petsGrid.GetChild(i).gameObject.SetActive(false);
            }

        }
        #endregion

        #region 输入历史

        private void ShowHistoryScrollView()
        {
            RefreshScrollViewShow(historyGrid.gameObject);

            var count = ChatUtil.ChatMgr.GetHistoryListCount();
            for(int i = 0 ; i < count ; i++)
            {
                GameObject cell;
                if(historyGrid.childCount > i)
                {
                    cell = historyGrid.GetChild(i).gameObject;
                }
                else
                {
                    cell = GameObject.Instantiate(historyCell);
                    cell.transform.SetParent(historyGrid, false);
                    cell.transform.localScale = Vector3.one;
                }


                cell.SetActive(true);
                var il = cell.GetComponent<ILMonoBehaviour>();
                Assert.IsNotNull(il, "historyGrid 的 cell 丢失ILMonoBehavior->HistoryCell组件");
                var item = (HistoryCell)il.GetObject();
                item.OnCellAdding(i);
            }

            // 将多余的物体隐藏
            for(int i = count ; i < historyGrid.childCount ; i++)
            {
                historyGrid.GetChild(i).gameObject.SetActive(false);
            }
        }

        #endregion

        #region 便捷输入

        private bool inputInit;

        private void ShowInputSimpleScrollView()
        {
            RefreshScrollViewShow(inputGrid.gameObject);
            // 便捷输入只初始化一次
            if(!inputInit)
            {
                var cfgs = ChatInputSimple.GetAll();
                var num = Mathf.Min(cfgs.Count, ChatDefins.ChatInputSimpleNum);
                for(int i = 0 ; i < num ; i++)
                {
                    GameObject cell;
                    if (inputGrid.childCount > i)
                    {
                        cell = inputGrid.GetChild(i).gameObject;
                    }
                    else
                    {
                        cell = GameObject.Instantiate(inputCell);
                        cell.transform.SetParent(inputGrid, false);
                        cell.transform.localScale = Vector3.one;
                    }

                    cell.SetActive(true);
                    var il = cell.GetComponent<ILMonoBehaviour>();
                    Assert.IsNotNull(il, "inputGrid 的 cell 丢失ILMonoBehavior->textcell组件");
                    var item = (TextCellcs)il.GetObject();
                    item.OnCellAdding(i + 1);
                }
                inputInit = true;
            }
        }

        #endregion
    }
}
#endif