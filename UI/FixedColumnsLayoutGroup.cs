/// Credit Simie
/// Sourced from - http://forum.unity3d.com/threads/flowlayoutgroup.296709/
/// Example http://forum.unity3d.com/threads/flowlayoutgroup.296709/
/// Update by Martin Sharkbomb - http://forum.unity3d.com/threads/flowlayoutgroup.296709/#post-1977028
/// Last item alignment fix by Vicente Russo - https://bitbucket.org/ddreaper/unity-ui-extensions/issues/22/flow-layout-group-align

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FancyUnity
{
    /// <summary>
    /// Layout Group controller that arranges children in rows, fitting as many on a line until total width exceeds parent bounds
    /// </summary>
    [AddComponentMenu("Layout/Extensions/FixedColumnsLayoutGroup")]
    public class FixedColumnsLayoutGroup : LayoutGroup
    {
        public int ColumnCount = 2;
        public Vector2 Gaps = new Vector2(5f, 5f);
        public float WHRatio = 16f / 9f;
        public bool CustomCellSize = false;
        public float CellWidth;
        public float CellHeight;
        public Vector2 Offset;

        protected RectTransform rectTrans;

        protected override void Start()
        {
            base.Start();
            rectTrans = GetComponent<RectTransform>();
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            if (rectTrans == null) return;
            var selfWidth = rectTrans.rect.width;
            SetLayoutInputForAxis(selfWidth, selfWidth, -1, 0);
        }

        public override void SetLayoutHorizontal()
        {
            //��������
            if(!CustomCellSize) CellWidth = Mathf.Abs(GetTotalPreferredSize(0) / ColumnCount);
            var cellWidth = CellWidth;
            cellWidth -= Gaps.x;
            transform.ForEach((tr, i) =>
            {
                var rectTr = tr as RectTransform;
                if (rectTr == null) return true;

                rectTr.anchorMin = rectTr.anchorMax = new Vector2(0, 0);
                var vec = rectTr.sizeDelta;
                vec.x = cellWidth;
                rectTr.sizeDelta = vec;
                var x = (i % ColumnCount) * CellWidth + Gaps.x + Offset.x;
                SetChildAlongAxis(rectTr, 0, x);

                return true;
            });
        }

        public override void CalculateLayoutInputVertical()
        {
            base.CalculateLayoutInputHorizontal();
            if (transform.childCount == 0)
            {
                var selfHeight = rectTrans.rect.height;
                SetLayoutInputForAxis(selfHeight, selfHeight, -1, 1);
            }
            else
            {
                if (!CustomCellSize) CellHeight = WHRatio * CellWidth;
                var selfHeight = CellHeight * (transform.childCount / ColumnCount) - Offset.y;
                SetLayoutInputForAxis(selfHeight, selfHeight, -1, 1);
                rectTrans.anchorMin = new Vector2(0, 1);
                rectTrans.anchorMax = new Vector2(1, 1);
                rectTrans.sizeDelta = new Vector2(0, selfHeight);
                rectTrans.anchoredPosition = Vector2.zero;
            }
        }

        public override void SetLayoutVertical()
        {
            
            var cellHeight = CellHeight;
            cellHeight -= Gaps.y;
            if (!CustomCellSize)  CellWidth = Mathf.Abs(GetTotalPreferredSize(0) / ColumnCount);
            var cellWidth = CellWidth;
            if (cellHeight < 0) cellHeight = 0;
            if (cellHeight == 0)
            {
                transform.ForEach((trans, i) =>
                {
                    var rectTrans = trans.GetComponent<RectTransform>();
                    if (rectTrans != null)
                    {
                        cellHeight = rectTrans.rect.height;
                        return false;
                    }
                    return true;
                });
            }
            transform.ForEach((trans, i) =>
            {
                var rectTr = trans.GetComponent<RectTransform>();
                if (rectTr == null) return true;
                //����ê��
                rectTr.anchorMin = rectTr.anchorMax = new Vector2(0, 0);
                //���ø߶�
                var vec = rectTr.sizeDelta;
                vec.y = cellHeight;
                rectTr.sizeDelta = vec;
                //����y��λ��
                var height = (i / ColumnCount + 1) * Gaps.y
                    + Mathf.Abs(i / ColumnCount * cellHeight) + Offset.y;
                SetChildAlongAxis(rectTr, 1, height);
                return true;
            });
        }
    }
}
