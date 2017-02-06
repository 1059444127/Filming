using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.ControlTests.ViewModel;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class Board : IBoard
    {
        private int _cellCount = 1;
        private readonly IBoardContent _boardContent;
        private readonly IRepository _repository;

        public Board(IBoardContent boardContent)
        {
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
            _boardContent.AppendContent();
        }

        public object this[int i]
        {
            get { return new PageControlViewModel {Page = _boardContent[i]}; }
        }

        #endregion
    }


    internal class RepositoryStub : IRepository
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