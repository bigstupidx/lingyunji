namespace xys
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Config;
    using xys.UI;

    /// <summary>
    /// 创建对象的数据
    /// </summary>
    public class LcoalActorCxt
    {
        public int roleId;
        public Vector3 bornPos;
        public float rotateAngle;
        public string name;
        public object dataEx;
    }

    /// <summary>
    /// 使用角色表的角色类型
    /// </summary>
    public abstract class LocalActorBase : ILocalObject
    {
        public string name { get; protected set; }

        public RoleDefine cfgInfo { get; protected set; }

        public Vector3 bornPosition { get; protected set; }

        public float rotateAngle { get; protected set; }

        public Vector3 curPos
        {
            get
            {
                return modelEntity.rootObj.position;
            }
        }

        // ===============扩展模块================
        // 模型实例
        public LocalModelEntity modelEntity { get; protected set; }

        #region 提供给外部使用的接口

        public void SetPosition(Vector3 pos)
        {
            modelEntity.rootObj.localPosition = pos;
        }

        public void SetAngle(float angle)
        {
            rotateAngle = angle;
            modelEntity.rootObj.localEulerAngles = Vector3.up * rotateAngle;
        }

        /// <summary>
        /// 更换角色id
        /// </summary>
        /// <param name="roleId"></param>
        public void ReplaceModelObject(int roleId)
        {
            cfgInfo = RoleDefine.Get(roleId);
            modelEntity.ReloadModel();
        }

        #endregion

        #region Virtual Methods

        protected virtual void OnInit(object data)
        {
            
        }

        protected virtual void OnDestory()
        {

        }

        protected virtual void OnUpdate()
        {

        }

        protected virtual void OnPlayerClick()
        {

        }

        #endregion


        #region Interface Methods

        public int uid { get; protected set; }// 角色唯一id，自动生成

        public int roleId { get; protected set; }// 角色表id

        public object initData { get; protected set; }// 可以通过该对象传位置信息，名字等

        public void InitData(int uid, object data)
        {
            this.uid = uid;
            this.initData = data;
            LcoalActorCxt cxt = (LcoalActorCxt)data;
            this.roleId = cxt.roleId;
            this.name = cxt.name;
            this.bornPosition = cxt.bornPos;
            this.rotateAngle = cxt.rotateAngle;

            cfgInfo = RoleDefine.Get(cxt.roleId);
            if (string.IsNullOrEmpty(name))
                name = cfgInfo.name;

            OnInit(data);
            modelEntity = new LocalModelEntity(this);
        }

        public void Destroy()
        {
            modelEntity.Destroy();
            OnDestory();
        }

        public void Update()
        {
            OnUpdate();
        }

        /// <summary>
        /// 处理玩家点击角色
        /// </summary>
        public void HandlePlayerClick()
        {
            OnPlayerClick();
        }

        #endregion

    }
}
