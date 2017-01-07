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
            ViewerControl.InitializeWithoutCommProxy(@"D:/UIH/appdata/filming/config/");
                //Configure.Environment.Instance.ApplicationPath);
            ViewerControl.CanSelectCellByLeftClick = false; //Disable inner mouse left button down events.
        }

        private void SetLayout()
        {
            Layout.Setup(ViewerControl.LayoutManager);
            CompleteCells();
        }

        private void FillCells()
        {
            var cellCount = Math.Min(ImageCells.Count, ViewerControl.CellCount);
            for (var i = 0; i < cellCount; i++)
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
            if (deltaCellCount == 0) return;

            // cell count is less
            // TODO-later: New FilmingControlCell(), not smell good
            var cells = new List<FilmingControlCell>();
            for (var i = 0; i < deltaCellCount; i++)
            {
                cells.Add(new FilmingControlCell());
            }
            ViewerControl.AddCells(cells);

            // cell count is more,  not move, for performance conside ( ViewerControl only have interface to remove a cell, then refresh )

            // Register CellImpl.MouseDown Event
            for (var i = 0; i < deltaCellCount; i++)
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

        #region [--LayoutProperty--]

        public Layout Layout
        {
            get { return (Layout) GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Layout.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LayoutProperty =
            DependencyProperty.Register("Layout", typeof (Layout), typeof (ViewerControlAdapter),
                new PropertyMetadata(OnLayoutPropertyChanged));

        private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewerControlAdapter = d as ViewerControlAdapter;
            Debug.Assert(viewerControlAdapter != null);

            viewerControlAdapter.SetLayout();
        }

        #endregion [--LayoutProperty--]

        // TODO-New-Feature: ViewerControlAdapter.OnDrop (Original: use VisualTreeHelper to Location)


        // TODO-New-Feature: ControlCell.Clone
        

        // TODO-User-intent-later: Focus Status Stored in UI Layer

        // TODO-New-Feature: Remove Page

        // TODO-New-Feature: Select with up/down/left/right in a viewport

        // TODO-Later: ViewerControl.ImageCells and Memory Leak

        // TODO-Later: 图像逐个刷新，进度显示

        #region [--ImageCellsProperty--]

        public IList<ImageCell> ImageCells
        {
            get { return (IList<ImageCell>) GetValue(ImageCellsProperty); }
            set { SetValue(ImageCellsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageCells.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageCellsProperty =
            DependencyProperty.Register("ImageCells", typeof (IList<ImageCell>), typeof (ViewerControlAdapter),
                new PropertyMetadata(OnImageCellsPropertyChanged));

        private static void OnImageCellsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewerControlAdapter = d as ViewerControlAdapter;
            Debug.Assert(viewerControlAdapter != null);

            viewerControlAdapter.FillCells();
        }

        #endregion [--ImageCellsProperty--]
    }
}