#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;

namespace xys.hot.UI
{
    // UI物品格子类型
    class HotItemGrid : HotTComponentBase
    {
        public HotItemGrid() : base(null) { }
        public HotItemGrid(xys.UI.ItemGrid parent) : base(parent)
        {
            disableCoroutine = null;
        }

        [SerializeField]
        xys.UI.ItemBaseGrid ui;
        Coroutine disableCoroutine;

        // 设置此格子的数据
        public void SetData(NetProto.ItemGrid data)
        {
            ui.SetData(data);

            if (disableCoroutine != null)
            {
                parent.StopCoroutine(disableCoroutine);
                disableCoroutine = null;
            }

            disableCoroutine = parent.StartCoroutine(TimeLimitItem(data));
        }

        // 设置开放格子状态
        public void SetOpenGridState(int index)
        {
            if (this.Index < index)
                ui.SetOpenLock(false);
            else
                ui.SetOpenLock(true);
        }

        System.Collections.IEnumerator TimeLimitItem(NetProto.ItemGrid data)
        {
            while (true)
            {
                yield return 10;
                SetCdImage(data);
            }
        }

        // 设置遮罩cd
        public void SetCdImage(NetProto.ItemGrid itemData)
        {
            if (itemData == null)
            {
                ui.SetCdImage(false);
                return;
            }

            Config.ItemBase config = Config.ItemBaseAll.Get(itemData.data.id);
            if (config == null || config.type == Config.ItemType.equip)
            {
                ui.SetCdImage(false);
                return;
            }

            Config.Item itemConfig = Config.Item.Get(itemData.data.id);
            if (itemConfig == null || itemConfig.cooling == 0)
            {
                ui.SetCdImage(false);
                return;
            }

            if (App.my.localPlayer.cdMgr.isEnd(CDType.Item, (short)config.sonType))
            {
                ui.SetCdImage(false);
                return;
            }

            CDEventData data = App.my.localPlayer.cdMgr.GetData(CDType.Item, (short)config.sonType);
            float cdTime = (data.total - data.elapse) / data.total;
            ui.SetCdImage(transform, cdTime);
        }
    }
}
#endif