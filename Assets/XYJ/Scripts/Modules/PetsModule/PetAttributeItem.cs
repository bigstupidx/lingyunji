#if !USE_HOT
namespace xys.hot
{
    using battle;
    using UnityEngine;
    using Config;
    using UnityEngine.UI;

    class PetAttributeItem
    {
        public PetAttributeItem(int id, Button target, System.Action<int, AttributeDefine,Transform> clickHandle)
        {
            this.id = id;
            this.skin = target;

            prop = Config.AttributeDefine.Get(id);
            m_AttributeName = skin.transform.Find("Name").GetComponent<Text>();
            valueText = skin.transform.Find("value").GetComponent<Text>();
            if (skin.transform.Find("addValue") != null)
                m_AddValueText = skin.transform.Find("addValue").GetComponent<Text>();

            m_AttributeName.text = prop.attrName;
            this.clickHandle = clickHandle;
            target.onClick.AddListener(this.clickObjHandle);
        }

        private void clickObjHandle()
        {
            if (clickHandle != null)
                clickHandle(id, prop, skin.transform);
        }
        public void RefreshNum(PetObj petobj)
        {
            valueText.text = AttributeDefine.GetValueStr(prop.id, petobj.uibattleAttri.Get(prop.id)); 
        }

        public void RefreshNum(PetObj petobj, BattleAttri pAddArr)
        {
            if (m_AddValueText == null)
                return;

            valueText.text = AttributeDefine.GetValueStr(prop.id, pAddArr.Get(prop.id));

            if (pAddArr.Get(prop.id) > petobj.uibattleAttri.Get(prop.id))
            {
                double val = System.Math.Ceiling(pAddArr.Get(prop.id)) - System.Math.Ceiling(petobj.uibattleAttri.Get(prop.id));
                m_AddValueText.text = "+" + AttributeDefine.GetValueStr(prop.id, val);
            }
            else
                m_AddValueText.text = string.Empty;
        }

        int id;
        private Button skin;
        AttributeDefine prop;
        Text valueText;
        Text m_AttributeName;
        Text m_AddValueText;
        System.Action<int, AttributeDefine,Transform> clickHandle;
    }
}
#endif