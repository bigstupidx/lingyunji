using UnityEngine;
using UnityEngine.UI;

namespace xys.UI
{
    public class EquipGrid : MonoBehaviour
    {
        // 装备部件信息
        public Config.EquipPartsType type { get; private set; }

        [SerializeField]
        public ItemBaseGrid ui; // 需要的UI数据

        public void SetData(NetProto.ItemGrid grid)
        {
            ui.SetData(grid);
        }

        // 设置默认装备图标
        public void SetOpenGridState(NetProto.ItemData grid)
        {
            if (grid == null)
                ui.SetOpenLock(true);
            else
                ui.SetOpenLock(false);
        }
    }
}