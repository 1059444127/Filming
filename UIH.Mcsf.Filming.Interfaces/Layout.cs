using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Interfaces
{
    public abstract class Layout
    {
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
    }
}