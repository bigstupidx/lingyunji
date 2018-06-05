using UnityEngine;
using UnityEngine.UI;

namespace xys.UI
{
    // UI物品格子类型
    public class ItemGrid : HotTComponent
    {
        //[SerializeField]
        //ItemBaseGrid ui;

        // 设置此格子的数据
        public void SetData(NetProto.ItemGrid data)
        {
            refType.InvokeMethod("SetData", data);
        }

        //// 设置开放格子状态
        //public void SetOpenGridState(int index)
        //{
        //    if (this.Index < index)
        //        ui.SetOpenLock(false);
        //    else
        //        ui.SetOpenLock(true);
        //}

        //// 设置遮罩cd
        //public void SetCdImage(int sonType)
        //{
        //    if (App.my.localPlayer.cdMgr.isEnd(CDType.Item, (short)sonType))
        //    {
        //        ui.SetCdImage(false);
        //        return;
        //    }

        //    CDEventData data = App.my.localPlayer.cdMgr.GetData(CDType.Item, (short)sonType);
        //    float cdTime = (data.total - data.elapse) / data.total;
        //    ui.SetCdImage(transform, cdTime);
        //}
    }
}
