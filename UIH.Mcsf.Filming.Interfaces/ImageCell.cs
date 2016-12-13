using System;
using System.Diagnostics;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.MedDataManagement;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class ImageCell : ISelect
    {
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

        #region Implementation of ISelect

        public bool IsSelected { get; set; }

        public bool IsFocused { get; set; }

        public event EventHandler<ClickStatusEventArgs> Clicked = delegate {};

        #endregion

        public void OnClicked(IClickStatus clickStatus)
        {
            Clicked(this, new ClickStatusEventArgs(clickStatus));
        }
    }
}
