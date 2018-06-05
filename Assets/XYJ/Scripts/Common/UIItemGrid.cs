using UnityEngine;

namespace xys.UI
{
    public class UIItemGrid : MonoBehaviour
    {
        [SerializeField]
        public ItemBaseGrid ui; // 需要的UI数据

        public void SetData(int id, int num)
        {
            NetProto.ItemGrid data = new NetProto.ItemGrid();
            data.data.id = id;
            data.count = num;

            ui.SetData(data);
        }
    }
}
