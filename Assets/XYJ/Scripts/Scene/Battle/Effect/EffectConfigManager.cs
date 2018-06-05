using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys;

namespace Config
{
    public partial class EffectConfig
    {

        //创建特效,有回调
        public void CreateEffect(Transform trans, Action<GameObject, object> callback = null, object para = null, float scale = 1.0f )
        {
            if (string.IsNullOrEmpty(m_effectName))
                return;

            Transform bone = BoneManage.GetBone(trans, m_boneName);
            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            GetCreateInfo(trans,bone, ref pos, ref rot);

            XYJObjectPool.LoadPrefab(m_effectName, OnFinishCreateEffect, new object[] { bone, callback, para, scale }, pos, rot);
        }

        public PrefabsLoadReference CreateEffectReference(Transform trans, Action<GameObject, object> callback = null, object para = null, float scale = 1.0f)
        {
            if (string.IsNullOrEmpty(m_effectName))
                return null;

            PrefabsLoadReference loadRef = new PrefabsLoadReference(true);

            Transform bone = BoneManage.GetBone(trans, m_boneName);
            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            GetCreateInfo(trans, bone, ref pos, ref rot);

            loadRef.Load(m_effectName, OnFinishCreateEffect, new object[] { bone, callback, para, scale }, pos, rot);
            return loadRef;
        }


        //播放材质特效
        public ILoadReference PlayMaterialEffect(IObject obj)
        {
            if (string.IsNullOrEmpty(material))
                return null;
            MaterialLoadReference loadref = new MaterialLoadReference();
            loadref.Load(material, OnFinishLoadMaterial, obj);
            return loadref;
        }

        void GetCreateInfo(Transform root, Transform bone, ref Vector3 pos, ref Quaternion rot)
        {
            if (bone == null)
                bone = root;
            pos = bone.position;
            rot = bone.rotation;
            if (m_rotCreateZero)
                rot = Quaternion.identity;
        }


   
        void OnFinishCreateEffect(GameObject go, object para)
        {
            object[] pList = para as object[];

            Transform trans = (Transform)pList[0];
            Action<GameObject, object> callback = (Action<GameObject, object>)pList[1];
            object callbackPara = (object)pList[2];
            float scale = (float)pList[3];
            go.transform.localScale = Vector3.one * scale;
            //跟随
            if (m_followPos || m_followRot)
                EffectFollowBone.AddFollow(go, trans, m_followRot, Vector3.zero, false, m_followPos);

            //回调
            if (callback != null)
                callback(go, callbackPara);
        }

        void OnFinishLoadMaterial(Material mat, object para)
        {
            IObject obj = para as IObject;
            if(mat != null && obj!=null && obj.isAlive)
            {
                if (matAddType == 0)
                    obj.battle.actor.m_partManager.ChangeMaterial(mat);
                else
                    obj.battle.actor.m_partManager.AddMaterial(mat);
            }
        }

        static void OnLoadEnd()
        {
            foreach (var p in DataList.Values)
            {
                if (string.IsNullOrEmpty(p.m_effectName) && string.IsNullOrEmpty(p.material))
                    XYJLogger.LogError("特效没有配模型或材质 id=" + p.id);
            }
        }
    }
}
