using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace xys.UI
{
    public class GraphicRaycaster : BaseRaycaster
    {
        Canvas mCanvas;

        protected override void Awake()
        {
            mCanvas = GetComponent<Canvas>();
        }

        public override int sortOrderPriority
        {
            get
            {
                if (mCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    return mCanvas.sortingOrder;

                return base.sortOrderPriority;
            }
        }

        public override int renderOrderPriority
        {
            get
            {
                if (mCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    return mCanvas.renderOrder;

                return base.renderOrderPriority;
            }
        }

        protected GraphicRaycaster()
        {

        }

        static List<Graphic> m_RaycastResults = new List<Graphic>();

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (mCanvas == null)
                return;

            Camera ec = eventCamera;
            if (ec == null)
                return;

            Vector2 pos = eventData.position;
            if (pos.x < 0 || pos.x > ec.pixelWidth || pos.y < 0 || pos.y > ec.pixelHeight)
                return;

            Ray ray = ec.ScreenPointToRay(eventData.position);

            m_RaycastResults.Clear();
            Raycast(mCanvas, ec, eventData.position, m_RaycastResults);
            if (m_RaycastResults.Count == 0)
                return;

            Quaternion rotation = ec.transform.rotation;
            for (var index = 0; index < m_RaycastResults.Count; index++)
            {
                Graphic graphic = m_RaycastResults[index];
                bool appendGraphic = true;

                // If we have a camera compare the direction against the cameras forward.
                var cameraFoward = rotation * Vector3.forward;
                var dir = graphic.rectTransform.rotation * Vector3.forward;
                appendGraphic = Vector3.Dot(cameraFoward, dir) > 0;

                if (appendGraphic)
                {
                    float distance = 0;

                    if (mCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                        distance = 0;
                    else
                    {
                        Transform trans = graphic.rectTransform;
                        Vector3 transForward = trans.forward;
                        // http://geomalgorithms.com/a06-_intersect-2.html
                        distance = (Vector3.Dot(transForward, trans.position - ray.origin) / Vector3.Dot(transForward, ray.direction));

                        // Check to see if the go is behind the camera.
                        if (distance < 0)
                            continue;
                    }

                    var castResult = new RaycastResult
                    {
                        gameObject = graphic.gameObject,
                        module = this,
                        distance = distance,
                        screenPosition = eventData.position,
                        index = resultAppendList.Count,
                        depth = graphic.depth,
                        sortingLayer = mCanvas.sortingLayerID,
                        sortingOrder = mCanvas.sortingOrder
                    };
                    resultAppendList.Add(castResult);
                }
            }

            m_RaycastResults.Clear();
        }

        public override Camera eventCamera
        {
            get
            {
                switch (mCanvas.renderMode)
                {
                case RenderMode.ScreenSpaceOverlay:
                    return null;
                case RenderMode.ScreenSpaceCamera:
                    {
                        Camera c = mCanvas.worldCamera;
                        if (c != null)
                            return c;
                    }
                    break;
                }

                return Camera.main;
            }
        }

        /// <summary>
        /// Perform a raycast into the screen and collect all graphics underneath it.
        /// </summary>
        [NonSerialized]
        static readonly List<Graphic> s_SortedGraphics = new List<Graphic>();
        private static void Raycast(Canvas canvas, Camera eventCamera, Vector2 pointerPosition, List<Graphic> results)
        {
            // Debug.Log("ttt" + pointerPoision + ":::" + camera);
            // Necessary for the event system
            var foundGraphics = GraphicRegistry.GetGraphicsForCanvas(canvas);
            for (int i = 0; i < foundGraphics.Count; ++i)
            {
                Graphic graphic = foundGraphics[i];

                // -1 means it hasn't been processed by the canvas, which means it isn't actually drawn
                if (graphic.depth == -1 || !graphic.raycastTarget || !graphic.isActiveAndEnabled)
                    continue;

                if (!RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, pointerPosition, eventCamera))
                    continue;

                if (!UITools.IsMask(eventCamera, graphic, pointerPosition))
                    continue;

                s_SortedGraphics.Add(graphic);

                //if (graphic.Raycast(pointerPosition, eventCamera))
                //{
                //    s_SortedGraphics.Add(graphic);
                //}
            }

            s_SortedGraphics.Sort((g1, g2) => g2.depth.CompareTo(g1.depth));
            //		StringBuilder cast = new StringBuilder();
            for (int i = 0; i < s_SortedGraphics.Count; ++i)
                results.Add(s_SortedGraphics[i]);
            //		Debug.Log (cast.ToString());

            s_SortedGraphics.Clear();
        }
    }
}
