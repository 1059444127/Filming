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
                _pageDisplayModels[i].IsVisible = true;
            }
            for (int i = _displayMode; i < GlobalDefinitions.MaxDisplayMode; i++)
            {
                _pageDisplayModels[i].IsVisible = false;
            }
        }

        // TODO: BoardIndex
        // TODO: BoardCount

        public BoardModel()
        {
            for (int i = 0; i < GlobalDefinitions.MaxDisplayMode; i++)
            {
                _pageDisplayModels.Add(new PageDisplayModel());
            }
            _pageDisplayModels[0].IsVisible = true;
        }

        public List<PageDisplayModel> PageDisplayModels
        {
            get { return _pageDisplayModels; }
            set { _pageDisplayModels = value; }
        }


        private List<PageDisplayModel> _pageDisplayModels = new List<PageDisplayModel>();
        private IList<PageData> _pages = new List<PageData>(); 
        private int _displayMode;
        
        // TODO-New-Feature: New Page is Selected, and its first Cell is Focused and Select
        public void NewPage()
        {
            // TODO: Layout of New Page
            // TODO: if _pages is not empty, last page change to a break page
            var index =_pageDisplayModels.FindLastIndex(p => p.IsVisible)+1;
            _pageDisplayModels[index].IsVisible = true;
            if (index != 0)
                _pageDisplayModels[index - 1].IsBreak = true;

            _pages.Add(new PageData());
        }
    }
}
