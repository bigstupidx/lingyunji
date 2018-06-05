#if !USE_HOT

namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using Config;
    using NetProto;
    using xys.UI;

    [AutoILMono]
    class UITaskRewardInfo
    {
        // 金币经验等
        [SerializeField]
        UIGroup m_currencyGroup;

        // 道具信息
        [SerializeField]
        UIGroup m_itemGroup;

        void SetCurrency(int index, ItemIDType type, int count)
        {
            GameObject go = m_currencyGroup.Get(index);
            Text text = go.GetComponentInChildren<Text>();
            if (text != null)
                text.text = count.ToString();

            Image image = go.GetComponentInChildren<Image>();
            if (image!=null)
            {
                Helper.SetSprite(image, ItemBaseAll.GetIcon(type));
            }
        }

        /// <summary>
        /// 设置奖励信息
        /// </summary>
        /// <param name="define"></param>
        public void SetInfo(TaskDefine define)
        {
            // 金币经验等信息
            int count = 0;
            if (define.rewardSilvers > 0)
                ++count;
            if (define.rewardGolds > 0)
                ++count;
            if (define.rewardJasper > 0)
                ++count;
            if (define.rewardExp > 0)
                ++count;
            if (count>0)
            {
                int index =0;
                if (define.rewardSilvers > 0)
                    SetCurrency(index++, ItemIDType.SilverShell, define.rewardSilvers);
                if (define.rewardGolds > 0)
                    SetCurrency(index++, ItemIDType.GoldShell, define.rewardGolds);
                if (define.rewardJasper > 0)
                    SetCurrency(index++, ItemIDType.JasperJade, define.rewardJasper);
                if (define.rewardExp > 0)
                    SetCurrency(index++, ItemIDType.Exp, define.rewardExp);
            }
            else
            {
                m_currencyGroup.SetCount(0);
            }

            // 道具设置
            ItemCount[] items = define.GetRewardItemList();
            if (items != null && items.Length > 0)
            {
                m_itemGroup.SetCount(items.Length);
                for (int i = 0; i < items.Length; ++i)
                {
                    GameObject obj = m_itemGroup.Get(i);
                    UIItemGrid itemUI = obj.GetComponent<UIItemGrid>();
                    if (itemUI != null)
                        itemUI.SetData(items[i].id, items[i].count);
                }
            }
            else
            {
                m_itemGroup.SetCount(0);
            }
        }

    }
}

#endif