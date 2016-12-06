using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UIH.Mcsf.Filming.DataModel
{
    public class Segment
    {
        private readonly ObservableCollection<Page> _pages = new ObservableCollection<Page>();

        public Segment(Layout layout, IEnumerable<ImageCell> cells)
        {
            var cellList = cells == null ? new List<ImageCell>() : cells.ToList();
            do
            {
                var pageCellCount = Math.Min(layout.Capacity, cellList.Count);
                var pageCells = cellList.GetRange(0, pageCellCount);
                var page = new Page(layout, pageCells);
                _pages.Add(page);
            } while (cellList.Count > 0);
        }

        //Todo-later: Repack状态

        public IEnumerable<Page> Pages
        {
            get { return _pages; }
        }
    }
}