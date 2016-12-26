﻿using System;
using System.Collections.Generic;

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
            }
        }

        // TODO: BoardIndex
        // TODO: BoardCount

        public BoardModel()
        {
            for (int i = 0; i < GlobalDefinitions.MaxDisplayMode; i++)
            {
                _pageModels.Add(new PageModel());
            }
            _pageModels[0].IsVisible = true;
        }

        public IList<PageModel> PageModels
        {
            get { return _pageModels; }
            set { _pageModels = value; }
        }


        private IList<PageModel> _pageModels = new List<PageModel>();
        private IList<Page> _pages = new List<Page>(); 
        private int _displayMode;
        
        // TODO-New-Feature: New Page is Selected, and its first Cell is Focused and Select
        public void NewPage()
        {
            // TODO: Layout of New Page
            // TODO: if _pages is not empty, last page change to a break page
            _pages.Add(new Page());
        }
    }
}
