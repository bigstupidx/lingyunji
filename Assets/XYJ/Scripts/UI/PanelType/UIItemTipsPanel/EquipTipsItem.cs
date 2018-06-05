#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;

    class SoulData
    {
        public string name;
        public List<string> items;
    }

    class EquipTipsData : BaseTipsData
    {
        public string score; // 装备评分
        public string jobLimit; // 职业限制
        public int strengthlevel; // 强化等级
        public int strengthMaxLevel; // 强化最大等级
        public List<string> souls = new List<string>();
        public List<string> baseAtts = new List<string>(); // 基础属性
        public List<string> randomAtts = new List<string>(); // 随机属性
        public List<string> effectAtts = new List<string>(); // 特效属性
        public List<string> refineAtts = new List<string>(); // 炼化属性
        public List<SoulData> soulAtts = new List<SoulData>(); // 附魂属性

        public int currentDurable; // 当前耐久值
        public int maxDurable; // 最大耐久值

        public bool isCanReset; // 是否可重铸
        public bool isCanRefine; // 是否可炼化
    }

    [System.Serializable]
    class EquipTipsItem : BaseTipsItem
    {
        Text score; // 装备评分
        Text job; // 装备职业限制

        public Button eyeBtn { get; private set; }

        // 强化等级模版项
        GameObject strengthTemplate; // 强化等级项根结点

        class SoulItem
        {
            public GameObject root; // 根结点
            public Image icon; // 图标

            public SoulItem(GameObject root)
            {
                this.root = root;
                icon = root.transform.Find("BagItem/Icon").GetComponent<Image>();
            }

            public void Set(string image)
            {
                root.SetActive(true);
                xys.UI.Helper.SetSprite(icon, image);
            }
        }

        Image wearFlag; // 已经装备标识
        GameObject soulParent; // 附魂根结点 
        GameObject soulTemplate; // 附魂模块项
        List<SoulItem> soulItems = new List<SoulItem>();

        // 基础属性项
        GameObject baseAttRoot; // 基础属性根结点
        Text baseAttTemplate; // 基础属性的文本描述模版项
        List<Text> baseAttList = new List<Text>(); // 基础属性项
        Color defaultAttrColor;
        // 炼化属性项
        GameObject refineAttRoot; // 炼化属性根结点
        Text refineAttTemplate; // 炼化属性的文本描述模版项
        List<Text> refineAttList = new List<Text>(); // 炼化属性项

        // 附魂属性项
        GameObject soulAttRoot; // 附魂属性项
        GameObject soulAttTemplate; // 附魂模块项

        class SoulAttItem
        {
            public GameObject root;
            public Text name;
            public Text template;
            public List<Text> atts = new List<Text>();
            Color defaultAttrColor;
            public SoulAttItem(GameObject r)
            {
                root = r;
                name = root.transform.Find("name/name").GetComponent<Text>();
                template = root.transform.Find("list/1").GetComponent<Text>();
                defaultAttrColor = template.color;
                atts.Add(template);
            }

            public void Set(string n, List<string> attitems)
            {
                root.SetActive(true);
                name.text = n;
                SetTextList(attitems, null, template, atts, defaultAttrColor);
            }
        }

        List<SoulAttItem> soulAttItems = new List<SoulAttItem>();

        // 耐久
        Text durable; // 耐久值
        Text canRefinery; // 是否可炼化重铸提示

        Dictionary<int, string> m_EquipPartStr;

        public void OnInit()
        {
            var rt = root.transform;
            Transform rtm = null;
            icon = (rtm = rt.Find("content/headerArea/QualityBg/Icon")) == null ? null : rtm.GetComponent<Image>();
            quality = (rtm = rt.Find("content/headerArea/QualityBg")) == null ? null : rtm.GetComponent<Image>();
            bind = (rtm = rt.Find("content/headerArea/QualityBg/bindImg")) == null ? null : rtm.GetComponent<Image>();
            type = (rtm = rt.Find("content/headerArea/equipType")) == null ? null : rtm.GetComponent<Text>();
            name = (rtm = rt.Find("content/headerArea/Name")) == null ? null : rtm.GetComponent<Text>();
            score = (rtm = rt.Find("content/headerArea/Score")) == null ? null : rtm.GetComponent<Text>();
            levelLimit = (rtm = rt.Find("content/headerArea/Level")) == null ? null : rtm.GetComponent<Text>();
            job = (rtm = rt.Find("content/headerArea/Job")) == null ? null : rtm.GetComponent<Text>();
            strengthTemplate = (rtm = rt.Find("content/strengthArea/list/1")) == null ? null : rtm.gameObject;
            soulParent = (rtm = rt.Find("content/holeArea")) == null ? null : rtm.gameObject;
            soulTemplate = (rtm = rt.Find("content/holeArea/list/item")) == null ? null : rtm.gameObject;
            baseAttRoot = (rtm = rt.Find("content/attrArea/Scroll View/Content/baseAttrArea")) == null ? null : rtm.gameObject;
            baseAttTemplate = (rtm = rt.Find("content/attrArea/Scroll View/Content/baseAttrArea/list/1")) == null ? null : rtm.GetComponent<Text>();
            refineAttRoot = (rtm = rt.Find("content/attrArea/Scroll View/Content/refineAttrArea")) == null ? null : rtm.gameObject;
            refineAttTemplate = (rtm = rt.Find("content/attrArea/Scroll View/Content/refineAttrArea/list/item0")) == null ? null : rtm.GetComponent<Text>();
            soulAttRoot = (rtm = rt.Find("content/attrArea/Scroll View/Content/soulAttrArea")) == null ? null : rtm.gameObject;
            soulAttTemplate = (rtm = rt.Find("content/attrArea/Scroll View/Content/soulAttrArea/list/item0")) == null ? null : rtm.gameObject;
            durable = (rtm = rt.Find("content/bottomArea/item/Durable")) == null ? null : rtm.GetComponent<Text>();
            canRefinery = (rtm = rt.Find("content/bottomArea/item/limit")) == null ? null : rtm.GetComponent<Text>();            canRefinery = (rtm = rt.Find("content/bottomArea/item/limit")) == null ? null : rtm.GetComponent<Text>();            eyeBtn = (rtm = rt.Find("content/headerArea/eye")) == null ? null : rtm.GetComponent<Button>();            wearFlag = (rtm = rt.Find("content/headerArea/putOn")) == null ? null : rtm.GetComponent<Image>();
            soulItems.Add(new SoulItem(soulTemplate));
            baseAttList.Add(baseAttTemplate);
            refineAttList.Add(refineAttTemplate);
            soulAttItems.Add(new SoulAttItem(soulAttTemplate));
            defaultAttrColor = baseAttTemplate.color;
            m_EquipPartStr = new Dictionary<int, string>();

            m_EquipPartStr.Add(1, "武器");
            m_EquipPartStr.Add(2, "头盔");
            m_EquipPartStr.Add(3, "胸甲");
            m_EquipPartStr.Add(4, "手套");
            m_EquipPartStr.Add(5, "护腿");
            m_EquipPartStr.Add(6, "鞋子");
            m_EquipPartStr.Add(7, "项链");
            m_EquipPartStr.Add(8, "戒指");
            m_EquipPartStr.Add(9, "饰品");

            root.SetActive(false);
        }

        // 设置基础信息
        void SetBaseInfo(EquipTipsData etd)
        {
            base.Set(etd);
            score.text = etd.score;
            job.text = etd.jobLimit;

            canRefinery.gameObject.SetActive(true);
            durable.text = string.Format("耐久 {0} / {1}", etd.currentDurable, etd.maxDurable);
            if (!etd.isCanRefine && etd.isCanReset)
            {
                canRefinery.text = GlobalSymbol.ToUT(string.Format("#[R]{0}#n", "不可炼化"));
            }
            else if (!etd.isCanReset && etd.isCanRefine)
            {
                canRefinery.text = GlobalSymbol.ToUT(string.Format("#[R]{0}#n", "不可重铸"));
            }
            else if (!etd.isCanRefine && !etd.isCanReset)
            {
                canRefinery.text = GlobalSymbol.ToUT(string.Format("#[R]{0}#n", "不可炼化 / 重铸"));
            }
            else
            {
                canRefinery.text = "";
                canRefinery.gameObject.SetActive(false);
            }
        }

        // 设置强化等级
        void SetStrengthLevel(int level, int maxLevel)
        {
            var parent = strengthTemplate.transform.parent;

            int count = strengthTemplate.transform.parent.childCount;
            for (int i = count; i < maxLevel; ++i)
            {
                UITools.AddChild(strengthTemplate.transform.parent.gameObject, strengthTemplate);
            }

            if (count > maxLevel)
            {
                for (int i = maxLevel; i < count; ++i)
                    parent.GetChild(i).transform.gameObject.SetActive(false);
            }

            for (int i = level; i < maxLevel; ++i)
            {
                parent.GetChild(i).transform.gameObject.SetActive(true);
                parent.GetChild(i).Find("StrengthenPoint").GetComponent<Image>().gameObject.SetActive(false);
            }
            for (int i = 0; i < level; ++i)
            {
                parent.GetChild(i).transform.gameObject.SetActive(true);
                parent.GetChild(i).Find("StrengthenPoint").GetComponent<Image>().gameObject.SetActive(true);
            }
        }

        // 设置附魂的数据
        void SetSouls(List<string> souls)
        {
            if (souls.Count == 0)
            {
                soulParent.SetActive(false);
            }
            else
            {
                soulParent.SetActive(true);

                var parentGo = soulTemplate.transform.parent.gameObject;
                while (soulItems.Count < souls.Count)
                {
                    var child = UITools.AddChild(parentGo, soulTemplate);
                    soulItems.Add(new SoulItem(child));
                }

                for (int i = 0; i < souls.Count; ++i)
                {
                    soulItems[i].Set(souls[i]);
                }
                for (int i = souls.Count; i < soulItems.Count; ++i)
                {
                    soulItems[i].root.SetActive(false);
                }
            }
        }

        static void SetTextList(List<string> texts, GameObject root, Text template, List<Text> items, Color color, int startIndex = 0)
        {
            if (root != null)
                root.SetActive((texts.Count == 0) && (startIndex == 0) ? false : true);

            while (true)
            {
                if (items.Count < (texts.Count + startIndex))
                {
                    var item = UITools.AddChild(template.transform.parent.gameObject, template.gameObject).GetComponent<Text>();
                    items.Add(item);
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < texts.Count; ++i)
            {
                items[i + startIndex].text = texts[i];
                items[i + startIndex].color = color;
                items[i + startIndex].gameObject.SetActive(true);
            }

            for (int i = (texts.Count + startIndex); i < items.Count; ++i)
            {
                items[i].gameObject.SetActive(false);
            }
        }

        // 基础属性列表
        void SetBaseAttList(List<string> texts, int startIndex, Color color)
        {
            SetTextList(texts, baseAttRoot, baseAttTemplate, baseAttList, color, startIndex);
        }
        // 炼化属性列表
        void SetRefineAttList(List<string> texts, Color color)
        {
            SetTextList(texts, refineAttRoot, refineAttTemplate, refineAttList, color);
        }

        void SetSoulAtt(List<SoulData> sds)
        {
            if (sds == null || sds.Count == 0)
            {
                soulAttRoot.SetActive(false);
                return;
            }

            soulAttRoot.SetActive(true);
            while (true)
            {
                if (soulAttItems.Count < sds.Count)
                {
                    soulAttItems.Add(new SoulAttItem(UITools.AddChild(soulAttTemplate.transform.parent.gameObject, soulAttTemplate)));
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < sds.Count; ++i)
            {
                soulAttItems[i].Set(sds[i].name, sds[i].items);
            }

            for (int i = sds.Count; i < soulAttItems.Count; ++i)
                soulAttItems[i].root.SetActive(false);
        }

        public void Set(EquipTipsData etd)
        {
            SetBaseInfo(etd);
            SetStrengthLevel(etd.strengthlevel, etd.strengthMaxLevel);
            SetSouls(etd.souls);
            SetBaseAttList(etd.baseAtts, 0, defaultAttrColor);
            SetBaseAttList(etd.randomAtts, etd.baseAtts.Count, Config.NameColorConfig.ToColor(Config.ColorConfig.GetIndexByName(Config.kvClient.Get("EquipTipsRandomAttrColor").value)));
            SetBaseAttList(etd.effectAtts, etd.randomAtts.Count + etd.baseAtts.Count, Config.NameColorConfig.ToColor(Config.ColorConfig.GetIndexByName(Config.kvClient.Get("EquipTipsEffectAttrColor").value)));
            SetRefineAttList(etd.refineAtts, Config.NameColorConfig.ToColor(Config.ColorConfig.GetIndexByName(Config.kvClient.Get("EquipTipsRefineAttrColor").value)));
            SetSoulAtt(etd.soulAtts);

            root.SetActive(true);
        }

        public string GetEquipPartStr(int partId)
        {
            if (!m_EquipPartStr.ContainsKey(partId))
                return "";
            return m_EquipPartStr[partId];
        }

        public void SetWearFlagImage(bool isShow)
        {
            wearFlag.gameObject.SetActive(isShow);
        }
    }
}
#endif