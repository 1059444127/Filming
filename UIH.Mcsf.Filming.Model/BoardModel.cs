using System;
using System.Collections.Generic;
using System.Diagnostics;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.Model
{
    // TODO-working-on: Extract IBoardModel From BoardModel


    public class BoardModel : IBoardModel
    {
        private List<IBoardCell> _boardCells = new List<IBoardCell>();
        private int _boardCount = 1;
        private int _boardNO;
        private int _displayedBoardCellCount = 1;
        private int _groupNO; // number of MaxDisplayMode is a group
        private readonly PageRepository _pageRepository;


        public BoardModel(PageRepository pageRepository)
        {
            _pageRepository = pageRepository;

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
                // TODO: when _displayBoardCellCount changed, Board should be changed to 
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
                // TODO: when boardNO changed, BoardModel should be refreshed
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
            _pageRepository.AppendPage();
        }

        #endregion

        #region [--Event From PageRepository--]

        private void RegisterDataModelEvent()
        {
            _pageRepository.PageChanged += PageRepositoryOnPageChanged;
            _pageRepository.FocusChanged += PageRepositoryOnFocusChanged;
            _pageRepository.PageCountChanged += PageRepositoryOnPageCountChanged;
        }

        private void PageRepositoryOnPageCountChanged(object sender, EventArgs eventArgs)
        {
            var pageCount = _pageRepository.Count;
            BoardCount = pageCount > 0 
                ? (int)Math.Ceiling((0.0+pageCount)/_displayedBoardCellCount)
                : 1;
        }

        private void PageRepositoryOnFocusChanged(object sender, EventArgs args)
        {
            var pageNO = _pageRepository.FocusIndex;
            GroupNO = pageNO/GlobalDefinitions.MaxDisplayMode;
            Debug.Assert(pageNO >= 0);
            BoardNO = pageNO/_displayedBoardCellCount;
        }

        private void PageRepositoryOnPageChanged(object sender, IntEventArgs intEventArgs)
        {
            var pageNO = intEventArgs.Int;
            var boardCellNO = BoardCellNOMapFrom(pageNO);
            var boardCell = _boardCells[boardCellNO];

            boardCell.PageModel = _pageRepository[pageNO];

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
                boardCell.PageModel = _pageRepository[pageNO];
                boardCell.IsVisible = IsInBoard(boardCellNO);
            }
        }

        // TODO-New-Feature: New Page is Selected
        // TODO-New-Feature: First Cell of New Page is Focused and Selected
        // TODO-New-Feature-working-on: New Page is Displayed
    }
}