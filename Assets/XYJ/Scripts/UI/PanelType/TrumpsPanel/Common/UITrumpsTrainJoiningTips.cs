#if !USE_HOT
namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using xys.UI;
    using NetProto;
    using Config;
    using battle;
    using xys.UI.State;
    using System;

    [System.Serializable]
    class UITrumpsTrainJoiningTips
    {
        [SerializeField]
        Transform m_Transform;
        [SerializeField]
        UIGroup m_JoiningGroup;

        [SerializeField]
        Button m_CloseBtn;

//         [SerializeField]
//         ILMonoBehaviour m_ILTrumpTips;
//         UITrumpTips m_TrumpTips;

        [SerializeField]
        float m_TipsX = 0.0f;
        [SerializeField]
        float m_TipsY = 260.0f;

        public void OnInit()
        {
//             if (m_ILTrumpTips != null)
//                 m_TrumpTips = m_ILTrumpTips.GetObject() as UITrumpTips;
            m_CloseBtn.onClick.AddListener(() => { this.m_Transform.gameObject.SetActive(false); });
        }

        public void Set(int trumpId) 
        {
            TrumpsMgr mgr = hotApp.my.GetModule<HotTrumpsModule>().trumpMgr;
            if (mgr == null || !TrumpProperty.GetAll().ContainsKey(trumpId))
            {
                this.m_Transform.gameObject.SetActive(false);
                return;
            }
            this.m_Transform.gameObject.SetActive(true);
            //
            TrumpProperty property = TrumpProperty.Get(trumpId);
            this.m_JoiningGroup.SetCount(property.joinings.Length);
            List<TrumpJoining> activeJoining = mgr.GetActiveJoining();
            for(int i = 0; i < property.joinings.Length;i++)
            {
                Transform root = m_JoiningGroup.transform.GetChild(i);
                this.SetJoining(root, mgr, TrumpJoining.Get(property.joinings[i]), trumpId);
                //是否激活
                TrumpJoining joiningData = activeJoining.Find((data) => { return data.id == property.joinings[i]; });
                root.Find("ActiveIcon").gameObject.SetActive(joiningData != null);
            }
        }

        void SetJoining(Transform root, TrumpsMgr mgr,TrumpJoining data,int trumpid)
        {
            List<int> joiningList = new List<int>();
            for(int i = 0; i < data.trump.Length;i++)
            {
                if (data.trump[i] == trumpid)
                    joiningList.Insert(0, data.trump[i]);
                else
                    joiningList.Add(data.trump[i]);
            }
            //
            int pos = data.joiningdes.LastIndexOf("：");
            string res = data.joiningdes;
            if (pos != -1) res = res.Substring(pos);

            root.Find("Des").GetComponent<Text>().text = res;
            root.Find("Name").GetComponent<Text>().text = data.name;
            //
            Transform grid = root.Find("Grid");
            for (int i = 0;i < joiningList.Count; i++)
            {
                Transform child = grid.GetChild(i);
                child.gameObject.SetActive(true);

                int index = i;
                child.GetComponent<Button>().onClick.RemoveAllListeners();
                child.GetComponent<Button>().onClick.AddListener(()=> { this.ShowTrumpTips(joiningList[index]); });

                Helper.SetSprite(child.Find("Icon").GetComponent<Image>(), TrumpProperty.Get(joiningList[i]).icon);
                Helper.SetSprite(child.Find("Quality").GetComponent<Image>(), QualitySourceConfig.Get(TrumpProperty.Get(joiningList[i]).quality).icon);

                child.Find("Mask").gameObject.SetActive(!mgr.CheckTrumps(joiningList[i]));
                child.Find("Tag").gameObject.SetActive(mgr.GetEquipPos(joiningList[i]) != -1);
            }

            for (int i = joiningList.Count; i < grid.childCount; i++)
                grid.GetChild(i).gameObject.SetActive(false);
        }

        void ShowTrumpTips(int trumpId)
        {
            // m_TrumpTips.Set(trumpId, new Vector3(m_TipsX, m_TipsY, 0.0f));
            UICommon.ShowTrumpTips(trumpId, new Vector2(m_TipsX, m_TipsY));
        }
    }
}
#endif