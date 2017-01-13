using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

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

        IList<ContentControl> BoardCells = new List<ContentControl>();

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


        }
    }
}
