#if !USE_HOT

namespace xys.hot.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using xys.UI;
    using xys.battle;
    using UnityEngine.UI;
    using Config;
    using WXB;
    using global::UI;
    using wTimer;

    public class UIAttributeTipsParam
    {
        public string title;//面板首项名称
        public int titleOldValue;
        public int titleNewValue;

        public BattleAttri oldAttri;//旧属性
        public BattleAttri newAttri;//新属性
    }

    class UIAttributeTipsPanel : HotPanelBase
    {
        class Param
        {
            public string name;
            public double oldvalue;
            public double newValue;
        }
        public UIAttributeTipsPanel(UIHotPanel panel) : base(panel) { }

        public UIAttributeTipsPanel() : base(null) { }

        [SerializeField]
        UIGroup m_ItemGroup;
        [SerializeField]
        int m_MaxCount = 9;
        [SerializeField]
        float m_Duration = 2.0f;
        [SerializeField]
        float m_PosX = 0.0f;
        [SerializeField]
        float m_PosY = 0.0f;

        [SerializeField]
        float m_NumDuration = 0.2f;

        Coroutine m_ScreenCoroutine;
        Coroutine m_NumCoroutine;

        SimpleTimer m_Timer;        //计时器
        int m_TimerHandler;

        Dictionary<int,Param> m_Params;
        protected override void OnInit()
        {
            m_Params = new Dictionary<int, Param>();

            m_Timer = new SimpleTimer(App.my.mainTimer);
            m_TimerHandler = 0;
        }

        protected override void OnShow(object p)
        {
            UIAttributeTipsParam param = p as UIAttributeTipsParam;
            if(param == null)
            {
                App.my.uiSystem.HidePanel(PanelType.UIAttributeTipsPanel,false);
                return;
            }
            //关闭之前未关闭的协程
            if (m_ScreenCoroutine != null)
                parent.StopCoroutine(m_ScreenCoroutine);
            m_ScreenCoroutine = null;
            if (m_NumCoroutine != null)
                parent.StopCoroutine(m_NumCoroutine);
            m_NumCoroutine = null;
            //关闭之前为关闭的定时器
            if (m_TimerHandler != 0)
                m_Timer.Cannel(m_TimerHandler);
            //
            m_Params.Clear();
            //如果标题不为空，加入数组
            if(param.title != string.Empty)
            {
                Param item = new Param();
                item.name = param.title;
                item.oldvalue = param.titleOldValue;
                item.newValue = param.titleNewValue;
                m_Params.Add(0, item);
            }
            //计算需要显示的属性项
            foreach (int attributeIndex in param.oldAttri.GetKeys())
            {
                if (!m_Params.ContainsKey(attributeIndex))
                {
                    Param item = new Param();
                    item.name = AttributeDefine.Get(attributeIndex).attrName;
                    item.oldvalue = param.oldAttri.Get(attributeIndex);
                    item.newValue = param.newAttri.Get(attributeIndex);
                    m_Params.Add(attributeIndex, item);
                }
            }
            foreach (int attributeIndex in param.newAttri.GetKeys())
            {
                if (!m_Params.ContainsKey(attributeIndex))
                {
                    Param item = new Param();
                    item.name = AttributeDefine.Get(attributeIndex).attrName;
                    item.oldvalue = param.oldAttri.Get(attributeIndex);
                    item.newValue = param.newAttri.Get(attributeIndex);
                    m_Params.Add(attributeIndex, item);
                }
            }
            if (m_Params.Count == 0)
            {
                App.my.uiSystem.HidePanel(PanelType.UIAttributeTipsPanel, false);
                return;
            }
            //初始化
            List<int> keys = new List<int>(m_Params.Keys);
            keys.Sort();
            int maxIndex = keys.Count > m_MaxCount ? m_MaxCount : keys.Count;
            this.m_ItemGroup.SetCount(maxIndex);
            for (int i = 0; i < maxIndex; i++)
            {
                this.SetUI(m_ItemGroup.transform.GetChild(i), m_Params[keys[i]], keys[i]);
            }


            m_ScreenCoroutine = parent.StartCoroutine(this.Close());
            m_TimerHandler = m_Timer.Register(0.5f, 1 , ShowNewAttribute);
        }

        protected override void OnHide()
        {
            if (m_ScreenCoroutine != null)
                parent.StopCoroutine(m_ScreenCoroutine);
            m_ScreenCoroutine = null;
            
            if (m_NumCoroutine != null)
                parent.StopCoroutine(m_NumCoroutine);
            m_NumCoroutine = null;
        }

        IEnumerator Close()
        {
            float time = m_Duration;
            while(time > 0.0f)
            {
                time -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            App.my.uiSystem.HidePanel(PanelType.UIAttributeTipsPanel);
            yield break;
        }

        void SetUI(Transform item, Param param,int attributeIndex = 0)
        {
            item.Find("Name").GetComponent<Text>().text = param.name;
            item.Find("SourceValue").GetComponent<Text>().text = attributeIndex == 0? param.oldvalue.ToString():AttributeDefine.GetValueStr(attributeIndex, AttributeDefine.Get(attributeIndex).uiShowType == AttributeDefine.UIShowType.Percent ? param.oldvalue * 0.01f : param.oldvalue);
            item.Find("TargetValue").GetComponent<Text>().text = attributeIndex == 0 ? param.oldvalue.ToString() : AttributeDefine.GetValueStr(attributeIndex, AttributeDefine.Get(attributeIndex).uiShowType == AttributeDefine.UIShowType.Percent ? param.oldvalue * 0.01f : param.oldvalue);

            Color color;
            ColorConst.Get(param.oldvalue > param.newValue ? "R1" : "G2", out color);
            color.a = 0;
            item.Find("TargetValue").GetComponent<Text>().color = color;

            item.Find("DownArrow").gameObject.SetActive(false);
            item.Find("UpArrow").gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 数值渐变出现
        /// </summary>
        /// <param name="param"></param>
        void ShowNewAttribute()
        {
            List<int> keys = new List<int>(m_Params.Keys);
            keys.Sort();
            Transform item = null;
            Param param = null;
            for(int i = 0; i < m_ItemGroup.transform.childCount;i++)
            {
                param = m_Params[keys[i]];
                item = m_ItemGroup.transform.GetChild(i);
                if (param.oldvalue > param.newValue)
                {
                    item.Find("DownArrow").gameObject.SetActive(true);
                    item.Find("UpArrow").gameObject.SetActive(false);
                    UGUITweenAlpha.Begin(item.Find("DownArrow").gameObject, 0.1f, 1);
                }
                else if(param.oldvalue < param.newValue)
                {
                    item.Find("DownArrow").gameObject.SetActive(false);
                    item.Find("UpArrow").gameObject.SetActive(true);
                    UGUITweenAlpha.Begin(item.Find("UpArrow").gameObject, 0.1f, 1);
                }
                UGUITweenAlpha.Begin(item.Find("TargetValue").gameObject, 0.1f, 1);
            }
            m_NumCoroutine = parent.StartCoroutine(this.RefreshNum());
        }
        /// <summary>
        /// 滚动数值
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerator RefreshNum()
        {
            List<int> keys = new List<int>(m_Params.Keys);
            keys.Sort();
            //数值滚动
            Transform item = null;
            float time = 0.0f;
            float value = 0.0f;
            while(time < m_NumDuration)
            {
                time += Time.deltaTime;
                for (int i = 0; i < m_ItemGroup.transform.childCount; i++)
                {
                    item = m_ItemGroup.transform.GetChild(i);
                    value = Mathf.Lerp((float)m_Params[keys[i]].oldvalue, (float)m_Params[keys[i]].newValue, time / m_NumDuration);
                    item.Find("TargetValue").GetComponent<Text>().text = keys[i] == 0 ? value.ToString() : AttributeDefine.GetValueStr(keys[i], AttributeDefine.Get(keys[i]).uiShowType == AttributeDefine.UIShowType.Percent ? value * 0.01f : value);
                }
                yield return 0;
            }
            yield break;
        }
    }
}
#endif
