using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UIH.Mcsf.Filming.Interface;

namespace UIH.Mcsf.Filming.DataModel
{
    public class Page : /*ObjectWithAspects,*/ ISelect
    {
        private List<ImageCell> _cells = new List<ImageCell>();
        private bool _isFocused;
        private bool _isSelected;

        public Page(List<ImageCell> cells, int pageNo, int pageCount, LayoutBase layout)
        {
            Cells = cells;
            No = pageNo;
            PageCount = pageCount;
            Layout = Layout.CreateLayout(layout.Columns, layout.Rows);
        }

        public Page(Layout layout, List<ImageCell> cells)
        {
            Layout = layout;
            var deltaCellCount = layout.Capacity - cells.Count;
            Debug.Assert(deltaCellCount >= 0);
            var pageCells = cells.ToList();
            pageCells.AddRange(CellFactory.Instance.CreateCells(deltaCellCount));
            Cells = pageCells;
        }

        public int No { get; set; }
        public int PageCount { get; set; }
        public bool IsBreak { get; set; }

        public List<ImageCell> Cells
        {
            get { return _cells; }
            set
            {
                _cells.ForEach(cell =>
                {
                    cell.SelectedChanged -= CellOnSelectedChanged;
                    cell.FocusedChanged -= CellOnFocusedChanged;
                });
                _cells = value;
                _cells.ForEach(cell =>
                {
                    cell.SelectedChanged += CellOnSelectedChanged;
                    cell.FocusedChanged += CellOnFocusedChanged;
                });
                IsSelected = _cells.Any(cell => cell.IsSelected);
            }
        }

        public Layout Layout { get; private set; }

        ~Page()
        {
            while (SelectedChanged != null)
            {
                SelectedChanged -= SelectedChanged;
            }
        }

        public event EventHandler<BoolEventArgs> SelectedChanged = delegate { };
        public event EventHandler<BoolEventArgs> FocusedChanged = delegate { };

        private void SetSelected(bool value)
        {
            if (_isSelected == value) return;
            _isSelected = value;
            SelectedChanged(this, new BoolEventArgs {Bool = value});
        }

        private void CellOnFocusedChanged(object sender, BoolEventArgs boolEventArgs)
        {
            IsFocused = boolEventArgs.Bool;
        }

        private void CellOnSelectedChanged(object sender, BoolEventArgs boolEventArgs)
        {
            SetSelected(boolEventArgs.Bool || Cells.Any(cell => cell.IsSelected));
        }

        public void Click(IClickStatus clickStatus)
        {
            Clicked(this, new ClickStatusEventArgs(clickStatus));
        }

        #region [--Implemented From ISelect--]

        public bool IsFocused
        {
            get { return _isFocused; }
            set
            {
                if (_isFocused == value) return;
                _isFocused = value;
                FocusedChanged(this, new BoolEventArgs {Bool = value});
            }
        }

        public event EventHandler<ClickStatusEventArgs> Clicked = delegate { };

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetSelected(value);
                _cells.ForEach(cell => cell.IsSelected = value);
            }
        }

        #endregion [--Implemented From ISelect--]
    }
}