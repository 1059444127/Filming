using System;
using System.Collections.Generic;
using System.Linq;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class BoardModel
    {
        public event EventHandler<IntEventArgs> DisplayModeChanged = delegate { };

        public int DisplayMode
        {
            set
            {
                if(_displayMode == value) return;
                _displayMode = value;
                DisplayModeChanged(this, new IntEventArgs(value));

                MakeBoardView();
            }
        }

        private void MakeBoardView()
        {
            for (int i = 0; i < _displayMode; i++)
            {
                _boardCells[i].IsVisible = true;
            }
            for (int i = _displayMode; i <= GlobalDefinitions.MaxDisplayMode; i++)
            {
                _boardCells[i].IsVisible = false;
            }
        }

        // TODO: BoardIndex
        private int _boardIndex = 0;
        // TODO: BoardCount


        public BoardModel()
        {
            for (int i = 0; i <= GlobalDefinitions.MaxDisplayMode; i++)
            {
                _boardCells.Add(new BoardCell());
            }
            _dataModel.PageChanged += DataModelOnPageChanged;
        }

        private void DataModelOnPageChanged(object sender, IntEventArgs intEventArgs)
        {
            int boardCellNO = BoardCellNOMapFrom(intEventArgs.Int);
            var boardCell = _boardCells[boardCellNO];

            boardCell.PageModel = _dataModel[intEventArgs.Int];

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

        public List<BoardCell> BoardCells
        {
            get { return _boardCells; }
            set { _boardCells = value; }
        }


        private List<BoardCell> _boardCells = new List<BoardCell>();
        private int _displayMode;
        private DataModel _dataModel = new DataModel();
        private int _groupNO = 0; // number of MaxDisplayMode is a group

        // TODO-New-Feature: New Page is Selected
        // TODO-New-Feature: First Cell of New Page is Focused and Selected
        public void NewPage()
        {
            _dataModel.AppendPage();
        }
    }

    class DataModel
    {
        private readonly IList<PageModel> _pages = new List<PageModel>();

        public void AppendPage()
        {
            MakeLastPageBreak();

            // TODO-Later: Layout of New Page is the same with LastPage
            _pages.Add(PageModel.CreatePageModel(Layout.CreateDefaultLayout()));
            PageChanged(this, new IntEventArgs(_pages.Count-1));
        }

        private void MakeLastPageBreak()
        {
            this[_pages.Count - 1].IsBreak = true;
        }

        public event EventHandler<IntEventArgs> PageChanged = delegate { };

        public PageModel this[int pageNO]
        {
            get
            {
                if (pageNO < 0 || pageNO >= _pages.Count) return PageModel.CreatePageModel(); 
                return _pages[pageNO];
            }
        }
    }
}
