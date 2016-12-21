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

        private IList<PageModel> _pageModels = new List<PageModel>();
        private int _displayMode;
    }
}
