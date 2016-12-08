using System.Windows;
using UIH.Mcsf.Filming.DataModel;

namespace UIH.Mcsf.Filming.Adapters
{
    /// <summary>
    ///     Interaction logic for ViewerControlAdapter.xaml
    /// </summary>
    public partial class ViewerControlAdapter
    {
        public ViewerControlAdapter()
        {
            InitializeComponent();
            // TODO-later: ViewerControl Configure Path From Class Configure
            ViewerControl.InitializeWithoutCommProxy(@"D:/UIH/appdata/filming/config/");//Configure.Environment.Instance.ApplicationPath);
            // TODO-later: ViewerControl Layout From ViewModel
            ViewerControl.LayoutManager.SetLayout(2,2);
        }



        public Layout Layout
        {
            get { return (Layout)GetValue(LayoutProperty); }
            set
            {
                SetValue(LayoutProperty, value);
                var simpleLayout = value as SimpleLayout;
                if(simpleLayout != null) ViewerControl.LayoutManager.SetLayout(simpleLayout.Row, simpleLayout.Col);
            }
        }

        // Using a DependencyProperty as the backing store for Layout.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LayoutProperty =
            DependencyProperty.Register("Layout", typeof(Layout), typeof(ViewerControlAdapter));

        #region Overrides of FrameworkElement

        #endregion

        // TODO-New-Feature: Focus

        // TODO-New-Feature: Selection

        // TODO-New-Feature: Cells Dependecy Property

        // TODO-New-Feature: ViewPort Layout Dependency Property

        // TODO-New-Feature: regularLayout & irregularLayout

        // TODO-New-Feature: MultiFormatCell
    }
}