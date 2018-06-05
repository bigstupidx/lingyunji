#if !USE_HOT
using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;
using xys;
using NetProto;
namespace xys.hot.UI
{
    [AutoILMono]
    class UINameField
    {

        [SerializeField]
        public Button m_CloseBtn;
        [SerializeField]
        Button m_RandomBtn;
        [SerializeField]
        InputField m_NameField;
        [SerializeField]
        Button m_BuildBtn;

        public RoleDisguiseHandle m_disguiseHandle = null;


        public bool m_IsMale;
        string m_RoleName = null;
        void Awake()
        {
            if(m_BuildBtn!=null)
            {
                m_BuildBtn.onClick.AddListener(BuildBtn);
            }
            if(m_NameField!=null)
            {
                m_NameField.onValueChanged.AddListener(GetName);
            }
            if(m_RandomBtn!=null)
            {
                m_RandomBtn.onClick.AddListener(RandomName);
            }

        }
        void OnEnable()
        {
            RandomName();
        }
        void BuildBtn()
        {
            if (string.IsNullOrEmpty(m_RoleName))
            {
                SystemHintMgr.ShowTipsHint(3212);
                return;
            }

            //需要经过字库筛选
            if (!UICommon.CheckSensitiveWord(m_RoleName))
            {
                SystemHintMgr.ShowTipsHint(3203);
                return;
            };

            //名字长度限制
            int len = System.Text.Encoding.Default.GetBytes(m_RoleName).Length;
            if (len > 12)
            {
                SystemHintMgr.ShowTipsHint(3201);
                return;
            }
            if (len < 3)
            {
                SystemHintMgr.ShowTipsHint(3204);
                return;
            }


            //不可单独使用数字或特殊符号作为角色名，单纯为数字与符号组合也不行
            bool check = false;
            char[] charList = m_RoleName.ToCharArray();
            for (int i = 0; i < charList.Length; ++i)
            {
                //必须包含中文或者字母
                if(TextRegexParser.CheckStringChineseReg(charList[i])|| TextRegexParser.CheckStringCharacterReg(charList[i]))
                {
                    check = true;
                }
            }
            if (!check)
            {
                SystemHintMgr.ShowTipsHint(3213);
                return;
            }
            

            Debug.Log("角色名： " + m_RoleName);

            //创建角色
            CreateCharVo vo = m_disguiseHandle.GetCreateData();
            vo.name = m_RoleName;

            InitAppearanceData(vo);

            App.my.eventSet.FireEvent(EventID.Login_CreateRole, vo);
        }
        void GetName(string _str)
        {
            m_NameField.text = m_RoleName = IgnoreSymbol.GetFilterName(_str);
            Debug.Log("角色名： "+m_RoleName);
        }
        public void RandomName()
        {
            string name= GetRandomName(m_IsMale);
            m_RoleName = name;
            m_NameField.text = name;
            Debug.Log("m_RoleName " + m_RoleName);
        }

        string GetRandomName(bool isMale)
        {
            Dictionary<int, RoleRandomName> dataList = RoleRandomName.GetAll();

            string name = "";
            int index = 0;
            List<RoleRandomName> list = new List<RoleRandomName>();
            foreach (var itor in dataList)
            {
                list.Add(itor.Value);
            }

            if (null != list && list.Count > 0)
            {
                index = Random.Range(0, list.Count - 1);

                while (string.IsNullOrEmpty(list[index].firstName))
                {
                    index = Random.Range(0, list.Count - 1);
                }
                name += list[index].firstName;
                if (isMale)
                {
                    //男角色
                    while (string.IsNullOrEmpty(list[index].maleName))
                    {
                        index = Random.Range(0, list.Count - 1);
                    }
                    name += list[index].maleName;
                }
                else
                {
                    //女角色
                    while (string.IsNullOrEmpty(list[index].femaleName))
                    {
                        index = Random.Range(0, list.Count - 1);
                    }
                    name += list[index].femaleName;
                }
            }

            return name;
        }
        //指定默认的外观数据
        void InitAppearanceData(CreateCharVo vo)
        {
            
            Dictionary<int, FashionDefine> dataDic = FashionDefine.GetAll();
            if(dataDic.ContainsKey(vo.appearance.clothStyleId))
            {
                ClothStyleItem temp = new ClothStyleItem();

                temp.clothStyleId = vo.appearance.clothStyleId;
                temp.clothCD = System.DateTime.MaxValue.Ticks;
                
                vo.appearance.clothItems.Add(temp);
            }

            Dictionary<int, List<WeaponDefine>> weaponGroup = WeaponDefine.GetAllGroupBykey();
            if(weaponGroup.ContainsKey(vo.appearance.weaponStyleId))
            {
                WeaponStyleItem tempWeapon = new WeaponStyleItem();

                tempWeapon.weaponStyleId = vo.appearance.weaponStyleId;
                tempWeapon.weaponCD = System.DateTime.MaxValue.Ticks;
                tempWeapon.weaponMaxEffect = 1;

                vo.appearance.weapoinItems.Add(tempWeapon);
            }
           
            


        }     
    }
}
#endif