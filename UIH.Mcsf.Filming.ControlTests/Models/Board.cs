using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.ControlTests.ViewModel;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class Board : IBoard
    {
        private IRepository _boardCellRepository;  

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

        public void AppendBoardCell()
        {
            _boardCellRepository.Add();
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

        private IList _boardCells;
        private int _cellCount = 1;


        private void InitializeBoardCells()
        {
            _boardCells = new ArrayList();
            for (int i = 0; i < GlobalDefinitions.MaxDisplayMode; i++)
            {
                _boardCells.Add(new PageControlViewModel());
            }
        }
    }

    
}