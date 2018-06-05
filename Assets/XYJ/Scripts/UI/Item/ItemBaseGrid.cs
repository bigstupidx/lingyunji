using UnityEngine;
using UnityEngine.UI;
using Config;

namespace xys.UI
{
    [System.Serializable]
    public class ItemBaseGrid
    {
        [SerializeField]
        Image icon; // 图标

        [SerializeField]
        Image isOpenLocak; // 格子开启锁

        [SerializeField]
        Image isBind; // 是否绑定

        [SerializeField]
        Image quality; // 品质

        [SerializeField]
        Image isItemLocak; // 道具锁

        [SerializeField]
        Text itemCount; // 道具数量

        [SerializeField]
        Text itemLevel; // 道具等级

        [SerializeField]
        Image isRare; // 是否是珍品

        [SerializeField]
        Image cdImage; // 冷却cd遮罩

        NetProto.ItemGrid m_ItemGrid;

        // 设置此格子的数据
        public void SetData(NetProto.ItemGrid data)
        {
            m_ItemGrid = data;
            if (data == null || data.data == null || (ItemBaseAll.Get(data.data.id) == null))
            {
                // 空的物品
                if (icon != null)
                    icon.gameObject.SetActive(false);
                if (isBind != null)
                    isBind.gameObject.SetActive(false);
                if (isOpenLocak != null)
                    isOpenLocak.gameObject.SetActive(false);
                if (isItemLocak != null)
                    isItemLocak.gameObject.SetActive(false);
                if (quality != null)
                    quality.gameObject.SetActive(false);
                if (itemCount != null)
                    itemCount.gameObject.SetActive(false);
                if (cdImage != null)
                    cdImage.gameObject.SetActive(false);
            }
            else
            {
                SetIcon(data.data.id);
                SetBind(data.data.id);
                SetItemLock(data.data.id);
                SetItemCount(data.count);
                SetQuality(data.data.id);
                SetCdImage(false, 0);
            }
        }

        void SetIcon(int id)
        {
            if (icon == null)
                return;
            var itemConfig = ItemBaseAll.Get(id);
            Helper.SetSprite(icon, itemConfig.icon);
            icon.gameObject.SetActive(true);
        }

        void SetBind(int id)
        {
            if (isBind == null)
                return;
            ItemBase itemConfig = ItemBaseAll.Get(id);
            if (itemConfig == null)
                return;
            isBind.gameObject.SetActive(itemConfig.isBind);
        }

        public void SetOpenLock(bool bFlag)
        {
            if (isOpenLocak == null)
                return;
            isOpenLocak.gameObject.SetActive(bFlag);
        }

        public void SetCdImage(bool bFlag, float cdTime = 0)
        {
            if (null == cdImage)
                return;

            if (m_ItemGrid == null)
            {
                cdImage.gameObject.SetActive(false);
                return;
            }

            cdImage.gameObject.SetActive(bFlag);

            if (bFlag)
                cdImage.fillAmount = cdTime;
        }

        void SetItemLock(int id)
        {
            // todo:
            if (isItemLocak == null)
                return;
            ItemBase config = ItemBaseAll.Get(id);
            if (config == null)
                return;
            if (config.job.Has((RoleJob.Job)App.my.localPlayer.carrerValue))
                isItemLocak.gameObject.SetActive(false);
            else
                isItemLocak.gameObject.SetActive(true);
        }

        void SetItemCount(int count)
        {
            if (itemCount == null)
                return;

            if (count <= 1)
                itemCount.gameObject.SetActive(false);
            else
            {
                itemCount.gameObject.SetActive(true);
                itemCount.text = count.ToString();
            }
        }

        void SetQuality(int id)
        {
            if (quality == null)
                return;
            ItemBase itemConfig = ItemBaseAll.Get(id);
            if (itemConfig == null)
                return;
            QualitySourceConfig qualitConfig = QualitySourceConfig.Get(itemConfig.quality);
            Helper.SetSprite(quality, qualitConfig.icon);
            quality.gameObject.SetActive(true);
        }
    }
}
