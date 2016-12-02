using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UIH.Mcsf.Utility;
using UIH.Mcsf.Viewer;
using UIH.Mcsf.App.Common;

namespace UIH.Mcsf.Filming
{
    /// <summary>
    /// Interaction logic for mcsf_review_2d_custom_rotate.xaml
    /// </summary>
    public partial class CustomRotation : UserControl
    {
        private FilmingPageCollection _activeFilmingPageList;
        private MedViewerControlCell _zoomViewerCell;
        public CustomRotation()
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

        public void InitialZoomViewerCell(MedViewerControlCell zoomCell)
        {
            _zoomViewerCell = zoomCell;
        }

        private double _value = 0;

        private void Button_Rotate_Confirm_Click(object sender, RoutedEventArgs e)
        {
            try{
                
                if (rotateAngle.Text == "非数字" || !Double.TryParse(rotateAngle.Text, out _value))
                    return;
                else
                {
                    if (true == ccw.IsChecked)
                    {
                        _value = -(_value % 360);
                    }
                    else
                        _value = _value % 360;
                }
                Logger.Instance.LogPerformanceRecord("[Action][CustomRotate][Begin]");
                foreach (var filmingPage in _activeFilmingPageList)
                {
                    foreach (var selectedCell in filmingPage.SelectedCells())
                    {
                        var cmd = new CmdRotateCustom(_value);
                        selectedCell.ExecuteCommand(cmd);

                        //selectedCell.Refresh();
                    }
                }
                
                if (_zoomViewerCell != null)
                {
                    var cmd = new CmdRotateCustom(_value);
                    _zoomViewerCell.ExecuteCommand(cmd);
                    _zoomViewerCell.Refresh();
                }
                Logger.Instance.LogPerformanceRecord("[Action][CustomRotate][End]");
            }
            catch (Exception exp)
            {
                Logger.LogFuncException("Exception: " + exp.ToString());
            }
        }

        private void Button_Rotate_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.CloseParentDialog();
        }

        private void rotateAngle_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {                
                if (e.Key == Key.Enter)
                {                    
                    if (rotateAngle.Text == "非数字" || !Double.TryParse(rotateAngle.Text, out _value))
                        return;
                    else
                    {
                        if (true == ccw.IsChecked)
                        {
                            _value = -(_value % 360);
                        }
                        else
                            _value = _value % 360;
                    }
                    Logger.Instance.LogPerformanceRecord("[Action][CustomRotate][Begin]");
                    foreach (var filmingPage in _activeFilmingPageList)
                    {
                        foreach (var selectedCell in filmingPage.SelectedCells())
                        {
                            var cmd = new CmdRotateCustom(_value);
                            selectedCell.ExecuteCommand(cmd);

                            selectedCell.Refresh();
                        }
                    }
                   
                    if (_zoomViewerCell != null)
                    {
                        var cmd = new CmdRotateCustom(_value);
                        _zoomViewerCell.ExecuteCommand(cmd);
                        _zoomViewerCell.Refresh();
                    }

                    Logger.Instance.LogPerformanceRecord("[Action][CustomRotate][End]");
                }

            }

            catch (System.Exception ex)
            {
                Logger.LogFuncException("Exception in rotateAngle_KeyDown" + ex.ToString());
            }
        }

        private void rotateAngle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var change = new TextChange[e.Changes.Count];
                e.Changes.CopyTo(change, 0);

                int offset = change[0].Offset;
                if (change[0].AddedLength > 0)
                {
                    if (!AngleValidate(rotateAngle.Text))
                    {
                        var caretIndex = rotateAngle.CaretIndex;
                        rotateAngle.Text = rotateAngle.Text.Remove(offset, change[0].AddedLength);
                        rotateAngle.CaretIndex = caretIndex - change[0].AddedLength;
                    }
                    else
                    {
                        rotateAngle.Text = rotateAngle.Text.Substring(0,
                            (rotateAngle.Text.IndexOf(".") == -1) ? rotateAngle.Text.Length : Math.Min(rotateAngle.Text.IndexOf(".") + 2, rotateAngle.Text.Length));
                        //rotateAngle.Text = rotateAngle.Text.Substring(0, rotateAngle.Text.IndexOf('.'));
                    }
                }
                e.Handled = true;
                rotateAngle.SelectionStart = rotateAngle.Text.Length;
            }
            catch (System.Exception ex)
            {
                Logger.LogFuncException("Exception in rotateAngle_TextChanged" + ex.ToString());
            }
        }
        private bool AngleValidate(string newContent)
        {
            string testString = newContent;
            
            Regex regExpr = null;
            regExpr = new Regex(@"^-?(0|[1-9]\d*)?(\.\d*)?$", RegexOptions.Compiled);
            //regExpr = new Regex(@"^-?(0|[1-9]\d*)$", RegexOptions.Compiled);
            if (regExpr.IsMatch(testString))
                return true;
            return false;
        }
       
        private void rotateAngle_cwSelected(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (true == ccw.IsChecked)
                    {
                        cw.IsChecked = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogFuncException("Exception in rotateAngle_cwSelected" + ex.ToString());
            }
        }

        private void rotateAngle_ccwSelected(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (true == cw.IsChecked)
                    {
                        ccw.IsChecked = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogFuncException("Exception in rotateAngle_ccwSelected" + ex.ToString());
            }
        }
    }
}
