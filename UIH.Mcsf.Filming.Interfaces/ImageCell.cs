using System;
using UIH.Mcsf.Filming.Abstracts;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Interfaces
{
    public abstract class ImageCell : ISelect
    {
        private bool _isFocused;
        private bool _isSelected;
        public abstract DisplayData DisplayData { get; }
        public event EventHandler SelectStatusChanged = delegate { };
        public event EventHandler FocusStatusChanged = delegate { };

        public void OnClicked(IClickStatus clickStatus)
        {
            Clicked(this, new ClickStatusEventArgs(clickStatus));
        }

        #region Implementation of ISelect

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                SelectStatusChanged(this, new EventArgs());
            }
        }

        public bool IsFocused
        {
            get { return _isFocused; }
            set
            {
                if (_isFocused == value) return;
                _isFocused = value;
                FocusStatusChanged(this, new EventArgs());
            }
        }

        public event EventHandler<ClickStatusEventArgs> Clicked = delegate { };

        #endregion
    }
}