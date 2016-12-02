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
using UIH.Mcsf.Filming.Command;
using UIH.Mcsf.Utility;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming
{
    /// <summary>
    /// Interaction logic for CustomWindowLevel.xaml
    /// </summary>
    public partial class CustomWindowLevel : UserControl
    {
        private FilmingPageCollection _activeFilmingPageList;

        public CustomWindowLevel()
        {
            InitializeComponent();
            if (FilmingViewerContainee.FilmingResourceDict != null)
            {
                Resources.MergedDictionaries.Add(FilmingViewerContainee.FilmingResourceDict);
            }
        }

        public void InitialActiveFilmingPage(FilmingPageCollection ActiveFilmingPageList)
        {
            _activeFilmingPageList = ActiveFilmingPageList;
        }

        private void OnOKClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                if (filmingCard == null) return;

                if (CustomWLViewModel.Modality == Modality.PT)
                {
                    foreach (var filmingPage in _activeFilmingPageList)
                    {
                        foreach (var selectedCell in filmingPage.SelectedCells())
                        {
                            filmingCard.SetTBValue(selectedCell, new WindowLevel(CustomWLViewModel.CurrentCenterValue, CustomWLViewModel.CurrentWidthValue));
                        }
                    }
                }
                else if (CustomWLViewModel.Modality == Modality.CT)
                {
                    foreach (var filmingPage in _activeFilmingPageList)
                    {
                        foreach (var selectedCell in filmingPage.SelectedCells())
                        {
                            filmingCard.SetWindowLevel(selectedCell, new WindowLevel(CustomWLViewModel.CurrentCenterValue, CustomWLViewModel.CurrentWidthValue));
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.LogFuncException("Exception: " + exp.ToString());
            }
            this.CloseParentDialog();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.CloseParentDialog();
        }
    }
}
