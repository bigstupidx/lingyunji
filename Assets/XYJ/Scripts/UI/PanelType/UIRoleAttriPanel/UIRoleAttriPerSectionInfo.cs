#if !USE_HOT
using Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using xys.UI;

namespace xys.hot.UI
{
    [System.Serializable]
    class UIRoleAttriPerSectionInfo
    {
        [SerializeField]
        Transform m_Transform;
        [SerializeField]
        Text m_Tips;
        [SerializeField]
        Transform m_DecsL;
        [SerializeField]
        Transform m_DecsR;

        public void OnInit(int leftId)
        {
            m_Transform.GetComponent<Button>().onClick.AddListener(() => { m_Transform.gameObject.SetActive(false); });

            ShowSection(leftId, m_DecsL);
            ShowSection(leftId + 1, m_DecsR);
            m_Transform.gameObject.SetActive(true);
        }

        void ShowSection(int id, Transform tf)
        {
            PersonalityDefine data = PersonalityDefine.Get(id);
            if (data == null) return;

            tf.Find("Title").GetComponent<Text>().text = data.name;
            for (int i = 0; i < tf.Find("Grid").childCount; i++)
            {
                GameObject textGo = tf.Find("Grid/" + (i + 1) + "/Text").gameObject;
                PersonalityDefine.PersonalitySection section = data.sectionGroup[i];
                textGo.GetComponent<Text>().text = string.Format("【第{0}阶】 {1} {2}~{3}", section.rank, section.desc, section.lowerLimit, section.upperLimit);
                textGo.SetActive(true);
            }
        }

    }
}
#endif