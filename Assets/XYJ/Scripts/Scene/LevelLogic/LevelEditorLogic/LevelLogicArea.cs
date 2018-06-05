#if !USE_HOT
namespace xys.hot
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// 关卡逻辑中的区域
    /// </summary>
    public class LevelLogicArea
    {
        public LevelDesignConfig.LevelAreaData m_areaData;
        public bool isActive { get; private set; }

        List<Matrix4x4> m_rectMatrixList = new List<Matrix4x4>();

        public LevelLogicArea(LevelDesignConfig.LevelAreaData areaData)
        {
            m_areaData = areaData;
            InitArea();
        }

        /// <summary>
        /// 设置区域是否激活
        /// </summary>
        public void SetActive(bool active)
        {
            isActive = active;
        }

        /// <summary>
        /// 初始化区域
        /// </summary>
        void InitArea()
        {
            isActive = m_areaData.m_isInitOpen;
            for (int i = 0; i < m_areaData.m_types.Count; ++i)
            {
                if (m_areaData.m_types[i] == LevelDesignConfig.LevelAreaData.AreaType.Rect)
                {
                    Matrix4x4 matrix = Matrix4x4.TRS(m_areaData.m_postions[i], Quaternion.Euler(m_areaData.m_dirs[i]), m_areaData.m_scales[i]).inverse;
                    m_rectMatrixList.Add(matrix);
                }
            }
        }

        /// <summary>
        /// 释放该区域
        /// </summary>
        void Release()
        {
            m_areaData = null;
            m_rectMatrixList.Clear();
        }

        /// <summary>
        /// 判断坐标是否在区域内
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsInArea(Vector3 pos)
        {
            if (!isActive)
                return false;

            if (null != m_areaData && null != m_areaData.m_types && m_areaData.m_types.Count > 0)
            {
                for (int i = 0; i < m_areaData.m_types.Count; ++i)
                {
                    LevelDesignConfig.LevelAreaData.AreaType areaType = m_areaData.m_types[i];
                    if (areaType == LevelDesignConfig.LevelAreaData.AreaType.Rect)
                    {
                        //矩形区域
                        //先通过矩阵旋转到单位位置
                        Vector3 local = m_rectMatrixList[i].MultiplyPoint3x4(pos);
                        Bounds bounds = new Bounds(m_areaData.m_centers[i], m_areaData.m_sizes[i]);
                        if (bounds.Contains(local))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        //圆形区域
                        Vector3 areaPos = m_areaData.m_postions[i] + m_areaData.m_centers[i];
                        pos = (areaPos - pos);
                        float max = GetVector3Max(m_areaData.m_scales[i]);
                        float radius = m_areaData.m_radiuses[i] * max;
                        if (pos.sqrMagnitude <= radius * radius)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        float GetVector3Max(Vector3 scale)
        {
            float max = int.MinValue;
            if (scale.x > max)
            {
                max = scale.x;
            }
            if (scale.y > max)
            {
                max = scale.y;
            }
            if (scale.z > max)
            {
                max = scale.z;
            }

            return max;
        }
    }
}
#endif