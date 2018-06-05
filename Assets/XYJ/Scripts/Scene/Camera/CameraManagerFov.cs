using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys
{

    //摄像机fov变化
    public partial class CameraManager : MonoBehaviour
    {
        private float m_fovSpeed;               //改变fov的速度
        private float m_fovGoalValue;           //改变fov的目标值
        private float m_fovStayTime;            //保持fov目标值的时间
        private float m_fovStartTime;           //还原fov的初始时间
        private float m_fovFinishTime;          //还原fov的最终时间
        private bool b_isLarge;                 //fov是变大变小

        float m_timePass;
        bool m_fovChangeFinish = false;
        bool m_autoReset = false;

        //恢复到默认fov
        public void FadeToDefaultFov(float speed = 20)
        {
            ChangeCameraFov(m_defaultFov, speed, 0);
        }

        //直接设置fov
        public void SetToDefaultFov()
        {
            if (null != m_mainCamera)
            {
                m_mainCamera.fieldOfView = m_defaultFov;
                m_fovSpeed = 0;
            }
            else
            {
                Debug.LogError("设置fov主射相机为空");
            }
        }

        //改变maincamera 的视野范围
        public void ChangeCameraFov(float value, float speed, float stayTime, bool autoReset = false)
        {
            m_fovSpeed = speed;
            m_fovGoalValue = value;
            m_fovStayTime = stayTime;
            m_autoReset = autoReset;
            m_fovFinishTime = 0;
            m_fovChangeFinish = false;
            m_timePass = 0;
        }

        //改变fov(每帧调用)
        private void ExecuteChangeCameraFov()
        {
            if (m_duringSkill)
                return;

            float value = m_fovSpeed * Time.deltaTime;
            m_timePass += Time.deltaTime;
            float dis = m_fovGoalValue - m_mainCamera.fieldOfView;
            if (dis > 0)
                b_isLarge = true;
            else
                b_isLarge = false;

            if (Mathf.Abs(value) > Mathf.Abs(dis))
            {
                m_mainCamera.fieldOfView = m_fovGoalValue;
                //判断是否需要等待时间
                if (m_fovStayTime > 0)
                {
                    m_fovStartTime = m_timePass;
                    m_fovFinishTime = m_fovStartTime + m_fovStayTime;
                    m_fovStayTime = -1;
                }
                else
                {
                    //无需等待，则fov变化结束
                    m_fovChangeFinish = true;
                    if (m_autoReset)
                    {
                        FadeToDefaultFov(m_fovSpeed);
                    }
                }
            }
            else
            {
                if (b_isLarge)
                {
                    m_mainCamera.fieldOfView += m_fovSpeed * Time.deltaTime;
                }
                else
                {
                    m_mainCamera.fieldOfView -= m_fovSpeed * Time.deltaTime;
                }
            }
            //if(m_playerCamera.GetComponent<Camera>().enabled)
            //    m_playerCamera.GetComponent<Camera>().fieldOfView = m_mainCamera.fieldOfView;
        }

        //更新fov值
        private void UpdateFieldOfView()
        {
            //fov变化结束
            if (m_fovChangeFinish)
            {
                return;
            }
            ExecuteChangeCameraFov();
            if (m_fovFinishTime > 0 && m_timePass > m_fovFinishTime)
            {
                //保持在fov的时间结束，返回默认fov
                FadeToDefaultFov(m_fovSpeed);
            }
        }
    }

}