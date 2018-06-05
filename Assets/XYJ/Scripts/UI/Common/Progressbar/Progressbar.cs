using Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace xys.UI
{

    public class ProgressMgr
    {
        Progressbar m_bar;
        public ProgressMgr( GameObject root )
        {
            RALoad.LoadPrefab("UIProgressbar", (go, para) =>
            {
                m_bar = go.GetComponent<Progressbar>();
                go.transform.SetParent(root.transform);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;            
            },null);
        }

        //技能进度条
        public ProgressBase PlaySkillCasting(ProgressData data)
        {
            if (m_bar == null)
                return null;
            return m_bar.AddLogic(new ProgressSkill(), data, m_bar.m_skillPrefabs);
        }

        //道具进度条
        public ProgressBase PlayItemCasting(ProgressData data)
        {
            if (m_bar == null)
                return null;
            return m_bar.AddLogic(new ProgressItem(), data, m_bar.m_actionPrefabs);
        }
    }

    public class Progressbar : MonoBehaviour
    {
        public GameObject m_actionPrefabs;
        public GameObject m_skillPrefabs;

        Transform m_root;

        //需要支持多个进度条
        List<ProgressBase> m_list = new List<ProgressBase>();

        void Awake()
        {
            m_root = transform.Find("root");

            m_actionPrefabs.SetActive(false);
            m_skillPrefabs.SetActive(false);
        }

        GameObject CreateGO(GameObject prefabs)
        {
            GameObject go = GameObject.Instantiate(prefabs, this.m_root);
            go.SetActive(true);
            return go;
        }

        public ProgressBase AddLogic(ProgressBase logic, ProgressData data, GameObject prefabs)
        {
            m_list.Add(logic);
            logic.Play(CreateGO(prefabs).transform, data);
            return logic;
        }

        void Update()
        {
            for(int i=m_list.Count-1;i>=0;i--)
            {
                m_list[i].Update();
                if(m_list[i].m_isFinish)
                {
                    m_list[i].Stop();
                    m_list.RemoveAt(i);
                }
            }
        }
    }

    public class ProgressData
    {
        public float timeLenght;        //总时间
        public float timeBegin;         //开始了多少时间
        public Action breakEvent = null;
        public Action finishEvent = null;
        public string titleName;
        public bool rightToLeft = true;//从左到右
        public int skillid;
    }


    public class ProgressBase
    {
        protected Transform progressObj;
        protected ProgressData m_data;
        float m_curTimePass;
        public bool m_isFinish { get; private set; }

        public void Play(Transform progressObj, ProgressData data)
        {
            this.progressObj = progressObj;
            m_data = data;
            m_curTimePass = data.timeBegin;
            OnPlay();
        }

        public void Stop()
        {
            if (!m_isFinish)
            {
                if (m_data.breakEvent != null)
                    m_data.breakEvent();
            }
            m_isFinish = true;
            if (progressObj != null)
                GameObject.Destroy(progressObj.gameObject);
        }

        protected float getPercent
        {
            get
            {
               return m_curTimePass / m_data.timeLenght;
            }
        }

        public void Update()
        {
            m_curTimePass += Time.deltaTime;
            if (m_curTimePass >= m_data.timeLenght)
            {
                m_curTimePass = m_data.timeLenght;
                if (!m_isFinish)
                {
                    m_isFinish = true;
                    OnFinish();
                    if (m_data.finishEvent != null)
                        m_data.finishEvent();
                }
            }
            OnUpdate();
        }

        protected virtual void OnPlay() { }
        protected virtual void OnFinish() { }
        protected virtual void OnUpdate() { }
    }

    class ProgressSkill : ProgressBase
    {
        protected Image m_TimeImg;
        protected override void OnPlay()
        {
            SkillConfig cfg = SkillConfig.Get(m_data.skillid);

            m_TimeImg = progressObj.Find("timeImg").GetComponent<Image>();
            progressObj.Find("title").GetComponent<Text>().text = cfg.name;
            SkillIconConfig iconCfg = SkillIconConfig.Get(m_data.skillid);
            if(iconCfg == null)
                return;

            string iconName = iconCfg.icon;
            progressObj.Find("IconBg").GetComponent<Image>().SetSprite(iconName);
        }
        protected override void OnUpdate()
        {
            float percent = m_data.rightToLeft ? getPercent : 1 - getPercent;
            m_TimeImg.rectTransform.localScale = new Vector3(percent, 1, 1);
        }
    }

    class ProgressItem : ProgressBase
    {
        protected Image m_TimeImg;
        protected override void OnPlay()
        {
            m_TimeImg = progressObj.Find("timeImg").GetComponent<Image>();
            progressObj.Find("title").GetComponent<Text>().text = m_data.titleName;
        }

        protected override void OnUpdate()
        {
            float percent = m_data.rightToLeft ? getPercent : 1 - getPercent;
            m_TimeImg.rectTransform.localScale = new Vector3(percent, 1, 1);
        }
    }

}
