using System;
using System.Diagnostics;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.MedDataManagement;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class ImageCell : ISelect
    {
        private bool _isSelected;

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

        public bool IsFocused { get; set; }

        public event EventHandler<ClickStatusEventArgs> Clicked = delegate {};

        #endregion

        public void OnClicked(IClickStatus clickStatus)
        {
            Clicked(this, new ClickStatusEventArgs(clickStatus));
        }
    }
}
