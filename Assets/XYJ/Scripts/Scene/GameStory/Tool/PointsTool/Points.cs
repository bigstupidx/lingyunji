using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{

    [System.Serializable]
    public class Points
    {

        #region Internal Values

        // 路径id
        public string id = string.Empty;

        // 描述或备注
        public string describe = string.Empty;

        // 位置和旋转信息必须同时存在
        // Positions
        [SerializeField]
        public List<Vector3> positions = new List<Vector3>();
        // Rotations
        [SerializeField]
        public List<Quaternion> rotations = new List<Quaternion>();

        #endregion

        /// <summary>
        /// 路点数量
        /// </summary>
        public int Count
        {
            get { return positions.Count; }
        }

        #region Public Points Methods

        /// <summary>
        /// 获取点信息
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        public void Get(int index, out Vector3 pos, out Quaternion rot)
        {
            if (positions.Count > 0 && index >= 0 && index < positions.Count)
                pos = positions[index];
            else
                pos = Vector3.zero;

            if (rotations.Count > 0 && index >= 0 && index < rotations.Count)
                rot = rotations[index];
            else
                rot = Quaternion.identity;
        }
        public Vector3 GetPosition(int index)
        {
            if (positions.Count > 0 && index >= 0 && index < positions.Count)
                return positions[index];
            else
                return Vector3.zero;
        }
        public Quaternion GetRotation(int index)
        {
            if (rotations.Count > 0 && index >= 0 && index < rotations.Count)
                return rotations[index];
            else
                return Quaternion.identity;
        }

        /// <summary>
        /// 设置点信息
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        public void Set(int index, Vector3 pos, Quaternion rot)
        {
            if (positions.Count > 0 && index >= 0 && index < positions.Count)
                positions[index] = pos;

            if (rotations.Count > 0 && index >= 0 && index < rotations.Count)
                rotations[index] = rot;
        }
        public void SetPosition(int index, Vector3 pos)
        {
            if (positions.Count > 0 && index >= 0 && index < positions.Count)
                positions[index] = pos;
        }
        public void SetRotation(int index, Quaternion rot)
        {
            if (rotations.Count > 0 && index >= 0 && index < rotations.Count)
                rotations[index] = rot;
        }

        /// <summary>
        /// 添加点
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        public void Add(Vector3 pos, Quaternion rot)
        {
            positions.Add(pos);
            rotations.Add(rot);
        }
        public void Add(Vector3 pos)
        {
            Add(pos, Quaternion.identity);
        }

        /// <summary>
        /// 插入点
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        public void Insert(int index, Vector3 pos, Quaternion rot)
        {
            positions.Insert(index, pos);
            rotations.Insert(index, rot);
        }
        public void Insert(int index, Vector3 pos)
        {
            Insert(index, pos, Quaternion.identity);
        }

        /// <summary>
        /// 移除点
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            if (index >= 0 && index < positions.Count)
            {
                positions.RemoveAt(index);
                rotations.RemoveAt(index);
            }
        }

        /// <summary>
        /// 将某个点移到某个位置
        /// </summary>
        /// <param name="sourceIndex"></param>
        /// <param name="targetIndex"></param>
        public void Move(int sourceIndex, int targetIndex)
        {
            if (sourceIndex != targetIndex &&
                sourceIndex >= 0 && sourceIndex <= positions.Count &&
                targetIndex >= 0 && targetIndex <= positions.Count)
            {
                Insert(targetIndex, positions[sourceIndex], rotations[sourceIndex]);
                RemoveAt(sourceIndex);
            }
        }

        #endregion

        /// <summary>
        /// 清空所有点
        /// </summary>
        public virtual void ClearAll()
        {
            positions.Clear();
            rotations.Clear();
        }
    }

}
