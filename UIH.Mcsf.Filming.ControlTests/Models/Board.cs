using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.ControlTests.ViewModel;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class Board : IBoard
    {
        private int _cellCount = 1;
        private readonly IBoardContent _boardContent;

        public Board(IBoardContent boardContent)
        {
            _boardContent = boardContent;
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
            get { return new FilmControlViewModel {Film = _boardContent[i]}; }
        }

        #endregion
    }
}