using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace UIH.Mcsf.Filming.Views
{
    /// <summary>
    /// Interaction logic for FilmingCard_PrintSetCtrl.xaml
    /// </summary>
    public partial class FilmingCardPrintSetCtrl : INotifyPropertyChanged
    {
      
        private FilmingCard Card { set; get; }

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public FilmingCardPrintSetCtrl(FilmingCard _card)
        {
            Card = _card;
            InitializeComponent();         

        }

        public PrinterSettingDataViewModal _oldDataViewModal = null;

        private PrinterSettingDataViewModal _dataViewModal = null;
        public PrinterSettingDataViewModal DataViewModal
        {
            get { return _dataViewModal; }
            set { _dataViewModal = value; }
        }

        public void CloneViewModel()
        {
            _oldDataViewModal = DataViewModal.Clone();
        }

        public void InitPrintDataViewModel()
        {
            _dataViewModal = new PrinterSettingDataViewModal();
          
            this.DataContext = DataViewModal;
            _oldDataViewModal = DataViewModal.Clone();
            InitFilmSizeComboBox();
        }

        void InitFilmSizeComboBox()
        {           
            int count=filmSizeComboBox.Items.Count;
            for(var i=0;i<count;i++ )
            {
                var item = filmSizeComboBox.Items[i];                
                if(item.ToString() == DataViewModal.CurrentFilmSize.ToString())
                {
                    filmSizeComboBox.SelectedIndex = i;
                    break;
                }
            }        
           
        }

       

        public void OnFilmingCardModalityChanged(object sender, FilmingCard.FilmingCardChangedEventArgs e)
        {
            //var filmingCard = sender as FilmingCard;
            //Debug.Assert(filmingCard != null);
            bool ret = e.ChangedModality != FilmingUtility.EFilmModality;
            DataViewModal.IsEnableAnnotationSelection = ret;
            //DataViewModal.IsEnableOrientationSelection  = ret;
            DataViewModal.IfSaveEFilmWhenFilming = ret && Printers.Instance.IfSaveEFilmWhenFilming;
            DataViewModal.IsEnableSaveEFilmSelection = ret;
            //DataViewModal.IfClearAfterAddFilmingJob     = ret && Printers.Instance.IfClearAfterAddFilmingJob;
            //DataViewModal.IsEnableClearFilmSelection    = ret;
            DataViewModal.IsEnableColorPrintSelection = ret;

            if (ret) DataViewModal.InitIsColorPrintingOptionChecked();
            else DataViewModal.IfColorPrint = IsColorEFilm(sender);
            //if (!ret) DataViewModal.IfColorPrint = IsColorEFilm(sender);

            _oldDataViewModal.IsEnableAnnotationSelection = DataViewModal.IsEnableAnnotationSelection;
            _oldDataViewModal.IsEnableOrientationSelection = DataViewModal.IsEnableOrientationSelection;
            _oldDataViewModal.IfSaveEFilmWhenFilming = DataViewModal.IfSaveEFilmWhenFilming;
            _oldDataViewModal.IsEnableSaveEFilmSelection = DataViewModal.IsEnableSaveEFilmSelection;
            _oldDataViewModal.IfClearAfterAddFilmingJob = DataViewModal.IfClearAfterAddFilmingJob;
            _oldDataViewModal.IfClearAfterSaveEFilm = DataViewModal.IfClearAfterSaveEFilm;

            _oldDataViewModal.IsEnableClearFilmSelection = DataViewModal.IsEnableClearFilmSelection;
            _oldDataViewModal.IsEnableColorPrintSelection = DataViewModal.IsEnableColorPrintSelection;
            _oldDataViewModal.IfColorPrint = DataViewModal.IfColorPrint;

            DataViewModal = _oldDataViewModal.Clone();
            this.DataContext = DataViewModal;
        }

        public bool IsColorEFilm(object sender)
        {
            try
            {
                var card = sender as FilmingCard;
                if (!card.IsCellModalitySC) return false;
                var film = card.EntityFilmingPageList.First();
                var cell = film.Cells.First();
                var displayData = cell.Image.CurrentPage;

                return displayData.SamplesPerPixel == 3;  //彩色电子胶片

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                return false;
            }
        }

        private void filmCopyCountDecimalUpDown_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void printerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void filmSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {           
            DataViewModal.SavePrintSet();
        }

        private void orientationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {           
            DataViewModal.SavePrintSet();
        }

        private void colorPrintComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            var comboBox = sender as ComboBox;
            DataViewModal.IfColorPrint = (comboBox.SelectedIndex==0?false:true);
            DataViewModal.SavePrintSet();
        }

        private void filmCopyCountDecimalUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //Console.WriteLine("filmCopyCount::" + DataViewModal.ChangePrinterAE);
            //DataViewModal.SavePrintSet();
        }

      

    }
}
