#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using UIWidgets;
using UI;
using Config;
using System.Collections.Generic;

namespace xys.hot.UI
{
    class UIMapPanel : HotPanelBase
    {
        [SerializeField]
        Button m_returnBtn;                         //返回按钮
        [SerializeField]
        Button m_layerBtn;                          //分层按钮
        [SerializeField]
        Button m_npcBtn;                            //npc列表按钮
        [SerializeField]
        RectTransform m_npcPart;                    //npc部分
        [SerializeField]
        ListViewIcons m_listViewIcons;              //npc列表控件
        [SerializeField]
        TweenPosition m_npcTween;                   //npc列表移动控件
        [SerializeField]
        GameObject m_worldMap;                      //世界地图
        [SerializeField]
        Button m_returnToDetailsBtn;                //世界地图返回地图细节
        [SerializeField]
        Button m_lingwujun;                         //世界地图中区域地图按钮

        [SerializeField]
        [PackTool.Pack]
        GameObject m_mapObj;
        [SerializeField]
        MapDetails m_mapDetails;
        [SerializeField]
        Transform m_parent;
        [SerializeField]
        float m_alpha;

        bool m_showNpc = false;
        GameObject m_map;

        public UIMapPanel() : base(null)
        {

        }

        public UIMapPanel(UIHotPanel parent) : base(parent)
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInit()
        {
            m_map = Object.Instantiate(m_mapObj);
            m_map.transform.SetParent(m_parent);
            m_map.transform.localScale = Vector3.one;
            m_map.transform.localPosition = Vector3.zero;
            ILMonoBehaviour il = m_map.GetComponent<ILMonoBehaviour>();
            m_mapDetails = il.GetObject() as MapDetails;

            m_returnBtn.onClick.RemoveAllListeners();
            m_returnBtn.onClick.AddListener(OnClickReturn);
            m_layerBtn.onClick.RemoveAllListeners();
            m_layerBtn.onClick.AddListener(OnClickLayer);
            m_npcBtn.onClick.RemoveAllListeners();
            m_npcBtn.onClick.AddListener(OnClickNpc);
            m_returnToDetailsBtn.onClick.AddListener(OnReturnToDetails);

            m_lingwujun.onClick.AddListener(() => { OnCLickWorldItem(m_lingwujun.gameObject); });

            m_listViewIcons.OnSelect.AddListener(OnSelectCallback);
            m_listViewIcons.OnDeselect.AddListener(OnDeselectCallback);
        }

        protected override void OnShow(object args)
        {
            int levelId = App.my.localPlayer.GetModule<LevelModule>().levelId;
            RefreshUI(levelId);
            m_mapDetails.Init(m_map.transform, Event);
            m_mapDetails.SetData(levelId, MapType.MapPanel);
        }

        protected override void OnHide()
        {

        }

        //点击返回
        void OnClickReturn()
        {
            App.my.uiSystem.HidePanel(PanelType.UIMapPanel);
        }

        //点击分层
        void OnClickLayer()
        {
            //从当前地图切换到世界地图
            m_worldMap.SetActive(true);
            m_worldMap.transform.SetAsLastSibling();

            //显示当前所在地图的箭头
            Transform btns = m_worldMap.transform.Find("Btns");
            for (int i = 0; i < btns.childCount; ++i)
            {
                Transform child = btns.GetChild(i);
                WorldMapItem item = child.GetComponent<WorldMapItem>();
                if (null == item || null == item.m_locateArrow)
                {
                    child.gameObject.SetActive(false);
                    continue;
                }

                if (item.m_sceneId == App.my.localPlayer.GetModule<LevelModule>().levelId)
                {
                    item.m_locateArrow.SetActive(true);
                }
                else
                {
                    item.m_locateArrow.SetActive(false);
                }
            }
        }

        //点击回到地图细节
        void OnReturnToDetails()
        {
            m_worldMap.SetActive(false);
            RefreshUI(App.my.localPlayer.GetModule<LevelModule>().levelId);
        }

        //点击npc按钮
        void OnClickNpc()
        {
            if (!m_showNpc)
            {
                //显示
                //m_npcPart.anchoredPosition = new Vector2(0, 0);
                m_npcTween.PlayForward();
            }
            else
            {
                //隐藏
                //m_npcPart.anchoredPosition = new Vector2(-300, 0);
                m_npcTween.PlayReverse();
            }
            m_showNpc = !m_showNpc;
        }

        //选择npc
        void OnSelectCallback(int index, ListViewItem item)
        {
            MapConfig mapData = GetMapData(App.my.localPlayer.GetModule<LevelModule>().levelId);
            if (null == mapData)
            {
                Debuger.LogError("当前地图id取不到数据");
                return;
            }

            string npcList = mapData.npc;
            if (string.IsNullOrEmpty(npcList))
            {
                Debuger.LogError("npc列表为空");
                return;
            }

            string[] npcs = npcList.Split(';');
            if (index >= npcs.Length)
            {
                Debuger.LogError("超过npc列表长度");
                return;
            }

            if (null != item)
            {
                UIMapNpcItem npcItem = item as UIMapNpcItem;
                npcItem.OnSelect(true);

                //选中npc则寻路
                RoleDefine data = npcItem.Item.Data as RoleDefine;
                if (null != data)
                {
                    int npcId = data.id;
                    float arriveDis = data.behitRaidus < 1.5f ? 1.5f : data.behitRaidus;
                    IObject npc = App.my.sceneMgr.GetObj(npcId);
                    if (null != npc)
                    {
                        //npc.NpcOnClick();
                        m_mapDetails.MapFindPath(npc.position, arriveDis - 0.1f, true);
                    }
                    else
                    {
                        Debuger.LogError("场景中找不到该npc  id: " + npcId);
                    }
                }
                else
                {
                    Debuger.LogError("取不到npcitem的数据");
                }
            }
        }

        //取消选择npc
        void OnDeselectCallback(int index, ListViewItem item)
        {
            MapConfig mapData = GetMapData(App.my.localPlayer.GetModule<LevelModule>().levelId);
            if (null == mapData)
            {
                Debuger.LogError("当前地图id取不到数据");
                return;
            }

            string npcList = mapData.npc;
            if (string.IsNullOrEmpty(npcList))
            {
                Debuger.LogError("npc列表为空");
                return;
            }

            string[] npcs = npcList.Split(';');
            if (index >= npcs.Length)
            {
                Debuger.LogError("超过npc列表长度");
                return;
            }
            if (null != item)
            {
                UIMapNpcItem npcItem = item as UIMapNpcItem;
                npcItem.OnSelect(false);
            }
        }

        //取消当前选择
        public void DeselectCurrent()
        {
            m_listViewIcons.SelectedIndex = -1;
        }

        //暂时没有世界地图
        void RefreshUI(int sceneId)
        {
            MapConfig mapData = GetMapData(sceneId);
            if (null == mapData)
            {
                Debuger.LogError("当前地图id取不到数据");
                return;
            }
            //显示地图资源
            string resId = mapData.resId;
            if (!string.IsNullOrEmpty(resId))
            {
                //m_map.gameObject.SetActive(true);
                m_map.SetActive(true);
                m_mapDetails.SetData(sceneId, MapType.MapPanel);
            }

            //显示npc列表
            string npcList = mapData.npc;
            if (!string.IsNullOrEmpty(npcList))
            {
                string[] npcs = npcList.Split(';');
                List<ListViewIconsItemDescription> desList = new List<ListViewIconsItemDescription>();
                for (int i = 0; i < npcs.Length; ++i)
                {
                    int npcId = 0;
                    if (int.TryParse(npcs[i], out npcId))
                    {
                        RoleDefine npcData = RoleDefine.Get(npcId);
                        if (null != npcData)
                        {
                            ListViewIconsItemDescription des = new ListViewIconsItemDescription();
                            des.Name = npcData.name;
                            //des.Icon = PackTool.SpritesLoad.Get(npcData.littleHead);
                            des.Data = npcData;
                            desList.Add(des);
                        }
                    }
                }
                if (null != desList && desList.Count > 0)
                {
                    ObservableList<ListViewIconsItemDescription> oList = new ObservableList<ListViewIconsItemDescription>(desList);
                    m_listViewIcons.DataSource = oList;
                }
            }
        }

        MapConfig GetMapData(int levelId)
        {
            LevelDefine levelConfig = LevelDefine.Get(levelId);
            return MapConfig.Get(int.Parse(levelConfig.mapId));
        }

        //世界地图点击区域
        void OnCLickWorldItem(GameObject go)
        {
            WorldMapItem item = go.GetComponent<WorldMapItem>();
            if (null == item)
            {
                Debuger.LogError("世界地图中的区域item没有附加WorldMapItem脚本并配置");
                return;
            }

            int sceneId = item.m_sceneId;
            if (null != GetMapData(sceneId))
            {
                m_worldMap.SetActive(false);
                RefreshUI(sceneId);
            }
            else
            {
                Debuger.LogError("取不到地图数据， 配置id: " + sceneId);
            }
        }
    }
}
#endif
