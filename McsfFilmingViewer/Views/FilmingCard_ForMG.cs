using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIH.Mcsf.Pipeline.Data;
using UIH.Mcsf.Viewer;
using UIH.Mcsf.MedDataManagement;
using UIH.Mcsf.Pipeline.Dictionary;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using UIH.Mcsf.Filming.Utility;
using System.Windows.Automation;

namespace UIH.Mcsf.Filming.Views
{
    public class FilmingCardForMG
    {
        private FilmingCard filmingCard;

        private readonly List<FilmLayout> DefaultButtonCellLayout = new List<FilmLayout>
                                                                        {
                                                                            new FilmLayout(1, 1),
                                                                            new FilmLayout(1, 2),
                                                                            new FilmLayout(2, 2)
                                                                        };
        private readonly List<FilmLayout> DefaultButtonCellLayoutForDBT = new List<FilmLayout>
                                                                        {
                                                                            new FilmLayout(1, 1),
                                                                            new FilmLayout(1, 2),
                                                                            new FilmLayout(2, 2),
                                                                            new FilmLayout(4, 4),
                                                                            new FilmLayout(5, 5)
                                                                        };
        public FilmingCardForMG(FilmingCard card)
        {
            filmingCard = card;
            ModifyUIForMG();
        }

        private void ModifyUIForMG()
        {
            try
            {
                Logger.LogFuncUp();

               // filmingCard.layoutCtrl.viewportLayoutPanel.Margin=new Thickness()
                AddMGViewLayoutBtn();
              //  InitDefaultViewportLayoutCollection();
                filmingCard.BtnEditCtrl.editButtonGrid.Children.Remove(filmingCard.BtnEditCtrl.btnCopy);
                filmingCard.BtnEditCtrl.editButtonGrid.Children.Remove(filmingCard.BtnEditCtrl.btnCut);
                filmingCard.BtnEditCtrl.editButtonGrid.Children.Remove(filmingCard.BtnEditCtrl.btnPaste);
                filmingCard.BtnEditCtrl.editButtonGrid.Children.Remove(filmingCard.BtnEditCtrl.insertOperationGrid);
                filmingCard.BtnEditCtrl.editButtonGrid.Children.Remove(filmingCard.BtnEditCtrl.btnRefImage);

                filmingCard.BtnEditCtrl.enhancementGrid.Visibility = Visibility.Collapsed;
                //this.BtnEditCtrl.btnRealSize.Visibility = Visibility.Visible;
                //filmingButtonGrid2.Children.Remove(advancedEditButtonGrid);
                filmingCard.layoutCtrl.cellLayoutGrid.Visibility = Visibility.Collapsed;
              //  filmingCard.layoutCtrl.addFilmPageButton.Margin = new Thickness(15, 0, 0, 0);
                filmingCard.BtnEditCtrl.advancedEditButtonGrid.Visibility = Visibility.Collapsed;
                filmingCard.BtnEditCtrl.editButtonGrid1.Visibility = Visibility.Collapsed;

                filmingCard.BtnEditCtrl.filmingButtonGrid2.Margin = new Thickness(7, 30, 0, 0); // +30

                filmingCard.studyTreeCtrl.studyListContextMenu.Items.Remove(filmingCard.studyTreeCtrl.batchMenuItem);
                filmingCard.studyTreeCtrl.studyListContextMenu.Items.Remove(filmingCard.studyTreeCtrl.compareMenuItem);

                filmingCard.layoutCtrl.cellLayoutGrid.Visibility = Visibility.Collapsed;

                filmingCard.layoutCtrl.filmCustomViewportLayout.Visibility = Visibility.Collapsed;
                //modify location
              //  filmingCard.layoutCtrl.displayGrid.Margin = new Thickness(0, 54, 0, 0);    //14-54 right               
                filmingCard.layoutCtrl.btnSaveFilmLayout.Visibility = Visibility.Collapsed;
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.Message + ex.StackTrace);
            }
        }

        //private void InitDefaultViewportLayoutCollection()
        //{
        //    foreach (var layout in DefaultButtonCellLayout)
        //    {
        //        //FilmLayout layout = ViewportLayoutCollection.ElementAt(0);
        //        //layout = new FilmLayout(1, 1, layout.LayoutName);
        //        layout.LayoutType = LayoutTypeEnum.RegularLayout;
        //        filmingCard.layoutCtrl.DefaultViewportLayoutCollection.Add(layout);
        //        //layout = ViewportLayoutCollection.ElementAt(1);
        //        //DefaultViewportLayoutCollection.Add(layout);
        //        //layout = ViewportLayoutCollection.ElementAt(2);
        //        //DefaultViewportLayoutCollection.Add(layout);
        //    }
          
        //}

        private void AddMGViewLayoutBtn()
        {
            filmingCard.layoutCtrl.viewportLayoutPanel.Children.Clear();
            try
            {
                var layoutList = filmingCard.IsModalityDBT() ? DefaultButtonCellLayoutForDBT : DefaultButtonCellLayout;
                int count = layoutList.Count;
                for (var i = 0; i < count; i++)
                {
                    var layout = layoutList[i] as FilmLayout;
                    var row = layout.LayoutRowsSize;
                    var col = layout.LayoutColumnsSize;
                    Button itemBtn = new Button();
                    var k = i + 1;
                    itemBtn.Name = "viewportLayoutButton" + k;
                    AutomationProperties.SetAutomationId(itemBtn, "ID_BTN_FILMING_VIEWPORTLAYOUT" + k);
                    itemBtn.Click += new RoutedEventHandler(ItemBtn_Click);
                    var btnStyle = "Style_Button_Common_CSW_Center";
                    if (i == 0)
                    {
                        btnStyle = "Style_Button_Common_CSW_Left";
                    }
                    if (i == count - 1)
                    {
                        btnStyle = "Style_Button_Common_CSW_Right";
                    }
                    var layoutName = col.ToString() + "x" + row.ToString();
                    
                    itemBtn.Style = filmingCard.FindResource(btnStyle) as Style;
                    itemBtn.HorizontalAlignment = HorizontalAlignment.Left;
                    itemBtn.VerticalAlignment = VerticalAlignment.Top;
                    var toopTip = layoutName + filmingCard.FindResource(string.Format("UID_Filming_ViewportLayout"));
                    itemBtn.ToolTip = toopTip as string;
                    
                    itemBtn.Width = 35;
                    itemBtn.Height = 35;
                    itemBtn.TabIndex = k;
                    Image img = new Image();
                    //if (layoutName == "1x2") layoutName = "2x1";
                    string imgPath = string.Format("layout{0}", layoutName);
                   
                    img.Source = filmingCard.FindResource(imgPath) as ImageSource;
                    itemBtn.Content = img;
                    filmingCard.layoutCtrl.viewportLayoutPanel.Children.Add(itemBtn);
                }
            }catch(Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void ItemBtn_Click(object sender, RoutedEventArgs e)
        {            
            try
            {
                Logger.LogFuncUp();
                var btn = sender as Button;
                var index = btn.TabIndex;
                if(filmingCard.IsModalityDBT())
                    filmingCard.layoutCtrl.ViewportLayoutActiveFilmingPages(DefaultButtonCellLayoutForDBT[index - 1]);
                else
                    filmingCard.layoutCtrl.ViewportLayoutActiveFilmingPages(DefaultButtonCellLayout[index-1]);
                filmingCard.UpdateUIStatus();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }
        
        public void SetFilmPageSizeForMg(FilmingPageControl filmingPage)
        {
            try
            {
                Logger.LogFuncUp();
                Logger.Instance.LogDevInfo("SetFilmPageSizeForMG enter::" + filmingPage.FilmPageIndex);
                
                double viewCtrlHeight = FilmingUtility.DisplayedFilmPageHeight;
                var correctedRatio = 1.00;
                if (filmingPage.PageTitle.DisplayPosition != "0")
                {
                    correctedRatio = 0.95;
                    var titleHeight = 54;
                    if (Math.Abs(filmingPage.PageTitle.DisplayFont - 15) < 0.1) titleHeight = 69;
                    if (Math.Abs(filmingPage.PageTitle.DisplayFont - 5) < 0.1) titleHeight = 36;
                    viewCtrlHeight = FilmingUtility.DisplayedFilmPageHeight - titleHeight;
                }
                else if (filmingPage.PageTitle.DisplayPosition == "0"
                         && filmingPage.PageTitle.PageNoVisibility == Visibility.Visible)
                {
                    correctedRatio = 0.97;
                    viewCtrlHeight = FilmingUtility.DisplayedFilmPageHeight - 25;
                }
                //高度顶边情况
                FilmingUtility.DisplayedFilmPageHeight = filmingCard.FilmPageCardSize.Height;
                filmingPage.Height = FilmingUtility.DisplayedFilmPageHeight;

                var tempDisplayedFilmPageWidth = viewCtrlHeight * filmingCard.Orientation2CurrentFilmSizeRatioForMg / correctedRatio;
                if (tempDisplayedFilmPageWidth <= filmingCard.FilmPageCardSize.Width)
                {
                    FilmingUtility.DisplayedFilmPageViewerHeight = viewCtrlHeight;
                    FilmingUtility.DisplayedFilmPageWidth = tempDisplayedFilmPageWidth;
                    filmingPage.Width = FilmingUtility.DisplayedFilmPageWidth;
                }
                else  //宽度顶边情况
                {
                    FilmingUtility.DisplayedFilmPageWidth = filmingCard.FilmPageCardSize.Width;
                    filmingPage.Width = FilmingUtility.DisplayedFilmPageWidth;
                    var viewerHeight = FilmingUtility.DisplayedFilmPageWidth * correctedRatio /
                                         filmingCard.Orientation2CurrentFilmSizeRatioForMg;
                    var tempDisplayedFilmPageHeight = viewerHeight + filmingPage.filmPageBarGrid.ActualHeight +
                                                  filmingPage.filmPageBarGridSimple.ActualHeight;
                    FilmingUtility.DisplayedFilmPageHeight = tempDisplayedFilmPageHeight;
                    filmingPage.Height = FilmingUtility.DisplayedFilmPageHeight;
                    FilmingUtility.DisplayedFilmPageViewerHeight = viewerHeight;
                }

                double xScale = filmingCard.FilmPageWidth / filmingPage.Width;
                double yScale = filmingCard.FilmPageHeight / filmingPage.Height;
                double scale = Math.Min(xScale, yScale);

                filmingPage.Scale.SetValue(ScaleTransform.ScaleYProperty, scale);
                filmingPage.Scale.SetValue(ScaleTransform.ScaleXProperty, scale);
                Logger.Instance.LogDevInfo("SetFilmPageSizeForMG exit::" + filmingPage.FilmPageIndex);
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void UpdateFilmPageSize()
        {
            if (filmingCard.IsModalityForMammoImage())
                filmingCard.DisplayedFilmPage.ToList().ForEach(SetFilmPageSizeForMg);
        }


        // Only MG use this method
        public unsafe void AddMask(DisplayData displayData, Sop sop)
        {
            if (sop.Modality != "MG") return;

            var header = sop.DicomSource;

            try
            {
                UInt16 padding = 0;
                UInt16 colomns = 0;
                UInt16 rows = 0;
                UInt16 bitAllocated = 0;
                UInt16 samplesPerPixel = 0;

                if (!GetUInt16(header, Pipeline.Dictionary.Tag.PixelPaddingValue, out padding))
                {
                    return;
                }

                if (!GetUInt16(header, Pipeline.Dictionary.Tag.Columns, out colomns))
                {
                    return;
                }
                if (!GetUInt16(header, Pipeline.Dictionary.Tag.Rows, out rows))
                {
                    return;
                }
                if (!GetUInt16(header, Pipeline.Dictionary.Tag.BitsAllocated, out bitAllocated))
                {
                    return;
                }
                if (!GetUInt16(header, Pipeline.Dictionary.Tag.SamplesPerPixel, out samplesPerPixel))
                {
                    return;
                }

                if (samplesPerPixel != 1) return;

                int size = colomns * rows;
                var mask = new byte[size];
                var imgSop = sop as ImageSop;
                if (imgSop == null) return;
                var pixelData = imgSop.GetNormalizedPixelData();

                fixed (byte* maskPtr = mask, pixelPtr = pixelData)
                {
                    if (bitAllocated == 16)
                    {
                        var pPixelData = (Int16*)(new IntPtr(pixelPtr));
                        for (int i = 0; i < size; i++)
                        {
                            if (pPixelData[i] == padding)
                            {
                                maskPtr[i] = 0;
                            }
                            else
                            {
                                maskPtr[i] = 255;
                            }
                        }
                    }
                }

                displayData.Mask = mask;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }

        }

        public bool GetUInt16(DicomAttributeCollection headInfo, Tag tag, out ushort value)
        {
            if (!headInfo.Contains(tag))
            {
                value = 0;
                return false;
            }
            else
            {
                DicomAttribute element = headInfo[tag];
                if (!element.GetUInt16(0, out value))
                {
                    return false;
                }
                return true;
            }

        }

       
        public void ApplyDisplayForMg(IViewerControlCell cell)
        {
            DisplayData displayData = cell.Image.CurrentPage;
            ICoordinateTransform transform = CoordinateTransformFactoryHelper.CoordinateTransformFactory.CreateCoordinateTransform(cell);
            Point point1 = new Point();
            Point point2 = new Point();
            Vector v = new Vector();
            var imageLaterality = string.Empty;
            var isLaterality = displayData.ImageHeader.DicomHeader.TryGetValue(ServiceTagName.ImageLaterality, out imageLaterality);
            if (!isLaterality) return;
            point1 = imageLaterality == "L" ? new Point(0, 0.5) : new Point(1, 0.5);

            var pos = GetPostion(cell);
            if (pos == -1) return;
            switch (pos)
            {
                case 0:
                    {
                        point2 = new Point(0, 0.5);
                        break;
                    }
                case 1:
                    {
                        point2 = new Point(0.5, 0);
                        break;
                    }
                case 2:
                    {
                        point2 = new Point(1, 0.5);
                        break;
                    }
                case 3:
                    {
                        point2 = new Point(0.5, 1);
                        break;
                    }
            }

            Point p1 = transform.TranslatePoint(point1, PointType.RatioImage, PointType.Cell);
            Point p2 = transform.TranslatePoint(point2, PointType.RatioCell, PointType.Cell);
            v = p2 - p1;
            displayData.PState.Translate(v.X, v.Y);
            cell.Refresh(CellRefreshType.Image);

        }
        private int GetPostion(IViewerControlCell cell)
        {
            int pos = 0;
            var overlay = cell.Image.CurrentPage.GetOverlay(OverlayType.FilmingF1ProcessText) as OverlayFilmingF1ProcessText;
            if (overlay != null)
            {
                string[] postions = overlay.GraphicFilmingF1ProcessText.OrientationTexts;
                for (int i = 0; i < postions.Count(); i++)
                {
                    if (postions[i].ToUpper() == "P")
                    {
                        pos = i;
                        break;
                    }
                }
                return pos;
            }
            return -1;
        }
    }
}
