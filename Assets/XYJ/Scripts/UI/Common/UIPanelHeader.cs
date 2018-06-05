#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace xys.UI
{
    public class UIPanelHeader : MonoBehaviour
    {
        RectTransform _startParent;
        RectTransform startParent
        {
            get
            {
                if (_startParent==null)
                    _startParent = xys.App.my.uiSystem.PanelRoot;
                return _startParent;
            }
        }

        GameObject m_Root;
        GameObject Root
        {
            get
            {
                return m_Root;
            }
        }

        [System.Serializable]
        struct Money
        {
            public NetProto.AttType type;
            public Text text;
            public Button button;

            public void Update()
            {
                text.text = App.my.localPlayer.GetMoney(type).ToString();
            }
        }

        [SerializeField]
        Money[] moneys;

        ObjectEventAgent Event { get; set; }

        // 状态，目前分两种
        // 1 正常显示
        // 2 隐藏返回按钮
        [SerializeField]
        UI.State.StateRoot stateRoot;

        public void Init()
        {
            m_Root = gameObject;
            _startParent = (RectTransform)transform.parent;
            Root.SetActive(false);
            state = State.Hide;

            if (openOrCloseAnimator == null)
                openOrCloseAnimator = GetComponent<Animator>();
            m_PanelAnimator = new PanelAnimator(openOrCloseAnimator);

            for (int i = 0; i < moneys.Length; ++i)
            {
                var money = moneys[i];
                moneys[i].button.onClick.AddListener(() => { OnClickMoneyBtn(money); });
            }

            Event = new ObjectEventAgent(App.my.localPlayer.eventSet);
        }

        public enum State
        {
            Showing, // 显示中
            Show, // 正常的显示状态下
            Hiding, // 开始播放界面关闭的动画
            Hide, // 隐藏了,面板都成未激活状态了
        }

        [SerializeField]
        protected Animator openOrCloseAnimator; // 打开或关闭面板动画

        protected PanelAnimator m_PanelAnimator;

        string panelType; // 谁打开了我

        public string panel_type { get { return panelType; } }

        State m_State = State.Hide;

        [SerializeField]
        Text panelName;

        public State state
        {
            get { return m_State; }
            protected set { m_State = value; }
        }

        void RegistEvent()
        {
            Event.Subscribe(NetProto.AttType.AT_SilverShell, SetMoneyData);
            Event.Subscribe(NetProto.AttType.AT_GoldShell, SetMoneyData);
            Event.Subscribe(NetProto.AttType.AT_FairyJade, SetMoneyData);
            Event.Subscribe(NetProto.AttType.AT_JasperJade, SetMoneyData);
        }

        void OnClickMoneyBtn(Money money)
        {

        }

        public void Restore()
        {
            transform.SetParent(startParent);
        }

        void OnDisable()
        {
            panelType = string.Empty;

            state = State.Hide;
            Event.Release();
        }

        private void OnEnable()
        {
            RegistEvent();
            SetMoneyData();
        }

        void OnDestory()
        {
            Debug.LogError("UIPanelHeader OnDestroy!");
            Event.Release();
        }

        public IEnumerator Show(UIPanelBase panel, bool isPlayAnim)
        {
            if (panelName != null)
            {
                panelName.text = panel.PanelName;
            }

            switch (state)
            {
            case State.Hide:
                {
                    panelType = panel.panelType;

                    RectTransform rootRect = Root.GetComponent<RectTransform>();

                    rootRect.SetParent(panel.panelHeadRoot);
                    {
                        rootRect.localScale = Vector3.one;
                        rootRect.localPosition = Vector3.zero;
                        rootRect.anchorMin = Vector2.zero;
                        rootRect.anchorMax = Vector2.one;

                        rootRect.offsetMin = Vector2.zero;
                        rootRect.offsetMax = Vector2.zero;
                    }

                    m_PanelAnimator.OnBeginShow(isPlayAnim);

                    stateRoot.CurrentState = panel.panelHeadState;

                    // 打开关播放动画
                    Root.SetActive(true);

                    if (isPlayAnim && openOrCloseAnimator != null)
                    {
                        state = State.Showing;
                        yield return m_PanelAnimator.PlayAnim(true, () => { return (int)state; });
                    }

                    state = State.Show;
                }
                break;

            case State.Show:
            case State.Showing:
                {
                    if (panelType == panel.panelType)
                    {
                        // 已经是打开状态下了，无视吧1
                        m_PanelAnimator.OnBeginShow(isPlayAnim);
                    }
                    else
                    {
                        // 另一个界面打开了
                        xys.App.my.uiSystem.StartCoroutine(Hide(false));

                        xys.App.my.uiSystem.StartCoroutine(Show(panel, isPlayAnim));
                    }
                }
                break;
            case State.Hiding:
                {
                    // 这种情况下，先关闭吧
                    // 处于关闭当中，又要重新打开
                    xys.App.my.uiSystem.StartCoroutine(Hide(false));

                    xys.App.my.uiSystem.StartCoroutine(Show(panel, isPlayAnim));
                }
                break;
            default:
                Debug.LogErrorFormat("Show State:{0} panel:{1} isPlayAnim:{2}", state, panel.panelType, isPlayAnim);
                break;
            }
        }

        public IEnumerator Hide(bool isPlayAnim)
        {
            switch (state)
            {
            case State.Show:
                {
                    if (isPlayAnim && openOrCloseAnimator != null)
                    {
                        state = State.Hiding;
                        IEnumerator itor = m_PanelAnimator.PlayAnim(false, () => { return (int)state; });
                        while (itor.MoveNext())
                            yield return 0;

                        if (state != State.Hiding)
                            yield break;
                    }

                    Restore();
                    Root.SetActive(false);
                    state = State.Hide;
                }
                break;
            case State.Showing:
                {
                    Restore();
                    Root.SetActive(false);
                    state = State.Hide;
                }
                break;

            case State.Hide:
                break;

            case State.Hiding:
                {
                    Restore();
                    Root.SetActive(false);
                    state = State.Hide;
                }
                break;
            default:
                {
                    Debug.LogErrorFormat("Hide:{0} isPlayAnim:{1}", state, isPlayAnim);
                }
                break;
            }
        }

        // 关闭界面
        public void OnClosePanel()
        {
            UIPanelBase pb = UISystem.Get(panelType);
            if (pb != null)
            {
                if (pb.IsCanCloseByBtn())
                {
                    pb.Hide(true);
                }
            }
        }

        public bool isStartParent
        {
            get
            {
                if (Root.transform.parent == startParent)
                    return true;
                return false;
            }
        }

        // 设置货币
        void SetMoneyData()
        {
            foreach (var item in moneys)
                item.Update();
        }
    }
}
