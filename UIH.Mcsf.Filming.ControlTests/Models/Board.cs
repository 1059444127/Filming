using System;
using System.Collections.Generic;
using System.Diagnostics;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.ControlTests.ViewModel;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class Board : IBoard
    {

        public Board()
        {
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

        public void NewPage()
        {
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
            for (int i = 0; i < GlobalDefinitions.MaxDisplayMode; i++)
            {
                _boardCells.Add(new PageControlViewModel());
            }
        }
    }

    
}