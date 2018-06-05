#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using xys.UI.State;
using xys;
using xys.UI;

namespace xys.hot.UI
{
    [System.Serializable]
    public class UIMainMiniMap
    {
        [SerializeField]
        [PackTool.Pack]
        GameObject m_mapObj;
        [SerializeField]
        MapDetails m_mapDetails;
        [SerializeField]
        Button m_mapBtn;
        [SerializeField]
        Transform m_parent;
        [SerializeField]
        float m_alpha;

        GameObject m_map;

        public void OnInit(Event.HotObjectEventAgent parentEvent)
        {
            m_map = Object.Instantiate(m_mapObj); 
            m_map.transform.SetParent(m_parent);
            m_map.transform.localScale = Vector3.one;
            m_map.transform.localPosition = Vector3.zero;

            ILMonoBehaviour il = m_map.GetComponent<ILMonoBehaviour>();
            m_mapDetails = il.GetObject() as MapDetails;
            m_mapDetails.Init(m_map.transform, parentEvent);
            m_mapDetails.SetData(App.my.localPlayer.GetModule<LevelModule>().levelId, MapType.MinType);
            m_mapDetails.SetAction(MapMove);
            m_mapBtn.onClick.AddListener(OnClickMap);
        }

        /// <summary>
        /// 点击小地图
        /// </summary>
        void OnClickMap()
        {
            App.my.uiSystem.ShowPanel(PanelType.UIMapPanel, null);
        }

        /// <summary>
        /// 小地图移动
        /// </summary>
        void MapMove()
        {
            if (null != m_mapDetails)
            {
                Vector3 mainPos = m_mapDetails.GetMainPos();
                m_map.transform.localPosition = -mainPos;
            }
        }
    }
}
#endif
