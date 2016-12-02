using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UIH.Mcsf.Filming.Interface;
using UIH.Mcsf.Filming.Models;
using UIH.Mcsf.Filming.ViewModels;
using UIH.Mcsf.Utility;
using UIH.Mcsf.Viewer;
using Environment = UIH.Mcsf.Filming.Configure.Environment;

namespace UIH.Mcsf.Filming.Views
{
    /// <summary>
    /// Interaction logic for ProtocolBindingLayoutControl.xaml
    /// </summary>
    public partial class ProtocolBindingLayoutControl
    {
        private ProtocolsViewModel _protocolsViewModel;
        private LayoutBindingModel _layoutBindingModel;

        public event EventHandler<EventArgs>  SaveBtnChanged;
        
        public bool IsChanged
        {
            get { return _protocolsViewModel.IsProtocolChanged; }
        }

        public void SaveProtocolLayouts()
        {
            _protocolsViewModel.SaveProtocolLayoutsHint();
        }

        public ProtocolBindingLayoutControl()
        {
            InitializeComponent();

	        var nlsDictionary = Environment.Instance.GetFilmingNlsDictionary();
            if (nlsDictionary != null)
            {
                Resources.MergedDictionaries.Add(nlsDictionary);
            }

            //xaml文件中找不到资源
	        NameColumn.Header = TryFindResource("UID_Filming_Protocol_Name");
	        LayoutColumn.Header = TryFindResource("UID_Filming_Layout");
            //searchImage.Source = new BitmapImage(new Uri("Images/search.png", UriKind.Relative));
            //ProtocolDataGrid.HorizontalScrollBarVisibility =  ScrollBarVisibility.Hidden;



            //InitialDataContext();
        }

        public void InitialDataContext(LayoutBindingModel layoutBindingModel)
        {
            SearchTextBox.Text = string.Empty;
            _layoutBindingModel = layoutBindingModel;
            _layoutBindingModel.LayoutChanged += LayoutBindingModelOnLayoutChanged;

            _protocolsViewModel = new ProtocolsViewModel();
            DataContext = _protocolsViewModel;
            _protocolsViewModel.SetLayout = _layoutBindingModel.Layout;
        }

        private void LayoutBindingModelOnLayoutChanged(object sender, LayoutEventArgs layoutEventArgs)
        {
            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[ProtocolBindingLayoutControl.LayoutBindingModelOnLayoutChanged]" + "[sender, layoutEventArgs]" );
            _protocolsViewModel.SetLayout = layoutEventArgs.Layout;
        }

        public void SetLayout(int columns, int rows)
        {
          //  _protocolsViewModel.SetLayout = LayoutFactory.Instance.CreateLayout(columns, rows);
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[ProtocolBindingLayoutControl.CancelButton_OnClick]" + "[sender, e]" );
            SearchTextBox.Text = string.Empty;
            this.CloseParentDialog();
        }

        private void OnProtocolDataGridPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[ProtocolBindingLayoutControl.OnProtocolDataGridPreviewMouseLeftButtonUp]" + "[sender, e]");

            if (e.ChangedButton != MouseButton.Left) return;
            if(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                return;
            if (_protocolsViewModel.SelectedProtocols.Count == 0) return;
            _layoutBindingModel.EndBindingSession();
        }


        private void OnProtocolDataGridPreviewMouseMove(object sender, MouseEventArgs e)
        {
            //dim 560227 不支持拖拽多选
            var grid = e.Source as DataGrid;
            if (grid != null)
            {
                DisableRowDraggingSelection(grid);
            }
        }

        
        private void DisableRowDraggingSelection(DataGrid dataGrid)
        {
            //Set _isDraggingSelection and disable system native drag selection feature.
            var property = typeof(DataGrid).GetField(
                                                      "_isDraggingSelection",
                                                      BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
            if (property != null)
            {
                property.SetValue(dataGrid, false);
            }
        }

        
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            _protocolsViewModel.SaveProtocolLayoutsHint();

            OnRaiseSaveChanged();
            SearchTextBox.Text = string.Empty;
            this.CloseParentDialog();

        }

        private void OnRaiseSaveChanged()
        {
            try
            {
                EventHandler<EventArgs> handler = SaveBtnChanged;
                if (handler != null)
                {
                    handler(this, new EventArgs());
                }
            }
            catch (Exception exp)
            {
                Logger.Instance.LogDevError("ProtocolBindingLayoutControl[OnRaiseSaveChanged]::"+exp.Message);
            }
        }
    }
}
