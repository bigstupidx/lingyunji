//
//SpingManager.cs for unity-chan!
//
//Original Script is here:
//ricopin / SpingManager.cs
//Rocket Jump : http://rocketjump.skr.jp/unity3d/109/
//https://twitter.com/ricopin416
//
//Revised by N.Kobayashi 2014/06/24
//           Y.Ebata
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityChan
{
	public class SpringManager : MonoBehaviour
//#if !SCENE_DEBUG
//        , Battle.IEvent
//#endif
    {
		//Kobayashi
		// DynamicRatio is paramater for activated level of dynamic animation 
		public float dynamicRatio = 1.0f;

		//Ebata
		public float			stiffnessForce;
		public AnimationCurve	stiffnessCurve;
		public float			dragForce;
		public AnimationCurve	dragCurve;
		public SpringBone[]     springBones;

        // 部件结点的骨骼点
        //int partStartPos = -1;

        //public void OnPartChange(System.Func<ModelPartSetting.Part, GameObject> getPart)
        //{
        //    if (getPart == null)
        //        return;

        //    List<SpringBone> TempSpringBones = new List<SpringBone>();
        //    if (partStartPos != -1)
        //    {
        //        //int max = Mathf.Min(partStartPos, springBones.Length);
        //        for (int i = 0; i < springBones.Length; ++i)
        //        {
        //            if (springBones[i] != null)
        //                TempSpringBones.Add(springBones[i]);
        //        }
        //    }
        //    else
        //    {
        //        TempSpringBones.AddRange(springBones);
        //    }
        //    partStartPos = TempSpringBones.Count;

        //    GameObject go = gameObject;
        //    List<SpringBone> parts = new List<SpringBone>();
        //    for (int i = 1; i < (int)ModelPartSetting.Part.Cnt; ++i)
        //    {
        //        // 部件结点
        //        GameObject part = getPart((ModelPartSetting.Part)i);
        //        if (part == null)
        //            continue;

        //        // 收集部位的SpringBone
        //        part.GetComponentsInChildren(parts);
        //        if (parts.Count==0)
        //        {
        //            // 新模型制作原因，特殊部位的骨骼会移到父节点(详细看ModelPartManage.UseOneClip_OnInitPart代码)
        //            // 如果部位没有找到SpringBone，就到父节点下找
        //            Transform boneTrans = null;
        //            if (i == (int)ModelPartSetting.Part.face)
        //            {
        //                boneTrans = part.transform.parent.FindChild("face");
        //            }
        //            else if (i == (int)ModelPartSetting.Part.Hair)
        //            {
        //                boneTrans = part.transform.parent.FindChild("hair_style_1");
        //            }

        //            if (boneTrans != null)
        //                boneTrans.GetComponentsInChildren(parts);
        //        }
        //        if (parts.Count != 0)
        //        {
        //            TempSpringBones.AddRange(parts);

        //            var colliders = part.GetComponentsInParent<SpringCollider>();
        //            if (colliders != null && colliders.Length != 0)
        //            {
        //                for (int m = 0; m < parts.Count; ++m)
        //                {
        //                    var sb = parts[m];
        //                    int l = sb.colliders == null ? 0 : sb.colliders.Length;
        //                    if (l == 0)
        //                    {
        //                        sb.colliders = colliders;
        //                    }
        //                    else
        //                    {
        //                        SpringCollider[] scs = new SpringCollider[l + colliders.Length];
        //                        System.Array.Copy(sb.colliders, scs, l);
        //                        System.Array.Copy(colliders, 0, scs, l, colliders.Length);

        //                        sb.colliders = scs;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    if (springBones.Length == TempSpringBones.Count)
        //    {
        //        for (int i = 0; i < TempSpringBones.Count; ++i)
        //            springBones[i] = TempSpringBones[i];
        //    }
        //    else
        //    {
        //        springBones = TempSpringBones.ToArray();
        //    }

        //    TempSpringBones.Clear();
        //}

        void Start ()
		{
            for (int i = 0; i < springBones.Length; i++)
            {
                if (springBones[i] == null)
                {
                    Debuger.LogError("SpringManager 有空骨骼 go=" + BoneManage.GetFullGoPath(this.gameObject.transform));
                }
            }

#if UNITY_EDITOR
            UpdateParameters();
#endif

#if !SCENE_DEBUG
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                AnimatorClipInfo[] infos = animator.GetCurrentAnimatorClipInfo(0);
                if (infos != null && infos.Length != 0 && infos[0].clip != null)
                    OnAniChange(infos[0].clip.name);
            }
#endif
        }

#if UNITY_EDITOR
        void Update ()
		{

		    //Kobayashi
		    if(dynamicRatio >= 1.0f)
			    dynamicRatio = 1.0f;
		    else if(dynamicRatio <= 0.0f)
			    dynamicRatio = 0.0f;
		    //Ebata
		    UpdateParameters();
		}
#endif

        private void LateUpdate()
        {
            if (!currentEnabled && disableCoroutine == null)
                return;

            //Kobayashi
            if (dynamicRatio != 0.0f)
            {
                for (int i = 0; i < springBones.Length; i++)
                {
                    if (springBones[i] == null)
                        continue;
                    if (dynamicRatio > springBones[i].threshold)
                    {
                        springBones[i].UpdateSpring();
                    }
                }
            }
        }

#if !SCENE_DEBUG
        [SerializeField]
        float disableSpeed = 0.5f;

        //Battle.BaseRole m_role;
//         public void AddEvent(Battle.BaseRole role)
//         {
//             m_role = role;
//             role.m_eventManage.AddEventListener<string>(Battle.RoleEventDefine.AnimationBegin, OnAniChange);
//             role.m_eventManage.AddEventListener<Battle.BaseRole>(Battle.RoleEventDefine.FinishLoadModel, OnPartChange);
//         }
// 
//         void OnDestroy()
//         {
//             if (m_role != null && m_role.isAlive)
//             {
//                 m_role.m_eventManage.RemoveEventListener<string>(Battle.RoleEventDefine.AnimationBegin, OnAniChange);
//                 m_role.m_eventManage.RemoveEventListener<Battle.BaseRole>(Battle.RoleEventDefine.FinishLoadModel, OnPartChange);
//             }
//             m_role = null;
//         }

        void OnAniChange(string playAni)
        {
            bool isEnable = UpdateCurrentEnable(playAni);
            if (currentEnabled != isEnable)
            {
                currentEnabled = isEnable;
                if (!currentEnabled)
                {
                    SetDisable(disableSpeed);
                }
                else
                {
                    CannelDisable();
                }
            }
        }

        // 切换部件
        //void OnPartChange(Battle.BaseObject role)
        //{
        //    if (role.m_partManage == null)
        //        return;

        //    OnPartChange(role.m_partManage.GetPart);
        //}
#endif

        [SerializeField]
        [HideInInspector]
        bool isType = true; // 过滤的类型true,在列表当中的不运算，false在列表当中的参与运算

        [SerializeField]
        string[] disableAnims;// 不播放动画列表

        [HideInInspector]
        public bool currentEnabled = true;

#if UNITY_EDITOR
        [HideInInspector]
        [System.NonSerialized]
        public string currentAnimName;
#endif

        Coroutine disableCoroutine;

        void CannelDisable()
        {
            if (disableCoroutine != null)
            {
                StopCoroutine(disableCoroutine);
                dynamicRatio = 1f;
                disableCoroutine = null;
            }
        }

        public void SetDisable(float speed = 0.5f)
        {
            CannelDisable();
            disableCoroutine = StartCoroutine(UpdateDisable(speed));
        }

        IEnumerator UpdateDisable(float speed)
        {
            speed = dynamicRatio / speed;
            while (true)
            {
                if (dynamicRatio <= 0)
                {
                    dynamicRatio = 1f;
                    disableCoroutine = null;
                    yield break;
                }

                float delay = Time.deltaTime;
                dynamicRatio -= delay * speed;
                yield return 0;
            }
        }

        bool UpdateCurrentEnable(string currentAnim)
        {
#if UNITY_EDITOR
            currentAnimName = currentAnim;
#endif
            // 过滤的类型true,在列表当中的不运算，false在列表当中的参与运算
            if (isType == true)
            {
                for (int i = 0; i < disableAnims.Length; ++i)
                {
                    if (currentAnim == disableAnims[i])
                        return false;
                }
                return true;
            }
            else
            {
                for (int i = 0; i < disableAnims.Length; ++i)
                {
                    if (currentAnim == disableAnims[i])
                        return true;
                }

                return false;
            }
        }

#if UNITY_EDITOR
        private void UpdateParameters ()
		{
			UpdateParameter ("stiffnessForce", stiffnessForce, stiffnessCurve);
			UpdateParameter ("dragForce", dragForce, dragCurve);
		}
	
		private void UpdateParameter (string fieldName, float baseValue, AnimationCurve curve)
		{

			var start = curve.keys [0].time;
			var end = curve.keys [curve.length - 1].time;
			//var step	= (end - start) / (springBones.Length - 1);
		
			var prop = springBones [0].GetType ().GetField (fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
		
			for (int i = 0; i < springBones.Length; i++) {
                if (springBones[i] == null)
                    continue;
				//Kobayashi
				if (!springBones [i].isUseEachBoneForceSettings) {
					var scale = curve.Evaluate (start + (end - start) * i / (springBones.Length - 1));
					prop.SetValue (springBones [i], baseValue * scale);
				}
			}

		}
#endif
    }
}