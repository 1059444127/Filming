using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.Views
{
    /// <summary>
    /// Interaction logic for BoardControl.xaml
    /// </summary>
    public partial class BoardControl
    {
        public BoardControl()
        {
            InitializeComponent();
        }

        public IBoard Board
        {
            get { return (IBoard)GetValue(BoardProperty); }
            set { SetValue(BoardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Board.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoardProperty =
            DependencyProperty.Register("Board", typeof(IBoard), typeof(BoardControl), new PropertyMetadata(OnBoardChanged));

        private static void OnBoardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var boardControl = d as BoardControl;
            Debug.Assert(boardControl != null);

            boardControl.RegisterBoardEvent();
        }

        private void RegisterBoardEvent()
        {
            Board.CellCountChanged -= BoardOnCellCountChanged;
            Board.CellCountChanged += BoardOnCellCountChanged;
        }

        private void BoardOnCellCountChanged(object sender, EventArgs eventArgs)
        {
            var displayMode = new DisplayMode(Board.CellCount);
            
            SetGrid(displayMode.Row, displayMode.Col);
        }

        private void SetGrid(int rows, int cols)
        {
            ComplementGrid(rows, cols);
        }

        private void ComplementGrid(int row, int col)
        {
            var rows = Grid.RowDefinitions;
            var curRow = rows.Count;
            var cols = Grid.ColumnDefinitions;
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
