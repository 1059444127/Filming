﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using UIH.Mcsf.Filming.Adapters;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.Model
{
    // TODO-working-on: Extract IBoardModel From BoardModel


    public class BoardModel : IBoardModel
    {
        private List<IBoardCell> _boardCells = new List<IBoardCell>();
        // TODO-working-on: BoardCount
        private int _boardCount = 1;
        // TODO: BoardNO
        private int _boardNO;
        private int _displayedBoardCellCount = 1;
        private int _groupNO; // number of MaxDisplayMode is a group
        private readonly DataModel _dataModel;
        // TODO: PageCount Changed notification from Selectable<PageModel>


        public BoardModel(DataModel dataModel)
        {
            _dataModel = dataModel;

            for (var i = 0; i <= GlobalDefinitions.MaxDisplayMode; i++)
            {
                _boardCells.Add(new BoardCell()); 
            }
            RegisterDataModelEvent();
        }

        #region [--Implement From ICellCount--]

        public int CellCount
        {
            get {return _displayedBoardCellCount;}
            set
            {
                if (_displayedBoardCellCount == value) return;
                _displayedBoardCellCount = value;
                CellCountChanged(this, new EventArgs());
                MakeBoardView();
            }
        }

        public event EventHandler CellCountChanged = delegate { };

        #endregion

        #region [--Implement From IBoardComponet--]

        public List<IBoardCell> BoardCells
        {
            get { return _boardCells; }
            set { _boardCells = value; }
        }

        #endregion

        #region [--Implement From IBoardModel--]

        public int BoardNO
        {
            get { return _boardNO; }
            set
            {
                if (_boardNO == value) return;
                _boardNO = value;
                Debug.Assert(_boardNO >= 0 && _boardNO < _boardCount);
                BoardNOChanged(this, new EventArgs());
            }
        }

        public int BoardCount
        {
            get { return _boardCount; }
            private set
            {
                if (_boardCount == value) return;
                _boardCount = value;
                // TODO-Bug-in-BoardModel: When DisplayModel=4, PageCount=4, NewPage, Then BoardCount=1, BoardNO=1, BoardCount=BoardNO
                Debug.Assert(_boardCount > _boardNO);
                BoardCountChanged(this, new EventArgs());
            }
        }

        // TODO-working-on: BoardModel.EventHandler<IntEventArgs> to EventHandler 
        public event EventHandler BoardNOChanged = delegate { };
        public event EventHandler BoardCountChanged = delegate { };

        public void NewPage()
        {
            _dataModel.AppendPage();
        }

        #endregion

        #region [--Event From DataModel--]

        private void RegisterDataModelEvent()
        {
            _dataModel.PageChanged += DataModelOnPageChanged;
            _dataModel.FocusChanged += DataModelOnFocusChanged;
            _dataModel.PageCountChanged += DataModelOnPageCountChanged;
        }

        private void DataModelOnPageCountChanged(object sender, EventArgs eventArgs)
        {
            var pageCount = _dataModel.Count;
            BoardCount = pageCount > 0 
                ? (int)Math.Ceiling((0.0+pageCount)/_displayedBoardCellCount)
                : 1;
        }

        private void DataModelOnFocusChanged(object sender, IntEventArgs intEventArgs)
        {
            var pageNO = intEventArgs.Int;
            GroupNO = pageNO/GlobalDefinitions.MaxDisplayMode;
            Debug.Assert(pageNO >= 0);
            BoardNO = pageNO/_displayedBoardCellCount;
        }

        private void DataModelOnPageChanged(object sender, IntEventArgs intEventArgs)
        {
            var pageNO = intEventArgs.Int;
            var boardCellNO = BoardCellNOMapFrom(pageNO);
            var boardCell = _boardCells[boardCellNO];

            boardCell.PageModel = _dataModel[pageNO];

            boardCell.IsVisible = IsInBoard(boardCellNO);
        }

        #endregion

        private int GroupNO
        {
            set
            {
                if (_groupNO == value) return;
                // TODO-Later: BoardModel.GroupNO Changed, PageModels Changed, make a progress Bar
                _groupNO = value;

                RefreshGroup();
            }
        }

        private void MakeBoardView()
        {
            for (var i = 0; i < _displayedBoardCellCount; i++)
            {
                _boardCells[i].IsVisible = true;
            }
            for (var i = _displayedBoardCellCount; i <= GlobalDefinitions.MaxDisplayMode; i++)
            {
                _boardCells[i].IsVisible = false;
            }
        }

        private bool IsInBoard(int boardCellNO)
        {
            return boardCellNO < _displayedBoardCellCount;
        }

        private int BoardCellNOMapFrom(int pageNO)
        {
            var groupNO = pageNO/GlobalDefinitions.MaxDisplayMode;
            if (groupNO != _groupNO) return GlobalDefinitions.MaxDisplayMode;

            return pageNO%GlobalDefinitions.MaxDisplayMode;
        }

        private void RefreshGroup()
        {
            for (int boardCellNO = 0, pageNO = _groupNO*GlobalDefinitions.MaxDisplayMode;
                boardCellNO < GlobalDefinitions.MaxDisplayMode;
                boardCellNO++, pageNO++)
            {
                var boardCell = _boardCells[boardCellNO];
                boardCell.PageModel = _dataModel[pageNO];
                boardCell.IsVisible = IsInBoard(boardCellNO);
            }
        }

        // TODO-New-Feature: New Page is Selected
        // TODO-New-Feature: First Cell of New Page is Focused and Selected
        // TODO-New-Feature-working-on: New Page is Displayed
    }

    public class DataModel : SelectableList<PageModel>
    {
        public override PageModel this[int pageNO]
        {
            get
            {
                if (pageNO < 0 || pageNO >= Count) return PageModelFactory.CreatePageModel();
                return base[pageNO];
            }
        }

        public virtual void AppendPage()
        {
            MakeLastPageBreak();

            // TODO-Later: Layout of New Page is the same with LastPage
            // TODO-Later： DataModel use LayoutFactory.CreateDefaultLayout(), Depends on File system, not good to UT 
            Add(PageModelFactory.CreatePageModel(LayoutFactory.CreateDefaultLayout()));

            var lastPageNO = Count - 1;
            PageChange(lastPageNO);
            FocusChange(lastPageNO);
        }

        private void MakeLastPageBreak()
        {
            this[Count - 1].IsBreak = true;
        }

        #region [--For UT--]

        protected void PageChange(int pageNO)
        {
            PageChanged(this, new IntEventArgs(pageNO));
        }

        protected void FocusChange(int pageNO)
        {
            FocusChanged(this, new IntEventArgs(pageNO));
        }

        #endregion

        #region [--Events--]

        public event EventHandler<IntEventArgs> PageChanged = delegate { };
        // TODO: PageControl.IsFocused(TitleBar.Border=Yellow & IsSelected(TitleBar.Fill=Aqua)
        public event EventHandler<IntEventArgs> FocusChanged = delegate { };
        // TODO-working-on: pageCountChanged event
        // TODO-UT: DataModel.PageCountChanged
        public event EventHandler PageCountChanged = delegate { };

        #endregion

        #region Overrides of SelectableList<PageModel>

        public override void Add(PageModel item)
        {
            base.Add(item);
            PageCountChanged(this, new EventArgs());
        }

        public override void Clear()
        {
            base.Clear();
            PageCountChanged(this, new EventArgs());
        }

        public override bool Remove(PageModel item)
        {
            var remove = base.Remove(item);
            PageCountChanged(this, new EventArgs());
            return remove;
        }

        public override void Insert(int index, PageModel item)
        {
            base.Insert(index, item);
            PageCountChanged(this, new EventArgs());
        }

        public override void RemoveAt(int index)
        {
            base.RemoveAt(index);
            PageCountChanged(this, new EventArgs());
        }

        #endregion
    }
}