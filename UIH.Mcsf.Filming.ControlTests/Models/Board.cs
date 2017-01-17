using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.ControlTests.ViewModel;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class Board : IBoard
    {
        private IPageRepository _pageRepository;

        public Board()
        {
            _pageRepository = new PageRepositoryStub();
            InitializeBoardCells();
        }

        #region Implementation of IBoard

        public int CellCount
        {
            get { return _cellCount; }
            set
            {
                if (_cellCount == value) return;
                _cellCount = value;
                CellCountChanged(this, new EventArgs());
            }
        }

        public event EventHandler CellCountChanged = delegate { };

        public IList<BoardCell> BoardCells { get; set; }

        public void NewPage()
        {
            _pageRepository.AppendPage();
        }

        public object this[int i]
        {
            get
            {
                Debug.Assert(i>=0 && i<_boardCells.Count);
                return _boardCells[i];              
            }
        }

        #endregion

        private IList<PageControlViewModel> _boardCells = new List<PageControlViewModel>();
        private int _cellCount = 1;


        private void InitializeBoardCells()
        {
            BoardCells = new List<BoardCell>();
            for (int i = 0; i < GlobalDefinitions.MaxDisplayMode; i++)
            {
                _boardCells.Add(new PageControlViewModel());
            }
        }
    }

    class PageRepositoryStub : IPageRepository
    {
        #region Implementation of IPageRepository

        public void AppendPage()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}