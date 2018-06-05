namespace xys
{
    using UnityEngine;
    using System.Collections.Generic;
    using UIWidgets;
    using UI;
    using UnityEngine.UI;

    public enum CircularType
    {
        Left,           //显示在左侧
        Right,          //显示在右侧
    }

    //圆形滑动控件
    [RequireComponent(typeof(ListViewIcons))]
    [RequireComponent(typeof(UnityEngine.UI.ScrollRect))]
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    [RequireComponent(typeof(UnityEngine.UI.Mask))]
    public class CircularController : MonoBehaviour
    {
        [SerializeField]
        public Transform m_circleCenter;
        [SerializeField]
        public RectTransform m_layout;
        [SerializeField]
        public GameObject m_defaultItem;
        [SerializeField]
        public float Radius
        {
            get { return m_radius / 1000f; }
            set
            {
                m_radius = value * 1000f;
                if (Mathf.Abs(m_radius) < Screen.height / 2)
                {
                    m_radius = Screen.height / 2;
                }
            }
        }
        private float m_radius = 1000;
        [SerializeField]
        public CircularType m_ciraularType = CircularType.Right;

        public List<CircularItem> m_itemList = new List<CircularItem>();

        //初始化
        void Awake()
        {

        }

        void Start()
        {
            CircularItem[] items = transform.GetComponentsInChildren<CircularItem>();
            m_itemList = new List<CircularItem>(items);
        }

        void Update()
        {
            if (null != m_itemList && m_itemList.Count > 0)
            {
                for (int i = 0; i < m_itemList.Count; ++i)
                {
                    //转换到圆心层级
                    Vector3 itemPos = transform.InverseTransformPoint(m_itemList[i].transform.position);
                    float tempY = itemPos.y - m_circleCenter.localPosition.y;
                    float tempX = Mathf.Sqrt(m_radius * m_radius - tempY * tempY);
                    if (float.IsNaN(tempX)) continue;
                    Vector3 targetPos = transform.TransformPoint(m_circleCenter.localPosition + new Vector3(tempX, tempY, 0.0f));
                    m_itemList[i].CircleMoveItem.localPosition = m_itemList[i].transform.InverseTransformPoint(targetPos);
                }
            }
        }

        //增加一个条目
        public void AddItem(CircularItem item)
        {
            if (null == m_itemList)
            {
                m_itemList = new List<CircularItem>();
            }
            m_itemList.Add(item);
        }

        //设置所有条目
        public void SetItems(List<CircularItem> list)
        {
            m_itemList = list;
        }

        //初始化控件
        public void InitComponent()
        {
            GetComponent<ListViewIcons>().ScrollRect = GetComponent<ScrollRect>();
            GetComponent<ScrollRect>().content = m_layout;
        }

        //设置圆心
        public void SetCircleCenter(object trans)
        {
            m_circleCenter = trans as Transform;
        }

        //设置layout
        public void SetLayout(object trans)
        {
            m_layout = trans as RectTransform;
            m_layout.gameObject.AddMissingComponent<UnityEngine.UI.ContentSizeFitter>();
            m_layout.gameObject.AddMissingComponent<EasyLayout.EasyLayout>();

            GetComponent<ListViewIcons>().Container = m_layout;
        }

        //设置默认对象
        public void SetDefaultItem(object item)
        {
            m_defaultItem = item as GameObject;
            CircularItem ci = m_defaultItem.AddMissingComponent<CircularItem>();

            //创建必要的子物体
            GameObject icon = CreateObj(m_defaultItem.transform, "Icon");
            GameObject text = CreateObj(m_defaultItem.transform, "Text");
            ci.Icon = icon.AddMissingComponent<Image>();
            ci.Text = text.AddMissingComponent<Text>();
            ci.SetNativeSize = false;

            GetComponent<ListViewIcons>().DefaultItem = ci;
        }

        public GameObject CreateObj(Transform parent, string name)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            return go;
        }

        //设置半径
        public void SetRadius(float radius)
        {
            Radius = radius;
            m_circleCenter.transform.localPosition = new Vector3(-m_radius, 0, 0);
        }

        //设置类型
        public void SetCircularType(System.Enum type)
        {
            m_ciraularType = (CircularType)type;
        }
    }
}