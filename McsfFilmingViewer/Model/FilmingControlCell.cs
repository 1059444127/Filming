using System.Windows.Input;
using System.Windows.Media;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Model
{
    public class FilmingControlCell : MedViewerControlCell
    {
        private bool _isNull = false;

        public bool IsNull
        {
            get { return _isNull; }
            set { _isNull = value; }
        }

        public FilmingControlCell()
        {

        }

        //  public override void SetAction(UIH.Mcsf.Viewer.ActionType type, System.Windows.Input.MouseButton e)
        public override void SetAction(ActionType type, MouseButton e)
        {
            if (IsEmpty)
                return;

            if (Image == null || Image.CurrentPage == null) return;

            // SSFS 417639: SSFS_Review_2D_Common_SaveAsSC_ROI_EditUnable
            // Second Capture Images refused to add graphics such as line, Ellipse
            // 注意，如果ActionType枚举定义的顺序或者值发生改变，此处可能会引起bug
            if (this.Image.CurrentPage.IsSecondCapture
                && (int)type > (int)ActionType.Enhance && (int)type < (int)ActionType.Magnifier)
            {
                base.SetAction(ActionType.Empty, e);
            }
            else
            {
                base.SetAction(type, e);
            }

        }
        //isYellow为true，边框黄色，为false边框为淡蓝色
        public void SetCellFocusSelected(bool isSelected, bool isYellow = false)
        {
            this.IsSelected = isSelected;
            var filmingControlCellImpl = this.Control as FilmingControlCellImpl;
            if (filmingControlCellImpl == null) return;
            filmingControlCellImpl.Border.Stroke = !isYellow
                                                   ? new SolidColorBrush(Color.FromRgb(0, 255, 255))
                                                   //: new SolidColorBrush(Color.FromRgb(255, 190, 60));
                                                   : new SolidColorBrush(Color.FromRgb(255, 255, 0));
            filmingControlCellImpl.Border.StrokeThickness = !isYellow
                                       ? 2
                                       : 1;
        }

        public  void ReloadFilmingImageTextConfig()
        {
            if (null == this.Image || null == this.Image.CurrentPage)
            {
                return;
            }
            var overlayFilmingF1ProcessText =
                this.Image.CurrentPage.GetOverlay(OverlayType.FilmingF1ProcessText) as OverlayFilmingF1ProcessText;
            if (null != overlayFilmingF1ProcessText)
            {
                var printerImageTextConfigContent = "";
                if (Printers.Instance.Modality2FilmingImageTextConfigContent.ContainsKey(this.Image.CurrentPage.Modality.ToString()))
                {
                    printerImageTextConfigContent = Printers.Instance.Modality2FilmingImageTextConfigContent[this.Image.CurrentPage.Modality.ToString()];
                }
                overlayFilmingF1ProcessText.ReloadImageTextConfig(printerImageTextConfigContent);
            }
        }

        public void ReplaceBy(FilmingControlCell newCell,bool needReferesh, ActionType curAction)
        {
            if (newCell.IsEmpty)
            {
                Image.Clear();
                Refresh();
            }
            var displaydata = newCell.Image.CurrentPage;
            bool flag = this.IsEmpty;
            this.Image.Clear();
            this.Image.AddPage(displaydata);
            this.IsSelected = newCell.IsSelected;
            if (flag) FilmPageUtil.SetAllActions(this, curAction);
            if (needReferesh)
            {
                Refresh();
            }
            else
                this.Image.CurrentPage.IsDirty = true;
        }
    }

    class FilmingLayoutCell : MedViewerLayoutCell
    {
        #region  [---Fields---]
        private bool _isSetDivideCell = false;
        #endregion


        #region  [---Constructors---]
        public FilmingLayoutCell()
        {

        }
        #endregion


        #region  [---Properties---]
        public bool IsSetDivideCell
        {
            get { return _isSetDivideCell; }
            set { _isSetDivideCell = value; }
        }
        #endregion

    }
}
