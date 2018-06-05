#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.UI
{
    public class UIAvatarShapePage : MonoBehaviour
    {
        public UIGroup m_subParts;

        public void Set(RoleShapePart part, RoleShapeHandle handle)
        {
            m_subParts.SetCount(part.subParts.Count);
            for (int i = 0; i < part.subParts.Count; ++i)
            {
                var data = part.subParts[i];
                var uiItem = m_subParts.Get<UIAvatarShapePart>(i);
                uiItem.text.text = string.Format("----------------{0}------------------", data.name);
                uiItem.ps.SetCount(data.units.Count);

                for (int j = 0; j < data.units.Count; ++j)
                {
                    var param = data.units[j];
                    var uiParam = uiItem.ps.Get<UIAvatarShapeParam>(j);
                    uiParam.text.text = param.name;
                    uiParam.slider.onValueChanged.RemoveAllListeners();
                    uiParam.slider.normalizedValue = handle.GetValue01(param);

                    uiParam.slider.onValueChanged.AddListener((v) =>
                    {
                        handle.SetValue01(param, v);
                    });

                }

            }
        }

    }
}
#endif