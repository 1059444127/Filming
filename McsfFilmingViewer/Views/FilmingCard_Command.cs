using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using UIH.Mcsf.Filming.Command;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.Filming.Widgets;
using UIH.Mcsf.Viewer;
using ICommand = System.Windows.Input.ICommand;
using UIH.Mcsf.App.Common;

namespace UIH.Mcsf.Filming.Views
{
    public class FilmingCardCommand
    {
        public enum ShortCutSelect
        {
            Up,
            Down,
            Left,
            Right,
            ShiftUp,
            ShiftDown,
            ShiftLeft,
            ShiftRight
        }
        private FilmingCard Card { set;get;}

        

        public FilmingCardCommand(FilmingCard _card)
        {
            Card = _card;
            BindInputings();
        }

        public void BindInputings()
        {
            var selectAllKeyGesture = new KeyGesture(Key.A, ModifierKeys.Control);
            var selectAllCommandKeyBinding = new KeyBinding(SelectAllCommand, selectAllKeyGesture);
            Card.InputBindings.Add(selectAllCommandKeyBinding);
            

            var cutKeyGesture = new KeyGesture(Key.X, ModifierKeys.Control);
            var cutCommandKeyBinding = new KeyBinding(CutCommand, cutKeyGesture);
            Card.InputBindings.Add(cutCommandKeyBinding);

            var copyKeyGesture = new KeyGesture(Key.C, ModifierKeys.Control);
            var copyCommandKeyBinding = new KeyBinding(CopyCommand, copyKeyGesture);
            Card.InputBindings.Add(copyCommandKeyBinding);

            var pasteKeyGesture = new KeyGesture(Key.V, ModifierKeys.Control);
            var pasteCommandKeyBinding = new KeyBinding(PasteCommand, pasteKeyGesture);
            Card.InputBindings.Add(pasteCommandKeyBinding);

            var deleteCommandKeyBinding = new KeyBinding() { Command = DeleteKeyCommand, Key = Key.Delete };
            Card.InputBindings.Add(deleteCommandKeyBinding);

            var pagedownKeyBinding = new KeyBinding() { Command = PageDownCommand, Key = Key.PageDown };
            Card.InputBindings.Add(pagedownKeyBinding);

            var pageupKeyBinding = new KeyBinding() { Command = PageUpCommand, Key = Key.PageUp };
            Card.InputBindings.Add(pageupKeyBinding);

            var spaceKeyBinding = new KeyBinding() { Command = SelectSeriesCommand, Key = Key.Space };
            Card.InputBindings.Add(spaceKeyBinding);

            var upKeyBinding = new KeyBinding() { Command = SelectUpCommand, Key = Key.Up };
            Card.InputBindings.Add(upKeyBinding);

            var downKeyBinding = new KeyBinding() { Command = SelectDownCommand, Key = Key.Down };
            Card.InputBindings.Add(downKeyBinding);

            var leftKeyBinding = new KeyBinding() { Command = SelectLeftCommand, Key = Key.Left };
            Card.InputBindings.Add(leftKeyBinding);

            var rightKeyBinding = new KeyBinding() { Command = SelectRightCommand, Key = Key.Right };
            Card.InputBindings.Add(rightKeyBinding);

            var f12KeyBinding = new KeyBinding() { Command = RecoverWlCommand, Key = Key.F12 };
            Card.InputBindings.Add(f12KeyBinding);

            var upShiftKeyGesture = new KeyGesture(Key.Up, ModifierKeys.Shift);
            var upShiftCommandKeyBinding = new KeyBinding(SelectShiftUpCommand, upShiftKeyGesture);
            Card.InputBindings.Add(upShiftCommandKeyBinding);

            var downShiftKeyGesture = new KeyGesture(Key.Down, ModifierKeys.Shift);
            var downShiftCommandKeyBinding = new KeyBinding(SelectShiftDownCommand, downShiftKeyGesture);
            Card.InputBindings.Add(downShiftCommandKeyBinding);

            var leftShiftKeyGesture = new KeyGesture(Key.Left, ModifierKeys.Shift);
            var leftShiftCommandKeyBinding = new KeyBinding(SelectShiftLeftCommand, leftShiftKeyGesture);
            Card.InputBindings.Add(leftShiftCommandKeyBinding);

            var rightShiftKeyGesture = new KeyGesture(Key.Right, ModifierKeys.Shift);
            var rightShiftCommandKeyBinding = new KeyBinding(SelectShiftRightCommand, rightShiftKeyGesture);
            Card.InputBindings.Add(rightShiftCommandKeyBinding);
            
        }
        

        private ICommand _cutCommand;
        public ICommand CutCommand
        {
            get
            {
                return _cutCommand ?? (_cutCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         if (CanCutGraphics())
                                                                         {
                                                                             ExeCutGraphics();
                                                                         }
                                                                         else
                                                                         {
                                                                             CutImages();
                                                                         }
                                                                     },
                                                                     param => (IsEnableCutImage || CanCutGraphics())));
            }
        }

        private ICommand _pasteCommand;
        public ICommand PasteCommand
        {
            get
            {
                return _pasteCommand ?? (_pasteCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         Paste();
                                                                     },
                                                                     param => (IsEnablePaste)));
            }
        }

      
        private ICommand _copyCommand;
        public ICommand CopyCommand
        {
            get
            {
                
                return _copyCommand ?? (_copyCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         if (CanCopyGraphics())
                                                                         {
                                                                             ExeCopyGraphics();
                                                                         }
                                                                         else
                                                                         {
                                                                             CopyImages();
                                                                         }
                                                                     },
                                                                     param => (IsEnableCopyImage || CanCopyGraphics())));
            }
        }

        private ICommand _deleteKeyCommand;
        public ICommand DeleteKeyCommand
        {
            get
            {
                return _deleteKeyCommand ?? (_deleteKeyCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         Logger.LogWarning("delete key entered");
                                                                         //delete graphics, just directly delete
                                                                         if (_isDeleteGraphic)
                                                                         {
                                                                             DeleteSelectedGraphics();
                                                                             _isDeleteGraphic = false;
                                                                             return;
                                                                         }
                                                                         WarnWhenDeleteImages(DeleteSelectedImages);
                                                                     },
                                                                     param =>
                                                                     (Card.IsInFilmingMainWindow)));
            }
        }


        private ICommand _deleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand = new RelayCommand(
                                                               param => WarnWhenDeleteImages(DeleteSelectedImages), param => (!Card.IsCellModalitySC && Card.CollectSelectedCells().Any(c => !c.IsEmpty))));
            }
        }

        private ICommand _deleteParentCommand;
        public ICommand DeleteParentCommand
        {
            get
            {
                return _deleteParentCommand ?? (_deleteParentCommand = new RelayCommand(
                                                               param =>
                                                               {
                                                                  
                                                               },
                                                               param => (IsEnableDelete)
                                                               ));

                //DeleteSelectedImages()));
            }
        }


        public bool IsEnableScaleRuler
        {
            get { return !Card.IsCellModalitySC; }
        }

        private ICommand _selectAllCommand;
        public ICommand SelectAllCommand
        {
            //get { return _selectAllCommand ?? (_selectAllCommand = new RelayCommand(param => OnSelectAllFilmPages(null, null), param => (IsInFilmingMainWindow))); }
            get
            {
                return _selectAllCommand ?? (_selectAllCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         Logger.LogWarning("select all key entered");
                                                                         Card.BtnEditCtrl.OnSelectAllFilmPages(null, null);
                                                                     },
                                                                     param => (Card.IsInFilmingMainWindow)));
            }
        }
        
        private ICommand _scaleRulerSwitchCommand;
        public ICommand ScaleRulerSwitchCommand
        {
            get
            {
                return _scaleRulerSwitchCommand ??
                       (_scaleRulerSwitchCommand = new RelayCommand(param => SwitchScaleRuler(), param => IsEnableScaleRuler));
            }
        }

        private ICommand _showHideColorBarCommond;
        public ICommand ShowHideColorBarCommond
        {
            get
            {
                return _showHideColorBarCommond ??
                       (_showHideColorBarCommond = new RelayCommand(param => ShowHideColorBar(), param => Card.BtnEditCtrl.IsEnableSelectSeries));
            }
        }

        private ICommand _insertEmptyCellCommand;
        public ICommand InsertEmptyCellCommand
        {
            get
            {
                return _insertEmptyCellCommand ??
                       (_insertEmptyCellCommand = new RelayCommand(param => InsertEmptyCell()));
            }
        }

        private ICommand _pageDownCommand;
        public ICommand PageDownCommand
        {
            get
            {
                return _pageDownCommand ?? (_pageDownCommand = new RelayCommand(
                                                                   param =>
                                                                   {
                                                                       Logger.LogWarning("page down key entered");
                                                                       GotoNextFilmBoard();
                                                                       Keyboard.Focus(this.Card);
                                                                   },
                                                                   param => (Card.IsInFilmingMainWindow)));
            }
        }
      

        private ICommand _pageUpCommand;
        public ICommand PageUpCommand
        {
            get
            {
                return _pageUpCommand ?? (_pageUpCommand = new RelayCommand(
                                                               param =>
                                                               {
                                                                   Logger.LogWarning("page up key entered");
                                                                   if(Card.IfZoomWindowShowState) return;
                                                                   GotoPreviousFilmBoard();
                                                                   Keyboard.Focus(this.Card);
                                                               },
                                                               param => (Card.IsInFilmingMainWindow)));
            }
        }

        private ICommand _selectSeriesCommand;
        public ICommand SelectSeriesCommand
        {
            get
            {
                return _selectSeriesCommand ?? (_selectSeriesCommand = new RelayCommand(
                                                                           param =>
                                                                           {
                                                                               Logger.LogWarning("space key entered");
                                                                               if (Card.IfZoomWindowShowState)
                                                                               {
                                                                                   Card.ZoomViewer.ExSpaceKeyDown();
                                                                                   return;
                                                                               }
                                                                               //if (FilmingHelper.SystemModality == "CT")
                                                                               //{
                                                                               //    SelectAllCellsOfSelectedViewports();
                                                                               //}
                                                                               //else
                                                                               //{
                                                                               Card.BtnEditCtrl.OnSelectSeries(null, null);
                                                                               //}
                                                                           },
                                                                           param =>
                                                                           (Card.IsInFilmingMainWindow &&Card.FilmingCardModality !=FilmingUtility.EFilmModality)));
            }
        }

        private ICommand _selectFilmPageCommand;
        public ICommand SelectFilmPageCommand
        {
            get
            {
                return _selectFilmPageCommand ?? (_selectFilmPageCommand = new RelayCommand(
                                                                               param =>
                                                                               {
                                                                                   SelectFilmingPages(Card.ActiveFilmingPageList);
                                                                               },
                                                                               param =>
                                                                               (Card.IsInFilmingMainWindow &&Card.IsEnableSelectFilm)));
            }
        }

        private ICommand _selectUpCommand;
        public ICommand SelectUpCommand
        {
            get
            {
                return _selectUpCommand ?? (_selectUpCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         if (Card.IfZoomWindowShowState) return;
                                                                         Logger.LogWarning("Up key entered");
                                                                         SelectByShortCuts(ShortCutSelect.Up);
                                                                     },
                                                                     param => (Card.IsEnableSingleSelectByShortCut)));
            }
        }

        private ICommand _selectDownCommand;
        public ICommand SelectDownCommand
        {
            get
            {
                return _selectDownCommand ?? (_selectDownCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         if (Card.IfZoomWindowShowState) return;
                                                                         Logger.LogWarning("Down key entered");
                                                                         SelectByShortCuts(ShortCutSelect.Down);
                                                                     },
                                                                     param => (Card.IsEnableSingleSelectByShortCut)));
            }
        }

        private ICommand _selectLeftCommand;
        public ICommand SelectLeftCommand
        {
            get
            {
                return _selectLeftCommand ?? (_selectLeftCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         if (Card.IfZoomWindowShowState) return;
                                                                         Logger.LogWarning("Left key entered");
                                                                         SelectByShortCuts(ShortCutSelect.Left);
                                                                     },
                                                                     param => (Card.IsEnableSingleSelectByShortCut)));
            }
        }

        private ICommand _selectRightCommand;
        public ICommand SelectRightCommand
        {
            get
            {
                return _selectRightCommand ?? (_selectRightCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         if (Card.IfZoomWindowShowState) return;
                                                                         Logger.LogWarning("Right key entered");
                                                                         SelectByShortCuts(ShortCutSelect.Right);
                                                                     },
                                                                     param => (Card.IsEnableSingleSelectByShortCut)));
            }
        }

        private ICommand _selectShiftUpCommand;
        public ICommand SelectShiftUpCommand
        {
            get
            {
                return _selectShiftUpCommand ?? (_selectShiftUpCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         if (Card.IfZoomWindowShowState) return;
                                                                         Logger.LogWarning("Shift Up key entered");
                                                                         SelectByShortCuts(ShortCutSelect.ShiftUp);
                                                                     },
                                                                     param => (Card.IsEnableShiftSelectByShortCut)));
            }
        }

        private ICommand _selectShiftDownCommand;
        public ICommand SelectShiftDownCommand
        {
            get
            {
                return _selectShiftDownCommand ?? (_selectShiftDownCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         if (Card.IfZoomWindowShowState) return;
                                                                         Logger.LogWarning("Shift Down key entered");
                                                                         SelectByShortCuts(ShortCutSelect.ShiftDown);
                                                                     },
                                                                     param => (Card.IsEnableShiftSelectByShortCut)));
            }
        }

        private ICommand _selectShiftLeftCommand;
        public ICommand SelectShiftLeftCommand
        {
            get
            {
                return _selectShiftLeftCommand ?? (_selectShiftLeftCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         if (Card.IfZoomWindowShowState) return;
                                                                         Logger.LogWarning("Shift Left key entered");
                                                                         SelectByShortCuts(ShortCutSelect.ShiftLeft);
                                                                     },
                                                                     param => (Card.IsEnableShiftSelectByShortCut)));
            }
        }

        private ICommand _selectShiftRightCommand;
        public ICommand SelectShiftRightCommand
        {
            get
            {
                return _selectShiftRightCommand ?? (_selectShiftRightCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         if (Card.IfZoomWindowShowState) return;
                                                                         Logger.LogWarning("Shift Right key entered");
                                                                         SelectByShortCuts(ShortCutSelect.ShiftRight);
                                                                     },
                                                                     param => (Card.IsEnableShiftSelectByShortCut)));
            }
        }

        private ICommand _recoverWlCommand;
        public ICommand RecoverWlCommand
        {
            get
            {
                return _recoverWlCommand ?? (_recoverWlCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         Logger.LogInfo("F12 key entered");
                                                                         Card.actiontoolCtrl.AdjustWlBtnClick(null, null);
                                                                     },
                                                                     param => (Card.IsInFilmingMainWindow && !Card.IsCellModalitySC)));
            }
        }

        private ICommand _selectSeriesForRightMouseClickCommand;

        public ICommand SelectSeriesForRightMouseClickCommand
        {
            get
            {
                return _selectSeriesForRightMouseClickCommand ??
                       (_selectSeriesForRightMouseClickCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         Card.BtnEditCtrl.OnSelectSeries(null, null);
                                                                     },
                                                                     param =>
                                                                     (Card.IsInFilmingMainWindow && Card.BtnEditCtrl.IsEnableSelectSeries)));
            }
        }

        private ICommand _selectSucceedCommand;

        public ICommand SelectSucceedCommand
        {
            get
            {
                return _selectSucceedCommand ?? (_selectSucceedCommand = new RelayCommand(
                                                                             param =>
                                                                             {
                                                                                 SelectSucceed();
                                                                             },
                                                                             param =>
                                                                             (Card.IsInFilmingMainWindow &&Card.IsEnableSelectSucceed)));
            }
        }

        private ICommand _selectAllFilmPageCommand;
        public ICommand SelectAllFilmPageCommand
        {
            get
            {
                return _selectAllFilmPageCommand ?? (_selectAllFilmPageCommand = new RelayCommand(
                                                                                     param =>
                                                                                     {
                                                                                         Card.BtnEditCtrl.OnSelectAllFilmPages(null, null);
                                                                                     },
                                                                                     param =>
                                                                                     (Card.IsInFilmingMainWindow &&Card.BtnEditCtrl.IsEnableSelectAllFilm)));
            }
        }

        private ICommand _insertPageBreakCommand;
        public ICommand InsertPageBreakCommand
        {
            get
            {
                return _insertPageBreakCommand ??
                       (_insertPageBreakCommand = new RelayCommand(param => { InsertPageBreak(); }, param => (Card.BtnEditCtrl.IsEnableInsertPageBreak)));
            }
        }


        #region graphic context menu

        #region paste graphic
        private string _graphicXml;

        internal enum ObjectCopyedOrCutedType
        {
            Graphic,
            Image,
        }


        private ObjectCopyedOrCutedType objectCopyedOrCutedType;

        public bool IsClipboardNotEmpty
        {
            get { return _clipboard.Count > 0; }
        }

        //

        public void Paste()
        {
            if (objectCopyedOrCutedType == ObjectCopyedOrCutedType.Graphic)
            {
                ExePasteGraphics();
            }
            else if (objectCopyedOrCutedType == ObjectCopyedOrCutedType.Image)
            {
                PasteImages();
            }
        }

        //there must have cell in clip board, and you must select a cell for insert after it.
        internal bool IsEnablePaste
        {
            get
            {
                if (objectCopyedOrCutedType == ObjectCopyedOrCutedType.Graphic)
                {
                    return IsEnablePasteGraphic;
                }
                else if (objectCopyedOrCutedType == ObjectCopyedOrCutedType.Image)
                {
                    return IsEnablePasteImage;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsEnablePasteImage
        {
            get
            {
                if (Card.IsCellModalitySC || !Card.IsInFilmingMainWindow)
                {
                    return false;
                }
                if (Card.LastSelectedFilmingPage != null && Card.LastSelectedFilmingPage.IsInSeriesCompareMode)
                {
                    return false;
                }

                if (IsClipboardNotEmpty &&
                    (Card.IsCellSelected || (Card.LastSelectedFilmingPage != null && !Card.LastSelectedFilmingPage.Cells.Any())))
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsEnableCopyImage
        {
            get { return (Card.IsCellSelected && !Card.IsCellModalitySC && Card.IsInFilmingMainWindow); }
        }

       

        public void CutImages()
        {
            try
            {
                Logger.LogFuncUp();

                if (!IsEnableCutImage) return;
                objectCopyedOrCutedType = ObjectCopyedOrCutedType.Image;
                if (Card.IfZoomWindowShowState)
                {
                    Card.ZoomViewer.CloseDialog();
                }
                DoCutImages();
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void CopyImagesOfSelectedCellsToClipBoard(List<MedViewerControlCell> selectedCells)
        {
            selectedCells.ForEach(cell => _clipboard.Add(cell.Image.CurrentPage));
        }

        public void CopyImages()
        {
            try
            {
                Logger.LogFuncUp();
                objectCopyedOrCutedType = ObjectCopyedOrCutedType.Image;
                CopyImagesToClipboard();
                //Card.UpdateUIStatus();
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void DoCutImages()
        {
            _clipboard.Clear();
            var selectedCells = Card.CollectSelectedCells();
            CopyImagesOfSelectedCellsToClipBoard(selectedCells);
            DeleteSelectedCells();
        }

        public void CutImagesToClipboard()
        {
            _clipboard.Clear();
            var selectedCells = Card.CollectSelectedCells();
            CopyImagesOfSelectedCellsToClipBoard(selectedCells);
            ClearImagesOfSelectedCells(selectedCells);
        }

        private void CopyImagesToClipboard()
        {
            _clipboard.Clear();
            var cells = Card.CollectSelectedCells();
            var n = cells.Count;
            var i = 1;
            cells.ForEach(cell => cell.ViewerControl.Dispatcher.BeginInvoke(DispatcherPriority.DataBind,new Action<MedViewerControlCell>((curcell) =>
                                                                                                                 {
                                                                                                                     curcell.ForceEndAction();
                                                                                                                     MedViewerControlCell clonedCell = curcell.Clone();
                                                                                                                     DisplayData displayData = clonedCell.Image.CurrentPage;
                                                                                                                     _clipboard.Add(displayData);
                                                                                                                     if(i++==n) Card.UpdateUIStatus();
                                                                                                                 }),cell));
        }

        public void ClearImagesOfSelectedCells(List<MedViewerControlCell> selectedCells)
        {
            selectedCells.ForEach(cell =>
            {

                FilmPageUtil.ClearAllActions(cell);
                cell.Image.Clear();
                cell.IsSelected = false; //删除图片后应去除多选状态
                cell.Refresh();
            });
        }


        public bool IsEnablePasteGraphic
        {
            get
            {
                if (Card.IsCellModalitySC || Card.LastSelectedCell == null) return false;
                Card.LastSelectedCell.ViewerControl.GraphicsXml = _graphicXml;
                return !string.IsNullOrEmpty(_graphicXml) && !Card.LastSelectedCell.IsEmpty && Card.LastSelectedCell.ViewerControl.CanPasteSelectedGraphics();
            }
        }

        private ICommand _pasteGraphicCommand;
        public ICommand PasteGraphicCommand
        {
            get
            {
                return _pasteGraphicCommand ??
                       (_pasteGraphicCommand = new RelayCommand(param => ExePasteGraphics()));
                //param => ExePasteGraphics(),
                //param => IsPasteGraphicEnabled));
            }
        }

        public void ExePasteGraphics()
        {
            foreach (var filmingPage in Card.ActiveFilmingPageList)
            {
                var viewcontrol = filmingPage.filmingViewerControl;
                viewcontrol.GraphicsXml = _graphicXml;
                viewcontrol.PasteSelectedGraphics();
            }
            if (Card.IfZoomWindowShowState)
            {
                Card.ZoomViewer.RefreshDisplayCell();
            }
        }
        #endregion paste graphic

        #region cut graphic
        private ICommand _cutGraphicCommand;
        public ICommand CutGraphicCommand
        {
            get
            {
                return _cutGraphicCommand ??
                       (_cutGraphicCommand = new MvvmLight.RelayCommand(ExeCutGraphics, CanCutGraphics));
                //param =>ExeCutGraphics(),
                //param => CanCutGraphics())); 
            }
        }

        public bool CanCutGraphics()
        {
            if (Card.LastSelectedFilmingPage == null)
            {
                return false;
            }
            return Card.LastSelectedFilmingPage.filmingViewerControl.CanCopySelectedGraphics()
                   &&
                   Card.ActiveFilmingPageList.Count(control => control.filmingViewerControl.CanCopySelectedGraphics()) == 1;
        }

        public void ExeCutGraphics()
        {
            objectCopyedOrCutedType = ObjectCopyedOrCutedType.Graphic;
            Card.LastSelectedFilmingPage.filmingViewerControl.CutSelectedGraphics();
            _graphicXml = Card.LastSelectedFilmingPage.filmingViewerControl.GraphicsXml;
            if (Card.IfZoomWindowShowState)
            {
                Card.ZoomViewer.RefreshDisplayCell();
            }
        }

        #endregion cut graphic

        #region copy graphic
        private ICommand _copyGraphicCommand;
        public ICommand CopyGraphicCommand
        {
            get
            {
                return _copyGraphicCommand ??
                       (_copyGraphicCommand = new MvvmLight.RelayCommand(ExeCopyGraphics, CanCopyGraphics));
                //param => ExeCopyGraphics(),
                //param => CanCopyGraphics()));
            }
        }

        public bool CanCopyGraphics()
        {
            if (Card.LastSelectedFilmingPage == null)
            {
                return false;
            }
            return Card.LastSelectedFilmingPage.filmingViewerControl.CanCopySelectedGraphics()
                   &&
                   Card.ActiveFilmingPageList.Count(control => control.filmingViewerControl.CanCopySelectedGraphics()) == 1;
        }

        public void ExeCopyGraphics()
        {
            objectCopyedOrCutedType = ObjectCopyedOrCutedType.Graphic;
            Card.LastSelectedFilmingPage.filmingViewerControl.CopySelectedGraphics();
            _graphicXml = Card.LastSelectedFilmingPage.filmingViewerControl.GraphicsXml;
        }


        #endregion copy graphic

        #region delete graphic
        private ICommand _deleteGraphicCommand;
        public ICommand DeleteGraphicCommand
        {
            get
            {
                return _deleteGraphicCommand ??
                       (_deleteGraphicCommand = new MvvmLight.RelayCommand(ExeDeleteGraphics, CanDeleteGraphics));
                //param => ExeDeleteGraphics(),
                //param => CanDeleteGraphics()));
            }
        }

        private bool CanDeleteGraphics()
        {
            if (Card.LastSelectedFilmingPage == null)
            {
                return false;
            }
            return Card.LastSelectedFilmingPage.filmingViewerControl.CanRemoveSelectedGraphics();
        }

        private void ExeDeleteGraphics()
        {
            foreach (var filmingPage in Card.ActiveFilmingPageList)
            {
                var viewcontrol = filmingPage.filmingViewerControl;
                viewcontrol.RemoveSelectedGraphics();
            }
            if (Card.IfZoomWindowShowState)
            {
                Card.ZoomViewer.RefreshDisplayCell();
            }
        }


        #endregion delete graphic

        #endregion

       

        private bool _ifShowImageRuler = true;
        public bool IfShowImageRuler
        {
            get { return _ifShowImageRuler; }
            set
            {
                if (value != _ifShowImageRuler)
                {
                    _ifShowImageRuler = value;
                    Printers.Instance.IfShowImageRuler = value;
                    Card.OnPropertyChanged(new PropertyChangedEventArgs("IfShowImageRuler"));
                }
            }
        }

        

       

        public void SelectSucceed()
        {
            try
            {
                Logger.LogFuncUp();
                var selectCell = Card.ActiveFilmingPageList.First().SelectedCells().First();
                if (selectCell == null) return;
                if (selectCell.Image == null) return;
                if (Card.IfZoomWindowShowState)
                {
                    Card.ZoomViewer.CloseDialog();
                }
                var seriesUiD = FilmPageUtil.GetSeriesUidFromImage(selectCell.Image);
                if (string.IsNullOrEmpty(seriesUiD)) return;
                foreach (var filmingPageControl in Card.EntityFilmingPageList)
                {
                    foreach (var cell in filmingPageControl.Cells.ToList())
                    {
                        if (cell.Image != null && seriesUiD == FilmPageUtil.GetSeriesUidFromImage(cell.Image)
                            && cell.Image.CurrentPage.UserSpecialInfo == selectCell.Image.CurrentPage.UserSpecialInfo
                            &&
                            Int32.Parse(FilmPageUtil.GetImageNumberFromImage(cell.Image)) >
                            Int32.Parse(FilmPageUtil.GetImageNumberFromImage(selectCell.Image)))
                        {
                            var viewPort = FilmPageUtil.ViewportOfCell(cell, filmingPageControl);
                            Card.SelectObject(filmingPageControl, viewPort, cell);
                        }
                    }
                }

                Card.UpdateButtonStatus();
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void SelectFilmingPages(List<FilmingPageControl> pages)
        {
            try
            {
                Logger.LogFuncUp();

                foreach (var page in pages)
                {
                    FilmPageUtil.SelectAllOfFilmingPage(page, true);
                }

                Card.LastSelectedFilmingPage = Card.ActiveFilmingPageList.LastOrDefault();
                Card.SelectLastCellOrFirstViewport(Card.LastSelectedFilmingPage);
                Card.UpdateUIStatus();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }



        private void SwitchScaleRuler()
        {
            Logger.Instance.LogPerformanceRecord("[Scale Ruler Operation][Begin]");
            if (Card.EntityFilmingPageList == null)
            {
                return;
            }
            this.IfShowImageRuler = !IfShowImageRuler;
            foreach (var filmPage in Card.EntityFilmingPageList)
            {
                foreach (var cell in filmPage.Cells)
                {
                    if (null != cell.Image
                        && null != cell.Image.CurrentPage)
                    {
                        var overlayFilmingF1ProcessText =
                            cell.Image.CurrentPage.GetOverlay(OverlayType.FilmingF1ProcessText) as OverlayFilmingF1ProcessText;
                        if (null != overlayFilmingF1ProcessText)
                        {
                            overlayFilmingF1ProcessText.SetRulerDisplayMode(this.IfShowImageRuler);
                        }
                    }
                }
            }
            if (Card.IfZoomWindowShowState)
            {
                Card.ZoomViewer.RefreshDisplayCell();
            }
            Logger.Instance.LogPerformanceRecord("[Scale Ruler Operation][End]");
        }

        private void ShowHideColorBar()
        {
            Logger.Instance.LogPerformanceRecord("[Begin][ColorBar]");
            var IfShoworHide = IfShowHideImageRuler;
            foreach (var film in Card.ActiveFilmingPageList)
            {
                if (!film.IsVisible) film.IsBeenRendered = false;
                foreach (var cell in film.SelectedCells())
                {
                    if (cell.Image != null && cell.Image.CurrentPage != null)
                    {
                       // film.SetOverlayVisibility(cell.Image.CurrentPage, OverlayType.Colorbar, IfShoworHide);
                        cell.Image.CurrentPage.GetOverlay(OverlayType.Colorbar).IsVisible = IfShoworHide;

                    }
                    
                }
            }
            if (Card.IfZoomWindowShowState)
            {
                Card.ZoomViewer.RefreshDisplayCell();
            }
            Logger.Instance.LogPerformanceRecord("[End][ColorBar]");
        }

        public bool IfShowHideImageRuler
        {
            get
            {
                foreach (var film in Card.ActiveFilmingPageList)
                {
                    foreach (var cell in film.SelectedCells())
                    {
                        if (cell.Image != null && cell.Image.CurrentPage != null)
                            return !cell.Image.CurrentPage.GetOverlay(OverlayType.Colorbar).IsVisible;
                    }
                }
                return true;
            }
        }

        public bool _isDeleteGraphic = false;
        public bool _ifDeleteReferenceImage = false;

        private void DeleteSelectedImages()
        {
            try
            {
                if (Card.IfZoomWindowShowState)
                {
                    Card.ZoomViewer.ExDeleteKeyDown();
                    return;
                }
                if (_isDeleteGraphic)
                {
                    DeleteSelectedGraphics();
                    _isDeleteGraphic = false;
                    return;
                }
                if (_ifDeleteReferenceImage)
                {
                    _ifDeleteReferenceImage = false;
                    Card.UpdateUIStatus();
                    return;
                }
                DeleteSelectedCells();

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void DeleteSelectedGraphics()
        {
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag);
                foreach (var filmPage in Card.ActiveFilmingPageList)
                {
                    foreach (var cell in filmPage.Cells)
                    {
                        if (cell != null && cell.Image != null && cell.Image.CurrentPage != null)
                        {
                            var overlay = cell.Image.CurrentPage.GetOverlay(OverlayType.Graphics);
                            var Graphics = new List<IDynamicGraphicObj>();
                            foreach (IGraphicObj g in overlay.SelectedGraphics)
                            {
                                if (g is IDynamicGraphicObj)
                                    Graphics.Add(g as IDynamicGraphicObj);
                            }
                            cell.DeleteGraphics(Graphics, overlay);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public bool GotoNextFilmBoard()
        {
            try
            {
                Logger.Instance.LogPerformanceRecord("[Begin][Scroll down]");

                Logger.LogTimeStamp("开始翻到下一页");

                Logger.LogFuncUp();

                if (Card.CurrentFilmPageBoardIndex < (Card.FilmPageBoardCount - 1))
                {
                    Card.CurrentFilmPageBoardIndex++;
                    //filmPageGrid.Children.Clear();
                }
                else
                {
                    return false;
                }

                int displayedFilmPageCount = 0;

                int startFilmPageIndex = Card.CurrentFilmPageBoardIndex * Card.SelectedFilmCardDisplayMode;
                int i = startFilmPageIndex;
                for (; i < Card.EntityFilmingPageList.Count; i++)
                {
                    FilmingPageControl filmPage = Card.EntityFilmingPageList[i];

                    if (displayedFilmPageCount < (Card.FilmingCardRows * Card.FilmingCardColumns))
                    {
                        Card.DisplayFilmPage(filmPage);
                    }
                    else
                    {
                        Card.UpdateFilmCardScrollBar();
                        return true;
                    }
                    displayedFilmPageCount++;
                    //if (filmPage.Cells != null)
                    //{
                    //    foreach (var cell in filmPage.Cells)
                    //    {
                    //        cell.Refresh();
                    //    }
                    //}
                }
                for (; displayedFilmPageCount < Card.SelectedFilmCardDisplayMode; displayedFilmPageCount++)
                {
                    Card._filmPlates[displayedFilmPageCount].Display(null);
                }

                Card.UpdateFilmCardScrollBar();

                Logger.LogFuncDown();

                Logger.LogTimeStamp("结束翻到下一页");

                return true;

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                return false;
            }
            finally
            {
                Logger.Instance.LogPerformanceRecord("[End][Scroll down] ");
            }
        }

        public bool GotoPreviousFilmBoard()
        {
            try
            {
                Logger.Instance.LogPerformanceRecord("[Begin][Scroll up]");
                Logger.LogFuncUp();

                Logger.LogTimeStamp("开始翻页");

                if (Card.CurrentFilmPageBoardIndex > 0)
                {
                    Logger.LogTimeStamp("开始移除旧的胶片");
                    Card.CurrentFilmPageBoardIndex--;
                    //todo: performance optimization begin page flipping
                    //filmPageGrid.Children.Clear();
                    //todo: performance optimization end
                    Logger.LogTimeStamp("完成移除旧的胶片");
                }
                else
                {
                    return false;
                }

                int displayedFilmPageCount = 0;

                Logger.LogTimeStamp("开始加载新的胶片");

                int startFilmPageIndex = Card.CurrentFilmPageBoardIndex * (Card.SelectedFilmCardDisplayMode);
                for (int i = startFilmPageIndex; i < Card.EntityFilmingPageList.Count; i++)
                {
                    var filmPage = Card.EntityFilmingPageList[i];

                    if (displayedFilmPageCount < (Card.FilmingCardRows * Card.FilmingCardColumns))
                    {
                        Logger.LogTimeStamp("开始显示一张胶片");
                        Card.DisplayFilmPage(filmPage);
                        Logger.LogTimeStamp("完成显示一张胶片");
                    }
                    else
                    {
                        Card.UpdateFilmCardScrollBar();
                        return true;
                    }
                    displayedFilmPageCount++;

                    Logger.LogTimeStamp("开始刷新所有的cell");
                    //if (filmPage.Cells != null)
                    //{
                    //    foreach (var cell in filmPage.Cells)
                    //    {
                    //        cell.Refresh();
                    //    }
                    //}
                    Logger.LogTimeStamp("完成刷新所有的cell");
                }

                Logger.LogTimeStamp("完成加载新的胶片");

                Card.UpdateFilmCardScrollBar();
                Logger.LogFuncDown();

                Logger.LogTimeStamp("完成翻页");

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                return false;
            }
            finally
            {
                Logger.Instance.LogPerformanceRecord("[End][Scroll up]");
            }
        }

        public void SelectByShortCuts(ShortCutSelect shortCut)
        {
            try
            {
                Logger.LogFuncUp();
                MedViewerControlCell selectedCell = null;
                McsfFilmViewport selectedViewport = null;
                FilmingPageControl selectedFilmingPage = null;

                //1.寻找当前焦点/锚点/选中对象起点 cell, viewport, page
                if (Card.LastSelectedCell != null && Card.LastSelectedViewport != null && Card.LastSelectedFilmingPage != null)
                {
                    selectedCell = Card.LastSelectedCell;
                    selectedViewport = Card.LastSelectedViewport;
                    selectedFilmingPage = Card.LastSelectedFilmingPage;
                }
                else
                {
                    selectedCell = Card.ActiveFilmingPageList.First().SelectedCells().First();
                    if (selectedCell == null) return;
                    foreach (var filmPage in Card.EntityFilmingPageList)
                    {
                        selectedViewport = FilmPageUtil.ViewportOfCell(selectedCell, filmPage);
                        if (selectedViewport == null) continue;
                        selectedFilmingPage = filmPage;
                        break;
                    }
                }

                //2.寻找选中对象终点 cellIndex in Viewport
                if (selectedCell == null || selectedViewport == null || selectedFilmingPage == null) return;
                var cellIndexOffset = 0;
                for (int i = 0; i < selectedViewport.IndexInFilm; i++)
                {
                    cellIndexOffset += selectedFilmingPage.ViewportList[i].CellLayout.MaxImagesCount;
                }
                int selectCellIndexInViewport = selectedCell.CellIndex - cellIndexOffset;
                int nextCellIndexInViewport = -1;
                switch (shortCut)
                {
                    case ShortCutSelect.Up:
                        nextCellIndexInViewport = selectCellIndexInViewport - selectedViewport.CellLayout.LayoutColumnsSize;
                        break;
                    case ShortCutSelect.Down:
                        nextCellIndexInViewport = selectCellIndexInViewport + selectedViewport.CellLayout.LayoutColumnsSize;
                        break;
                    case ShortCutSelect.Left:
                        nextCellIndexInViewport = selectCellIndexInViewport - 1;
                        break;
                    case ShortCutSelect.Right:
                        nextCellIndexInViewport = selectCellIndexInViewport + 1;
                        break;
                    case ShortCutSelect.ShiftUp:
                        nextCellIndexInViewport = selectCellIndexInViewport - selectedViewport.CellLayout.LayoutColumnsSize;
                        break;
                    case ShortCutSelect.ShiftDown:
                        nextCellIndexInViewport = selectCellIndexInViewport + selectedViewport.CellLayout.LayoutColumnsSize;
                        break;
                    case ShortCutSelect.ShiftLeft:
                        nextCellIndexInViewport = selectCellIndexInViewport - 1;
                        break;
                    case ShortCutSelect.ShiftRight:
                        nextCellIndexInViewport = selectCellIndexInViewport + 1;
                        break;
                }

                //3.选中起点和终点之间的cell
                if (nextCellIndexInViewport >= 0 && nextCellIndexInViewport <= selectedViewport.CellLayout.MaxImagesCount - 1)
                {
                    if (shortCut == ShortCutSelect.Up || shortCut == ShortCutSelect.Down || shortCut == ShortCutSelect.Left
                         || shortCut == ShortCutSelect.Right)
                    {
                        Card.ActiveFilmingPageList.UnSelectAllCells();
                    }
                    Card.SelectObject(selectedFilmingPage, selectedViewport, selectedViewport.RootLayoutCell.ControlCells[nextCellIndexInViewport]);
                    Card.UpdateButtonStatus();
                }
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void PasteImages()
        {
            try
            {
                Logger.LogFuncUp();

                if (!IsEnablePasteImage)
                    return;
             
                //fix bug 346962
                GetPastePosition(ref Card._dropViewCell, ref Card._dropFilmingPage);

                if (null == Card._dropViewCell)
                {
                    MessageBoxHandler.Instance.ShowInfo("UID_Filming_Warning_Form_Paste_Position");
                    return;
                }

                Card.EntityFilmingPageList.UnSelectAllCells();
                List<DisplayData> lstdisdata = CloneClipboard();
                //
                var userSpecialInfo = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffffff");
                foreach (var disData in lstdisdata)
                {
                    if(disData != null)
                        disData.UserSpecialInfo = userSpecialInfo;
                }

                List<MedViewerControlCell> cells = Card.CreateCellsByDisplayData(lstdisdata);
                Card.InsertCells(cells);
               
                if (Card.IsEnableRepack)
                {
                    Card.contextMenu.Repack(RepackMode.RepackPaste);
                }
                else
                {
                    Card.RefreshAnnotationDisplayMode();
                }
                Card._cellsToBeMoveForward.Clear();
                Card._pagesToBeRefreshed.Clear();
                Card.UpdateFilmCardScrollBar();
                Card.ReOrderCurrentFilmPageBoard();
                Card.UpdateUIStatus();
                Card.UpdateImageCount();

                if (!Card.IsEnableRepack)
                {
                    Card.EntityFilmingPageList.ForEach((film) => film.RefereshPageTitle());
                    Card.EntityFilmingPageList.UpdatePageLabel();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }


        private void GetPastePosition(ref MedViewerControlCell dropViewCell, ref FilmingPageControl dropFilmingPage)
        {
            if (Card.LastSelectedCell == null || Card.LastSelectedCell.CellIndex == -1)
            {
                Card._dropViewCell = null;
                //MessageBoxHandler.Instance.ShowInfo("UID_Filming_Warning_Form_Paste_Position");
                return;
            }
            if (Card.LastSelectedCell.IsSelected)
            {
                Card._dropViewCell = Card.LastSelectedCell;
                Card._dropFilmingPage = Card.LastSelectedFilmingPage;
            }
            else
            {
                Card._dropFilmingPage = Card.ActiveFilmingPageList.FirstOrDefault();
                Card._dropViewCell = null == Card._dropFilmingPage ? null : Card._dropFilmingPage.SelectedCells().FirstOrDefault();
            }

        }


        public readonly List<DisplayData> _clipboard = new List<DisplayData>();

        /// <summary>
        /// Make deep copy of images from clipboard
        /// </summary>
        /// <returns></returns>
        private List<DisplayData> CloneClipboard()
        {
            List<DisplayData> images = new List<DisplayData>();
            // Make deep copy
            foreach (var displayData in _clipboard)
            {
               
                MedViewerControlCell tmpCell = new MedViewerControlCell();
                MedViewerControlCell clonedCell = null;

                if (displayData != null)
                    tmpCell.Image.AddPage(displayData);

                clonedCell = tmpCell.Clone();

                DisplayData clonedData = clonedCell.Image.CurrentPage;
                images.Add(clonedData);
                
            }

            return images;
        }
       
        public void InsertPageBreak()
        {
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag);

                var selectedFilmPage = Card.ActiveFilmingPageList.FirstOrDefault();
                if (selectedFilmPage != null)
                {
                    var selectedCell = selectedFilmPage.SelectedCells().LastOrDefault();
                    if (selectedCell != null)
                    {
                        FilmingPageControl breakFilmPage;
                        var backupSelectPageCells = PopUpCellsForInsertPageBreak(selectedFilmPage,
                                                                                 selectedCell.CellIndex);
                        if (backupSelectPageCells.Count > 0)
                        {
                            if (Card.EntityFilmingPageList.IndexOf(selectedFilmPage) + 1 > Card.EntityFilmingPageList.Count - 1)
                            //the last page
                            {
                                breakFilmPage = Card.InsertFilmPage(Card.EntityFilmingPageList.IndexOf(selectedFilmPage) + 1);
                                breakFilmPage.FilmPageType = FilmPageType.BreakFilmPage;
                            }
                            else
                            {
                                breakFilmPage =
                                    Card.EntityFilmingPageList[Card.EntityFilmingPageList.IndexOf(selectedFilmPage) + 1];
                                if (breakFilmPage.FilmPageType == FilmPageType.BreakFilmPage) //last page of region 
                                {
                                    breakFilmPage = Card.InsertFilmPage(Card.EntityFilmingPageList.IndexOf(selectedFilmPage) + 1);
                                }
                                breakFilmPage.FilmPageType = FilmPageType.BreakFilmPage;
                            }

                            List<MedViewerControlCell> backupCells = new List<MedViewerControlCell>();
                            var linkedPages = Card.GetLinkedPage(breakFilmPage);

                            Card._pagesToBeRefreshed.Clear();

                            foreach (var page in linkedPages)
                            {
                                foreach (var cell in page.Cells)
                                {
                                    MedViewerControlCell newCell = new MedViewerControlCell();
                                    if (!cell.IsEmpty)
                                    {
                                        newCell.Image.AddPage(cell.Image.CurrentPage);
                                        cell.Image.Clear();
                                    }
                                    backupCells.Add(newCell);
                                }
                                Card._pagesToBeRefreshed.Add(page);
                            }

                            Card._pagesToBeRefreshed.Remove(breakFilmPage);
                            Card._loadingTargetCellIndex = 0;
                            Card._loadingTargetPage = breakFilmPage;

                            foreach (var bCell in backupSelectPageCells)
                            {
                                Card.ReplaceCellBy(bCell);
                            }
                            int firstNonEmpty = backupCells.FindIndex(0, cell => !cell.IsEmpty);
                            int lastNonEmpty = backupCells.FindLastIndex(cell => !cell.IsEmpty);
                            if (firstNonEmpty >= 0)
                            {
                                for (int i = firstNonEmpty; i <= lastNonEmpty; i++)
                                {
                                    Card.ReplaceCellBy(backupCells[i]);
                                }
                            }
                        }
                        else
                        {
                            breakFilmPage = Card.EntityFilmingPageList[Card.EntityFilmingPageList.IndexOf(selectedFilmPage) + 1];
                            breakFilmPage.FilmPageType = FilmPageType.BreakFilmPage;
                        }

                        //select cell
                        Card.ActiveFilmingPageList.UnSelectAllCells();

                        McsfFilmViewport viewport = selectedFilmPage.ViewportOfCell(selectedCell);
                        Card.SelectObject(selectedFilmPage, viewport, selectedCell);
                        selectedFilmPage.filmingViewerControl.SelectedCells.Add(selectedCell);

                        //DisplayFilmPage(breakFilmPage);
                        Card.ReOrderCurrentFilmPageBoard();

                        Card.UpdateUIStatus();
                    }
                    //todo: performance optimization begin pageTitle
                    selectedFilmPage.RefereshPageTitle();
                    //todo: performance optimization end
                    Card.EntityFilmingPageList.UpdatePageLabel();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);

            }
            Card.EntityFilmingPageList.UpdateBreakPageFlag();
        }

        private List<MedViewerControlCell> PopUpCellsForInsertPageBreak(FilmingPageControl film, int index)
        {
            List<MedViewerControlCell> backupCells = new List<MedViewerControlCell>();

            var pageCells = film.Cells.ToList();

            int lastNonEmpty = pageCells.FindLastIndex(cell => !cell.IsEmpty);
            if (lastNonEmpty < index + 1) return backupCells;

            var isEnableRepack = Card.IsEnableRepack;
            int firstNonEmpty = pageCells.FindIndex(index + 1, cell => !cell.IsEmpty);
            if (firstNonEmpty >= 0)
            {
                // backup displaydata
                for (int i = firstNonEmpty; i <= lastNonEmpty; i++)
                {
                    MedViewerControlCell cell = new MedViewerControlCell();
                    MedViewerControlCell existingCell = pageCells.ElementAt(i);
                    if (!existingCell.IsEmpty)
                    {
                        cell.Image.AddPage(existingCell.Image.CurrentPage);
                        existingCell.Image.Clear();
                        existingCell.Refresh();
                    }
                    else if (isEnableRepack) continue;
                    backupCells.Add(cell);
                }
            }

            return backupCells;
        }

        private bool IsCellSelectedAndNotInSeriesCompareMode()
        {
            if (!Card.IsCellSelected)
            {
                return false;
            }

            return Card.ActiveFilmingPageList.All(page => !page.SelectedCells().Any() || !page.IsInSeriesCompareMode);
        }
       


        public bool IsEnableCutImage
        {
            get { return (IsCellSelectedAndNotInSeriesCompareMode() && !Card.IsCellModalitySC && Card.IsInFilmingMainWindow); }
        }

        public bool IsEnableDelete
        {
            get
            {
                bool isDelete = IsEnableDeleteImage || IsEnableDeletePageBreak || Card.BtnEditCtrl.IsEnableDeleteActiveFilm;
                return isDelete;
            }
        }

      

        //public bool IsEnableSelect
        //{
        //    get
        //    {
        //        bool isSelect = Card.BtnEditCtrl.IsEnableSelectSeries || Card.IsEnableSelectSucceed || Card.IsEnableSelectFilm ||
        //                        IsEnableSelectAllFilm;
        //        return isSelect;
        //    }
        //}

        public bool IsEnableDeleteImage
        {
            get { return IsCellSelectedAndNotInSeriesCompareMode() && !Card.IsCellModalitySC; }
        }

        public Visibility IsDeleteImageNotSelectedVisible
        {
            get
            {
                return FilmingHelper.SystemModality == "CT" ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool IsEnableDeleteAllImage
        {
            get { return (Card.IsAnyImageLoaded && !Card.IsCellModalitySC); }
        }


        public bool IsEnableDeletePageBreak
        {
            get
            {
                if (Card.IsCellModalitySC) return false;
                if (!IsCellSelectedAndNotInSeriesCompareMode())
                    return false;

                var selectedFilmPage = Card.ActiveFilmingPageList.FirstOrDefault();
                if (selectedFilmPage != null)
                {
                    if (selectedFilmPage.ViewportLayout.LayoutType == LayoutTypeEnum.DefinedLayout)//selectedFilmPage.ViewportList.Count > 1) //|| selectedFilmPage.SelectedCells().Count != 1)
                    {
                        return false;
                    }

                    if (selectedFilmPage.FilmPageIndex >= Card.EntityFilmingPageList.Count - 1) return false;
                    var nextFilm = Card.EntityFilmingPageList[selectedFilmPage.FilmPageIndex + 1];

                    if (nextFilm.FilmPageType == FilmPageType.BreakFilmPage) return true;

                }

                return false;
            }
        }


        private ICommand _deleteNotSelectedCommand;
        public ICommand DeleteNotSelectedCommand
        {
            get
            {
                return _deleteNotSelectedCommand ?? (_deleteNotSelectedCommand = new RelayCommand(
                                                               param =>
                                                               {
                                                                   //var cellCount = Card.ActiveFilmingPageList.Aggregate<FilmingPageControl, uint>(0, (current, page) => current + (uint)page.SelectedCells().Count(c => c != null && !c.IsEmpty));
                                                                   //if (cellCount == 0) return;
                                                                   
                                                                   WarnWhenDeleteImages(DeleteNotSelectedImages);
                                                               },
                                                              param =>(Card.IsInFilmingMainWindow && IsEnableDeleteImage)
                                                               ));


            }
        }

        private ICommand _deletePageBreakCommand;
        public ICommand DeletePageBreakCommand
        {
            get
            {
                return _deletePageBreakCommand ??
                       (_deletePageBreakCommand = new RelayCommand(param => DeletePageBreak(),
                                                              param => (Card.IsInFilmingMainWindow && IsEnableDeletePageBreak)));
            }
        }

        private ICommand _deleteFilmPageCommand;
        public ICommand DeleteFilmPageCommand
        {
            get
            {
                return _deleteFilmPageCommand ?? (_deleteFilmPageCommand = new RelayCommand(
                                                                               param =>
                                                                               {
                                                                                   Logger.LogWarning(
                                                                                       "delete key entered");

                                                                                   Card.BtnEditCtrl.OnDeleteActiveFilmPages(null,
                                                                                                              null);
                                                                               },
                                                                               param =>
                                                                               (Card.IsInFilmingMainWindow &&Card.BtnEditCtrl.IsEnableDeleteActiveFilm)));
            }
        }

        private ICommand _deleteAllCommand;

        public ICommand DeleteAllCommand
        {
            //get { return _deleteAllCommand ?? (_deleteAllCommand = new RelayCommand(param => DeleteAllImages())); }
            get
            {
                return _deleteAllCommand ?? (_deleteAllCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         Logger.LogWarning("delete key entered");
                                                                         Card.BtnEditCtrl.OnDeleteAllFilmPages(null, null);
                                                                     },
                                                                     param =>
                                                                     (Card.IsInFilmingMainWindow && Card.BtnEditCtrl.IsEnableDeleteAllFilm)));
            }
        }


        private void DeleteNotSelectedImages()
        {
            try
            {
                Logger.LogFuncUp();

                DeleteNotSelectedCells();
                if (Card.IsEnableRepack)
                {
                    List<FilmingPageControl> linkedPages = Card.GetLinkedPage(Card.ActiveFilmingPageList.FirstOrDefault());
                    foreach (FilmingPageControl seletedPage in linkedPages)
                    {
                        //最后一张空胶片不能删除
                        if (seletedPage.IsEmpty() && (Card.EntityFilmingPageList.Count > 1))
                        {
                            seletedPage.IsSelected = false;
                            DeleteFilmPage(seletedPage);
                        }
                    }
                    Card.ReOrderCurrentFilmPageBoard();
                    Card.EntityFilmingPageList.UpdatePageLabel();
                }
                //todo: performance optimization begin pageTitle
                Card.EntityFilmingPageList.ForEach((film) => film.RefereshPageTitle());
                //todo: performance optimization end
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void DeleteSelectedCells()
        {
            
            ClearImagesOfSelectedCells(Card.CollectSelectedCells());
            Card.UpdateImageCount();
            
            int iLastSelectedPageIndex = Card.LastSelectedFilmingPage.FilmPageIndex;
           // int iLastSelectedViewportIndex = Card.LastSelectedViewport!=null?Card.LastSelectedViewport.IndexInFilm:0;
            int iLastSelectedCellIndex = Card.LastSelectedFilmingPage.IsEmpty()?0: Card.LastSelectedCell.CellIndex;

            if (Card.IsEnableRepack) Card.contextMenu.Repack(RepackMode.RepackDelete);

            if (iLastSelectedPageIndex < Card.EntityFilmingPageList.Count)
            {
                FilmingPageControl page = Card.EntityFilmingPageList[iLastSelectedPageIndex];
               // McsfFilmViewport viewport = null;
                MedViewerControlCell cell = null;
                //if (iLastSelectedViewportIndex < page.ViewportList.Count)
                //{
                //    viewport = page.ViewportList[iLastSelectedViewportIndex];
                //}
                if (iLastSelectedCellIndex < page.Cells.Count())
                {
                    cell = page.Cells.ElementAt(iLastSelectedCellIndex < 0 ? 0 : iLastSelectedCellIndex);

                    if (cell.IsEmpty && Card.IsEnableRepack)
                    {
                        var tempcell = page.Cells.LastOrDefault(c => !c.IsEmpty);
                        if (tempcell == null)
                            cell = page.Cells.ElementAt(0);
                        else
                        {
                            cell = tempcell.CellIndex > cell.CellIndex ? cell : tempcell;
                        }
                    }
                }
                Card.EntityFilmingPageList.UnSelectAllCells();
                var vp = page.ViewportOfCell(cell);
                Card.SelectObject(page, vp, cell);
            }
            else
            {
                var page = Card.DisplayedFilmPage.FirstOrDefault();
                var viewport = page.ViewportList.FirstOrDefault();
                var cell = page.Cells.FirstOrDefault(c => !c.IsEmpty);
                if(cell==null)
                {
                    cell = page.Cells.FirstOrDefault();
                }
                Card.EntityFilmingPageList.UnSelectAllCells();
                Card.SelectObject(page, viewport, cell);
            }

            Card.UpdateUIStatus();
            if(!Card.IsEnableRepack)
                Card.EntityFilmingPageList.ForEach((film) => film.RefereshPageTitle());

        }

        private void DeleteNotSelectedCells()
        {
            var notSelectedCells =
                Card.EntityFilmingPageList.OrderBy(a => a.FilmPageIndex).ToList().Aggregate(new List<MedViewerControlCell>(),
                                                                                       (total, current) =>
                                                                                       {
                                                                                           total.AddRange(
                                                                                               current.NotSelectedCells().OrderBy(
                                                                                                       cell =>
                                                                                                       cell.CellIndex));
                                                                                           return total;
                                                                                       });
            if (Card.IfZoomWindowShowState && notSelectedCells.Count > 0)
            {
                Card.ZoomViewer.CloseDialog();
            }
            ClearImagesOfSelectedCells(notSelectedCells);
            Card.UpdateImageCount();

            if (Card.IsEnableRepack) 
                Card.contextMenu.Repack(RepackMode.RepackDelete);
            else
                Card.EntityFilmingPageList.UpdatePageLabel();
            Card.UpdateUIStatus();
            //todo: performance optimization begin pageTitle
            Card.EntityFilmingPageList.ForEach((film) => film.PageTitle.PatientInfoChanged());
            //todo: performance optimization end
        }

       

        public void DeleteAllFilmPages()
        {
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag);
                Card.DeleteAllFilmPage();
                Card.OnAddFilmPageAfterClearFilmingCard(null, null);
                //todo: performance optimization begin pageTitle
                Card.EntityFilmingPageList.ForEach((film) => film.RefereshPageTitle());
                //todo: performance optimization end
                Card.UpdateImageCount();

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void DeletePageBreak()
        {
            try
            {
                Logger.LogFuncUp();

                var selectedFilmPage = Card.ActiveFilmingPageList.FirstOrDefault();
                if (selectedFilmPage == null) return;

                if (selectedFilmPage.FilmPageIndex >= Card.EntityFilmingPageList.Count - 1) return;
                var breakPage = Card.EntityFilmingPageList[selectedFilmPage.FilmPageIndex + 1];
                breakPage.FilmPageType = FilmPageType.NormalFilmPage;


                var selectedCell = selectedFilmPage.SelectedCells().LastOrDefault();
                //List<MedViewerControlCell> backupCells = new List<MedViewerControlCell>();
                //var linkedPages = Card.GetLinkedPage(selectedFilmPage);
                //linkedPages.Remove(selectedFilmPage);

                //Card._pagesToBeRefreshed.Clear();

                //foreach (var page in linkedPages)
                //{
                //    foreach (var cell in page.Cells)
                //    {
                //        MedViewerControlCell newCell = new MedViewerControlCell();
                //        if (!cell.IsEmpty)
                //        {
                //            newCell.Image.AddPage(cell.Image.CurrentPage);
                //            cell.Image.Clear();
                //            cell.Refresh();
                //        }
                //        backupCells.Add(newCell);
                //    }
                //    Card._pagesToBeRefreshed.Add(page);
                //}
                //int beginIndex = selectedFilmPage.Cells.ToList().FindLastIndex(cell => !cell.IsEmpty);
                //if (beginIndex < 0) beginIndex = selectedFilmPage.MaxImagesCount - 1;

                //Card._loadingTargetCellIndex = beginIndex + 1;
                //Card._loadingTargetPage = selectedFilmPage;

                //int firstNonEmpty = backupCells.FindIndex(0, cell => !cell.IsEmpty);
                //int lastNonEmpty = backupCells.FindLastIndex(cell => !cell.IsEmpty);
                //if (firstNonEmpty >= 0)
                //{
                //    for (int i = firstNonEmpty; i <= lastNonEmpty; i++)
                //    {
                //        Card.ReplaceCellBy(backupCells[i]);
                //    }
                //}

                //var linkedRegionPages = Card.GetLinkedPage(selectedFilmPage);
                //linkedRegionPages.Remove(selectedFilmPage);
                //foreach (var page in linkedRegionPages)
                //{
                //    if (page.Cells.ToList().All(cell => cell.IsEmpty))
                //    {
                //        DeleteFilmPage(page);
                //        Card._pagesToBeRefreshed.RemoveAll(p => p.FilmPageIndex == page.FilmPageIndex);
                //    }
                //}

                //select cell
                Card.SelectObject(selectedFilmPage, Card.LastSelectedViewport, selectedCell);
                selectedFilmPage.filmingViewerControl.SelectedCells.Add(selectedCell);
                //Card.ReOrderCurrentFilmPageBoard();
                //Card.UpdateUIStatus();
                //todo: performance optimization begin pageTitle
                //Card.EntityFilmingPageList.ForEach((film) => film.RefereshPageTitle());
                //todo: performance optimization end
                //Card.EntityFilmingPageList.UpdatePageLabel();
                //}
                if (Card.IsEnableRepack)
                {
                    Card.contextMenu.Repack(RepackMode.RepackDelete);
                }
                else
                {
                    Card.ReOrderCurrentFilmPageBoard();
                }
                Card.UpdateUIStatus();
                if (!Card.IsEnableRepack)
                {
                    Card.EntityFilmingPageList.ForEach((film) => film.RefereshPageTitle());
                    Card.EntityFilmingPageList.UpdatePageLabel();
                }
                //Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

       
        public void DeleteActiveFilmPages()
        {
            try
            {
                Logger.LogFuncUp();
                if (Card.IfZoomWindowShowState)
                {
                    Card.ZoomViewer.CloseDialog();
                }
                foreach (var filmingPage in Card.ActiveFilmingPageList.ToList())
                {
                    DeleteFilmPage(filmingPage);
                }

                if (Card.EntityFilmingPageList.Count == 0)
                {
                    Card.OnAddFilmPageAfterClearFilmingCard(null, null);

                    Card.EntityFilmingPageList.ForEach((film) => film.RefereshPageTitle());

                }
                else
                {
                    Card.ReOrderCurrentFilmPageBoard();
                    ClearSelectedObjects();
                }
                var page = Card.DisplayedFilmPage.FirstOrDefault();
                if (page != null)
                {
                    var viewport = page.ViewportList.FirstOrDefault();
                    var cell = page.Cells.FirstOrDefault();
                    Card.SelectObject(page, viewport, cell);
                }
                Card.EntityFilmingPageList.UpdatePageLabel();
                Card.UpdateUIStatus();
                Card.UpdateImageCount();

                FilmingCard._miniCellsList.Clear();
                FilmingCard._miniCellsParentCellsList.Clear();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void ClearSelectedObjects()
        {
            Card.ActiveFilmingPageList.Clear();

            Card.LastSelectedFilmingPage = null;
            Card.LastSelectedViewport = null;
            Card.LastSelectedCell = null;
        }

        //todo: performance optimization begin New Page
        public void DeleteFilmPage(FilmingPageControl filmingPage)
        {
            try
            {
                var filmIndex = filmingPage.FilmPageIndex;
                var filmCount = Card.EntityFilmingPageList.Count;
                for (int i = filmIndex; i < filmCount; i++)
                {
                    var plateIndex = i % Card.layoutCtrl._selectedFilmCardDisplayMode;
                    var plate = Card._filmPlates[plateIndex];
                    plate.Remove(Card.EntityFilmingPageList[i]);
                }
                
                filmingPage.Visibility = Visibility.Hidden;
                filmingPage.IsSelected = false;
                filmingPage.SelectedAll(false);
                filmingPage.Clear();

                if (filmingPage.FilmPageType == FilmPageType.BreakFilmPage)
                {
                    var linkPages = Card.GetLinkedPage(filmingPage);
                    if (linkPages.Count > 1) linkPages[1].FilmPageType = FilmPageType.BreakFilmPage;
                }
                filmingPage.FilmPageType = FilmPageType.NormalFilmPage;
                if(Card.DeletedFilmingPageList.Count < 100)
                    Card.DeletedFilmingPageList.Add (filmingPage);
                Card.RemoveFromEntityFilmingPageList(filmingPage);

                Card.UpdateLayout();
                //if (filmPageGrid.Children.Contains(filmingPage))
                //{
                //    int startIndex = filmPageGrid.Children.IndexOf(filmingPage);
                //    filmPageGrid.Children.RemoveRange(startIndex, filmPageGrid.Children.Count);
                //}

            }
            catch (Exception ex)
            {
                Logger.LogError("Exception occurred in DeleteFilmPage: " + ex.StackTrace);
                throw;
            }
        }

        #region [-- Warning when images will be deleted (SSFS 101980) --]

        public void WarnWhenDeleteImages(Action deleteAction)
        {
            //2014-08-04, key 101980, 不再需要删除图片的提示
            //MessageBoxHandler.Instance.ShowQuestion(
            //    "UID_Filming_Delete_Images_Warning", 
            //    (r) => { if (r == MessageBoxResponse.YES)
            //    deleteAction(); },
            //    numberofImagesToBeDeleted);
            deleteAction();

            if (Card.EntityFilmingPageList.Count == 0) return;
            var film = Card.EntityFilmingPageList.Last();
            var nonEmptyImageCount = film.NonEmptyImageCount;
            Card.UpdateImageCountRemain(nonEmptyImageCount);
        }

        #endregion //[-- Warning when images will be deleted (SSFS 101980) --]


        #region  [打印显示选中胶片]
        private ICommand _printDisplayedSelectedFilmCommand;
        public ICommand PrintDisplayedSelectedFilmCommand
        {
            get
            {
                return _printDisplayedSelectedFilmCommand ?? (_printDisplayedSelectedFilmCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         Card.PrintAndSave.PrintDisplayedSelectedFilm(null, null);
                                                                     },
                                                                     param =>
                                                                     (IsEnablePrintDisplayedSelectedFilm)));
            }
        }
        private bool IsEnablePrintDisplayedSelectedFilm
        {
            get
            {
                return Card.PrintAndSave.DisplayedSelectedFilmPage != null && Card.PrintAndSave.DisplayedSelectedFilmPage.Count() == 1 && !Card.PrintAndSave.DisplayedSelectedFilmPage.First().IsEmpty();
            }
        }

        

        #endregion

        public void InsertEmptyCell()
        {
            try
            {
                Logger.LogFuncUp();

                //1.backupCells
                var film = Card.ActiveFilmingPageList.FirstOrDefault();
                if (film == null) return;

                var cellIndex = Card.LastSelectedCell.CellIndex;
                var filmIndex = film.FilmPageIndex;

                var backupCells = new List<MedViewerControlCell>();
                var backupDisplayData = new List<DisplayData>();
                var backupFilms = new FilmingPageCollection(null);
                var filmCount = Card.EntityFilmingPageList.Count;

                while (filmIndex < filmCount)
                {
                    backupFilms.Add(film);
                    backupCells.AddRange(film.Cells.Skip(cellIndex));
                    var nextFilmIndex = filmIndex + 1;
                    if (nextFilmIndex >= filmCount || Card.EntityFilmingPageList[nextFilmIndex].FilmPageType == FilmPageType.BreakFilmPage)    //filmPage
                    {
                        //backup displayData
                        backupDisplayData.AddRange(backupCells.Select(cell => cell.IsEmpty ? null : cell.Image.CurrentPage));

                        var lastCell = backupCells.Last();
                        if (!lastCell.IsEmpty)
                        {
                            var newFilm = Card.InsertFilmPage(nextFilmIndex);
                            backupCells.Add(newFilm.Cells.First());
                            backupFilms.Add(newFilm);
                        }
                        break;
                    }
                    cellIndex = 0;
                    filmIndex = nextFilmIndex;
                    film = Card.EntityFilmingPageList[filmIndex];
                }

                //2.Clear Old Cell(include first cell)
                foreach (var cell in backupCells)
                {
                    cell.Image.Clear();
                    cell.Refresh(CellRefreshType.ImageText);
                    cell.Refresh();
                }
                backupCells.RemoveAt(0);

                //3.Refresh cell
                for (int i = 0; i < backupDisplayData.Count; i++)
                {
                    var cell = backupCells[i];
                    var displayData = backupDisplayData[i];
                    if (displayData != null)
                    {
                        cell.Image.AddPage(displayData);
                        cell.Refresh();
                        FilmingHelper.RefereshDisplayMode(displayData);
                    }
                }

                backupFilms.UpdateAllPageTitle();
                Card.ReOrderCurrentFilmPageBoard();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }


        private bool IsEnablePopMiniPaCommandInsertRefImg
        {
            get
            {
                if (!Card.BtnEditCtrl.IsEnableInsertRefImage) return false;
                var seriesList = new List<string>();
                bool result = true;
                foreach (var film in Card.ActiveFilmingPageList)
                {
                    foreach (var cell in film.SelectedCells())
                    {
                        if (cell.Image != null && cell.Image.CurrentPage != null)
                        {
                            var seriesID = cell.Image.CurrentPage.ImageHeader.DicomHeader[ServiceTagName.SeriesInstanceUID];
                            if (seriesList.Count == 0)
                            {
                                seriesList.Add(seriesID);
                            }
                            else
                            {
                                if (!seriesList.Contains(seriesID))
                                {
                                    result = false;
                                    break;
                                }
                            }
                        }

                        if (!result) break;
                    }
                }
                return result;
            }
        }

        private ICommand _popMiniPaCommandInsertRefImg;
        public ICommand PopMiniPaCommandInsertRefImg
        {
            get
            {
                return _popMiniPaCommandInsertRefImg ?? (_popMiniPaCommandInsertRefImg = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         PopMiniPa();
                                                                     },
                                                                     param =>
                                                                     (IsEnablePopMiniPaCommandInsertRefImg)));
            }
        }


        private bool IsEnablePopMiniPaCommandAddLocalImage
        {
            get
            {
                return Card.contextMenu.IsLocalizedImageReferenceLineEnabled;
            }
        }
        private ICommand _popMiniPaCommandAddLocalImage;
        public ICommand PopMiniPaCommandAddLocalImage
        {
            get
            {
                return _popMiniPaCommandAddLocalImage ?? (_popMiniPaCommandAddLocalImage = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         PopMiniPa();
                                                                     },
                                                                     param =>
                                                                     (IsEnablePopMiniPaCommandAddLocalImage)));
            }
        }


        public void PopMiniPa()
        {
            try
            {
                var cmd = new UIH.Mcsf.Core.CommandContext();
                cmd.iCommandId = (int)CommandID.PopMiniPA_MainFrame;
                cmd.sReceiver = "MainFrame_FE@@";
                ComProxyManager.GetCurrentProxy().AsyncSendCommand(cmd);
            }catch(Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
          

        }



    }
}
