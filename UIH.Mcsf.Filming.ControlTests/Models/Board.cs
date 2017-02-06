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
        public Board(IBoardContent boardContent)
        {
            InitializeBoardCells();
            _boardContent = boardContent;
            _repository = new RepositoryStub();
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

        // TODO: When NewPage, PageCount changed -- That means How Board Send Message to every PageControlViewModel
        // TODO: When PageCount > 8, New Page, Then PageDown, and Focus on the NewPage
        public void AppendBoardCell()
        {
            _repository.Append();
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

        private IList<PageControlViewModel> _boardCells;
        private int _cellCount = 1;
        private IRepository _repository;
        private IBoardContent _boardContent;


        private void InitializeBoardCells()
        {
            _boardCells = new List<PageControlViewModel>();
            for (int i = 0; i < GlobalDefinitions.MaxDisplayMode; i++)
            {
                _boardCells.Add(new PageControlViewModel());
            }
        }
    }


    class RepositoryStub : IRepository
    {
        #region Implementation of IRepository

        public void Append()
        {
            throw new NotImplementedException();
        }

        public int Focus { get; set; }

        public event EventHandler FocusChanged = delegate { };

        #endregion
    }
}