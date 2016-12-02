using System;
using System.Collections.Generic;
using UIH.Mcsf.Filming.Wrapper;

namespace UIH.Mcsf.Filming.DataModel
{
    //职责：视图
    public interface IBoardChange
    {
        event EventHandler<IntEventArgs> DisplayModeChanged;
        event EventHandler<BoardEventArgs> BoardChanged;
    }

    public class Board : ObjectWithAspects, IBoardChange
    {
        private int _displayMode;
        private List<Page> _pages;

        public Board(int displayMode)
        {
            _pages = new List<Page>();
            DisplayMode = displayMode;
        }

        public int DisplayMode
        {
            set
            {
                if (_displayMode == value) return;
                _displayMode = value;
                DisplayModeChanged(this, new IntEventArgs {Int = value});
            }
        }

        public List<Page> Pages
        {
            set
            {
                _pages = value;
                BoardChanged(this, new BoardEventArgs(value));
            }
        }

        #region [--Implement From IBoardChange--]

        public event EventHandler<IntEventArgs> DisplayModeChanged = delegate { };

        public event EventHandler<BoardEventArgs> BoardChanged = delegate { }

            #endregion [--Implement From IBoardChange--]

            ;
    }
}