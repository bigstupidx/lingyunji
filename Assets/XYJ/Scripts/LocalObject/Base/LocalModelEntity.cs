namespace xys
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Config;
    using xys.UI;

    /// <summary>
    /// 模型实体，包含模型，挂点，和移动控制机
    /// 可以支持：模型相关功能，动画和移动等
    /// </summary>
    public class LocalModelEntity
    {

        LocalActorBase m_obj;

        CharacterController m_characterCtrl;// 移动控制器，控制root对象移动

        public Transform rootObj { get; protected set; }

        public LocalObjectMono monoObj { get; protected set; }

        public ModelPartManage m_modelMananger;// 模型

        public ModelUIHangPointHandler m_handler;// 挂点

        #region 接口

        // 模型动画

        // 移动控制
        
        // 销毁
        public void Destroy()
        {
            m_handler.Destory();
            m_modelMananger.Destroy();
            if (rootObj != null)
            {
                GameObject.Destroy(rootObj.gameObject);
            }
        }

        public void ReloadModel()
        {
            m_modelMananger.Destroy();
            m_handler.Destory();
            m_modelMananger.LoadModel(m_obj.cfgInfo.model, LoadModelEnd, m_obj.cfgInfo.modelScale);
        }

        #endregion

        public LocalModelEntity (LocalActorBase obj)
        {
            m_obj = obj;

            // 创建跟root对象
            rootObj = new GameObject(string.Format("{0}-{1}(Local)", m_obj.roleId, m_obj.uid)).transform;
            rootObj.position = m_obj.bornPosition;
            monoObj = rootObj.gameObject.AddComponent<LocalObjectMono>();
            monoObj.Init(m_obj);

            // 创建控制器
            m_characterCtrl = rootObj.AddComponentIfNoExist<CharacterController>();
            m_characterCtrl.center = new Vector3(0, m_characterCtrl.height / 2, 0);
            rootObj.gameObject.layer = ComLayer.Layer_NoColliderRole;

            // 模型组件
            m_modelMananger = new ModelPartManage();
            m_modelMananger.LoadModel(m_obj.cfgInfo.model, LoadModelEnd, m_obj.cfgInfo.modelScale);

            m_handler = new ModelUIHangPointHandler();
        }

        void LoadModelEnd(GameObject go)
        {
            go.transform.parent = rootObj;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            m_handler.Init(m_modelMananger.TitleObject);
            m_handler.SetName(m_obj.name);
        }

    }
}
