using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace UIH.Mcsf.Filming.Models
{
    public class PrinterSettingInfoModel
    {

        #region [---Constructors---]
        #endregion



        #region [---Properties---]

        public string CurrPrinterAE { get; private set; }
        public string CurrFilmSize { get; private set; }
        public string CurrOrientation { get; private set; }
        public string CurrCopyCount { get; private set; }
        public string CurrMediumType { get; private set; }
        public string CurrFilmDestination { get; private set; }
        public string CurrPrinterDPI { get; set; }
        public string IfSaveHighPrecisionEFilm { get; set; }
        public string IfSave { get; private set; }
        public string IfColorPrint { get; private set; }
        //public string IfSaveHighPrecisionEFilm { get; private set; }

        #endregion



        #region [---Public Methods---]

        public void DeserializedFromXml(XmlNode currNode)
        {
            try
            {
                Logger.LogFuncUp();

                if (null == currNode)
                {
                    return;
                }

                this.CurrPrinterAE = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.CURRENT_PRINTER_AE);
                this.CurrFilmSize = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.CURRENT_FILM_SIZE);
                this.CurrOrientation = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.CURRENT_ORIENTATION);
                this.CurrCopyCount = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.CURRENT_COPY_COUNT);
                this.CurrMediumType = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.CURRENT_MEDIUM_TYPE);
                this.CurrFilmDestination = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.CURRENT_FILM_DESTINATION);
                this.CurrPrinterDPI = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.CURRENT_PRINTER_DPI);
                this.IfSaveHighPrecisionEFilm = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.IF_SAVE_HIGH_PRECISION_EFILM);
                this.IfSave = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.IF_SAVE_EFILM_WHEN_FILMING);
                this.IfColorPrint = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.IF_COLOR_PRINT);


                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        #endregion
    }
}
