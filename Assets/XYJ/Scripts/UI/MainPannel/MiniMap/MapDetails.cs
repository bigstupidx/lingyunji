#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using System.Collections.Generic;
    using xys.UI;
    using UnityEngine.UI;
    using Battle;
    using System;
    using UnityEngine.AI;
    using Config;
    using NetProto;

    public enum MapType
    {
        MapPanel,           //ui
        MinType,            //小地图
    }

    [AutoILMono]
    public class MapDetails
    {
#pragma warning disable
        const float MAPSIZE = 750;                                                      //常量地图大小

        bool m_isInit = false;                                                          //是否已经初始化
        Vector2 m_originPos = new Vector2(-MAPSIZE / 2F, -MAPSIZE / 2F);              //左下角原点坐标
        MapConfig m_mapData;                                                         //地图数据
        List<Vector2> truePosList = new List<Vector2>();
        float m_proportion = 0f;                                                        //真实场景和地图的比例
        MapType m_mapType = MapType.MapPanel;                                           //地图类型

        LocalPlayer m_mainPlayer;                               //主角
        Transform m_transform;

        [SerializeField]
        RectTransform m_mainRoleArrow;      //主角箭头
        [SerializeField]
        Transform m_monsterPointParent;     //怪物显示
        [SerializeField]
        GameObject m_monterPonit;       
        [SerializeField]
        Transform m_npcPointParent;         //npc显示，npc需要监听npc的刷新和消失
        [SerializeField]
        GameObject m_npcPoint;
        [SerializeField]
        Transform m_teamPointParent;        //队伍显示,组队的需要监听队伍的变更
        [SerializeField]
        GameObject m_teamPoint;
        [SerializeField]
        Transform m_pathPointParent;        //路径点显示
        [SerializeField]
        GameObject m_pathPoint;
        [SerializeField]
        GameObject m_pathEnd;
        [SerializeField]
        Image m_mapName;                    //地图名称

        Dictionary<int, GameObject> m_pointDIc = new Dictionary<int, GameObject>();
        Dictionary<long, GameObject> m_teamDic = new Dictionary<long, GameObject>();
        List<GameObject> m_pathPointList = new List<GameObject>();
        int m_currentIndex = 0;

        System.Action m_action;

        public bool IsInit
        {
            get { return m_isInit; }
            set { m_isInit = false; }
        }
#pragma warning restore

        public void Init(Transform trans, hot.Event.HotObjectEventAgent parentEvent)
        {
            m_transform = trans;

            parentEvent.Subscribe<IObject>(EventID.Scene_AddObj, OnAddObject);
            parentEvent.Subscribe<int>(EventID.Scene_RemoveObj, OnDeleteObject);
            parentEvent.Subscribe<TeamAllTeamInfo>(EventID.Team_DataChange, OnTeamChange);
            parentEvent.Subscribe<int>(EventID.MainPanel_ChangeMinimap, OnChangeMap);

            m_pointDIc = new Dictionary<int, GameObject>();
            m_teamDic = new Dictionary<long, GameObject>();
            m_pathPointList = new List<GameObject>();
            m_currentIndex = 0;
        }

        void Awake()
        {
        }

        void Start()
        {
            //m_mainPlayer = App.my.localPlayer;
            //m_mainRoleArrow = m_transform.Find("MainRoleArrow").GetComponent<RectTransform>();
            //m_monsterPointParent = m_transform.Find("MonsterList");
            //m_monterPonit = m_transform.Find("monster").gameObject;
            //m_npcPointParent = m_transform.Find("NpcList");
            //m_npcPoint = m_transform.Find("npc").gameObject;
            //m_teamPointParent = m_transform.Find("TeamList");
            //m_teamPoint = m_transform.Find("team").gameObject;
            //m_pathPointParent = m_transform.Find("PathList");
            //m_pathPoint = m_transform.Find("path").gameObject;
            //m_pathEnd = m_transform.Find("End").gameObject;
            //m_pathEnd.SetActive(false);
            //m_mapName = m_transform.Find("MapName").GetComponent<Image>();
        }

        void OnDestroy()
        {
            //m_monsterPointParent.DestroyChildren();
            //m_npcPointParent.DestroyChildren();
            //m_teamPointParent.DestroyChildren();
            //m_pathPointParent.DestroyChildren();
            if (null != m_pointDIc)
            {
                m_pointDIc.Clear();
            }
            m_pointDIc = null;
            if (null != m_pathPointList)
            {
                m_pathPointList.Clear();
            }
            m_pathPointList = null;
            if (null != m_teamDic)
            {
                m_teamDic.Clear();
            }
            m_teamDic = null;

            m_currentIndex = 0;
            m_mainPlayer = null;
        }

        void OnEnable()
        {
            //if (m_mainPlayer == null)
            //    m_mainPlayer = App.my.localPlayer;
        }

        void OnDisable()
        {
            m_pathPointParent.DestroyChildren();
            //m_mainPlayer = null;
            m_pathEnd.SetActive(false);

            m_currentIndex = 0;
        }

        void Update()
        {
            //实时更新坐标，更新自己，队友和怪物的
            if (!m_isInit)
                return;

            //正在切换场景
            if (App.my.appStateMgr.curState == AppStateType.ChangeScene)
                return;

            if (m_mainPlayer == null)
                m_mainPlayer = m_mainPlayer = App.my.localPlayer;

            if (null == m_mainPlayer)
                return;

            //更新主角的位置和朝向
            m_mainRoleArrow.localPosition = WorldToMapPos(m_mainPlayer.position);
            Vector3 rotation = new Vector3(0, 0, -m_mainPlayer.rotateAngle);
            m_mainRoleArrow.localRotation = Quaternion.Lerp(m_mainRoleArrow.localRotation, Quaternion.Euler(rotation), Time.deltaTime * 3);

            //更新怪物和npc的位置
            foreach (KeyValuePair<int, GameObject> pair in m_pointDIc)
            {
                IObject iobj = App.my.sceneMgr.GetObj((int)pair.Key);
                if (null != iobj)
                {
                    pair.Value.transform.localPosition = WorldToMapPos(iobj.position);
                }
            }

            //更新队友的位置
            foreach (KeyValuePair<long, GameObject> pair in m_teamDic)
            {
                IObject iobj = App.my.sceneMgr.GetObj((int)pair.Key);

                if (null != iobj)
                {
                    pair.Value.transform.localPosition = WorldToMapPos(iobj.position);
                }
            }

            if (Input.GetMouseButtonUp(0) && m_mapType == MapType.MapPanel)
            {
                Vector2 pos = ToMapUI(Input.mousePosition);
                if (pos.x >= m_originPos.x && pos.y >= m_originPos.y && pos.x <= -m_originPos.x && pos.y <= -m_originPos.y)
                {
                    NavMeshPath temp = new NavMeshPath();
                    if (!NavMesh.CalculatePath(m_mainPlayer.position, MapToWorldPos(pos), NavMesh.AllAreas, temp))
                    {
                        return;
                    }
                    MapFindPath(pos, 0.1f);

                    //UIMapPanel panel = UI.UISystem.Get<UIMapPanel>();
                    //if (null != panel)
                    //{
                    //    //取消当前选择
                    //    panel.DeselectCurrent();
                    //}
                }
            }

            if (m_mapType == MapType.MapPanel)
            {
                //更新路点
                m_pathPointList.Remove(null);
                if (null != m_pathPointList && m_pathPointList.Count > 0)
                {
                    Vector3 p0 = m_pathPointList[0].transform.localPosition;
                    Vector3 pm = m_mainRoleArrow.localPosition;
                    if (Vector3.Distance(p0, pm) < 10f)
                    {
                        GameObject.Destroy(m_pathPointList[0]);
                        m_pathPointList.RemoveAt(0);
                    }
                }
            }

            if (null != m_action)
                m_action();
        }

        public void MapFindPath(Vector3 pos, float arr, bool truePos = false)
        {
            Vector3 target = MapToWorldPos(pos);
            Vector3 final = truePos ? pos : target;
            //todo 寻路
            //LevelSystem.Instance.MoveToPos(final, PathCallback, arr, true, MoveFinished);
            //BattleSystem.SetTrusteeshipType(TrusteeshipType.Path);
            CreatePath(final, arr);
        }

        private void MoveFinished()
        {
            //if (BattleSystem.trusteeshipType == TrusteeshipType.Path)
            //{
            //    BattleSystem.SetTrusteeshipType(TrusteeshipType.None);
            //}
        }

        void PathCallback()
        {
            //临时处理
            if (this == null)
                return;

            if (m_transform.gameObject.activeSelf)
            {
                m_pathPointParent.DestroyChildren();
                m_currentIndex = 0;
                m_pathEnd.SetActive(false);
            }
            MoveFinished();
        }

        /// <summary>
        /// 侦听切换地图
        /// </summary>
        /// <param name="levelId"></param>
        void OnChangeMap(int levelId)
        {
            SetData(levelId, MapType.MinType);
        }

        /// <summary>
        /// 设置小地图的数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="trans"></param>
        public void SetData(int levelId, MapType type)
        {
            LevelDefine levelConfig = LevelDefine.Get(levelId);
            if (null == levelConfig)
                return;
            m_mainPlayer = App.my.localPlayer;
            int mapId;
            if(!int.TryParse(levelConfig.mapId, out mapId))
            {
                return;
            }
            m_mapData = MapConfig.Get(mapId);
            if (null == m_mapData)
                return;
            m_mapType = type;

            //设置图片
            Helper.SetSprite(m_transform.GetComponent<Image>(), m_mapData.resId);
            m_transform.gameObject.SetActive(true);

            //设置地图名称
            if (m_mapType == MapType.MapPanel)
            {
                if (!string.IsNullOrEmpty(m_mapData.nameId))
                {
                    Helper.SetSprite(m_mapName, m_mapData.nameId);
                    m_mapName.gameObject.SetActive(true);
                }
                else
                {
                    m_mapName.gameObject.SetActive(false);
                }
            }
            else
            {
                m_mapName.gameObject.SetActive(false);
            }

            //解析比例
            ParseProportion(m_mapData.pos);

            if (null != m_pointDIc)
            {
                m_pointDIc.Clear();
            }

            if (null != m_teamDic)
            {
                m_teamDic.Clear();
            }

            if (null != m_pathPointList)
            {
                m_pathPointList.Clear();
            }

            //创建npc
            CreateNpc();

            //创建怪物
            CreateMonster();

            //创建队友
            CreateTeam();

            m_isInit = true;
        }

        //解析比例,顺序左上，右上，右下，左下
        void ParseProportion(string posStr)
        {
            if (string.IsNullOrEmpty(posStr))
            {
                Debuger.LogError("坐标配置为空");
                return;
            }

            if (!posStr.Contains(";"))
            {
                Debuger.LogError("坐标格式不正确");
                return;
            }

            string[] poss = posStr.Split(';');
            if (4 != poss.Length)
            {
                Debuger.LogError("坐标格式不正确");
                return;
            }

            truePosList = new List<Vector2>();
            for (int i = 0; i < poss.Length; ++i)
            {
                string pos = poss[i];
                pos = pos.Replace("(", "");
                pos = pos.Replace(")", "");
                string[] tempList = pos.Split('|');
                if (3 != tempList.Length)
                {
                    Debuger.LogError("坐标格式不正确");
                    return;
                }
                float x, z;
                if (float.TryParse(tempList[0], out x) && float.TryParse(tempList[2], out z))
                {
                    Vector2 vec = new Vector2(x, z);
                    truePosList.Add(vec);
                }
                else
                {
                    Debuger.LogError("坐标解析错误");
                    return;
                }
            }

            if (4 != truePosList.Count)
            {
                Debuger.LogError("坐标解析错误");
                return;
            }

            float trueSize = Mathf.Abs(truePosList[2].x - truePosList[3].x);
            m_proportion = MAPSIZE / trueSize;
        }

        //新增角色
        void OnAddObject(IObject obj)
        {
            int id = obj.charSceneId;
            if (m_pointDIc.ContainsKey(id))
                return;

            //if (string.IsNullOrEmpty(ob.cfgInfo.showInMap))
            //    return;

            //判断是npc还是怪物
            //if (obj.type == ObjectType.Monster)
            //{
            //    //增加怪物
            //    GameObject go = CreateObj(m_monsterPointParent, m_monterPonit);
            //    m_pointDIc.Add(id, go);
            //}
            if (obj.type == ObjectType.Npc)
            {
                //增加npc
                GameObject go = CreateObj(m_npcPointParent, m_npcPoint);
                m_pointDIc.Add(id, go);
            }
        }

        //删除角色
        void OnDeleteObject(int id)
        {
            if (!m_pointDIc.ContainsKey(id))
                return;

            GameObject.Destroy(m_pointDIc[id]);
            m_pointDIc[id] = null;
            m_pointDIc.Remove(id);
        }

        //创建物体
        GameObject CreateObj(Transform parent, GameObject prefab)
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.SetActive(true);

            return go;
        }

        //创建怪物
        void CreateMonster()
        {
            //m_monsterPointParent.DestroyChildren();
            //List<IObject> monsterList = App.my.sceneMgr.GetMapRoleByType(ObjectType.Monster);
            //if (null != monsterList && monsterList.Count > 0)
            //{
            //    for (int i = 0; i < monsterList.Count; ++i)
            //    {
            //        GameObject go = CreateObj(m_monsterPointParent, m_monterPonit);
            //        m_pointDIc.Add(monsterList[i].charSceneId, go);
            //    }
            //}
        }

        //创建npc
        void CreateNpc()
        {
            m_npcPointParent.DestroyChildren();
            List<IObject> npcList = App.my.sceneMgr.GetMapRoleByType(ObjectType.Npc);
            if (null != npcList && npcList.Count > 0)
            {
                for (int i = 0; i < npcList.Count; ++i)
                {
                    GameObject go = CreateObj(m_npcPointParent, m_npcPoint);
                    m_pointDIc.Add(npcList[i].charSceneId, go);
                }
            }
        }

        //创建队员
        void CreateTeam()
        {
            m_teamPointParent.DestroyChildren();
            if (m_mainPlayer == null)
            {
                Debuger.LogError("取不到队伍系统");
                return;
            }

            //没有队伍
            if (!m_mainPlayer.GetModule<TeamModule>().InTeam())
            {
                return;
            }

            NetProto.TeamAllTeamInfo allTeamInfo = m_mainPlayer.GetModule<TeamModule>().GetTeamAllInfo();
            if (null != allTeamInfo.members && allTeamInfo.members.Count > 0)
            {
                foreach (KeyValuePair<long, TeamMemberData> pair in allTeamInfo.members)
                {
                    if (pair.Key != m_mainPlayer.charSceneId && pair.Value.zoneId == App.my.localPlayer.GetModule<LevelModule>().zoneId)
                    {
                        //跳过自己
                        GameObject go = CreateObj(m_teamPointParent, m_teamPoint);
                        m_teamDic.Add(pair.Key, go);
                    }
                }
            }
        }

        /// <summary>
        /// 队伍变更
        /// </summary>
        /// <param name="teamInfo"></param>
        void OnTeamChange(TeamAllTeamInfo teamInfo)
        {
            //队伍成员变更的时候，都全部删除并重新创建
            foreach (var itor in m_teamDic)
            {
                GameObject.Destroy(itor.Value);
            }
            m_teamDic.Clear();

            //重新创建
            Dictionary<long, TeamMemberData> dic = teamInfo.members;
            if (null != dic && dic.Count > 0)
            {
                foreach (var itor in dic)
                {
                    if (itor.Key == m_mainPlayer.charSceneId)
                        continue;
                    GameObject go = CreateObj(m_teamPointParent, m_teamPoint);
                    m_teamDic.Add(itor.Key, go);
                }
            }
        }

        void CreatePath(Vector3 target, float arr)
        {
            m_pathPointParent.DestroyChildren();
            m_pathPointList.Remove(null);
            m_pathPointList.Clear();
            Vector3[] pathList = /*m_mainPlayer.m_pathManage.GetCurPathPoint()*/null;
            if (null != pathList && pathList.Length > 0)
            {
                for (int i = 0; i < pathList.Length; ++i)
                {
                    GameObject go = CreateObj(m_pathPointParent, m_pathPoint);
                    go.name = i.ToString();
                    m_pathPointList.Add(go);
                    go.transform.localPosition = WorldToMapPos(pathList[i]);

                    if (i + 1 < pathList.Length)
                    {
                        Vector3 p1 = WorldToMapPos(pathList[i]);
                        Vector3 p2 = WorldToMapPos(pathList[i + 1]);
                        if (Vector3.Distance(p1, p2) > 10)
                        {
                            //插入点
                            Vector3 dir = (p2 - p1).normalized;
                            int count = (int)(Vector3.Distance(p1, p2) / 10f);
                            for (int j = 0; j < count; ++j)
                            {
                                GameObject goTemp = CreateObj(m_pathPointParent, m_pathPoint);
                                m_pathPointList.Add(goTemp);
                                goTemp.transform.localPosition = p1 + dir * 10 * (j + 1);
                            }
                        }
                    }
                }
            }
            m_currentIndex = 0;

            if (m_pathPointList.Count > 0)
            {
                //设置终点坐标
                m_pathEnd.SetActive(true);
                m_pathEnd.transform.localPosition = m_pathPointList[m_pathPointList.Count - 1].transform.localPosition;
            }
            else
                m_pathEnd.SetActive(false);

        }

        //从世界坐标转换到地图坐标
        Vector3 WorldToMapPos(Vector3 pos)
        {
            Vector2 v2pos = new Vector2(pos.x, pos.z);
            Vector2 trueOriginPos = truePosList[3];
            float deltaX = (v2pos.x - trueOriginPos.x) * m_proportion;
            float deltaY = (v2pos.y - trueOriginPos.y) * m_proportion;

            return new Vector3(m_originPos.x + deltaX, m_originPos.y + deltaY, 0);
        }

        //从地图坐标转换到世界坐标
        Vector3 MapToWorldPos(Vector2 pos)
        {
            Vector2 trueOriginPos = truePosList[3];
            float deltaX = (pos.x - m_originPos.x) / m_proportion;
            float deltaY = (pos.y - m_originPos.y) / m_proportion;
            Vector3 temp = new Vector3(trueOriginPos.x + deltaX, 0, trueOriginPos.y + deltaY);
            return SetGroundHeight(temp);

            //return new Vector3(trueOriginPos.x + deltaX, 0, trueOriginPos.y + deltaY);
        }

        //鼠标坐标转换到ugui屏幕坐标
        Vector2 ToMapUI(Vector3 pos)
        {
            //CanvasScaler scaler = UI.UISystem.Instance.RootCanvas.GetComponent<CanvasScaler>();

            //float resolutionX = scaler.referenceResolution.x;
            //float resolutionY = scaler.referenceResolution.y;
            //float prop = resolutionY / (float)Screen.height;
            ////左下角0，0  右上角1334，750
            //Vector2 uiPos = new Vector2(pos.x * prop - resolutionX / 2, pos.y * prop - resolutionY / 2);

            float scaleFactor = 1f / App.my.uiSystem.RootCanvas.scaleFactor;
            float tempX = Screen.width * scaleFactor;
            float tempY = Screen.height * scaleFactor;

            Vector2 uiPos = new Vector2(pos.x * scaleFactor - tempX / 2, pos.y * scaleFactor - tempY / 2);

            //Debuger.LogError("pos : " + pos);
            //Debuger.LogError("uiPos : " + uiPos);
            return uiPos;
        }

        //寻路路径点改变
        void OnPathIndexChange(int index)
        {
            if (null != m_pathPointList && m_pathPointList.Count > 0)
            {
                GameObject.Destroy(m_pathPointList[0]);
                m_pathPointList.RemoveAt(0);
            }
        }

        //获取主角位置
        public Vector3 GetMainPos()
        {
            if (null != m_mainRoleArrow)
                return m_mainRoleArrow.localPosition;
            return Vector3.zero;
        }

        //获取地面高度
        Vector3 SetGroundHeight(Vector3 pos)
        {
            Vector3 temp = pos + Vector3.up * 1000;
            RaycastHit hit;
            int layerMask = ComLayer.Layer_GroundMask;
            if (Physics.Raycast(temp, Vector3.down, out hit, 2000, layerMask))
            {
                pos = hit.point + Vector3.up * 0.1f;
            }

            return pos;
        }

        public void SetAction(System.Action action)
        {
            m_action = action;
        }
    }

    [System.Serializable]
    public class MapDetailsVo
    {
        public GameObject m_prefab;
        public Transform m_parent;
        public float m_alpha = 1;
    }
}
#endif