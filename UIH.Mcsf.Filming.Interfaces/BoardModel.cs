﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class BoardModel
    {
        public event EventHandler<IntEventArgs> DisplayModeChanged = delegate { };

        public int DisplayMode
        {
            get { return _displayMode; }
            set
            {
                if(_displayMode == value) return;
                _displayMode = value;
                DisplayModeChanged(this, new IntEventArgs(value));

                MakeBoardView();
            }
        }

        private void MakeBoardView()
        {
            for (int i = 0; i < _displayMode; i++)
            {
                _boardCells[i].IsVisible = true;
            }
            for (int i = _displayMode; i < GlobalDefinitions.MaxDisplayMode; i++)
            {
                _boardCells[i].IsVisible = false;
            }
        }

        // TODO: BoardIndex
        private int _boardIndex = 0;
        // TODO: BoardCount


        public BoardModel()
        {
            for (int i = 0; i < GlobalDefinitions.MaxDisplayMode; i++)
            {
                _boardCells.Add(new BoardCell());
            }
        }

        public List<BoardCell> BoardCells
        {
            get { return _boardCells; }
            set { _boardCells = value; }
        }


        private List<BoardCell> _boardCells = new List<BoardCell>();
        private IList<PageModel> _pages = new List<PageModel>(); 
        private int _displayMode;
        
        // TODO-New-Feature: New Page is Selected, and its first Cell is Focused and Select
        public void NewPage()
        {
            // TODO: Layout of New Page
            // TODO: if _pages is not empty, last page change to a break page
            var boardCell = BoardCells[0];
            boardCell.PageModel = PageModel.CreatePageModel(Layout.CreateDefaultLayout());
            boardCell.IsVisible = true;

        }
    }
}
