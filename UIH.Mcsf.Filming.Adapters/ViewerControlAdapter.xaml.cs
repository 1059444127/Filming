using System.Diagnostics;
using System.Windows;
using UIH.Mcsf.Filming.Interfaces;

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
                //TODO: ViewerControlAdapter Set Layout for ViewerConrol
                value.Setup(ViewerControl.LayoutManager);
            }
        }


        // Using a DependencyProperty as the backing store for Layout.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LayoutProperty =
            DependencyProperty.Register("Layout", typeof(Layout), typeof(ViewerControlAdapter));


        #region Overrides of FrameworkElement

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            var property = e.Property;
            if (property == LayoutProperty)
            {
                OnLayoutPropertyChanged(e);
            }
        }

        private void OnLayoutPropertyChanged(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var layout = dependencyPropertyChangedEventArgs.NewValue as Layout;
            Debug.Assert(layout != null);
            layout.Setup(ViewerControl.LayoutManager);
        }

        #endregion

        // TODO-New-Feature: ViewerControlAdatper.Focus

        // TODO-New-Feature: ViewerControlAdatper.Selection

        // TODO-New-Feature: ViewerControlAdatper.Cells Dependecy Property

        // TODO-New-Feature: ViewerControlAdatper.ViewPort Layout Dependency Property

        // TODO-New-Feature: ViewerControlAdatper.regularLayout & irregularLayout

        // TODO-New-Feature: ViewerControlAdatper.MultiFormatCell

        // TODO-New-Feature: ViewerControlAdatper.Complete Cells when set layout
    }
}