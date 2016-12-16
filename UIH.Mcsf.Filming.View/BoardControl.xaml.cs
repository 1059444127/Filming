using System.Windows;
using System.Windows.Controls;

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
        }

        //TODO-working-on: BoardControl.Dependency Property : DisplayMode


        public int DisplayMode
        {
            get { return (int)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(int), typeof(BoardControl));

        private int _diplayMode;


        //TODO: BoardControl.Dependency Property : Pages

        #region Overrides of FrameworkElement

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == DisplayModeProperty)
            {
                SetDisplayMode(DisplayMode);
            }
        }

        #endregion

        private void SetDisplayMode(int displayMode)
        {
            if (_diplayMode == displayMode) return;
            _diplayMode = displayMode;

            SetGrid();
        }

        private void SetGrid()
        {
            int row = _diplayMode%2==0 ? 2 : 1;
            int col = _diplayMode/row;
            SetGrid(row, col);
        }

        private void SetGrid(int row, int col)
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