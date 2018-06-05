using Config;
using UnityEngine;
using UnityEngine.UI;

namespace xys.UI
{
    namespace ItemObtainShow
    {
        public class UIItemUseTip : MonoBehaviour
        {
            [SerializeField]
            Button closeBtn;
            [SerializeField]
            Button useBtn;

            [SerializeField]
            Text itemName;
            [SerializeField]
            Transform bagItem;

            int itemId = 0;     //物品ID
            int itemCount = 0;  //物品数量

            public void OnShow(int id, int count)
            {
                closeBtn.onClick.AddListener(() => { CloseUseTips(); });
                useBtn.onClick.AddListener(() => { UseItem(); });

                ItemBase item = ItemBaseAll.Get(id);
                itemName.text = string.Format("#c{1}{0}#n", item.name, QualitySourceConfig.Get(item.quality).color + "FF");
                SetData();

                itemId = id;
                itemCount = count;
            }

            // 设置此物品格子的数据
            public void SetData()
            {
                RALoad.LoadPrefab("UIItemGrid",
                   (GameObject go, object p) =>
                   {
                       GameObject itemGo = UnityEngine.Object.Instantiate(go);
                       itemGo.transform.SetParent(bagItem);
                       itemGo.transform.localPosition = Vector3.zero;
                       itemGo.transform.localScale = Vector3.one;
                       UIItemGrid itemGrid = itemGo.GetComponent<UIItemGrid>();
                       itemGrid.SetData(itemId, itemCount);
                       itemGo.SetActive(true);
                   }, null, false, true);
            }

            void CloseUseTips()
            {
                Animator ani = gameObject.transform.Find("Panel").GetComponent<Animator>();
                ani.Play("close");
                Destroy(gameObject, 0.3f);
            }

            void UseItem()
            {
                PackageModule pm = App.my.localPlayer.GetModule<PackageModule>();
                if (pm != null) pm.UseItemById(itemId, itemCount);

                CloseUseTips();
            }
        }
    }
}