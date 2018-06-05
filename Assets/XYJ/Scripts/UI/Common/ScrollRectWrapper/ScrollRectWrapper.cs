using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace xys.UI
{
    /// <summary>
    /// 先设置MaxCount再进行其他操作
    /// 必须从外部传入一个委托OnCellAdding，用以处理子物体实例化
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public abstract class ScrollRectWrapper : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, IPointerClickHandler, ICanvasElement, ILayoutElement, ILayoutGroup
    {
        #region Defines
        public enum MovementType
        {
            /// <summary>
            /// 无限滚动
            /// </summary>
            Unrestricted,
            /// <summary>
            /// 可越界，越界回弹
            /// </summary>
            Elastic,
            /// <summary>
            /// 限制不可越界
            /// </summary>
            Clamped,
        }
        public enum ScrollbarVisibility
        {
            Permanent,
            AutoHide,
            AutoHideAndExpandViewport,
        }
        #endregion

        #region Field
        // 对指定特例进行变化
        public delegate string PrefabNameDelegate(int index);
        public delegate int PrefabCountDelegate(int index);

        [HideInInspector]
        public PrefabNameDelegate PrefabNameFunc = null;
        [HideInInspector]
        public PrefabCountDelegate PrefabCountFunc = null;

        [HideInInspector]
        public Action<GameObject, int> OnCellAdding = null;
        [HideInInspector]
        public Action<GameObject> OnCellPooling = null;

        public string PrefabName;

        public bool Horizontal = true;
        public bool Vertical = true;

        [SerializeField]
        public MovementType MoveType = MovementType.Elastic;
        [SerializeField]
        [Tooltip("仅MovementType.Elastic可用，滑动时长")]
        public float Elasticity = 0.1f;

        [SerializeField]
        public bool Inertia = true;
        [Tooltip("仅Inertia可用，减速倍率")]
        public float DecelerationRate = 0.135f;

        [SerializeField]
        [Tooltip("定位滚动速度")]
        public float MarkerScrollSpeed = 20f;
        [Tooltip("滚动时敏感度")]
        public float ScrollSensitivity = 1.0f;
        [Tooltip("越界回弹胶量")]
        public float RubberScale = 1;
        [Tooltip("是否自动更新")]
        public bool AutoUpdate = true;
        // TODO 待优化
        //[Tooltip("是否反向拖动")]
        //public bool ReverseDirection = false;
        [SerializeField]
        [Tooltip("可以容纳的数据量")]
        public int TotalCount;
        [SerializeField]
        [Tooltip("列表当前缓存数量")]
        public int PoolSize = 7;

        // 当前element的大小集合
        [HideInInspector]
        public Dictionary<int, float> SizeDatas = new Dictionary<int, float>();

        protected bool AddNewData = false;
        // 当前缓存索引
        protected int ItemTypeStart = 0;
        protected int ItemTypeEnd = 0;

        // 加阈值防止反复创建和删除
        protected float Threshold = 0;
        protected int DirectionSign = 0;
        protected GridLayoutGroup GridLayout;

        // 可以缓存的最大数量
        protected int MaxCount;
        protected bool Dragging;
        // 定位相关
        protected bool IsMarkerMoving;
        protected Vector2 ConstOffset;
        protected Action MarkerCallback;
        private Vector2 m_movingsign = Vector2.zero;
        // 鼠标位移量
        private Vector2 m_pointerStartLocalCursor = Vector2.zero;
        private Vector2 m_contentStartPosition = Vector2.zero;

        // 预渲染数据
        private Vector2 m_prevPosition = Vector2.zero;
        private Bounds m_prevContentBounds;
        private Bounds m_prevViewBounds;
        [NonSerialized]
        private bool m_hasRebuiltLayout;

        private Vector2 Velocity;

        #endregion

        #region Property
        [SerializeField]
        private bool autoLock = false;

        public bool AutoLock
        {
            get { return autoLock; }
            protected set { autoLock = value; }
        }
        public bool IsLock { get; set; }
        [SerializeField]
        private RectTransform content;
        public RectTransform Content { get { return content; } set { content = value; } }

        [SerializeField]
        private RectTransform m_viewport;
        public RectTransform Viewport
        {
            get
            {
                if(null == m_viewport)
                {
                    m_viewport = (RectTransform)transform;
                }
                return m_viewport;
            }
            set
            {
                m_viewport = value;
                SetDirtyCaching();
            }
        }

        private RectTransform m_viewRect;
        protected RectTransform ViewRect
        {
            get
            {
                if(null == m_viewRect)
                {
                    m_viewRect = Viewport;
                }
                return m_viewRect;
            }
        }

        [SerializeField]
        private Scrollbar m_horizontalScrollbar;
        public Scrollbar HorizontalScrollbar
        {
            get
            {
                return m_horizontalScrollbar;
            }
            set
            {
                if(m_horizontalScrollbar)
                    m_horizontalScrollbar.onValueChanged.RemoveListener(SetHorizontalNormalizedPosition);
                m_horizontalScrollbar = value;
                if(m_horizontalScrollbar)
                    m_horizontalScrollbar.onValueChanged.AddListener(SetHorizontalNormalizedPosition);
                SetDirtyCaching();
            }
        }

        [SerializeField]
        private Scrollbar m_verticalScrollbar;
        public Scrollbar VerticalScrollbar
        {
            get
            {
                return m_verticalScrollbar;
            }
            set
            {
                if(m_verticalScrollbar)
                    m_verticalScrollbar.onValueChanged.RemoveListener(SetVerticalNormalizedPosition);
                m_verticalScrollbar = value;
                if(m_verticalScrollbar)
                    m_verticalScrollbar.onValueChanged.AddListener(SetVerticalNormalizedPosition);
                SetDirtyCaching();
            }
        }

        [SerializeField]
        private ScrollbarVisibility m_horizontalScrollbarVisibility;
        public ScrollbarVisibility HorizontalScrollbarVisibility
        {
            get
            {
                return m_horizontalScrollbarVisibility;
            }
            set
            {
                m_horizontalScrollbarVisibility = value;
                SetDirtyCaching();
            }
        }

        [SerializeField]
        private ScrollbarVisibility m_verticalScrollbarVisibility;
        public ScrollbarVisibility VerticalScrollbarVisibility
        {
            get
            {
                return m_verticalScrollbarVisibility;
            }
            set
            {
                m_verticalScrollbarVisibility = value;
                SetDirtyCaching();
            }
        }

        [SerializeField]
        private float m_horizontalScrollbarSpacing;
        public float HorizontalScrollbarSpacing
        {
            get
            {
                return m_horizontalScrollbarSpacing;
            }
            set
            {
                m_horizontalScrollbarSpacing = value;
                SetDirty();
            }
        }

        [SerializeField]
        private float m_verticalScrollbarSpacing;
        public float VerticalScrollbarSpacing
        {
            get
            {
                return m_verticalScrollbarSpacing;
            }
            set
            {
                m_verticalScrollbarSpacing = value;
                SetDirty();
            }
        }

        private float m_contentSpacing = -1;
        protected float ContentSpacing
        {
            get
            {
                if(m_contentSpacing >= 0)
                {
                    return m_contentSpacing;
                }
                m_contentSpacing = 0;
                if(content == null)
                    return m_contentSpacing;

                var layout1 = content.GetComponent<HorizontalOrVerticalLayoutGroup>();
                if(layout1 != null)
                {
                    m_contentSpacing = layout1.spacing;
                }
                GridLayout = content.GetComponent<GridLayoutGroup>();
                if(GridLayout != null)
                {
                    m_contentSpacing = GetDimension(GridLayout.spacing);
                }
                return m_contentSpacing;
            }
        }

        private int m_contentConstraintCount;
        protected int ContentConstraintCount
        {
            get
            {
                if(m_contentConstraintCount > 0)
                {
                    return m_contentConstraintCount;
                }
                m_contentConstraintCount = 1;
                if(content == null)
                    return m_contentConstraintCount;

                var layout2 = content.GetComponent<GridLayoutGroup>();
                if(layout2 == null)
                    return m_contentConstraintCount;

                if(layout2.constraint == GridLayoutGroup.Constraint.Flexible)
                {
                    Debug.LogError("[ScrollRectWrapper] Flexible not supported yet");
                }
                m_contentConstraintCount = layout2.constraintCount;
                return m_contentConstraintCount;
            }
        }

        [NonSerialized]
        private RectTransform m_rect;
        private RectTransform rectTransform
        {
            get { return m_rect ?? ( m_rect = GetComponent<RectTransform>() ); }
        }

        private Bounds m_contentBounds;
        private Bounds m_viewBounds;
        public Vector2 NormalizedPosition
        {
            get
            {
                return new Vector2(HorizontalNormalizedPosition, VerticalNormalizedPosition);
            }
            set
            {
                SetNormalizedPosition(value.x, 0);
                SetNormalizedPosition(value.y, 1);
            }
        }
        public float HorizontalNormalizedPosition
        {
            get
            {
                UpdateBounds();

                if(TotalCount > 0 && ItemTypeEnd > ItemTypeStart)
                {
                    // 空间计算待优化
                    float elementSize = m_contentBounds.size.x / ( ItemTypeEnd - ItemTypeStart );
                    float totalSize = elementSize * TotalCount;
                    float offset = m_contentBounds.min.x - elementSize * ItemTypeStart;

                    if(totalSize <= m_viewBounds.size.x)
                        return ( m_viewBounds.min.x > offset ) ? 1 : 0;
                    return ( m_viewBounds.min.x - offset ) / ( totalSize - m_viewBounds.size.x );
                }
                return 0.5f;
            }
            set
            {
                SetNormalizedPosition(value, 0);
            }
        }
        public float VerticalNormalizedPosition
        {
            get
            {
                UpdateBounds();

                if(TotalCount > 0 && ItemTypeEnd > ItemTypeStart)
                {
                    // 空间计算待优化
                    float elementSize = m_contentBounds.size.y / ( ItemTypeEnd - ItemTypeStart );
                    float totalSize = elementSize * TotalCount;
                    float offset = m_contentBounds.max.y + elementSize * ItemTypeStart;

                    if(totalSize <= m_viewBounds.size.y)
                        return ( offset > m_viewBounds.max.y ) ? 1 : 0;
                    return ( offset - m_viewBounds.max.y ) / ( totalSize - m_viewBounds.size.y );
                }
                return 0.5f;
            }
            set
            {
                SetNormalizedPosition(value, 1);
            }
        }

        [Serializable]
        public class ScrollRectEvent : UnityEvent<Vector2> { }
        public ScrollRectEvent OnValueChanged;
        [Serializable]
        public class ScrollRectClickEvent : UnityEvent { }
        public ScrollRectClickEvent OnClickedEvent;
        public ScrollRectClickEvent OnClickedWithoutDragEvent;

        #endregion

        #region Interface
        protected abstract float GetSize(RectTransform item);
        protected abstract float GetDimension(Vector2 vector);
        protected abstract Vector2 GetVector(float value);
        /// <summary>
        /// 将view滚动至标记物体处,效果在lateUpdate执行。
        /// 所以注意同帧逻辑
        /// </summary>
        /// <param name="index">该物体的索引值</param>
        /// <param name="callBack">滚动完毕后的回调</param>
        /// <returns>是否可以定位</returns>
        public abstract bool SetMarkerToCenter(int index, Action callBack = null);
        public virtual void StopMovement()
        {
            Velocity = Vector2.zero;
        }

        /// <summary>
        /// 添加数据，调用前先设置SetMaxCount
        /// </summary>
        /// <returns></returns>
        public virtual void AddData()
        {
            //Assert.IsTrue(MaxCount > 0,"调用前先设置maxcount");
            //if(TotalCount / 2 > MaxCount && TotalCount > 100)
            //{
            //    var index = TotalCount - MaxCount;
            //    SizeDatas.Remove(index);
            //}

            if(!IsLock)
            {
                AddNewData = true;
            }
            TotalCount++;
        }
        public virtual void SetMaxCount(int number)
        {
            MaxCount = number;
        }

        public void OnInit()
        {
            Awake();
        }
        /// <summary>
        /// 用于二次显示时的数据导入
        /// </summary>
        /// <param name="start"></param>
        /// <param name="maxCount"></param>
        /// <param name="totalCount"></param>
        public void Init(int start, int maxCount, int totalCount)
        {
            MaxCount = maxCount;
            TotalCount = totalCount;
            ItemTypeStart = start;
            ItemTypeEnd = ItemTypeStart;
            if(start > 0)
            {
                RefillCells(start);
            }
        }

        public int GetStartIndex()
        {
            return ItemTypeStart;
        }
        #endregion

        #region Constructer
        protected ScrollRectWrapper()
        {
            flexibleWidth = -1;
        }
        #endregion

        #region Editor Test
        // Editor仅供测试使用
        public void ClearCells()
        {
            if(!Application.isPlaying)
                return;

            ItemTypeStart = 0;
            ItemTypeEnd = 0;
            TotalCount = 0;
            for(var i = content.childCount - 1 ; i >= 0 ; i--)
            {
                SendMessageOnPooling(content.GetChild(i).gameObject);
            }
        }

        public void RefreshCells()
        {
            if(!Application.isPlaying || !isActiveAndEnabled)
                return;

            ItemTypeEnd = ItemTypeStart;
            // 尽可能循环利用
            for(int i = 0 ; i < content.childCount ; i++)
            {
                if(ItemTypeEnd < TotalCount)
                {
                    SendMessageOnAdding(content.GetChild(i).gameObject, ItemTypeEnd);
                    ItemTypeEnd++;
                }
                else
                {
                    SendMessageOnPooling(content.GetChild(i).gameObject);
                    i--;
                }
            }
        }

        public void RefillCells(int startIdx = 0/*,int childCount = 0*/)
        {
            if(Application.isPlaying)
            {
                StopMovement();
                ItemTypeStart = /*ReverseDirection ? TotalCount - startIdx : */startIdx;
                ItemTypeEnd = ItemTypeStart;

                // 不要在这里使用Canvas.ForceUpdateCanvases(),不然它会创建/删除cell并让itemTypeStart/End发生改变
                if(PrefabNameFunc != null/* && prefabCountFunc != null*/)
                {
                    // prefab名不一样则返回所有go
                    for(int i = 0 ; i < content.childCount ; i++)
                    {
                        SendMessageOnPooling(content.GetChild(i).gameObject);
                        i--;
                    }
                }
                else
                {
                    // 尽可能循环利用
                    for(int i = 0 ; i < content.childCount ; i++)
                    {
                        if(TotalCount >= 0 && ItemTypeEnd >= TotalCount)
                        {
                            SendMessageOnPooling(content.GetChild(i).gameObject);
                            i--;
                        }
                        else
                        {
                            SendMessageOnAdding(content.GetChild(i).gameObject, ItemTypeEnd);
                            ItemTypeEnd++;
                        }
                    }
                }

                Vector2 pos = content.anchoredPosition;
                if(DirectionSign == -1)
                    pos.y = 0;
                else if(DirectionSign == 1)
                    pos.x = 0;
                content.anchoredPosition = pos;
            }
        }
        #endregion

        #region Protected
        protected virtual float NewItemAtStart()
        {
            if(TotalCount >= 0 && ItemTypeStart - ContentConstraintCount < 0 || ItemTypeStart <= TotalCount - MaxCount/*todo ReverseDirection*/)
            {
                return 0;
            }

            float size = 0;
            for(int i = 0 ; i < ContentConstraintCount ; i++)
            {
                ItemTypeStart--;
                RectTransform newItem = InstantiateNextItem(ItemTypeStart);
                newItem.SetAsFirstSibling();
                size = Mathf.Max(GetSize(newItem), size);
            }

            //if(!ReverseDirection)
            //{
            Vector2 offset = GetVector(size);
            content.anchoredPosition += offset;
            m_prevPosition += offset;
            m_contentStartPosition += offset;
            //}
            return size;
        }

        protected virtual float DeleteItemAtStart()
        {
            if(TotalCount >= 0 && ItemTypeEnd >= TotalCount - 1 || content.childCount == 0)
            {
                return 0;
            }

            float size = 0;
            for(int i = 0 ; i < ContentConstraintCount ; i++)
            {
                RectTransform oldItem = content.GetChild(0) as RectTransform;
                size = Mathf.Max(GetSize(oldItem), size);

                if(null != oldItem)
                {
                    SendMessageOnPooling(oldItem.gameObject);
                }
                ItemTypeStart++;

                if(content.childCount == 0)
                {
                    break;
                }
            }

            //if(!ReverseDirection)
            //{
            Vector2 offset = GetVector(size);
            content.anchoredPosition -= offset;
            m_prevPosition -= offset;
            m_contentStartPosition -= offset;
            //}
            return size;
        }


        protected virtual float NewItemAtEnd()
        {
            if(TotalCount >= 0 && ItemTypeEnd >= TotalCount)
            {
                return 0;
            }
            float size = 0;

            int count = ContentConstraintCount - content.childCount % ContentConstraintCount;
            for(int i = 0 ; i < count ; i++)
            {
                RectTransform newItem = InstantiateNextItem(ItemTypeEnd);
                size = Mathf.Max(GetSize(newItem), size);

                if(!SizeDatas.ContainsKey(ItemTypeEnd))
                {
                    SizeDatas.Add(ItemTypeEnd, size);
                }

                ItemTypeEnd++;
                if(TotalCount >= 0 && ItemTypeEnd >= TotalCount)
                {
                    break;
                }
            }

            //if(ReverseDirection)
            //{
            //    Vector2 offset = GetVector(size);
            //    content.anchoredPosition += offset;
            //    m_prevPosition += offset;
            //    m_contentStartPosition += offset;
            //}

            var vb = new Bounds(ViewRect.rect.center, ViewRect.rect.size);
            var cb = GetBounds();
            var v = cb.min.y - size - ContentSpacing;
            if(AutoUpdate && !IsLock && !Dragging && vb.min.y > v)
            {
                var change = 0f;
                // 删除顶部结点
                if(vb.max.y < cb.max.y - Threshold)
                {
                    for(int i = 0 ; i < ContentConstraintCount ; i++)
                    {
                        RectTransform oldItem = content.GetChild(0) as RectTransform;

                        if(null != oldItem)
                        {
                            SendMessageOnPooling(oldItem.gameObject);
                        }
                        change = SizeDatas[ItemTypeStart];
                        ItemTypeStart++;

                        if(content.childCount == 0)
                        {
                            break;
                        }
                    }
                }

                Vector2 offset = GetVector(v + change);
                content.anchoredPosition -= offset;
            }

            return size;
        }

        protected virtual float DeleteItemAtEnd()
        {
            if(TotalCount >= 0 && ItemTypeStart < ContentConstraintCount || content.childCount == 0)
            {
                return 0;
            }

            float size = 0;
            for(int i = 0 ; i < ContentConstraintCount ; i++)
            {
                RectTransform oldItem = content.GetChild(content.childCount - 1) as RectTransform;
                size = Mathf.Max(GetSize(oldItem), size);
                if(null != oldItem)
                {
                    SendMessageOnPooling(oldItem.gameObject);
                }

                ItemTypeEnd--;
                if(ItemTypeEnd % ContentConstraintCount == 0 || content.childCount == 0)
                {
                    break;
                }
            }

            //if(ReverseDirection)
            //{
            //    Vector2 offset = GetVector(size);
            //    content.anchoredPosition += offset;
            //    m_prevPosition += offset;
            //    m_contentStartPosition += offset;
            //}
            return size;
        }

        protected void SetDirty()
        {
            if(!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        protected void SetDirtyCaching()
        {
            if(!IsActive())
                return;

            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        protected virtual bool UpdateItems(Bounds viewBounds, Bounds contentBounds) { return false; }


        #endregion

        #region POOL

        #region Message
        protected void SendMessageOnPooling(GameObject go)
        {
            if(null != OnCellPooling)
            {
                OnCellPooling(go);
            }
            AddObjectToPool(go);
        }
        protected void SendMessageOnAdding(GameObject go, int index)
        {
            if(null != OnCellAdding)
            {
                Debug.Log("send message to " + index.ToString().Color(ConsoleColor.Red));
                OnCellAdding(go, index);
            }
            else
            {
                go.SendMessage("OnCellAdding",index);
            }
        }

        protected virtual RectTransform InstantiateNextItem(int itemIndex)
        {
            string tempName = PrefabName;
            int count = PoolSize;
            if(PrefabNameFunc != null)
            {
                tempName = PrefabNameFunc(itemIndex);
            }
            if(PrefabCountFunc != null)
            {
                count = PrefabCountFunc(itemIndex);
            }
            GameObject nextItem = GetObjectFromPool(tempName, true, count);
            // 根据索引发送数据
            SendMessageOnAdding(nextItem.gameObject, itemIndex);
            return nextItem.GetComponent<RectTransform>();
        }

        #endregion

        #region Pool

        private readonly Stack<GameObject> CellPool = new Stack<GameObject>();
        private GameObject gameObjectPoolParent;
        private GameObject gameObjectPool;
        protected virtual void AddObjectToPool(GameObject go)
        {
            go.transform.SetParent(gameObjectPool.transform, false);
            go.SetActive(false);
            CellPool.Push(go);
        }

        protected virtual GameObject GetObjectFromPool(string prefabname, bool autoActive = true, int autoCreate = 0)
        {
            // 保留一个源种，避免再次Load
            if(2 > CellPool.Count)
            {
                int times = Mathf.Max(autoCreate, 1);

                for(int i = 0 ; i < times ; i++)
                {
                    GameObject temp = Instantiate(CellPool.Peek());
                    temp.name = prefabname;
                    AddObjectToPool(temp);
                }
            }

            var result = CellPool.Pop();
            if(autoActive)
            {
                result.gameObject.SetActive(true);
            }
            result.transform.SetParent(content, false);
            return result;
        }

        #endregion

        #endregion

        #region Impl

        #region UIBehavior Implementation

        protected override void Awake()
        {
            if(null == gameObjectPoolParent)
            {
                gameObjectPoolParent = GameObject.Find("ScrollRectObjectPool");
                if(null == gameObjectPoolParent)
                {
                    gameObjectPoolParent = new GameObject("ScrollRectObjectPool");
                }
            }
            if(null == gameObjectPool)
            {
                gameObjectPool = GameObject.Find(string.Format("{0}Pool", PrefabName));
                if(null == gameObjectPool)
                {
                    gameObjectPool = new GameObject(string.Format("{0}Pool", PrefabName));
                    gameObjectPool.transform.SetParent(gameObjectPoolParent.transform);
                }
            }
        }

        protected override void Start()
        {
            RALoad.LoadPrefab(PrefabName, (o, p) =>
            {
                AddObjectToPool(o);
            }, null);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if(m_horizontalScrollbar)
                m_horizontalScrollbar.onValueChanged.AddListener(SetHorizontalNormalizedPosition);
            if(m_verticalScrollbar)
                m_verticalScrollbar.onValueChanged.AddListener(SetVerticalNormalizedPosition);

            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

        protected override void OnDisable()
        {
            CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);

            if(m_horizontalScrollbar)
                m_horizontalScrollbar.onValueChanged.RemoveListener(SetHorizontalNormalizedPosition);
            if(m_verticalScrollbar)
                m_verticalScrollbar.onValueChanged.RemoveListener(SetVerticalNormalizedPosition);

            m_hasRebuiltLayout = false;
            m_tracker.Clear();
            SizeDatas.Clear();
            Velocity = Vector2.zero;
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            OnDisable();
            SizeDatas.Clear();
            Destroy(gameObjectPool);
            CellPool.Clear();
        }

        public override bool IsActive()
        {
            return base.IsActive() && content != null;
        }
        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        protected virtual void LateUpdate()
        {
            if(!content)
                return;

            EnsureLayoutHasRebuilt();
            UpdateScrollbarVisibility();
            UpdateBounds();
            float deltaTime = Time.unscaledDeltaTime;
            // content偏移复位
            Vector2 offset = CalculateOffset(Vector2.zero);
            if(!Dragging && ( offset != Vector2.zero || Velocity != Vector2.zero ) && !IsMarkerMoving)
            {
                Vector2 position = content.anchoredPosition;
                for(int axis = 0 ; axis < 2 ; axis++)
                {
                    // 如果运动是elastic且content对view有偏移，则使用弹簧回弹
                    if(MoveType == MovementType.Elastic && Mathf.Abs(offset[axis]) > 0.001f)
                    {
                        float speed = Velocity[axis];
                        position[axis] = Mathf.SmoothDamp(content.anchoredPosition[axis], content.anchoredPosition[axis] + offset[axis], ref speed, Elasticity, Mathf.Infinity, deltaTime);
                        Velocity[axis] = speed;
                    }
                    // 根据速度和减速来移动内容
                    else if(Inertia)
                    {
                        Velocity[axis] *= Mathf.Pow(DecelerationRate, deltaTime);
                        if(Mathf.Abs(Velocity[axis]) < 1)
                            Velocity[axis] = 0;
                        position[axis] += Velocity[axis] * deltaTime;
                    }
                    // 既不是Elastic也不是inertia,不应该有速度
                    else
                    {
                        Velocity[axis] = 0;
                    }
                }

                if(Velocity != Vector2.zero)
                {
                    if(MoveType == MovementType.Clamped)
                    {
                        offset = CalculateOffset(position - content.anchoredPosition);
                        position += offset;
                    }

                    SetContentAnchoredPosition(position);
                }
                else if(Vector2.zero == offset)
                {
                    IsLock = false;
                }
            }

            // 定量滑动
            if(!Dragging && Vector2.zero != ConstOffset && IsMarkerMoving)
            {
                // 覆盖滚动条的偏移量
                offset = Vector2.zero;
                for(int axis = 0 ; axis < 2 ; axis++)
                {
                    if(Mathf.Abs(m_movingsign[axis]) < 0.01f)
                    {
                        // 记录初始方向标记
                        m_movingsign[axis] = Math.Sign(ConstOffset[axis]);
                    }
                    offset[axis] += m_movingsign[axis] * MarkerScrollSpeed;

                    ConstOffset[axis] -= offset[axis];
                    // 已过临界值，不再滑动
                    if(Mathf.Abs(ConstOffset[axis]) > 0.01f && ConstOffset[axis] * m_movingsign[axis] < 0)
                    {
                        ConstOffset[axis] = 0f;
                    }
                }

                SetContentAnchoredPosition(content.anchoredPosition + offset);

                if(Vector2.zero == ConstOffset)
                {
                    m_movingsign = Vector2.zero;
                    Velocity = Vector2.zero;
                    IsMarkerMoving = false;
                    if(null != MarkerCallback)
                    {
                        MarkerCallback();
                    }
                }
            }

            if(Dragging && Inertia)
            {
                Vector3 newVelocity = ( content.anchoredPosition - m_prevPosition ) / deltaTime;
                Velocity = Vector3.Lerp(Velocity, newVelocity, deltaTime * 10);
            }

            if(m_viewBounds != m_prevViewBounds || m_contentBounds != m_prevContentBounds || content.anchoredPosition != m_prevPosition)
            {
                UpdateScrollbars(offset);
                if(null != OnValueChanged)
                {
                    OnValueChanged.Invoke(NormalizedPosition);
                }
                UpdatePrevData();
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirtyCaching();
        }
#endif
        #endregion

        #region IInitializePotentialDragHandler Implementation
        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if(eventData.button != PointerEventData.InputButton.Left)
                return;

            Velocity = Vector2.zero;
        }
        #endregion

        #region IBeginDragHandler Implementation
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if(eventData.button != PointerEventData.InputButton.Left)
                return;

            if(!IsActive())
                return;

            UpdateBounds();

            m_pointerStartLocalCursor = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(ViewRect, eventData.position, eventData.pressEventCamera, out m_pointerStartLocalCursor);
            m_contentStartPosition = content.anchoredPosition;
            if(eventData.dragging && autoLock)
            {
                Dragging = true;
                // 拖动则锁定
                IsLock = true;
            }
        }
        #endregion

        #region IEndDragHandler Implementation
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if(eventData.button != PointerEventData.InputButton.Left)
                return;

            Dragging = false;
        }
        #endregion

        #region IDragHandler Implementation
        public virtual void OnDrag(PointerEventData eventData)
        {
            if(eventData.button != PointerEventData.InputButton.Left)
                return;

            if(!IsActive())
                return;

            Vector2 localCursor;
            if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(ViewRect, eventData.position, eventData.pressEventCamera, out localCursor))
                return;

            UpdateBounds();

            var pointerDelta = localCursor - m_pointerStartLocalCursor;
            Vector2 position = m_contentStartPosition + pointerDelta;

            Vector2 offset = CalculateOffset(position - content.anchoredPosition);
            position += offset;
            if(MoveType == MovementType.Elastic)
            {
                if(Mathf.Abs(offset.x) > 0.001f)
                    position.x = position.x - RubberDelta(offset.x, m_viewBounds.size.x) * RubberScale;
                if(Mathf.Abs(offset.y) > 0.001f)
                    position.y = position.y - RubberDelta(offset.y, m_viewBounds.size.y) * RubberScale;
            }

            SetContentAnchoredPosition(position);
        }
        #endregion

        #region IScrollHandler Implementation
        public virtual void OnScroll(PointerEventData data)
        {
            if(!IsActive())
                return;

            EnsureLayoutHasRebuilt();
            UpdateBounds();

            Vector2 delta = data.scrollDelta;
            // scroll下滑事件是正数，在UI system里面上为正
            delta.y *= -1;
            if(Vertical && !Horizontal)
            {
                if(Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    delta.y = delta.x;
                delta.x = 0;
            }
            if(Horizontal && !Vertical)
            {
                if(Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
                    delta.x = delta.y;
                delta.y = 0;
            }

            Vector2 position = content.anchoredPosition;
            position += delta * ScrollSensitivity;
            if(MoveType == MovementType.Clamped)
                position += CalculateOffset(position - content.anchoredPosition);

            SetContentAnchoredPosition(position);
            UpdateBounds();
        }
        #endregion

        #region IPointerClickHandler Implementation
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if(null != OnClickedEvent)
            {
                OnClickedEvent.Invoke();
            }
            if(!Dragging && null != OnClickedWithoutDragEvent)
            {
                OnClickedWithoutDragEvent.Invoke();
            }
        }
        #endregion

        #region ICanvasElement Implementation
        public virtual void Rebuild(CanvasUpdate executing)
        {
            if(executing == CanvasUpdate.Prelayout)
            {
                UpdateCachedData();
            }

            if(executing == CanvasUpdate.PostLayout)
            {
                UpdateBounds(false);
                UpdateScrollbars(Vector2.zero);
                UpdatePrevData();

                m_hasRebuiltLayout = true;
            }
        }
        public virtual void LayoutComplete() { }
        public virtual void GraphicUpdateComplete() { }
        #endregion

        #region ILayoutElement Implementation

        public virtual float minWidth { get { return -1; } }
        public virtual float preferredWidth { get { return -1; } }
        public virtual float flexibleWidth { get; private set; }

        public virtual float minHeight { get { return -1; } }
        public virtual float preferredHeight { get { return -1; } }
        public virtual float flexibleHeight { get { return -1; } }

        public virtual int layoutPriority { get { return -1; } }
        public virtual void CalculateLayoutInputHorizontal() { }
        public virtual void CalculateLayoutInputVertical() { }

        #endregion

        #endregion

        #region Position Calculate
        protected Vector3 GetWidgetWorldPoint(RectTransform target)
        {
            var pivotOffset = new Vector3(
                ( 0.5f - target.pivot.x ) * target.rect.size.x,
                ( 0.5f - target.pivot.y ) * target.rect.size.y,
                0f);
            var localPosition = target.localPosition + pivotOffset;
            return target.parent.TransformPoint(localPosition);
        }
        protected virtual void SetContentAnchoredPosition(Vector2 position)
        {
            if(!Horizontal)
                position.x = content.anchoredPosition.x;
            if(!Vertical)
                position.y = content.anchoredPosition.y;

            if(position != content.anchoredPosition)
            {
                content.anchoredPosition = position;
                UpdateBounds();
            }
        }
        private void SetHorizontalNormalizedPosition(float value) { SetNormalizedPosition(value, 0); }
        private void SetVerticalNormalizedPosition(float value) { SetNormalizedPosition(value, 1); }
        private void SetNormalizedPosition(float value, int axis)
        {
            if(TotalCount <= 0 || ItemTypeEnd <= ItemTypeStart)
                return;

            EnsureLayoutHasRebuilt();
            UpdateBounds();

            Vector3 localPosition = content.localPosition;
            float newLocalPosition = localPosition[axis];
            if(axis == 0)
            {
                float elementSize = m_contentBounds.size.x / ( ItemTypeEnd - ItemTypeStart );
                float totalSize = elementSize * TotalCount;
                float offset = m_contentBounds.min.x - elementSize * ItemTypeStart;

                newLocalPosition += m_viewBounds.min.x - value * ( totalSize - m_viewBounds.size[axis] ) - offset;
            }
            else if(axis == 1)
            {
                float elementSize = m_contentBounds.size.y / ( ItemTypeEnd - ItemTypeStart );
                float totalSize = elementSize * TotalCount;
                float offset = m_contentBounds.max.y + elementSize * ItemTypeStart;

                newLocalPosition -= offset - value * ( totalSize - m_viewBounds.size.y ) - m_viewBounds.max.y;
            }

            if(Mathf.Abs(localPosition[axis] - newLocalPosition) > 0.01f)
            {
                localPosition[axis] = newLocalPosition;
                content.localPosition = localPosition;
                Velocity[axis] = 0;
                UpdateBounds();
            }
        }

        private bool hScrollingNeeded
        {
            get
            {
                if(Application.isPlaying)
                    return m_contentBounds.size.x > m_viewBounds.size.x + 0.01f;
                return true;
            }
        }
        private bool vScrollingNeeded
        {
            get
            {
                if(Application.isPlaying)
                    return m_contentBounds.size.y > m_viewBounds.size.y + 0.01f;
                return true;
            }
        }

        private bool m_hSliderExpand;
        private bool m_vSliderExpand;
        private float m_hSliderHeight;
        private float m_vSliderWidth;
        private DrivenRectTransformTracker m_tracker;
        public virtual void SetLayoutHorizontal()
        {
            m_tracker.Clear();

            if(m_hSliderExpand || m_vSliderExpand)
            {
                m_tracker.Add(this, ViewRect,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.SizeDelta |
                    DrivenTransformProperties.AnchoredPosition);

                // 设置view为full来查看content是否合适
                ViewRect.anchorMin = Vector2.zero;
                ViewRect.anchorMax = Vector2.one;
                ViewRect.sizeDelta = Vector2.zero;
                ViewRect.anchoredPosition = Vector2.zero;

                // 在没有滚动条的情况下用这个大小重新计算内容布局，看看是否依然适合。
                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
                m_viewBounds = new Bounds(ViewRect.rect.center, ViewRect.rect.size);
                m_contentBounds = GetBounds();
            }

            // 如果非垂直滚动，则滚动条空间可以利用
            if(m_vSliderExpand && vScrollingNeeded)
            {
                ViewRect.sizeDelta = new Vector2(-( m_vSliderWidth + m_verticalScrollbarSpacing ), ViewRect.sizeDelta.y);

                // 当有垂直滚动条时利用此高度重新布局来查看内容是否依然合适（内容可能会被滚动条膨胀）
                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
                m_viewBounds = new Bounds(ViewRect.rect.center, ViewRect.rect.size);
                m_contentBounds = GetBounds();
            }

            // 如果非水平滚动，则滚动条空间可以被利用
            if(m_hSliderExpand && hScrollingNeeded)
            {
                ViewRect.sizeDelta = new Vector2(ViewRect.sizeDelta.x, -( m_hSliderHeight + m_horizontalScrollbarSpacing ));
                m_viewBounds = new Bounds(ViewRect.rect.center, ViewRect.rect.size);
                m_contentBounds = GetBounds();
            }

            // 如果垂直和水平滚动条第一次都没有启动，我们需要再次检查垂直滚动条是否需要开启
            // 如果不开启就利用空余出来的空间
            if(m_vSliderExpand && vScrollingNeeded && Mathf.Abs(ViewRect.sizeDelta.x) < 0.001f && ViewRect.sizeDelta.y < 0)
            {
                ViewRect.sizeDelta = new Vector2(-( m_vSliderWidth + m_verticalScrollbarSpacing ), ViewRect.sizeDelta.y);
            }
        }
        public virtual void SetLayoutVertical()
        {
            UpdateScrollbarLayout();
            m_viewBounds = new Bounds(ViewRect.rect.center, ViewRect.rect.size);
            m_contentBounds = GetBounds();
        }

        private RectTransform m_horizontalScrollbarRect;
        private RectTransform m_verticalScrollbarRect;
        private void UpdateScrollbarVisibility()
        {
            if(m_verticalScrollbar && m_verticalScrollbarVisibility != ScrollbarVisibility.Permanent && m_verticalScrollbar.gameObject.activeSelf != vScrollingNeeded)
                m_verticalScrollbar.gameObject.SetActive(vScrollingNeeded);

            if(m_horizontalScrollbar && m_horizontalScrollbarVisibility != ScrollbarVisibility.Permanent && m_horizontalScrollbar.gameObject.activeSelf != hScrollingNeeded)
                m_horizontalScrollbar.gameObject.SetActive(hScrollingNeeded);
        }
        private void UpdateScrollbarLayout()
        {
            if(m_vSliderExpand && m_horizontalScrollbar)
            {
                m_tracker.Add(this, m_horizontalScrollbarRect,
                    DrivenTransformProperties.AnchorMinX |
                    DrivenTransformProperties.AnchorMaxX |
                    DrivenTransformProperties.SizeDeltaX |
                    DrivenTransformProperties.AnchoredPositionX);
                m_horizontalScrollbarRect.anchorMin = new Vector2(0, m_horizontalScrollbarRect.anchorMin.y);
                m_horizontalScrollbarRect.anchorMax = new Vector2(1, m_horizontalScrollbarRect.anchorMax.y);
                m_horizontalScrollbarRect.anchoredPosition = new Vector2(0, m_horizontalScrollbarRect.anchoredPosition.y);
                m_horizontalScrollbarRect.sizeDelta = vScrollingNeeded
                                                    ? new Vector2(-( m_vSliderWidth + m_verticalScrollbarSpacing ), m_horizontalScrollbarRect.sizeDelta.y)
                                                    : new Vector2(0, m_horizontalScrollbarRect.sizeDelta.y);
            }

            if(m_hSliderExpand && m_verticalScrollbar)
            {
                m_tracker.Add(this, m_verticalScrollbarRect,
                    DrivenTransformProperties.AnchorMinY |
                    DrivenTransformProperties.AnchorMaxY |
                    DrivenTransformProperties.SizeDeltaY |
                    DrivenTransformProperties.AnchoredPositionY);
                m_verticalScrollbarRect.anchorMin = new Vector2(m_verticalScrollbarRect.anchorMin.x, 0);
                m_verticalScrollbarRect.anchorMax = new Vector2(m_verticalScrollbarRect.anchorMax.x, 1);
                m_verticalScrollbarRect.anchoredPosition = new Vector2(m_verticalScrollbarRect.anchoredPosition.x, 0);
                m_verticalScrollbarRect.sizeDelta = hScrollingNeeded
                                                  ? new Vector2(m_verticalScrollbarRect.sizeDelta.x, -( m_hSliderHeight + m_horizontalScrollbarSpacing ))
                                                  : new Vector2(m_verticalScrollbarRect.sizeDelta.x, 0);
            }
        }
        private void UpdateScrollbars(Vector2 offset)
        {
            if(m_horizontalScrollbar)
            {

                if(m_contentBounds.size.x > 0 && TotalCount > 0)
                {
                    m_horizontalScrollbar.size = Mathf.Clamp01(( m_viewBounds.size.x - Mathf.Abs(offset.x) ) / m_contentBounds.size.x * ( ItemTypeEnd - ItemTypeStart ) / TotalCount);
                }

                else
                    m_horizontalScrollbar.size = 1;

                m_horizontalScrollbar.value = HorizontalNormalizedPosition;
            }

            if(m_verticalScrollbar)
            {

                if(m_contentBounds.size.y > 0 && TotalCount > 0)
                {
                    m_verticalScrollbar.size = Mathf.Clamp01(( m_viewBounds.size.y - Mathf.Abs(offset.y) ) / m_contentBounds.size.y * ( ItemTypeEnd - ItemTypeStart ) / TotalCount);
                }

                else
                    m_verticalScrollbar.size = 1;

                m_verticalScrollbar.value = VerticalNormalizedPosition;
            }
        }
        private void UpdatePrevData()
        {
            m_prevPosition = content == null ? Vector2.zero : content.anchoredPosition;
            m_prevViewBounds = m_viewBounds;
            m_prevContentBounds = m_contentBounds;
        }
        private void UpdateCachedData()
        {
            var trans = transform;
            m_horizontalScrollbarRect = m_horizontalScrollbar == null ? null : m_horizontalScrollbar.transform as RectTransform;
            m_verticalScrollbarRect = m_verticalScrollbar == null ? null : m_verticalScrollbar.transform as RectTransform;

            // 无论elements是子物体或不存在，viewIsChild始终为真
            var viewIsChild = ViewRect.parent == trans;
            bool hScrollbarIsChild;
            if(null != m_horizontalScrollbarRect)
            {
                hScrollbarIsChild = !m_horizontalScrollbarRect || m_horizontalScrollbarRect.parent == trans;
            }
            else
            {
                hScrollbarIsChild = !m_horizontalScrollbarRect;
            }

            bool vScrollbarIsChild;
            if(null != m_verticalScrollbarRect)
            {
                vScrollbarIsChild = !m_verticalScrollbarRect || m_verticalScrollbarRect.parent == trans;
            }
            else
            {
                vScrollbarIsChild = !m_verticalScrollbarRect;
            }

            bool allAreChildren = viewIsChild && hScrollbarIsChild && vScrollbarIsChild;

            m_hSliderExpand = allAreChildren && m_horizontalScrollbarRect && HorizontalScrollbarVisibility == ScrollbarVisibility.AutoHideAndExpandViewport;
            m_vSliderExpand = allAreChildren && m_verticalScrollbarRect && VerticalScrollbarVisibility == ScrollbarVisibility.AutoHideAndExpandViewport;
            m_hSliderHeight = m_horizontalScrollbarRect == null ? 0 : m_horizontalScrollbarRect.rect.height;
            m_vSliderWidth = m_verticalScrollbarRect == null ? 0 : m_verticalScrollbarRect.rect.width;
        }
        private void UpdateBounds(bool updateItems = true)
        {
            m_viewBounds = new Bounds(ViewRect.rect.center, ViewRect.rect.size);
            m_contentBounds = GetBounds();

            if(content == null)
                return;

            // Rebuild时不要执行此操作
            if(Application.isPlaying && updateItems && UpdateItems(m_viewBounds, m_contentBounds))
            {
                Canvas.ForceUpdateCanvases();
                m_contentBounds = GetBounds();
            }

            /*  
             *  在不进行填充时尽量确保content边界至少和view一样大。
             *  一般来说，如果content比view小，那么滚动应该是不允许的，因为那不是scrollview的正常工作方式。
             *  只有当content大于view时才可能滚动。
             *  我们使用content bounds的center来决定内容边界应该扩展的方向。
             *  如果pivot在顶部，边界就会向下扩展。
             *  当内容中使用ContentSizeFitter时也可以正常工作。
             */
            Vector3 contentSize = m_contentBounds.size;
            Vector3 contentPos = m_contentBounds.center;
            Vector3 excess = m_viewBounds.size - contentSize;
            if(excess.x > 0)
            {
                contentPos.x -= excess.x * ( content.pivot.x - 0.5f );
                contentSize.x = m_viewBounds.size.x;
            }
            if(excess.y > 0)
            {
                contentPos.y -= excess.y * ( content.pivot.y - 0.5f );
                contentSize.y = m_viewBounds.size.y;
            }

            m_contentBounds.size = contentSize;
            m_contentBounds.center = contentPos;
        }
        private void EnsureLayoutHasRebuilt()
        {
            if(!m_hasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout())
                Canvas.ForceUpdateCanvases();
        }

        private readonly Vector3[] m_corners = new Vector3[4];
        // 获取线框矩阵
        private Bounds GetBounds()
        {
            if(content == null)
                return new Bounds();

            var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            var toLocal = ViewRect.worldToLocalMatrix;
            content.GetWorldCorners(m_corners);
            for(int j = 0 ; j < 4 ; j++)
            {
                Vector3 v = toLocal.MultiplyPoint3x4(m_corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }

            var bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
            return bounds;
        }
        // content在view中的偏移
        private Vector2 CalculateOffset(Vector2 delta)
        {
            Vector2 offset = Vector2.zero;
            if(MoveType == MovementType.Unrestricted)
                return offset;

            Vector2 min = m_contentBounds.min;
            Vector2 max = m_contentBounds.max;

            if(Horizontal)
            {
                min.x += delta.x;
                max.x += delta.x;
                if(min.x > m_viewBounds.min.x)
                    offset.x = m_viewBounds.min.x - min.x;
                else if(max.x < m_viewBounds.max.x)
                    offset.x = m_viewBounds.max.x - max.x;
            }

            if(Vertical)
            {
                min.y += delta.y;
                max.y += delta.y;
                if(max.y < m_viewBounds.max.y)
                    offset.y = m_viewBounds.max.y - max.y;
                else if(min.y > m_viewBounds.min.y)
                    offset.y = m_viewBounds.min.y - min.y;
            }

            return offset;
        }
        private static float RubberDelta(float overStretching, float viewSize)
        {
            return ( 1 - ( 1 / ( ( Mathf.Abs(overStretching) * 0.55f / viewSize ) + 1 ) ) ) * viewSize * Mathf.Sign(overStretching);
        }

        #endregion
    }
}
