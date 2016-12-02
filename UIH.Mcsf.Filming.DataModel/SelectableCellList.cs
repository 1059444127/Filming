using System;
using System.Collections.Generic;
using System.Linq;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.Filming.Interface;
using UIH.Mcsf.Filming.Wrapper;

namespace UIH.Mcsf.Filming.DataModel
{
    [CallTrace(true)]
    public class SelectableCellList : SelectableList<ImageCell> //List<ImageCell>
    {
        //private int _lastSelected = -1;
        private LayoutBase _layout;

        public SelectableCellList(LayoutBase layout)
        {
            _layout = layout;
            AddRange(CellFactory.Instance.CreateCells(_layout.Capacity));
        }

        public LayoutBase Layout
        {
            set
            {
                if (_layout.Equals(value)) return;
                _layout = value;
                Polish();
            }
        }

        public int PageCount
        {
            get { return Count/_layout.Capacity; }
        }

        public event EventHandler Changed = delegate { };

        public void AddSeries(string seriesUid, int index = 0)
        {
            var newCells =
                DBWrapperHelper.DBWrapper.GetImageListBySeriesInstanceUID(seriesUid).Select(
                    image => CellFactory.Instance.CreateCell(image)).ToList();
            TrimBegin(index, newCells.Count());

            InsertRange(index, newCells, true);

            Polish();
        }

        private void TrimBegin(int index, int count)
        {
            var nonEmptyIndex = FindIndex(index, cell => !cell.IsEmpty);
            if (nonEmptyIndex == -1) nonEmptyIndex = Count;
            var emptyCellCount = nonEmptyIndex - index;
            var toBeRemovedCellCount = Math.Min(emptyCellCount, count);
            RemoveRange(index, toBeRemovedCellCount);
        }

        private void Polish()
        {
            TrimEnd();
            Complement();
            Changed(this, new EventArgs());
        }

        private void Complement()
        {
            var pageCellCount = _layout.Capacity;
            var lastPageCellCount = Count%pageCellCount;
            if (lastPageCellCount != 0)
                AddRange(CellFactory.Instance.CreateCells(pageCellCount - lastPageCellCount));
        }

        private void TrimEnd()
        {
            var emptyRangeStart = FindLastIndex(cell => !cell.IsEmpty);
            if (emptyRangeStart == -1) emptyRangeStart += _layout.Capacity; //we have to leave at lease an empty page
            if (emptyRangeStart > Count - 1) return;
            RemoveRange(emptyRangeStart + 1, Count - emptyRangeStart - 1);
        }

        public IList<ImageCell> GetCells(int pageIndex)
        {
            var pageCellCount = _layout.Capacity;
            var cellIndex = pageIndex*pageCellCount;
            if (cellIndex >= Count) return null;
            return GetRange(cellIndex, pageCellCount);
        }

        public IEnumerable<Page> BuildPages()
        {
            var pageCellCount = _layout.Capacity;
            var pageCount = Count/pageCellCount;
            for (int cellIndex = 0, pageIndex = 0; cellIndex < Count; cellIndex += pageCellCount, pageIndex++)
            {
                yield return new Page(GetRange(cellIndex, pageCellCount), pageIndex, pageCount, _layout);
            }
        }
    }
}