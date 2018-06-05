using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public class StoryEventPlayCamAnim : StoryEventBase
    {
        
        StoryEventDataParam m_config;

        #region Camera Animation Values

        Camera m_mainCam;
        Animation m_camAni;
        AnimationClip m_camAnimClip;

        Vector3 orgPos = Vector3.zero;
        Quaternion orgRot = Quaternion.identity;

        const string CamAnimName = "StoryEventPlayCamAnim";

        #endregion

        // 恢复镜头

        /// <summary>
        /// 执行事件
        /// </summary>
        public override void OnFire()
        {
            Debug.Log("StoryEvent.Fire : " + type);
            m_config = eventData as StoryEventDataParam;
            if (m_config == null)
                return;
            CamPoints camPoints = GetCamPoints(m_config.m_param);
            if (camPoints == null)
                return;

            // 生成动画clip
            if (m_camAnimClip == null)
            {
                m_camAnimClip = new AnimationClip();
                m_camAnimClip.name = CamAnimName;
                m_camAnimClip.legacy = true;
                m_camAnimClip.wrapMode = WrapMode.Default;
            }
            camPoints.BuildCameraAnimClip(ref m_camAnimClip);

            // 获取镜头和动画组件
            if (m_mainCam == null)
                m_mainCam = Camera.main;
            if (m_mainCam == null)
                return;
            if (m_camAni == null)
                m_camAni = m_mainCam.GetComponent<Animation>();
            if (m_camAni == null)
                m_camAni = m_mainCam.gameObject.AddComponent<Animation>();

            orgPos = m_camAni.transform.position;
            orgRot = m_camAni.transform.rotation;

            // 播放动画
            //m_camAni.RemoveClip(CamAnimName);
            m_camAni.AddClip(m_camAnimClip, CamAnimName);
            m_camAni.Stop();
            m_camAni.Play(CamAnimName);
        }

        /// <summary>
        /// 退出事件
        /// </summary>
        public override void OnExit()
        {
            Debug.Log("StoryEvent.Exit : " + type);
            if (m_camAni.isPlaying)
                m_camAni.Stop();

            if (HasFire)
            {
                m_camAni.transform.position = orgPos;
                m_camAni.transform.rotation = orgRot;
            }
        }

        /// <summary>
        /// 更新事件
        /// </summary>
        /// <param name="timePass"></param>
        public override void Update(float timePass)
        {
            //Debug.Log("StoryEvent.Update : " + type);
        }

        /// <summary>
        /// 暂停事件
        /// </summary>
        public override void OnPause()
        {
            Debug.Log("StoryEvent.OnPause : " + type);
        }

        /// <summary>
        /// 恢复事件
        /// </summary>
        public override void OnResume()
        {
            Debug.Log("StoryEvent.OnResume : " + type);
        }

        /// <summary>
        /// 停止事件
        /// </summary>
        public override void OnStop()
        {
            Debug.Log("StoryEvent.OnStop : " + type);
            if (m_camAni.isPlaying)
                m_camAni.Stop();

            if (HasFire)
            {
                m_camAni.transform.position = orgPos;
                m_camAni.transform.rotation = orgRot;
            }
        }
    }
}
