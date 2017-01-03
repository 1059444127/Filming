using System;
using System.Collections.Generic;
using System.Diagnostics;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.Model
{
    public class BoardModel
    {
        private List<BoardCell> _boardCells = new List<BoardCell>();
        // TODO-working-on: BoardCount
        private int _boardCount = 10;
        // TODO: BoardNO
        private int _boardNO;
        private int _displayMode;
        private int _groupNO; // number of MaxDisplayMode is a group
        private readonly DataModel _dataModel = new DataModel();
        // TODO: PageCount Changed notification from Selectable<PageModel>


        public BoardModel()
        {
            for (var i = 0; i <= GlobalDefinitions.MaxDisplayMode; i++)
            {
                _boardCells.Add(new BoardCell());
            }
            _dataModel.PageChanged += DataModelOnPageChanged;
            _dataModel.FocusChanged += DataModelOnFocusChanged;
        }

        public int DisplayMode
        {
            set
            {
                if (_displayMode == value) return;
                _displayMode = value;
                DisplayModeChanged(this, new IntEventArgs(value));

                MakeBoardView();
            }
        }

        public List<BoardCell> BoardCells
        {
            get { return _boardCells; }
            set { _boardCells = value; }
        }

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

        private int BoardNO
        {
            set
            {
                if (_boardNO == value) return;
                _boardNO = value;
                Debug.Assert(_boardNO >= 0 && _boardNO < _boardCount);
                BoardNOChanged(this, new IntEventArgs(value));
            }
        }

        private int BoardCount
        {
            set
            {
                if (_boardCount == value) return;
                _boardCount = value;
                Debug.Assert(_boardCount > _boardNO);
                BoardCountChanged(this, new IntEventArgs(value));
            }
        }

        public event EventHandler<IntEventArgs> DisplayModeChanged = delegate { };
        public event EventHandler<IntEventArgs> BoardNOChanged = delegate { };
        public event EventHandler<IntEventArgs> BoardCountChanged = delegate { };

        private void MakeBoardView()
        {
            for (var i = 0; i < _displayMode; i++)
            {
                _boardCells[i].IsVisible = true;
            }
            for (var i = _displayMode; i <= GlobalDefinitions.MaxDisplayMode; i++)
            {
                _boardCells[i].IsVisible = false;
            }
        }

        private void DataModelOnFocusChanged(object sender, IntEventArgs intEventArgs)
        {
            var pageNO = intEventArgs.Int;
            GroupNO = pageNO/GlobalDefinitions.MaxDisplayMode;
            Debug.Assert(pageNO >= 0);
            BoardNO = pageNO/_displayMode;
        }

        private void DataModelOnPageChanged(object sender, IntEventArgs intEventArgs)
        {
            var pageNO = intEventArgs.Int;
            var boardCellNO = BoardCellNOMapFrom(pageNO);
            var boardCell = _boardCells[boardCellNO];

            boardCell.PageModel = _dataModel[pageNO];

            boardCell.IsVisible = IsInBoard(boardCellNO);
        }

        private bool IsInBoard(int boardCellNO)
        {
            return boardCellNO < _displayMode;
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
        public void NewPage()
        {
            _dataModel.AppendPage();
        }
    }

    internal class DataModel
    {
        private readonly IList<PageModel> _pages = new List<PageModel>();

        public PageModel this[int pageNO]
        {
            get
            {
                if (pageNO < 0 || pageNO >= _pages.Count) return PageModel.CreatePageModel();
                return _pages[pageNO];
            }
        }

        public void AppendPage()
        {
            MakeLastPageBreak();

            // TODO-Later: Layout of New Page is the same with LastPage
            _pages.Add(PageModel.CreatePageModel(Layout.CreateDefaultLayout()));

            var lastPageNO = _pages.Count - 1;
            PageChanged(this, new IntEventArgs(lastPageNO));
            FocusChanged(this, new IntEventArgs(lastPageNO));
        }

        private void MakeLastPageBreak()
        {
            this[_pages.Count - 1].IsBreak = true;
        }

        public event EventHandler<IntEventArgs> PageChanged = delegate { };
        public event EventHandler<IntEventArgs> FocusChanged = delegate { };
        // TODO-working-on: pageCountChanged event
    }
}