using System;
using System.Linq;
using System.Windows.Media.Imaging;
using UIH.Mcsf.Utility;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Utility
{
    static class MedViewerControlCellExtension
    {
        public static bool IsGraphicSelected(this MedViewerControlCell cell)
        {
            if (cell.Image != null && cell.Image.CurrentPage != null)
            {
                var page = cell.Image.CurrentPage;
                var o = page.GetOverlay(OverlayType.Graphics);
                if (o == null)
                    return false;

                //return o.Graphics.Any(gb => gb.IsSelected);
                return o.SelectedGraphics != null && o.SelectedGraphics.Count > 0;
            }

            return false;
        }

        public static bool IsShutterSelected(this MedViewerControlCell cell)
        {
            if (cell.Image != null && cell.Image.CurrentPage != null)
            {
                var page = cell.Image.CurrentPage;
                var o = page.GetOverlay(OverlayType.MRShutter);
                if (o == null)
                    return false;

                //return o.Graphics.Any(gb => gb.IsSelected);
                return o.SelectedGraphics != null && o.SelectedGraphics.Count > 0;
            }

            return false;
        }

        private static DataAccessor dataAccessor;
        public static MedViewerControlCell Clone(this MedViewerControlCell cell)
        {
            try
            {
                var duplicatedCell = new MedViewerControlCell();
                if (cell.Image != null && cell.Image.Pages != null && cell.Image.Pages.Any())
                {
                    //fix bug 154567 2012-09-19
                    var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                    if (filmingCard != null)
                    {
                        if (dataAccessor == null)
                        {
                            dataAccessor = new DataAccessor(filmingCard.FilmingViewerConfiguration); //use filming card 's configuration, //why default configuration not use filming's
                        }

                        var sourceDisplayData = cell.Image.Pages.First();
                        var dd = dataAccessor.CopyImageDataForFilmingF1(sourceDisplayData);
                        dd.UserSpecialInfo = sourceDisplayData.UserSpecialInfo;
                        dd.Tag = sourceDisplayData.Tag.DeepClone();
                        if (null != sourceDisplayData.DicomHeader)
                        {
                            dd.DicomHeader = sourceDisplayData.DicomHeader;
                        }
                        duplicatedCell.Image.AddPage(dd);
                    }
                }

                return duplicatedCell;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
                throw;
            }
        }

        public static void ColorInverse(this MedViewerControlCell cell)
        {
            try
            {
                foreach (var dd in cell.Image.Pages)
                {
                    dd.PState.IsPaletteReversed = !dd.PState.IsPaletteReversed;
                }
                cell.Refresh();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message + ex.StackTrace);
                throw;
            }
        }

        public static void FitToWindow(this MedViewerControlCell cell)
        {
            try
            {
                if (cell.Image != null && cell.Image.Pages.Any())
                {
                    foreach (var dd in cell.Image.Pages)
                    {
                        dd.FitWindow();
                    }
                    cell.Refresh();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message + ex.StackTrace);
                throw;
            }
        }

        public static void FitToWindowByShort(this MedViewerControlCell cell)
        {
            try
            {
                if (cell.Image != null && cell.Image.Pages.Any())
                {
                    foreach (var dd in cell.Image.Pages)
                    {
                        dd.FitAndFillWindow();
                    }
                    cell.Refresh();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message + ex.StackTrace);
                throw;
            }
        }

        public static string StudyIdOfCell(this MedViewerControlCell cell)
        {
            var temp = string.Empty;
            if (cell.Image != null && cell.Image.CurrentPage != null)
            {
                var dicomHeader = cell.Image.CurrentPage.ImageHeader.DicomHeader;

                if (dicomHeader.ContainsKey(ServiceTagName.StudyInstanceUID))
                {
                    temp = dicomHeader[ServiceTagName.StudyInstanceUID];
                }
            }

            return temp;
        }
    }
}
