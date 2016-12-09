using System.Diagnostics;
using System.Windows;
using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Viewer;

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
            ViewerControl.CanSelectCellByLeftClick = false; //Disable inner mouse left button down events.
        }



        public Layout Layout
        {
            get { return (Layout)GetValue(LayoutProperty); }
            set
            {
                SetValue(LayoutProperty, value);
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
            CompleteCells();
        }

        private void CompleteCells()
        {
            var capacity = ViewerControl.LayoutManager.RootCell.DisplayCapacity;
            var currentCellCount = ViewerControl.CellCount;
            var deltaCellCount = capacity - currentCellCount;

            // cell count is less
            for (int i = 0; i < deltaCellCount; i++)
            {
                ViewerControl.AddCell(new MedViewerControlCell());
            }

            // cell count is more
            for (int i = capacity-deltaCellCount-1; i >= capacity ; i--)
            {
                ViewerControl.RemoveCell(i);
            }

        }

        #endregion

        // TODO-New-Feature: ViewerControlAdapter.OnDrop (Original: use VisualTreeHelper to Location)
 
        // TODO-New-Feature: ViewerControlAdapter.MouseClick

        // TODO-New-Feature: ViewerControlAdapter.KeyPressed( UP/DOWN/LEFT/RIGHT )

        // TODO-New-Feature: ViewerControlAdapter.ModifierKeyPressed ( Ctrl/Shift )

        // TODO-New-Feature: ViewerControlAdapter.Cells Dependecy Property




    }
}