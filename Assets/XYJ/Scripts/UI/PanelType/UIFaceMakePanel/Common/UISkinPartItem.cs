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
    class UISkinPartItem
    {
        [SerializeField]
        StateRoot m_TitleSR;
        public StateRoot titleSR { get { return m_TitleSR; } }
        [SerializeField]
        Text m_TitleText;
        [SerializeField]
        Button m_ResetBtn;

        [SerializeField]
        Button m_BigResetBtn;

        [SerializeField]
        Transform m_NomalRoot;
        [SerializeField]
        Transform m_ColorItemRoot;

        [SerializeField]
        GameObject m_ContentGO;


        [SerializeField]
        Transform m_ColorRoot;

        [SerializeField]
        Slider m_ColorSlider;
        [SerializeField]
        Text m_ColorNumText;

        [SerializeField]
        Slider m_ThickSlider;
        [SerializeField]
        Text m_ThickNumText;

        [SerializeField]
        Slider m_AlphaSlider;
        [SerializeField]
        Text m_AlphaNumText;

        [SerializeField]
        Transform m_TextureItemRoot;


        GameObject m_ItemPref = null;
        Transform m_ItemSource = null;

        private UIFaceMakePanel m_FaceMakeupModule = null;
        public int m_TabIndex = 0;
        public int m_PartIndex=0;
        public int m_currentTexItemIndex = 0;

        public List<UITextureItem> m_TextureItemList = new List<UITextureItem>();

        private System.Action<UISkinPartItem> m_ClickEvent = null; 
        private System.Action<UISkinPartItem> m_StateChangeEvent = null;

        static int[] transfer = { 4, 1, 2, 3, 0, 5, 6 };

        public void Set(int tabIndex,int partIndex,string title,UIFaceMakePanel faceMakeModule, System.Action<UISkinPartItem> clickEvent, System.Action<UISkinPartItem> stateChangeEvent)
        {
            InitItem();
            m_TabIndex = tabIndex;
            m_PartIndex = partIndex;
            m_TitleText.text = title;
            m_ClickEvent = clickEvent;
            m_StateChangeEvent = stateChangeEvent;
            m_FaceMakeupModule = faceMakeModule;
        }
        void InitItem()
        {
            m_FaceMakeupModule = null;
            m_TabIndex = 0;
            m_PartIndex = 0;
            m_currentTexItemIndex = 0;
            m_TitleSR.CurrentState = 0;
            m_ClickEvent = null;
            m_StateChangeEvent = null;
            m_TextureItemList.Clear();
            if (m_TitleSR != null)
            {
                m_TitleSR.onClick.RemoveAllListeners();
                m_TitleSR.onStateChange.RemoveAllListeners();
               
                m_TitleSR.onClick.AddListener(OnClick);
                m_TitleSR.onStateChange.AddListener(OnStateChange);                        
            }
        }
        void OnClick()
        {
            if (m_ClickEvent != null)
            {
                m_ClickEvent(this);
            }
        }
        void OnStateChange()
        {
            if (m_StateChangeEvent != null)
            {
                m_StateChangeEvent(this);
            }
        }
        /// <summary>
        /// index参数是选项卡的序号，也是部位的序号，分部位的序号是this.m_PartIndex,在创建PartItem时Set
        /// </summary>
        /// <param name="index"></param>
        /// <param name="merge"></param>
        public void OpenPart()
        {
            if (m_ContentGO!=null)
            {
                m_ContentGO.SetActive(true);
                
            }
            
            //创建TextureItem

            RoleSkinHandle merge= m_FaceMakeupModule.GetRoleSkinHandle();
            RoleSkinPart skinPart = merge.GetConfig();
            string[] keys = skinPart.GetTabsKeys(m_TabIndex);
            RoleSkinUnit skinUnit = skinPart.Get(keys[m_PartIndex]);
         
            RoleSkinUnitData roleSkinUnit = merge.GetData().Get(keys[m_PartIndex]);
            if(1==m_TabIndex)
            {
                if (m_ResetBtn != null)
                {
                    m_ResetBtn.gameObject.SetActive(true);
                    m_ResetBtn.GetComponentInChildren<Text>().text = "重置" + skinUnit.name;
                    m_ResetBtn.onClick.AddListener(Reset);
                }
            }
            else
            {
                if(m_BigResetBtn!=null)
                {
                    m_BigResetBtn.gameObject.SetActive(true);
                    m_BigResetBtn.GetComponentInChildren<Text>().text = "重置" + skinUnit.name;
                    m_BigResetBtn.onClick.AddListener(Reset);
                }
            }
            


            m_currentTexItemIndex = roleSkinUnit.texStyle;

            int ItemCount = skinUnit.texStyles.Count;
            
            if(ItemCount > 0)
            {
                m_TextureItemRoot.gameObject.SetActive(true);
            }
            if(ItemCount==1)
            {
                ItemCount = skinUnit.colorStyles.Count;
            }
            for(int i=0;i< ItemCount; i++)
            {

                if (m_TextureItemRoot == null) return;
                if (m_TabIndex==2)
                {
                    m_ItemSource = m_ColorItemRoot;
                }
                else
                {
                    m_ItemSource = m_NomalRoot;
                }
                if (m_ItemSource == null) return;
                int curChildCount = m_ItemSource.childCount;
                if (curChildCount > 0)
                {
                    m_ItemPref = m_ItemSource.GetChild(0).gameObject;
                }
                if (m_ItemPref == null) return;
                GameObject tempObj = null;
                if (i < curChildCount)
                {
                    tempObj = m_ItemSource.GetChild(i).gameObject;
                }
                else
                {
                    tempObj = GameObject.Instantiate(m_ItemPref);
                }

                int existCount = m_TextureItemRoot.transform.childCount;
                
                tempObj.SetActive(true);
                tempObj.transform.SetParent(m_TextureItemRoot,false);
                tempObj.transform.localScale = Vector3.one;

                if (tempObj == null) return;
                ILMonoBehaviour ILObj = tempObj.GetComponent<ILMonoBehaviour>();
                if (ILObj == null) return;
                UITextureItem item = (UITextureItem)ILObj.GetObject();
                if(skinUnit.texStyles.Count==1)
                {
                    item.Set(i, skinUnit.colorStyles[i].RBGFloatColor, OnClick, OnStateChange);
                }
                else
                {
                    item.Set(i, 0,OnClick, OnStateChange, skinUnit.texStyles[i].previewUIName);
                    
                }
                
                m_TextureItemList.Add(item);
            }


            m_TextureItemList[m_currentTexItemIndex].m_StateRoot.SetCurrentState(1, false);

            m_ColorSlider.gameObject.SetActive(skinUnit.h);

            if(skinUnit.h)
            {
                if(m_ColorRoot!=null)
                {
                    m_ColorRoot.gameObject.SetActive(true);
                }
                m_ColorSlider.onValueChanged.RemoveAllListeners();
                m_ColorSlider.value = roleSkinUnit.h / 360f;
                SetValue(m_ColorNumText, m_ColorSlider.value); 
                m_ColorSlider.onValueChanged.AddListener((_value) =>
                {
                    SetValue(m_ColorNumText, _value);
                    var vv = (int)(_value * 360f);
                    if (vv != roleSkinUnit.h)
                    {
                        roleSkinUnit.h = vv;
                        merge.SetUnitData(roleSkinUnit);
                        UIFaceMakePanel.m_NeedComfirm = true;
                    }

                });
            }

            m_ThickSlider.gameObject.SetActive(skinUnit.s);
            if(skinUnit.s)
            {
                if (m_ColorRoot != null)
                {
                    m_ColorRoot.gameObject.SetActive(true);
                }
                m_ThickSlider.onValueChanged.RemoveAllListeners();
                m_ThickSlider.value = roleSkinUnit.s / 3f;
                SetValue(m_ThickNumText, m_ThickSlider.value);
                m_ThickSlider.onValueChanged.AddListener((_value) =>
                {
                    roleSkinUnit.s = _value * 3f;
                    merge.SetUnitData(roleSkinUnit);
                    SetValue(m_ThickNumText, _value);
                    UIFaceMakePanel.m_NeedComfirm = true;
                });
            }

            m_AlphaSlider.gameObject.SetActive(skinUnit.v);
            if (skinUnit.v)
            {
                if (m_ColorRoot != null)
                {
                    m_ColorRoot.gameObject.SetActive(true);
                }
                m_AlphaSlider.onValueChanged.RemoveAllListeners();
                
                m_AlphaSlider.value = roleSkinUnit.v / 3f;
                SetValue(m_AlphaNumText, m_AlphaSlider.value);
                m_AlphaSlider.onValueChanged.AddListener((_value) =>
                {
                    roleSkinUnit.v = _value * 3f;
                    merge.SetUnitData(roleSkinUnit);
                    SetValue(m_AlphaNumText, _value);
                    UIFaceMakePanel.m_NeedComfirm = true;
                });
            }


        }
        void SetValue(Text _text,float _value)
        {
            if (_value > 0)
            {
                _text.text = "+" + ((int)(_value * 100)).ToString();
            }
            else
            {
                _text.text = "-" + ((int)(_value * 100)).ToString();
            }
        }
        public void ClosePart()
        {
            m_TextureItemList.Clear();
            if(m_TitleSR!=null)
            {
                m_TitleSR.gameObject.SetActive(true);
            }
            if (m_ContentGO != null)
            {
                m_ContentGO.SetActive(false);
            }
            if (m_TextureItemRoot != null)
            {
                int texItemCount = m_TextureItemRoot.childCount;
                while (m_TextureItemRoot.childCount != 0)
                {
                    m_TextureItemRoot.GetChild(0).SetParent(m_ItemSource);
                }
            }
            if (m_ColorRoot != null)
            {
                int colorCount = m_ColorRoot.childCount;
                for (int i = 0; i < colorCount; i++)
                {
                    m_ColorRoot.GetChild(i).gameObject.SetActive(false);
                }
                m_ColorRoot.gameObject.SetActive(false);
            }

            if (m_ResetBtn!=null)
            {
                m_ResetBtn.onClick.RemoveAllListeners();
                m_ResetBtn.gameObject.SetActive(false);
            }
            if(m_BigResetBtn!=null)
            {
                m_BigResetBtn.onClick.RemoveAllListeners();
                m_BigResetBtn.gameObject.SetActive(false);
            }
        }

        //重置单个部位
        public void  Reset()
        {
            RoleSkinHandle merge = m_FaceMakeupModule.GetRoleSkinHandle();
            int idx = 0;
            if(m_TabIndex>=2)
            {
                idx = transfer[m_TabIndex + m_PartIndex + 2];
            }
            else
            {
                idx = transfer[m_TabIndex + m_PartIndex];
            }
            
            merge.ResetByIndex(idx);
            UIFaceMakePanel.m_NeedComfirm = true;
            ResetUI();
        }
        public void ResetUI()
        {
            if (m_FaceMakeupModule == null)
            {
                Debug.Log("找不到数据模块");
                return;
            }
            RoleSkinHandle merge = m_FaceMakeupModule.GetRoleSkinHandle();
            RoleSkinPart skinPart = merge.GetConfig();
            string[] keys = skinPart.GetTabsKeys(m_TabIndex);

            RoleSkinUnit skinUnit = skinPart.Get(keys[m_PartIndex]);
            RoleSkinUnitData roleSkinUnit = merge.GetData().Get(keys[m_PartIndex]);


            //刷新界面
            if (m_TitleSR.CurrentState == 1)
            {
                
                int tempTextrue = roleSkinUnit.texStyle;
                if (skinUnit.texStyles.Count == 1)
                {
                    tempTextrue = roleSkinUnit.colorStyle;
                }
                if (skinUnit.h)
                {
                    m_ColorSlider.value = roleSkinUnit.h / 360f;
                }
                if (skinUnit.s)
                {
                    m_ThickSlider.value = roleSkinUnit.s / 3f;
                }
                if (skinUnit.v)
                {
                    m_AlphaSlider.value = roleSkinUnit.v / 3f;
                }
                m_TextureItemList[m_currentTexItemIndex].m_StateRoot.SetCurrentState(0, false);
                m_currentTexItemIndex = tempTextrue;
                m_TextureItemList[m_currentTexItemIndex].m_StateRoot.SetCurrentState(1, false);
            }
        }
        void OnClick(UITextureItem item)
        {         
            if(m_currentTexItemIndex==item.m_Index)
            {
                return;
            }
            else
            {
                m_TextureItemList[m_currentTexItemIndex].m_StateRoot.SetCurrentState(0, true);
                item.m_StateRoot.SetCurrentState(1, true);
            }

        }
        void OnStateChange(UITextureItem item)
        {
            if(item.m_StateRoot.CurrentState==0)
            {
                return;
            }
            else
            {
                m_currentTexItemIndex = item.m_Index;


                RoleSkinHandle merge = m_FaceMakeupModule.GetRoleSkinHandle();

                RoleSkinPart skinPart = merge.GetConfig();
                string[] keys = skinPart.GetTabsKeys(m_TabIndex);
                RoleSkinUnit skinUnit = skinPart.Get(keys[m_PartIndex]);
                RoleSkinUnitData value = merge.GetData().Get(keys[m_PartIndex]);

                if(skinUnit.texStyles.Count==1)
                {
                    value.colorStyle = item.m_Index;
                    value.h = skinUnit.colorStyles[item.m_Index].h;
                    value.s = skinUnit.colorStyles[item.m_Index].s;
                    value.v = skinUnit.colorStyles[item.m_Index].v;
                    ResetUI();
                }
                else
                {
                    value.texStyle = item.m_Index;
                }         
                merge.SetUnitData(value);

                UIFaceMakePanel.m_NeedComfirm = true;
            }
        }
        
    }
}
#endif