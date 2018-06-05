#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [AutoILMono]
    class UISkinPartTab
    {

        [SerializeField]
        GameObject m_SkinPartItemPref;
        [SerializeField]
        Transform m_TabRoot;

        [SerializeField]
        Button m_ResetBtn;
        public List<UISkinPartItem> m_SkinPartList = new List<UISkinPartItem>();

        public int m_TabIndex = 0;
        public int m_currentPartIndex = 0;
        private UIFaceMakePanel m_FaceMakeModule;
        
        public void OpenTab(int index, UIFaceMakePanel faceMakeModule)
        {
            InitTab();
            m_FaceMakeModule = faceMakeModule;
            
            RoleSkinHandle merge= faceMakeModule.GetRoleSkinHandle();
            //读取角色部位配置信息
            RoleSkinPart data = merge.GetConfig();
            //存储当前页面序号
            m_TabIndex = index;
            //获取部位的名称信息
            string[] keys = data.GetTabsKeys(index);

            //SkinPartItem的数量
            int childPartCount = keys.Length;
            //创建分部位
            for(int i=0;i< childPartCount;i++)
            {
                if(m_TabRoot!=null)
                {
                    //对应部位的配置文件
                    var key = keys[i];
                    RoleSkinUnit partData = data.Get(key);

                    //角色对应部位的值
                    RoleSkinUnitData value = merge.GetData().Get(key);
                    
                    GameObject tempObj = null;
                    int count = m_TabRoot.childCount;
                    if(i<count)
                    {
                        Transform tempTran = m_TabRoot.GetChild(i);
                        if(tempTran!=null)
                        {
                            tempObj = tempTran.gameObject;
                        }
                    }
                    else
                    {
                        tempObj = GameObject.Instantiate(m_SkinPartItemPref);
                    }
                    if (tempObj == null) return;
                    tempObj.SetActive(true);
                    tempObj.transform.SetParent(m_TabRoot, false);
                    tempObj.transform.localScale = Vector3.one;
                    ILMonoBehaviour ILObj = tempObj.GetComponent<ILMonoBehaviour>();
                    if (ILObj == null) return;
                    UISkinPartItem item = (UISkinPartItem)ILObj.GetObject();
                    item.Set(m_TabIndex,i, partData.name,m_FaceMakeModule,this.OnClick,this.OnStateChange);
                    m_SkinPartList.Add(item);
                }               
            }
            m_SkinPartList[m_currentPartIndex].titleSR.SetCurrentState(1, true);
            
            if(m_SkinPartList.Count==1)
            {
                m_SkinPartList[0].titleSR.gameObject.SetActive(false);
            }
            else if(m_SkinPartList.Count>1)
            {
                if (m_ResetBtn != null)
                {
                    m_ResetBtn.gameObject.SetActive(true);
                    m_ResetBtn.GetComponentInChildren<Text>().text = "重置" + data.tabsName[m_TabIndex];
                    Debug.Log(data.tabsName[m_TabIndex]);
                    m_ResetBtn.onClick.AddListener(OnClickReset);
                }
            }
        }
        //关闭前的清理
        public void CloseTab()
        {
            if (m_TabRoot!=null)
            {
                int count = m_TabRoot.childCount;
                for (int i = 0; i < count; i++)
                {
                    m_TabRoot.GetChild(i).gameObject.SetActive(false);
                }
            } 
            for(int i=0;i<m_SkinPartList.Count;i++)
            {
                m_SkinPartList[i].ClosePart();
            }
            if (m_ResetBtn != null)
            {
                m_ResetBtn.gameObject.SetActive(false);
                m_ResetBtn.onClick.RemoveAllListeners();
            }
        }
        void InitTab()
        {
            m_SkinPartList.Clear();
            m_TabIndex = 0;
            m_currentPartIndex = 0;
            m_FaceMakeModule=null;
        }
        //重置按钮
        void OnClickReset()
        {
            for(int i=0;i<m_SkinPartList.Count;i++)
            {
                m_SkinPartList[i].Reset();
            }
        }
        void OnClick(UISkinPartItem item)
        {
            if(m_currentPartIndex==item.m_PartIndex)
            {
                if(item.titleSR.CurrentState==0)
                {
                    item.titleSR.SetCurrentState(1, true);
                }
                else
                {
                    item.titleSR.SetCurrentState(0, true);
                }
                
            }
            else
            {
                if(m_SkinPartList[m_currentPartIndex].titleSR.CurrentState==1)
                {
                    m_SkinPartList[m_currentPartIndex].titleSR.SetCurrentState(0, true);
                }
                m_currentPartIndex = item.m_PartIndex;
                item.titleSR.SetCurrentState(1, true);
            }
        }
        void OnStateChange(UISkinPartItem item)
        {
            if(item.titleSR.CurrentState==0)
            {
                //关闭该部位选项
                item.ClosePart();
                Debug.Log("关闭Part " + item.m_PartIndex);
            }
            else
            {
                //打开该部位选项
                item.OpenPart();
                Debug.Log("打开Part " + item.m_PartIndex);
            }
        }
    }
}
#endif
