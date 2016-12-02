using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

using UIH.Mcsf.AppControls.Viewer;

namespace UIH.Mcsf.Filming
{
    //view model
    public class SingleSeriesComparePrintViewModel : INotifyPropertyChanged
    {

        private SeriesTreeViewItemModel _series;

        public SingleSeriesComparePrintViewModel(SeriesTreeViewItemModel series)
        {
            _series = series;

            Initialize();
        }

        private void Initialize()
        {
            try
            {

                if (WindowLevels.Count <= 1 )
                {
                    Logger.LogWarning("not enough window level for user to select");
                }
                Window1Index = 0;
                Window2Index = Window1Index + 1;

                //compare style
                CurrentCompareStyle = (int)CompareStyleEnum.Vertical;

                //ww/wl constrait SSFS KEY 101999 2014-03-13 15:49 
                //var seriesDBData = _series.SeriesDBData;
                //if(seriesDBData == null) return;
                //var modality = seriesDBData.Modality.ToUpper();
                //if (modality == "CT" || modality == "MR")
                //{
                //    var images = seriesDBData.GetImageBaseList();
                //    if (images == null || images.Count==0) return;
                //    var image = images.FirstOrDefault();
                //    if (image == null) return; 
                //    MaxWindowWidth = 1 << image.BitsStored;
                //    MaxWindowLevel = MaxWindowWidth - 1;
                //    MinWindowWidth = 1;
                //    MinWindowLevel = 0;
                //}
                MaxWindowWidth = 9727;
                MaxWindowLevel = 8191;
                MinWindowWidth = 1;
                MinWindowLevel = -1536;

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

#region [--SeriesInfo--]

        public string SeriesUID
        {
            get { return _series.SeriesInstanceUID; }
        }

        public string SeriesDescription
        {
            get { return _series.SeriesDescription; }
        }

        public int ImageCount
        {
            get { return _series.ImageCount; }
        }

        public string ImageUnit
        {
            get { return ImageCount >= 1 ? "images" : "image"; }
        }

#endregion [--SeriesInfo--]

#region [--Winding--]

        //private IDictionary<string, FilmingWindowLevel> _windowLevelDictionary = Printers.Instance.WindowLevelDictionary;

        //winding collection
        public IList<FilmingWindowLevel> WindowLevels
        {
            get {return Printers.Instance.WindowLevels;}
        }

        private int _window1Index;
        public int Window1Index
        {
            get { return _window1Index; }
            set
            {
                _window1Index = (value+WindowLevels.Count) % WindowLevels.Count; //avoid that value is negative
                CurrentWindow1 = WindowLevels[_window1Index];
                if (Window1Index == Window2Index)
                {
                    Window2Index = Window1Index + 1;
                }
                OnPropertyChanged(new PropertyChangedEventArgs("Window1Index"));
            }
        }

        private int _window2Index;
        public int Window2Index
        {
            get { return _window2Index; }
            set
            {
                _window2Index = value % WindowLevels.Count;
                CurrentWindow2 = WindowLevels[_window2Index];
                if (Window2Index == Window1Index)
                {
                    Window1Index = Window2Index - 1;
                }
                OnPropertyChanged(new PropertyChangedEventArgs("Window2Index"));
            }
        }

        private FilmingWindowLevel _currentWindow1;
        public FilmingWindowLevel CurrentWindow1
        {
            get { return _currentWindow1; }
            set 
            { 
                _currentWindow1 = value;
                Window1Width = value.Width;
                Window1Center = value.Center;
            }
        }

        private FilmingWindowLevel _currentWindow2;
        public FilmingWindowLevel CurrentWindow2
        {
            get { return _currentWindow2; }
            set 
            { 
                _currentWindow2 = value;
                Window2Width = value.Width;
                Window2Center = value.Center;
            }
        }

        //public ObservableCollection<Winding> WindingList
        //{
        //    get; set;
        //}

        private double _window1Width;
        public double Window1Width
        {
            get {return _window1Width;}
            set 
            {
                _window1Width = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Window1Width"));
            }
        }

        private double _window1Center;
        public double Window1Center
        {
            get { return _window1Center; }
            set
            {
                _window1Center = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Window1Center"));
            }
        }

        private double _window2Width;
        public double Window2Width
        {
            get { return _window2Width; }
            set
            {
                _window2Width = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Window2Width"));
            }
        }

        private double _window2Center;
        public double Window2Center
        {
            get { return _window2Center; }
            set
            {
                _window2Center = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Window2Center"));
            }
        }

        private double _maxWindowWidth = double.MaxValue;

        public Double MaxWindowWidth
        {
            get { return _maxWindowWidth; }
            set 
            {
                _maxWindowWidth = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MaxWindowWidth"));
            }
        }

        private double _maxWindowLevel = double.MaxValue;
        public double MaxWindowLevel
        {
            get { return _maxWindowLevel; }
            set
            {
                _maxWindowLevel = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MaxWindowLevel"));
            }
        }

        private double _minWindowWidth = double.MinValue;

        public Double MinWindowWidth
        {
            get { return _minWindowWidth; }
            set
            {
                _minWindowWidth = value;
                OnPropertyChanged(new PropertyChangedEventArgs("minWindowWidth"));
            }
        }

        private double _minWindowLevel = double.MinValue;
        public double MinWindowLevel
        {
            get { return _minWindowLevel; }
            set
            {
                _minWindowLevel = value;
                OnPropertyChanged(new PropertyChangedEventArgs("minWindowLevel"));
            }
        }

#endregion [--Winding--]


#region [--Layouts--]

        private Layouts _currentLayouts;
        public Layouts CurrentLayouts
        {
            get { return _currentLayouts; }
            set
            {
                _currentLayouts = value;
                Rows = _currentLayouts.Rows;
                Columns = _currentLayouts.Columns;
            }
        }

        private int _currentCompareStyle;
        public int CurrentCompareStyle
        {
            get { return _currentCompareStyle; }
            set
            {
                _currentCompareStyle = value;
                CurrentLayouts = new Layouts { CompareStyle = (CompareStyleEnum)(value) };
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentCompareStyle"));
            }
        }

        //public IList<CompareStyleEnum> CompareStyleList
        //{
        //    get
        //    {
        //        return new List<CompareStyleEnum> { CompareStyleEnum.Horizontal, CompareStyleEnum.Vertical };
        //    }
        //}

        private uint _currentRow=1;
        public uint CurrentRow
        {
            get { return _currentRow; }
            set
            {
                _currentRow = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentRow"));
                FilmCount = RefreshFilmCount();
            }
        }

        private int RefreshFilmCount()
        {
            return (int)Math.Ceiling((0.0 + ImageCount) * 2 / (CurrentColumn * CurrentRow));
        }

        private IList<uint> _rows;
        public IList<uint> Rows
        {
            get { return _rows; }
            set
            {
                try
                {
                    _rows = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Rows"));
                    CurrentRow = _rows[_rows.Count / 2];
                }
                catch (Exception ex)
                {
                    Logger.LogFuncException(ex.Message+ex.StackTrace);
                }
            }
        }

        private uint _currentColumn=1;
        public uint CurrentColumn
        {
            get { return _currentColumn; }
            set
            {
                _currentColumn = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentColumn"));
                FilmCount = RefreshFilmCount();
            }
        }

        private IList<uint> _columns;
        public IList<uint> Columns
        {
            get { return _columns; }
            set
            {
                try
                {
                    _columns = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Columns"));
                    CurrentColumn = _columns[_columns.Count / 2];
                }
                catch (Exception ex)
                {
                    Logger.LogFuncException(ex.Message+ex.StackTrace);
                }
            }
        }

#endregion [--Layouts--]

        private int _filmCount = 1;
        public int FilmCount
        {
            get { return _filmCount; }
            set 
            {
                _filmCount = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FilmCount"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    //model

    //window width & level
    public class Winding 
    {
        public string Name
        {
            get; set;
        }

        public double Width
        {
            get; set;
        }
        
        public double Level
        {
            get; set;
        }
    }

    public enum CompareStyleEnum
    {
        Vertical,
        Horizontal
    }

    //Layouts: calculated by horizon and 
    public class Layouts
    {
        private static readonly uint MAX_ROW = 10;
        private static readonly uint MAX_COL = 10;

        private CompareStyleEnum _compareStyle;
        public CompareStyleEnum CompareStyle
        {
            get
            {
                return _compareStyle;
            }
            set
            {
                _compareStyle = value;
                switch (value)
                {
                    case CompareStyleEnum.Horizontal:
                        Rows = HorizontalCompareRows;
                        Columns = HorizontalCompareColumns;
                        break;
                    case CompareStyleEnum.Vertical:
                        Rows = VerticalCompareRows;
                        Columns = VerticalCompareColumns;
                        break;
                    default:
                        Logger.LogWarning("not supported Compare type");
                        break;
                }
            }
        }        


        public IList<uint> Rows
        {
            get; set;
        }

        public IList<uint> Columns
        {
            get; set;
        }


        private IList<uint> CreateUintList(uint max, uint step)
        {
            if (step > max || 0 == step)
            {
                return null;
            }
            var result = new List<uint>();
            for (uint i = step; i <= max; i += step)
            {
                result.Add(i);
            }
            return result;
        }

        private IList<uint> _horizontalCompareRows = null;
        public IList<uint> HorizontalCompareRows
        {
            get { return _horizontalCompareRows ?? CreateUintList(MAX_ROW, 2); }
        }

        private IList<uint> _horizontalCompareColumns = null;
        public IList<uint> HorizontalCompareColumns
        {
            get { return _horizontalCompareColumns ?? CreateUintList(MAX_COL, 1); }
        }

        private IList<uint> _verticalCompareRows = null;
        public IList<uint> VerticalCompareRows
        {
            get { return _verticalCompareRows ?? CreateUintList(MAX_ROW, 1); }
        }

        private IList<uint> _verticalCompareColumns = null;
        public IList<uint> VerticalCompareColumns
        {
            get { return _verticalCompareColumns ?? CreateUintList(MAX_COL, 2); }
        }

    }

}
