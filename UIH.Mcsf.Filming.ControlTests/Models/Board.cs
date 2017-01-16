using System;
using System.Collections.Generic;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class Board : IBoard
    {
        #region Implementation of IBoard

        public int CellCount
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public event EventHandler CellCountChanged = delegate { };

        public IList<BoardCell> BoardCells
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void NewPage()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}