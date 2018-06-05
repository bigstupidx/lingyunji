using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace DynamicShadowProjector
{
    public class TargetDirection : MonoBehaviour
    {
        public static Transform target { get; private set; }

        private void OnEnable()
        {
            target = transform;
        }

        private void OnDisable()
        {
            target = null;
        }
    }
}