using System;
using System.Collections.Generic;
using System.Xml;
using McsfCommonSave;

namespace UIH.Mcsf.Filming.Models
{
    public class PageModel
    {

        #region [---Constructors---]

        public PageModel(PrinterSettingInfoModel printerSettingInfo)
        {
            this.Layout = string.Empty;
            this.PageTitleInfoModel = new PageTitleInfoModel();
            this.PrinterSettingInfoModel = printerSettingInfo;
            this.PageGraphicOperationModel = new PageGraphicOperationModel();
            this.CellModels = new List<CellModel>();
        }

        #endregion



        #region [---Properties---]

        public string Layout { get; private set; }
        public List<CellModel> CellModels { get; private set; }
        public EFilmModel EFilmModel { get; set; }
        public PrinterSettingInfoModel PrinterSettingInfoModel { get; private set; }
        public PageTitleInfoModel PageTitleInfoModel { get; private set; }
        public PageGraphicOperationModel PageGraphicOperationModel { get; private set; }
        #endregion



        #region [---Public Methods---]

        public bool DeserializedFromXml(XmlNode currNode)
        {
            try
            {
                Logger.LogFuncUp();

                if (null == currNode)
                {
                    Logger.LogWarning("[FilmingSerivceFE]No page info of a page for print");
                    return false;
                }

            this.Layout = currNode.Attributes[OffScreenRenderXmlHelper.FILMING_PAGE_LAYOUT].Value;

            var filmPageTitileInfoNode = currNode.SelectSingleNode((OffScreenRenderXmlHelper.FILMING_PAGE_TITLE_INFO));
            if (null != filmPageTitileInfoNode)
            {
                this.PageTitleInfoModel.DeserializedFromXml(filmPageTitileInfoNode);
            }

            //Ω∫∆¨Õº‘™…Ë÷√œÓ
            var filmingPageGraphicOperation = currNode.SelectSingleNode(OffScreenRenderXmlHelper.FIlMING_PAGE_Graphic_OPERATION);
            this.PageGraphicOperationModel.DeserializedFromXml(filmingPageGraphicOperation);
            
            var imageDataNodes = currNode.SelectNodes(OffScreenRenderXmlHelper.IMAGE_DATA);
            if (null == imageDataNodes || imageDataNodes.Count <= 0)
            {
                Logger.LogWarning("[FilmingSerivceFE]No image info of a page for print");
                return false;
            }

            foreach (XmlNode imageDataNode in imageDataNodes)
            {
                var cellModel = new CellModel();
                this.CellModels.Add(cellModel);
                if (!cellModel.DeserializedFromXml(imageDataNode))
                {
                    Logger.LogWarning("[FilmingSerivceFE]Fail to get a cell info of a film page");
                    return false;
                }
            }

                this.EFilmModel.FillTagsFrom(this);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                return false;
            }

            return true;

        }

        #endregion


    }
}
