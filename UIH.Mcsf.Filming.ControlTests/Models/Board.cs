using System;
using System.Collections.Generic;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class Board : IBoard
    {
        private IPageRepository _pageRepository;

        public Board()
        {
            PageRepository = new PageRepositoryStub();
        }

        #region Implementation of IBoard

        public int CellCount
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public event EventHandler CellCountChanged = delegate { };

        public IList<BoardCell> BoardCells { get; set; }

        public void NewPage()
        {
            PageRepository.AppendPage();
        }

        #endregion

        public IPageRepository PageRepository
        {
            private get { return _pageRepository; }
            set
            {
                if (_pageRepository == value) return;
                _pageRepository = value;
                InitializeBoardCells();
            }
        }

        private void InitializeBoardCells()
        {
            BoardCells = new List<BoardCell>();
            for (int i = 0; i < GlobalDefinitions.MaxDisplayMode; i++)
            {
                BoardCells.Add(new BoardCell());
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