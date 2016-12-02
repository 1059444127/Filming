using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UIH.Mcsf.Viewer;
namespace UIH.Mcsf.Filming.Models
{
    public class PageGraphicOperationModel
    {
        #region[---Private Field---]

        private string _ellipseROIMenuMode = ((int)StatisticMode.All).ToString();
        private string _circleROIMenuMode = ((int)StatisticMode.All).ToString();
        private string _rectangleROIMenuMode = ((int)StatisticMode.All).ToString();
        private string _freehandROIMenuMode = ((int)StatisticMode.All).ToString();
        private string _splineROIMenuMode = ((int)StatisticMode.All).ToString();
        private string _poligonROIMenuMode = ((int)StatisticMode.All).ToString();

        #endregion

        #region [---Properties----]

        public Dictionary<StatisticGraphicType, StatisticMode> GraphicsStatisticItemsMode { get; set; }

        #endregion

        #region [---Public Methods---]

        public PageGraphicOperationModel()
        {
            GraphicsStatisticItemsMode=new Dictionary<StatisticGraphicType, StatisticMode>();
        }

        public void DeserializedFromXml(XmlNode currNode)
        {
            try
            {
                Logger.LogFuncUp();

                if (null == currNode)
                {
                    return;
                }

                var graphicMenuNode = currNode.SelectSingleNode(OffScreenRenderXmlHelper.FIlMING_PAGE_Graphics_ROI_MENU);

                GetAttributesValue(graphicMenuNode, OffScreenRenderXmlHelper.FIlMING_PAGE_Ellipse_ROI_MENU, ref _ellipseROIMenuMode);
                GetAttributesValue(graphicMenuNode, OffScreenRenderXmlHelper.FIlMING_PAGE_Freehand_ROI_MENU, ref _freehandROIMenuMode);
                GetAttributesValue(graphicMenuNode, OffScreenRenderXmlHelper.FIlMING_PAGE_Rectangle_ROI_MENU, ref _rectangleROIMenuMode);
                GetAttributesValue(graphicMenuNode, OffScreenRenderXmlHelper.FIlMING_PAGE_Circle_ROI_MENU, ref _circleROIMenuMode);
                GetAttributesValue(graphicMenuNode, OffScreenRenderXmlHelper.FIlMING_PAGE_Poligon_ROI_MENU, ref _poligonROIMenuMode);
                GetAttributesValue(graphicMenuNode, OffScreenRenderXmlHelper.FIlMING_PAGE_Spline_ROI_MENU, ref _splineROIMenuMode);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            finally
            {
                GraphicsStatisticItemsMode.Add(StatisticGraphicType.Circle2D, (StatisticMode)(Int32.Parse(_circleROIMenuMode)));
                GraphicsStatisticItemsMode.Add(StatisticGraphicType.Ellipse2D, (StatisticMode)(Int32.Parse(_ellipseROIMenuMode)));
                GraphicsStatisticItemsMode.Add(StatisticGraphicType.FreeHand2D, (StatisticMode)(Int32.Parse(_freehandROIMenuMode)));
                GraphicsStatisticItemsMode.Add(StatisticGraphicType.Rectangle2D, (StatisticMode)(Int32.Parse(_rectangleROIMenuMode)));
                GraphicsStatisticItemsMode.Add(StatisticGraphicType.Polygon2D, (StatisticMode)(Int32.Parse(_poligonROIMenuMode)));
                GraphicsStatisticItemsMode.Add(StatisticGraphicType.Spline2D, (StatisticMode)(Int32.Parse(_splineROIMenuMode)));
            }
        }

        private void GetAttributesValue(XmlNode node, string attribute, ref string result)
        {
            if (result == null) throw new ArgumentNullException("result");

            if (ExistedAttribute(node, attribute))
            {
                result = node.Attributes[attribute].Value;
            }
            else
            {
                result = ((int)StatisticMode.All).ToString();
            }
        }

        protected bool ExistedAttribute(XmlNode node, string propName)
        {
            if (node.Attributes[propName] != null && node.Attributes[propName].Value != null)
                return true;
            else
                return false;
        }
        #endregion
    }
}
