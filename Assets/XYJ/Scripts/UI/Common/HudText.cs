using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using EditorExtensions;

namespace Battle
{
    public class HudText : MonoBehaviour 
    {
        public enum enType
        {
            heroBaoji,//主角暴击伤害飘字
            heroDamage,//主角伤害飘字
            otherBaoji,//其他人的暴击伤害飘字
            otherDamage,//其他人伤害飘字
            huixue,//回血飘字
            huixueBaoji,//暴击回血飘字
            zengyi,//增益状态
            jianyi,//减益状态
            heroZengyi,//主角增益状态
            heroJianyi,//主角减益状态
        }
       
        [System.Serializable]
        public class HudType
        {
#if UNITY_EDITOR
            [EditorPropertyName("类型名")]
#endif
            public string desc = "";
#if UNITY_EDITOR
            [EditorPropertyName("动作")]
#endif
            public RuntimeAnimatorController m_controller;
#if UNITY_EDITOR
            [EditorPropertyName("文字色(顶部)")]
#endif
            public Color32 gradientTop = Color.white;
#if UNITY_EDITOR
            [EditorPropertyName("文字色(底部)")]
#endif
            public Color32 gradientBottom= Color.white;
#if UNITY_EDITOR
            [EditorPropertyName("文字缩放")]
#endif
            public float size = 1;
        }

        
        public HudType[] m_types ;
        public UnityEngine.UI.Gradient m_gradient;
        public Text m_contentTxt;//飘字内容
        public Animator m_animator;//动画
        public RectTransform m_sizeScale;

        [Header("测试相关")]
#if UNITY_EDITOR
        [EditorPropertyName("测试类型名(或索引)")]
#endif
        public string m_testType = "0";
#if UNITY_EDITOR
        [EditorPropertyName("测试文字")]
#endif
        public string m_test = "10086";
        //[InspectorButton("测试", "Test",true)]
        public bool m_btnTest;
        //public RectTransform m_follow;
        //public Camera m_camera;
        

        //void LateUpdate()
        //{
        //    RectTransform thisTran = this.GetComponent<RectTransform>();
        //    Vector2 myposition;
        //    var m_trans = thisTran.parent.GetComponent<RectTransform>();
        //    Vector3 screenPoint = m_camera.WorldToScreenPoint(thisTran.position);
        //    if (screenPoint.z < 0&&!m_camera.orthographic)//如果不是平行相机，那么屏幕后方要取反
        //    {
        //        screenPoint.x = Screen.width - screenPoint.x;
        //        screenPoint.y = Screen.height - screenPoint.y;
        //    }
        //    RectTransformUtility.ScreenPointToLocalPointInRectangle(m_trans, screenPoint, m_camera, out myposition);
        //    Rect bound = XTools.MathUtil.GetRectByCenter(myposition.x, myposition.y, thisTran.sizeDelta.x, thisTran.sizeDelta.y);
        //    m_follow.anchoredPosition = XTools.MathUtil.GetPosOfBoxEdge(m_trans.rect, bound);
            
        //}

        //播放飘字
        public void Play(enType type, string text)
        {
            Play((int)type, text);
        }
        
        void Play(int type,string text)
        {
            if (!this.gameObject.activeSelf)
                this.gameObject.SetActive(true);
            var hudType = m_types[type];
            m_contentTxt.text = text;
            m_contentTxt.GetComponent<CanvasGroup>().alpha = 1;
            m_sizeScale.localScale = Vector3.one * hudType.size;
            
            m_gradient.enabled = false;
            m_gradient.gradientTop = hudType.gradientTop;
            m_gradient.gradientBottom = hudType.gradientBottom;
            m_gradient.enabled = true;

            transform.Find("size/offset").GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

            m_animator.enabled = false;
            m_animator.runtimeAnimatorController = hudType.m_controller;
            AnimationClip[] hudClips = m_animator.runtimeAnimatorController.animationClips;
            AnimationClip clip = hudClips[UnityEngine.Random.Range(0, hudClips.Length)];
            m_animator.enabled = true;
            m_animator.Play(clip.name, 0, 0);

            //脚本销毁
            if (xys.App.my.uiSystem != null)
                this.gameObject.AddMissingComponent<ObjectPoolDestroy>().m_destroyTime = clip.length;
        }


        bool TryParse(string s,out int type)
        {
            type = 0;

            int idx;
            if (int.TryParse(s, out idx))
            {
                type = idx;
                return true;
            }
                

            for (int i = 0; i < m_types.Length; ++i)
            {
                if (m_types[i].desc == m_testType)
                {
                    type = i;
                    return true;
                }
            }
            return false;
        }
        
        [ContextMenu("测试")]
        void Test()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("运行时才可以测试飘字");
                return;
            }

           
            int type;
            if(!TryParse(m_testType,out type)|| type<0 || type>=m_types.Length)
            {
                Debug.LogError("找不到要测试的类型");
                return;
            }
                

            Play(type, m_test);
        }
    }
}
