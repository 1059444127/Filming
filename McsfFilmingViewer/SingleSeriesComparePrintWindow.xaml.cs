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

using UIH.Mcsf.Controls;
using System.Text.RegularExpressions;
using UIH.Mcsf.AppControls.Viewer;
using UIH.Mcsf.Filming.Command;
using UIH.Mcsf.MHC;
using UIH.Mcsf.Utility;

namespace UIH.Mcsf.Filming
{
    /// <summary>
    /// Interaction logic for SingleSeriesComparePrintWindow.xaml
    /// </summary>
    public partial class SingleSeriesComparePrintWindow : UserControl
    {
        private SingleSeriesComparePrintViewModel viewModel;

        public SingleSeriesComparePrintWindow(SeriesTreeViewItemModel series)
        {
            InitializeComponent();

            IsQuit = true;

            if (FilmingViewerContainee.FilmingResourceDict != null)
            {
                Resources.MergedDictionaries.Add(FilmingViewerContainee.FilmingResourceDict);
            }

            viewModel = new SingleSeriesComparePrintViewModel(series);
            this.DataContext = viewModel;

            var orientItemsSource = new List<string>();
            orientItemsSource.Add(Resources["UID_Filming_Vertical"] as string);
            orientItemsSource.Add(Resources["UID_Filming_Horizontal"] as string);
            orientationComboBox.ItemsSource = orientItemsSource;
            orientationComboBox.SelectedIndex = 0;
        }

        public bool? IsQuit { get; set; }

        //private void OnClosingWindow(object sender, CancelEventArgs e)
        //{

        //}

        private void OnApplyButtonClick(object sender, RoutedEventArgs e)
        {
            //refer to filmingCard.Ondrop()
            try
            {
                Logger.LogFuncUp();

                IsQuit = null;

                if (widthTextBox1.Text==string.Empty || widthTextBox2.Text==string.Empty || levelTextBox1.Text==string.Empty || levelTextBox2.Text==string.Empty)
                {
                    Command.MessageBoxHandler.Instance.ShowError("UID_Filming_Warning_Form_Not_Complete");
                    return;
                }
                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                if (filmingCard == null) return;

                if (viewModel.ImageCount * 2 > FilmingUtility.COUNT_OF_IMAGES_WARNING_THRESHOLD_ONCE_LOADED)
                {
                    MessageBoxHandler.Instance.ShowQuestion("UID_Filming_Warning_LoadingImagesWhichMayBeSlowForCountExceedThreshold", LoadImageThresholdHandler);
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
                    MessageBoxHandler.Instance.ShowInfo("UID_Filming_Warning_NewFilmPageForLoadSeries", LoadSeriesHandler);
                }
                //else
                //{
                    LoadSeriesHandler();
                //}  

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        private void LoadSeriesHandler(MessageBoxResponse response = MessageBoxResponse.OK)
        {
            try
            {
                Logger.LogFuncUp();

                if (response != MessageBoxResponse.OK) return;

                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                if (filmingCard == null) return;

                IsQuit = false;
                this.CloseParentDialog();

                filmingCard.LoadSeries(viewModel);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
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
                    MessageBoxHandler.Instance.ShowQuestion("UID_Filming_Load_Different_Films_Need_To_Clear_Old_Films", ClearAndLoadImageHandler);
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


        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                IsQuit = true;
                //throw new NotImplementedException();
                this.CloseParentDialog();
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }
        #region [--UI Enable--]

        private bool IsValidFloat(string text)
        {
            Regex validFloat = new Regex(@"^-?(((0|[1-9]\d*)(\.\d*)?)|((0|[1-9]\d*)?(\.\d+)))$", RegexOptions.Compiled);
            return validFloat.IsMatch(text);
        }

        private bool IsEnableApply
        {
            get
            {
                return IsValidFloat(widthTextBox1.Text)
                    && IsValidFloat(widthTextBox2.Text)
                    && IsValidFloat(levelTextBox1.Text)
                    && IsValidFloat(levelTextBox2.Text);
            }

        }

        private void widthTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            applyButton.IsEnabled = IsEnableApply;
        }

        private void levelTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            applyButton.IsEnabled = IsEnableApply;
        }

        private void widthTextBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            applyButton.IsEnabled = IsEnableApply;
        }

        private void levelTextBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            applyButton.IsEnabled = IsEnableApply;
        }

        #endregion [--UI Enable--]

    }
}
