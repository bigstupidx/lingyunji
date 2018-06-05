using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace xys
{
    public class DPOnCenterChild : MonoBehaviour
    {
        //起始偏移值
        public float ceterOffset = 0.0f;

        int m_Index;

        private ScrollRect _scrollView;
        private Transform _container;
        private List<float> _childrenPos = new List<float>();
        private float _targetPos;

        public void SetIndex(int index)
        {
            this.ReCalculateItems();

            if (index > _childrenPos.Count)
                return;

            _targetPos = _childrenPos[index];
            m_Index = index;

            _targetPos = _childrenPos[m_Index];

            if (_scrollView.horizontal)
                _container.localPosition = new Vector3(_targetPos, _container.localPosition.y, _container.localPosition.z);
            else if (_scrollView.vertical)
                _container.localPosition = new Vector3(_container.localPosition.x, _targetPos, _container.localPosition.z);
        }

        //刷新ITEMS位置
        void ReCalculateItems()
        {
            _scrollView = GetComponent<ScrollRect>();
            if (_scrollView == null)
                return;
            _container = _scrollView.content;
            _childrenPos.Clear();

            GridLayoutGroup grid;
            grid = _container.GetComponent<GridLayoutGroup>();
            if (grid != null)
            {
                for (int i = 0; i < _container.childCount; i++)
                {
                    if (_scrollView.horizontal)
                        _childrenPos.Add(ceterOffset + (i * (grid.cellSize.x + grid.spacing.x)));
                    else if (_scrollView.vertical)
                        _childrenPos.Add(ceterOffset + (i * (grid.cellSize.y + grid.spacing.y)));
                }
            }
                
            EasyLayout.EasyLayout eGird;
            eGird = _container.GetComponent<EasyLayout.EasyLayout>();
            if (eGird != null)
            {
                Vector2 margin = eGird.Margin;
                Vector2 spacing = eGird.Spacing;
                for (int i = 0; i < _container.childCount; i++)
                {
                    RectTransform rf = _container.GetChild(i).GetComponent<RectTransform>();
                    if (_scrollView.horizontal)
                        _childrenPos.Add(ceterOffset /*+ margin.x */+ (i / eGird.CompactConstraintCount * (spacing.x + rf.sizeDelta.x)));
                    else if (_scrollView.vertical)
                        _childrenPos.Add(ceterOffset /*+ margin.y*/ + (i / eGird.CompactConstraintCount * (spacing.y + rf.sizeDelta.y)));
                }
            }
        }
    }
}

