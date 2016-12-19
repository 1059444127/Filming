using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Filming.ViewModel;

namespace UIH.Mcsf.Filming.View
{
    /// <summary>
    ///     Interaction logic for BoardControl.xaml
    /// </summary>
    public partial class BoardControl
    {
        public BoardControl()
        {
            InitializeComponent();

            for (int i = 0; i < GlobalDefinitions.MaxDisplayMode; i++)
            {
                var pageControl = new PageControl();
                _pages.Add(pageControl);
                MainGrid.Children.Add(pageControl);
            }
        }

        //TODO-later: Page Management in BoardControl
        private List<PageControl> _pages = new List<PageControl>(); 

        //TODO-working-on: BoardControl.Dependency Property : DisplayMode


        public int DisplayMode
        {
            get { return (int)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(int), typeof(BoardControl));



        //TODO: BoardControl.Dependency Property : Pages



        public IList<PageModel> PageModels
        {
            get { return (IList<PageModel>)GetValue(PageModelsProperty); }
            set { SetValue(PageModelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageModels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageModelsProperty =
            DependencyProperty.Register("PageModels", typeof(IList<PageModel>), typeof(BoardControl));

        private int _displayMode;

        #region Overrides of FrameworkElement

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == DisplayModeProperty)
            {
                SetDisplayMode();
                return;
            }
            if (e.Property == PageModelsProperty)
            {
                FillPages();
            }
        }

        #endregion

        // TODO-working-on: BoardControl.FillPages
        private void FillPages()
        {
            int i = 0;
            while (i<PageModels.Count && i<_displayMode)
            {
                var pageControl = _pages[i];
                pageControl.Visibility = Visibility.Visible;
                var pageModel = PageModels[i];
                //TODO-later: 出于性能方面的考虑，View（BoardControl）依赖了ViewModel（PageViewModel）
                //TODO-working-on: Create Constructor PageControlViewModel(PageModel)
                pageControl.DataContext = new PageControlViewModel(pageModel);
                i++;
            }
            while (i<GlobalDefinitions.MaxDisplayMode)
            {
                var pageControl = _pages[i];
                pageControl.Visibility = Visibility.Hidden;
                i++;
            }
        }

        private void SetDisplayMode()
        {
            if (_displayMode == DisplayMode) return;
            _displayMode = DisplayMode;
            SetGrid();
        }

        private void SetGrid()
        {
            int row = _displayMode%2==0 ? 2 : 1;
            int col = _displayMode/row;
            SetGrid(row, col);
        }

        private void SetGrid(int row, int col)
        {
            CompleteGrid(row, col);

            PlacePagesToGrid(row, col);
        }

        private void PlacePagesToGrid(int row, int col)
        {
            var scale = new ScaleTransform(1.0/col, 1.0/row);
            var pageIndex = 0;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    var page = _pages[pageIndex];
                    Grid.SetRow(page, i);
                    Grid.SetColumn(page, j);
                    page.LayoutTransform = scale;
                    pageIndex++;
                }
            }
        }

        private void CompleteGrid(int row, int col)
        {
            var rows = MainGrid.RowDefinitions;
            var curRow = rows.Count;
            var cols = MainGrid.ColumnDefinitions;
            var curCol = cols.Count;

            var rowDelta = row - curRow;
            var colDelta = col - curCol;

            for (var i = 0; i < rowDelta; i++)
            {
                rows.Add(new RowDefinition());
            }
            for (var i = 0; i < colDelta; i++)
            {
                cols.Add(new ColumnDefinition());
            }
            if (rowDelta < 0) rows.RemoveRange(row, -rowDelta);
            if (colDelta < 0) cols.RemoveRange(col, -colDelta);
        }
    }
}