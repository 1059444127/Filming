using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using UIH.Mcsf.Filming.Command;

using UIH.Mcsf.Controls;
using UIH.Mcsf.Filming.Configure;
using UIH.Mcsf.Filming.Interface;
using UIH.Mcsf.Filming.Models;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.Filming.Views;
using UIH.Mcsf.MHC;
using UIH.Mcsf.Utility;
using UIH.Mcsf.Viewer;
using Environment = UIH.Mcsf.Filming.Configure.Environment;
using System.Windows.Automation;


namespace UIH.Mcsf.Filming
{
    /// <summary>
    /// Interaction logic for CustomCellLayoutWindow.xaml
    /// </summary>
    public partial class CustomCellLayoutWindow : UserControl
    {
        public delegate void CustomCellLayoutChanged(object sender, CustomCelllLayoutChangeEventArgs e);

        public event CustomCellLayoutChanged RaiseCustomCellLayoutChanged;

        private const int MaxRowCount = FilmLayout.MaxRowCount;
        private const int MaxColumnCount = FilmLayout.MaxColCount;

        public CustomCellLayoutWindow()
        {
            InitializeComponent();
            if (FilmingViewerContainee.FilmingResourceDict != null)
            {
                Resources.MergedDictionaries.Add(FilmingViewerContainee.FilmingResourceDict);
            }
            this.Loaded += new RoutedEventHandler(CellLayoutWindowLoaded);

            _messageHandler = MessageBoxHandler.MessageHanlder;
            _layoutBindingModel.BindingSessionEnded += LayoutBindingModelOnBindingSessionEnded;

            InitCustomCellLayoutUI();
           
        }

        private void InitCustomCellLayoutUI()
        {           
            for(var i=1;i<8;i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Name="btnApplytoLayout"+i;
                AutomationProperties.SetAutomationId(item, "ID_CMB_FILMING_APPLYTOLAYOUT" + i);
                item.Selected += new RoutedEventHandler(ApplytoLayoutItem_Selected);
                item.Content =Resources["UID_Filming_ComboBox_ApplytoLayout"] as string +i;
                item.HorizontalContentAlignment = HorizontalAlignment.Center;
                item.VerticalContentAlignment = VerticalAlignment.Center;
                item.ToolTip=Resources["UID_Filming_ComboBox_ApplytoLayoutToolTip"] as string +i;
                item.Width = 115;
                item.Height = 30;
                item.TabIndex=i;
                cmbPresetLayout.Items.Add(item);
            }

            ComboBoxItem itemDefault = new ComboBoxItem();
            itemDefault.Name = "btnApplytoDefaultLayout";
            AutomationProperties.SetAutomationId(itemDefault, "ID_CMB_FILMING_APPLYTODEFAULTLAYOUT");
            itemDefault.Selected += new RoutedEventHandler(ApplytoDefaultLayoutComboBoxSelected);
            itemDefault.Content = Resources["UID_Filming_ComboBox_ApplytoDefaultLayout"] as string;
            itemDefault.HorizontalContentAlignment = HorizontalAlignment.Center;
            itemDefault.VerticalContentAlignment = VerticalAlignment.Center;
            itemDefault.ToolTip = Resources["UID_Filming_ComboBox_ApplytoDefaultLayoutToolTip"] as string;
            itemDefault.Width = 110;
            itemDefault.Height = 30;
            itemDefault.TabIndex = 0;
            cmbPresetLayout.Items.Add(itemDefault);

            if (FilmingHelper.IsEnablePresetLayoutBar)
            {
                cmbPresetLayout.IsEnabled = true;
                cmbPresetLayout.Visibility = Visibility.Visible;
            }
            else
            {
                cmbPresetLayout.IsEnabled = false;
                cmbPresetLayout.Visibility = Visibility.Collapsed;
            }

            ProtocolConfigureButton.Visibility = Environment.Instance.GetProtocolsConfig().Protocols.Any() ? Visibility.Visible : Visibility.Collapsed;

            preSettingButton.Content = (cmbPresetLayout.Items[0] as ComboBoxItem).Content;


        }

        private void ApplytoLayoutItem_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                ComboBoxItem item = sender as ComboBoxItem;
                int index = item.TabIndex;
                ApplytoLayoutByIndex(index);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private bool _bLeftButtonDown;
        private void CustomCellLayoutGridMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                Keyboard.Focus(applySettingButton);

                _bLeftButtonDown = true;
                ClearAllGrid();
                Point beginPoint = e.GetPosition(customCellLayoutGrid);

                VisualTreeHelper.HitTest(customCellLayoutGrid,
                    MyHitTestFilter,
                    MyHitTestResult,
                    new PointHitTestParameters(beginPoint));

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        
        public void Initialize()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
                                                       {
                                                           LayoutBindingModelOnBindingSessionEnded(null, null);
                                                           Keyboard.Focus(columnNumberLabel); }), DispatcherPriority.Background); 
        }

        public HitTestFilterBehavior MyHitTestFilter(DependencyObject o)
        {
            try
            {
                // Test for the object value you want to filter.
                if (o.GetType() != typeof(Grid))
                {
                    // Visual object and descendants are NOT part of hit test results enumeration.
                    return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
                }

                var grid = o as Grid;
                if (grid != null && !grid.Equals(customCellLayoutGrid))
                {
                    _beginHittedGrid = grid;
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
            // Visual object is part of hit test results enumeration.
            return HitTestFilterBehavior.Continue;
        }

        public HitTestFilterBehavior EndHitTestFilter(DependencyObject o)
        {
            try
            {
                // Test for the object value you want to filter.
                if (o.GetType() != typeof(Grid))
                {
                    // Visual object and descendants are NOT part of hit test results enumeration.
                   // _bLeftButtonDown = false;
                   return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
                   // return HitTestFilterBehavior.Stop;
                }

                var grid = o as Grid;
                if (grid != null && !grid.Equals(customCellLayoutGrid))
                {
                    _endHittedGrid = grid;
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
            // Visual object is part of hit test results enumeration.
            return HitTestFilterBehavior.Continue;
        }

        private Grid _beginHittedGrid;

        // Return the result of the hit test to the callback.
        public HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            // Add the hit test result to the list that will be processed after the enumeration.
            //hitResultsList.Add(result.VisualHit);

            // Set the behavior to return visuals at all z-order levels.
            return HitTestResultBehavior.Continue;
        }


        private Grid _endHittedGrid;
        private void CustomCellLayoutGridMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                customCellLayoutGrid.ReleaseMouseCapture();
                
                Point beginPoint = e.GetPosition(customCellLayoutGrid);

                VisualTreeHelper.HitTest(customCellLayoutGrid,
                                         EndHitTestFilter,
                                         MyHitTestResult,
                                         new PointHitTestParameters(beginPoint));
                if (_beginHittedGrid.Equals(_endHittedGrid))
                {
                    _beginHittedGrid.Background = new SolidColorBrush(Color.FromArgb(255, 156, 156, 156));
                    columnNumberLabel.Text = "1";
                    rowNumberLabel.Text = "1";
                }
                _bLeftButtonDown = false;
            }
            catch (Exception exception)
            {
                Logger.LogFuncException(exception.Message);
            }
        }

        private void CustomCellLayoutGridMouseLeave(object sender, MouseEventArgs e)
        {

            //Keyboard.Focus(applySettingButton);

            _bLeftButtonDown = false;
        }

        public int CustomCellRows
        {
            get
            {
                try
                {
                    return Convert.ToInt32(rowNumberLabel.Text);
                }
                catch (Exception)
                {
                    return 0;
                    //throw;
                }
            }
        }

        public int CustomCellColumns
        {
            get
            {
                try
                {
                    return Convert.ToInt32(columnNumberLabel.Text);
                }
                catch (Exception)
                {
                    return 0;
                    //throw;
                }
            }
        }

        public FilmLayout CustomCellLayout { get; private set; }

        private readonly SolidColorBrush _selectedCellColor = new SolidColorBrush(Color.FromArgb(255, 156, 156, 156));
        private readonly SolidColorBrush _selectedBorderColor = new SolidColorBrush(Color.FromArgb(255, 167, 167, 167));
        private readonly SolidColorBrush _plainCellColor = new SolidColorBrush(Color.FromArgb(255, 206, 206, 206));
        private readonly SolidColorBrush _plainBorderColor = new SolidColorBrush(Color.FromArgb(255, 226, 226, 226));


         

        private void CustomCellLayoutGridMouseMove(object sender, MouseEventArgs e)
        {
            
            try
            {
                if (_bLeftButtonDown)
                    {
                        customCellLayoutGrid.CaptureMouse();

                        Point beginPoint = e.GetPosition(customCellLayoutGrid);
                        Rect rect = VisualTreeHelper.GetDescendantBounds(customCellLayoutGrid);
                        if (rect.Contains(beginPoint))
                        {
                            VisualTreeHelper.HitTest(customCellLayoutGrid,
                                                 EndHitTestFilter,
                                                 MyHitTestResult,
                                                 new PointHitTestParameters(beginPoint));
                        }
                        else
                        {
                            //right
                            if (beginPoint.X > rect.Width && beginPoint.Y > 0 && beginPoint.Y < rect.Height)
                            {
                                int gridIndex = ((int) (beginPoint.Y/(rect.Height/MaxRowCount)) + 1)*MaxColumnCount - 1;
                                var childGrid = customCellLayoutGrid.Children[gridIndex] as Grid;
                                _endHittedGrid = childGrid;
                            }
                            //left
                            if (beginPoint.X < 0 && beginPoint.Y > 0 && beginPoint.Y < rect.Height)
                            {
                                int gridIndex = ((int) (beginPoint.Y/(rect.Height/MaxRowCount)))*MaxColumnCount;
                                var childGrid = customCellLayoutGrid.Children[gridIndex] as Grid;
                                _endHittedGrid = childGrid;
                            }
                            //bottom
                            if (beginPoint.Y > rect.Height && beginPoint.X > 0 && beginPoint.X < rect.Width)
                            {
                                int gridIndex = (int) (beginPoint.X/(rect.Width/MaxColumnCount)) +
                                                MaxColumnCount*(MaxRowCount - 1);
                                var childGrid = customCellLayoutGrid.Children[gridIndex] as Grid;
                                _endHittedGrid = childGrid;
                            }
                            //up
                            if (beginPoint.Y <0 && beginPoint.X > 0 && beginPoint.X < rect.Width)
                            {
                                int gridIndex = (int) (beginPoint.X/(rect.Width/MaxColumnCount));
                                var childGrid = customCellLayoutGrid.Children[gridIndex] as Grid;
                                _endHittedGrid = childGrid;
                            }
                        }

                        int beginRow = customCellLayoutGrid.Children.IndexOf(_beginHittedGrid) / MaxColumnCount;
                        int beginColumn = customCellLayoutGrid.Children.IndexOf(_beginHittedGrid) % MaxColumnCount;

                        int endRow = customCellLayoutGrid.Children.IndexOf(_endHittedGrid) / MaxColumnCount;
                        int endColumn = customCellLayoutGrid.Children.IndexOf(_endHittedGrid) % MaxColumnCount;

                        if (beginRow > endRow)
                        {
                            int tempRow = endRow;
                            endRow = beginRow;
                            beginRow = tempRow;
                        }

                        if (beginColumn > endColumn)
                        {
                            int tempColumn = endColumn;
                            endColumn = beginColumn;
                            beginColumn = tempColumn;
                        }

                       ClearAllGrid();
                        SelectRowsAndCols(endColumn, beginRow, beginColumn, endRow);

                        rowNumberLabel.Text = (endRow < beginRow ? 0 : endRow - beginRow + 1).ToString(CultureInfo.InvariantCulture);
                        columnNumberLabel.Text = (endColumn < beginColumn ? 0 : endColumn - beginColumn + 1).ToString(CultureInfo.InvariantCulture);

                        if (rowNumberLabel.Text.ToString() == "0" || columnNumberLabel.Text.ToString() == "0")
                        {
                            columnNumberLabel.Text = "0";
                            rowNumberLabel.Text = "0";
                        }
                    }
                else
                {
                    customCellLayoutGrid.ReleaseMouseCapture();
                }
            }
            catch (Exception exception)
            {
                Logger.LogFuncException(exception.Message);
            }
        }

        private void SelectRowsAndCols(int endColumn, int beginRow, int beginColumn, int endRow)
        {
            for (int rowIndex = beginRow; rowIndex <= endRow; rowIndex++)
            {
                for (int columnIndex = beginColumn; columnIndex <= endColumn; columnIndex++)
                {
                    int childGridIndex = rowIndex*MaxColumnCount + columnIndex;
                    var childGrid = customCellLayoutGrid.Children[childGridIndex] as Grid;
                    if (childGrid != null)
                    {
                        childGrid.Background = _selectedCellColor;
                    }
                    var childBorder =
                        customCellLayoutGrid.Children[MaxRowCount*MaxColumnCount + childGridIndex] as Border;
                    if (childBorder != null)
                    {
                        childBorder.BorderBrush = _selectedBorderColor;
                    }
                }
            }
        }

        private void ApplySettingButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                if(PreWarning())    return;

                CustomCellLayout = new FilmLayout(CustomCellRows, CustomCellColumns) { LayoutType = LayoutTypeEnum.StandardLayout };

                if (CustomCellLayout != null)
                {
                    var eventArgs = new CustomCelllLayoutChangeEventArgs(CustomCellLayout);

                    RaiseCustomCellLayoutChanged(this, eventArgs);
                }

                //Hide();
                //WindowHostManagerWrapper.CloseSecondaryWindow();
                this.CloseParentDialog();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        private bool PreWarning()
        {
            if (CustomCellRows > MaxRowCount || CustomCellColumns > MaxColumnCount)
            {
                //MessageBox.Show(FilmingNLSFactory.WarningBigThanMaxNumber);
                MessageBoxHandler.Instance.ShowWarning("UID_Filming_Warning_BigThanMaxNumber");

                rowNumberLabel.Text = MaxRowCount.ToString(CultureInfo.InvariantCulture);
                columnNumberLabel.Text = MaxColumnCount.ToString(CultureInfo.InvariantCulture);

                return true;
            }

            if (CustomCellColumns <= 0 || CustomCellRows <= 0)
            {
                //MessageBox.Show(FilmingNLSFactory.WarningLessThanMinNumber);
                MessageBoxHandler.Instance.ShowWarning("UID_Filming_Warning_NotCorrectNumber");

                return true;
            }

            return false;
        }
        private void CancelSettingButtonClick(object sender, RoutedEventArgs e)
        {
            //Hide();
            try
            {
                Logger.LogFuncUp();
                //WindowHostManagerWrapper.CloseSecondaryWindow(); 
                this.CloseParentDialog();
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

	    //private void OnClosingWindow(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    e.Cancel = true;
        //    Hide();
        //}

        private void SelectGrids(int row, int col)
        {
            ClearAllGrid();
            SelectRowsAndCols(col-1, 0, 0, row-1);
            
            //SetCellAndBorderColor(_selectedBorderColor, _selectedCellColor, 0, row, 0, col);
        }

        private void ClearAllGrid()
        {
            SetCellAndBorderColor(_plainBorderColor, _plainCellColor, 0, MaxRowCount, 0, MaxColumnCount);
        }

        private void SetCellAndBorderColor(SolidColorBrush borderColor, SolidColorBrush cellColor, int startRowIndex, int endRowIndext, int startColIndex, int endColIndex)
        {
            try
            {
                for (int rowIndex = startRowIndex; rowIndex < endRowIndext; rowIndex++)
                {
                    for (int columnIndex = startColIndex; columnIndex < endColIndex; columnIndex++)
                    {
                        int childGridIndex = rowIndex*endColIndex + columnIndex;
                        var childGrid = customCellLayoutGrid.Children[childGridIndex] as Grid;
                        if (childGrid != null)
                        {
                            childGrid.Background = cellColor;
                        }
                        var childBorder =
                            customCellLayoutGrid.Children[endRowIndext*endColIndex + childGridIndex] as Border;
                        if (childBorder != null)
                        {
                            childBorder.BorderBrush = borderColor;
                        }
                    }
                }
            }catch(Exception erro)
            {
                Logger.LogFuncException("SetCellAndBorderColor" + erro.ToString());
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            try
            {
                if (Double.IsNaN(availableSize.Width))
                {
                    Logger.LogFuncException("customCellLayoutGrid MeasureOverride erro ");
                    return new Size(1, 1);
                }
                if (Math.Abs(availableSize.Width - 0) < Double.Epsilon)
                {
                    Logger.LogFuncException("customCellLayoutGrid Width 0 ");
                    return new Size(1, availableSize.Height);
                }
                if (Math.Abs(availableSize.Height - 0) < Double.Epsilon)
                {
                    Logger.LogFuncException("customCellLayoutGrid Height 0 ");
                    return new Size(availableSize.Width, 1);
                }

                return availableSize;
            }
            catch (Exception erro)
            {
                Logger.LogFuncException("customCellLayoutGrid MeasureOverride: " + erro.StackTrace);
                return new Size(10, 10);
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var presetCmb = sender as ComboBox;
            if (presetCmb != null)
            {
                var presetItem = presetCmb.SelectedItem as ComboBoxItem;
                if (presetItem != null)
                {
                    preSettingButton.Content = presetItem.Content;
                }
            }
        }

        private void OnCurrentPresetClick(object sender, RoutedEventArgs e)
        {

            if (PreWarning()) return;

            var presetItem = cmbPresetLayout.SelectedItem as ComboBoxItem;
            if (presetItem != null)
            {
                int index = presetItem.TabIndex;
                if (index > 0)
                {
                    ApplytoLayoutByIndex(index);
                }else{
                    ApplytoDefaultLayout();
                }
            }
        }      


        private void ApplytoDefaultLayoutComboBoxSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ApplytoDefaultLayout())
                    FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Save_DefaultLayout_Succeed");
                else
                    FilmingViewerContainee.Main.ShowStatusWarning("UID_Filming_Save_DefaultLayout_Failed");

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void CellLayoutWindowLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(columnNumberLabel);
            this.Dispatcher.BeginInvoke(new Action(() => { _protocolBindingLayoutControl=new ProtocolBindingLayoutControl(); }));
        }

        public bool FoucusOnTextBox { get; private set; }

        private void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            FoucusOnTextBox = true;
            ClearforPt();
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.SelectAll();
                textBox.PreviewMouseDown -= new MouseButtonEventHandler(TextBoxPreviewMouseDown);
            }
        }
        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            FoucusOnTextBox = false;
            RecoverforPt();
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.PreviewMouseDown += new MouseButtonEventHandler(TextBoxPreviewMouseDown);
            }
        }

        private void TextBoxPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            if (e.ChangedButton == MouseButton.Left && !textBox.IsFocused)
            {
                textBox.Focus();
                e.Handled = true;
            }
        }

        private void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    ApplySettingButtonClick(null, null);
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogFuncException("Exception in TextBoxKeyDown" + ex.ToString());
            }
        }
        
        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var textBox = sender as TextBox;
                if (textBox == null) return;

                var change = new TextChange[e.Changes.Count];
                e.Changes.CopyTo(change, 0);

                int offset = change[0].Offset;
                if (change[0].AddedLength > 0)
                {
                    if (!Validate(textBox.Text))
                    {
                        var caretIndex = textBox.CaretIndex;
                        textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                        textBox.CaretIndex = caretIndex - change[0].AddedLength;
                    }
                    else
                    {
                        if (Convert.ToInt32(textBox.Text) > FilmLayout.MaxColCount)
                        {
                            textBox.Text = FilmLayout.MaxColCount.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }
                e.Handled = true;

                //if (_protocolBindingLayoutControl != null)
                    //_protocolBindingLayoutControl.SetLayout(CustomCellColumns, CustomCellRows);
                _layoutBindingModel.Layout = LayoutFactory.Instance.CreateLayout(CustomCellColumns, CustomCellRows);

                if (FoucusOnTextBox)
                    SelectGrids(CustomCellRows, CustomCellColumns);
            }
            catch (System.Exception ex)
            {
                Logger.LogFuncException("Exception in TextChanged" + ex.ToString());
            }
        }
        private bool Validate(string newContent)
        {
            string testString = newContent;

            Regex regExpr = null;
            regExpr = new Regex(@"^([1-9]|10)$", RegexOptions.Compiled);
            //regExpr = new Regex(@"^-?(0|[1-9]\d*)?(\.\d*)?$", RegexOptions.Compiled);
            //regExpr = new Regex(@"^-?(0|[1-9]\d*)$", RegexOptions.Compiled);
            if (regExpr.IsMatch(testString))
                return true;
            return false;
        }

        private void ApplytoLayoutByIndex(int index)
        {
            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag+"Index:"+index.ToString());
            var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
            if (filmingCard == null) return;
            try
            {
                var customCellRows = CustomCellRows;
                var customCellColumns = CustomCellColumns;
                if (customCellRows <= 0 || customCellColumns <= 0)
                {
                    Logger.Instance.LogDevWarning(string.Format("[Not set valid row or column][Col]{0}[Row]{1}", customCellColumns, customCellRows));
                    return;
                }

                var cellLayout = Printers.Instance.PresetCellLayouts[index - 1];
                cellLayout.SetLayout(index - 1, customCellRows, customCellColumns);
                filmingCard.layoutCtrl.UpdateMIPresetCellLayoutButton(index, customCellRows, customCellColumns);
            }
            catch (Exception e)
            {
                Logger.LogWarning(e.Message);
            }
            //if (FilmingHelper.SetPresetCellLayoutByIndex(index, CustomCellRows, CustomCellColumns))
            //{
            //    filmingCard.UpdateMIPresetCellLayoutButton(index, CustomCellRows, CustomCellColumns);
            //}
            //else
            //{
            //    MessageBoxHandler.Instance.ShowWarning("UID_Filming_Warning_NotCorrectNumber");
            //    return;
            //}
        }

        private bool ApplytoDefaultLayout()
        {
            if (!FilmLayout.DefinedFilmRegionLayoutList.Any()) return false;
            var defaultLayout = FilmLayout.DefinedFilmRegionLayoutList.First();
            if (CustomCellColumns == 0 || CustomCellRows == 0) return false;
            if(defaultLayout.LayoutColumnsSize == CustomCellColumns && defaultLayout.LayoutRowsSize == CustomCellRows) return true;
            Environment.Instance.GetDefaultLayoutConfigure().WriteBackToDefaultLayout(CustomCellRows,CustomCellColumns);
            return FilmLayout.RefreshDefaultLayout(CustomCellRows, CustomCellColumns);
        }
        private void ClearforPt()
        {
            var filmingcard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
            if (filmingcard != null)
            {
                filmingcard.ClearPtInputBindings();
            }
        }

        private void RecoverforPt()
        {
            var filmingcard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
            if (filmingcard != null &&
                filmingcard.CurrentActionType != ActionType.AnnotationText && filmingcard.CurrentActionType != ActionType.AnnotationArrow)
            {
                filmingcard.RecoverPtInputBindings();
            }
        }

        public void SetButtonState(bool isEnable)
        {
            this.applySettingButton.IsEnabled = isEnable;
            this.cancelSettingButton.IsEnabled = isEnable;
            this.preSettingButton.IsEnabled = isEnable;
            this.cmbPresetLayout.IsEnabled = isEnable;
        }

	    #region [--Protocol Binding Layout Configure--]

        private readonly MessageHanlder _messageHandler;
        private readonly LayoutBindingModel _layoutBindingModel = new LayoutBindingModel();
        ProtocolBindingLayoutControl _protocolBindingLayoutControl = null;

        private void LayoutBindingModelOnBindingSessionEnded(object sender, EventArgs eventArgs)
        {
            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[CustomCellLayoutWindow.LayoutBindingModelOnBindingSessionEnded]" + "[sender, eventArgs]" );
            rowNumberLabel.Text = string.Empty;
            columnNumberLabel.Text = string.Empty;
            if (sender!=null)
                ClearAllGrid();
        }

        private void ProtocolConfigureButton_OnClicked(object sender, RoutedEventArgs e)
	    {
            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[CustomCellLayoutWindow.ProtocolConfigureButton_OnClicked]" + "[sender, e]" );
            //protocolControl.Width = 300;
		    //Width = Width + 300;
            //MessageHanlder.SetWindowPos(MessageHanlder.HostedHWnd, MessageHanlder.HWND_TOPMOST, 300, 300, 300, 300,
            //    0);
            //_messageHandler.ShowHostedAppModelessForm(_protocolBindingLayoutControl, "title", null,
            //    ProtocolConfigureDialogClosed, null);
            
            _protocolBindingLayoutControl.InitialDataContext(_layoutBindingModel);
            _protocolBindingLayoutControl.SaveBtnChanged -= new EventHandler<EventArgs>(ProtocolBindingLayoutControl_SaveBtnChanged);
            _protocolBindingLayoutControl.SaveBtnChanged += new EventHandler<EventArgs>(ProtocolBindingLayoutControl_SaveBtnChanged);
          

            var messageWindow = new MessageWindow
            {
                WindowTitle = Resources["UID_Filming_Protocol"] as string,//Card.Resources["UID_Filming_CustomCellLayout_Setting_Title"] as string,
                WindowChild = _protocolBindingLayoutControl,
                WindowDisplayMode = WindowDisplayMode.Default
            };

            //Card.HostAdornerCount++;
            messageWindow.Closing -= ProtocolConfigureDialogClosed;
            messageWindow.Closing += ProtocolConfigureDialogClosed;
            messageWindow.ShowModelessDialog(this.FindAncestorWindow());

            messageWindow.Top = 339;
            messageWindow.Left +=400 ;

            var wControl = this.Parent as WindowHostControl;
            if (wControl != null)
            {
                wControl.OnClosing -= CloseProtoclWindow;
                wControl.OnClosing += CloseProtoclWindow;
            }
            //WindowHostManager.HostAdorner.OnClosing -= ProtocolConfigureDialogClosed;
            //WindowHostManager.HostAdorner.OnClosing += ProtocolConfigureDialogClosed;
            //WindowHostManager.Content = _protocolBindingLayoutControl;
            //WindowHostManager.HostAdorner.Title = "Title";
            //WindowHostManager.Show();

            ProtocolConfigureButton.IsEnabled = false;
            SetButtonState(false);
	    }


        private void ProtocolConfigureDialogClosed(object sender, CancelEventArgs args)
        {
           
            //_protocolBindingLayoutControl = null;
            //Card.HostAdornerCount--;

            bool isClose = true;
            _bProtocolSettingClosing = true;
            if (_protocolBindingLayoutControl!=null && _protocolBindingLayoutControl.IsChanged)
            {
                MessageBoxType messagetype = MessageBoxType.YesNoCancel;
                MessageBoxResponse rs = MessageBoxHandler.Instance.ShowQuestion("UID_Filming_Protocol_CloseHint",new MsgResponseHander(CloseHandler), messagetype, "UID_MessageBox_Title_Info", _protocolBindingLayoutControl.FindAncestorWindow());
                if (rs == MessageBoxResponse.CANCEL || rs== MessageBoxResponse.NONE)
                {
                    args.Cancel = true;
                    isClose = false;
                    _bProtocolSettingClosing = false;
                }

            }

            SetButtonState(isClose);

            if (isClose)
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[CustomCellLayoutWindow.ProtocolConfigureDialogClosed]" + "[sender, args]");
                ProtocolConfigureButton.IsEnabled = true;
                Keyboard.Focus(columnNumberLabel);
            }

            
        }

        private bool _bClosing = false;
        private bool _bProtocolSettingClosing = false;
        public void CloseProtoclWindow(object sender,CancelEventArgs args)
        {
            var messageWindow = this._protocolBindingLayoutControl.FindAncestorWindow();
            if (messageWindow == null || !_protocolBindingLayoutControl.IsVisible)
            {
                _bClosing = false; 
                return; 
            }

            if (!_bClosing)
            {
                _bClosing = true;
                messageWindow.Close();
                args.Cancel = true;
            }
            else
            {
                args.Cancel = !_bProtocolSettingClosing;
                _bClosing = false;
            }

        }

        private void CloseHandler(MessageBoxResponse response = MessageBoxResponse.OK)
        {
            try
            {
                Logger.LogFuncUp();

                if (response != MessageBoxResponse.YES)
                {
                    return;
                }
                _protocolBindingLayoutControl.SaveProtocolLayouts();
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                //throw;
            }
        }



        public void ProtocolBindingLayoutControl_SaveBtnChanged(object sender, EventArgs e)
        {
            FilmingViewerContainee.Main.BroadcastEvent("FilmingFE", (int)CommandID.Update_Protocal_Content, "");
            MessageBoxHandler.Instance.ShowInfoOwnerWin("UID_Filming_Protocol_SaveSuccess", _protocolBindingLayoutControl.FindAncestorWindow());
        }

	    #endregion //[--Protocol Binding Layout Configure--]


    }

    public class CustomCelllLayoutChangeEventArgs : EventArgs
    {
        public CustomCelllLayoutChangeEventArgs(FilmLayout customFilmLayout)
        {
            CustomFilmLayout = customFilmLayout;
        }

        public FilmLayout CustomFilmLayout { get; private set; }
    }

    public class GridHelper{

        public static bool GetShowBorder(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowBorderProperty);
        }

        public static void SetShowBorder(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowBorderProperty, value);
        }

        public static readonly DependencyProperty ShowBorderProperty =
            DependencyProperty.RegisterAttached("ShowBorder", typeof(bool), typeof(GridHelper),
                                                new PropertyMetadata(OnShowBorderChanged));


        private static void OnShowBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as Grid;
            if ((bool)e.OldValue)
            {
                grid.Loaded -= (s, arg) => { };
            }
            if ((bool)e.NewValue)
            {
                grid.Loaded += (s, arg) =>
                                   {
                                       var rows = grid.RowDefinitions.Count;
                                       var columns = grid.ColumnDefinitions.Count;

                                       for (int i = 0; i < rows; i++)
                                       {
                                           for (int j = 0; j < columns; j++)
                                           {
                                               var border = new Border()
                                                                {
                                                                    BorderBrush =
                                                                        new SolidColorBrush(Color.FromArgb(255, 226, 226, 226)),
                                                                    BorderThickness = new Thickness(1)
                                                                };
                                               Grid.SetRow(border, i);
                                               Grid.SetColumn(border, j);

                                               grid.Children.Add(border);
                                           }
                                       }
                                   };
            }
        }
    }
}




