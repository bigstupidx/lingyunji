#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;

namespace xys.UI
{
    public class UIAvatarSkinMergePage : MonoBehaviour
    {
        public UIGroup m_subParts;
        public int m_curTab;

        int subTabsLen = 0;
        int curSelectStyle = 0;

        void Awake()
        {

        }

        public void Set(int idx, RoleSkinHandle merge)
        {

            RoleSkinPart config = merge.GetConfig();
            RoleSkinPartData data = merge.GetData();

            m_curTab = idx;
            string[] keys = config.GetTabsKeys(idx);
            subTabsLen = keys.Length;
            m_subParts.SetCount(keys.Length);

            for (int i = 0; i < keys.Length; ++i)
            {
                int itemIdx = i;
                var key = keys[i];
                var item = m_subParts.Get<UIAvatarSkinMergeItem>(i);
                RoleSkinUnit partData = config.Get(key);
                RoleSkinUnitData value = data.Get(key);

                //标题，选中则展开内容
                if (keys.Length == 1)
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
                if (partData.texStyles.Count == 1)
                    item.icons.gameObject.SetActive(false);
                else
                {
                    item.icons.gameObject.SetActive(true);
                    item.icons.SetCount(partData.texStyles.Count);
                    for (int j = 0; j < partData.texStyles.Count; ++j)
                    {
                        var iconIdx = j;
                        var tex = partData.texStyles[j];
                        var btn = item.icons.Get<Button>(j);
                        var icon = btn.transform.Find("icon").GetComponent<RawImage>();
                        icon.texture = tex.texName.text;
                        icon.color = j == value.texStyle ? Color.yellow : Color.white * 0.8f;
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() =>
                        {
                            value.texStyle = iconIdx;
                            for (int k = 0; k < partData.texStyles.Count; ++k)
                            {
                                item.icons.Get(k).transform.Find("icon").GetComponent<RawImage>().color = iconIdx == k ? Color.yellow : Color.white * 0.8f;
                            }
                            merge.SetUnitData(value);
                        });
                    }
                }

                //色相、饱和度、明度
                var slider = item.hh;
                slider.gameObject.SetActive(partData.h);
                if (partData.h)
                {
                    slider.slider.onValueChanged.RemoveAllListeners();
                    slider.slider.value = value.h / 360f;
                    slider.slider.onValueChanged.AddListener((v) =>
                    {
                        var vv = (int)(v * 360f);
                        if (vv != value.h)
                        {
                            value.h = vv;
                            if (curSelectStyle>=0 && curSelectStyle<partData.colorStyles.Count)
                                partData.colorStyles[curSelectStyle].h = vv;
                            merge.SetUnitData(value);
                        }
                    });
                }

                slider = item.ss;
                slider.gameObject.SetActive(partData.s);
                if (partData.s)
                {
                    slider.slider.onValueChanged.RemoveAllListeners();
                    slider.slider.value = value.s / 3f;
                    slider.slider.onValueChanged.AddListener((v) =>
                    {
                        value.s = v * 3f;
                        if (curSelectStyle >= 0 && curSelectStyle < partData.colorStyles.Count)
                            partData.colorStyles[curSelectStyle].s = v * 3f;
                        merge.SetUnitData(value);
                    });
                }

                slider = item.vv;
                slider.gameObject.SetActive(partData.v);
                if (partData.v)
                {
                    slider.slider.onValueChanged.RemoveAllListeners();
                    slider.slider.value = value.v / 3f;
                    slider.slider.onValueChanged.AddListener((v) =>
                    {
                        value.v = v * 3f;
                        if (curSelectStyle >= 0 && curSelectStyle < partData.colorStyles.Count)
                            partData.colorStyles[curSelectStyle].v = v * 3f;
                        merge.SetUnitData(value);
                    });
                }

                // 颜色样式
                if (partData.colorStyles.Count == 1)
                    item.styles.gameObject.SetActive(false);
                else
                {
                    item.styles.gameObject.SetActive(true);
                    item.styles.SetCount(partData.colorStyles.Count);
                    for (int j = 0; j < partData.colorStyles.Count; ++j)
                    {
                        var style = partData.colorStyles[j];
                        var iconIdx = j;
                        var btn = item.styles.Get<Button>(j);
                        var icon = btn.GetComponent<Image>();
                        icon.color = style.RBGFloatColor;
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() =>
                        {
                            curSelectStyle = iconIdx;
                            value.colorStyle = iconIdx;
                            value.h = style.h;
                            value.s = style.s;
                            value.v = style.v;
                            merge.SetUnitData(value);

                            if (partData.h)
                            {
                                slider = item.hh;
                                slider.slider.value = value.h / 360f;
                            }
                            if (partData.s)
                            {
                                slider = item.ss;
                                slider.slider.value = value.s / 3f;
                            }
                            if (partData.v)
                            {
                                slider = item.vv;
                                slider.slider.value = value.v / 3f;
                            }
                            
                        });
                    }
                }
            }

        }

        void OnSel(int idx)
        {

            for (int i = 0; i < subTabsLen; ++i)
            {
                var item = m_subParts.Get<UIAvatarSkinMergeItem>(i);
                item.content.SetActive(i == idx);
            }

        }
    }
}
#endif