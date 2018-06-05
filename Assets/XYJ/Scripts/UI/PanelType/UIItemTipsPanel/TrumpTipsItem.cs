#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;

    // 法宝技能
    class TrumpSkill
    {
        public string name; // 名字
        public string icon; // 图标
        public int level; // 等级
    }

    // 连携属性
    class TrumpUnionAtt
    {
        public string icon; // 图标
        public string text; // 描述
    }

    class TrumpTipsData : BaseTipsData
    {
        public string jobLimit; // 职业限制
        public List<string> attList = new List<string>(); // 属性列表
        public List<TrumpSkill> skillList = new List<TrumpSkill>(); // 技能列表
        public List<TrumpUnionAtt> unionAttList = new List<TrumpUnionAtt>(); // 连携属性
    }

    // 法宝
    [System.Serializable]
    class TrumpTipsItem : BaseTipsItem
    {
        public Text jobLimit; // 职业限制

        public Button unionBtn; // 连携按钮
        public GameObject attTemplate; // 属性列表
        public GameObject skillTemplate; // 属性列表

        public GameObject unionRoot; // 连携根结点
        public GameObject unionItemTemplate; // 连携项模版

        class Att
        {
            public GameObject root;
            public Text value;

            public Att(GameObject r)
            {
                root = r;
                value = root.transform.Find("Name").GetComponent<Text>();
            }
        }

        List<Att> attList = new List<Att>(); // 属性列表

        class Skill
        {
            public GameObject root;
            public Image icon;
            public Text name;
            public Text level;

            public Skill(GameObject r)
            {
                root = r;
                var rt = root.transform;
                icon = rt.Find("Icon").GetComponent<Image>();
                name = rt.Find("Name").GetComponent<Text>();
                level = rt.Find("Level").GetComponent<Text>();
            }

            public void Set(TrumpSkill data)
            {
                xys.UI.Helper.SetSprite(icon, data.icon);
                name.text = data.name;
                level.text = data.level.ToString();

                root.SetActive(true);
            }
        }

        List<Skill> skillList = new List<Skill>();

        class UnionItem
        {
            public GameObject root;
            public Image icon; // 连携图标
            public Text text; // 连携描述

            public UnionItem(GameObject r)
            {
                root = r;
                var rt = root.transform;
                icon = rt.Find("Bg/Icon").GetComponent<Image>();
                text = rt.Find("Bg/Name").GetComponent<Text>();
            }

            public void Set(TrumpUnionAtt data)
            {
                xys.UI.Helper.SetSprite(icon, data.icon);
                text.text = data.text;

                root.SetActive(true);
            }
        }

        List<UnionItem> unionList = new List<UnionItem>();

        public void OnInit()
        {
            root.SetActive(false);
            attList.Add(new Att(attTemplate));
            skillList.Add(new Skill(skillTemplate));
            unionList.Add(new UnionItem(unionItemTemplate));
            unionBtn.onClick.AddListener(OnClickUnion);
        }

        void SetAtt(List<string> atts)
        {
            while (attList.Count < atts.Count)
                attList.Add(new Att(UITools.AddChild(attTemplate.transform.parent.gameObject, attTemplate).gameObject));

            for (int i = 0; i < atts.Count; ++i)
            {
                attList[i].value.text = atts[i];
                attList[i].root.SetActive(true);
            }

            for (int i = atts.Count; i < attList.Count; ++i)
                attList[i].root.SetActive(false);
        }

        void SetSkills(List<TrumpSkill> skills)
        {
            while (skillList.Count < skills.Count)
                skillList.Add(new Skill(UITools.AddChild(skillTemplate.transform.parent.gameObject, skillTemplate).gameObject));

            for (int i = 0; i < skills.Count; ++i)
            {
                skillList[i].Set(skills[i]);
            }

            for (int i = skills.Count; i < skillList.Count; ++i)
                skillList[i].root.SetActive(false);
        }

        void SetUnion(List<TrumpUnionAtt> unions)
        {
            unionBtn.gameObject.SetActive(unions.Count == 0 ? false : true);
            unionRoot.SetActive(false);

            if (unions.Count == 0)
                return;

            while (unionList.Count < unions.Count)
                unionList.Add(new UnionItem(UITools.AddChild(unionItemTemplate.transform.parent.gameObject, unionItemTemplate)));

            for (int i = 0; i < unions.Count; ++i)
                unionList[i].Set(unions[i]);

            for (int i = unions.Count; i < unionList.Count; ++i)
                unionList[i].root.SetActive(false);
        }

        public void Set(TrumpTipsData ptd)
        {
            base.Set(ptd);
            jobLimit.text = ptd.jobLimit;
            SetAtt(ptd.attList);
            SetSkills(ptd.skillList);
            SetUnion(ptd.unionAttList);
            root.SetActive(true);
        }

        void OnClickUnion()
        {
            unionRoot.SetActive(!unionRoot.activeSelf);
        }
    }
}
#endif