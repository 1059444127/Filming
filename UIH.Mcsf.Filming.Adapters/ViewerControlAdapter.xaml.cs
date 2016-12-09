using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Filming.Model;
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
                Layout.Setup(ViewerControl.LayoutManager);
                CompleteCells();
                return;
            }
            if (property == ImageCellsProperty)
            {
                RefreshCells();
            }
        }

        private void RefreshCells()
        {

        }

        private void CompleteCells()
        {
            var capacity = ViewerControl.LayoutManager.RootCell.DisplayCapacity;
            var currentCellCount = ViewerControl.CellCount;
            var deltaCellCount = capacity - currentCellCount;

            // cell count is less
            for (int i = 0; i < deltaCellCount; i++)
            {
                //TODO: ViewerControl.AddCell(New FilmingControlCell);
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

        // TODO-New-Feature-working-on: ViewerControlAdapter.Cells Dependecy Property

        // TODO-Later: ViewerControl.ImageCells and Memory Leak
        public IList<ImageCell> ImageCells
        {
            get { return (IList<ImageCell>)GetValue(ImageCellsProperty); }
            set { SetValue(ImageCellsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageCells.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageCellsProperty =
            DependencyProperty.Register("ImageCells", typeof(IList<ImageCell>), typeof(ViewerControlAdapter));

        
        

    }
}