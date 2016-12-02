using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UIH.Mcsf.Filming.Card.Model;
using UIH.Mcsf.Filming.Widgets;
using UIH.Mcsf.Utility;

namespace UIH.Mcsf.Filming.Card.View
{
    /// <summary>
    /// Interaction logic for FilmGrid.xaml
    /// </summary>
    public partial class FilmGrid
    {
        public FilmGrid()
        {
            InitializeComponent();

            //BindingBase displayModeBinding = new Binding("SelectedDisplayMode") {Source = this};
            //BindingOperations.SetBinding(this, DisplayModeProperty, displayModeBinding);
            //_cardModel = cardModel;

            //_cardModel.DisplayModeChanged += CardModelOnDisplayModeChanged;
        }


        #region [--DependencyProperties--]

        #region [--DisplayMode DependencyProperty--]

        public int DisplayMode
        {
            get { return (int)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(int), typeof(FilmGrid), new PropertyMetadata(GlobalDefinition.DefaultDisplayMode, DisplayModePropertyValueChangedCallBack), DisplayModePropertyValidateValueCallBack);

        private static bool DisplayModePropertyValidateValueCallBack(object value)
        {
            
            if (value is int && GlobalDefinition.DisplayModes.Contains((int)value))
                return true;

            var log = "Invalidate DisplayMode in Film Grid : " + value;
            Logger.Instance.LogDevWarning(log);
            DebugHelper.Trace(TraceLevel.Warning, log);

            return false;
        }

        private static void DisplayModePropertyValueChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
            Logger.LogInfo("Film Grid DisplayMode Changed, oldValue = " + e.OldValue + ", newValue = " + newValue);
            int displayMode = 1;
            if (!int.TryParse(newValue.ToString(), out displayMode))
            {
                var log = "Change to invalidate displayMode: " + newValue + ", ignored";
                Logger.Instance.LogDevWarning(log);
                DebugHelper.Trace(TraceLevel.Warning, log);
                return;
            }
            var filmGrid = d as FilmGrid;
            if (filmGrid == null)
            {
                var log = "DisplayModeProperty not binding to filmGrid, but " + d;
                Logger.Instance.LogDevWarning(log);
                DebugHelper.Trace(TraceLevel.Warning, log);
                return;
            }

            filmGrid.SetDisplayMode(displayMode);
        }

        #endregion //[--DisplayMode DependencyProperty--]


        #region [--BoardCursor DependencyProperty--]

        public int BoardCursor
        {
            get { return (int)GetValue(BoardCursorProperty); }
            set { SetValue(BoardCursorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BoardCursor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoardCursorProperty =
            DependencyProperty.Register("BoardCursor", typeof(int), typeof(FilmGrid), new UIPropertyMetadata(0, BoardCursorPropertyChangedCallback));

        private static void BoardCursorPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            //todo:change film board
            Logger.Instance.LogSvcInfo("Goto Film Board " + dependencyObject);
        }

        #endregion //[--BoardCursor DependencyProperty--]



        #region [--BoardEnd DependencyProperty--]

        public int BoardEnd
        {
            get { return (int)GetValue(BoardEndProperty); }
            set { SetValue(BoardEndProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BoardEnd.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoardEndProperty =
            DependencyProperty.Register("BoardEnd", typeof(int), typeof(FilmGrid), new UIPropertyMetadata(0));

        #endregion //[--BoardEnd DependencyProperty--]




        #region [--FocusedFilmCursor DependencyProperty--]

        public int FocusedFilmCursor
        {
            get { return (int)GetValue(FocusedFilmCursorProperty); }
            set { SetValue(FocusedFilmCursorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FocusedFilmCursor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FocusedFilmCursorProperty =
            DependencyProperty.Register("FocusedFilmCursor", typeof(int), typeof(FilmGrid), new UIPropertyMetadata(0, FocusedFilmCursorPropertyChangedCallback));

        private static void FocusedFilmCursorPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            Logger.LogInfo("Focused Film page Index Changed, From " + dependencyPropertyChangedEventArgs.OldValue
                + " to " + dependencyPropertyChangedEventArgs.NewValue);
        }

        #endregion //[--FocusedFilmCursor DependencyProperty--]




        #endregion //[--DependencyProperties--]

        

        private void SetDisplayMode(int displayMode)
        {
            int row = 1;
            int column = 1;
            switch (displayMode)
            {
                case 1:
                    row = 1;
                    column = 1;
                    break;
                case 2:
                    row = 1;
                    column = 2;
                    break;
                case 3:
                    row = 1;
                    column = 3;
                    break;
                case 4:
                    row = 2;
                    column = 2;
                    break;
                case 6:
                    row = 2;
                    column = 3;
                    break;
                case 8:
                    row = 2;
                    column = 4;
                    break;
            }

            SetDisplayMode(row, column);
        }

        private void SetDisplayMode(int row, int column)
        {
            var filmGridChildren = filmPlateGrid.Children;
            filmGridChildren.Clear();
            filmPlateGrid.RowDefinitions.Clear();
            filmPlateGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < row; i++)
                filmPlateGrid.RowDefinitions.Add(new RowDefinition());
            for (int j = 0; j < column; j++)
                filmPlateGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int r = 0, plateIndex = 0; r < row; r++)
            {
                for (int c = 0; c < column; c++)
                {
                    var plate = _filmPlates[plateIndex];
                    plateIndex++;
                    Grid.SetRow(plate, r);
                    Grid.SetColumn(plate, c);
                    filmGridChildren.Add(plate);
                }
            }

        }


        private IList<FilmPlate> _filmPlates = Enumerable.Repeat(0, GlobalDefinition.MaxDisplayMode).Select(i => new FilmPlate()).ToList();
       
    }
}
