using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Interfaces
{
    public abstract class Layout
    {
        // TODO-New-Feature: Layout.ViewPort Layout Dependency Property
        // TODO-New-Feature: Layout.regularLayout & irregularLayout

        public abstract void Setup(LayoutManager layoutManager);

        public static Layout CreateDefaultLayout()
        {
            var layout = Configure.Environment.Instance.GetDefaultLayoutConfigure().Layout;
            return CreateLayout(layout.Rows, layout.Columns);
        }
        // TODO-Later: Layout.Equals HashCode For Dependency Property ViewerControlAdapter.Layout
        public static Layout CreateLayout(int row, int col)
        {
            return new SimpleLayout(row, col);
        }

        public abstract int Capacity { get; }
    }
}