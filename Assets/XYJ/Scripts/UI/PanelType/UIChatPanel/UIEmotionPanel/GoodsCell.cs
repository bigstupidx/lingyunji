#if !USE_HOT
using System;
using Config;
using NetProto;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using xys.UI;

namespace xys.hot.UI
{
    [AutoILMono]
    class GoodsCell
    {
        #region Field
        [SerializeField]
        private ButtonEx btnEx;                                                 // 道具按钮
        [SerializeField]
        private Image icon;                                                     // 道具图标
        [SerializeField]
        private GameObject equipFlag;                                           // 装备标识
        [SerializeField]
        private GameObject lockFlag;                                            // 绑定标识
        [SerializeField]
        private Text count;                                                     // 道具数量

        private ItemData itemData;

        private bool isEquiped = false;                                         // 是否已装备
        private bool isEquipType = false;                                       // 是否是装备
        private int gridPos = -1;                                               // 索引位置
        private int number = 1;
        private int subType = -1;
        #endregion

        #region Impl
        private void OnEnable()
        {
            btnEx.longPressInterval = 1f;
            btnEx.OnLongPress.AddListenerIfNoExist(OnBtnLongPress);
            btnEx.OnClickWithoutDrag.AddListenerIfNoExist(OnBtnClickWitouDrag);
        }

        private void OnDisable()
        {
            btnEx.OnLongPress.RemoveAllListeners();
            btnEx.OnClickWithoutDrag.RemoveAllListeners();
        }

        #endregion

        #region Interface

        public void OnCellAdding(bool equiped, ItemData data, int pos)
        {
            isEquiped = equiped;
            itemData = data;
            gridPos = pos;

            var baseConfig = ItemBaseAll.Get(data.id);
            isEquipType = baseConfig.type == ItemType.equip;

            // 已装备的道具
            if(isEquiped)
            {
                var equipConfig = EquipPrototype.Get(data.id);

                icon.SetSprite(equipConfig.icon);
                equipFlag.SetActive(true);
                lockFlag.SetActive(equipConfig.isBind);
                count.gameObject.SetActive(false);
                subType = equipConfig.sonType;
            }
            // 身上的道具
            else
            {
                if(isEquipType)
                {
                    var equipConfig = EquipPrototype.Get(data.id);

                    icon.SetSprite(equipConfig.icon);
                    // 背包中的装备不显示装备标识
                    equipFlag.SetActive(false);
                    lockFlag.SetActive(equipConfig.isBind);
                    // 背包中的装备不会堆叠
                    count.gameObject.SetActive(false);
                    //// 数据的索引列表
                    //var posList = package.GetItemPosition(id);
                    //int itemCountIndex = -1;
                    //for(int i = 0 ; i < posList.Count ; i++)
                    //{
                    //    if(posList[i] == pos)
                    //    {
                    //        itemCountIndex = i;
                    //    }
                    //}
                    //Assert.IsFalse(itemCountIndex == -1, "该装备传入的索引在背包中找不到无对应索引数据！");
                    //// 根据是否有已装备的数据和在背包中的索引位来调整fullName
                    //var equipedData = equipMgr.GetEquipData(equipConfig.sonType);
                    //// 该部件装备是否被更换
                    //if (equipedData.id != itemData.id)
                    //{
                    //    equipedData = null;
                    //}
                    //var name = equipConfig.name;
                    //// 如果没有已装备数据就忽视索引调整
                    //itemCountIndex += null != equipedData ? 1 : 0;
                    //fullName = name + (itemCountIndex > 0 ? itemCountIndex.ToString() : "");
                }
                else
                {
                    var itemConfig = Item.Get(data.id);
                    icon.SetSprite(itemConfig.icon);
                    equipFlag.SetActive(false);
                    lockFlag.SetActive(itemConfig.isBind);

                    count.gameObject.SetActive(true);
                    var grid = hotApp.my.GetModule<HotPackageModule>().packageMgr.package.GetItemInfo(pos);
                    count.text = grid.count.ToString();
                    number = grid.count;
                }
            }
        }

        #endregion

        #region Event
        private void OnBtnClickWitouDrag()
        {
            ChatDefins.CellData data = new ChatDefins.CellData
            {
                isEquiped = isEquiped,
                type = isEquipType ? ChatDefins.CellData.Type.Equip : ChatDefins.CellData.Type.Item,
                itemData = itemData,
                pos = gridPos,
                count = number,
                subType = subType,
                index =  -1,
                isVariation = false,
                petsData = null,
            };
            hotApp.my.eventSet.FireEvent(EventID.ChatInput_OnReceiveItemData, data);
        }

        private void OnBtnLongPress()
        {
            ChatUtil.ShowItemTips(itemData);
        } 
        #endregion
    }
}

#endif