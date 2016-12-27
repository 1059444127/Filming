using System;
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

        // TODO: Rule For MakeBoardView  
        // TODO: 区分 IsVisible & IsNull
        private void MakeBoardView()
        {
            for (int i = 0; i < _displayMode; i++)
            {
                _pageModels[i].IsVisible = true;
            }
            for (int i = _displayMode; i < GlobalDefinitions.MaxDisplayMode; i++)
            {
                _pageModels[i].IsVisible = false;
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

        public List<PageModel> PageModels
        {
            get { return _pageModels; }
            set { _pageModels = value; }
        }


        private List<PageModel> _pageModels = new List<PageModel>();
        private IList<Page> _pages = new List<Page>(); 
        private int _displayMode;
        
        // TODO-New-Feature: New Page is Selected, and its first Cell is Focused and Select
        public void NewPage()
        {
            // TODO: Layout of New Page
            // TODO: if _pages is not empty, last page change to a break page
            var index =_pageModels.FindLastIndex(p => p.IsVisible)+1;
            _pageModels[index].IsVisible = true;
            if (index != 0)
                _pageModels[index - 1].IsBreak = true;

            _pages.Add(new Page());
        }
    }
}
