using System;
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

        
        public BoardModel()
        {
            for (int i = 0; i < GlobalDefinitions.MaxDisplayMode; i++)
            {
                _pageModels.Add(new PageModel());
            }
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
            _pageModels[0].IsVisible = true;
        }
    }
}
