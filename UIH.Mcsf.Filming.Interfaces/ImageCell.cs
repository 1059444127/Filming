using System;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class ImageCell : ISelect
    {
        private bool _isSelected;
        private bool _isFocused;

        public ImageCell()
        {
            DisplayData = DisplayDataFactory.Instance.CreateDisplayData();
        }

        public DisplayData DisplayData { get; private set; }
        // TODO: try 异步加载方式

        public ImageCell(string sopInstanceUid)
        {
            DisplayData = DisplayDataFactory.Instance.CreateDisplayData(sopInstanceUid);
        }

        public event EventHandler<BoolEventArgs> SelectStatusChanged = delegate { };
        public event EventHandler<BoolEventArgs> FocusStatusChanged = delegate { }; 

        #region Implementation of ISelect

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if(_isSelected == value) return;
                _isSelected = value;
                SelectStatusChanged(this, new BoolEventArgs(value));
            }
        }

        public bool IsFocused
        {
            get { return _isFocused; }
            set
            {
                if(_isFocused == value) return;
                _isFocused = value;
                FocusStatusChanged(this, new BoolEventArgs(value));
            }
        }

        public event EventHandler<ClickStatusEventArgs> Clicked = delegate {};

        #endregion

        public void OnClicked(IClickStatus clickStatus)
        {
            Clicked(this, new ClickStatusEventArgs(clickStatus));
        }
    }
}
