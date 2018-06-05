using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;

namespace xys.GameStory
{
    public class ModelComponent : IObjectComponent
    {
        public StoryObjectBase m_obj;

        public ModelPartManage m_modelMananger;

        public ModelUIHangPointHandler m_handler;

        Transform rootObject { get { return m_obj == null ? null : m_obj.ComHandler.rootObject; } }

        public void SetAngle(float angle)
        {
            if (rootObject!=null)
            {
                rootObject.localEulerAngles = Vector3.up * angle;
            }
        }

        public void PlayFx(string fxName, int followType=0, string followBone="")
        {
            if (string.IsNullOrEmpty(fxName))
                return;

            Transform bone = BoneManage.GetBone(rootObject, followBone);
            if (bone == null)
            {
                bone = rootObject;
            }

            XYJObjectPool.LoadPrefab(fxName, OnLoadFxEnd, new object[] { bone, followType});
        }

        void OnLoadFxEnd(GameObject go, object para)
        {
            object[] pList = para as object[];

            Transform trans = (Transform)pList[0];

            //跟随
            int followType = (int)pList[1];
            if (followType == 1 || followType == 2)
                EffectFollowBone.AddFollow(go, trans, followType == 2, Vector3.zero, false);

            //回调
            //System.Action<GameObject, object> callback = (System.Action<GameObject, object>)pList[2];
            //object callbackPara = (object)pList[3];
            //if (callback != null)
            //    callback(go, callbackPara);
        }

        #region 动画相关接口

        public void PlayStateAni()
        {
            if (m_modelMananger == null)
                return;

            StoryObjectState state = m_obj.ComHandler.m_state.CurState;
            string aniName = string.Empty;
            float aniSpeed = 1.0f;
            if (state == StoryObjectState.Move || state == StoryObjectState.Path)
            {
                aniName = m_obj.ComHandler.m_posture.m_cfg.normalRun;
                // 移动动画的播放速率
                //aniSpeed = m_obj.cfgInfo.speed;
            }
            else
            {
                aniName = m_obj.ComHandler.m_posture.m_cfg.normalIdle;
            }
            PlayAnim(aniName, aniSpeed);
        }

        public void PlayAnim(string name, float speed = 1.0f, float normalizedTime = 0.0f, bool isLoop = false)
        {
            if (m_modelMananger == null)
                return;
            m_modelMananger.PlayAnim(name, speed, normalizedTime, isLoop);
        }

        public void SetAnimSpeed(float speed)
        {
            if (m_modelMananger == null)
                return;
            m_modelMananger.SetAnimSpeed(speed);
        }

        public float GetAnimLength(string name)
        {
            if (m_modelMananger == null)
                return 0.0f;
            return m_modelMananger.GetAnimLength(name);
        }

        #endregion

        public void OnAwake(StoryObjectBase obj)
        {
            m_obj = obj;

            m_modelMananger = new ModelPartManage();
            m_handler = new ModelUIHangPointHandler();
            m_modelMananger.LoadModel(m_obj.model, OnLoadModelEnd);
        }

        void OnLoadModelEnd(GameObject go)
        {
            if (rootObject == null)
                return;

            go.transform.parent = rootObject;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            m_handler.Init(m_modelMananger.TitleObject);
            // 这里设置title挂点
            m_handler.SetName(m_obj.name);
        }

        public void OnStart()
        {

        }

        public void OnDestroy()
        {
            if (m_handler!=null)
            {
                m_handler.Destory();
                m_handler = null;
            }

            m_modelMananger.Destroy();
            m_modelMananger = null;
        }

        public void OnUpdate()
        {

        }

    }

}