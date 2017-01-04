using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Adapters
{
    public class NullImageCell : ImageCell
    {
        private readonly DisplayData _displayData;

        public NullImageCell()
        {
            _displayData = DisplayDataFactory.Instance.CreateDisplayData();
        }

        #region Overrides of ImageCell

        public override DisplayData DisplayData
        {
            get { return _displayData; }
        }

        #endregion
    }
}