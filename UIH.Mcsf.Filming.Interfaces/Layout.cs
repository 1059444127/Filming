using UIH.Mcsf.Filming.Configure;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Interfaces
{
    public abstract class Layout
    {
        public abstract int Capacity { get; }
        // TODO-New-Feature: Layout.ViewPort Layout Dependency Property
        // TODO-New-Feature: Layout.regularLayout & irregularLayout

        public abstract void Setup(LayoutManager layoutManager);

        public static Layout CreateDefaultLayout()
        {
            var layout = Environment.Instance.GetDefaultLayoutConfigure().Layout;
            return CreateLayout(layout.Rows, layout.Columns);
        }

        // TODO-Later: Layout.Equals HashCode For Dependency Property ViewerControlAdapter.Layout
        public static Layout CreateLayout(int row, int col)
        {
            return new SimpleLayout(row, col);
        }

        // TODO-Later: NullLayout.Singleton
        public static Layout CreateLayout()
        {
            return new NullLayout();
        }
    }
}