#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace xys.UI
{
    public class UIAvatarPrefabPage : MonoBehaviour
    {
        public UIGroup m_subParts;
        public int m_curTab;

        int subTabsLen = 0;

        public static string[] s_tabs = new string[] { "默认", "换脸", "皮肤", "发型" };
        // tab里显示的类型
        public static int[][] s_tabKeys = new int[][] {
        new int[] { 5 },
        new int[] { 4 },
        new int[] { 3 },
        new int[] { 1, 2 },
    };

        public void Set(int idx, RoleDisguiseHandle handle)
        {
            RoleDisguiseCareer config = handle.GetOverallConfig();
            m_curTab = idx;

            int[] tabsKeys = s_tabKeys[idx];
            subTabsLen = tabsKeys.Length;
            m_subParts.SetCount(subTabsLen);

            for (int i=0; i<subTabsLen; ++i)
            {
                int itemIdx = i;
                var type = tabsKeys[i];
                var item = m_subParts.Get<UIAvatarSkinMergeItem>(i);
                RoleDisguiseType partData = config.GetType(type);
                if (partData == null || partData.items == null || partData.items.Count == 0)
                    continue;


                //标题，选中则展开内容
                if (subTabsLen == 1)
                    item.btnTitle.gameObject.SetActive(false);
                else
                {
                    item.btnTitle.gameObject.SetActive(true);
                    item.btnTitle.onClick.RemoveAllListeners();
                    item.btnTitle.onClick.AddListener(() => OnSel(itemIdx));
                    item.title.text = partData.name;
                }

                //内容，选中才展开
                item.content.SetActive(i == 0);

                //图标，多个才支持可以选
                if (type == 1 || type == 4 || type == 5)
                {
                    item.styles.gameObject.SetActive(false);
                    item.icons.gameObject.SetActive(true);
                    
                    item.icons.SetCount(partData.items.Count);
                    for (int j = 0; j < partData.items.Count; ++j)
                    {
                        var iconIdx = j;
                        var tex = partData.items[j];
                        // 设置图片
                        var btn = item.icons.Get<Button>(j);
                        var icon = btn.transform.Find("icon").GetComponent<Text>();
                        icon.text = tex.iconName;
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() =>
                        {
                            handle.SetStyle(type, iconIdx);
                        });
                    }
                }
                else
                {
                    // 颜色样式
                    item.styles.gameObject.SetActive(true);
                    item.icons.gameObject.SetActive(false);

                    item.styles.SetCount(partData.items.Count);
                    for (int j = 0; j < partData.items.Count; ++j)
                    {
                        var iconIdx = j;
                        var style = partData.items[j];
                        var btn = item.styles.Get<Button>(j);
                        var icon = btn.GetComponent<Image>();
                        icon.color = style.color.RBGFloatColor;
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() =>
                        {
                            handle.SetStyle(type, iconIdx);
                            OnSelectColorType(handle, itemIdx, type);
                        });
                    }

                    //色相、饱和度、明度
                    var slider = item.hh;
                    slider.gameObject.SetActive(false);

                    slider = item.ss;
                    slider.gameObject.SetActive(false);

                    slider = item.vv;
                    slider.gameObject.SetActive(false);

                    
                }

            }
        }

        void OnSelectColorType(RoleDisguiseHandle handle, int idx, int type)
        {
            RoleDisguiseItem itemCfg = handle.GetSelectedItemConfig(type);
            var item = m_subParts.Get<UIAvatarSkinMergeItem>(idx);
            //色相、饱和度、明度
            var slider = item.hh;
            slider.gameObject.SetActive(true);
            slider.slider.onValueChanged.RemoveAllListeners();
            slider.slider.value = itemCfg.color.h / 3f;
            slider.slider.onValueChanged.AddListener((h) =>
            {
                itemCfg.color.h = (int)(h * 360f);

                handle.SetSelectedItem(type);
            });

            slider = item.ss;
            slider.gameObject.SetActive(true);
            slider.slider.onValueChanged.RemoveAllListeners();
            slider.slider.value = itemCfg.color.s / 3f;
            slider.slider.onValueChanged.AddListener((s) =>
            {
                itemCfg.color.s = s * 3f;

                handle.SetSelectedItem(type);
            });

            slider = item.vv;
            slider.gameObject.SetActive(true);
            slider.slider.onValueChanged.RemoveAllListeners();
            slider.slider.value = itemCfg.color.v / 3f;
            slider.slider.onValueChanged.AddListener((v) =>
            {
                itemCfg.color.v = v * 3f;

                handle.SetSelectedItem(type);
            });
        }

        void OnSel(int idx)
        {

            for (int i = 0; i < subTabsLen; ++i)
            {
                var item = m_subParts.Get<UIAvatarSkinMergeItem>(i);
                item.content.SetActive(i == idx);
            }

        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
#endif