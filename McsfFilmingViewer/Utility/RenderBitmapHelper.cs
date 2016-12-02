using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using UIH.Mcsf.Database;
using UIH.Mcsf.Pipeline.Data;
using UIH.Mcsf.Viewer;
using Image = System.Windows.Controls.Image;

namespace UIH.Mcsf.Filming.Utility
{
    public static class RenderBitmapHelper
    {
      
        public static WriteableBitmap StitchBitmapsHorizontally(bool ifSaveImageAsGreyScale, params WriteableBitmap[] args)
        {
            try
            {
                Logger.LogFuncUp();
                if (!args.Any()) throw new Exception("no bitmap to be stitched");
                var width = args[0].PixelWidth;
                var height = args.Sum((bitmap) => bitmap.PixelHeight);
                var dpi = Printers.Instance.DefaultPaperPrintDPI;
                var stitchedBitmap = ifSaveImageAsGreyScale
                                         ? new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray8, null)
                                         : new WriteableBitmap(width, height, dpi, dpi, PixelFormats.Rgb24, null);

                unsafe
                {
                    var stitchedBytes = stitchedBitmap.BackBuffer;
                    int stitchIndex = 0;
                    foreach (var bitmap in args)
                    {
                        var bytes = bitmap.BackBuffer;
                        int length = bitmap.BackBufferStride*bitmap.PixelHeight;
                        NativeMethods.CopyMemory(stitchedBytes+stitchIndex, bytes, length);
                        stitchIndex += length;
                    }
                }
                Logger.LogFuncDown();

                return stitchedBitmap;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        public static void ShowBitmapInWnd(BitmapSource rtb)
        {
                Window cellWnd = new Window();
                Image cellImageCtrl = new Image();
                cellImageCtrl.Source = rtb;
                cellWnd.Content = cellImageCtrl;

                cellWnd.ShowDialog();
        }

        public static BitmapSource CaptureScreen(Visual target, int pixelWidth, int pixelHeight)
        {
            try
            {
                Logger.LogFuncUp();

                if (target == null || pixelWidth <= 0 || pixelHeight <= 0)
                {
                    return null;
                }

                double dpiX = 96.0 * pixelWidth / (target as FrameworkElement).ActualWidth;
                double dpiY = 96.0 * pixelHeight / (target as FrameworkElement).ActualHeight;

                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext context = drawingVisual.RenderOpen())
                {
                    VisualBrush brush = new VisualBrush(target) { Stretch = Stretch.None };
                    context.DrawRectangle(brush, null, new Rect(0, 0, (target as FrameworkElement).ActualWidth, (target as FrameworkElement).ActualHeight));
                    context.Close();
                }


                //delete this capture method(the position if the grid changed, the capture will capture the wrong area)
                RenderTargetBitmap rtb = new RenderTargetBitmap(pixelWidth, pixelHeight, dpiX, dpiY, PixelFormats.Pbgra32);
                rtb.Render(drawingVisual);
                //rtb.Render(target);

                Logger.LogFuncDown();

                return rtb;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                
            }
            return null;
        }
        public static WriteableBitmap RenderViewerControlToBitmap(Size viewerControlSize, 
                                                            FilmingCard filmingCard, 
                                                            FilmingPageControl filmingPageControl,
                                                            MedViewerControl filmingViewerControl,bool DoingForceRender = true,
                                                            bool ifSaveImageAsGreyScale = true)
        {
            try
            {
                Logger.LogFuncUp();

              
                Logger.LogFuncUp("Start DisplayFilmPage");
                filmingCard.DisplayFilmPage(filmingPageControl);
                Logger.LogFuncDown("End DisplayFilmPage");
                Logger.LogFuncUp("Start UpdateLayout");
                filmingCard.filmPageGrid.UpdateLayout();
                Logger.LogFuncDown("End UpdateLayout");
              
                MedViewerScreenSaver viewerScreenSaver = new MedViewerScreenSaver(filmingViewerControl);
                //viewerScreenSaver.IfDoingForceRender = DoingForceRender;       //设置Medview处是否ForceRender，优化1张胶片打印速度。
                Logger.LogFuncUp("Start RenderViewerControlToBitmap");
                BitmapSource viewerControlBitmap = viewerScreenSaver.RenderViewerControlToBitmap(viewerControlSize, Printers.Instance.IfPrintSplitterLine, true);
             
                Logger.LogFuncDown("End RenderViewerControlToBitmap");
                WriteableBitmap writableViewerControlBitmap = ifSaveImageAsGreyScale ?
                        new WriteableBitmap(new FormatConvertedBitmap(viewerControlBitmap, PixelFormats.Gray8, null, 0))
                    : new WriteableBitmap(new FormatConvertedBitmap(viewerControlBitmap, PixelFormats.Rgb24, null, 0));
                Logger.LogFuncDown();

                return writableViewerControlBitmap;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        public static WriteableBitmap RenderToBitmap(Size filmSize, 
                                               FilmingPageTitle pageTitle, 
                                               FilmingCard filmingCard, 
                                               FilmingPageControl filmingPageControl, 
                                               MedViewerControl medViewerControl,
                                               Grid filmingPageBarGrid,bool DoingForceRender = true,
                                               bool ifSaveImageAsGreyScale = true)
        {
            try
            {
                Logger.LogFuncUp();

                double scale = filmSize.Height > filmSize.Width 
                    ? FilmingUtility.HEADER_PERCENTAGE_OF_FILMPAGE 
                    : FilmingUtility.HEADER_PERCENTAGE_OF_FILMPAGE * filmSize.Width / filmSize.Height;

                if (filmingCard._filmingCardModality == FilmingUtility.EFilmModality
                    || pageTitle.DisplayPosition == "0") scale = 0;

                var viewerControlSize = new Size((int)filmSize.Width, (int)filmSize.Height * (1-scale));
             //   FilmingHelper.PrintTimeInfo("start render MedViewer");
                var viewerControlBitmap = RenderViewerControlToBitmap(viewerControlSize,filmingCard,filmingPageControl,medViewerControl,DoingForceRender,ifSaveImageAsGreyScale);
              //  FilmingHelper.PrintTimeInfo("End render MedViewer");
                var headerSize = new Size(viewerControlBitmap.PixelWidth, (int)filmSize.Height * scale);
             //   FilmingHelper.PrintTimeInfo("start render Header");
                WriteableBitmap headerBitmap = null;
              
                WriteableBitmap filmpageBitmap;
                if (pageTitle.DisplayPosition == "0" ) //no film page bar
                {
                    filmpageBitmap = viewerControlBitmap;
                    Logger.LogFuncDown();
                    return filmpageBitmap;
                }
               
                headerBitmap = RenderHeaderToBitmap(headerSize, filmingPageBarGrid, ifSaveImageAsGreyScale);

                if (pageTitle.DisplayPosition == "2")    //film page bar at bottom
                    filmpageBitmap = RenderBitmapHelper.StitchBitmapsHorizontally(ifSaveImageAsGreyScale, viewerControlBitmap, headerBitmap);
                else //if (DisplayPosition == "1")    //film page bar on top  
                    filmpageBitmap = RenderBitmapHelper.StitchBitmapsHorizontally(ifSaveImageAsGreyScale, headerBitmap, viewerControlBitmap);

                Logger.LogFuncDown();

                return filmpageBitmap;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        private static WriteableBitmap RenderHeaderToBitmap(Size headerSize, Grid filmPageBarGrid, bool ifSaveImageAsGreyScale = true)
        {
            try
            {
                Logger.LogFuncUp();

                var headerBitmap = CaptureScreen(filmPageBarGrid, (int)headerSize.Width, (int)headerSize.Height);

                var writeableHeaderBitmap = ifSaveImageAsGreyScale
                                                ? new WriteableBitmap(new FormatConvertedBitmap(headerBitmap,
                                                                                                PixelFormats.Gray8,
                                                                                                null, 0))
                                                : new WriteableBitmap(new FormatConvertedBitmap(headerBitmap,
                                                                                                PixelFormats.Rgb24,
                                                                                                null, 0));
                Logger.LogFuncDown();

                return writeableHeaderBitmap;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
            return null;
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

        //Refer to MedViewerCellImpl.RefreshBitmap
        [HandleProcessCorruptedStateExceptions]
        public static WriteableBitmap ConvertDisplayDataToBitmap(WriteableBitmap bitmap, DisplayData currentPage)
        {
            try
            {
                if (currentPage.SkipInterpolationForColorImage && (currentPage.SamplesPerPixel == 3 || currentPage.SamplesPerPixel == 4))
                {
                    if (currentPage.SamplesPerPixel == 3)
                    {
                        //if (bitmap.PixelWidth != currentPage.Width || bitmap.PixelHeight != currentPage.Height ||
                        //    bitmap.Format != PixelFormats.Rgb24)
                        //{
                        bitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight, 96, 96, PixelFormats.Rgb24, null);
                        //    this.ImageControl.Source = bitmap;
                        //}
                    }
                    else if (currentPage.SamplesPerPixel == 4)
                    {
                        //if (bitmap.PixelWidth != currentPage.Width || bitmap.PixelHeight != currentPage.Height ||
                        //    bitmap.Format != PixelFormats.Bgra32)
                        //{
                        bitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight, 96, 96, PixelFormats.Bgra32, null);
                        //    this.ImageControl.Source = bitmap;
                        //}
                    }

                    IntPtr imageDataPtr = currentPage.CreateSafePixelDataPtr();

                    bitmap.Lock();
                    bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
                    NativeMethods.CopyMemory(bitmap.BackBuffer, imageDataPtr, bitmap.BackBufferStride * bitmap.PixelHeight);
                    currentPage.FreeSafePixelDataPtr();
                }
                else
                {
                    //if (this.DataSource.ViewerControlSetting.UseAsynchronizedRender)
                    //{
                    //    byte[] rgbPixels = new byte[bitmap.BackBufferStride * bitmap.PixelHeight];

                    //    Task.Factory.StartNew(new Action(() => {
                    //        unsafe
                    //        {
                    //            fixed (byte* imageDataPtr = rgbPixels)
                    //            {
                    //                TransformImage(currentPage, new IntPtr(imageDataPtr));
                    //                bitmap.Dispatcher.BeginInvoke(new Action(() => {
                    //                    var rect = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
                    //                    bitmap.Lock();
                    //                    bitmap.AddDirtyRect(rect);
                    //                    bitmap.WritePixels(rect, rgbPixels, bitmap.BackBufferStride, 0);
                    //                    bitmap.Unlock();
                    //                }));
                    //            }
                    //        }
                    //    }));
                    //}
                    //else
                    //{
                    bitmap.Lock();
                    bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
                    TransformImage(currentPage, bitmap.BackBuffer);
                    //}
                }
            }
            catch (Exception exp)
            {
                MedViewerLogger.Instance.LogDevError(MedViewerLogger.Source,
                     "Exception: " + exp.ToString());
            }
            finally
            {
                bitmap.Unlock();
            }
            return bitmap;
        }

        //refer to MedViewerCellImpl.TransformImage
        [HandleProcessCorruptedStateExceptions]
        public static void TransformImage(DisplayData currentPage, IntPtr outputRGBBuffer)
        {
            try
            {
                if (outputRGBBuffer == IntPtr.Zero)
                {
                    return;
                }

                PresentationState pstate = currentPage.PState;

                int renderWidth = pstate.RenderWidth;
                int renderHeight = pstate.RenderHeight;

                int samplesPerPixel = currentPage.SamplesPerPixel;
                int width = currentPage.Width;
                int height = currentPage.Height;
                int pixelBits = currentPage.PixelBits;
                int pixelRepresentation = currentPage.PixelRepresentation;
                double rescaleSlope = currentPage.RescaleSlope;
                double rescaleIntercept = currentPage.RescaleIntercept;

                //if a image is a original PET image, its SopClassUID is 1.2.840.10008.5.1.4.1.1.128, So we use bIsOriPT to describe
                //if a image, its Modality is PT and it's not a RGB image, we use bIsNotRGBPT to describe
                bool bIsOriPT = (currentPage.SOPClassUID == "1.2.840.10008.5.1.4.1.1.128");
                bool bIsNotRGBPT = (currentPage.Modality == Modality.PT && currentPage.SamplesPerPixel == 1);
                bool bIsPseudoColor = false;
                if (bIsOriPT)
                {
                    bIsPseudoColor = !(PresentationState.PalettesEqual(pstate.Palette, PresentationState.DefaultPalette) ||
                        PresentationState.PalettesEqual(pstate.Palette, PresentationState.GetInversePalette(PresentationState.DefaultPalette)));
                }
                IntPtr imageDataPtr = currentPage.CreateSafePixelDataPtr();
                IntPtr imageMaskPtr = currentPage.CreateSafeMaskDataPtr();

                if (imageDataPtr != IntPtr.Zero)
                {
                    NativeMethods.ImageTransform(renderWidth, renderHeight,
                        0, 0, renderWidth, renderHeight, imageDataPtr, imageMaskPtr, outputRGBBuffer,
                        width, height, samplesPerPixel, pixelBits, pixelRepresentation, rescaleSlope, rescaleIntercept,
                        pstate.RotationClockwise, pstate.OffsetLeft, pstate.OffsetTop,
                        pstate.ScaleX, pstate.ScaleY, pstate.WindowLevel.WindowCenter,
                        pstate.WindowLevel.WindowWidth, pstate.EnhancePara, pstate.PaletteData,
                        (bIsOriPT && !bIsPseudoColor) ? !pstate.IsPaletteReversed : pstate.IsPaletteReversed,
                        (int)DisplayData.InterpMode.Bilinear, bIsNotRGBPT);
                }
                else
                {
                    int stride = (renderWidth * 3 % 4 == 0) ? (renderWidth * 3) : ((renderWidth * 3 / 4 + 1) * 4);
                    NativeMethods.MemSet(outputRGBBuffer, 0, (uint)(stride * renderHeight));
                }

                currentPage.FreeSafePixelDataPtr();
                currentPage.FreeSafeMaskDataPtr();
            }
            catch (Exception exp)
            {
                MedViewerLogger.Instance.LogDevError(MedViewerLogger.Source,
                     "Exception: " + exp.ToString());
            }

        }
        public static void CreateThumbnail(string filename, BitmapSource image)  //将BitmapSource装换成bmp格式的文件，调试使用。例：RenderBitmapHelper.CreateThumbnail("C:\\sss.bmp",wtb);
        {
            if (filename != string.Empty)
                using (FileStream stream = new FileStream(filename, FileMode.Create))
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(stream);
                    stream.Close();
                }
        }
    }
}
