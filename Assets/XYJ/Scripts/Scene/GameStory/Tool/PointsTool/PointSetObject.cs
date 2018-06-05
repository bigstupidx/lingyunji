using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    /// <summary>
    /// 单纯的点集合对象
    /// </summary>
    public class PointSetObject : PointsObject<Points>
    {
        // Use this for initialization
        void Start()
        {
            //InitMouseInput();
        }

        // Update is called once per frame
        void Update()
        {
            //UpdateMouseInput();
        }

        void FixedUpdate()
        {
            UpdateKeyInput();
        }

        public override int GetSampleType()
        {
            return 1;
        }

        #region static methods

        /// <summary>
        /// 创建点集对象
        /// </summary>
        /// <param name="orgPos"></param>
        /// <returns></returns>
        public static PointSetObject Create(Vector3 orgPos)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            PointSetObject pointSet = go.AddComponent<PointSetObject>();
            go.name = pointSet.objectName;
            go.transform.parent = PointsRoot;
            if (Vector3.zero == orgPos)
                go.transform.position = pointSet.samplePosition;
            else
                go.transform.position = orgPos;

            return pointSet;
        }
        public static PointSetObject Create()
        {
            // 暂时设置原点为初始位置
            return Create(Vector3.zero);
        }

        #endregion
    }

}
