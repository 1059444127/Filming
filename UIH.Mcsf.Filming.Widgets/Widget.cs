using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UIH.Mcsf.Viewer;


namespace UIH.Mcsf.Filming.Widgets
{
   public class Widget
    {
       public static String CreatePrintImageFullPath(int iCellIndex, int iStackIndex)
       {
           DateTime dateTime = DateTime.Now;
           string sPrintImageFullPath = Printers.Instance.PrintObjectStoragePath;
           sPrintImageFullPath += "\\"
                                                   + dateTime.Hour.ToString(CultureInfo.InvariantCulture)
                                                   + dateTime.Minute.ToString(CultureInfo.InvariantCulture)
                                                   + dateTime.Second.ToString(CultureInfo.InvariantCulture)
                                                   + dateTime.Millisecond.ToString(CultureInfo.InvariantCulture)
                                                   + iCellIndex.ToString(CultureInfo.InvariantCulture)
                                                   + iStackIndex.ToString(CultureInfo.InvariantCulture);

           return sPrintImageFullPath;
       }

       public static string GetTagValueFromCell(MedViewerControlCell cell, uint tag)
       {
           var dicomHeader = cell.Image.CurrentPage.ImageHeader.DicomHeader;
           return dicomHeader.ContainsKey(tag) ? dicomHeader[tag] : string.Empty;
       }

       public static void GetTagValue(MedViewerControlCell cell, Dictionary<uint, string> tagName2Value)
       {
           if (cell == null) return;
           var image = cell.Image; if (image == null) return;
           var displayData = image.CurrentPage; if (displayData == null) return;
           var imageHeader = displayData.ImageHeader; if (imageHeader == null) return;
           var dicomHeader = imageHeader.DicomHeader; if (dicomHeader == null) return;

           for (int i = 0; i < tagName2Value.Count; i++)
           {
               var key = tagName2Value.ElementAt(i).Key;
               tagName2Value[key] = dicomHeader.ContainsKey(key) ? dicomHeader[key] : string.Empty;
           }
       }

       public static byte[] ProcessImage(WriteableBitmap wrtBmp)
       {
           try
           {
               Logger.LogFuncUp();

               int stride = wrtBmp.PixelWidth;
               int height = wrtBmp.PixelHeight;
               byte[] data = null;

               if (wrtBmp.Format == PixelFormats.Gray8)
               {
                   stride = wrtBmp.PixelWidth;
               }
               else if (wrtBmp.Format == PixelFormats.Rgb24)
               {
                   stride = wrtBmp.PixelWidth * 3;
               }
               data = new byte[stride * height];
               if (wrtBmp.PixelWidth % 4 != 0)
               {
                   Int32Rect rect = new Int32Rect(0, 0, wrtBmp.PixelWidth, height);
                   wrtBmp.CopyPixels(rect, data, stride, 0);
               }
               else
               {
                   wrtBmp.CopyPixels(data, stride, 0);
               }

               Logger.LogFuncDown();
               return data;
           }
           catch (Exception ex)
           {
               Logger.LogFuncException(ex.Message+ex.StackTrace);
               throw;
           }
       }

        #region [--Film Size Convertor--]

       public static Size ConvertFilmSizeFrom(string filmSize, int DPI, string currentFilmOrientation)
       // Config:  <FilmSizeID>8INX10IN\10INX12IN\10INX14IN\11INX14IN\14INX14IN\14INX17IN\24CMX24CM\24CMX30CM</FilmSizeID>
       {
           try
           {
               Logger.LogFuncUp();
               //split by 'X'
               string[] sParameters = filmSize.Split('X');
               if (sParameters.Length != 2) throw new Exception(filmSize); //log: wrong string

               //remove unit
               string sWidth = sParameters[0];
               string sHeight = sParameters[1];
               double width, height;
               if (sWidth.EndsWith("IN") && sHeight.EndsWith("IN"))
               {
                   width = Convert.ToInt32(sWidth.TrimEnd('I', 'N'));
                   height = Convert.ToInt32(sHeight.TrimEnd('I', 'N'));
                   return ConvertFilmSizeFromInchSize(width, height, DPI, currentFilmOrientation);
               }
               if (sWidth.EndsWith("CM") && sHeight.EndsWith("CM")) // convert unit from cm to inch
               {
                   width = (Convert.ToInt32(sWidth.TrimEnd('C', 'M')) * 0.3937); //1cm = 0.3937inch
                   height = (Convert.ToInt32(sHeight.TrimEnd('C', 'M')) * 0.3937);
                   return ConvertFilmSizeFromInchSize(width, height, DPI, currentFilmOrientation);
               }

               Logger.LogFuncDown();

               return ConvertSizeFromPaperSize(filmSize, DPI, currentFilmOrientation);
           }
           catch (Exception ex)
           {
               Logger.LogWarning("Film Size format is wrong : (" + ex.StackTrace + ")");
               return ConvertSizeFromPaperSize(filmSize, DPI, currentFilmOrientation);
           }
       }

       private static Size ConvertFilmSizeFromInchSize(double width, 
                                                                            double height, 
                                                                            int DPI,
                                                                            string currentFilmOrientation )
       {
           //multiple with DPI
           var filmOrientation = currentFilmOrientation;
           //(FilmOrientationEnum)(PrinterSettingDialog.DataViewModal.CurrentOrientation);
           switch (filmOrientation)
           {
               case "1":
                   return new Size(height * DPI, width * DPI);
               default:
                   return new Size(width * DPI, height * DPI);
           }
       }

       private static Size ConvertFilmSizeFromCmSize(double width, double height, int DPI, string currentFilmOrientation)
       {
           return ConvertFilmSizeFromInchSize(width * 0.3937, height * 0.3937, DPI, currentFilmOrientation);//1cm = 0.3937inch
       }

       public static Size ConvertSizeFromPaperSize(string paperSize, int DPI, string currentFilmOrientation)
       {
           switch (paperSize)
           {
               case "ISOA3":
                   return ConvertFilmSizeFromCmSize(29.7, 42, DPI, currentFilmOrientation);
               default:    // "ISOA4"
                   return ConvertFilmSizeFromCmSize(21, 29.7, DPI, currentFilmOrientation);
           }
       }
        #endregion  

    }
}
