#if !USE_HOT
namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using NetProto;
    using Config;
    using xys.UI.State;
    using xys.UI;

    [System.Serializable]
     class UITrumpsInfused
    {
        [SerializeField]
        Transform m_Transform;
        [SerializeField]
        Text m_TasteLv;
        [SerializeField]
        Text m_InfuseLv;

        [SerializeField]
        Transform m_Grids;
        [SerializeField]
        GameObject m_LinePrefab;

        [SerializeField]
        StateRoot m_ButtonSR;
        [SerializeField]
        Button m_MarBtn1;
        [SerializeField]
        Button m_MarBtn2;
//         [SerializeField]
//         Text m_MarNum;
//         [SerializeField]
//         Image m_MarIcon;
//         [SerializeField]
//         Image m_MarQuality;
        [SerializeField]
        Text m_TasteUpCost;
        [SerializeField]
        Text m_TasteLvNeed;

        [SerializeField]
        Button m_InfusedBtn;
        [SerializeField]
        Button m_TasteBtn;

        TrumpsMgr m_TrumpMgr;
        
        [SerializeField]
        Transform m_ItemGrid;
        [SerializeField]
        Transform m_DropItems;

        [SerializeField]
        Transform m_InfuseTips;

        HotTablePageBase m_Page;

        int m_TrumpId;
        int m_UseItemId;

        TrumpInfused m_SelectedInfused;
        Dictionary<int, Transform> m_Items;
        public void OnInit(HotTablePageBase page)
        {
            m_Page = page;
            m_SelectedInfused = null;
            m_Items = new Dictionary<int, Transform>();
            m_TrumpMgr = hotApp.my.GetModule<HotTrumpsModule>().trumpMgr;
            m_TasteBtn.onClick.AddListener(this.OnTasteEvent);
            m_InfusedBtn.onClick.AddListener(this.OnInfuseEvent);
        }
        public void OnHide()
        {
            this.Clear();
        }
        public void Set(int trumpId)
        {
            if (!m_TrumpMgr.CheckTrumps(trumpId))
                return;
            this.m_TrumpId = trumpId;
            this.Clear();
            //
            TrumpAttribute attribute = m_TrumpMgr.table.attributes[trumpId];
            int tasteLv = attribute.tastelv;
            int infuseLv = attribute.infuseds.Count;
            if (!TrumpInfused.infusedDic.ContainsKey(trumpId) || !TrumpInfused.infusedDic[trumpId].ContainsKey(tasteLv))
                return;
            m_TasteLv.text = m_TrumpMgr.GetTasteDes(trumpId);
            m_InfuseLv.text = string.Empty;

            List<int> infuseList = attribute.infuseds;//已激活
            infuseList.Sort();
            List<TrumpInfused> infuseAll = TrumpInfused.infusedDic[trumpId][tasteLv];
            infuseAll.Sort((x, y) => { return x.infusedid > y.infusedid ? 1 : -1; });
            //设置线
            for (int i = 0; i < infuseAll.Count; i++)
            {
                int bindId = infuseAll[i].bindid;
                if (TrumpInfused.FindIndex(bindId) == -1)
                    continue;
                TrumpInfused frontInfused = TrumpInfused.Get(bindId);
                this.SetLine(m_Grids.GetChild(frontInfused.posid - 1), m_Grids.GetChild(infuseAll[i].posid - 1), infuseAll[i].infusedid);
            }
            //设置点
            for (int i = 0; i < infuseAll.Count; i++)
            {
                int index = i;
                Transform root = m_Grids.GetChild(infuseAll[index].posid - 1);
                root.gameObject.SetActive(true);
                root.Find("Prompt").gameObject.SetActive(false);
                root.Find("Light").gameObject.SetActive(false);
                root.GetComponent<StateRoot>().CurrentState = 2;
                //注册tips点击事件
                root.GetComponent<Button>().onClick.RemoveAllListeners();
                root.GetComponent<Button>().onClick.AddListener(() => { this.OnShowInfuseTips(root, infuseAll[index].infusedid); });
            }
            for (int i = 0; i < infuseList.Count;i++)
            {
                TrumpInfused item = TrumpInfused.Get(infuseList[i]);
                if (item == null) continue;
                int index = i;
                Transform root = m_Grids.GetChild(item.posid - 1);
                root.gameObject.SetActive(true);
                root.Find("Prompt").gameObject.SetActive(false);
                root.Find("Light").gameObject.SetActive(false);
                item = infuseAll.Find((data) => { return data.infusedid == infuseList[index]; });
                root.GetComponent<StateRoot>().CurrentState = item == null ? 2 : (int)item.type - 1;

                if(m_Items.ContainsKey(infuseList[index]))
                    m_Items[infuseList[index]].GetComponent<StateRoot>().CurrentState = 1;
            }
            //获取所有可以被点亮的点
            List<TrumpInfused> list = new List<TrumpInfused>();
            if (infuseList.Count > 0)
                for (int i = 0; i < infuseAll.Count; i++)
                    if (!infuseList.Contains(infuseAll[i].infusedid) && !list.Contains(infuseAll[i]))
                        list.Add(infuseAll[i]);

            if (list.Count == 0)
                list.Add(infuseAll[0]);
            //高亮可以点亮的点
            for (int i = 0;i < list.Count;i++)
                if (list[i].bindid == 0 || (infuseList.Count > 0 && infuseList.Contains(list[i].bindid)))
                    m_Grids.GetChild(list[i].posid - 1).Find("Prompt").gameObject.SetActive(true);

            if (list.Count > 0 && infuseList.Count != infuseAll.Count)
                this.OnSelectedInfusePoint(list[0]);
            else if(m_SelectedInfused != null)
                this.OnSelectedInfusePoint(m_SelectedInfused);
            else
                this.ResetMarUI(null);

        }
        void Clear()
        {
            //重置所有线
            while(m_ItemGrid.childCount > 0)
            {
                Transform item = m_ItemGrid.transform.GetChild(0);
                item.SetParent(m_DropItems, false);
                item.gameObject.SetActive(false);
            }
            //重置所有点
            for(int i = 0; i  < m_Grids.childCount;i++)
            {
                m_Grids.GetChild(i).GetComponent<StateRoot>().CurrentState = 2;
                m_Grids.GetChild(i).gameObject.SetActive(false);
            }

            m_Items.Clear();


            if (m_SelectedInfused != null)
                m_SelectedInfused = null;
        }

        //计算点位置角度
        void SetLine(Transform firstPoint,Transform secondPoint,int trumpInfuseId)
        {
            GameObject item = null;
            if(m_DropItems.childCount > 0)
            {
                item = m_DropItems.GetChild(0).gameObject;
            }
            else
            {
                item = GameObject.Instantiate(m_LinePrefab);
                if (item == null)
                    return;
            }

            item.GetComponent<StateRoot>().CurrentState = 0;

            item.SetActive(true);
            item.transform.SetParent(m_ItemGrid, false);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = new Vector3(firstPoint.localPosition.x + firstPoint.GetComponent<RectTransform>().sizeDelta.x * 0.5f, firstPoint.localPosition.y, 0.0f) ;

            Vector3 targetDir = firstPoint.localPosition - secondPoint.localPosition; // 目标坐标与当前坐标差的向量
            item.transform.eulerAngles = new Vector3(0.0f, 0.0f, Vector3.Angle(firstPoint.right, targetDir) - 180.0f); // 返回当前坐标与目标坐标的角度
            RectTransform rectTransform = item.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(Vector2.Distance(firstPoint.localPosition ,secondPoint.localPosition), rectTransform.sizeDelta.y);//当前连点坐标

            m_Items.Add(trumpInfuseId, item.transform);
        }

        void OnShowInfuseTips(Transform root,int infuseId)
        {
            if(TrumpInfused.FindIndex(infuseId) == -1)
                return;
                
            m_InfuseTips.gameObject.SetActive(true);
            m_InfuseTips.SetParent(root);
            m_InfuseTips.localScale = Vector3.one;
            m_InfuseTips.localPosition = Vector3.zero;

            TrumpInfused item = TrumpInfused.Get(infuseId);
            m_InfuseTips.Find("Bg/Tital").GetComponent<StateRoot>().CurrentState = (int)item.type - 1;
            //
            string res = string.Empty;
            List<int> keys = new List<int>(item.battleAttri.GetKeys());
            for(int i = 0; i < keys.Count;i++)
            {
                res += AttributeDefine.Get(keys[i]).attrName + " +" + item.battleAttri.Get(keys[i]);
                if (i != keys.Count - 1)
                    res += "\n";
            }
            m_InfuseTips.Find("Bg/Content").GetComponent<Text>().text = res.ToString();

            this.OnSelectedInfusePoint(item);
        }

        void OnSelectedInfusePoint(TrumpInfused infusedData)
        {
            if(m_SelectedInfused != infusedData)
            {
                if(m_SelectedInfused != null)
                {
                    Transform root = m_Grids.GetChild(m_SelectedInfused.posid - 1);
                    root.Find("Light").gameObject.SetActive(false);
                }

                m_SelectedInfused = infusedData;

                if (m_SelectedInfused != null)
                {
                    Transform root = m_Grids.GetChild(m_SelectedInfused.posid - 1);
                    root.Find("Light").gameObject.SetActive(true);

                    this.ResetMarUI(m_SelectedInfused);
                }
            }
        }

        void ResetMarUI(TrumpInfused infusedData)
        {
            //注灵状态按钮判断
            if (infusedData != null && m_TrumpMgr.CanInfuse(m_TrumpId,infusedData.infusedid))
            {
                m_ButtonSR.CurrentState = 0;
                //材料设置
                m_MarBtn1.gameObject.SetActive(false);
                m_MarBtn2.gameObject.SetActive(false);
                if (infusedData.material.list.Length >= 1)
                {
                    m_MarBtn1.gameObject.SetActive(true);

                    int id = infusedData.material.list[0].id;
                    int count = infusedData.material.list[0].count;
                    m_MarBtn1.onClick.RemoveAllListeners();
                    m_MarBtn1.onClick.AddListener(() => { this.OnShowItemTips(id); });

                    int hasCount = hotApp.my.GetModule<HotPackageModule>().GetItemCount(id);
                    m_MarBtn1.transform.Find("Text").GetComponent<Text>().text = GlobalSymbol.ToUT(string.Format("#[{0}]{1}/{2}#n", hasCount >= count ? "G2" : "R1", hasCount,count));
                    Helper.SetSprite(m_MarBtn1.transform.Find("Icon").GetComponent<Image>(), Item.Get(id).icon);
                    Helper.SetSprite(m_MarBtn1.transform.Find("Quality").GetComponent<Image>(), QualitySourceConfig.Get(Item.Get(id).quality).icon);
                }
                if(infusedData.material.list.Length >= 2)
                {
                    m_MarBtn2.gameObject.SetActive(true);

                    int id = infusedData.material.list[1].id;
                    int count = infusedData.material.list[1].count;
                    m_MarBtn2.onClick.RemoveAllListeners();
                    m_MarBtn2.onClick.AddListener(() => { this.OnShowItemTips(infusedData.material.list[1].id); });

                    int hasCount = hotApp.my.GetModule<HotPackageModule>().GetItemCount(id);
                    m_MarBtn2.transform.Find("Text").GetComponent<Text>().text = GlobalSymbol.ToUT(string.Format("#[{0}]{1}/{2}#n", hasCount >= count ? "G2" : "R1", hasCount, count));
                    Helper.SetSprite(m_MarBtn2.transform.Find("Icon").GetComponent<Image>(), Item.Get(id).icon);
                    Helper.SetSprite(m_MarBtn2.transform.Find("Quality").GetComponent<Image>(), QualitySourceConfig.Get(Item.Get(id).quality).icon);
                }
            }
            else if(m_TrumpMgr.CanTasteUp(m_TrumpId))
            {
                TrumpAttribute attribute = m_TrumpMgr.GetTrumpAttribute(m_TrumpId);
                m_ButtonSR.CurrentState = 1;
                int hasCost = (int)App.my.localPlayer.silverShellValue;
                int need = TrumpSoul.soulDic[attribute.id][attribute.tastelv].cost;
                m_TasteUpCost.text = need > hasCost ? GlobalSymbol.ToUT(string.Format("#[R1]{0}", need)) : need.ToString();

                if (TrumpSoul.soulDic[attribute.id][attribute.tastelv].soullv > attribute.souldata.lv)
                    m_TasteLvNeed.text = GlobalSymbol.ToUT(string.Format("#[R1]需要潜修达到{0}级#n", TrumpSoul.soulDic[attribute.id][attribute.tastelv].soullv));
                else
                    m_TasteLvNeed.text = string.Empty;
            }
            else if (m_TrumpMgr.MaxTaste(m_TrumpId))//满级
            {
                m_ButtonSR.CurrentState = 2;
            }
            else
            {
                m_ButtonSR.CurrentState = 3;
            }
        }

        void OnShowItemTips(int itemId)
        {
            UICommon.ShowItemTips(itemId);
        }

        void OnTasteEvent()
        {
            TrumpAttribute attribute = m_TrumpMgr.GetTrumpAttribute(m_TrumpId);
            if (TrumpSoul.soulDic[attribute.id][attribute.tastelv].soullv > attribute.souldata.lv)
            {
                return;
            }
            m_Page.Event.FireEvent(EventID.Trumps_TasteUp, m_TrumpId);
        }

        void OnInfuseEvent()
        {
            if (m_SelectedInfused == null)
                return;
            TrumpsInfusedRequest request = new TrumpsInfusedRequest();
            request.trumpid = m_TrumpId;
            request.infusedid = m_SelectedInfused.infusedid;
            m_Page.Event.FireEvent(EventID.Trumps_InfusedUp, request);
        }
    }
}
#endif