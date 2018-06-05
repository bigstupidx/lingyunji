#if !USE_HOT
using NetProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using xys.hot.Team;
using xys.UI;

namespace xys.hot.UI
{
    [AutoILMono]
    class TeamOrganizeApplyJoinDialog
    {
        [SerializeField]
        GameObject m_prefab;
        [SerializeField]
        GameObject m_self;

        System.Action<TeamJoinReqInfo> m_allowBtnCb;
        public void Init(System.Action<TeamJoinReqInfo> allowBtnCb)
        {
            m_allowBtnCb = allowBtnCb;
        }

        public void OnDataChange(List<TeamJoinReqInfo> infos)
        {
            this.Show(infos);
        }

        public void Show(List<TeamJoinReqInfo> infos)
        {
            for (int i = 0; i < m_self.transform.childCount; ++i)
            {
                GameObject.Destroy(m_self.transform.GetChild(i).gameObject);
            }

            foreach (TeamJoinReqInfo info in infos)
            {
                GameObject item = GameObject.Instantiate(m_prefab);
                item.transform.SetParent(m_self.transform);
                item.transform.localScale = Vector3.one;
                item.SetActive(true);
                item.transform.Find("Name").GetComponent<Text>().text = info.name;
                item.transform.Find("Level").GetComponent<Text>().text = string.Format("{0}级", info.level);
                item.transform.Find("YaoQingBtn").GetComponent<Button>().onClick.AddListenerIfNoExist(() => { OnClickAllow(info); });
                TeamProfSexResConfig resCfg = TeamUtil.GetProfSexResCfg(info.prof, info.sex);
                if (null != resCfg)
                {
                    Helper.SetSprite(item.transform.Find("Head/Icon").GetComponent<Image>(), resCfg.headIcon);
                    Helper.SetSprite(item.transform.Find("Job").GetComponent<Image>(), resCfg.profIcon);
                }
            }
        }

        public void OnClickAllow(TeamJoinReqInfo info)
        {
            if (null != m_allowBtnCb)
            {
                m_allowBtnCb(info);
            }
        }
    }
}

#endif