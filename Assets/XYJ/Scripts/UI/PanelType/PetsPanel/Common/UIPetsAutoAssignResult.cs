#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [System.Serializable]
    class UIPetsAutoAssignResult
    {
        public Transform m_Transform;
        public Transform m_Grid;

        int[] m_TempAssignNum;
        public void Set(UIPetsAssginPanel panel,int[] tempAssignNum)
        {
            PetsMgr pm = panel.panel.petsMgr;

            if (panel.panel.selectedPetObj == null)
                return;

            m_TempAssignNum = tempAssignNum;
            //设置分配点信息
            int index = 0;
            SetRightProperty(m_Grid,Config.AttributeDefine.Get(Config.AttributeDefine.iStrength).name, m_TempAssignNum[0], ref index);
            SetRightProperty(m_Grid,Config.AttributeDefine.Get(Config.AttributeDefine.iIntelligence).name, m_TempAssignNum[1], ref index);
            SetRightProperty(m_Grid,Config.AttributeDefine.Get(Config.AttributeDefine.iBone).name, m_TempAssignNum[2], ref index);
            SetRightProperty(m_Grid,Config.AttributeDefine.Get(Config.AttributeDefine.iPhysique).name, m_TempAssignNum[3], ref index);
            SetRightProperty(m_Grid,Config.AttributeDefine.Get(Config.AttributeDefine.iAgility).name, m_TempAssignNum[4], ref index);
            SetRightProperty(m_Grid,Config.AttributeDefine.Get(Config.AttributeDefine.iBodyway).name, m_TempAssignNum[5], ref index);
            panel.parent.StartCoroutine(OutScreen());
        }
        void SetRightProperty(Transform nRoot, string attr, int value, ref int index)
        {
            nRoot.GetChild(index).Find("Name").GetComponent<Text>().text = attr;
            nRoot.GetChild(index).Find("num").GetComponent<Text>().text = value.ToString();
            if (m_TempAssignNum[index] > 0)
                nRoot.GetChild(index).Find("num").GetComponent<Text>().color = XTools.Utility.ParseColor("61e171",0);
            else
                nRoot.GetChild(index).Find("num").GetComponent<Text>().color = Color.white;

            index++;
        }
        IEnumerator OutScreen()
        {
            CanvasGroup cg = this.m_Transform.GetComponent<CanvasGroup>();
            cg.alpha = 1.0f;
            yield return new WaitForSeconds(2.0f);
            while (cg.alpha > 0.0f)
            {
                cg.alpha -= Time.deltaTime;
                yield return 0;
            }
        }
    }
}
#endif