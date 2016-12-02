using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using UIH.Mcsf.AppControls.Viewer;
using UIH.Mcsf.Filming.Command;
using UIH.Mcsf.Filming.Model;
using UIH.Mcsf.Filming.Widgets;
using UIH.Mcsf.MHC;
using UIH.Mcsf.Utility;
using ICommand = System.Windows.Input.ICommand;
using System.Windows.Media;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.Viewer;
using UIH.Mcsf.Controls;

namespace UIH.Mcsf.Filming.Views
{
    public class FilmingCardContextMenu:IDisposable
    {
        private FilmingCard Card;
        private CustomWindowLevel _customWindowLevel;
        public MenuItem configMenuItem;
        public MenuItem filmingContextMenuRepack;
        private MenuItem ctPresetWindowMenuItem;
        private MenuItem ptPresetWindowMenuItem;
        private MenuItem suvMenuItem;
        private MenuItem filmingContextMenuAnnotationCustomization;
        private MenuItem filmingContextMenuScaleRuler;
        private ContextMenu filmingContextMenu;
        private MenuItem filmingContextMenuDivideCell;
        private MenuItem filmingContextMenuUndoDivideCell;
        public MenuItem filmingContextMenuLocalizedImageReferenceLine;
        private MenuItem filmingContextMenuManualLocalizedImage;
        private MenuItem filmingContextMenuManualReferenceLine;
        public Dictionary<string, WindowLevelInfo> presetWindowInfos = new Dictionary<string, WindowLevelInfo>();
        private bool isModalityMG = false;

        public FilmingCardContextMenu(FilmingCard card)
        {
            Card = card;
            _customWindowLevel = new CustomWindowLevel();
            ctPresetWindowMenuItem = new MenuItem();
            ptPresetWindowMenuItem = new MenuItem();
            InitializePresetWindowInfo();
            // InitContextMenu();
        }


        public void InitContextMenu()
        {
            isModalityMG = Card.IsModalityForMammoImage();
            filmingContextMenu = new ContextMenu();
            AutomationProperties.SetAutomationId(filmingContextMenu, "ID_CTM_FILMING_CONTEXTMENU");
            filmingContextMenu.Style = Card.TryFindResource("Style_ContextMenu_CSW_CC_Default") as Style;
            filmingContextMenu.Foreground = Brushes.White;

            InitSelectMenuItems();
            InitEditMenuItems();
            InitDeleteMenuItems();

            filmingContextMenu.Items.Add(new Separator());

            if (!isModalityMG)
            {
                #region insertPageBreak

                var insertPageBreakMenuItem = new MenuItem();
                insertPageBreakMenuItem.Header = Card.TryFindResource("UID_Filming_ComboBox_Insert_PageBreak");
                insertPageBreakMenuItem.Command = Card.commands.InsertPageBreakCommand;
                AutomationProperties.SetAutomationId(insertPageBreakMenuItem, "ID_MNU_FILMING_PAGEBREAK");
                filmingContextMenu.Items.Add(insertPageBreakMenuItem);

                var deletePagebreakMenuItem = new MenuItem();
                deletePagebreakMenuItem.Header = Card.TryFindResource("UID_Filming_Delete_Pagebreak");
                deletePagebreakMenuItem.Command = Card.commands.DeletePageBreakCommand;
                AutomationProperties.SetAutomationId(deletePagebreakMenuItem, "ID_MNU_FILMING_DELETEPAGEBREAK");
                filmingContextMenu.Items.Add(deletePagebreakMenuItem);

                #endregion insertPageBreak

                InitDivideMenuItems();
            }


            var presetWindowSeparator = new Separator();
            filmingContextMenu.Items.Add(presetWindowSeparator);
            presetWindowSeparator.Visibility = Visibility.Collapsed;
            if (CTPreSetWindowingVisibility == Visibility.Visible || PTPreSetWindowingVisibility == Visibility.Visible)
            {
                presetWindowSeparator.Visibility = Visibility.Visible;
            }

            #region presetWindow

            ctPresetWindowMenuItem.Header = Card.TryFindResource("UID_Filming_PreSet_CT_Windowing");
            ctPresetWindowMenuItem.Visibility = CTPreSetWindowingVisibility;
            AutomationProperties.SetAutomationId(ctPresetWindowMenuItem, "ID_MNU_FILMING_PRESETCTWINDOWING");
            filmingContextMenu.Items.Add(ctPresetWindowMenuItem);

            // var ptPresetWindowMenuItem = new MenuItem();
            ptPresetWindowMenuItem.Header = Card.TryFindResource("UID_Filming_PreSet_PT_Windowing");
            ptPresetWindowMenuItem.Visibility = PTPreSetWindowingVisibility;
            AutomationProperties.SetAutomationId(ptPresetWindowMenuItem, "ID_MNU_FILMING_PRESETPTWINDOWING");
            filmingContextMenu.Items.Add(ptPresetWindowMenuItem);
            #endregion presetWindow

            #region suv
            suvMenuItem = new MenuItem();
            suvMenuItem.Header = Card.TryFindResource("UID_Filming_SetSUVType");
            suvMenuItem.Visibility = PreSUVTypeSettingVisibility;
            AutomationProperties.SetAutomationId(suvMenuItem, "ID_MNU_FILMING_SUVTYPESETTING");
            InitSuvMenuItems();
            filmingContextMenu.Items.Add(suvMenuItem);

            #endregion

            #region 恢复窗宽窗位
            var filmingContextMenuResetWindowing = new MenuItem();
            filmingContextMenuResetWindowing.Header =Card.TryFindResource("UID_Filming_Windowing");
            filmingContextMenuResetWindowing.Command = Card.commands.RecoverWlCommand;
            filmingContextMenuResetWindowing.InputGestureText = "F12";
            AutomationProperties.SetAutomationId(filmingContextMenuResetWindowing,
                                                 "ID_MNU_FILMING_RESTWINDOWING");
            filmingContextMenu.Items.Add(filmingContextMenuResetWindowing);
            #endregion
            if (!isModalityMG)
            {
                filmingContextMenu.Items.Add(new Separator());

                filmingContextMenuLocalizedImageReferenceLine = new MenuItem();
                filmingContextMenuLocalizedImageReferenceLine.Header =
                    Card.TryFindResource("UID_Filming_ContextMenu_LocalizedImageReferenceLine");
                filmingContextMenuLocalizedImageReferenceLine.IsCheckable = true;
                filmingContextMenuLocalizedImageReferenceLine.Checked += OnLocalizedImageReferenceLineChecked;
                filmingContextMenuLocalizedImageReferenceLine.Unchecked += OnLocalizedImageReferenceLineUnChecked;
                filmingContextMenuLocalizedImageReferenceLine.IsChecked = IsLocalizedImageReferenceLineChecked;
                filmingContextMenuLocalizedImageReferenceLine.IsEnabled = IsLocalizedImageReferenceLineEnabled;
                AutomationProperties.SetAutomationId(filmingContextMenuLocalizedImageReferenceLine,
                                                     "ID_MNU_FILMING_LOCALIZEDIMAGEREFERENCELINE");
                filmingContextMenu.Items.Add(filmingContextMenuLocalizedImageReferenceLine);

                filmingContextMenuManualLocalizedImage = new MenuItem();
                filmingContextMenuManualLocalizedImage.Header =
                    Card.TryFindResource("UID_Filming_ContextMenu_ManualLocalizedImageReferenceLine");
                filmingContextMenuManualLocalizedImage.IsEnabled = IsLocalizedImageReferenceLineEnabled;
                AutomationProperties.SetAutomationId(filmingContextMenuManualLocalizedImage,
                                                     "ID_MNU_FILMING_MANUALLOCALIZEDIMAGEREFERENCELINE");
                filmingContextMenuManualLocalizedImage.Command = Card.commands.PopMiniPaCommandAddLocalImage;
                filmingContextMenu.Items.Add(filmingContextMenuManualLocalizedImage);

                filmingContextMenuManualReferenceLine = new MenuItem();
                filmingContextMenuManualReferenceLine.Header =
                    Card.TryFindResource("UID_Filming_ContextMenu_ManualReferenceLine");
                filmingContextMenuManualReferenceLine.IsEnabled = Card.BtnEditCtrl.IsEnableInsertRefImage;
                AutomationProperties.SetAutomationId(filmingContextMenuManualReferenceLine,
                                                     "ID_MNU_FILMING_MANUALREFERENCELINE");
                filmingContextMenuManualReferenceLine.Command = Card.commands.PopMiniPaCommandInsertRefImg;
                filmingContextMenu.Items.Add(filmingContextMenuManualReferenceLine);

            }


            InitConfigMenuItems();

            if (!isModalityMG)
            {
                var filmingContextMenuPrintDisplayedSelectedFilm = new MenuItem();
                filmingContextMenuPrintDisplayedSelectedFilm.Header =
                    Card.TryFindResource("UID_Filming_ContextMenu_PrintDisplayedSelectedFilm");
                AutomationProperties.SetAutomationId(filmingContextMenuPrintDisplayedSelectedFilm,
                                                     "ID_MNU_FILMING_PRINTDISPLAYEDSELECTEDFILM");
                filmingContextMenuPrintDisplayedSelectedFilm.Command = Card.commands.PrintDisplayedSelectedFilmCommand;
                filmingContextMenu.Items.Add(filmingContextMenuPrintDisplayedSelectedFilm);
            }

            Card.filmPageGrid.ContextMenu = filmingContextMenu;
            Card.filmPageGrid.ContextMenuOpening += new ContextMenuEventHandler(filmPageGrid_ContextMenuOpening);

        }

        void filmPageGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (Card.ActiveFilmingPageList.Count == 0 && (FilmingCard._miniCellsList == null || FilmingCard._miniCellsList.Count == 0))
            {
                foreach (var page in Card.EntityFilmingPageList)
                {
                    var cells = page.filmingViewerControl.Cells;
                    foreach (var cell in cells)
                    {
                        cell.IsSelected = true;
                    }
                    Card.ActiveFilmingPageList.AddPage(page);
                }
            }
           
        }


        #region 选择菜单
        private void InitSelectMenuItems()
        {
            #region selectSeries
            var selectSeriesMenuItem = new MenuItem();
            selectSeriesMenuItem.Header = Card.TryFindResource("UID_Filming_SelectSeriesForRightMouseClick");
            selectSeriesMenuItem.Command = Card.commands.SelectSeriesForRightMouseClickCommand;
            AutomationProperties.SetAutomationId(selectSeriesMenuItem, "ID_MNU_FILMING_SELECTSERIES");
            filmingContextMenu.Items.Add(selectSeriesMenuItem);
            #endregion selectSeries

            #region selectSucceed
            var selectSucceedMenuItem = new MenuItem();
            selectSucceedMenuItem.Header = Card.TryFindResource("UID_Filming_SelectSucceed");
            selectSucceedMenuItem.Command = Card.commands.SelectSucceedCommand;
            AutomationProperties.SetAutomationId(selectSucceedMenuItem, "ID_MNU_FILMING_SELECTSUCCEED");
            filmingContextMenu.Items.Add(selectSucceedMenuItem);
            #endregion selectSucceed

            #region selectFilmPage
            var selectFilmPageMenuItem = new MenuItem();
            selectFilmPageMenuItem.Header = Card.TryFindResource("UID_Filming_SelectFilmPage");
            selectFilmPageMenuItem.Command = Card.commands.SelectFilmPageCommand;
            AutomationProperties.SetAutomationId(selectFilmPageMenuItem, "ID_MNU_FILMING_SELECTFILMPAGE");
            filmingContextMenu.Items.Add(selectFilmPageMenuItem);
            #endregion selectFilmPage

            #region selectAllFilmPage
            var selectAllFilmPageMenuItem = new MenuItem();
            selectAllFilmPageMenuItem.Header = Card.TryFindResource("UID_Filming_SelectAllFilmPage");
            selectAllFilmPageMenuItem.Command = Card.commands.SelectAllFilmPageCommand;
            AutomationProperties.SetAutomationId(selectFilmPageMenuItem, "ID_MNU_FILMING_SELECTALLFILMPAGE");
            filmingContextMenu.Items.Add(selectAllFilmPageMenuItem);
            #endregion selectAllFilmPage
        }

        #endregion


        #region 复制/剪切/粘贴菜单
        private void InitEditMenuItems()
        {
            #region copy
            var copyMenuItem = new MenuItem();
            copyMenuItem.Header = Card.TryFindResource("UID_Filming_Copy");
            copyMenuItem.Command = Card.commands.CopyCommand;
            copyMenuItem.InputGestureText = "Ctrl+C";
            AutomationProperties.SetAutomationId(copyMenuItem, "ID_MNU_FILMING_COPY");
            filmingContextMenu.Items.Add(copyMenuItem);
            #endregion copy

            #region cut
            var cutMenuItem = new MenuItem();
            cutMenuItem.Header = Card.TryFindResource("UID_Filming_Cut");
            cutMenuItem.Command = Card.commands.CutCommand;
            cutMenuItem.InputGestureText = "Ctrl+X";
            AutomationProperties.SetAutomationId(cutMenuItem, "ID_MNU_FILMING_CUT");
            filmingContextMenu.Items.Add(cutMenuItem);
            #endregion cut

            #region paste image
            var pasteMenuItem = new MenuItem();
            pasteMenuItem.Header = Card.TryFindResource("UID_Filming_Paste");
            pasteMenuItem.Command = Card.commands.PasteCommand;
            pasteMenuItem.InputGestureText = "Ctrl+V";
            AutomationProperties.SetAutomationId(pasteMenuItem, "ID_MNU_FILMING_PASTE");
            filmingContextMenu.Items.Add(pasteMenuItem);
            #endregion paste image

            //#region paste graphic
            //var pasteGraphicMenuItem = new MenuItem();
            //pasteGraphicMenuItem.Header = Card.TryFindResource("UID_Filming_PasteGraphics");
            //pasteGraphicMenuItem.Command = Card.commands.PasteGraphicCommand;
            //AutomationProperties.SetAutomationId(pasteGraphicMenuItem, "ID_MNU_FILMING_PASTE_GRAPHIC");
            //filmingContextMenu.Items.Add(pasteGraphicMenuItem);
            //#endregion paste graphic
        }
        #endregion

        #region 删除菜单项目
        private void InitDeleteMenuItems()
        {
            var deleteMenuItem = new MenuItem();
            deleteMenuItem.Header = Card.TryFindResource("UID_Filming_Delete");
            deleteMenuItem.Command = Card.commands.DeleteParentCommand;
            AutomationProperties.SetAutomationId(deleteMenuItem, "ID_MNU_FILMING_DELETE");

            var deleteImageMenuItem = new MenuItem();
            deleteImageMenuItem.Header = Card.TryFindResource("UID_Filming_Delete_Image");
            deleteImageMenuItem.Command = Card.commands.DeleteCommand;
            AutomationProperties.SetAutomationId(deleteImageMenuItem, "ID_MNU_FILMING_DELETEIMAGE");
            deleteMenuItem.Items.Add(deleteImageMenuItem);

            var deleteImageNotSelectMenuItem = new MenuItem();
            deleteImageNotSelectMenuItem.Header = Card.TryFindResource("UID_Filming_Delete_ImageNotSelected");
            //deleteImageNotSelectMenuItem.Visibility = Card.commands.IsDeleteImageNotSelectedVisible;
            deleteImageNotSelectMenuItem.Command = Card.commands.DeleteNotSelectedCommand;
            AutomationProperties.SetAutomationId(deleteImageNotSelectMenuItem,
                                                 "ID_MNU_FILMING_DELETEIMAGENOTSELECTED");
            deleteMenuItem.Items.Add(deleteImageNotSelectMenuItem);

            var deleteFilmPageMenuItem = new MenuItem();
            deleteFilmPageMenuItem.Header = Card.TryFindResource("UID_Filming_DeleteFilmPage");
            deleteFilmPageMenuItem.Command = Card.commands.DeleteFilmPageCommand;
            AutomationProperties.SetAutomationId(deleteFilmPageMenuItem, "ID_MNU_FILMING_DELETEFILMPAGE");
            deleteMenuItem.Items.Add(deleteFilmPageMenuItem);

            var deleteAllMenuItem = new MenuItem();
            deleteAllMenuItem.Header = Card.TryFindResource("UID_Filming_DeleteAll");
            deleteAllMenuItem.Command = Card.commands.DeleteAllCommand;
            AutomationProperties.SetAutomationId(deleteAllMenuItem, "ID_MNU_FILMING_DELETEALL");
            //filmingContextMenu.Items.Add(deleteAllMenuItem);
            deleteMenuItem.Items.Add(deleteAllMenuItem);

            filmingContextMenu.Items.Add(deleteMenuItem);

        }

        #endregion

        #region PET SUV菜单子项目
        private void InitSuvMenuItems()
        {
            suvMenuItem.Items.Clear();
            MenuItem filmingSetSUVbw = new MenuItem();
            filmingSetSUVbw.Header = Card.TryFindResource("UID_Filming_SetSUVbw");
            filmingSetSUVbw.IsChecked = IsSetSUVbwChecked;
            filmingSetSUVbw.Command = SetSUVbw;
            filmingSetSUVbw.IsCheckable = true;
            filmingSetSUVbw.Checked += OnSetSUVbwChecked;
            AutomationProperties.SetAutomationId(filmingSetSUVbw, "ID_MNU_FILMING_SETSUVBW");
            suvMenuItem.Items.Add(filmingSetSUVbw);

            MenuItem filmingSetSUVbsa = new MenuItem();
            filmingSetSUVbsa.Header = Card.TryFindResource("UID_Filming_SetSUVbsa");
            filmingSetSUVbsa.IsChecked = IsSetSUVbsaChecked;
            filmingSetSUVbsa.Command = SetSUVbsa;
            filmingSetSUVbsa.IsCheckable = true;
            filmingSetSUVbsa.Checked += OnSetSUVbsaChecked;
            AutomationProperties.SetAutomationId(filmingSetSUVbsa, "ID_MNU_FILMING_SETSUVBSA");
            suvMenuItem.Items.Add(filmingSetSUVbsa);

            MenuItem filmingSetSUVlbm = new MenuItem();
            filmingSetSUVlbm.Header = Card.TryFindResource("UID_Filming_SetSUVlbm");
            filmingSetSUVlbm.IsChecked = IsSetSUVlbmChecked;
            filmingSetSUVlbm.Command = SetSUVlbm;
            filmingSetSUVlbm.IsCheckable = true;
            filmingSetSUVlbm.Checked += OnSetSUVlbmChecked;
            AutomationProperties.SetAutomationId(filmingSetSUVlbm, "ID_MNU_FILMING_SETSUVLBM");
            suvMenuItem.Items.Add(filmingSetSUVlbm);

            MenuItem filmingSetSUVTypeConcentration = new MenuItem();
            filmingSetSUVTypeConcentration.Header = Card.TryFindResource("UID_Filming_SetSUVTypeConcentration");
            filmingSetSUVTypeConcentration.IsChecked = IsSetSUVTypeConcentrationChecked;
            filmingSetSUVTypeConcentration.Command = SetSUVTypeConcentration;
            filmingSetSUVTypeConcentration.IsCheckable = true;
            filmingSetSUVTypeConcentration.Checked += OnSetSUVTypeConcentrationChecked;
            AutomationProperties.SetAutomationId(filmingSetSUVTypeConcentration, "ID_MNU_FILMING_SETSUVTYPE_CONCENTRATION");
            suvMenuItem.Items.Add(filmingSetSUVTypeConcentration);

            MenuItem filmingSetSUVPercent = new MenuItem();
            filmingSetSUVPercent.Header = Card.TryFindResource("UID_Filming_SetSUVPercent");
            filmingSetSUVPercent.IsChecked = IsSetSUVPercentChecked;
            filmingSetSUVPercent.Command = SetSUVPercent;
            filmingSetSUVPercent.IsCheckable = true;
            filmingSetSUVPercent.Checked += OnSetSUVPercentChecked;
            AutomationProperties.SetAutomationId(filmingSetSUVPercent, "ID_MNU_FILMING_SETSUVPERCENT");
            suvMenuItem.Items.Add(filmingSetSUVPercent);


        }
        #endregion

        #region 多分格
        private void InitDivideMenuItems()
        {
            filmingContextMenuDivideCell = new MenuItem();
            filmingContextMenuDivideCell.Header = Card.TryFindResource("UID_Filming_ContextMenu_InsertDivideCell");
            AutomationProperties.SetAutomationId(filmingContextMenuDivideCell, "mnuDivideCell");
            filmingContextMenuDivideCell.Command = InsertDivideCellCommand;
            filmingContextMenu.Items.Add(filmingContextMenuDivideCell);

            filmingContextMenuUndoDivideCell = new MenuItem();
            filmingContextMenuUndoDivideCell.Header = Card.TryFindResource("UID_Filming_ContextMenu_UndoDivideCell");
            AutomationProperties.SetAutomationId(filmingContextMenuUndoDivideCell, "mnuUndoDivideCell");
            filmingContextMenuUndoDivideCell.Command = UndoDivideCellCommand;
            filmingContextMenu.Items.Add(filmingContextMenuUndoDivideCell);
            filmingContextMenuUndoDivideCell.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region 配置菜单项
        private void InitConfigMenuItems()
        {
            #region repack

            filmingContextMenuRepack = new MenuItem();
            filmingContextMenuRepack.Header = Card.TryFindResource("UID_Filming_Repack");
            filmingContextMenuRepack.IsEnabled = IsRepackMenuEnable;
            filmingContextMenuRepack.IsCheckable = true;
            filmingContextMenuRepack.IsChecked = true;
            filmingContextMenuRepack.Checked += OnRepackChecked;
            filmingContextMenuRepack.Unchecked += OnRepackUnChecked;
            AutomationProperties.SetAutomationId(filmingContextMenuRepack, "ID_MNU_FILMING_REPACK");

            #endregion repack
            
            #region ScaleRuler

            filmingContextMenuScaleRuler = new MenuItem();
            filmingContextMenuScaleRuler.Header = Card.TryFindResource("UID_Filming_ScaleRuler");
            filmingContextMenuScaleRuler.IsCheckable = true;
            filmingContextMenuScaleRuler.IsChecked = Card.commands.IfShowImageRuler;
            filmingContextMenuScaleRuler.Command = Card.commands.ScaleRulerSwitchCommand;
            AutomationProperties.SetAutomationId(filmingContextMenuScaleRuler, "ID_MNU_FILMING_SCALERULER");


            #endregion ScaleRuler

            filmingContextMenuAnnotationCustomization = new MenuItem();
            filmingContextMenuAnnotationCustomization.Header = Card.TryFindResource("UID_Filming_AnnotationDisplayType_Partial");
            AutomationProperties.SetAutomationId(filmingContextMenuAnnotationCustomization, "ID_MNU_FILMING_IMAGETEXTDISPLAY_CUSTOMIZATION");
            filmingContextMenuAnnotationCustomization.IsCheckable = true;
            filmingContextMenuAnnotationCustomization.IsChecked = IsAnnotationCustomizationChecked;
            filmingContextMenuAnnotationCustomization.Command = SetImageTextDisplayCustomization;

            #region ColorBar
            var filmingShowHideColorBar = new MenuItem();
            filmingShowHideColorBar.Header = Card.TryFindResource("UID_Filming_ShowHideColorBar");
            filmingShowHideColorBar.Command = Card.commands.ShowHideColorBarCommond;
            AutomationProperties.SetAutomationId(filmingShowHideColorBar, "ID_MNU_FILMING_SHOWHIDECOLORBAR");

            #endregion ColorBar


            configMenuItem = new MenuItem();
            if (!isModalityMG)
            {
                configMenuItem.Header = Card.TryFindResource("UID_Filming_Config");
                // configMenuItem.Command = Card.commands.DeleteParentCommand;
                AutomationProperties.SetAutomationId(configMenuItem, "ID_MNU_FILMING_Config");
                configMenuItem.Items.Add(filmingContextMenuRepack);
                configMenuItem.Items.Add(filmingContextMenuScaleRuler);
                configMenuItem.Items.Add(filmingShowHideColorBar);
                filmingContextMenu.Items.Add(configMenuItem);

            }
            else
            {
                filmingContextMenu.Items.Add(filmingContextMenuRepack);
                filmingContextMenu.Items.Add(filmingContextMenuScaleRuler);
                
            }

            filmingContextMenu.Items.Add(filmingContextMenuAnnotationCustomization);


        }

        #endregion


        public MenuItem GetContextMenuItem(string header)
        {
            MenuItem selectMenu = null;
            if (null != this.filmingContextMenu && null != this.filmingContextMenu.Items)
            {
                foreach (var item in this.filmingContextMenu.Items)
                {
                    var mItems = item as MenuItem;
                    if (mItems != null
                        && mItems.Header != null
                        && mItems.Header.ToString() == header)
                    {
                        selectMenu = mItems;
                        break;
                    }
                }
            }

            return selectMenu;
        }

        #region [--SUVType status--]

        public void OnSetSUVbwChecked(object sender, RoutedEventArgs e)
        {
            //filmingSetSUVbsa.IsChecked = false;
            //filmingSetSUVlbm.IsChecked = false;
            //filmingSetSUVTypeConcentration.IsChecked = false;
            //filmingSetSUVPercent.IsChecked = false;
        }

        public void OnSetSUVbsaChecked(object sender, RoutedEventArgs e)
        {
            //filmingSetSUVbw.IsChecked = false;
            //filmingSetSUVlbm.IsChecked = false;
            //filmingSetSUVTypeConcentration.IsChecked = false;
            //filmingSetSUVPercent.IsChecked = false;
        }

        public void OnSetSUVlbmChecked(object sender, RoutedEventArgs e)
        {
            //filmingSetSUVbsa.IsChecked = false;
            //filmingSetSUVbw.IsChecked = false;
            //filmingSetSUVTypeConcentration.IsChecked = false;
            //filmingSetSUVPercent.IsChecked = false;
        }

        public void OnSetSUVTypeConcentrationChecked(object sender, RoutedEventArgs e)
        {
            //filmingSetSUVbsa.IsChecked = false;
            //filmingSetSUVbw.IsChecked = false;
            //filmingSetSUVlbm.IsChecked = false;
            //filmingSetSUVPercent.IsChecked = false;
        }

        public void OnSetSUVPercentChecked(object sender, RoutedEventArgs e)
        {
            //filmingSetSUVbsa.IsChecked = false;
            //filmingSetSUVbw.IsChecked = false;
            //filmingSetSUVlbm.IsChecked = false;
            //filmingSetSUVTypeConcentration.IsChecked = false;
        }

        public void OnAnnotationAllChecked(object sender, RoutedEventArgs e)
        {
            //filmingContextMenuAnnotationNone.IsChecked = false;
            //filmingContextMenuAnnotationCustomization.IsChecked = false;
            //filmingContextMenuAnnotationCustomization2.IsChecked = false;
        }

        public void OnAnnotationNoneChecked(object sender, RoutedEventArgs e)
        {
            //filmingContextMenuAnnotationAll.IsChecked = false;
            //filmingContextMenuAnnotationCustomization.IsChecked = false;
            //filmingContextMenuAnnotationCustomization2.IsChecked = false;
        }

        public void OnAnnotationCustomizationChecked(object sender, RoutedEventArgs e)
        {
            //filmingContextMenuAnnotationAll.IsChecked = false;
            //filmingContextMenuAnnotationNone.IsChecked = false;
            //filmingContextMenuAnnotationCustomization2.IsChecked = false;
        }

        public void OnAnnotationCustomization2Checked(object sender, RoutedEventArgs e)
        {
            //filmingContextMenuAnnotationAll.IsChecked = false;
            //filmingContextMenuAnnotationNone.IsChecked = false;
            //filmingContextMenuAnnotationCustomization.IsChecked = false;
        }

        private bool IsSetSUVCheckedByUnit(string unit)
        {
            var isSetSUVChecked = Card.ActiveFilmingPageList.All(page => page.SelectedCells().All(
                            cell => (cell.IsEmpty || (cell.Image != null && cell.Image.CurrentPage != null && cell.Image.CurrentPage.Modality != Modality.PT)
                                || (!cell.IsEmpty && cell.Image.CurrentPage.Unit == unit))));
            return isSetSUVChecked;
        }

        private bool _isSetSUVbwChecked;
        public bool IsSetSUVbwChecked
        {
            get
            {
                _isSetSUVbwChecked = IsSetSUVCheckedByUnit(" SUV bw");
                return _isSetSUVbwChecked;
            }
            set
            {
                if (_isSetSUVbwChecked != value)
                {
                    _isSetSUVbwChecked = value;
                }
            }
        }

        private bool _isSetSUVbsaChecked;
        public bool IsSetSUVbsaChecked
        {
            get
            {
                _isSetSUVbsaChecked = IsSetSUVCheckedByUnit(" SUV bsa");
                return _isSetSUVbsaChecked;
            }
            set
            {
                if (_isSetSUVbsaChecked != value)
                {
                    _isSetSUVbsaChecked = value;
                }
            }
        }

        private bool _isSetSUVlbmChecked;
        public bool IsSetSUVlbmChecked
        {
            get
            {
                _isSetSUVlbmChecked = IsSetSUVCheckedByUnit(" SUV lbm");
                return _isSetSUVlbmChecked;
            }
            set
            {
                if (_isSetSUVlbmChecked != value)
                {
                    _isSetSUVlbmChecked = value;
                }
            }
        }

        private bool _isSetSUVTypeConcentrationChecked;
        public bool IsSetSUVTypeConcentrationChecked
        {
            get
            {
                _isSetSUVTypeConcentrationChecked = IsSetSUVCheckedByUnit(" Bq/ml");
                return _isSetSUVTypeConcentrationChecked;
            }
            set
            {
                if (_isSetSUVTypeConcentrationChecked != value)
                {
                    _isSetSUVTypeConcentrationChecked = value;
                }
            }
        }

        private bool _isSetSUVPercentChecked;
        public bool IsSetSUVPercentChecked
        {
            get
            {
                _isSetSUVPercentChecked = IsSetSUVCheckedByUnit(" %");
                return _isSetSUVPercentChecked;
            }
            set
            {
                if (_isSetSUVPercentChecked != value)
                {
                    _isSetSUVPercentChecked = value;
                }
            }
        }

        private bool IsSetSUVEnabledByUnit(string unit)
        {
            var isSetSUVEnabled = Card.ActiveFilmingPageList.All(page => page.SelectedCells().All(
                            cell => (cell.IsEmpty || (cell.Image != null && cell.Image.CurrentPage != null && cell.Image.CurrentPage.Modality != Modality.PT)
                                || (!cell.IsEmpty && cell.Image.CurrentPage.DisplayDataExtension != null && cell.Image.CurrentPage.DisplayDataExtension.CanConvertToTargetUnit(unit)))));
            return isSetSUVEnabled;
        }

        private bool _isSetSUVbwEnabled = true;
        public bool IsSetSUVbwEnabled
        {
            get
            {
                _isSetSUVbwEnabled = IsSetSUVEnabledByUnit(" SUV bw");
                return _isSetSUVbwEnabled;
            }
        }


        private bool _isSetSUVbsaEnabled = true;
        public bool IsSetSUVbsaEnabled
        {
            get
            {
                _isSetSUVbsaEnabled = IsSetSUVEnabledByUnit(" SUV bsa");
                return _isSetSUVbsaEnabled;
            }
        }

        private bool _isSetSUVlbmEnabled = true;
        public bool IsSetSUVlbmEnabled
        {
            get
            {
                _isSetSUVlbmEnabled = IsSetSUVEnabledByUnit(" SUV lbm");
                return _isSetSUVlbmEnabled;
            }
        }

        private bool _isSetSUVTypeConcentrationEnabled = true;
        public bool IsSetSUVTypeConcentrationEnabled
        {
            get
            {
                _isSetSUVTypeConcentrationEnabled = IsSetSUVEnabledByUnit(" Bq/ml");
                return _isSetSUVTypeConcentrationEnabled;
            }
        }

        private bool _isSetSUVPercentEnabled = true;
        public bool IsSetSUVPercentEnabled
        {
            get
            {
                _isSetSUVPercentEnabled = IsSetSUVEnabledByUnit(" %");
                return _isSetSUVPercentEnabled;
            }
        }

        #endregion

        #region [添加删除定位像参考线][jinyang.li]
        private ICommand _addLocalizedImageReferenceLineCommand;
        public ICommand AddLocalizedImageReferenceLineCommand
        {
            get
            {
                return _addLocalizedImageReferenceLineCommand ?? (_addLocalizedImageReferenceLineCommand = new RelayCommand(
                                                                                                     param =>
                                                                                                     {
                                                                                                         AddLocalizedImageReferenceLine();
                                                                                                     },
                                                                                                     param =>
                                                                                                     (IsEnableAddLocalizedImageReferenceLine)));
            }
        }
        public bool IsLocalizedImageReferenceLineEnabled
        {
            get { return (IsEnableAddLocalizedImageReferenceLine || IsEnableDeleteLocalizedImageReferenceLine); }
        }

        public bool IsLocalizedImageReferenceLineChecked
        {
            get { return IsEnableDeleteLocalizedImageReferenceLine; }
        }

        public bool IsEnableAddLocalizedImageReferenceLine
        {
            get
            {
                List<DisplayData> refPages;
                if (false == GetSelectedPages(out refPages))
                {
                    return false;
                }
                else
                {
                    int haveNotLocalizedImageCount = 0;
                    int canLocalizedImageCount = 0;//判断选中的cell中的displayData是否包含tag ==》“ReferencedSopInstanceUID”
                    foreach (var displayData in refPages)
                    {
                        List<string> sopUIDs;
                        if (OverlayLocalizedImageHelper.Instance.GetLocalizedImageSopUIDList(displayData, out sopUIDs))
                        {
                            canLocalizedImageCount++;
                            if (false == displayData.Overlays.Any(n => n is OverlayLocalizedImage))
                                haveNotLocalizedImageCount++;
                        }
                    }

                    if (canLocalizedImageCount >= refPages.Count && haveNotLocalizedImageCount > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        private void AddLocalizedImageReferenceLine()
        {
            Logger.Instance.LogPerformanceRecord("[Begin][AddLocalRefImage]");
            List<DisplayData> refPages;

            GetSelectedPages(out refPages);
            string imgTxtConfigContent = "";
            if (refPages.Count > 0)
            {
                if(Printers.Instance.Modality2FilmingImageTextConfigContent.ContainsKey(refPages[0].Modality.ToString()))
                    imgTxtConfigContent = Printers.Instance.Modality2FilmingImageTextConfigContent[refPages[0].Modality.ToString()];
            }
            int succeedLoad;
            var position = Card.BtnEditCtrl.GetLocalizedImageLastAddedPosition();
            var firstDisplayedFilmpageControl = this.Card.DisplayedFilmPage.First().filmingViewerControl;
            OverlayLocalizedImageHelper.Instance.GenerateOverlayLocalizedImage(refPages,
                                                                                                                                 position,
                                                                                                                                 firstDisplayedFilmpageControl,
                                                                                                                                 out succeedLoad,
                                                                                                                                 imgTxtConfigContent);

            if (succeedLoad <= 0)
            {
                MessageBoxHandler.Instance.ShowWarning("UID_Filming_Warning_HaveNotLocalizedImage");
            }
            Logger.Instance.LogPerformanceRecord("[End][AddLocalRefImage]");
        }

        public void SingleUseCellPositionChangedEvent(object sender, SingleUseCellPositionChangedEventArgs e)
        {
            Logger.Instance.LogPerformanceRecord("[Begin][RefImagePositionChanged]");
            SetLocalizedImageLastAddedPositionIndex(e.ChangeToPos);
            if (!Card.IfZoomWindowShowState)
            {
                foreach (var cell in FilmingCard._miniCellsParentCellsList)
                {
                    if (cell.Image == null || cell.Image.CurrentPage == null) continue;
                    var overlayLocalizedImage = cell.GetOverlay(OverlayType.LocalizedImage) as OverlayLocalizedImage;
                    if (null != overlayLocalizedImage)
                    {
                        overlayLocalizedImage.GraphicLocalizedImage.LocalizedImagePos = e.ChangeToPos;
                        
                    }
                }

            }
            else
            {
                string sopInfo = Card.SelectedLocalizedImageInfo;
                
                foreach (var filmingPage in Card.EntityFilmingPageList)
                {
                    List<MedViewerControlCell> cellsOFflimingPage = new List<MedViewerControlCell>();
                    cellsOFflimingPage.AddRange(filmingPage.Cells);

                    foreach (var cell in cellsOFflimingPage)
                    {
                        if (null == cell
                            || cell.IsEmpty
                            || null == cell.Image
                            || null == cell.Image.CurrentPage)
                        {
                            continue;
                        }
                        var overlayLocalizedImage = cell.GetOverlay(OverlayType.LocalizedImage) as OverlayLocalizedImage;
                        if (null != overlayLocalizedImage )
                        {
                            if (!cell.Image.CurrentPage.ImageHeader.DicomHeader.ContainsKey(ServiceTagName.SeriesInstanceUID)) continue;
                            var uid = cell.Image.CurrentPage.ImageHeader.DicomHeader[ServiceTagName.SeriesInstanceUID];
                            var changedInfo = uid + cell.Image.CurrentPage.UserSpecialInfo;
                            if (sopInfo != changedInfo) continue;
                            overlayLocalizedImage.GraphicLocalizedImage.LocalizedImagePos = e.ChangeToPos;
                        }
                    }
                }
            }

            Logger.Instance.LogPerformanceRecord("[End][RefImagePositionChanged]");
        }
      
        private void SetLocalizedImageLastAddedPositionIndex(ImgTxtPosEnum pos)
        {
            int index = -1;
            if (pos == ImgTxtPosEnum.BottomRight) index = 2;
            else if (pos == ImgTxtPosEnum.TopRight) index = 1;
            else if (pos == ImgTxtPosEnum.BottomLeft) index = 3;
            else if (pos == ImgTxtPosEnum.TopLeft) index = 0;
            if (index > -1)
                Printers.Instance.LocalizedImagePosition = index;
        }

        private bool GetSelectedPages(out List<DisplayData> selectedPages)
        {
            selectedPages = new List<DisplayData>();
            foreach (var filmingPage in Card.EntityFilmingPageList)
            {
                List<MedViewerControlCell> selectCellsOFflimingPage = new List<MedViewerControlCell>();
                selectCellsOFflimingPage.AddRange(filmingPage.SelectedCells());

                foreach (var cell in selectCellsOFflimingPage)
                {
                    if (null == cell
                        || cell.IsEmpty
                        || null == cell.Image
                        || null == cell.Image.CurrentPage)
                    {
                        continue;
                    }
                    selectedPages.Add(cell.Image.CurrentPage);
                }
            }

            if (selectedPages.Count <= 0) return false;
            return true;
        }

        private ICommand _deleteLocalizedImageReferenceLineCommand;
        public ICommand DeleteLocalizedImageReferenceLineCommand
        {
            get
            {
                return _deleteLocalizedImageReferenceLineCommand ?? (_deleteLocalizedImageReferenceLineCommand = new RelayCommand(
                                                                                                     param =>
                                                                                                     {
                                                                                                         DeleteLocalizedImageReferenceLine();
                                                                                                     },
                                                                                                     param =>
                                                                                                     (IsEnableDeleteLocalizedImageReferenceLine)));
            }
        }

        public bool IsEnableDeleteLocalizedImageReferenceLine
        {
            get
            {
                List<DisplayData> refPages;
                if (false == GetSelectedPages(out refPages))
                {
                    return false;
                }
                else
                {
                    int haveLocalizedImageCount = 0;
                    foreach (var displayData in refPages)
                    {
                        if (displayData.Overlays.Any(n => n is OverlayLocalizedImage))
                        {
                            haveLocalizedImageCount++;
                        }
                    }

                    if (haveLocalizedImageCount >= 1)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        private void DeleteLocalizedImageReferenceLine()
        {
            List<DisplayData> refPages;
            GetSelectedPages(out refPages);

            foreach (var displayData in refPages)
            {
                IOverlay overlayLocalizedImage = displayData.GetOverlay(OverlayType.LocalizedImage);
                if (null != overlayLocalizedImage)
                {
                    displayData.Overlays.Remove(overlayLocalizedImage);
                }
                displayData.Image.Cell.Refresh();
            }
        }
        public void DeleteSelectedLocalizedImageReferenceLine()
        {
            List<DisplayData> refPages;
            GetSelectedPages(out refPages);
           
            foreach (var displayData in refPages)
            {
                IOverlay overlayLocalizedImage = displayData.GetOverlay(OverlayType.LocalizedImage);

                if (null != overlayLocalizedImage && null != (overlayLocalizedImage as OverlayLocalizedImage) && (overlayLocalizedImage as OverlayLocalizedImage).GraphicLocalizedImage.MiniCell.IsSelected)
                {
                    displayData.Overlays.Remove(overlayLocalizedImage);
                }
                displayData.Image.Cell.Refresh();
            }
        }
        #endregion

        public void OnLocalizedImageReferenceLineChecked(object sender, RoutedEventArgs e)
        {
            if (!IsEnableAddLocalizedImageReferenceLine) return;
            AddLocalizedImageReferenceLine();
        }

        public void OnLocalizedImageReferenceLineUnChecked(object sender, RoutedEventArgs e)
        {
            if (!IsEnableDeleteLocalizedImageReferenceLine) return;
            DeleteLocalizedImageReferenceLine();
        }

        public void OnRepackChecked(object sender, RoutedEventArgs e)
        {
            if (!Card.initializded) return;

            Repack(RepackMode.RepackMenu);
            //FilmingHelper.WriteRepackStatusToConfigFile(true);
            Printers.Instance.IfRepack = true;
            Card.UpdateUIStatus();
        }

        public void OnRepackUnChecked(object sender, RoutedEventArgs e)
        {
            //FilmingHelper.WriteRepackStatusToConfigFile(false);
            if (!Card.initializded) return;

            Printers.Instance.IfRepack = false;
            //Card.UpdateUIStatus();
        }

        public bool IsCellModalitySC
        {
            get { return Card.FilmingCardModality == FilmingUtility.EFilmModality; }
        }

        public bool IsRepackMenuEnable
        {
            get { return !IsCellModalitySC; }
        }

        public void Repack(RepackMode repackMode = RepackMode.RepackDefault)
        {
            Logger.Instance.LogPerformanceRecord("[Begin][Repack]");
            try
            {
                if (Card.CurrentFilmingState != FilmingRunState.ChangeLayout)
                {
                    Card.DisableUI();
                }
                int beginPageIndex = 0;
                //int regionNo = 0;

                while (beginPageIndex < Card.EntityFilmingPageList.Count)
                {
                    //计算每个分区开始的胶片index，第一个含有空格子的胶片index,找不到即不需repack，直接返回
                    if(Printers.Instance.RepackMode == 0)
                    {
                        beginPageIndex = Card.EntityFilmingPageList.FindIndex(beginPageIndex,
                                                                     film =>
                                                                     film.IfNeedRepack);
                    }
                    else
                    {
                        beginPageIndex = Card.EntityFilmingPageList.FindIndex(beginPageIndex,
                                                                     film =>
                                                                     film.IfNeedRepackControlCell);
                    }
                    
                    if (beginPageIndex == -1) return;

                    //查找该分区的结束胶片的下一张胶片，找不到即赋值整个胶片数
                    int endPageIndex = Card.EntityFilmingPageList.FindIndex(beginPageIndex + 1,
                                                                       film =>
                                                                       film.FilmPageType == FilmPageType.BreakFilmPage);
                    if (endPageIndex == -1) endPageIndex = Card.EntityFilmingPageList.Count;

                    //根据查询结果创建需要repack的胶片的list
                    List<FilmingPageControl> lstPages = new List<FilmingPageControl>();
                    lstPages = Card.EntityFilmingPageList.GetRange(beginPageIndex, endPageIndex - beginPageIndex);
                    var pr = new RepackRegion(lstPages);
                    //若所有胶片都是普通布局胶片则调用采用RepackRegion类优化方法
                    if (lstPages.All(page => page.ViewportLayout.LayoutType == LayoutTypeEnum.RegularLayout))
                    {
                        if(Printers.Instance.RepackMode == 0)
                            pr.Repack();
                        else
                            pr.RepackControlCell();
                    }
                    else
                    {
                        pr.RepackControlCell();
                    }

                    beginPageIndex = endPageIndex;
                }

                FilmingHelper.PrintTimeInfo("End:");

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            finally
            {
                var emptyFilms = new List<FilmingPageControl>();
                for (int i = 1; i < Card.EntityFilmingPageList.Count; i++)
                {
                    var film = Card.EntityFilmingPageList[i];
                    if (film.IsEmpty())
                        emptyFilms.Add(film);
                    else if (!film.SelectedCells().Any()) //if no cell is selected, film is not selected -- bug 592264 -- 2016/02/15 -- hui.wang
                        film.IsSelected = false;
                }

                foreach (var filmingPageControl in emptyFilms.Where(f => f.FilmPageType != FilmPageType.BreakFilmPage))
                {
                    if (Card.EntityFilmingPageList.Count > 1)
                    {
                        Card.commands.DeleteFilmPage(filmingPageControl);
                    }
                }

                // Keep at least one page if all the pages are empty and deleted
                if (Card.EntityFilmingPageList.Count == 0)
                    Card.OnAddFilmPageAfterClearFilmingCard(null, null);
                if (Card.CurrentFilmingState != FilmingRunState.ChangeLayout)
                {
                    Card.EnableUI();
                    Card.EntityFilmingPageList.UpdatePageLabel();
                }

                Logger.Instance.LogPerformanceRecord("[End][Repack]");
            }
        }

        //private void RepackWhenClose()
        //{
        //    if (Card.IsEnableRepack)
        //    {
        //        Mouse.SetCursor(CursorUtility.Cursors[CursorUtility.WAIT]);
        //        this.Repack();
        //        Mouse.SetCursor(CursorUtility.Cursors[CursorUtility.NORMAL]);
        //    }
        //}

        private void repackRegion(int regionNo, int beginPageIndex, int endPageIndex, RepackMode repackMode)
        {
            try
            {
                Logger.LogFuncUp();
                Logger.LogTracing("repacking region : " + beginPageIndex + "-" + endPageIndex);

                // Judge if it needs to repack in this region
                // There is no need to repack if there is no empty cells(if any) in between of the non-empty cells
                List<int> emptyCellIdxInRegion = new List<int>();
                List<int> selectedCellIdxInRegion = new List<int>();

                GetSelectedAndEmptyCellIndexInRegion(beginPageIndex, endPageIndex, ref selectedCellIdxInRegion,
                                                     ref emptyCellIdxInRegion);

                // There is no empty cells, no need to do repack in this region
                if ((emptyCellIdxInRegion.Count == 0) && (repackMode != RepackMode.RepackCut) &&
                    (repackMode != RepackMode.RepackDelete))
                    return;

                // Remember the last selected cell index in the context of the region
                // Judge if the LastSelectedFilmingPage is in this region
                // Need to reinstate the last selected filming page and last selected cell after layout change
                int lastSelectedCellIndexInRegion = -1;
                int emptyCellCntBeforeLastSelected = 0;
                if (Card.LastSelectedFilmingPage != null && Card.LastSelectedCell != null)
                {
                    if (Card.LastSelectedFilmingPage.FilmPageIndex >= beginPageIndex &&
                        Card.LastSelectedFilmingPage.FilmPageIndex < endPageIndex)
                    {
                        int totalCnt = 0;
                        int totalEmptyCnt = 0;
                        for (int i = beginPageIndex; i < Card.LastSelectedFilmingPage.FilmPageIndex; i++)
                        {
                            totalCnt += Card.EntityFilmingPageList[i].Cells.Count();
                            totalEmptyCnt += Card.EntityFilmingPageList[i].Cells.Count(cell => cell.IsEmpty);
                        }
                        lastSelectedCellIndexInRegion = totalCnt + Card.LastSelectedCell.CellIndex;
                        emptyCellCntBeforeLastSelected = totalEmptyCnt +
                                                         Card.LastSelectedFilmingPage.Cells.Count(
                                                             cell =>
                                                             cell.IsEmpty &&
                                                             (cell.CellIndex < Card.LastSelectedCell.CellIndex));
                    }
                }


                //1. backup image cells
                var imageCells = new List<MedViewerControlCell>();
                for (int i = beginPageIndex; i < endPageIndex; i++)
                {
                    imageCells.AddRange(Card.EntityFilmingPageList[i].Cells.Where(cell => !cell.IsEmpty));
                }
                //2. rearrange image cells
                int pageIndex = beginPageIndex;
                var film = Card.EntityFilmingPageList[pageIndex];
                int cellIndex = 0;
                int cellCountOfFilm = film.filmingViewerControl.LayoutManager.RootCell.DisplayCapacity;
                var cellsTobBeUpdate = film.Cells;
                foreach (var imageCell in imageCells)
                {
                    if (cellIndex >= cellCountOfFilm)
                    {
                        pageIndex++;
                        film = Card.EntityFilmingPageList[pageIndex];
                        cellIndex = 0;
                        cellCountOfFilm = film.filmingViewerControl.LayoutManager.RootCell.DisplayCapacity;
                        cellsTobBeUpdate = film.Cells;
                    }
                    var cell = cellsTobBeUpdate.ElementAt(cellIndex);
                    var image = cell.Image;
                    var displayData = imageCell.Image.CurrentPage;
                    if (image.Count == 0)
                        image.AddPage(imageCell.Image.CurrentPage);
                    else //image.ReplacePage(displayData, 0); //No DataSourceChanged Event after ReplacePage 
                    {
                        image.Clear();
                        cell.Refresh();
                        image.AddPage(displayData);
                    }
                    #region	[edit by jinyang.li_OK]
                    /*    FilmingHelper.RefereshDisplayMode(displayData);*/
                    #endregion

                    //add by hui.wang, repacked image in original empty cellimpl has no these mouse action 2013/09/23
                    FilmPageUtil.SetAllActionExceptLeftButton(cell);
                    //add by hui.wang, repacked image in original empty cellimpl has no these mouse action 2013/09/23


                    cell.Refresh();

                    cellIndex++;
                }

                //3. clear dirty image cells
                while (pageIndex < endPageIndex)
                {
                    if (cellIndex >= cellCountOfFilm)
                    {
                        pageIndex++;
                        if (pageIndex >= endPageIndex) break;
                        film = Card.EntityFilmingPageList[pageIndex];
                        cellIndex = 0;
                        cellCountOfFilm = film.filmingViewerControl.LayoutManager.RootCell.DisplayCapacity;
                        cellsTobBeUpdate = film.Cells;
                    }
                    var cell = cellsTobBeUpdate.ElementAt(cellIndex);
                    if (!cell.IsEmpty)
                    {
                        FilmPageUtil.ClearAllActions(cell);
                        cell.Image.Clear();
                        cell.Refresh();
                    }
                    cellIndex++;
                }

                //4. reselect the cells
                int emptyCellCntInFront = 0;
                int indexAfterRepack = 0;
                int indexInPage = 0;
                List<FilmingPageControl> pagesInRegion = new List<FilmingPageControl>();
                for (int i = beginPageIndex; i < endPageIndex; i++)
                {
                    FilmingPageControl filmPage = Card.EntityFilmingPageList[i];
                    pagesInRegion.Add(filmPage);

                    // Unselect the page and its cells
                    filmPage.IsSelected = false;
                    filmPage.SelectedAll(false);
                }

                // For cut, keep the original cut cells as selected

                if ((repackMode == RepackMode.RepackCut) || (repackMode == RepackMode.RepackDelete))
                {
                    emptyCellIdxInRegion.Clear();
                }

                foreach (var index in selectedCellIdxInRegion)
                {
                    emptyCellCntInFront = emptyCellIdxInRegion.FindIndex(idx => idx >= index);
                    if (emptyCellCntInFront == -1) // All empty cells are in front of the selected cell
                        emptyCellCntInFront = emptyCellIdxInRegion.Count;

                    indexAfterRepack = index - emptyCellCntInFront;

                    FilmingPageControl page = GetCellFromIndexInRegion(pagesInRegion, indexAfterRepack, ref indexInPage);

                    if (page != null)
                    {
                        MedViewerControlCell cell = page.GetCellByIndex(indexInPage);
                        cell.IsSelected = true;
                        page.IsSelected = true;
                        FilmPageUtil.ViewportOfCell(cell, page).IsSelected = true;
                    }
                    else
                    {
                        // Cell is not in the range
                        break;
                    }
                }

                // 5. Reset the Last Selected Cell to the new cell
                if (lastSelectedCellIndexInRegion != -1)
                {
                    int targetIndex = 0;
                    List<FilmingPageControl> filmPagesInRegion = GetFilmingPagesInRegion((uint)regionNo);
                    int targetSelectedCellIndex = lastSelectedCellIndexInRegion - emptyCellCntBeforeLastSelected;
                    FilmingPageControl targetPage = GetCellFromIndexInRegion(filmPagesInRegion, targetSelectedCellIndex,
                                                                             ref targetIndex);

                    if (targetPage != null)
                    {
                        Card.LastSelectedFilmingPage = targetPage;
                        Card.LastSelectedCell = Card.LastSelectedFilmingPage.GetCellByIndex(targetIndex);
                        Card.LastSelectedViewport = FilmPageUtil.ViewportOfCell(Card.LastSelectedCell, Card.LastSelectedFilmingPage);
                    }
                    else
                    {
                        Logger.LogError(
                            "LastSelectedFilmingPage out of its region after repack! Set it to last selected cell");
                        ResetLastSelectedInRegion(pagesInRegion);
                    }
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Get the selected cell indices and empty cell indices before the last non-empty cell in the region
        /// these indices are in the context of the region rather than of the page
        /// </summary>
        /// <param name="startPageIdx"></param>
        /// <param name="endPageIdx"></param>
        /// <param name="selectedCellIdx"></param>
        /// <param name="emptyCellIdx"></param>
        private void GetSelectedAndEmptyCellIndexInRegion(int startPageIdx, int endPageIdx,
                                                          ref List<int> selectedCellIdx, ref List<int> emptyCellIdx)
        {
            int totalIndex = 0;

            List<MedViewerControlCell> allCells = new List<MedViewerControlCell>();
            for (int i = startPageIdx; i < endPageIdx; i++)
            {
                FilmingPageControl page = Card.EntityFilmingPageList[i];
                foreach (var cell in page.Cells)
                {
                    if (cell.IsEmpty)
                        emptyCellIdx.Add(totalIndex + cell.CellIndex);

                    if (cell.IsSelected)
                        selectedCellIdx.Add(totalIndex + cell.CellIndex);

                    allCells.Add(cell);
                }

                totalIndex += page.Cells.Count();
            }
            // Find the last non-empty cell in the whole region
            int lastNonEmptyCellIdx = allCells.FindLastIndex(cell => !cell.IsEmpty);
            if (lastNonEmptyCellIdx == -1) // all cells are empty
                emptyCellIdx.Clear();
            else if (lastNonEmptyCellIdx < allCells.Count - 1) // there is empty cell after the last non-empty cell
            {
                int idx = emptyCellIdx.FindIndex(index => index == lastNonEmptyCellIdx + 1);
                emptyCellIdx.RemoveRange(idx, emptyCellIdx.Count - idx);
            }
        }

        /// <summary>
        /// This function returns the cell by specifying the index in region (page range)
        /// </summary>
        /// <param name="pages"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private FilmingPageControl GetCellFromIndexInRegion(List<FilmingPageControl> pages, int indexInRegion,
                                                            ref int indexInPage)
        {
            int cellCntInPage = 0;
            int index = indexInRegion;

            foreach (var page in pages)
            {
                cellCntInPage = page.Cells.Count();
                if (cellCntInPage > index)
                {
                    indexInPage = index;
                    return page;
                }
                else
                    index -= cellCntInPage;
            }

            // The index is beyond the range of the pages if we reach here
            return null;
        }

        private List<FilmingPageControl> GetFilmingPagesInRegion(uint regionNo)
        {
            //return EntityFilmingPageList.GetRange(startPageIndex, endPageIndex-startPageIndex);
            int pageCount = Card.EntityFilmingPageList.Count();
            int beginPageIndex = 0;
            int endPageIndex = Card.EntityFilmingPageList.Count();
            int currentRegionNo = -1;
            for (int i = 0; i < pageCount; i++)
            {
                var page = Card.EntityFilmingPageList[i];
                if (page.FilmPageType != FilmPageType.BreakFilmPage) continue;
                currentRegionNo++;
                if (regionNo == currentRegionNo + 1) beginPageIndex = page.FilmPageIndex;
                if (regionNo == currentRegionNo) endPageIndex = page.FilmPageIndex;
            }
            return Card.EntityFilmingPageList.GetRange(beginPageIndex, endPageIndex - beginPageIndex);
        }

        /// <summary>
        /// This function calculate the index of the cell in the whole region
        /// </summary>
        /// <param name="filmPage"></param>
        /// <param name="cellIndexInPage"></param>
        /// <param name="emptyCellCntInFront"></param>
        /// <returns></returns>
        private int GetCellIndexInRegion(int regionNo, FilmingPageControl filmPage, int cellIndexInPage,
                                         ref int emptyCellCntInFront)
        {
            int indexInRegion = cellIndexInPage;
            List<FilmingPageControl> pagesInRegion = GetFilmingPagesInRegion((uint)regionNo);

            // Check the film page is in the region
            if (!pagesInRegion.Exists(page => page.FilmPageIndex == filmPage.FilmPageIndex))
                return -1;

            emptyCellCntInFront = 0;

            foreach (var page in pagesInRegion)
            {
                if (page.FilmPageIndex < filmPage.FilmPageIndex)
                {
                    indexInRegion += page.Cells.Count();
                    emptyCellCntInFront += page.Cells.Count(cell => cell.IsEmpty);
                }
                else if (page.FilmPageIndex == filmPage.FilmPageIndex)
                {
                    emptyCellCntInFront += page.Cells.Count(cell => cell.IsEmpty && cell.CellIndex < cellIndexInPage);
                    break;
                }
            }

            return indexInRegion;
        }

        /// <summary>
        /// This function is to set the last selected page and cell to the last selected cell in region
        /// </summary>
        /// <param name="pagesInRegion"></param>
        private void ResetLastSelectedInRegion(List<FilmingPageControl> pagesInRegion)
        {
            for (int i = pagesInRegion.Count - 1; i >= 0; i--)
            {
                FilmingPageControl page = pagesInRegion[i];

                List<MedViewerControlCell> selectedCells = page.Cells.Where(c => c.IsSelected).ToList();
                if (selectedCells.Count > 0)
                {
                    Card.LastSelectedFilmingPage = page;
                    Card.LastSelectedCell = selectedCells[selectedCells.Count - 1];
                    Card.LastSelectedViewport = FilmPageUtil.ViewportOfCell(Card.LastSelectedCell, Card.LastSelectedFilmingPage);
                }
            }
        }

        private void SetSUVTypeForSelectedCells(string unit)
        {
            foreach (var film in Card.EntityFilmingPageList)
            {
                if (!film.IsVisible) film.IsBeenRendered = false;
                foreach (var cell in film.SelectedCells())
                {
                    FilmingHelper.SetSUVTypeForCell(cell, unit);
                }
            }
            if (Card.IfZoomWindowShowState)
            {
                Card.ZoomViewer.RefreshDisplayCell();
            }
        }



        #region private SetSUVType

        private ICommand _setSUVbw;
        public ICommand SetSUVbw
        {
            get
            {
                return _setSUVbw ?? (_setSUVbw = new RelayCommand(
                                                        param => { SetSUVTypeForSelectedCells(" SUV bw"); },
                                                        param => (IsEnableSUVTypeSetting && IsSetSUVbwEnabled)));
            }
        }

        private ICommand _setSUVbsa;
        public ICommand SetSUVbsa
        {
            get
            {
                return _setSUVbsa ?? (_setSUVbsa = new RelayCommand(
                                                        param => { SetSUVTypeForSelectedCells(" SUV bsa"); },
                                                        param => (IsEnableSUVTypeSetting && IsSetSUVbsaEnabled)));
            }
        }

        private ICommand _setSUVlbm;
        public ICommand SetSUVlbm
        {
            get
            {
                return _setSUVlbm ?? (_setSUVlbm = new RelayCommand(
                                                        param => { SetSUVTypeForSelectedCells(" SUV lbm"); },
                                                        param => (IsEnableSUVTypeSetting && IsSetSUVlbmEnabled)));
            }
        }

        private ICommand _setSUVTypeConcentration;
        public ICommand SetSUVTypeConcentration
        {
            get
            {
                return _setSUVTypeConcentration ?? (_setSUVTypeConcentration = new RelayCommand(
                                                        param => { SetSUVTypeForSelectedCells(" Bq/ml"); },
                                                        param => (IsEnableSUVTypeSetting && IsSetSUVTypeConcentrationEnabled)));
            }
        }

        private ICommand _setSUVPercent;
        public ICommand SetSUVPercent
        {
            get
            {
                return _setSUVPercent ?? (_setSUVPercent = new RelayCommand(
                                                        param => { SetSUVTypeForSelectedCells(" %"); },
                                                        param => (IsEnableSUVTypeSetting && IsSetSUVPercentEnabled)));
            }
        }
        #endregion


        public bool IsEnableImageTextDisplaySetting
        {
            get
            {
                if (IsCellModalitySC) return false;
                return Card.IsCellSelected;
            }
        }


        #region Private method



        private ICommand _setImageTextDisplayAll;
        public ICommand SetImageTextDisplayAll
        {
            get
            {
                return _setImageTextDisplayAll ?? (_setImageTextDisplayAll = new RelayCommand(
                                                                                 param =>
                                                                                 {
                                                                                     Card.BtnEditCtrl.UpdateCornerTextForSelectedCells(ImgTxtDisplayState.All);
                                                                                 },
                                                                                 param =>
                                                                                 (IsEnableImageTextDisplaySetting)));
            }
        }

        private ICommand _setImageTextDisplayNone;
        public ICommand SetImageTextDisplayNone
        {
            get
            {
                return _setImageTextDisplayNone ?? (_setImageTextDisplayNone = new RelayCommand(
                                                                                   param =>
                                                                                   {
                                                                                       Card.BtnEditCtrl.UpdateCornerTextForSelectedCells(ImgTxtDisplayState.None);
                                                                                   },
                                                                                   param =>
                                                                                   (IsEnableImageTextDisplaySetting)));
            }
        }

        private ICommand _setImageTextDisplayCustomization;

        public ICommand SetImageTextDisplayCustomization
        {
            get
            {
                return _setImageTextDisplayCustomization ?? (_setImageTextDisplayCustomization = new RelayCommand(
                                                                                                     param =>
                                                                                                     {
                                                                                                         if (IsAnnotationCustomizationChecked)
                                                                                                             Card.BtnEditCtrl.UpdateCornerTextForSelectedCells(ImgTxtDisplayState.None);         //key 457571改动
                                                                                                         else
                                                                                                             Card.BtnEditCtrl.UpdateCornerTextForSelectedCells(ImgTxtDisplayState.Customization);

                                                                                                     },
                                                                                                     param =>
                                                                                                     (IsEnableImageTextDisplaySetting)));
            }
        }

        //private ICommand _setImageTextDisplayCustomization2;

        //public ICommand SetImageTextDisplayCustomization2
        //{
        //    get
        //    {
        //        return _setImageTextDisplayCustomization2 ?? (_setImageTextDisplayCustomization2 = new RelayCommand(
        //                                                                                             param =>
        //                                                                                             {
        //                                                                                                 Card.BtnEditCtrl.UpdateCornerTextForSelectedCells(ImgTxtDisplayState.Partial2);
        //                                                                                             },
        //                                                                                             param =>
        //                                                                                             (IsEnableImageTextDisplaySetting)));
        //    }
        //}

        #endregion

        private WindowingProtocolsConfigInfo getPreSetWinow(Modality modality)
        {
            var appMisReader = AppMiscellaneousReader.Instance;
            var appMisConfigInfo = appMisReader.Analyze();
            var presetWindows = appMisConfigInfo.Windowing.Protocols.Where(protocols => protocols.Modality == modality);
            return presetWindows.Any() ? presetWindows.First() : null;
        }

        private string createPresetWindowAutoID(string name)
        {
            return "ID_MNU_FILMING_PRESET_WINDOWING_" + name;
        }



        private void OnCtCustomWindowingClick()
        {
            try
            {
                Logger.LogFuncUp();
                _customWindowLevel.InitialActiveFilmingPage(Card.ActiveFilmingPageList);

                var cell = Card.ActiveFilmingPageList.Last().SelectedCells().FindLast(c => c != null && !c.IsEmpty && c.Image != null && c.Image.CurrentPage != null
                    && c.Image.CurrentPage.Modality == Modality.CT);
                if (cell != null && !cell.IsEmpty && cell.Image != null && cell.Image.CurrentPage != null
                    && cell.Image.CurrentPage.Modality == Modality.CT) //CT
                {
                    _customWindowLevel.CustomWLViewModel.Modality = Modality.CT;
                    var wl = cell.Image.CurrentPage.PState.WindowLevel;
                    _customWindowLevel.CustomWLViewModel.CurrentWidthValue = wl.WindowWidth;
                    _customWindowLevel.CustomWLViewModel.CurrentCenterValue = wl.WindowCenter;
                }

                var messageWindow = new MessageWindow
                {
                    WindowTitle = Card.Resources["UID_Filming_PreSet_Windowing_CustomWindowLevel"] as string,
                    WindowChild = _customWindowLevel,
                    WindowDisplayMode = WindowDisplayMode.Default
                };
                Card.HostAdornerCount++;
                messageWindow.ShowModelDialog();
                Card.HostAdornerCount--;

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void OnPtCustomWindowingClick()
        {
            try
            {
                Logger.LogFuncUp();
                _customWindowLevel.InitialActiveFilmingPage(Card.ActiveFilmingPageList);

                var cell = Card.ActiveFilmingPageList.Last().SelectedCells().FindLast(c => c != null && !c.IsEmpty && c.Image != null && c.Image.CurrentPage != null
                    && c.Image.CurrentPage.Modality == Modality.PT);
                if (cell != null && !cell.IsEmpty && cell.Image != null && cell.Image.CurrentPage != null
                    && cell.Image.CurrentPage.Modality == Modality.PT && (cell.Image.CurrentPage.DisplayDataExtension as PETExtension) != null) //PT
                {
                    _customWindowLevel.CustomWLViewModel.Modality = Modality.PT;
                    var petExtension = cell.Image.CurrentPage.DisplayDataExtension as PETExtension;
                    _customWindowLevel.CustomWLViewModel.CurrentWidthValue = petExtension.GetTBValue().Item1;
                    _customWindowLevel.CustomWLViewModel.CurrentCenterValue = petExtension.GetTBValue().Item2;
                }

                var messageWindow = new MessageWindow
                {
                    WindowTitle = Card.Resources["UID_Filming_PreSet_Windowing_CustomWindowLevel"] as string,
                    WindowChild = _customWindowLevel,
                    WindowDisplayMode = WindowDisplayMode.Default
                };
                Card.HostAdornerCount++;
                messageWindow.ShowModelDialog();
                Card.HostAdornerCount--;

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void AddKeyBindings(WindowingProtocolConfigInfo windowingInfo, RelayCommand relayCommand)
        {
            var keyBinding = new KeyBinding();
            if (windowingInfo.KeyGestrue != null && windowingInfo.KeyGestrue.Key != Key.None)
            {
                keyBinding.Key = windowingInfo.KeyGestrue.Key;
                keyBinding.Command = relayCommand;
                if (!Card.InputBindings.Contains(keyBinding))
                {
                    Card.InputBindings.Add(keyBinding);
                }
            }
            else
            {
                var keyBinding1 = new KeyBinding();
                switch (windowingInfo.InputGestureText)
                {
                    case "1":
                        keyBinding.Key = Key.D1;
                        keyBinding1.Key = Key.NumPad1;
                        break;
                    case "2":
                        keyBinding.Key = Key.D2;
                        keyBinding1.Key = Key.NumPad2;
                        break;
                    case "3":
                        keyBinding.Key = Key.D3;
                        keyBinding1.Key = Key.NumPad3;
                        break;
                    case "4":
                        keyBinding.Key = Key.D4;
                        keyBinding1.Key = Key.NumPad4;
                        break;
                    case "5":
                        keyBinding.Key = Key.D5;
                        keyBinding1.Key = Key.NumPad5;
                        break;
                    case "6":
                        keyBinding.Key = Key.D6;
                        keyBinding1.Key = Key.NumPad6;
                        break;
                    case "7":
                        keyBinding.Key = Key.D7;
                        keyBinding1.Key = Key.NumPad7;
                        break;
                    case "8":
                        keyBinding.Key = Key.D8;
                        keyBinding1.Key = Key.NumPad8;
                        break;
                    case "9":
                        keyBinding.Key = Key.D9;
                        keyBinding1.Key = Key.NumPad9;
                        break;
                    case "0":
                        keyBinding.Key = Key.D0;
                        keyBinding1.Key = Key.NumPad0;
                        break;
                }
                keyBinding1.Command = relayCommand;
                if (!Card._ptInputBindingList.Contains(keyBinding1))
                {
                    Card.InputBindings.Add(keyBinding1);
                    Card._ptInputBindingList.Add(keyBinding1);
                }
                keyBinding.Command = relayCommand;
                if (!Card._ptInputBindingList.Contains(keyBinding))
                {
                    Card.InputBindings.Add(keyBinding);
                    Card._ptInputBindingList.Add(keyBinding);
                }
            }
        }

        private void AddCtCustomMenuItem()
        {
            var menuItem = new MenuItem();
            menuItem.Header = Card.Resources["UID_Filming_PreSet_Windowing_Custom"] as string;
            var relayCommand = new RelayCommand(param => OnCtCustomWindowingClick());
            menuItem.Command = relayCommand;
            AutomationProperties.SetAutomationId(menuItem, createPresetWindowAutoID("CUSTOM"));
            menuItem.IsCheckable = false;
            //ctPresetWindowMenuItem.Items.Add(menuItem);
        }

        private void AddPtCustomMenuItem()
        {
            var menuItem = new MenuItem();
            menuItem.Header = Card.Resources["UID_Filming_PreSet_Windowing_Custom"] as string;
            var relayCommand = new RelayCommand(param => OnPtCustomWindowingClick());
            menuItem.Command = relayCommand;
            AutomationProperties.SetAutomationId(menuItem, createPresetWindowAutoID("CUSTOM"));
            menuItem.IsCheckable = false;
            //  ptPresetWindowMenuItem.Items.Add(menuItem);
        }

        public void InitializePresetWindowInfo()
        {
            ctPresetWindowMenuItem.Items.Clear();
            ptPresetWindowMenuItem.Items.Clear();
            presetWindowInfos.Clear();
            Card.ClearPtInputBindings();
            Card._ptInputBindingList.Clear();
            ResetPresetWindowInfo(getPreSetWinow(Modality.PT));
            ResetPresetWindowInfo(getPreSetWinow(Modality.CT));
            if (Card.IsCellSelected && IsSelectedCellsWithSameModalityOrEmptyCell(Modality.CT))
            {
                AddCtCustomMenuItem();
            }
            else if (Card.IsCellSelected && IsSelectedCellsWithSameModalityOrEmptyCell(Modality.PT))
            {
                AddPtCustomMenuItem();
            }
            else
            {
                AddCtCustomMenuItem();
                AddPtCustomMenuItem();
            }
        }

        public void RefreshPtWindowInfo()
        {
            try
            {
                //var headTB = Printers.Instance.HeadTB.Split('|');
                //var bodyTB = Printers.Instance.BodyTB.Split('|');

                foreach (var film in Card.EntityFilmingPageList)
                {
                    var cells = film.Cells;
                    //var config = film.filmingViewerControl.Configuration;
                    double oldHeadT = film.filmingViewerControl.Configuration.BasicConfig.TBValue.HeadT;
                    double oldHeadB = film.filmingViewerControl.Configuration.BasicConfig.TBValue.HeadB;
                    double oldBodyT = film.filmingViewerControl.Configuration.BasicConfig.TBValue.BodyT;
                    double oldBodyB = film.filmingViewerControl.Configuration.BasicConfig.TBValue.BodyB;
                    film.filmingViewerControl.Controller.LoadConfigReader();
                    //config.HeadT = Double.Parse(headTB[0]);
                    //config.HeadB = Double.Parse(headTB[1]);
                    //config.BodyT = Double.Parse(bodyTB[0]);
                    //config.BodyB = Double.Parse(bodyTB[1]);
                    bool isDisplay = Card.DisplayedFilmPage.Contains(film);
                    foreach (var cell in cells.Where(cell => null != cell
                                                             && !cell.IsEmpty
                                                             && null != cell.Image
                                                             && null != cell.Image.CurrentPage
                                                             && Modality.PT == cell.Image.CurrentPage.Modality))
                    {
                        var displayData = cell.Image.CurrentPage;
                        var petExtension = displayData.DisplayDataExtension as PETExtension;
                        var imageHeader = cell.Image.CurrentPage.ImageHeader;
                        string bodyPartExamined = "WHOLEBODY";
                        if (imageHeader.DicomHeader.ContainsKey(ServiceTagName.BodyPartExamined))
                        {
                            imageHeader.DicomHeader.TryGetValue(ServiceTagName.BodyPartExamined, out bodyPartExamined);
                        }
                        if (petExtension != null)
                        {
                            petExtension.SetSUVBound(cell.ViewerControlSetting.Configuration, imageHeader);
                            if(petExtension.CanConvertToTargetUnit(" SUV bw"))
                            {
                                if (bodyPartExamined == null || bodyPartExamined.ToUpper() != "HEAD")
                                {
                                    if (oldBodyT != PETExtension.DefaultT || oldBodyB != PETExtension.DefaultB)
                                    {
                                        var suvT = UnitManager.ConvertSUVToSpecialValue(" SUV bw", PETExtension.DefaultT, displayData);
                                        var suvB = UnitManager.ConvertSUVToSpecialValue(" SUV bw", PETExtension.DefaultB, displayData);
                                        var setT = UnitManager.ConvertValueByUnitFromRescaledValue(displayData.Unit,
                                            suvT, displayData);
                                        var setB = UnitManager.ConvertValueByUnitFromRescaledValue(displayData.Unit,
                                            suvB, displayData);
                                        petExtension.SetTBValue(setT, ServiceTagName.T);
                                        petExtension.SetTBValue(setB, ServiceTagName.B);
                                    }
                                }
                                else
                                {
                                    if (oldHeadT != PETExtension.DefaultT || oldHeadB != PETExtension.DefaultB)
                                    {
                                        var suvT = UnitManager.ConvertSUVToSpecialValue(" SUV bw", PETExtension.DefaultT, displayData);
                                        var suvB = UnitManager.ConvertSUVToSpecialValue(" SUV bw", PETExtension.DefaultB, displayData);
                                        var setT = UnitManager.ConvertValueByUnitFromRescaledValue(displayData.Unit,
                                            suvT, displayData);
                                        var setB = UnitManager.ConvertValueByUnitFromRescaledValue(displayData.Unit,
                                            suvB, displayData);
                                        petExtension.SetTBValue(setT, ServiceTagName.T);
                                        petExtension.SetTBValue(setB, ServiceTagName.B);
                                    }
                                }
                            }
                            petExtension.UpdateTBValueWhenExtensionChange();
                            //if (displayData.WindowLevel.Count() == 1 || displayData.PSXml == string.Empty)
                            //    displayData.WindowLevel[0] = displayData.PState.WindowLevel;
                        }
                        if (isDisplay)
                        {
                            //  cell.Refresh();
                            Card.Dispatcher.Invoke(new Action<MedViewerControlCell>((curcell) => curcell.Refresh()),
                                                   cell);
                        }
                        else
                        {
                            //cell.Image.CurrentPage.IsDirty = true;
                            Card.Dispatcher.BeginInvoke(
                                new Action<MedViewerControlCell>((curcell) => curcell.Refresh()), cell);
                        }

                    }
                }
                if (Card.IfZoomWindowShowState)
                {
                    var zoomViewer = Card.zoomWindowGrid.Children[0] as FilmingZoomViewer;
                    if (zoomViewer != null && zoomViewer.DisplayedZoomCell != null)
                    {
                        zoomViewer.ctrlZoomViewer.Controller.LoadConfigReader();
                        zoomViewer.RefreshDisplayCell();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private Thread _preSetWindowingThread;
        private ManualResetEvent _preSetPETWLmanualResetEvent;
        private WindowLevelInfo _windowLevelInfo;
        private void PreSetWindowingThread()
        {
            while (true)
            {
                _preSetPETWLmanualResetEvent.WaitOne();
                _preSetPETWLmanualResetEvent.Reset();

                var displayedPages = new List<FilmingPageControl>();
                for (int i = 0; i < Card.SelectedFilmCardDisplayMode; i++)
                {
                    var pageIndex = Card.CurrentFilmPageBoardIndex * Card.SelectedFilmCardDisplayMode + i;
                    if (Card.EntityFilmingPageList.Count <= pageIndex)
                    {
                        break;
                    }
                    displayedPages.Add(Card.EntityFilmingPageList[pageIndex]);
                }

                var displayedCells = displayedPages.SelectMany(p => p.Cells);

                foreach (var displayedCell in displayedCells)
                {
                    if (null != displayedCell
                        && displayedCell.IsSelected
                        && !displayedCell.IsEmpty
                        && null != displayedCell.Image
                        && null != displayedCell.Image.CurrentPage
                        && Modality.PT == displayedCell.Image.CurrentPage.Modality)
                    {
                        Card.SetTBValue(displayedCell, this._windowLevelInfo.WindowLevel, false);
                        Card.Dispatcher.Invoke(new Action(() =>
                        {
                            try
                            {
                                displayedCell.Image.CurrentPage.PState.
                                    CanRaiseWLValueChanged = true;
                                displayedCell.Refresh();
                            }
                            catch (Exception e)
                            {
                                Logger.LogError(e.Message + e.StackTrace);
                            }
                        }));
                    }
                }

                foreach (var film in Card.ActiveFilmingPageList)
                {
                    var cells = film.SelectedCells();
                    foreach (var cell in cells.Where(cell => null != cell
                                                             && !cell.IsEmpty
                                                             && null != cell.Image
                                                             && null != cell.Image.CurrentPage
                                                             && Modality.PT == cell.Image.CurrentPage.Modality))
                    {
                        Card.SetTBValue(cell, _windowLevelInfo.WindowLevel, false);
                    }
                }

                Card.Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (var film in Card.ActiveFilmingPageList)
                    {
                        var cells = film.SelectedCells();
                        foreach (var cell in cells.Where(cell => null != cell
                                                                 && !cell.IsEmpty
                                                                 && null != cell.Image
                                                                 && null != cell.Image.CurrentPage
                                                                 && Modality.PT == cell.Image.CurrentPage.Modality))
                        {
                            cell.Image.CurrentPage.PState.CanRaiseWLValueChanged = true;
                        }
                    }
                }));

            }
        }

        private void SetWindowLevelForSelectedCells1(String selectPreset)
        {
            try
            {
                Logger.Instance.LogPerformanceRecord("[Action][WindowLevel][Begin]");

                Logger.LogFuncUp();

                WindowLevelInfo winLevelInfo;
                if (presetWindowInfos.TryGetValue(selectPreset, out winLevelInfo))
                {
                    if (winLevelInfo.Modality == Modality.CT)
                    {
                        foreach (var film in Card.ActiveFilmingPageList)
                        {
                            var cells = film.SelectedCells();
                            foreach (var cell in cells)
                            {
                                Card.SetWindowLevel(cell, winLevelInfo.WindowLevel);
                            }
                        }
                        if (Card.IfZoomWindowShowState)
                        {
                            var zoomViewer = Card.zoomWindowGrid.Children[0] as FilmingZoomViewer;
                            if (zoomViewer != null)
                            {
                                zoomViewer.RefreshDisplayCell();
                            }
                        }
                    }
                    else if (winLevelInfo.Modality == Modality.PT)
                    {
                        if (null == _preSetWindowingThread && null == _preSetPETWLmanualResetEvent)
                        {
                            _preSetPETWLmanualResetEvent = new ManualResetEvent(false);
                            _preSetWindowingThread = new Thread(PreSetWindowingThread);
                            _preSetWindowingThread.Start();
                        }

                        this._windowLevelInfo = winLevelInfo;
                        this._preSetPETWLmanualResetEvent.Set();

                        if (Card.IfZoomWindowShowState)
                        {
                            var zoomViewer = Card.zoomWindowGrid.Children[0] as FilmingZoomViewer;
                            if (zoomViewer != null&&zoomViewer.DisplayedZoomCell!=null)
                            {
                                Card.SetTBValue(zoomViewer.DisplayedZoomCell, winLevelInfo.WindowLevel,true);
                                zoomViewer.DisplayedZoomCell.Refresh();
                            }
                        }
                    }
                }

                Logger.LogFuncDown();
                Logger.Instance.LogPerformanceRecord("[Action][WindowLevel][End]");

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);

            }
        }

        private void ResetPresetWindowInfo(WindowingProtocolsConfigInfo sourcePresetWindowInfo)
        {
            Card.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    if (sourcePresetWindowInfo == null) return;
                    foreach (var windowingInfo in sourcePresetWindowInfo.Items)
                    {
                        if (windowingInfo.Name == string.Empty) continue;
                        var tempWinInfo = windowingInfo;
                        var menuItem = new AppMenuItem();
                        menuItem.Header = windowingInfo.Name;
                        menuItem.RecognizesAccessKey = false;
                        RelayCommand relayCommand = null;
                        if (sourcePresetWindowInfo.Modality == Modality.CT)
                        {
                            relayCommand = new RelayCommand(param => SetWindowLevelForSelectedCells1("ct" + tempWinInfo.Name));
                        }
                        else if (sourcePresetWindowInfo.Modality == Modality.PT)
                        {
                            relayCommand = new RelayCommand(param => SetWindowLevelForSelectedCells1("pt" + tempWinInfo.Name));
                        }
                        menuItem.Command = relayCommand;
                        menuItem.InputGestureText = windowingInfo.InputGestureText;
                        AutomationProperties.SetAutomationId(menuItem, createPresetWindowAutoID(windowingInfo.Name.ToUpper()));
                        menuItem.IsCheckable = true;
                        menuItem.Checked -= OnPreSetWindowMenuItemChecked;
                        menuItem.Checked += OnPreSetWindowMenuItemChecked;
                        if (sourcePresetWindowInfo.Modality == Modality.PT)
                        {
                            //  menuItem.SetBinding(Card.IsEnabledProperty, new Binding() { Path = new PropertyPath("IsSetSUVbwChecked") });
                        }

                        AddKeyBindings(windowingInfo, relayCommand);
                        if (sourcePresetWindowInfo.Modality == Modality.CT)
                        {
                            presetWindowInfos.Add("ct" + windowingInfo.Name, new WindowLevelInfo(new WindowLevel(windowingInfo.WC, windowingInfo.WW), sourcePresetWindowInfo.Modality));
                            ctPresetWindowMenuItem.Items.Add(menuItem);
                        }
                        else if (sourcePresetWindowInfo.Modality == Modality.PT)
                        {
                            presetWindowInfos.Add("pt" + windowingInfo.Name, new WindowLevelInfo(new WindowLevel(windowingInfo.WC, windowingInfo.WW), sourcePresetWindowInfo.Modality));
                            ptPresetWindowMenuItem.Items.Add(menuItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogFuncException(ex.Message + ex.StackTrace);
                }
            }));

        }

        public void OnPreSetWindowMenuItemChecked(object sender, RoutedEventArgs e)
        {
            var checkedMenu = sender as MenuItem;
            if (checkedMenu == null) return;
            if (ctPresetWindowMenuItem.Items.Contains(checkedMenu))
            {
                foreach (var item in ctPresetWindowMenuItem.Items)
                {
                    var mnuItem = item as AppMenuItem;
                    if (mnuItem == null) continue;
                    mnuItem.RecognizesAccessKey = false;
                    if ((mnuItem.Header as String) != (checkedMenu.Header as String) && mnuItem.IsCheckable == true) mnuItem.IsChecked = false;
                }
            }
            if (ptPresetWindowMenuItem.Items.Contains(checkedMenu))
            {
                foreach (var item in ptPresetWindowMenuItem.Items)
                {
                    var mnuItem = item as AppMenuItem;
                    if (mnuItem == null) continue;
                    mnuItem.RecognizesAccessKey = false;
                    if ((mnuItem.Header as String) != (checkedMenu.Header as String) && mnuItem.IsCheckable == true) mnuItem.IsChecked = false;
                }
            }
        }

        public void UpdatePreSetWindowCheckedStatus()
        {
            InitializePresetWindowInfo();
            foreach (var item in ctPresetWindowMenuItem.Items)
            {
                var mnuItem = item as AppMenuItem;
                if (mnuItem == null) continue;
                mnuItem.RecognizesAccessKey = false;
                WindowLevelInfo winLevelInfo;
                var header = mnuItem.Header as String;
                if (header == null) continue;
                if (presetWindowInfos.TryGetValue("ct" + header, out winLevelInfo) && winLevelInfo.Modality == Modality.CT)
                {
                    if (Card.ActiveFilmingPageList.All(page => page.SelectedCells().All(
                     cell => (cell != null) && (cell.IsEmpty || (cell.Image != null && cell.Image.CurrentPage != null && cell.Image.CurrentPage.Modality != Modality.CT)
                         || (cell.Image != null && cell.Image.CurrentPage != null && cell.Image.CurrentPage.IsRGB)
                         || (!cell.IsEmpty && cell.Image != null && cell.Image.CurrentPage != null && cell.Image.CurrentPage.PState.WindowLevel == winLevelInfo.WindowLevel)))))
                    {
                        mnuItem.IsChecked = true;
                        break;
                    }
                    else
                    {
                        if (mnuItem.IsChecked)
                            mnuItem.IsChecked = false;
                    }
                }
            }

            foreach (var item in ptPresetWindowMenuItem.Items)
            {
                var mnuItem = item as AppMenuItem;
                if (mnuItem == null) continue;
                mnuItem.RecognizesAccessKey = false;
                if (!IsSetSUVbwChecked)
                {
                    mnuItem.IsChecked = false;
                    mnuItem.IsEnabled = false;
                    continue;
                }
                WindowLevelInfo winLevelInfo;
                var header = mnuItem.Header as String;
                if (header == null) continue;
                if (presetWindowInfos.TryGetValue("pt" + header, out winLevelInfo) && winLevelInfo.Modality == Modality.PT && winLevelInfo.WindowLevel != null)
                {
                    if (Card.ActiveFilmingPageList.All(page => page.SelectedCells().All(
                         cell => (cell != null) && (cell.IsEmpty || (cell.Image != null && cell.Image.CurrentPage != null && cell.Image.CurrentPage.Modality != Modality.PT)
                             || (cell.Image != null && cell.Image.CurrentPage != null && cell.Image.CurrentPage.IsRGB)
                             || (!cell.IsEmpty && cell.Image != null && cell.Image.CurrentPage != null && cell.Image.CurrentPage.Modality == Modality.PT
                             && (cell.Image.CurrentPage.DisplayDataExtension as PETExtension) != null
                             && DoubleUtil.AreClose((cell.Image.CurrentPage.DisplayDataExtension as PETExtension).GetTBValue().Item1, winLevelInfo.WindowLevel.WindowWidth)
                             && DoubleUtil.AreClose((cell.Image.CurrentPage.DisplayDataExtension as PETExtension).GetTBValue().Item2, winLevelInfo.WindowLevel.WindowCenter))))))
                    {
                        mnuItem.IsChecked = true;
                        break;
                    }
                    else
                    {
                        if (mnuItem.IsChecked)
                            mnuItem.IsChecked = false;
                    }
                }
            }

        }

        public void FilmPageGridContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            //todo: performance optimization begin (remove group action transmit)
            //2015.02.10 fix bug 457551, force end context menu on Dynamic Graphics
            //if (ActiveFilmingPageList.Any(p => p.IsGroupMouseMove))
            //if(ActiveFilmingPageList.Any(p=>!p.filmingViewerControl.CanOpenContextMenu()))
            Point currentPt = Mouse.GetPosition(this.Card.filmPageGrid);
            Card._dropFilmingPage = null;
            Card._dropViewCell = null;
            VisualTreeHelper.HitTest(Card.filmPageGrid,
                                     Card.FilmingPageHitTestFilter,
                                     Card.FilmingPageHitTestResult,
                                     new PointHitTestParameters(currentPt));
            if (Card._dropFilmingPage == null || !Card._dropFilmingPage.filmingViewerControl.CanOpenContextMenu())
            {
                e.Handled = true;
                //ActiveFilmingPageList.ForEach(p => p.IsGroupMouseMove = false);
                Card.MouseGestureButton = MouseButton.Left;
                return;
            }

            UpdatePreSetWindowCheckedStatus();
            UpdatePreSetWindowVisibility();
            UpdateCheckedAndEnabledStatus();
            UpdateFourConverCheckedStatus();
            UpdateScaleRulerStatus();
            UpdateMutiCellStatus();
        }

        private void UpdateMutiCellStatus()
        {
            if (filmingContextMenuDivideCell == null) return;
            if (IsEnableUndoDivideCell)
            {
                filmingContextMenuDivideCell.Visibility = Visibility.Collapsed;
                filmingContextMenuUndoDivideCell.Visibility = Visibility.Visible;
            }
            else
            {
                filmingContextMenuDivideCell.Visibility = Visibility.Visible;
                filmingContextMenuUndoDivideCell.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateScaleRulerStatus()
        {
            if (filmingContextMenuScaleRuler == null) return;
            var isEnabled = true;
            if (Card.IsCellModalitySC)
            {
                isEnabled = false;
            }
            else
            {
                if (Card.EntityFilmingPageList == null)
                {
                    isEnabled = false;
                }
                else
                {
                    var isShowRuler = false;
                    foreach (var filmPage in Card.EntityFilmingPageList)
                    {
                        foreach (var cell in filmPage.Cells)
                        {
                            var page = cell.Image.CurrentPage;
                            if (page != null)
                            {
                                var isNotShowRuler = (page.PixelSizeY == 0 || page.IsTableOrCurve);
                                if (!isNotShowRuler)
                                {
                                    isShowRuler = true;
                                    break;
                                }

                            }
                        }
                    }

                    if (!isShowRuler)
                    {
                        isEnabled = false;
                    }
                }

            }

            filmingContextMenuScaleRuler.IsChecked = isEnabled && Card.commands.IfShowImageRuler;
            filmingContextMenuScaleRuler.IsEnabled = isEnabled;

        }

        private void UpdateCheckedAndEnabledStatus()
        {
            var fourConverItem = GetContextMenuItem(Card.Resources["UID_Filming_AnnotationDisplayType_Partial"].ToString());
            if (fourConverItem != null)
                fourConverItem.IsChecked = IsAnnotationCustomizationChecked;

           // var localizedImageReferenceLineItem = GetContextMenuItem(Card.Resources["UID_Filming_ContextMenu_LocalizedImageReferenceLine"].ToString());
            if (!isModalityMG)
            {
                filmingContextMenuLocalizedImageReferenceLine.IsEnabled = IsLocalizedImageReferenceLineEnabled;
                filmingContextMenuLocalizedImageReferenceLine.Checked -= OnLocalizedImageReferenceLineChecked;
                filmingContextMenuLocalizedImageReferenceLine.IsChecked = IsLocalizedImageReferenceLineChecked;
                filmingContextMenuLocalizedImageReferenceLine.Checked += OnLocalizedImageReferenceLineChecked;

                filmingContextMenuManualLocalizedImage.IsEnabled = IsLocalizedImageReferenceLineEnabled;
                filmingContextMenuManualReferenceLine.IsEnabled = Card.BtnEditCtrl.IsEnableInsertRefImage;
            }

            filmingContextMenuRepack.IsEnabled = IsRepackMenuEnable;
            configMenuItem.IsEnabled = !IsCellModalitySC;

        }

        private void UpdatePreSetWindowVisibility()
        {
            ctPresetWindowMenuItem.Visibility = CTPreSetWindowingVisibility;
            ptPresetWindowMenuItem.Visibility = PTPreSetWindowingVisibility;
            suvMenuItem.Visibility = PreSUVTypeSettingVisibility;
            InitSuvMenuItems();
        }

        private void UpdateFourConverCheckedStatus()
        {
            filmingContextMenuAnnotationCustomization.IsChecked = IsAnnotationCustomizationChecked;
        }

        //fix dim 159949   2012-09-29
        private void FilmPageGridContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            foreach (var film in Card.EntityFilmingPageList)
            {
                //film.filmingViewerControl.IsContextMenuOpen = false;
            }
        }

        #region  [多分格][edit by jinyang.li]

        private ICommand _insertOrUndoDivideCellCommand;

        public ICommand InsertDivideCellCommand
        {
            get
            {
                return _insertOrUndoDivideCellCommand ?? (_insertOrUndoDivideCellCommand = new RelayCommand(param => ShowDivideCellWindow(), param => (IsEnableDivideCell)));
            }
        }

        //private ICommand _divideCellCommand;

        //public ICommand DivideCellCommand
        //{
        //    get
        //    {
        //        return _divideCellCommand ?? (_divideCellCommand = new RelayCommand(
        //                                                              param => ShowDivideCellWindow(),
        //                                                                param =>
        //                                                             (IsEnableDivideCell)));
        //    }
        //}

        private void ShowDivideCellWindow()
        {
            var messageWindow = new MessageWindow
            {
                WindowTitle = Card.Resources["UID_Filming_MultiFormat_Title"] as string,
                WindowChild = Card.MultiFormatLayoutWindow,
                WindowDisplayMode = WindowDisplayMode.Default
            };
            Card.HostAdornerCount++;
            messageWindow.ShowModelDialog();
            Card.HostAdornerCount--;
        }

        private bool IsEnableDivideCell
        {
            get
            {
                try
                {
                    if (Card._filmingCardModality == FilmingUtility.EFilmModality)
                        return false;

                    if (!Card.IsSuccessiveSelected()) return false;

                    if (Card.ActiveFilmingPageList.Any(f => f.ViewportLayout.LayoutType == LayoutTypeEnum.DefinedLayout))
                        return false;

                    foreach (var film in Card.ActiveFilmingPageList)
                    {
                        foreach (var cell in film.SelectedCells())
                        {
                            if (cell != null)
                            {
                                //var medviewerLayoutCell = cell.ParentCell as MedViewerLayoutCell;
                                //if (medviewerLayoutCell == null) continue;
                                //if (film.ViewportLayout.LayoutType != LayoutTypeEnum.RegularLayout) return false;
                                //if (medviewerLayoutCell.Rows != 1 || medviewerLayoutCell.Columns != 1) return false;

                                var layoutCell = cell.ParentCell as FilmingLayoutCell;
                                if (layoutCell == null) continue;
                                if (layoutCell.IsMultiformatLayoutCell) return false;
                                if (layoutCell.Rows != 1 || layoutCell.Columns != 1) return false;
                            }
                        }
                    }


                    return true;
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex.StackTrace);
                    return false;
                }
            }
        }
       

        public bool DivideCell()
        {
            try
            {
                Logger.LogFuncUp();

                if (Card.IfZoomWindowShowState)
                {
                    Card.ZoomViewer.CloseDialog();
                }
                //step1:    找到多分格的对象和多分格的位置
                int row = Card.MultiFormatLayoutWindow.SelectedMultiFormatLayout.LayoutRowsSize;
                int column = Card.MultiFormatLayoutWindow.SelectedMultiFormatLayout.LayoutColumnsSize;

                FilmingControlCell firstSelectedCell = null;
                FilmingPageControl firstSelectedFilmingPageControl = null;
                var newCellList = new List<FilmingControlCell>();

                var sortedActiveFilmingPageList = Card.ActiveFilmingPageList.OrderBy(a => a.FilmPageIndex).ToList();

                foreach (var filmingPage in sortedActiveFilmingPageList)
                {
                    List<MedViewerControlCell> selectCellsOFflimingPage = new List<MedViewerControlCell>();
                    selectCellsOFflimingPage.AddRange(filmingPage.SelectedCells());
                    if (sortedActiveFilmingPageList.First() == filmingPage)
                    {
                        firstSelectedCell = selectCellsOFflimingPage.FirstOrDefault() as FilmingControlCell;
                        firstSelectedFilmingPageControl = filmingPage;
                    }

                    foreach (var cell in selectCellsOFflimingPage)
                    {
                        //if (null == cell
                        //    || cell.IsEmpty
                        //    || null == cell.Image
                        //    || null == cell.Image.CurrentPage)
                        //{
                        //    continue;
                        //}

                        var newCell = new FilmingControlCell();
                        if (!cell.IsEmpty)
                        {
                            newCell.Image.AddPage(cell.Image.CurrentPage);
                            cell.Image.Clear();
                            cell.Refresh();
                        }
                        newCellList.Add(newCell);
                    }
                }

                //step2:    计算多分格的个数,并做多分格
                if (null != firstSelectedCell
                    && null != firstSelectedFilmingPageControl
                    && newCellList.Count > 0)
                {

                    double needDivideCellCount = Math.Ceiling((newCellList.Count * 1.0) / (row * column));

                    var parentCell = firstSelectedCell.ParentCell as MedViewerLayoutCell;
                    if (null == parentCell) return false;

                    int availableSequenceParentCellsCount = (int)needDivideCellCount;// 0;
                    //var result = FindAvailableSequenceParentCellsCount(parentCell, (int)needDivideCellCount, out availableSequenceParentCellsCount);
                    //if (result)
                    //{
                    SetDivideCell(firstSelectedFilmingPageControl,
                                  parentCell,
                                  newCellList,
                                  row,
                                  column,
                                  availableSequenceParentCellsCount);

                    if(Card.IsEnableRepack)
                        this.Card.contextMenu.Repack();
                    else
                    {
                        Card.EnableUI();
                    }
                    Logger.LogFuncDown();
                    return true;
                    //}
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            finally
            {
                Card.UpdateButtonStatus();
            }
            return false;
        }

        /// <summary>
        /// 查找可以进行多分格的数量。
        /// </summary>
        /// <param name="parentCell">第一个进行多分格的Cell</param>
        /// <param name="needSequenceParentCellsCount">预估计的需要进行多分格的Cell数量</param>
        /// <param name="availableSequenceParentCellsCount">可用的可以进行多分格的Cell数量</param>
        /// <returns></returns>
        private bool FindAvailableSequenceParentCellsCount(MedViewerLayoutCell parentCell,
                                                                                               int needSequenceParentCellsCount,
                                                                                               out int availableSequenceParentCellsCount)
        {
            availableSequenceParentCellsCount = 0;
            //test - do
            if (null == parentCell || 0 == needSequenceParentCellsCount)
            {
                //System.Console.WriteLine("《FindAvailableSequenceParentCellsCount》《Test--Do》");
                return false;
            }

            //step1:查找剩余的Cell的数量
            int parentCellInex = parentCell.ParentCell.Children.ToList().IndexOf(parentCell);
            int retainParentCellsCount = parentCell.ParentCell.Children.ToList().Count - parentCellInex;

            availableSequenceParentCellsCount = retainParentCellsCount >= needSequenceParentCellsCount ? needSequenceParentCellsCount : retainParentCellsCount;

            //step2:在剩余的Cell中删除掉不能进行多分格的Cell
            MedViewerLayoutCell firstUnavailableParentCell = null;
            for (int i = 0; i < availableSequenceParentCellsCount; i++)  //?
            {
                var nextParentCell =
                    parentCell.ParentCell.Children.ToList().ElementAt(parentCellInex + i) as MedViewerLayoutCell;
                if (null != nextParentCell
                    && 1 == nextParentCell.Rows
                    && 1 == nextParentCell.Columns
                    )
                {
                    continue;
                }
                firstUnavailableParentCell = nextParentCell;
                break;
            }

            if (null == firstUnavailableParentCell)
            {
                return true;
            }
            else
            {
                int lastAvalibleParentCellIndex = parentCell.ParentCell.Children.ToList().IndexOf(firstUnavailableParentCell) - 1;
                availableSequenceParentCellsCount = parentCell.ParentCell.Children.Count() - lastAvalibleParentCellIndex;
                return true;
            }
        }

        /// <summary>
        /// 设置多分格。如果按传进来的Row和Column不能应付多分格，则该函数自动增加Row和Column。
        /// </summary>
        /// <param name="filmingPageControl"></param>
        /// <param name="firstParentCell"></param>
        /// <param name="newCellList"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="realNeedDivideCellCount"></param>
        /// <returns></returns>
        private bool SetDivideCell(FilmingPageControl filmingPageControl,
                                                        MedViewerLayoutCell firstParentCell,
                                                        List<FilmingControlCell> newCellList,
                                                        int rows,
                                                        int columns,
                                                        int realNeedDivideCellCount)    //?这个参数做什么用的?
        {

            //test - do
            if (null == filmingPageControl
                || null == firstParentCell
                || 0 == newCellList.Count
                || 0 == realNeedDivideCellCount)
            {
                return false;
            }
            Card.ActiveFilmingPageList.UnSelectAllCells();
            ApplyDivideCell(filmingPageControl,
                                   firstParentCell,
                                   newCellList,
                                   rows,
                                   columns,
                                   realNeedDivideCellCount);

            return true;
        }
        /// <summary>
        ///  根据最终的Row和Column，以及Cell数量设置多分格
        /// </summary>
        /// <param name="filmingPageControl"></param>
        /// <param name="firstParentCell"></param>
        /// <param name="newCellList"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="realNeedDivideCellCount"></param>
        private void ApplyDivideCell(FilmingPageControl filmingPageControl,
                                                    MedViewerLayoutCell firstParentCell,
                                                    List<FilmingControlCell> newCellList,
                                                    int rows,
                                                    int columns,
                                                    int realNeedDivideCellCount)
        {
            //test - do
            if (null == filmingPageControl
                || null == firstParentCell
                || 0 == newCellList.Count
                || 0 == realNeedDivideCellCount)
            {
                return;
            }

            var divideCellFilm = filmingPageControl;
            List<DisplayData> retainDisplayData = new List<DisplayData>();

            var rootCell = firstParentCell.ParentCell;
            int firstParentCellIndex = rootCell.Children.ToList().IndexOf(firstParentCell);
            int newCellIndex = 0;
            for (int i = 0; i < realNeedDivideCellCount; i++)
            {//
                var cellIndex = firstParentCellIndex + i;
                if (cellIndex >= rootCell.Children.Count())
                {
                    realNeedDivideCellCount -= i;
                    i = 0;
                    divideCellFilm = Card.EntityFilmingPageList.ElementAt(divideCellFilm.FilmPageIndex + 1);
                    rootCell = divideCellFilm.RootCell;
                    firstParentCellIndex = 0;
                    cellIndex = 0;
                }
                var nextParentCell = rootCell.Children.ElementAt(cellIndex) as FilmingLayoutCell;
                if (null == nextParentCell)
                {
                    continue;
                }

                //多分格只支持1X1
                if (firstParentCell != nextParentCell && nextParentCell.ControlCells[0].Image.Count > 0)
                {
                    retainDisplayData.Add(nextParentCell.ControlCells[0].Image.CurrentPage);
                }

                nextParentCell.RemoveAll();
                nextParentCell.SetLayout(rows, columns);
                for (int j = 0; j < rows * columns; j++)
                {
                    nextParentCell.AddCell(new FilmingControlCell());
                }
                nextParentCell.Refresh();   //mouse button event can be registered
                Card._loadingTargetPage = divideCellFilm;
                Card._loadingTargetCellIndex = (nextParentCell.Children.First() as MedViewerControlCell).CellIndex;
                for (int j = 0; j < rows * columns; j++)
                {
                    if (newCellIndex < newCellList.Count)
                    {

                        var cell = newCellList.ElementAt(newCellIndex);
                        cell.IsSelected = true;

                        Card.ReplaceCell(cell, cell.IsSelected);
                        Card._loadingTargetCellIndex++;
                        //ImgTxtDisplayState state = GetImgTxtDisplayState(cell); 
                        //filmingPageControl.AddControlCellToLayoutCell(nextParentCell, cell);
                        //FilmingHelper.UpdateCornerTextForCell(cell, state);
                        newCellIndex++;
                    }
                    else
                    {
                        var cell = new FilmingControlCell();
                        //filmingPageControl.AddControlCellToLayoutCell(nextParentCell, cell);
                        Card.ReplaceCell(cell);
                    }
                }
                nextParentCell.Refresh();
                //foreach (var cell in nextParentCell.ControlCells)
                //{
                //    filmingPageControl.OnNewCellAdded(null, new MedViewerEventArgs(cell));
                //}
                //if (null != nextParentCell.Control)
                //{
                //    (nextParentCell.Control as MedViewerLayoutCellImpl).Border.Stroke = new SolidColorBrush(Colors.Red);
                //}
                nextParentCell.IsMultiformatLayoutCell = true;
            }
            filmingPageControl.IsSelected = true;


            filmingPageControl.ReBuildViewportList();
        }
        #endregion

        #region  [撤销多分格][edit by jinyang.li]
        private ICommand _undoDivideCellCommand;

        public ICommand UndoDivideCellCommand
        {
            get
            {
                return _undoDivideCellCommand ?? (_undoDivideCellCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         UndoDivideCell();
                                                                     },
                                                                     param =>
                                                                     (IsEnableUndoDivideCell)));
            }
        }

        private bool IsEnableUndoDivideCell
        {
            get
            {
                try
                {
                    return Card.ActiveFilmingPageList.Any(
                        film => film.RootCell.Children.OfType<FilmingLayoutCell>().Any(c => c.IsSelected() && c.IsMultiformatLayoutCell));
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex.StackTrace);
                    return false;
                }

            }
        }

        private void UndoDivideCell()
        {
            try
            {
                Logger.LogFuncUp();


                FilmingPageControl targetFilmingPage = null;
                FilmingLayoutCell targetLayoutCell = null;

                var selectedControlCells = Card.ActiveFilmingPageList.SelectMany(film => film.SelectedCells()).ToList();

                while (GetASelectedMultiFormatCell(ref targetFilmingPage, ref targetLayoutCell))
                    UndoDivideACell(targetFilmingPage, targetLayoutCell);

                selectedControlCells.ForEach(c => c.IsSelected = true);

                foreach (var film in Card.EntityFilmingPageList.Where(film => !film.IsSelected).Where(film => film.Cells.Any(c => c.IsSelected)))
                {
                    film.IsSelected = true;
                }

                if (Card.IsEnableRepack) this.Card.contextMenu.Repack();
                else
                {
                    Card.ReOrderCurrentFilmPageBoard();

                    Card.EntityFilmingPageList.UpdatePageLabel();
                }


                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }

        }

        private bool GetASelectedMultiFormatCell(ref FilmingPageControl targetFilmingPage, ref FilmingLayoutCell targetLayoutCell)
        {
            foreach (var activeFilmingPage in Card.ActiveFilmingPageList.OrderBy(a => a.FilmPageIndex))
            {
                foreach (var cell in activeFilmingPage.SelectedCells())
                {
                    var medViewerLayoutCell = cell.ParentCell as FilmingLayoutCell;
                    if (medViewerLayoutCell == null) continue;
                    if (medViewerLayoutCell.Control == null) continue;
                    if (!medViewerLayoutCell.IsMultiformatLayoutCell) continue;
                    targetLayoutCell = medViewerLayoutCell;
                    targetFilmingPage = activeFilmingPage;
                    return true;
                }
            }
            return false;
        }

        private void UndoDivideACell(FilmingPageControl targetFilmingPage, FilmingLayoutCell targetLayoutCell)
        {
            //1.Backup layoutCells
            var pages = Card.GetLinkedPage(targetFilmingPage);
            var layoutCells = pages.SelectMany(p => p.RootCell.Children).OfType<FilmingLayoutCell>().ToList();
            var startIndex = layoutCells.IndexOf(targetLayoutCell);
            if (startIndex > 0) layoutCells.RemoveRange(0, startIndex);

            //2.Rearrange cells
            List<MedViewerCellBase> rearrangedLayoutCells = layoutCells.OfType<MedViewerCellBase>().ToList();
            var selectedMultiFormatCells = rearrangedLayoutCells.OfType<FilmingLayoutCell>().Where(c => c.IsMultiformatLayoutCell && c.IsSelected()).ToList();
            //int newCellCount = 0;
            foreach (var selectedMultiFormatCell in selectedMultiFormatCells)
            {
                //1.找到需要拆出来的cell
                var multiCellIndex = rearrangedLayoutCells.IndexOf(selectedMultiFormatCell);
                var newControlCells = selectedMultiFormatCell.Children.OfType<FilmingControlCell>().ToList();
                var firstNonEmptyCellIndex = newControlCells.FindIndex(c => !c.IsEmpty);
                var lastNonEmptyCellIndex = newControlCells.FindLastIndex(c => !c.IsEmpty);
                if (firstNonEmptyCellIndex == -1)
                {
                    firstNonEmptyCellIndex = 0;
                    lastNonEmptyCellIndex = 0;
                }
                newControlCells = newControlCells.GetRange(firstNonEmptyCellIndex,
                                                           lastNonEmptyCellIndex - firstNonEmptyCellIndex + 1);

                rearrangedLayoutCells.InsertRange(multiCellIndex, newControlCells);
                rearrangedLayoutCells.Remove(selectedMultiFormatCell);
                if (!Card.IsEnableRepack)
                {
                    var beginCount = multiCellIndex + newControlCells.Count;
                    var cellNo = newControlCells.Count-1;
                    while (rearrangedLayoutCells.Count > beginCount && cellNo>0)
                    {
                        var currentCell = rearrangedLayoutCells[beginCount] as FilmingLayoutCell;
                        if (currentCell == null || currentCell.IsMultiformatLayoutCell)
                        {
                            beginCount++;
                            continue;
                        }
                        if (currentCell.IsEmpty())
                        {
                            rearrangedLayoutCells.Remove(currentCell);
                            cellNo--;
                        }
                        else
                        {
                            cellNo = 0;
                        }
                    }
                }
            }

            var newCellCount = rearrangedLayoutCells.Count - layoutCells.Count;
            var lastRearrageLayoutCell = rearrangedLayoutCells.Last() as FilmingLayoutCell;
            while (lastRearrageLayoutCell != null && !lastRearrageLayoutCell.IsMultiformatLayoutCell && lastRearrageLayoutCell.IsEmpty() && newCellCount > 0)
            {
                rearrangedLayoutCells.Remove(lastRearrageLayoutCell);
                newCellCount--;
                lastRearrageLayoutCell = rearrangedLayoutCells.Last() as FilmingLayoutCell;
            }

            var layoutCellCountOfPage = targetFilmingPage.RootCell.Children.Count();

            var newPageCount = (int)
                Math.Ceiling((0.0 + newCellCount) / layoutCellCountOfPage);
            var lastFilmPageIndex = pages.Last().FilmPageIndex;
            var layout = pages.Last().ViewportLayout;
            for (int i = 0; i < newPageCount; i++)
            {
                var page = Card.InsertFilmPage(lastFilmPageIndex + 1 + i, layout);
                layoutCells.AddRange(page.RootCell.Children.OfType<FilmingLayoutCell>());
            }

            var layoutCellCount = layoutCells.Count;
            var rearrangeLayoutCellCount = rearrangedLayoutCells.Count;
            var deltaCount = rearrangeLayoutCellCount - layoutCellCount;
            if (deltaCount > 0)
                layoutCells.RemoveRange(rearrangeLayoutCellCount - deltaCount, deltaCount);


            //4.Replace cells
            for (int i = rearrangeLayoutCellCount - 1; i >= 0; i--)
            {
                var layoutCell = layoutCells[i];
                var rearrageLayoutCell = rearrangedLayoutCells[i];
                if (layoutCell == rearrageLayoutCell) continue;
                //bool needToRegisterMouseEvent = !rearrageLayoutCell.IsMouseEventRegistered &&
                //                                layoutCell.IsMultiformatLayoutCell;
                var viewControl = layoutCell.ViewerControl;
                var film = Card.EntityFilmingPageList.GetFilmPageControlByViewerControl(viewControl);
                bool needToRegisterMouseEvent = layoutCell.ReplaceBy(rearrageLayoutCell);
                if (rearrageLayoutCell is FilmingControlCell)
                {
                    var reControlCell = rearrageLayoutCell as FilmingControlCell;
                    reControlCell.SetAction(film.CurrentActionType,MouseButton.Left);
                    needToRegisterMouseEvent = true;
                }
                if (needToRegisterMouseEvent)
                    layoutCell.Children.OfType<FilmingControlCell>().ToList().ForEach(film.RegisterEventFromCell);
                //if(!needToRegisterMouseEvent) continue;

            }

        }


        #endregion

        public bool IsSelectedCellsWithSameModalityOrEmptyCell(Modality modality)
        {
            bool result = false;
            foreach (var film in Card.ActiveFilmingPageList)
            {
                var cells = from cell in film.Cells
                            where cell.IsSelected
                            select cell;
                foreach (var cell in cells)
                {
                    if (!cell.IsEmpty)
                    {
                        var displayData = cell.Image.CurrentPage;
                        //var imageTypes = displayData.ImageType.ToArray();
                        string imageType = string.Join("\\", displayData.ImageType);
                        if (displayData.Modality != modality || imageType == FilmingUtility.EFilmImageType)
                            return false;
                    }
                    result |= !cell.IsEmpty;
                }
            }
            return result;
        }

        /// <summary>
        /// obey SSFS key:101991
        /// </summary>
        public bool IsEnableInsertEmptyCell
        {
            get
            {
                if (Card.IsEnableRepack) return false;
                if (IsCellModalitySC) return false;
                if (Card.ActiveFilmingPageList.Count != 1)
                {
                    return false;
                }

                var page = Card.ActiveFilmingPageList.First();
                if (/*page.ViewportList.Count > 1*/ page.ViewportLayout.LayoutType == LayoutTypeEnum.DefinedLayout || page.SelectedCells().Count != 1)
                {
                    return false;
                }

                if (page.IsInSeriesCompareMode)
                {
                    return false;
                }

                return true;
            }
        }

        public bool IsEnableCtSetWindowing
        {
            get
            {
                if (IsCellModalitySC) return false;
                return (Card.IsCellSelected && Card.ActiveFilmingPageList.Any(page => page.SelectedCells().Any(
                   cell => (!cell.IsEmpty && cell.Image != null && cell.Image.CurrentPage != null &&!cell.Image.CurrentPage.IsRGB
                       && cell.Image.CurrentPage.Modality == Modality.CT))));
            }
        }

        public bool IsEnablePtSetWindowing
        {
            get
            {
                if (IsCellModalitySC) return false;
                return (Card.IsCellSelected && Card.ActiveFilmingPageList.Any(page => page.SelectedCells().Any(
                 cell => (!cell.IsEmpty && cell.Image != null && cell.Image.CurrentPage != null && !cell.Image.CurrentPage.IsRGB && cell.Image.CurrentPage.Modality == Modality.PT))));
            }
        }

        public Visibility CTPreSetWindowingVisibility
        {
            get { return IsEnableCtSetWindowing ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility PTPreSetWindowingVisibility
        {
            get { return IsEnablePtSetWindowing ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool IsEnableSUVTypeSetting
        {
            get { return IsEnablePtSetWindowing; }
        }
        public Visibility PreSUVTypeSettingVisibility
        {
            get { return IsEnableSUVTypeSetting ? Visibility.Visible : Visibility.Collapsed; }
        }

        #region [--Annotation status--]

        private bool _isAnnotationAllChecked;

        public bool IsAnnotationAllChecked
        {
            get
            {
                _isAnnotationAllChecked = Card.ActiveFilmingPageList.All(page => page.SelectedCells().All(
                    cell => (!cell.IsEmpty && cell.Image.CurrentPage.PState.DisplayMode == Viewer.ImgTxtDisplayState.All)));
                return _isAnnotationAllChecked;
            }
            set
            {
                if (_isAnnotationAllChecked != value)
                {
                    _isAnnotationAllChecked = value;
                }
            }
        }

        private bool _isAnnotationNoneChecked;

        public bool IsAnnotationNoneChecked
        {
            get
            {
                _isAnnotationNoneChecked = Card.ActiveFilmingPageList.All(page => page.SelectedCells().All(
                    cell => (!cell.IsEmpty && cell.Image.CurrentPage.PState.DisplayMode == Viewer.ImgTxtDisplayState.None)));
                return _isAnnotationNoneChecked;
            }
            set
            {
                if (_isAnnotationNoneChecked != value)
                {
                    _isAnnotationNoneChecked = value;
                }
            }
        }

        private bool _isAnnotationCustomizationChecked;

        //private bool _isAnnotationCustomization2Checked;

        public bool IsAnnotationCustomizationChecked
        {
            get
            {
                _isAnnotationCustomizationChecked = !Card.ActiveFilmingPageList.All(page => page.SelectedCells().All(emptycell => emptycell.IsEmpty) || page.SelectedCells().Any(
                    cell =>
                    (!cell.IsEmpty && cell.Image.CurrentPage.PState.DisplayMode == Viewer.ImgTxtDisplayState.None)));
                return _isAnnotationCustomizationChecked;
            }
            set
            {
                if (_isAnnotationCustomizationChecked != value)
                {
                    _isAnnotationCustomizationChecked = value;
                }
            }
        }

        //public bool IsAnnotationCustomization2Checked
        //{
        //    get
        //    {
        //        _isAnnotationCustomization2Checked = Card.ActiveFilmingPageList.All(page => page.SelectedCells().All(
        //            cell =>
        //            (!cell.IsEmpty && cell.Image.CurrentPage.PState.DisplayMode == Viewer.ImgTxtDisplayState.Customization2)));
        //        return _isAnnotationCustomization2Checked;
        //    }
        //    set
        //    {
        //        if (_isAnnotationCustomization2Checked != value)
        //        {
        //            _isAnnotationCustomization2Checked = value;
        //        }
        //    }
        //}

        public bool IsEnableAnnotationAllChecked
        {
            get
            {
                if (Card.ActiveFilmingPageList.All(page => page.SelectedCells().All(cell => cell.IsEmpty))) return false;
                return !_isAnnotationAllChecked;
            }
        }

        public bool IsEnableAnnotationNoneChecked
        {
            get
            {
                if (Card.ActiveFilmingPageList.All(page => page.SelectedCells().All(cell => cell.IsEmpty))) return false;
                return !_isAnnotationNoneChecked;
            }
        }

        public bool IsEnableAnnotationCustomizationChecked
        {
            get
            {
                if (Card.ActiveFilmingPageList.All(page => page.SelectedCells().All(cell => cell.IsEmpty))) return false;
                return !_isAnnotationCustomizationChecked;
            }
        }

        #endregion

        public void Dispose()
        {
            _preSetPETWLmanualResetEvent.Dispose();
        }
    }
}
