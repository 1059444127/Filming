using UIH.Mcsf.Filming.Configure;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.Adapters
{
    public class LayoutFactory
    {
        public static Layout CreateDefaultLayout()
        {
            var layout = Environment.Instance.GetDefaultLayoutConfigure().Layout;
            return CreateLayout(layout.Rows, layout.Columns);
        }

        public static Layout CreateLayout(int row, int col)
        {
            return new SimpleLayout(row, col);
        }

        public static Layout CreateLayout()
        {
            return NullLayout;
        }

        private static readonly Layout NullLayout = new NullLayout();
    }
}