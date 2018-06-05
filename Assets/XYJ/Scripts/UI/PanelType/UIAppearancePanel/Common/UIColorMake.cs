#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;
using NetProto;
namespace xys.hot.UI
{
    [AutoILMono]
    class UIColorMake
    {
        [SerializeField]
        GameObject m_leftBtnsRoot;

        [SerializeField]
        Button m_backBtn;
        [SerializeField]
        Button m_resetBtn;
        [SerializeField]
        Transform m_colorBtnRoot;
        [SerializeField]
        GameObject m_colorMake;
        [SerializeField]
        Slider m_hueSlider;

        [SerializeField]
        Slider m_saturationSlider;
        [SerializeField]
        Text m_saturationText;

        [SerializeField]
        Slider m_valueSlider;
        [SerializeField]
        Text m_valueText;

        [SerializeField]
        Button m_paintBtn;

        [SerializeField]
        ILMonoBehaviour m_ilclothContent;
        UIClothContent m_clothContent;

        [SerializeField]
        Image m_itemIcon;
        [SerializeField]
        Text m_itemName;
        [SerializeField]
        Text m_type;


        [SerializeField]
        GameObject m_paintTab;
        [SerializeField]
        Image m_material;
        [SerializeField]
        Text m_num;

        //模块事件注册工具
        public hot.Event.HotObjectEventAgent m_eventAgent;
        hot.Event.HotObjectEventSet m_hotEventagent;
        HotAppearanceModule module;
        RoleDisguiseHandle m_disguiseHandle;
        ClothHandle m_clothHandle;

        private List<UIEditColorBtn> m_colorBtnList = new List<UIEditColorBtn>();

        public ClothItem m_clothItem = null;
        public int m_curColorIdx = 0;
        public bool m_isChangedRole = false;  
        private NetProto.ApColor m_appColor = new NetProto.ApColor();
        void Awake()
        {
            module = hotApp.my.GetModule<HotAppearanceModule>();
            m_hotEventagent = module.Event;
            if (m_paintBtn != null)
            {
                m_paintBtn.onClick.AddListener(OnClickPaint);
            }
            if (m_ilclothContent != null)
            {
                m_clothContent = m_ilclothContent.GetObject() as UIClothContent;
            }
            if (m_colorBtnRoot != null)
            {
                int childCount = m_colorBtnRoot.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    Transform tempTran = m_colorBtnRoot.GetChild(i);
                    if (tempTran == null) return;
                    ILMonoBehaviour tempIL = tempTran.GetComponent<ILMonoBehaviour>();
                    if (tempIL == null) return;
                    UIEditColorBtn tempItem = tempIL.GetObject() as UIEditColorBtn;
                    m_colorBtnList.Add(tempItem);
                }
            }
            if (m_backBtn != null)
            {
                m_backBtn.onClick.AddListener(Close);
            }
            if(m_resetBtn!=null)
            {
                m_resetBtn.onClick.AddListener(ResetColor);
            }
        }
        public void Open(ClothItem clothItem)
        {
            if (m_eventAgent!=null)
            {
                m_eventAgent.Subscribe(EventID.AP_RefreshUI, RefreshUI);
            }        
            m_colorMake.gameObject.SetActive(true);
            m_leftBtnsRoot.gameObject.SetActive(false);
            m_paintTab.SetActive(false);
            m_backBtn.gameObject.SetActive(true);
            if (clothItem == null) return;
            m_clothItem = clothItem;
            m_disguiseHandle = module.GetMgr().GetDisguiHandle();
            m_clothHandle = module.GetMgr().GetClothHandle();
            InitTab();
            SetItem();
        }
        void Close()
        {
            m_hueSlider.onValueChanged.RemoveAllListeners();
            m_saturationSlider.onValueChanged.RemoveAllListeners();
            m_valueSlider.onValueChanged.RemoveAllListeners();
            m_leftBtnsRoot.gameObject.SetActive(true);
            m_colorMake.gameObject.SetActive(false);
            m_backBtn.gameObject.SetActive(false);
            m_clothContent.OpenContent();
        }
        void InitTab()
        {
            
            int curColorCount = m_clothItem.GetColorList().Count;
            for (int i = 0; i < curColorCount; i++)
            {
                m_colorBtnList[i].Set(i, 0, m_clothItem.GetColorList()[i], OnClickDelete, OnClickItem);
            }
            for (int i = curColorCount; i < m_colorBtnList.Count; i++)
            {
                m_colorBtnList[i].Set(i);
            }

            if (m_clothItem.m_id == m_clothHandle.m_roleClothData.m_clothId)
            {
                m_curColorIdx = m_clothHandle.m_roleClothData.m_curColor;
                m_colorBtnList[m_curColorIdx].m_stateRoot.SetCurrentState(2, false);
            }
            else
            {
                m_curColorIdx = 0;

            }
            
            Color colorUsing = m_clothItem.GetColorList()[m_curColorIdx];
            float h = 0f;
            float s = 0f;
            float v = 0f;
            Color.RGBToHSV(colorUsing, out h, out s, out v);
          
            m_hueSlider.value = h;
            m_hueSlider.onValueChanged.AddListener((value) =>
            {
                m_appColor.h = (int)(value*360);
                OpenMaterialTab();
                RefreshClothColor();
            });
          
            m_saturationSlider.value = s;
            m_saturationText.text = "+"+ ((int)(s * 100)).ToString(); 
            m_saturationSlider.onValueChanged.AddListener((value) =>
            {
                m_appColor.s = value * 3f;
                m_saturationText.text = "+" + ((int)(value * 100)).ToString();
                OpenMaterialTab();
                RefreshClothColor();
            });

            
            m_valueSlider.value = v;
            m_valueText.text = "+" + ((int)(v * 100)).ToString();
            m_valueSlider.onValueChanged.AddListener((value) =>
            {
                m_appColor.v = value * 3f;
                m_valueText.text = "+" + ((int)(value * 100)).ToString();
                OpenMaterialTab();
                RefreshClothColor();
            });
        }

        void RefreshClothColor()
        {
            Color temp = Color.HSVToRGB(m_appColor.h / 360f, m_appColor.s / 3f, m_appColor.v / 3f);
            m_disguiseHandle.SetClothColor(temp);
        }
        void OpenMaterialTab()
        {
            if (m_paintTab.activeInHierarchy) return;
            m_paintTab.SetActive(true);
            Config.kvCommon materialNeed = Config.kvCommon.Get("ClothingDyeing");
            int id = 0;
            int num = 0;
            ClothItem.StrToTwoInt(materialNeed.value, out id, out num);
            int curOwn = App.my.localPlayer.GetModule<PackageModule>().GetItemCount(id);

            Config.Item tempItem = Config.Item.Get(id);
            Helper.SetSprite(m_material, tempItem.icon);
            if(curOwn< num)
            {
                m_num.color = Color.red;
            }
            else
            {
                m_num.color = Color.green;
            }
            m_num.text = curOwn.ToString() + "/" + num.ToString();
        }
        void SetItem()
        {
            if (m_clothItem != null)
            {
                Helper.SetSprite(m_itemIcon, m_clothItem.GetIconName());
            }
            m_itemName.text = m_clothItem.GetName();
            int type = m_clothItem.GetFashionType();
            if (type == 1)
            {
                m_type.text = "【时装】";
            }
            else
            {
                m_type.text = "【门派】";
            }

        }
        void OnClickItem(UIEditColorBtn item)
        {
            Color colorUsing = m_clothItem.GetColorList()[item.m_index];
            float h = 0f;
            float s = 0f;
            float v = 0f;
            Color.RGBToHSV(colorUsing, out h, out s, out v);

            m_hueSlider.value = h;
            m_saturationSlider.value = s;
            m_saturationText.text = "+" + ((int)(s * 100)).ToString();
            m_valueSlider.value = v;
            m_valueText.text = "+" + ((int)(v * 100)).ToString();
            m_paintTab.SetActive(false);
        }
        //删除颜色方案
        void OnClickDelete(UIEditColorBtn item)
        {
            //显示再次确认框
            xys.UI.Dialog.TwoBtn m_TwoBtn = new xys.UI.Dialog.TwoBtn();
            m_TwoBtn = xys.UI.Dialog.TwoBtn.Show("", "确定删除选定的颜色方案？",
                "取消", () => { return false; },
                "确定", () =>
                {
                    DeleteColor(item.m_index);
                    return false;
                }, true, true, () => { m_TwoBtn = null; }
                );
        }
        
        void DeleteColor(int index)
        {
            DeleteColorReq input = new NetProto.DeleteColorReq();
            input.itemId = m_clothItem.m_id;
            input.colorIndex = index - 1;
            m_hotEventagent.FireEvent(EventID.Ap_DeleteColor, input);
            SystemHintMgr.ShowTipsHint(7601);
            //如果是正在使用的颜色被删除，重置为默认色
            if(m_clothItem.m_id==m_clothHandle.m_roleClothData.m_clothId)
            {
                if(index== m_clothHandle.m_roleClothData.m_curColor)
                {
                    m_clothHandle.m_roleClothData.m_curColor = 0;
                    m_clothItem.m_curColor = 0;
                }
                else if(index< m_clothHandle.m_roleClothData.m_curColor)
                {
                    m_clothHandle.m_roleClothData.m_curColor -=1;
                    m_clothItem.m_curColor  -=1;
                }  
            }
            else
            {
                if(index==m_clothItem.m_curColor)
                {
                    m_clothItem.m_curColor = 0;
                }
                else if(index< m_clothItem.m_curColor)
                {
                    m_clothItem.m_curColor -= 1;
                }
            }            
        }
        void OnClickPaint()
        {
            if(m_clothItem.GetColorList().Count>=4)
            {
                //显示再次确认框
                xys.UI.Dialog.TwoBtn m_TwoBtn = new xys.UI.Dialog.TwoBtn();
                m_TwoBtn = xys.UI.Dialog.TwoBtn.Show("", "3个颜色保存方案已满，请先清理再染色。若【继续染色】将不会保存新方案。",
                    "取消", () => { return false; },
                    "继续染色", () =>
                    {
                        //
                        return false;
                    }, true, true, () => { m_TwoBtn = null; }
                    );
                return;
            }
            PaintCloth();
        }

        void PaintCloth()
        {
            Config.kvCommon materialNeed = Config.kvCommon.Get("ClothingDyeing");
            int id = 0;
            int num = 0;
            ClothItem.StrToTwoInt(materialNeed.value, out id, out num);
            int curOwn = App.my.localPlayer.GetModule<PackageModule>().GetItemCount(id);
            if(curOwn<num)
            {
                ItemTipsPanel.Param param = new ItemTipsPanel.Param();
                param.itemId = id;
                App.my.uiSystem.ShowPanel(PanelType.UIItemTipsPanel, param, true);
            }
            else
            {
                App.my.localPlayer.GetModule<PackageModule>().UseItemById(id, num); 

                PaintRequest input = new PaintRequest();
                input.itemId = m_clothItem.m_id;

                input.color.h = m_appColor.h;
                input.color.s = m_appColor.s;
                input.color.v = m_appColor.v;

                m_hotEventagent.FireEvent(EventID.Ap_PaintCloth, input);
                SystemHintMgr.ShowTipsHint(7603);
            }
            
        }

        void RefreshUI()
        {
            Debug.Log("染色界面");
            InitTab();
            m_paintTab.SetActive(false);
        }

       
        void ResetColor()
        {
            //将服饰设置为默认色
            OnClickItem(m_colorBtnList[0]);
            SystemHintMgr.ShowTipsHint(7602);
        }
    }
}
#endif