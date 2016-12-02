using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using UIH.Mcsf.Filming.Utility;
using System.Windows.Automation;

namespace UIH.Mcsf.Filming
{
    /// <summary>
    /// Interaction logic for FilmingMIPresetCellLayoutPanel.xaml
    /// </summary>
    public partial class FilmingMIPresetCellLayoutPanel : UserControl
    {
        private FilmingCard Card
        {
            get 
            { 
                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                return filmingCard;
            }
        }
        
        public FilmingMIPresetCellLayoutPanel()
        {
            InitializeComponent();
            if (FilmingViewerContainee.FilmingResourceDict != null)
            {
                Resources.MergedDictionaries.Add(FilmingViewerContainee.FilmingResourceDict);
            }
        }

        private List<FilmLayout> _cellLayouts = new List<FilmLayout>(); 

        public void Initialize()
        {            
            //if (Card != null)
            //{
            //    Card.layoutCtrl.PropertyChanged += UpdateIsEnableChangeCellLayout;
            //}
            
             var cellLayoutCount = Printers.Instance.PresetCellLayouts.Count;

             for (int i = 0; i < cellLayoutCount; i++)
             {
                 try
                 {
                     CreateLayoutCellButton(i, cellLayoutCount);                     
                 }
                 catch (Exception e)
                 {
                     Logger.LogWarning(e.Message + "No preset layout of index " + (i + 1));
                 }            
                
             }
        }

        private void CreateLayoutCellButton(int i, int cellLayoutCount)
        {
            Button btnCellLayout = new Button();
            btnCellLayout.Name = "cellLayoutButton" + (i + 1);
            AutomationProperties.SetAutomationId(btnCellLayout, "ID_BTN_FILMING_CELLLAYOUT" + (i + 1));
            btnCellLayout.Width = 35;
            btnCellLayout.Height = 35;
            btnCellLayout.Click += new RoutedEventHandler(btnCellLayout_Click);
            var style = "Style_Button_Common_CSW_Center";
            if (i == 0)
            {
                style = "Style_Button_Common_CSW_Left";
            }
            if (i == cellLayoutCount - 1)
            {
                style = "Style_Button_Common_CSW_Right";
            }
            btnCellLayout.Style = Card.FindResource(style) as Style;
            btnCellLayout.FontSize = 12;
            btnCellLayout.Focusable = false;

            //Binding b1 = new Binding("IsEnabled") { Source = ViewModel.IsEnablePresetCellLayoutButton };
            //btnCellLayout.SetBinding(Button.IsEnabledProperty, b1);
            var cellLayout = Printers.Instance.PresetCellLayouts[i];
            int row = cellLayout.Row;
            int col = cellLayout.Col;
            btnCellLayout.Tag = i + 1;
            SetLayoutCellButtonAttribute(btnCellLayout, i + 1, row, col);
            layoutCellPanel.Children.Add(btnCellLayout);
            _cellLayouts.Add(new FilmLayout(row, col));
        
        }

        private void SetLayoutCellButtonAttribute(Button btn, int index, int row, int col)
        {
            //Image img = new Image();
            //string imgPath = string.Format("layout{0}", col.ToString() + "x" + row.ToString());
            //img.Source = Card.FindResource(imgPath) as ImageSource;
            var layoutName = col.ToString() + "x" + row.ToString();
            btn.Content = layoutName;
            btn.ToolTip = col.ToString() + "x" + row.ToString() + (this.Resources["UID_Filming_PresetCellLayout"] as string);
        }

        private void UpdateCellLayoutButton(int index,int row,int col)
        { 
            var findBtnName="cellLayoutButton" + index;
            var findBtn= new Button();
            bool isFindBtn = false;
            foreach (var item in layoutCellPanel.Children)
            {
                var itemBtn = item as Button;
                if(itemBtn!=null)
                { 
                    if(itemBtn.Name==findBtnName){
                        findBtn = itemBtn;
                        isFindBtn = true;
                        break;
                    }
                }
            }

            if (isFindBtn)
            {
                SetLayoutCellButtonAttribute(findBtn,index, row, col);
            }
        
        }

        void btnCellLayout_Click(object sender, RoutedEventArgs e)
        {
            var index =(int) (sender as Button).Tag;
            SetLayoutByIndex(index);          
        }

        public void UpdateButtonStatus(int index, int row, int col)
        {
            if (row <= 0 || col <= 0 || row > FilmLayout.MaxRowCount || col > FilmLayout.MaxColCount) return;

            if(index >0 && index <=_cellLayouts.Count)
                _cellLayouts[index-1] = new FilmLayout(row, col);
            UpdateCellLayoutButton(index, row, col);            
        }

        //private void UpdateIsEnableChangeCellLayout(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "IsEnableChangeCellLayout")
        //    {
        //        ViewModel.IsEnablePresetCellLayoutButton = (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).layoutCtrl.IsEnableChangeCellLayout;
        //    }
        //}

        private void SetLayoutByIndex(int index)
        {
            try
            {
                Logger.LogFuncUp();
                if (Card != null)
                {
                    Card.layoutCtrl.CellLayoutActiveFilmingPages(_cellLayouts[index - 1]);
                }

                Logger.LogFuncDown();

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }
    }
}
