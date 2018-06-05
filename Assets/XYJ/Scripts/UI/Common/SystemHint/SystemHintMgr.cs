
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using xys.battle;
using xys.UI.SystemHint;
using Config;

namespace xys.UI
{
    public enum HintEventID
    {
        Level1Finish,
    }

    [Serializable]
    public class SystemHintMgr : MonoBehaviour
    {
        [SerializeField]
        GameObject m_UIHintPrefab;

        private Queue<string> m_HintTextQue;
        private GameObject m_HintGoNewest;
        private HintGo m_HintButtom;

        private float m_lastTriggerTime;
        private const float m_minCutTime = 0.8f;

        EmptyGraphic m_ModalGraphic;
        EmptyGraphic modalGraphic
        {
            get
            {
                if (m_ModalGraphic == null)
                    m_ModalGraphic = GetComponent<EmptyGraphic>();

                return m_ModalGraphic;
            }
        }

        RectTransform rectTransform;

        public bool isModalGraphicOpen
        {
            get { return modalGraphic.isActiveAndEnabled; }
            set { modalGraphic.enabled = value; }
        }

#if UNITY_EDITOR
        void Awake()
        {
            if (App.my == null)
            {
                gameObject.SetActive(false);
            }
        }
#endif

        public void Init()
        {
            rectTransform = GetComponent<RectTransform>();
            m_HintTextQue = new Queue<string>();

            m_UIHintPrefab.SetActive(false);
            m_UIHintPrefab.transform.SetParent(rectTransform);

            isModalGraphicOpen = false;

            App.my.eventSet.Subscribe(EventID.FinishLoadScene, this.OnFinishLoadScene);
        }


        private void OnFinishLoadScene()
        {
            m_lastTriggerTime = BattleHelp.timePass;
        }

        public static void ShowHint(string text)
        {
            App.my.uiSystem.systemHintMgr.ShowHintVO(text);
        }

        public static void ShowHint(string desc, params object[] args)
        {
            if (null != args)
            {
                string text = string.Format(desc, args);
                App.my.uiSystem.systemHintMgr.ShowHintVO(text);
            }
            else
            {
                ShowHint(desc);
            }
        }

        public static void ShowTipsHint(int id, params object[] args)
        {
            TipsContent config = TipsContent.Get(id);
            if (null == config)
            {
                Debuger.LogError("提示不存在 " + id);
                return;
            }

            string text = config.des;
            if (string.IsNullOrEmpty(text))
            {
                Debuger.LogError("提示内容为空" + id);
                return;
            }
            ShowHint(text, args);
        }

        public static void ShowTipsHint(string name, params object[] args)
        {
            TipsContent config = TipsContent.GetByName(name);
            if (null == config)
            {
                Debuger.LogError("提示不存在:" + name);
                return;
            }
            string text = config.des;
            if(string.IsNullOrEmpty(text))
            {
                Debuger.LogError("提示内容为空:" + name);
                return;
            }
            ShowHint(text, args);
        }

        public static void ShowTipsHint(int id)
        {
            TipsContent config = TipsContent.Get(id);
            if (null == config)
            {
                Debuger.LogError("提示不存在 " + id);
                return;
            }

            string text = config.des;
            if (string.IsNullOrEmpty(text))
            {
                Debuger.LogError("提示内容为空" + id);
                return;
            }
            ShowHint(text);
        }

        private void ShowHintVO(string text)
        {
            m_HintTextQue.Enqueue(text);
            TriggerHint();
        }

        public GameObject ShowHintGo(string text)
        {
            GameObject go = Instantiate(m_UIHintPrefab);
            go.transform.SetParent(rectTransform);
            go.transform.localScale = Vector3.one;
            UISystemHint hint = go.GetComponent<UISystemHint>();
            hint.OnShow(text);

            TweenAlpha ta = go.GetComponent<TweenAlpha>();
            ta.enabled = false;
            return go;
        }

        public void TriggerHint()
        {
            float interval = BattleHelp.timePass - m_lastTriggerTime - m_minCutTime;
            if (interval < 0)
            {
                xys.App.my.mainTimer.Register(-interval, 1, TriggerHint);
                return;
            }

            string[] texts = m_HintTextQue.ToArray();
            if (texts.Length <= 0) return;
            m_lastTriggerTime = BattleHelp.timePass;

            CreateNewHint();

            texts = m_HintTextQue.ToArray();
            if (texts.Length > 0)
                xys.App.my.mainTimer.Register(m_minCutTime, 1, TriggerHint);
        }

        public void CreateNewHint()
        {
            m_HintGoNewest = ShowHintGo(m_HintTextQue.Dequeue());
        }
    }
}