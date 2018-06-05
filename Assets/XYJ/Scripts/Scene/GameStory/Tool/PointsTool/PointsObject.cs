using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    /// <summary>
    /// 点集合模版对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PointsObject<T> : MonoBehaviour where T : Points, new()
    {
        /// <summary>
        /// 编辑对象的数据
        /// </summary>
        [SerializeField]
        public T editData = new T();

        /// <summary>
        /// 对象名
        /// </summary>
        public string objectName
        {
            get
            {
                if (string.IsNullOrEmpty(editData.id))
                    editData.id = System.DateTime.Now.Ticks.ToString();
                return string.Format("{0}({1})", typeof(T).Name, editData.id);
            }
        }

        #region 编辑对象时用到的变量值

        [HideInInspector]
        public bool m_hasSelected = false;// 是否被选中，选中可以控制

        [HideInInspector]
        public bool showGizmos = true;
        [HideInInspector]
        public bool editPosition = true;
        [HideInInspector]
        public bool editRotation = false;

        // 运行时可以通过控制一个物体来编辑点
        public float m_moveSpeed = 10.0f;// 移动速度

        public bool toggle
        {
            get { return m_hasSelected; }
            set { m_hasSelected = value; }
        }
        List<bool> m_selectedToggles = new List<bool>();// 编辑器用
        public List<bool> selectedToggles
        {
            get {
                CheckToggles();
                return m_selectedToggles;
            }
            set { m_selectedToggles = value; }
        }

        /// <summary>
        /// 位置信息取样类型
        /// 0：取样自身对象，1：取样主角对象，2：取样摄像机
        /// </summary>
        /// <returns></returns>
        public virtual int GetSampleType() { return 0; }

        /// <summary>
        /// 取样位置值
        /// </summary>
        public Vector3 samplePosition
        {
            get
            {
                int sampleType = GetSampleType();
                if (sampleType == 1 && App.my != null && App.my.localPlayer != null && App.my.localPlayer.root != null)
                    return App.my.localPlayer.root.position;
                else if (sampleType == 2 && App.my != null && App.my.cameraMgr.m_mainCamera != null)
                    return App.my.cameraMgr.m_mainCamera.transform.position;
                return this.transform.position;
            }
        }

        /// <summary>
        /// 取样旋转值
        /// </summary>
        public Quaternion sampleRotation
        {
            get
            {
                if (GetSampleType() == 1 && App.my != null && App.my.localPlayer != null && App.my.localPlayer.root != null)
                    return App.my.localPlayer.root.rotation;
                else if (GetSampleType() == 2 && App.my != null && App.my.cameraMgr.m_mainCamera != null)
                    return App.my.cameraMgr.m_mainCamera.transform.rotation;
                return this.transform.rotation;
            }
        }

        #endregion

        public void CheckToggles()
        {
            if (editData.Count != m_selectedToggles.Count)
            {
                m_selectedToggles.Clear();
                for (int i=0; i<editData.Count; ++i)
                {
                    m_selectedToggles.Add(true);
                }
            }
        }
        public void SetToggles(bool toggle)
        {
            for (int i=0; i<m_selectedToggles.Count; ++i)
            {
                m_selectedToggles[i] = toggle;
            }
        }

        void OnEnable()
        {
            // 初始化
        }

        //void OnDrawGizmosSelected() { }
        protected virtual void OnDrawGizmos()
        {
            if (!showGizmos)
                return;

            if (editData.Count > 0)
            {
                Gizmos.color = Color.yellow;
                for (int i = 0; i < editData.Count; ++i)
                {
                    Vector3 pointPos = editData.GetPosition(i);
                    Quaternion pointRot = editData.GetRotation(i);

                    Gizmos.DrawCube(pointPos, Vector3.one * 0.2f);
                }

                Gizmos.color = Color.white;

                if (editData.Count > 1)
                {
                    for (int i = 0; i < editData.Count - 1; i++)
                    {
                        Vector3 from = editData.GetPosition(i);
                        Vector3 to = editData.GetPosition(i + 1);

                        Gizmos.DrawLine(from, to);
                    }
                }
            }
        }

        #region Input Operater Methods

        /// <summary>
        /// 取样数据
        /// </summary>
        protected virtual void SampleData()
        {
            Add();
        }

        protected virtual void UpdateKeyInput()
        {
            if (!m_hasSelected)
                return;

            if (Input.GetKeyUp(KeyCode.Space))
            {
                SampleData();
            }

            float lr = 0.0f;// 左右
            float fb = 0.0f;// 前后
            float ud = 0.0f;// 上下
            if (Input.GetKey(KeyCode.A))
            {
                lr = -1.0f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                lr = 1.0f;
            }

            if (Input.GetKey(KeyCode.W))
            {
                fb = 1.0f;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                fb = -1.0f;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                ud = 1.0f;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                ud = -1.0f;
            }

            Vector3 dir = transform.right * lr + transform.up * ud + transform.forward * fb;
            if (dir == Vector3.zero)
                return;
            this.transform.localPosition += dir * m_moveSpeed * Time.fixedDeltaTime;
        }

        #endregion

        #region Virtual Methods

        public virtual void Add()
        {
            CheckToggles();

            editData.Add(samplePosition, sampleRotation);

            m_selectedToggles.Add(true);
        }

        public virtual void Set (int index)
        {
            CheckToggles();

            editData.Set(index, samplePosition, sampleRotation);
        }

        public virtual void Insert(int index)
        {
            CheckToggles();

            editData.Insert(index, samplePosition, sampleRotation);

            m_selectedToggles.Insert(index, true);
        }

        public virtual void RemoveAt(int index)
        {
            CheckToggles();

            editData.RemoveAt(index);

            m_selectedToggles.RemoveAt(index);
        }

        public virtual void Move(int sourceIndex, int targetIndex)
        {
            CheckToggles();

            editData.Move(sourceIndex, targetIndex);

            if (sourceIndex != targetIndex &&
                sourceIndex >= 0 && sourceIndex <= m_selectedToggles.Count &&
                targetIndex >= 0 && targetIndex <= m_selectedToggles.Count)
            {
                bool tmp = m_selectedToggles[sourceIndex];
                m_selectedToggles.Insert(targetIndex, tmp);
                m_selectedToggles.RemoveAt(sourceIndex);
            }
        }

        public virtual void Clear()
        {
            editData.ClearAll();

            m_selectedToggles.Clear();
        }

        #endregion

        /// <summary>
        /// 点集合根对象
        /// </summary>
        static Transform _pointsRoot;
        public static Transform PointsRoot
        {
            get
            {
                if (_pointsRoot == null)
                {
                    GameObject root = GameObject.Find("[PointsRoot]");
                    if (root == null)
                    {
                        root = new GameObject("[PointsRoot]");
                    }
                    _pointsRoot = root.transform;
                    _pointsRoot.transform.position = Vector3.zero;
                }
                return _pointsRoot;
            }
        }

    }

}
