using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    /// <summary>
    /// 用来编辑路径的对象
    /// </summary>
    public class WayPointsObject : PointsObject<WayPoints>
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void FixedUpdate()
        {
            UpdateKeyInput();
        }

        public override int GetSampleType()
        {
            return 1;
        }

        #region Override Methods

        public override void Add()
        {
            base.Add();

            editData.AddWayData();
        }

        public override void Insert(int index)
        {
            base.Insert(index);

            editData.InsertWayData(index);
        }

        public override void RemoveAt(int index)
        {
            base.RemoveAt(index);

            editData.RemoveAt(index);
        }

        public override void Move(int sourceIndex, int targetIndex)
        {
            base.Move(sourceIndex, targetIndex);

            editData.MoveWayData(sourceIndex, targetIndex);
        }

        #endregion

        #region static methods


        /// <summary>
        /// 创建路点对象
        /// </summary>
        /// <param name="orgPos"></param>
        /// <returns></returns>
        public static WayPointsObject Create(Vector3 orgPos)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            WayPointsObject wayPoints = go.AddComponent<WayPointsObject>();
            go.name = wayPoints.objectName;
            go.name = wayPoints.objectName;
            go.transform.parent = PointsRoot;
            if (Vector3.zero == orgPos)
                go.transform.position = wayPoints.samplePosition;
            else
                go.transform.position = orgPos;

            return wayPoints;
        }
        public static WayPointsObject Create()
        {
            return Create(Vector3.zero);
        }

        #endregion

    }

}
