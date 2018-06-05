using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace xys.UI.State
{
    public partial class PositionEA : TTTEASmooth<Transform>
    {
        public override void Init(Element element, ElementStateData sd)
        {
            if (element.target != null)
                sd.vector3 = element.GetTarget<Transform>().localPosition;
        }

        public override void InitBySmooth(Transform target, SmoothData sd)
        {
            sd.vector3 = target.localPosition;
        }

        public override void SetBySmooth(Transform target, SmoothData sd, ElementStateData esd, float progress)
        {
            if (sd == null)
            {
                target.localPosition = esd.vector3;
            }
            else
            {
                target.localPosition = sd.Get(esd.vector3, progress);
            }
        }

#if UNITY_EDITOR
        public override bool ShowState(Element element, ElementStateData sc, int index)
        {
            return ShowVector3(ref sc.vector3, "位置");
        }
#endif
    }

    public partial class RotateEA : TTTEASmooth<Transform>
    {
        public override void Init(Element element, ElementStateData sd)
        {
            if (element.target != null)
                sd.vector3 = element.GetTarget<Transform>().localEulerAngles;
        }

        public override void InitBySmooth(Transform target, SmoothData sd)
        {
            sd.vector3 = target.localEulerAngles;
        }

        public override void SetBySmooth(Transform target, SmoothData sd, ElementStateData esd, float progress)
        {
            if (sd == null)
            {
                target.localEulerAngles = esd.vector3;
            }
            else
            {
                target.localEulerAngles = sd.Get(esd.vector3, progress);
            }
        }

#if UNITY_EDITOR
        public override bool ShowState(Element element, ElementStateData sc, int index)
        {
            return ShowVector3(ref sc.vector3, "旋转");
        }
#endif
    }

    public class ScaleEA : TTTEASmooth<Transform>
    {
        public override void Init(Element element, ElementStateData sd)
        {
            if (element.target != null)
                sd.vector3 = element.GetTarget<Transform>().localScale;
        }

        public override void InitBySmooth(Transform target, SmoothData sd)
        {
            sd.vector3 = target.localScale;
        }

        public override void SetBySmooth(Transform target, SmoothData sd, ElementStateData esd, float progress)
        {
            if (sd == null)
            {
                target.localScale = esd.vector3;
            }
            else
            {
                target.localScale = sd.Get(esd.vector3, progress);
            }
        }

#if UNITY_EDITOR
        public override bool ShowState(Element element, ElementStateData sc, int index)
        {
            return ShowVector3(ref sc.vector3, "缩放");
        }
#endif
    }

}