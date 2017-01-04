using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Adapters
{
    class NullLayout : Layout
    {
        #region Overrides of Layout

        public override void Setup(LayoutManager layoutManager)
        {
        }

        public override int Capacity
        {
            get { return 0; }
        }

        #endregion
    }
}