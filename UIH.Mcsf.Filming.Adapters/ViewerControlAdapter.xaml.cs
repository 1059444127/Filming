using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
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
                Layout.Setup(ViewerControl.LayoutManager);
                CompleteCells();
                return;
            }
            if (property == ImageCellsProperty)
            {
                FillCells();
            }
        }

        private void RegisterEvent()
        {
            
        }

        private void FillCells()
        {
            var cellCount = Math.Min(ImageCells.Count, ViewerControl.CellCount);
            for (int i = 0; i < cellCount; i++)
            {
                var controlCell = ViewerControl.Cells.ElementAt(i) as FilmingControlCell;
                Debug.Assert(controlCell != null);
                controlCell.FillImage(ImageCells[i]);
            }

        }

        private void CompleteCells()
        {
            var capacity = ViewerControl.LayoutManager.RootCell.DisplayCapacity;
            var currentCellCount = ViewerControl.CellCount;
            var deltaCellCount = capacity - currentCellCount;

            // cell count is less
            // TODO-later: New FilmingControlCell(), not smell good
            var cells = new List<FilmingControlCell>();
            for (int i = 0; i < deltaCellCount; i++)
            {
                cells.Add(new FilmingControlCell());
            }
            ViewerControl.AddCells(cells);

            // cell count is more,  not move, for performance conside ( ViewerControl only have interface to remove a cell, then refresh )

            // Register CellImpl.MouseDown Event
            for (int i = 0; i < deltaCellCount; i++)
            {
                var cellImpl = cells[i].Control;
                Debug.Assert(cellImpl != null);
                cellImpl.MouseDown -= CellImplOnMouseDown;
                cellImpl.MouseDown += CellImplOnMouseDown;
            }

        }

        private void CellImplOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var cellImpl = sender as MedViewerControlCellImpl;
            Debug.Assert(cellImpl != null);
            var controlCell = cellImpl.DataSource as FilmingControlCell;
            Debug.Assert(controlCell != null);

            controlCell.OnClicked(new ClickStatus(mouseButtonEventArgs.LeftButton == MouseButtonState.Pressed,
                mouseButtonEventArgs.RightButton == MouseButtonState.Pressed));
        }


        #endregion

        // TODO-New-Feature: ViewerControlAdapter.OnDrop (Original: use VisualTreeHelper to Location)
 




        // TODO-New-Feature: ControlCell.Clone

        
        // TODO-User-intent: Focus Status Stored in UI Layer

        // TODO-New-Feature: Remove Page

        // TODO-New-Feature: Select with up/down/left/right in a viewport

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