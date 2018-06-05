using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

namespace xys.UI
{
    [RequireComponent(typeof(Canvas))]
    public abstract partial class UIPanelBase : MonoBehaviour
    {
        public string panelType { get; private set; }

        [HideInInspector]
        public bool isShowGravity = false; // 是否显示重力背景

        [HideInInspector]
        public bool isShowPanelHead = false; // 是否显示面板头
        [HideInInspector]
        public int panelHeadState = 0; // 面板头的状态

        [HideInInspector]
        public bool isHideMainPanel = false; // 是否隐藏主界面

        [HideInInspector]
        public bool isRecordingPanel = false; // 是否记录面板
        int showPanelFrameCount = -1; // 显示时的桢数
        public int ShowPanelFrameCount { get { return showPanelFrameCount; } }

        [HideInInspector]
        public string panelName; // 面板的中文名称

        public string PanelName
        {
            get { return panelName; }
        }

        [SerializeField]
        [HideInInspector]
        bool isClickSpaceClosePanel = false; // 点击空白地方是否关闭界面

        public EventAgent Event { get; private set; }

        // 面板当前状态
        public enum State
        {
            Initing, // 初始化中
            Inited, // 初始化结束
            Showing, // 显示中，有些界面的显示逻辑可能需要异步
            ShowAnim, // 开始播放界面打开的动画
            Show, // 显示状态中
            Hiding, // 开始播放界面关闭的动画
            Hide, // 隐藏了,面板都成未激活状态了
        }

        public State state { get; protected set; }

        [HideInInspector]
        public bool isExclusive = false; // 排他性面板，此面板显示时，其他面板都会暂时隐藏

        // 默认的深度,0为自动调整，其他数值为固定值
        // >= 10 <= 100自动调整的层级
        // <10手动设置的层级
        // > 100 手动设置的层级
        [SerializeField]
        protected sbyte defaultDepth = 0;

        // 此面板是否固定深度
        public bool isFixedDepth { get { return defaultDepth != 0 ? true : false; } }

        [HideInInspector]
        protected PanelDepth panelDepth;

        static UIPanelHeader panelHeader
        {
            get
            {
#if UNITY_EDITOR
                return App.my == null ? null : App.my.uiSystem.PanelHeader;
#else
                return App.my.uiSystem.PanelHeader;
#endif
            }
        }

        [SerializeField]
        protected Button closeUbutton;
        [SerializeField]
        protected Button backUbutton;
        [SerializeField]
        protected Canvas canvasRoot; // 面板的根结点渲染
        protected RectTransform rectTranRoot; // 根结点

        [SerializeField]
        protected RectTransform m_PanelHeadRoot;

        [SerializeField]
        PanelCameraType panelCameraType = PanelCameraType.Default; // 默认层级

        public PanelCameraType cameraType { get { return panelCameraType; } }

        // 面板头的根结点
        public RectTransform panelHeadRoot
        {
            get
            {
                if (m_PanelHeadRoot == null)
                {
                    return cachedTransform;
                }

                return m_PanelHeadRoot;
            }
        }

        public Canvas canvas { get { return canvasRoot; } }

        // 打开与关闭的动画控制器
        [SerializeField]
        protected Animator openOrCloseAnimator; // 打开或关闭面板动画

        [FMODUnity.EventRef]
        public string openSound; // 关闭界面时的音效

        [FMODUnity.EventRef]
        public string closeSound; // 打开界面时的音效

        private GameObject cacheGo;
        private RectTransform cacheRect;

        public GameObject cachedGameObject
        {
            get
            {
                if (cacheGo == null)
                    cacheGo = gameObject;
                return cacheGo;
            }
        }

        public RectTransform cachedTransform
        {
            get
            {
                if (cacheRect == null)
                    cacheRect = GetComponent<RectTransform>();

                return cacheRect;
            }
        }

        public bool isVisible
        {
            get
            {
                switch (state)
                {
                    case State.Initing:
                    case State.Inited:
                    case State.Show:
                    case State.ShowAnim:
                    case State.Showing:
                    case State.Hiding:
                        return true;
                }

                return false;
            }
        }

#if UNITY_EDITOR
        void Awake()
        {
            if (App.my == null)
                StartCoroutine(Test());
        }

        IEnumerator Test()
        {
            yield return 0;
            yield return 0;
            IEnumerator itor = Init();
            while (itor.MoveNext())
                yield return 0;

            itor = Show(null, true);
            while (itor.MoveNext())
                yield return 0;
        }
#endif
        protected GraphicRaycaster m_Raycaster;

        protected PanelAnimator m_PanelAnimator;

        public bool isClickSpaceClose
        {
            get { return isClickSpaceClosePanel; }
            set
            {
                if (isClickSpaceClosePanel == value)
                    return;

                isClickSpaceClosePanel = value;
                OnClickHidePanel chp = cachedGameObject.GetOrAddComponent<OnClickHidePanel>();
                chp.enabled = isClickSpaceClosePanel;
            }
        }

        // 初始化面板
        public virtual IEnumerator Init()
        {
            panelType = name;
            state = State.Initing;
            canvasRoot = GetComponent<Canvas>();
            rectTranRoot = GetComponent<RectTransform>();
            m_Raycaster = Helper.CheckRaycaster(cachedGameObject) as GraphicRaycaster;

            if (isClickSpaceClosePanel)
            {
                cachedGameObject.GetOrAddComponent<OnClickHidePanel>();
            }

            if (openOrCloseAnimator == null)
            {
                openOrCloseAnimator = GetComponent<Animator>();
            }

            m_PanelAnimator = new PanelAnimator(openOrCloseAnimator);
            m_PanelAnimator.OnBeginShow(true);

            Event = new EventAgent();

            panelDepth = new PanelDepth();
            OnInit();
            IEnumerator itor = OnInitSync();
            while (itor.MoveNext())
                yield return 0;

            panelDepth.InitDefaultDepth(gameObject);
            if (isFixedDepth)
                panelDepth.AddDepth(defaultDepth);

            if (closeUbutton != null)
            {
                closeUbutton.onClick.AddListener(OnCloseBtnClick);
            }

            if (backUbutton != null)
            {
                backUbutton.onClick.AddListener(OnBackBtnClick);
            }

            SoundMgr.CheckSoundEvent(ref openSound, "ui_open_interface");
            SoundMgr.CheckSoundEvent(ref closeSound, "ui_close_interface");

            state = State.Inited;
            OnInitEnd();

            if (startBlurExcep)
            {
                isBlurExcep = true;
            }
        }

        bool isTemporaryHide = false;

        // 是否临时隐藏
        public bool temporaryHide
        {
            get { return isTemporaryHide; }
            set
            {
                if (value == temporaryHide)
                    return;

                isTemporaryHide = value;
                if (isTemporaryHide)
                {
                    // 临时隐藏,要暂时取消掉模糊效果
                    if (isBlurExcep)
                        isBlurExcep = false;
                }
                else
                {
                    if (startBlurExcep)
                    {
                        isBlurExcep = true;
                    }
                }

                Vector3 pos = cachedTransform.localPosition;
                pos.z = isTemporaryHide ? 100000 : 0;
                cachedTransform.localPosition = pos;
            }
        }

        UIBlurExcepRoot cacheExcepRoot;

        [HideInInspector]
        public bool startBlurExcep = false; // 初始是否开启模糊效果

#if UNITY_EDITOR
        [EditorField]
        public float animScaler = 1f;
#endif

        public bool isBlurExcep
        {
            get 
            {
                if (cacheExcepRoot == null)
                    return false;

                return cacheExcepRoot.enabled;
            }

            set 
            {
                if (isBlurExcep == value)
                    return;

                if (cacheExcepRoot == null)
                    cacheExcepRoot = UIBlurExcepRoot.Get(cachedGameObject);
                cacheExcepRoot.enabled = value;
            }
        }

        IEnumerator PlayAnim(bool forward)
        {
#if UNITY_EDITOR
            m_PanelAnimator.speedScaler = animScaler;
#endif
            return m_PanelAnimator.PlayAnim(forward, () => { return (int)state; });
        }

        public void SetDepth(int depth)
        {
            canvas.sortingOrder = depth;
        }

        // 增加面板的深度
        public void AddDepth(float depth)
        {
            panelDepth.AddDepth(depth);
        }

        // 默认深度
        public void DefaultDepth()
        {
            AddDepth(0);
        }

        static List<UIPanelBase> TempList = new List<UIPanelBase>();

        public float currentDepth
        {
            get { return panelDepth.currentDepth; }
        }

        public int maxDepth
        {
            get { return panelDepth.GetMaxDepth(); }
        }

        // 移到最高层
        public void MoveToFront()
        {
            TempList.Clear();
#if UNITY_EDITOR
            if (App.my != null)
                App.my.uiSystem.GetMaxDepthPanel(TempList);
#else
            App.my.uiSystem.GetMaxDepthPanel(TempList);
#endif
            if (TempList.Contains(this))
            {
                if (TempList.Count >= 2)
                {
                    AddDepth(TempList[0].currentDepth + 1);
                }
                else if (TempList[0].currentDepth <= 10)
                {
                    AddDepth(11);
                }
            }
            else if (TempList.Count == 0)
            {
                AddDepth(11);
            }
            else
            {
                AddDepth(TempList[0].currentDepth + 1);
            }
        }

        public virtual IEnumerator Show(object args, bool isPlayAnim)
        {
            switch (state)
            {
            case State.Inited:
            case State.Hide:
                {
                    showPanelFrameCount = Time.frameCount;
                    SoundMgr.PlaySound(openSound);

                    if (state == State.Hide)
                    {
                        cachedGameObject.SetActive(true);
                    }

                    if (!isFixedDepth)
                        MoveToFront();

                    //Debug.LogFormat("Panel Show {0}", this.name);
                    m_PanelAnimator.OnBeginShow(isPlayAnim);

                    state = State.Showing;
                    try
                    {
                        OnShow(args);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex);
                    }


                    bool isNext = false;
                    IEnumerator itor = null;
                    try
                    {
                        itor = OnShowSync(args);
                        isNext = itor.MoveNext();
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                    
                    while (isNext)
                    {
                        yield return 0;
                        try
                        {
                            isNext = itor.MoveNext();
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogException(ex);
                            break;
                        }
                    }

                    //Debug.LogFormat("Panel OnShow end {0}", this.name);

                    if (!isVisible)
                        yield break;

                    // 先初始化面板数据
                    if (isPlayAnim && openOrCloseAnimator != null)
                    {
                        if (isShowPanelHead && panelHeader != null)
                            StartCoroutine(panelHeader.Show(this, true));
                        state = State.ShowAnim;
                        OnBeginPlayAnimShow();
                        itor = PlayAnim(true);
                        while (itor.MoveNext())
                            yield return 0;
                        OnEndPlayAnimShow();

                        if (state != State.ShowAnim)
                            yield break;
                    }
                    else
                    {
                        if (isShowPanelHead && panelHeader != null)
                            StartCoroutine(panelHeader.Show(this, false));
                    }

                    state = State.Show;
                    Event.FireEvent(EID.OpenPanel, this);
                    Event.FireEvent(EID.OpenPanel, panelType, this);
                }
                break;
            case State.Show: // 当前已经处于显示当中
            case State.ShowAnim: // 当前已经处于显示当中
                {
//                     OnShow(args);
//                     IEnumerator itor = OnShowSync(args);
//                     while (itor.MoveNext())
//                         yield return 0;
                }
                break;
            default:
                Debuger.ErrorLog("{0} Show state:{1} error!", PanelName, state);
                break;
            }
        }
       

        public virtual void Hide(bool isPlayAnim)
        {
            if (isVisible && state != State.Hide)
            {
                if (enabled == false)
                    return;

                StartCoroutine(HideImp(isPlayAnim));
            }
        }

        protected void OnDisable()
        {
            if (state != State.Hide)
            {
#if UNITY_EDITOR
                if (App.my == null)
                    return;
#endif
                App.my.uiSystem.StartCoroutine(HideImp(false));
            }
        }

        // 面板关闭的时间
        public float last_hide_timer { get; protected set; }

        public virtual void DestroySelf()
        {
#if UNITY_EDITOR
            if (App.my == null)
                return;
#endif
            App.my.uiSystem.DestroyPanel(panelType);
        }

        protected virtual void OnDestroy()
        {
#if UNITY_EDITOR
            if (App.my == null)
                return;
#endif
            App.my.uiSystem.DestroyPanel(panelType);

#if USE_RESOURCESEXPORT
            PackTool.AtlasLoad.DestroyByName(panelType);
#endif
        }

        // 关闭按钮
        protected virtual void OnCloseBtnClick()
        {
            if (IsCanCloseByBtn())
                Hide(true);
        }

        // 回调按钮
        protected virtual void OnBackBtnClick()
        {
            if (IsCanCloseByBtn())
                Hide(true);
        }

        // 关闭界面
        public virtual bool IsCanCloseByBtn()
        {
            return true;
        }

        // 开始隐藏面板
        protected virtual IEnumerator HideImp(bool isPlayAnim)
        {
            switch (state)
            {
            case State.Hide:
                yield break;

            case State.Hiding:
                {
                    OnEndPlayAnimHide();
                }
                break;

            case State.Show:
                {
                    SoundMgr.PlaySound(closeSound);

                    // 先初始化面板数据
                    if (isPlayAnim && openOrCloseAnimator != null)
                    {
                        state = State.Hiding;
                        OnBeginPlayAnimHide();
                        if (isShowPanelHead)
                            App.my.uiSystem.StartCoroutine(panelHeader.Hide(true));

                        IEnumerator itor = PlayAnim(false);
                        while (itor.MoveNext())
                            yield return 0;

                        if (isShowPanelHead)
                        {
                            while (panelHeader.state != UIPanelHeader.State.Hide)
                                yield return 0;
                        }

                        if (state != State.Hiding)
                        {
                            yield break;
                        }
                        else
                        {
                            OnEndPlayAnimHide();
                        }
                    }
                    else
                    {
                        OnEndPlayAnimHide();
                        if (isShowPanelHead)
                            xys.App.my.uiSystem.StartCoroutine(panelHeader.Hide(false));
                    }
                }
                break;
            case State.ShowAnim:
                {
                    // 当前正在播放显示动画
                    if (isShowPanelHead)
                    {
                        xys.App.my.uiSystem.StartCoroutine(panelHeader.Hide(false));
                    }
                }
                break;
            }

            last_hide_timer = Time.realtimeSinceStartup;
            Event.Release(); // 取消所有事件的注册
            OnHide();

            state = State.Hide;

            // 关闭结束
            cachedGameObject.SetActive(false);

            if (isShowPanelHead)
            {
                UIPanelBase maxShowPanel = null; // 最上层的全局界面
                App.my.uiSystem.ForEach((UIPanelBase pb) =>
                {
                    if (!pb.isShowPanelHead || pb.state != State.Show)
                        return;

                    if (maxShowPanel == null)
                        maxShowPanel = pb;
                    else if (pb.currentDepth > maxShowPanel.currentDepth)
                    {
                        maxShowPanel = pb;
                    }
                });

                if (maxShowPanel != null)
                {
                    App.my.uiSystem.StartCoroutine(panelHeader.Show(maxShowPanel, false));
                }
            }

            Event.FireEvent(EID.ClosePanel, this);
            Event.FireEvent(EID.ClosePanel, panelType, this);
        }


        protected abstract void OnInit();

        protected virtual IEnumerator OnInitSync() { yield break; }

        protected virtual void OnBeginPlayAnimShow() { }
        protected virtual void OnEndPlayAnimShow() { }
        protected virtual void OnBeginPlayAnimHide() { }
        protected virtual void OnEndPlayAnimHide() { }
        protected virtual void OnInitEnd(){ }
        protected abstract void OnShow(object args);
        protected virtual IEnumerator OnShowSync(object args) { yield break; }
        protected virtual void OnHide() { }        
    }
}