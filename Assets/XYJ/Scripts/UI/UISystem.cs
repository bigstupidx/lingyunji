using XTools;
using PackTool;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace xys.UI
{
    public class UISystem : MonoBehaviour
    {
        [SerializeField]
        Transform m_Root;

        public GameObject m_maskPanelObj;

        [SerializeField]
        UIBlurControl control;

        public UIBlurControl Blur { get { return control; } }

        [SerializeField]
        LoadingMgr mLoadingMgr;
        public LoadingMgr loadingMgr { get { return mLoadingMgr; } }

        [SerializeField]
        UIPanelHeader mPanelHeader;
        public UIPanelHeader PanelHeader { get { return mPanelHeader; } }

        [SerializeField]
        DialogMgr mDialogMgr;
        public DialogMgr dialogMgr { get { return mDialogMgr; } }

        [SerializeField]
        SystemHintMgr mSystemHintMgr;
        public SystemHintMgr systemHintMgr { get { return mSystemHintMgr; } }

        [SerializeField]
        ObtainItemShowMgr mObtainItemShowMgr;
        public ObtainItemShowMgr obtainItemShowMgr { get { return mObtainItemShowMgr; } }

        ProgressMgr m_ProgressMgr;
        public ProgressMgr progressMgr { get { return m_ProgressMgr; } }

        [SerializeField]
        PureShowTipsMgr mPureShowTipsMgr;
        public PureShowTipsMgr pureShowTipsMgr { get { return mPureShowTipsMgr; } }

        [SerializeField]
        int livePanelMax = 3; // 存在面板的最大个数

        [SerializeField]
        RectTransform mTopPanelRoot;
        public RectTransform TopPanelRoot { get { return mTopPanelRoot; } }

        [SerializeField]
        Hud2DSystem mHud2DSystem;
        public Hud2DSystem hud2DSystem { get { return mHud2DSystem; } }

        // 记录面板项
        List<UIPanelBase> RecordingPanels = new List<UIPanelBase>();

        protected void Awake()
        {
#if UNITY_EDITOR
            if (App.my == null)
                return;
#endif
            //DontDestroyOnLoad(gameObject);

            TipCanvas.transform.localPosition = new Vector3(10000, 0, 0);
            mRootCanvas.transform.localPosition = new Vector3(20000, 0, 0);
            Blur.transform.localPosition = new Vector3(30000, 0, 0); ;

            StandaloneInputModule sim = null;
            PointerInputModule pim = eventSystem.GetComponent<PointerInputModule>();
            if (pim == null)
            {
                sim = eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            }
            else if (!(pim is StandaloneInputModule))
            {
                DestroyImmediate(pim);
                sim = eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            }
            else
            {
                sim = pim as StandaloneInputModule;
            }

            sim.forceModuleActive = true;

            if (PanelHeader != null)
                PanelHeader.Init();

            dialogMgr.Init();

            m_ProgressMgr = new ProgressMgr(this.gameObject);
        }

        void Start()
        {
#if UNITY_EDITOR
            if (App.my == null)
                return;
#endif
            if (eventSystem == null)
                eventSystem = EventSystem.current;

            if (mRootCanvas != null)
            {
                canvasGameObject = mRootCanvas.gameObject;
                canvasGameObject.GetOrAddComponent<ScreenEvent>();

                CanvasScaler scaler = mRootCanvas.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                scaler.referenceResolution = new Vector2(1334, 750);
            }

            mBlurUpdate = new BlurUpdate();
        }
        // 当前是否隐藏主界面
        bool CurrentHideMainPanel()
        {
            foreach (var p in mPanels)
            {
                switch (p.second.state)
                {
                case UIPanelBase.State.Show:
                case UIPanelBase.State.ShowAnim:
                case UIPanelBase.State.Showing:
                    {
                        // 显示中的界面
                        if (p.second.isHideMainPanel)
                        {
                            return true;
                        }
                    }
                    break;
                }
            }

            return false;
        }

        void UpdateByMainPanel()
        {
            var mainPanel = Get<UIMainPanel>();
            if (mainPanel != null)
                mainPanel.isHideState = CurrentHideMainPanel();
        }

        static BlurToTexture sceneBlurToTexture;

        class BlurUpdate
        {
            public void DisableEffect()
            {
                if (!App.my.uiSystem.Blur.isBlur)
                    return;

                App.my.uiSystem.Blur.isBlur = false;
                if (sceneBlurToTexture != null)
                {
                    sceneBlurToTexture.EndCapture();
                }
            }

            public void ActiveEffect()
            {
                if (App.my.uiSystem.Blur.isBlur)
                    return;

                App.my.uiSystem.Blur.isBlur = true;
                sceneBlurToTexture = BlurToTexture.Get(Camera.main);
                if (sceneBlurToTexture != null)
                {
                    sceneBlurToTexture.BeginCapture();
                }
            }

            UIBlurExcepRoot current; // 当前生效的

            public void Update()
            {
                List<UIBlurExcepRoot> list = UIBlurExcepRoot.ActiveList;
                UIBlurExcepRoot newRoot = null;
                if (list.Count != 0)
                {
                    list.Sort((UIBlurExcepRoot x, UIBlurExcepRoot y) => { return x.depth.CompareTo(y.depth); });
                    newRoot = list[list.Count - 1];
                }

                if (object.Equals(current, newRoot))
                    return;

                if (current != null)
                {
                    current.Close();
                }

                if (newRoot != null)
                {
                    ActiveEffect();
                    newRoot.Open();
                }
                else
                {
                    DisableEffect();
                }

                current = newRoot;
            }
        }

        BlurUpdate mBlurUpdate = null;

        void UpdateByBlur()
        {
            mBlurUpdate.Update();
        }

        // 检测面板
        void LateUpdate()
        {
#if UNITY_EDITOR
            if (App.my == null)
                return;

            //if (Input.GetKeyDown(KeyCode.Alpha0))
            //    HideUI();

            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //    ShowUI();
#endif
            UpdateByRecordingPanel();
            UpdateByBlur();
            UpdateByMainPanel();
        }

        [SerializeField]
        UI3D.TitleSystem m_TitleSystem;

        public UI3D.TitleSystem titleSystem { get { return m_TitleSystem; } }

        [SerializeField]
        Title2DSystem m_Title2DSystem;

        public Title2DSystem title2DSystem
        {
            get { return m_Title2DSystem; }
        }

        // 当前位置是否点击在UI上面
        public bool isContain()
        {
            return PointerOverGameObject() != null;
        }

        public GameObject PointerOverGameObject()
        {
            if (EventSystem.current == null)
                return null;

            int id = PointerInputModule.kMouseLeftId;

#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR            
            if (Input.touchCount == 0)
                return null;
            
            id = Input.GetTouch(0).fingerId;
#endif
            UI.StandaloneInputModule input = EventSystem.current.currentInputModule as UI.StandaloneInputModule;
            if (input == null)
                return null;

            return input.PointerOverGameObject(id);
        }

        public Camera uguiCamera
        {
            get { return mRootCanvas.worldCamera; }
        }

        [SerializeField]
        EventSystem eventSystem;

        public void ShowMaskPanel(bool isShow)
        {
            if (m_maskPanelObj != null)
            {
                m_maskPanelObj.SetActive(isShow);
            }
        }

        public void ShowPanelRoot(bool isShow)
        {
            if(m_Root != null)
            {
                m_Root.gameObject.SetActive(isShow);
            }
        }

        public static T Get<T>() where T : UIPanelBase
        {
#if UNITY_EDITOR
            if (App.my.uiSystem == null)
                return null;

            PanelType pt;
            if (!Str2Enum.To(typeof(T).Name, out pt))
            {
                Debug.LogFormat("PanelType not find! name:{0}", typeof(T).Name);
                return null;
            }
#endif
            return App.my.uiSystem.GetByType(typeof(T).Name) as T;
        }

        public static UIPanelBase Get(string type)
        {
#if UNITY_EDITOR
            if (App.my.uiSystem == null)
                return null;
#endif
            return App.my.uiSystem.GetByType(type);
        }

        public static UIPanelBase Get(PanelType type)
        {
#if UNITY_EDITOR
            if (App.my.uiSystem == null)
                return null;
#endif
            return App.my.uiSystem.GetByType(type.ToString());
        }

        // 当前所有的面板对象
        List<Pair<string, UIPanelBase>> mPanels = new List<Pair<string, UIPanelBase>>();

        public void ForEach(System.Action<UIPanelBase> fun)
        {
            UIPanelBase pb;
            for (int i = 0; i < mPanels.Count; ++i)
            {
                pb = mPanels[i].second;
                if (pb != null)
                    fun(pb);
            }
        }

        public bool DestroyPanel(string type)
        {
            int index = GetIndexByType(type);
            if (index == -1)
                return false;

            var panel = mPanels[index].second;
            mPanels.RemoveAt(index);
            if (PanelHeader.panel_type == type)
            {
                PanelHeader.Hide(false);
            }

            if (panel != null)
            {
                panel.Hide(false);
                Destroy(panel.cachedGameObject);
                return true;
            }

            return false;
        }

        protected UIPanelBase GetByType(string type)
        {
            for (int i = 0; i < mPanels.Count; ++i)
            {
                if (mPanels[i].first == type)
                {
                    return mPanels[i].second;
                }
            }

            return null;
        }

        protected int GetIndexByType(string type)
        {
            for (int i = 0; i < mPanels.Count; ++i)
            {
                if (mPanels[i].first == type)
                {
                    return i;
                }
            }

            return -1;
        }

        GameObject mGo;
        Transform mTrans;

        [SerializeField]
        Canvas mRootCanvas;

        [SerializeField]
        Canvas mTipCanvas;

        GameObject canvasGameObject;

        public Canvas RootCanvas { get { return mRootCanvas; } }

        public Canvas TipCanvas { get { return mTipCanvas; } }

        [SerializeField]
        RectTransform mPanelRoot; // 所有的UGUI面板都绑定到此结点上

        public RectTransform PanelRoot { get { return mPanelRoot; } }

        public GameObject cachedGameObject { get { if (mGo == null) mGo = gameObject; return mGo; } }

        public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

        public void ShowPanel(PanelType type, object args, bool isPlayAnim = true)
        {
            ShowPanel(type.ToString(), args, isPlayAnim);
        }
        public void ShowPanel(string type)
        {
            ShowPanel(type, null, true);
        }

        // 显示某个面板
        public void ShowPanel(string type, object args, bool isPlayAnim)
        {
            UIPanelBase panel = GetByType(type);
            if (panel != null)
            {
                StartCoroutine(panel.Show(args, isPlayAnim && !panel.cachedGameObject.activeSelf));
            }
            else
            {
                StartCoroutine(ShowPanelAsync(type, args, isPlayAnim));
            }
        }

        public void HidePanel<T>(bool isPlayAnim = true) where T : UIPanelBase
        {
            HidePanel(typeof(T).Name, isPlayAnim);
        }

        public void HidePanel(PanelType type, bool isPlayAnim = true)
        {
            HidePanel(type.ToString(), isPlayAnim);
        }

        public void HidePanel(string type, bool isPlayAnim = true)
        {
            UIPanelBase panel = Get(type);
            if (panel != null)
            {
                if (isPlayAnim && panel.state != UIPanelBase.State.Show)//可能正在关闭中
                    return;

                panel.Hide(isPlayAnim);
            }
        }

        // UI的显示和隐藏
        int m_hideUICount = 0;
        public void ShowUI()
        {
            m_hideUICount--;
            if (m_hideUICount == 0)
            {
                uguiCamera.cullingMask = Layer.uiMask;
                TipCanvas.worldCamera.enabled = true;
                //ShowPanel(PanelType.UIMainPanel, null, false);
            }
        }

        public void HideUI()
        {
            if (m_hideUICount==0)
            {
                uguiCamera.cullingMask = Layer.passMask;
                TipCanvas.worldCamera.enabled = false;
                //HideAllPanel();
            }
            m_hideUICount++;
        }

        class InitPanelData
        {
            public object args;
            public bool isPlayAnim;
        }

        Dictionary<string, InitPanelData> InitPanelList = new Dictionary<string, InitPanelData>();

        IEnumerator ShowPanelAsync(string type, object args, bool isPlayAnim)
        {
            InitPanelData initData = null;
            if (InitPanelList.TryGetValue(type, out initData))
            {
                // 正在加载当中的
                initData.args = args;
                initData.isPlayAnim = isPlayAnim;
                yield break;
            }
            else if (GetByType(type) != null)
            {
                yield break;
            }
            else
            {
                initData = new InitPanelData();
                initData.args = args;
                initData.isPlayAnim = isPlayAnim;
                InitPanelList.Add(type, initData);
            }

            string file = type;
            GameObject obj = null;
            {
                if (PanelCount >= livePanelMax)
                {
                    UIPanelBase destroy = null;
                    ForEach((UIPanelBase pb) =>
                    {
                        if (pb.state != UIPanelBase.State.Hide)
                            return;

                        if (destroy == null || destroy.last_hide_timer < pb.last_hide_timer)
                            destroy = pb;
                    });

                    if (destroy != null)
                    {
                        destroy.DestroySelf();
                    }
                }

                //PrefabLoad load = (PrefabLoad)PrefabLoad.LoadAsync(file, null, null, false, true).load;
                bool isDone = false;
                GameObject asset = null;
                RALoad.LoadPrefab(file, (GameObject go, object p) => { asset = go; isDone = true; }, null, false, true);
                while (!isDone)
                    yield return 0;

                if (asset == null)
                {

                }
                else
                {
#if UNITY_EDITOR
                    bool isA = asset.activeSelf;
#endif
                    asset.SetActive(false);
                    obj = Instantiate(asset);
                    obj.name = asset.name;
#if UNITY_EDITOR
                    asset.SetActive(isA);
#endif
                }
            }

            if (obj == null)
            {
                Debuger.LogError("Resources not found panel name is:" + type);
                InitPanelList.Remove(type);
                yield break;
            }

            UIPanelBase panelBase = obj.GetComponent<UIPanelBase>();
            if (panelBase == null)
            {
                Debuger.ErrorLog("type:{0} not find Compoment!", type);
                InitPanelList.Remove(type);
                yield break;
            }

            if (!panelBase.enabled)
                panelBase.enabled = true;

            RectTransform tran = panelBase.cachedTransform;
            if (panelBase.startBlurExcep)
            {
                tran.SetParent(Blur.rectTransform);
            }
            else
            {
                switch (panelBase.cameraType)
                {
                case PanelCameraType.Default:
                    tran.SetParent(mPanelRoot);
                    break;
                case PanelCameraType.Top:
                    tran.SetParent(mTopPanelRoot);
                    break;
                }
            }

            tran.localScale = Vector3.one;
            tran.localPosition = Vector3.zero;
            tran.anchorMin = Vector2.zero;
            tran.anchorMax = Vector2.one;

            tran.offsetMin = Vector2.zero;
            tran.offsetMax = Vector2.zero;

            try
            {
                tran.gameObject.SetActive(true);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }

            if (panelBase != null)
            {
                IEnumerator itor = panelBase.Init();
                while (itor.MoveNext())
                {
                    yield return 0;
                    if (panelBase == null)
                        break;
                }

                InitPanelList.Remove(type);
                if (panelBase != null)
                {
                    mPanels.Add(new Pair<string, UIPanelBase>(type, panelBase));
                    itor = panelBase.Show(initData.args, initData.isPlayAnim);
                    while (itor.MoveNext())
                        yield return 0;
                }
            }
            else
            {
                InitPanelList.Remove(type);
                Debuger.ErrorLog("panel:{0} cont bind UIPanelBase!", type);
            }
        }

        public void GetMaxDepthPanel(List<UIPanelBase> list)
        {
            ForEach((UIPanelBase pb) =>
            {
                if (pb == null || pb.isFixedDepth || !pb.isVisible)
                    return;

                list.Add(pb);
            });

            list.Sort((UIPanelBase y, UIPanelBase x) =>
            {
                return x.currentDepth.CompareTo(y.currentDepth);
            });
        }

        //关闭所有面板
        public void HideAllPanel()
        {
            UIPanelBase pb = null;
            for (int i = 0; i < mPanels.Count; ++i)
            {
                pb = mPanels[i].second;
                if (pb != null)
                {
                    pb.Hide(true);
                }
            }
        }

        public void HideShowingPanel()
        {
            UIPanelBase pb = null;
            for (int i = 0; i < mPanels.Count; ++i)
            {
                pb = mPanels[i].second;
                if (pb != null && pb.state == UIPanelBase.State.Show)
                {
                    pb.Hide(true);
                }
            }
        }

        public int PanelCount { get { return mPanels.Count; } }

        //判断当前是否打开了背景模糊的ui
        public bool ContainsBlurPanel()
        {
            UIPanelBase pb = null;
            for (int i = 0; i < mPanels.Count; ++i)
            {
                pb = mPanels[i].second;
                if (pb != null && pb.state == UIPanelBase.State.Show)
                {
                    if (pb.startBlurExcep)
                        return true;
                }
            }
            return false;
        }

        List<UIPanelBase> TempList = new List<UIPanelBase>();

        void UpdateByRecordingPanel()
        {
            TempList.Clear();
            for (int i = 0; i < mPanels.Count; ++i)
            {
                var panel = mPanels[i].second;
                if (panel.isVisible)
                    TempList.Add(panel);
            }

            // 检测是否有排队性面板
            UIPanelBase p = null;
            {
                bool isExclusive = false;
                for (int i = 0; i < TempList.Count; ++i)
                {
                    p = TempList[i];
                    if (p.isExclusive)
                    {
                        isExclusive = true;
                        p.temporaryHide = false;
                    }
                }

                if (isExclusive)
                {
                    for (int i = 0; i < TempList.Count; ++i)
                    {
                        p = TempList[i];
                        if (!p.isExclusive)
                            p.temporaryHide = true;
                    }

                    return;
                }
            }

            // 检测面板链
            int startpos = TempList.Count;
            {
                for (int i = 0; i < startpos; ++i)
                {
                    p = TempList[i];
                    if (p.isRecordingPanel)
                        TempList.Add(p);
                }
            }

            int Count = TempList.Count - startpos;
            switch (Count)
            {
            case 0:
                for (int i = 0; i < TempList.Count; ++i)
                    TempList[i].temporaryHide = false;
                return;
            case 1:
                TempList[startpos].temporaryHide = false;
                return;
            }

            TempList.Sort(startpos, Count, s_rc);
            for (int i = TempList.Count - 2; i >= startpos; --i)
                TempList[i].temporaryHide = true;

            TempList[TempList.Count - 1].temporaryHide = false;
        }

        static RC s_rc = new RC();

        class RC : IComparer<UIPanelBase>
        {
            int IComparer<UIPanelBase>.Compare(UIPanelBase x, UIPanelBase y)
            {
                return x.ShowPanelFrameCount.CompareTo(y.ShowPanelFrameCount);
            }
        }

        // 指导面板是否在打开状态中
        public bool IsShow(PanelType type)
        {
            UIPanelBase ui = Get(type);
            if (ui == null)
                return false;

            return ui.state == UIPanelBase.State.Show;
        }
    }
} 