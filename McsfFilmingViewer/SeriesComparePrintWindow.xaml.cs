using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UIH.Mcsf.Controls;
using UIH.Mcsf.Filming.Command;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.MHC;
using UIH.Mcsf.Utility;

namespace UIH.Mcsf.Filming
{
    public class SeriesCompareSettingChangedEventArgs : EventArgs
    {
        public SeriesCompareSettingChangedEventArgs(int imageCount, bool isVertical, int rows, int columns)
        {
            ImageCount = imageCount;
            IsVertical = isVertical;
            Rows = rows;
            Columns = columns;
        }

        public bool IsVertical { get; private set; }
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public int ImageCount { get; private set; }
    }

    /// <summary>
    /// Interaction logic for SeriesComparePrintWindow.xaml
    /// </summary>
    public partial class SeriesComparePrintWindow : UserControl
    {
        public delegate void SeriesCompareSettingChangedDelegate(object sender, SeriesCompareSettingChangedEventArgs args);
        public event SeriesCompareSettingChangedDelegate SeriesCompareSettingChangedHandler;

        public void OnSeriesCompareSettingChangedHandler()
        {
            var handler = SeriesCompareSettingChangedHandler;
            if (handler != null)
                handler(this, new SeriesCompareSettingChangedEventArgs(ImageCount, IsVertical, CellRows, CellColumns));
        }

        //private const string OrientNodeName = "MultiSeriesCompareOrientIndex";
        //private const string RowNodeName = "MultiSeriesCompareRowIndex";
        //private const string ColNodeName = "MultiSeriesCompareColIndex";

        //private const int CellCount = 3;
        //private const int MinCount = 4;
        //private const int MaxCount = 9;

        #region properties
        private List<int> _itemList1;
        private List<int> ItemList1
        {
            get
            {
                //if (_itemList1 == null)
                //{
                //    _itemList1 = new List<int>();
                //    for (int i = 1; i <= CellCount; i++)
                //    {
                //        _itemList1.Add(i * SeriesCount);
                //    }
                //}

                return _itemList1;
            }
        }

        private List<int> _itemList2;
        private List<int> ItemList2
        {
            get
            {
                //if (_itemList2 == null)
                //{
                //    _itemList2 = new List<int>();
                //    for (int count = MinCount; count <= MaxCount; count++)
                //    {
                //        _itemList2.Add(count);
                //    }
                //}

                return _itemList2;
            }
        }

        private int _seriesCount = 3;   // default is 3
        public int SeriesCount
        {
            get { return _seriesCount; }
            set
            {
                _seriesCount = value;

                // updated cell count combobox
                _itemList1 = null;
                UpdateCellCountComboBox();

                Series3Grid.Visibility = (_seriesCount == 3) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private int _series1ImageCount;
        public int Series1ImageCount
        { 
            get { return _series1ImageCount; }
            set
            {
                _series1ImageCount = value;
                UpdatedSeries1ImageCountLabel(_series1ImageCount);
            }
        }

        private int _series2ImageCount;
        public int Series2ImageCount
        {
            get { return _series2ImageCount; }
            set
            {
                _series2ImageCount = value;
                UpdatedSeries2ImageCountLabel(_series2ImageCount);
            }
        }

        private int _series3ImageCount = 0;
        public int Series3ImageCount
        {
            get { return _series3ImageCount; }
            set
            {
                _series3ImageCount = value;
                UpdatedSeries3ImageCountLabel(_series3ImageCount);
            }
        }

        private string _series1Modality;
        public string Series1Modality
        {
            get { return _series1Modality; }
            set
            {
                _series1Modality = value;
                UpdatedSeries1ModalityLabel(value);
            }
        }

        private string _series2Modality;
        public string Series2Modality
        {
            get { return _series2Modality; }
            set
            {
                _series2Modality = value;
                UpdatedSeries2ModalityLabel(value);
            }
        }

        private string _series3Modality;
        public string Series3Modality
        {
            get { return _series3Modality; }
            set
            {
                _series3Modality = value;
                UpdatedSeries3ModalityLabel(value);
            }
        }

        private int ImageCount { get; set; }
        private int CellRows { get; set; }
        private int CellColumns { get; set; }
        private bool IsVertical { get; set; }
        private int CellRowsIndex { get; set; }
        private int CellColumnsIndex { get; set; }



        #endregion

        public SeriesComparePrintWindow()
        {
            InitializeComponent();

            if (FilmingViewerContainee.FilmingResourceDict != null)
            {
                Resources.MergedDictionaries.Add(FilmingViewerContainee.FilmingResourceDict);
            }

            InitUI();
        }

        //private void OnClosingWindow(object sender, CancelEventArgs e)
        //{
        //    e.Cancel = true;
        //    Hide();
        //}

        private void OnApplyButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                WindowHostManagerWrapper.IsQuittingJob = null;

                if (Series1ImageCount + Series2ImageCount + Series3ImageCount > FilmingUtility.COUNT_OF_IMAGES_WARNING_THRESHOLD_ONCE_LOADED)
                {
                    MessageBoxHandler.Instance.ShowQuestion("UID_Filming_Warning_LoadingImagesWhichMayBeSlowForCountExceedThreshold", new MsgResponseHander(LoadImageThresholdHandler));
                }
                else
                {
                    LoadImageThresholdHandler();
                } 
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        private void LoadImageThresholdHandler(MessageBoxResponse response = MessageBoxResponse.YES)
        {
            try
            {
                Logger.LogFuncUp();

                if (response != MessageBoxResponse.YES) return;

                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                if (filmingCard == null) return;
                if (filmingCard.FilmingCardModality == FilmingUtility.EFilmModality)//If FilmingCard Modality is "EFilm", ie. electronic films have been loaded, then, before loading images, we need to clear images
                {
                    MessageBoxHandler.Instance.ShowQuestion("UID_Filming_Load_Different_Films_Need_To_Clear_Old_Films", new MsgResponseHander(ClearAndLoadImageHandler));
                }
                else
                {
                    ClearAndLoadImageHandler(MessageBoxResponse.OK);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        private void ClearAndLoadImageHandler(MessageBoxResponse response = MessageBoxResponse.YES)
        {
            try
            {
                Logger.LogFuncUp();
                if (response == MessageBoxResponse.NO) return;

                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                if (filmingCard == null) return;
                if (response == MessageBoxResponse.YES) filmingCard.DeleteAllFilmPage();
                var filmingPageControl = filmingCard.EntityFilmingPageList.LastOrDefault();
                if (filmingPageControl != null && filmingPageControl.IsAnyImageLoaded())
                {
                    MessageBoxHandler.Instance.ShowInfo("UID_Filming_Warning_NewFilmPageForLoadSeries", LoadSeriesCompareHandler);
                }
                //else
                //{
                    LoadSeriesCompareHandler();
                //}  
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        private void LoadSeriesCompareHandler(MessageBoxResponse response = MessageBoxResponse.OK)
        {
            try
            {
                Logger.LogFuncUp();

                if (response != MessageBoxResponse.OK)  return;

                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                if (filmingCard == null) return;

                WindowHostManagerWrapper.IsQuittingJob = false;
                //WindowHostManagerWrapper.CloseSecondaryWindow();
                this.CloseParentDialog();
                OnSeriesCompareSettingChangedHandler();

                //FilmingHelper.WriteMultiSeriesCompareConfigByNode(RowNodeName, CellRowsIndex);
                //FilmingHelper.WriteMultiSeriesCompareConfigByNode(ColNodeName, CellColumnsIndex);
                //FilmingHelper.WriteMultiSeriesCompareConfigByNode(OrientNodeName, IsVertical ? 0 : 1);
                Printers.Instance.MultiSeriesCompareOrientIndex = IsVertical ? 0 : 1;
                Printers.Instance.MultiSeriesCompareColIndex = CellColumnsIndex;
                Printers.Instance.MultiSeriesCompareRowIndex = CellRowsIndex;

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        public void OnCancelButtonClick(object sender, EventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                //Hide();
                WindowHostManagerWrapper.IsQuittingJob = true;
                FilmingViewerContainee.Main.OnExitSecondaryUI();
                this.CloseParentDialog();
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        private void OnCellRowsChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                int index = comboBox.SelectedIndex;
                if (index >= 0)
                {
                    CellRows = ItemList1[index];
                    CellRowsIndex = index;
                    UpdatedFilmNumberLabel();
                }
            }
        }

        private void OnCellColumnsChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                int index = comboBox.SelectedIndex;
                if (index >= 0)
                {
                    CellColumns = ItemList2[index];
                    CellColumnsIndex = index;
                    UpdatedFilmNumberLabel();
                }
            }
        }

        private void OnLayoutChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CellColumnsComboBox == null || CellRowsComboBox == null)
            {
                return;
            }

            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                int index = comboBox.SelectedIndex;
                IsVertical = (index == 0);

                UpdateCellCountComboBox();
            }
        }

        #region update UI

        public void InitUI()
        {
            Series1ImageCount = 0;
            Series2ImageCount = 0;
            Series3ImageCount = 0;
            Series1Modality = "";
            Series2Modality = "";
            Series3Modality = "";

            var orientItemsSource = new List<string>();
            orientItemsSource.Add(Resources["UID_Filming_Vertical"] as string);
            orientItemsSource.Add(Resources["UID_Filming_Horizontal"] as string);
            LayoutComboBox.ItemsSource = orientItemsSource;

            LayoutComboBox.SelectedIndex = Printers.Instance.MultiSeriesCompareOrientIndex;//FilmingHelper.GetMultiSeriesCompareConfigByNode(OrientNodeName);

            IsVertical = (LayoutComboBox.SelectedIndex == 0);

            UpdateCellCountComboBox();

            UpdatedFilmNumberLabel();
        }

        private void UpdateCellCountComboBox()
        {
            //if (IsVertical)
            //{
            //    CellColumnsComboBox.ItemsSource = ItemList1;
            //    CellRowsComboBox.ItemsSource = ItemList2;
            //}
            //else
            //{
            //    CellRowsComboBox.ItemsSource = ItemList1;
            //    CellColumnsComboBox.ItemsSource = ItemList2;
            //}

            FilmingUtility.GenerateRowsAndColsForSeriesCompare(IsVertical, SeriesCount,
                                                               out _itemList1,
                                                               out _itemList2);
            CellRowsComboBox.ItemsSource = ItemList1;
            CellColumnsComboBox.ItemsSource = ItemList2;

            CellRowsComboBox.SelectedIndex = Math.Min(ItemList1.Count - 1, Printers.Instance.MultiSeriesCompareRowIndex /*FilmingHelper.GetMultiSeriesCompareConfigByNode(RowNodeName)*/);
            CellColumnsComboBox.SelectedIndex = Math.Min(ItemList2.Count - 1, Printers.Instance.MultiSeriesCompareColIndex/*FilmingHelper.GetMultiSeriesCompareConfigByNode(ColNodeName)*/);
        }

        private void UpdatedFilmNumberLabel()
        {
            if (CellColumns > 0 && CellRows > 0)
            {
                ImageCount = Math.Min(Series1ImageCount, Series2ImageCount);
                if (SeriesCount == 3)
                {
                    ImageCount = Math.Min(Series3ImageCount, ImageCount);
                }
                int totalImageCount = ImageCount * SeriesCount;

                var filmCount = (int)Math.Ceiling(totalImageCount * 1.0 / (CellColumns * CellRows));

                FilmNumberLabel.Content = filmCount.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void UpdatedSeries1ImageCountLabel(int imageCount)
        {
            Series1ImageCountLabel.Content = imageCount.ToString(CultureInfo.InvariantCulture);
            UpdatedFilmNumberLabel();
        }

        private void UpdatedSeries1ModalityLabel(string modality)
        {
            Series1ModalityLabel.Content = modality;
        }

        private void UpdatedSeries2ImageCountLabel(int imageCount)
        {
            Series2ImageCountLabel.Content = imageCount.ToString(CultureInfo.InvariantCulture);
            UpdatedFilmNumberLabel();
        }

        private void UpdatedSeries2ModalityLabel(string modality)
        {
            Series2ModalityLabel.Content = modality;
        }

        private void UpdatedSeries3ImageCountLabel(int imageCount)
        {
            Series3ImageCountLabel.Content = imageCount.ToString(CultureInfo.InvariantCulture);
            UpdatedFilmNumberLabel();
        }

        private void UpdatedSeries3ModalityLabel(string modality)
        {
            Series3ModalityLabel.Content = modality;
        }

        public void UpdateSeries1Description(string seriesDescription)
        {
            string original = Series1Label.Text.Split(':').FirstOrDefault();
            Series1Label.Text = original + ": " + seriesDescription;
            Series1Label.ToolTip = Series1Label.Text;
        }

        public void UpdateSeries2Description(string seriesDescription)
        {
            string original = Series2Label.Text.Split(':').FirstOrDefault();
            Series2Label.Text = original + ": " + seriesDescription;
            Series2Label.ToolTip = Series2Label.Text;
        }

        public void UpdateSeries3Description(string seriesDescription)
        {
            string original = Series3Label.Text.Split(':').FirstOrDefault();
            Series3Label.Text = original + ": " + seriesDescription;
            Series3Label.ToolTip = Series3Label.Text;
        }

        #endregion

    }
}
