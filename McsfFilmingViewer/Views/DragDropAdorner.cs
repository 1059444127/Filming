using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using UIH.Mcsf.Filming;
using UIH.Mcsf.Viewer;
using System.Collections;
using UIH.Mcsf.AppControls.Viewer;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace UIH.Mcsf.Filming.Views
{
    internal class FilmingDragDropAdorner : Adorner
    {
        private ContentPresenter _contentPresenter;
        private double left;
        private double top;
        private AdornerLayer _adornerLayer;

        public FilmingDragDropAdorner(object dragDropData, UIElement adornedElement, AdornerLayer adornerLayer)
            : base(adornedElement)
        {
            try
            {
                this._adornerLayer = adornerLayer;

                this._contentPresenter = new ContentPresenter();
                BitmapSource dragDropDataBitMap = dragDropData as BitmapSource;
                Rectangle rectangle = new Rectangle();
                rectangle.Width = dragDropDataBitMap.Width;
                rectangle.Height = dragDropDataBitMap.Height;
                rectangle.Stroke = new SolidColorBrush(Color.FromRgb(0, 255, 255));
                var imageBrush = new ImageBrush();

                imageBrush.ImageSource = dragDropDataBitMap;
                rectangle.Fill = imageBrush;
                this._contentPresenter.Content = rectangle;
                this._contentPresenter.Opacity = 0.5;

                this._adornerLayer.Add(this);
            }
            catch (System.Exception ex)
            {
                Logger.LogFuncException("DragDropAdorner FilmingDragDropAdorner: " + ex.StackTrace);
            }
        }

        public void SetPosition(double left, double top)
        {
            this.left = left;
            this.top = top;
            if (this._adornerLayer != null)
            {
                this._adornerLayer.Update(this.AdornedElement);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this._contentPresenter.Measure(constraint);
            return this._contentPresenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this._contentPresenter.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this._contentPresenter;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(this.left, this.top));

            return result;
        }

        public void Detach()
        {
            this._adornerLayer.Remove(this);
        }

    }
}
