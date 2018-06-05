#if !USE_HOT
namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using xys.UI;
    using xys.UI.State;

    [System.Serializable]
    class UITrumpsInfo
    {
        [SerializeField]
        Transform m_Transform;

        [SerializeField]
        RawImage m_TrumpRawIcon;
        [SerializeField]
        Text m_TrumpName;
        [SerializeField]
        Transform m_ActiveSkillRoot;
        [SerializeField]
        Transform m_PassiveSkillRoot;

        RTTModelPartHandler m_Rtt;
        int m_AttributeIndex = -1;

        public void OnInit()
        {
            this.OnCreateModel();
        }

        public void OnDestory()
        {

        }

        public void Set(int trumpid)
        {

        }

        //////
        void OnCreateModel()
        {
            m_Rtt = new RTTModelPartHandler("RTTModelPart", m_TrumpRawIcon.GetComponent<RectTransform>(), "", true, new Vector3(1000, 1000, 0),
               () =>
               {
                   m_Rtt.SetModelRotate(new Vector3(0.0f, 150.0f, 0.0f));
                   m_TrumpRawIcon.gameObject.SetActive(true);
               });
        }
        void OnDelectModel()
        {
            if (m_Rtt != null) m_Rtt.Destroy();
            m_Rtt = null;
            m_AttributeIndex = -1;
        }
    }
}
#endif