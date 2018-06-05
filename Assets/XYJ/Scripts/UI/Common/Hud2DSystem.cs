#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
/********************************************************************************
 *	文件名：	Hud2DSystem.cs
 *	创建人：	weipeng
 *	创建时间：  2016.07.20
 *
 *	功能说明： 飘字处理
 *           
 *	
 *	修改记录：
 *********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI;
using xys.UI;
using xys;


namespace xys.UI
{    
    [RequireComponent(typeof(Canvas))]
    public class Hud2DSystem : MonoBehaviour
    {
        class HudItem
        {
            public RectTransform tran;
            public Vector3 targetPosition;//初始位置
            public bool showInEdge;//当在屏幕外的时候要不要显示到屏幕边缘
        }

        private Canvas m_canvas;
        private RectTransform m_tran;
        private HashSet<HudItem> m_hudTextList = new HashSet<HudItem>();


        protected void Awake()
        {
            m_tran = this.GetComponent<RectTransform>();
            m_canvas = GetComponent<Canvas>();
            enabled = false;
        }

        //showInEdge表明如果在屏幕外的
        public void Bind(GameObject go, Vector3 targetPosition, bool showInEdge)
        {
            RectTransform tran = go.GetComponent<RectTransform>();
            Helper.SetLayer(tran, Layer.ui);
            tran.SetParent(m_tran);
            tran.localScale = Vector3.one;
            tran.localEulerAngles = Vector3.zero;

            HudItem item = new HudItem();
            item.tran = tran;
            item.targetPosition = targetPosition;
            item.showInEdge = showInEdge;

            m_hudTextList.Add(item);

            if (!enabled)
                enabled = true;
        }

        List<HudItem> m_removes = new List<HudItem>();
        void LateUpdate()
        {
            Camera mainCamera = ArtWrap.mainCamera;
            Camera uiCamera =App.my.uiSystem.RootCanvas.worldCamera;
            if (mainCamera == null || uiCamera == null)
            {
                m_canvas.enabled = false;
                return;
            }
            m_canvas.enabled = true;
            var caTran = mainCamera.transform;
            Rect tranRect = m_tran.rect;

            HudItem item = null;
            var enumerator = m_hudTextList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                item = enumerator.Current;
                if (item.tran == null || !item.tran.gameObject.activeSelf)
                {
                    m_removes.Add(item);
                    continue;
                }

                //计算屏幕坐标
                Vector3 screenPoint = mainCamera.WorldToScreenPoint(item.targetPosition);

                //如果在相机后面，那么不需要显示
                if (screenPoint.z < 0 && !item.showInEdge)
                {
                    item.tran.anchoredPosition = new Vector2(10000, 10000);//移到很远的地方隐藏掉
                    continue;
                }

                //如果不是平行相机而且在相机后面，那么屏幕坐标要取反
                if (screenPoint.z < 0 && !mainCamera.orthographic)//如果不是平行相机，那么屏幕后方要取反
                {
                    screenPoint.x = Screen.width - screenPoint.x;
                    screenPoint.y = Screen.height - screenPoint.y;
                }

                //计算ui坐标
                Vector2 uiPoint;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(m_tran, screenPoint, uiCamera, out uiPoint))
                {
                    item.tran.anchoredPosition = new Vector2(10000, 10000);//移到很远的地方隐藏掉
                    continue;
                }

                //显示在边缘
                if (item.showInEdge)
                {
                    Rect bound = XTools.MathUtil.GetRectByCenter(uiPoint.x, uiPoint.y, item.tran.sizeDelta.x, item.tran.sizeDelta.y);
                    if (screenPoint.z < 0)//如果是在后方统一显示在屏幕下方就可以了
                        item.tran.anchoredPosition = XTools.MathUtil.GetPosOfBoxYMin(tranRect, bound);
                    else
                        item.tran.anchoredPosition = XTools.MathUtil.GetPosInsideBox(tranRect, bound);
                }
                else
                    item.tran.anchoredPosition = uiPoint;
            }

            //删除已经清空的
            if (m_removes.Count != 0)
            {
                foreach (var t in m_removes)
                {
                    if (t != null)
                        m_hudTextList.Remove(t);
                }
                m_removes.Clear();
            }

            if (m_hudTextList.Count == 0)
                enabled = false;
        }

        public void Clear()
        {
            m_hudTextList.Clear();
            this.transform.DestroyChildren();
        }
    }

}

