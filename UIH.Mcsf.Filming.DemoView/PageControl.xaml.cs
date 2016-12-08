using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using UIH.Mcsf.Filming.DataModel;
using UIH.Mcsf.Filming.DemoViewModel;
using UIH.Mcsf.Filming.Interface;
using UIH.Mcsf.Filming.Wrapper;
using UIH.Mcsf.Viewer;
using Environment = UIH.Mcsf.Filming.Configure.Environment;

namespace UIH.Mcsf.Filming.DemoView
{
    /// <summary>
    ///     Interaction logic for PageControl.xaml
    /// </summary>
    [CallTrace(true)]
    public partial class PageControl
    {
        private Page _pageModel;

        public PageControl()
        {
            // TODO-Working-on: PageControl 改用 ViewerControlAdapter (original: MedViewerControl)
            InitializeComponent();
            Initialize();
        }

        public Page PageModel
        {
            set
            {
                _pageModel = value;
                Refresh();
                DataContext = new PageViewModel(_pageModel);
            }
        }

        // TODO: PageControl.OnTitleBarMouseDown : Replaced By Command
        private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            _pageModel.Click(new ClickStatus(
                e.LeftButton == MouseButtonState.Pressed,
                e.RightButton == MouseButtonState.Pressed
                ));
        }

        #region [--Private Viewer Control Related Function--]

        private void Initialize()
        {
            InitViewerControl();

            ImageCell.ViewerConfiguration = _viewerControl.Configuration;
        }

        private void InitViewerControl()
        {
            _viewerControl.InitializeWithoutCommProxy(Environment.Instance.FilmingUserConfigPath);
            _viewerControl.CanSelectCellByLeftClick = false; //Disable inner mouse left button down events.

            _viewerControl.RemoveAll();
            AddCells(2*2); //viewerControl默认布局2x2
        }

        private LayoutBase Layout
        {
            set
            {
                if (Layout.Equals(value)) return;

                var layoutManager = _viewerControl.LayoutManager;
                layoutManager.SetLayout(value.Rows, value.Columns);

                var rootCell = layoutManager.RootCell;
                var deltaCellCnt = rootCell.DisplayCapacity - _viewerControl.CellCount;
                ComplementCells(deltaCellCnt);
            }
            get
            {
                var layoutManager = _viewerControl.LayoutManager;
                return LayoutFactory.Instance.CreateLayout(layoutManager.Columns, layoutManager.Rows);
            }
        }

        private void ComplementCells(int deltaCellCnt)
        {
            AddCells(deltaCellCnt);

            for (var i = 0; i < -deltaCellCnt; i++)
            {
                _viewerControl.RemoveCell(_viewerControl.CellCount - 1);
            }
        }

        private void Refresh()
        {
            SetLayout(_pageModel.Layout);
            Refresh(_pageModel.Cells);
            RegisterCellMouseEvent();
        }

        private void SetLayout(Layout layout)
        {
            var layoutManager = _viewerControl.LayoutManager;

            if (layout is SimpleLayout)
            {
                var simpleLayout = layout as SimpleLayout;
                layoutManager.SetLayout(simpleLayout.Row, simpleLayout.Col);
            }
            else
            {
                var xmlLayout = layout as XmlLayout;
                layoutManager.SetLayoutByXML(xmlLayout.Xml);
            }

            ComplementCells();
        }

        private void ComplementCells()
        {
            var rootCell = _viewerControl.LayoutManager.RootCell;
            var deltaCellCnt = rootCell.DisplayCapacity - _viewerControl.CellCount;
            ComplementCells(deltaCellCnt);
        }

        private void RegisterCellMouseEvent()
        {
            foreach (var control in _viewerControl.Cells.Select(cell => cell.Control))
            {
                Debug.Assert(control != null);
                control.MouseDown -= ControlCellOnMouseDown;
                control.MouseDown += ControlCellOnMouseDown;
            }
        }

        private void ControlCellOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            //Console.WriteLine("ImageCell clicked");
            var control = sender as MedViewerControlCellImpl;
            Debug.Assert(control != null);
            var controlCell = control.DataSource as ControlCell;
            Debug.Assert(controlCell != null);
            controlCell.OnClicked(mouseButtonEventArgs);
        }

        private void Refresh(IList<ImageCell> cells)
        {
            var controlCells = _viewerControl.Cells.ToList();
            for (var i = 0; i < controlCells.Count; i++)
            {
                var controlCell = controlCells[i] as ControlCell;
                Debug.Assert(controlCell != null);

                controlCell.ImageCell = cells[i];
            }
        }

        private void AddCells(int deltaCellCnt)
        {
            var newCells = new List<MedViewerControlCell>();
            for (var i = 0; i < deltaCellCnt; i++)
            {
                newCells.Add(new ControlCell());
            }
            _viewerControl.AddCells(newCells);
        }

        #endregion [--Private Viewer Control Related Function--]
    }

    public class ControlCell : MedViewerControlCell
    {
        private ImageCell _imageCell;

        public ControlCell()
        {
            _imageCell = CellFactory.Instance.CreateCell();
        }

        public ImageCell ImageCell
        {
            set
            {
                if (_imageCell.Equals(value)) return;
                _imageCell.SelectedChanged -= ImageCellOnSelectedChanged;
                _imageCell.FocusedChanged -= ImageCellOnFocusedChanged;
                _imageCell = value;
                _imageCell.SelectedChanged += ImageCellOnSelectedChanged;
                _imageCell.FocusedChanged += ImageCellOnFocusedChanged;
                Refresh();
            }
        }

        private void ImageCellOnFocusedChanged(object sender, BoolEventArgs boolEventArgs)
        {
        }

        private void ImageCellOnSelectedChanged(object sender, BoolEventArgs boolEventArgs)
        {
            IsSelected = boolEventArgs.Bool;
        }

        private void Refresh()
        {
            IsSelected = _imageCell.IsSelected;

            var displayData = _imageCell.DisplayData;
            if (displayData == null)
            {
                if (IsEmpty) return;
                Image.Clear();
            }
            else
            {
                if (IsEmpty) Image.AddPage(displayData);
                else if (Image.CurrentPage == displayData) return;
                else Image.ReplacePage(displayData, 0);
            }
            base.Refresh();
        }

        public void OnClicked(MouseButtonEventArgs mouseButtonEventArgs)
        {
            var clickStatus = new ClickStatus(mouseButtonEventArgs.LeftButton == MouseButtonState.Pressed,
                mouseButtonEventArgs.RightButton == MouseButtonState.Pressed);
            _imageCell.Click(clickStatus);
        }
    }

    public class SelectedStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isSelected = value is bool && (bool) value;
            return isSelected ? Brushes.Aqua : Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}