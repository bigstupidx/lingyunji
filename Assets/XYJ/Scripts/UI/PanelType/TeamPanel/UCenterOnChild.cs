#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace xys.UI
{
    /// <summary>
    /// 
    /// 拖动ScrollRect结束时始终让一个子物体位于中心位置。
    /// 
    /// </summary>
    public class UCenterOnChild : MonoBehaviour, IEndDragHandler, IDragHandler
    {
        //将子物体拉到中心位置时的速度
        public float centerSpeed = 9f;
        //起始偏移值
        public float ceterOffset = 0.0f;

        //注册该事件获取当拖动结束时位于中心位置的子物体
        public delegate void OnCenterHandler(GameObject centerChild);
        public event OnCenterHandler onCenter;

        private ScrollRect _scrollView;
        private Transform _container;

        private List<float> _childrenPos = new List<float>();
        private float _targetPos;
        private bool _centering = true;

        void Update()
        {
            if (_centering)
            {
                Vector3 v = _container.localPosition;
                v.y = Mathf.Lerp(_container.localPosition.y, _targetPos, centerSpeed * Time.deltaTime);
                _container.localPosition = v;
                if (Mathf.Abs(_container.localPosition.y - _targetPos) < 1.0f)
                {
                    _container.localPosition = new Vector3(_container.localPosition.x, _targetPos, _container.localPosition.z);
                }
            }
        }

        public void ResetItem()
        {
            _scrollView = GetComponent<ScrollRect>();
            if (_scrollView == null)
                return;
            _container = _scrollView.content;
            _scrollView.movementType = ScrollRect.MovementType.Unrestricted;

            _childrenPos.Clear();
            GridLayoutGroup grid;
            grid = _container.GetComponent<GridLayoutGroup>();
            if (grid == null)
                return;
            for (int i = 0; i < _container.childCount; i++)
            {
                _childrenPos.Add(ceterOffset + (i * (grid.cellSize.y + grid.spacing.y)));
            }
        }

        public void SetBeginIndex(int index)
        {
            if (index > _childrenPos.Count)
                return;
            _targetPos = _childrenPos[index];
            m_Index = index;
            ChangeFontState(-1, m_Index);
            _centering = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _centering = true;
            _targetPos = FindClosestPos(_container.localPosition.y);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _centering = false;
        }

        int m_Index;
        private float FindClosestPos(float currentPos)
        {
            int childIndey = 0;
            float closest = 0;
            float distance = Mathf.Infinity;

            int startPos = _childrenPos.Count - 1;
            if (startPos >= _container.childCount)
                startPos = _container.childCount - 1;

            for (int i = startPos; i >=0 ; i--)
            {
                float p = _childrenPos[i];
                float d = Mathf.Abs(p - currentPos);
                if (d < distance)
                {
                    distance = d;
                    closest = p;
                    childIndey = i;
                }
            }

            Debuger.Log(childIndey);
            GameObject centerChild = _container.GetChild(childIndey).gameObject;
            if (onCenter != null)
                onCenter(centerChild);

            ChangeFontState(m_Index, childIndey);
            m_Index = childIndey;
            return closest;
        }

        protected void ChangeFontState(int nOldIndex,int nNewIndex)
        {
            {
                string[] text = new string[] { "<color=#636172>", "</color>" };
                string[] nAnalysisFont;
                if(nOldIndex != -1)
                {
                    Text oldObj = _container.GetChild(nOldIndex).GetComponent<Text>();
                    nAnalysisFont = oldObj.text.Split(text, System.StringSplitOptions.RemoveEmptyEntries);
                    oldObj.text = "<color=#636172>" + nAnalysisFont[0] + "</color>";
                }
                if (nNewIndex != -1)
                {
                    Text newObj = _container.GetChild(nNewIndex).GetComponent<Text>();
                    nAnalysisFont = newObj.text.Split(text, System.StringSplitOptions.RemoveEmptyEntries);
                    newObj.text = nAnalysisFont[0];
                }
            }
        }

        public void ResetFontState()
        {
            string[] text = new string[] { "<color=#636172>", "</color>" };
            string[] nAnalysisFont;
            for(int i = 0 ; i < _container.childCount;i++)
            {
                Text oldObj = _container.GetChild(i).GetComponent<Text>();
                nAnalysisFont = oldObj.text.Split(text, System.StringSplitOptions.RemoveEmptyEntries);
                oldObj.text = "<color=#636172>" + nAnalysisFont[0] + "</color>";
            }
        }
    }
}

