#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using NetProto;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;

namespace xys.hot.UI
{
    [AutoILMono]
    class PetsCell
    {
        #region Field
        [SerializeField]
        private Image icon;                            // 宠物图标
        [SerializeField]
        private Text name;                             // 名称
        [SerializeField]
        private Text level;                            // 等级
        [SerializeField]
        private Text score;                            // 评分 TODO
        [SerializeField]
        private ButtonEx btnEx;                        // 按钮

        private int index;                             // 槽位
        private PetsAttribute petsData;                // 属性
        private bool isVariation = false;              // 是否变异

        #endregion

        #region Impl

        private void OnEnable()
        {
            btnEx.longPressInterval = 1f;
            btnEx.OnLongPress.AddListenerIfNoExist(OnBtnLongPress);
            btnEx.OnClickWithoutDrag.AddListenerIfNoExist(OnBtnClick);
        }

        private void OnDisable()
        {
            btnEx.OnLongPress.RemoveAllListeners();
            btnEx.OnClickWithoutDrag.RemoveAllListeners();
        }

        #endregion

        #region Event
        private void OnBtnClick()
        {
            ChatDefins.CellData data = new ChatDefins.CellData
            {
                index = index,
                petsData = petsData,
                isVariation = isVariation,
                type = ChatDefins.CellData.Type.Pet,
                pos = -1,
                count = -1,
                subType = -1,
                isEquiped = false,
                itemData = null,
            };
            hotApp.my.eventSet.FireEvent(EventID.ChatInput_OnReceivePetsData, data);
        }

        private void OnBtnLongPress()
        {
            ChatUtil.ShowPetsTips(petsData);
        }
        #endregion

        #region Interface

        public void OnCellAdding(int index)
        {
            this.index = index;
            petsData = hotApp.my.GetModule<HotPetsModule>().petsMgr.m_PetsTable.attribute[index];
            var config = PetAttribute.Get(petsData.id);

            isVariation = config.variation == 1;
            icon.SetSprite(config.icon);
            name.text = config.name;
            level.text = petsData.lv.ToString();
            score.text = "评分暂无";
        }
        #endregion
    }
} 
#endif
