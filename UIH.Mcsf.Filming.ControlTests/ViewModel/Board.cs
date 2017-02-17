using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    public class Board : IBoard
    {
        private int _count = 1;
        private readonly IBoardContent _boardContent;
        private readonly FilmControlViewModel[] _films = new FilmControlViewModel[GlobalDefinitions.MaxDisplayMode];

        public Board(IBoardContent boardContent)
        {
            _boardContent = boardContent;
            for (int i = 0; i < _films.Length; i++)
            {
                _films[i] = new FilmControlViewModel{Film = boardContent[i]};
            }
        }

        #region Implementation of IBoard

        public int Count
        {
            get { return _count; }
            set
            {
                if (_count == value) return;
                _count = value;
                CountChanged(this, new EventArgs());
            }
        }

        public event EventHandler CountChanged = delegate { };

        // TODO: When NewPage, PageCount changed -- That means How Board Send Message to every PageControlViewModel
        // TODO: When PageCount > 8, New Page, Then PageDown, and Focus on the NewPage
        public void Append()
        {
            _boardContent.Append();
        }

        public object this[int i]
        {
            get
            {
                Debug.Assert(i<GlobalDefinitions.MaxDisplayMode);
                return _films[i];
            }
        }

        #endregion
    }
}