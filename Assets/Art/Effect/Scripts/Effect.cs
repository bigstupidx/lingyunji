using System;
using UnityEngine;
using System.Collections.Generic;

namespace UnityStandardAssets.ImageEffects
{
    [Serializable]
    public class Effect
    {
        public CameraStruct cameraStruct = new CameraStruct();

        CameraEffects current;

        GameObject root;

        public CameraEffects.Effect parent;

        public Camera currentCamera
        {
            get { return current != null ? current.Camera : null; }
        }

        public void Init(GameObject gameObject, CameraEffects.Effect p)
        {
            root = gameObject;
            parent = p;

            LateUpdate();
        }

        public void OnDisable()
        {
            OnRemove();
        }

        public void OnRemove()
        {
            if (current != null)
            {
                parent.RestoreCamera();
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    UnityEngine.Object.DestroyImmediate(current);
                else
                    UnityEngine.Object.Destroy(current);
#else
					UnityEngine.Object.Destroy(current);
#endif
                current = null;
            }
        }

        public void LateUpdate()
        {
            Camera nowcurrent = cameraStruct.GetCurrent(root);
            if (nowcurrent == null)
            {
                OnRemove();
                return;
            }

            if (current != null && current.Camera == nowcurrent)
                return;

            OnRemove();
            current = nowcurrent.gameObject.AddComponent<CameraEffects>();
            current.current = parent;
        }
    }
}