using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace xys.UI.State
{
    public partial class UImageEA : TTTEA<Image>
    {
        public override void Init(Element element, ElementStateData sd)
        {
            if (element.target != null)
                sd.obj = element.GetTarget<Image>().sprite;
        }

        public override void Set(Element element, int index)
        {
            if (element.target != null)
            {
                Image image = element.GetTarget<Image>();
                image.sprite = element.stateData[index].obj as Sprite;

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(image);
#endif
            }
        }

#if UNITY_EDITOR
        public override bool ShowState(Element element, ElementStateData sc, int index)
        {
            sc.obj = EditorGUILayout.ObjectField(sc.obj, typeof(Sprite), true);

            return false;
        }
#endif
    }

    public partial class UTextEA : TTTEA<Text>
    {
        public override void Init(Element element, ElementStateData sd)
        {
            if (element.target != null)
                sd.strValue = element.GetTarget<Text>().text;
        }

        public override void Set(Element element, int index)
        {
            if (element.target != null)
            {
                Text label = element.GetTarget<Text>();
                label.text = element.stateData[index].strValue;

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(label);
#endif
            }
        }

#if UNITY_EDITOR
        public override bool ShowState(Element element, ElementStateData sc, int index)
        {
            return ShowString(ref sc.strValue);
        }
#endif
    }

    public partial class UAlphaEA : TTTEASmooth<MaskableGraphic>
    {
        public override void Init(Element element, ElementStateData sd)
        {
            if (element.target != null)
                sd.vector3.x = element.GetTarget<MaskableGraphic>().color.a;
        }

        public override void InitBySmooth(MaskableGraphic target, SmoothData sd)
        {
            sd.vector3.x = target.color.a;
        }

        public override void SetBySmooth(MaskableGraphic target, SmoothData sd, ElementStateData esd, float progress)
        {
            if (sd == null)
            {
                Color color = target.color;
                color.a = esd.vector3.x;
                target.color = color;
            }
            else
            {
                Color color = target.color;
                color.a = sd.Get(esd.vector3.x, progress);
                target.color = color;
            }
        }

#if UNITY_EDITOR
        public override bool ShowState(Element element, ElementStateData sc, int index)
        {
            return ShowSliderFloat(ref sc.vector3, "透明", 0, 1);
        }
#endif
    }

    public partial class UWidthEA : TTTEASmooth<RectTransform>
    {
        public override void Init(Element element, ElementStateData sd)
        {
            if (element.target != null)
                sd.vector3.x = element.GetTarget<RectTransform>().rect.width;
        }

        public override void InitBySmooth(RectTransform target, SmoothData sd)
        {
            sd.vector3.x = target.rect.width;
        }

        public override void SetBySmooth(RectTransform target, SmoothData sd, ElementStateData esd, float progress)
        {
            if (sd == null)
            {
                target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, esd.vector3.x);
            }
            else
            {
                target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sd.Get(esd.vector3.x, progress));
            }
        }

#if UNITY_EDITOR
        public override bool ShowState(Element element, ElementStateData sc, int index)
        {
            return ShowFloat(ref sc.vector3, "宽度");
        }
#endif
    }

    public partial class UHeightEA : TTTEASmooth<RectTransform>
    {
        public override void Init(Element element, ElementStateData sd)
        {
            if (element.target != null)
                sd.vector3.x = element.GetTarget<RectTransform>().rect.height;
        }

        public override void InitBySmooth(RectTransform target, SmoothData sd)
        {
            sd.vector3.x = target.rect.height;
        }

        public override void SetBySmooth(RectTransform target, SmoothData sd, ElementStateData esd, float progress)
        {
            if (sd == null)
            {
                target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, esd.vector3.x);
            }
            else
            {
                target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sd.Get(esd.vector3.x, progress));
            }
        }

#if UNITY_EDITOR
        public override bool ShowState(Element element, ElementStateData sc, int index)
        {
            return ShowFloat(ref sc.vector3, "高度");
        }
#endif
    }


    public partial class UColorEA : TTTEASmooth<MaskableGraphic>
    {
        public override void Init(Element element, ElementStateData sd)
        {
            if (element.target != null)
                sd.color32 = element.GetTarget<MaskableGraphic>().color;
        }

        public override void InitBySmooth(MaskableGraphic target, SmoothData sd)
        {
            sd.color32 = target.color;
        }

        public override void SetBySmooth(MaskableGraphic target, SmoothData sd, ElementStateData esd, float progress)
        {
            if (sd == null)
            {
                target.color = esd.color32;
            }
            else
            {
                target.color = sd.Get(esd.color32, progress);
            }
        }

#if UNITY_EDITOR
        public override bool ShowState(Element element, ElementStateData sc, int index)
        {
            return ShowColor32(ref sc.color32, "颜色");
        }
#endif
    }

    public partial class UMaterialEA : TTTEA<Graphic>
    {
        static Material GetGraphicMaterial(Graphic graphic)
        {
            if (graphic.material == Graphic.defaultGraphicMaterial)
                return null;
            return graphic.material;
        }

        public override void Init(Element element, ElementStateData sd)
        {
            if (element.target != null)
            {
                Graphic graphic = element.GetTarget<Graphic>();
                sd.obj = GetGraphicMaterial(graphic);
            }
        }

        public override void Set(Element element, int index)
        {
            if (element.target != null)
            {
                Graphic graphic = element.GetTarget<Graphic>();
                graphic.material = element.stateData[index].obj as Material;

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(graphic);
#endif
            }
        }

#if UNITY_EDITOR
        public override bool ShowState(Element element, ElementStateData sc, int index)
        {
            Material old = sc.obj as Material;
            Material material = (Material)EditorGUILayout.ObjectField(old, typeof(Material), false);
            if (GUILayout.Button("灰"))
                material = Resources.Load<Material>("UIGray");

            if (GUILayout.Button("空"))
                material = null;

            if (material != old)
            {
                RegisterUndo(() =>
                {
                    sc.obj = material;
                    if (sc.obj == Graphic.defaultGraphicMaterial)
                        sc.obj = null;
                });
                return true;
            }

            return false;
        }
#endif
    }

    public partial class UGradientEA : TTTEA<UnityEngine.UI.Gradient>
    {
        public override void Init(Element element, ElementStateData sd)
        {
            if (element.target != null)
            {
                sd.topColor = element.GetTarget<UnityEngine.UI.Gradient>().gradientTop;
                sd.bottomColor = element.GetTarget<UnityEngine.UI.Gradient>().gradientBottom;
            }
        }

        public override void Set(Element element, int index)
        {
            if (element.target != null)
            {
                UnityEngine.UI.Gradient gradient = element.GetTarget<UnityEngine.UI.Gradient>();
                gradient.gradientTop = element.stateData[index].topColor;
                gradient.gradientBottom = element.stateData[index].bottomColor;

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(gradient);
#endif
            }
        }

#if UNITY_EDITOR
        public override bool ShowState(Element element, ElementStateData sc, int index)
        {
            Color32 v = UnityEditor.EditorGUILayout.ColorField("顶颜色", sc.topColor);
            Color32 v2 = UnityEditor.EditorGUILayout.ColorField("底颜色", sc.bottomColor);
            if (XTools.Utility.Color32Equal(ref v, ref sc.topColor) && XTools.Utility.Color32Equal(ref v2, ref sc.bottomColor))
            {
                return false;
            }

            RegisterUndo<Color32>((ref Color32 vv) =>
            {
                vv = v;
            },
            ref sc.topColor);

            RegisterUndo<Color32>((ref Color32 vv) =>
            {
                vv = v2;
            },
            ref sc.bottomColor);

            return true;
        }
#endif
    }
}