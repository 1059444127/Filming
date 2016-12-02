using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace UIH.Mcsf.Filming
{
    public class OffScreenRenderXmlHelper
    {
        public static readonly string DataHeaderFilePath = "E:\\Images\\TempOfFilmingPrint\\";

        public static readonly string PRINT_JOB_INFO = "PrintJob";
        public static readonly string OPERATION = "Operation";



        #region  [---PrinterSettings Tag Name--]

        public static readonly string PRINTER_SETTING_INFO = "PrinterSettings";

        public static readonly string CURRENT_PRINTER_AE = "CurrPrinterAE";
        public static readonly string CURRENT_FILM_SIZE = "CurrFilmSize";
        public static readonly string CURRENT_ORIENTATION = "CurrOrientation";
        public static readonly string CURRENT_COPY_COUNT = "CurrCopyCount";
        public static readonly string CURRENT_MEDIUM_TYPE = "CurrMediumType";
        public static readonly string CURRENT_FILM_DESTINATION = "CurrFilmDestination";
        public static readonly string CURRENT_PRINTER_DPI = "CurrPrinterDPI";
        public static readonly string IF_SAVE_HIGH_PRECISION_EFILM = "IfClear";
        public static readonly string IF_SAVE_EFILM_WHEN_FILMING = "IfSave";
        public static readonly string IF_COLOR_PRINT = "IfColorPrint";


        #endregion



        #region  [---FilmingPageTitle Tag Name---]

        public static readonly string FILMING_PAGE_TITLE_SETTING = "FilmPageTitleSetting";
        public static readonly string FILMING_PAGE_TITLE_INFO = "FilmPageTitleInfo";

        public static readonly string DISPLAY_FONT = "DisplayFont";
        public static readonly string DISPLAY_POSITION = "DisplayPosition";
        public static readonly string PATIENT_NAME_FLAG = "PatientNameFlag";
        public static readonly string PATIENT_ID_FLAG = "PatientIdFlag";
        public static readonly string AGE_FLAG = "AgeFlag";
        public static readonly string SEX_FLAG = "SexFlag";
        public static readonly string STUDY_DATE_FLAG = "StudyDateFlag";
        public static readonly string HOSPITAL_INFO_FLAG = "HospitalInfoFlag";
        public static readonly string COMMENT = "Comment";
        public static readonly string PAGE_NO = "PageNo";
        public static readonly string ACCESSION_NO = "AccessionNo";
        public static readonly string LOGO = "Logo";
        

        #endregion



        #region  [---Image Text---]

        public static readonly string IMG_TEXT_FILE_PATH_OR_CONTENT = "TextFilePathOrContent";
        public static readonly string IMG_TEXT_ITEM_PATH_OR_CONTENT = "TextItemPathOrContent";
        public static readonly string IMG_TEXT_CONTENT = "TextContent";
        public static readonly string IMG_TEXT_POSITION = "ImgTextPosition";
        public static readonly string MG_Orientation_IsShow = "MgOrientationIsShow";
        public static readonly string Is_Show_ImgText = "IsShowImgTxt";
        public static readonly string Is_Show_Ruler = "IsShowRuler";

        public static readonly string PT_UNIT = "PtUnit";

        #endregion



        #region [---FilmingCard Tag Name---]

        public static readonly string ALL_FILMING_PAGE_INFO = "AllFilmPages";
        
        #endregion



        #region [---FilmingPage Tag Name---]

        public static readonly string FIlMING_PAGE_INFO = "FilmPage";
        public static readonly string FILMING_PAGE_LAYOUT = "Layout";
        public static readonly string IMAGE_DATA = "ImageData";
        public static readonly string SOP_UID = "SopUID";
        public static readonly string LOCALIZED_SOP_UID = "LocalizedSopUID";
        public static readonly string LOCALIZED_IMAGE_PS_INFO = "LocalizedImagePSInfo";
        public static readonly string LOCALIZED_IMAGE_POSITION = "LocalizedImagePosition";
        public static readonly string SCALE_RULER = "ScaleRuler";
        public static readonly string ISAPP = "IsApp";
        public static readonly string DATA_HEADER_FILE_PATH = "DataHeaderFilePath";
        public static readonly string DATA_HEADER_LENGTH = "DataHeaderLength";

        public static readonly string FILMING_PAGE_SIZE = "FilmingPageSize";
        public static readonly string FILMING_PAGE_HEIGHT = "FilmingPageHeight";
        public static readonly string FILMING_PAGE_WIDTH = "FilmingPageWidth";
        public static readonly string FILMING_PAGE_VIEWER_HEIGHT = "FilmingPageViewerHeight";

        public static readonly string FILMING_STUDY_INSTANCE_UID = "FilmingStudyInstanceUid";
        #endregion

        #region[--FilmingPage Graphic Operation--]

        public static readonly string FIlMING_PAGE_Graphic_OPERATION = "GraphicOperation";
        public static readonly string FIlMING_PAGE_Graphics_ROI_MENU = "GraphicsROIMenu";
        public static readonly string FIlMING_PAGE_Ellipse_ROI_MENU = "EllipseROI";
        public static readonly string FIlMING_PAGE_Circle_ROI_MENU = "CircleROI";
        public static readonly string FIlMING_PAGE_Spline_ROI_MENU = "SplineROI";
        public static readonly string FIlMING_PAGE_Freehand_ROI_MENU = "FreehandROI";
        public static readonly string FIlMING_PAGE_Poligon_ROI_MENU = "PoligonROI";
        public static readonly string FIlMING_PAGE_Rectangle_ROI_MENU = "RectangleROI";
		
        #endregion

        public static readonly string VALUE = "Value";



        #region  [---Static Method---]
        public static void AppendChildNode(XmlNode parentNode, string childNodeName, string value)
        {
            if (null == parentNode
                || null == parentNode.OwnerDocument
                || String.IsNullOrEmpty(childNodeName)
                || String.IsNullOrEmpty(value))
            {
                return;
            }

            var childNode = parentNode.OwnerDocument.CreateElement(childNodeName);
            parentNode.AppendChild(childNode);
            childNode.SetAttribute(VALUE, value);
        }

        public static string GetChildNodeValue(XmlNode parentNode, string childNodeName)
        {
            if (null == parentNode || String.IsNullOrEmpty(childNodeName))
            {
                return String.Empty;
            }

            var vauleNode = parentNode.SelectSingleNode(childNodeName + "/@" + VALUE);

            return vauleNode == null ? String.Empty : vauleNode.Value;
        }
        #endregion
    }
}
