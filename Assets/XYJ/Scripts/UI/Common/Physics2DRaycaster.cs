using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [AddComponentMenu("UI/Physics 2D Raycaster")]
    [RequireComponent(typeof(Camera))]
    public class Physics2DRaycaster : BaseRaycaster
    {
        Camera m_EventCamera;

        protected override void Awake()
        {
            m_EventCamera = GetComponent<Camera>();
        }

        public override Camera eventCamera
        {
            get
            {
                return m_EventCamera;
            }
        }

        public virtual int depth
        {
            get { return (m_EventCamera != null) ? (int)m_EventCamera.depth : 0xFFFFFF; }
        }

        public int finalEventMask
        {
            get { return m_EventCamera.cullingMask; }
        }

        static RaycastHit2D[] s_hits = new RaycastHit2D[10];

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (m_EventCamera == null)
                return;

            var ray = m_EventCamera.ScreenPointToRay(eventData.position);

            float dist = m_EventCamera.farClipPlane - m_EventCamera.nearClipPlane;

            int count = Physics2D.RaycastNonAlloc(ray.origin, ray.direction, s_hits, dist, finalEventMask);

            if (count != 0)
            {
                Vector3 cameraPosition = m_EventCamera.transform.position;
                for (int b = 0, bmax = count; b < bmax; ++b)
                {
                    RaycastHit2D hit = s_hits[b];

                    var graphic = hit.collider.GetComponent<Graphic>();
                    if (graphic == null)
                        continue;

                    Canvas canvas = graphic.canvas;
                    if (canvas == null)
                        continue;

                    var result = new RaycastResult
                    {
                        gameObject = hit.collider.gameObject,
                        module = this,
                        distance = Vector3.Distance(cameraPosition, hit.transform.position),
                        worldPosition = hit.point,
                        worldNormal = hit.normal,
                        screenPosition = eventData.position,
                        index = resultAppendList.Count,
                        depth = graphic.depth,
                        sortingLayer = canvas.sortingLayerID,
                        sortingOrder = canvas.sortingOrder,
                    };
                    resultAppendList.Add(result);
                }
            }
        }
    }
}
