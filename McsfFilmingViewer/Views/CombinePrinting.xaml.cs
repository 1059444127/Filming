using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UIH.Mcsf.Filming.Command;
using UIH.Mcsf.Filming.Model;
using UIH.Mcsf.MHC;
using UIH.Mcsf.Utility;

namespace UIH.Mcsf.Filming.Views
{
    /// <summary>
    /// Interaction logic for CombinePrinting.xaml
    /// </summary>
    public partial class CombinePrinting : UserControl
    {
        public CombinePrinting()
        {
            InitializeComponent();

            IsQuit = true;

            if (FilmingViewerContainee.FilmingResourceDict != null)
            {
                Resources.MergedDictionaries.Add(FilmingViewerContainee.FilmingResourceDict);
            }
        }
        
        public bool? IsQuit { get; set; }

        public static CombinePrinting CombinePrint = new CombinePrinting();

        private void FirstImageSpinButton_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            firstImageSpinButton.Focus();
        }

        private void LastImageSpinButton_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            lastImageSpinButton.Focus();
        }

        private void EverySpinButton_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            Every.Focus();
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                IsQuit = null;

                if (firstImageSpinButton.Text==string.Empty || lastImageSpinButton.Text == string.Empty || Every.Text == string.Empty)
                {
                    MessageBoxHandler.Instance.ShowError("UID_Filming_Warning_Form_Not_Complete");
                    return;
                }
                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                if (filmingCard == null) return;

                int imageCount = (int)ViewModel.ImageNumbers;
                if (imageCount == 0)
                {
                    CombinePrint.CloseParentDialog();
                    IsQuit = true;
                    return;
                }
                if (imageCount > FilmingUtility.COUNT_OF_IMAGES_WARNING_THRESHOLD_ONCE_LOADED)
                {
                    MessageBoxHandler.Instance.ShowQuestion("UID_Filming_Warning_LoadingImagesWhichMayBeSlowForCountExceedThreshold", new MsgResponseHander(DoBatchFilmThresholdHandler));
                }
                else
                {
                    DoBatchFilmThresholdHandler();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }

        }

        private void DoBatchFilmThresholdHandler(MessageBoxResponse response = MessageBoxResponse.YES)
        {
            try
            {
                Logger.LogFuncUp();
                if (response != MessageBoxResponse.YES) return;
               
                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                if (filmingCard == null) return;
                if (filmingCard.FilmingCardModality == FilmingUtility.EFilmModality)
                {
                    MessageBoxHandler.Instance.ShowQuestion("UID_Filming_Load_Different_Films_Need_To_Clear_Old_Films", new MsgResponseHander(DoBatchFilmHandler));
                } 
                else
                {
                    DoBatchFilmHandler(MessageBoxResponse.OK);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        private void DoBatchFilmHandler(MessageBoxResponse response = MessageBoxResponse.YES)
        {
            var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
            if (filmingCard == null) return;
            try
            {
                Logger.LogFuncUp();
                if (response == MessageBoxResponse.NO) return;
                
                if (response == MessageBoxResponse.YES)
                {
                    filmingCard.DeleteAllFilmPage();
                    filmingCard.OnAddFilmPageAfterClearFilmingCard(null, null);
                }

                IsQuit = false;

                CombinePrint.CloseParentDialog();

                filmingCard.DisableUI();
                filmingCard.layoutCtrl.DoBatchFilm();

                CreateMemento();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                filmingCard.EnableUI();
                throw;
            }
            
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            CombinePrint.CloseParentDialog();
            IsQuit = true;
        }

        private void firstImageSpinButton_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsNumberic(text))
                { 
                    e.CancelCommand(); 
                }
            }
            else 
            { 
                e.CancelCommand(); 
            }
        }

        private void firstImageSpinButton_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void firstImageSpinButton_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsNumberic(e.Text))
            {
                e.Handled = true;
            }
        }

        public bool IsNumberic(string _string)
        {
            string str = @"^\d+$";
            Regex regex = new Regex(str);
            if (regex.IsMatch(_string))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CreateMemento()
        {
            _viewModelMemento = ViewModel.CreateMemento();
        }

        public void RestoreMemento()
        {
            ViewModel.RestoreMemento(_viewModelMemento);
        }

        private CombinePrintingViewModelMemento _viewModelMemento;


        private void Every_KeyUp(object sender, KeyEventArgs e)
        {
            Every.IsHitTestVisible = false;
            Every.Focus();
            Every.IsHitTestVisible = true;
        }

        private void firstImageSpinButton_KeyUp(object sender, KeyEventArgs e)
        {
            firstImageSpinButton.IsHitTestVisible = false;
            firstImageSpinButton.Focus();
            firstImageSpinButton.IsHitTestVisible = true;
        }

        private void LastImageSpinButton_KeyUp(object sender, KeyEventArgs e)
        {
            lastImageSpinButton.IsHitTestVisible = false;
            lastImageSpinButton.Focus();
            lastImageSpinButton.IsHitTestVisible = true;
        }

        public void CheckNullParameter()
        {
            if (firstImageSpinButton.Value == null) firstImageSpinButton.Value = ViewModel.FirstImage;
            if (lastImageSpinButton.Value == null) lastImageSpinButton.Value = ViewModel.LastImage;
            if (Every.Value == null) Every.Value = ViewModel.Every;
        }
    }
}
