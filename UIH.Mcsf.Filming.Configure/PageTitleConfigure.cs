using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using UIH.Mcsf.Core;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Configure
{
    public class PageTitleConfigure
    {
        public static readonly PageTitleConfigure Instance = new PageTitleConfigure();
        private string _configureFilePath = @"\config\filming\config\FilmingPage.xml";
        public FilmingPageInfo FilmingPageInfo;
        private PageTitleConfigure()
        {
            ParseFilmingPageConfig();
        }
        public double DisplayFont { get; private set; }
        public string DisplayPosition { get; private set; }
        public string PatientName { get; private set; }
        public string PatientID { get; private set; }
        public string Age { get; private set; }
        public string Sex { get; private set; }
        public string StudyDate { get; private set; }
        public string HospitalInfo { get; private set; }
        public string OperatorName { get; private set; }
        public string Comment { get; private set; }
        public string PageNo { get; private set; }
        public string AccessionNo { get; private set; }
        public string StudyInstanceUid { get; private set; }
        public string Logo { get; private set; }
        public void ParseFilmingPageConfig()
        {
            FilmingPageInfo = ConfigFileHelper.LoadConfigObject<FilmingPageInfo>(_configureFilePath);
            DisplayFont = FilmingPageInfo.DisplayFont;
            PatientName= FilmingPageInfo.PatientName;
            PatientID= FilmingPageInfo.PatientID;
            Age= FilmingPageInfo.Age;
            Sex= FilmingPageInfo.Sex;
            StudyDate= FilmingPageInfo.StudyDate;
            HospitalInfo= FilmingPageInfo.HospitalInfo;
            OperatorName= FilmingPageInfo.OperatorName;
            Comment= FilmingPageInfo.Comment;
            PageNo= FilmingPageInfo.PageNo;
            AccessionNo= FilmingPageInfo.AccessionNo;
            Logo= FilmingPageInfo.Logo;
            DisplayFont = FilmingPageInfo.DisplayFont;
            DisplayPosition= FilmingPageInfo.DisplayPosition;
        }

        #region [Jinyang.li Performance]

        public void SerializeToXml(XmlNode parentNode,bool isShowPageNo=true)
        {
            if (null == parentNode || null == parentNode.OwnerDocument)
            {
                return;
            }

            var filmingPageTitleInfoNode = parentNode.OwnerDocument.CreateElement(OffScreenRenderXmlHelper.FILMING_PAGE_TITLE_SETTING);
            parentNode.AppendChild(filmingPageTitleInfoNode);

            OffScreenRenderXmlHelper.AppendChildNode(filmingPageTitleInfoNode, OffScreenRenderXmlHelper.DISPLAY_FONT, this.DisplayFont.ToString(CultureInfo.InvariantCulture));
            OffScreenRenderXmlHelper.AppendChildNode(filmingPageTitleInfoNode, OffScreenRenderXmlHelper.DISPLAY_POSITION, this.DisplayPosition);
            OffScreenRenderXmlHelper.AppendChildNode(filmingPageTitleInfoNode, OffScreenRenderXmlHelper.PATIENT_NAME_FLAG, this.PatientName);
            OffScreenRenderXmlHelper.AppendChildNode(filmingPageTitleInfoNode, OffScreenRenderXmlHelper.PATIENT_ID_FLAG, this.PatientID);
            OffScreenRenderXmlHelper.AppendChildNode(filmingPageTitleInfoNode, OffScreenRenderXmlHelper.AGE_FLAG, this.Age);
            OffScreenRenderXmlHelper.AppendChildNode(filmingPageTitleInfoNode, OffScreenRenderXmlHelper.SEX_FLAG, this.Sex);
            OffScreenRenderXmlHelper.AppendChildNode(filmingPageTitleInfoNode, OffScreenRenderXmlHelper.STUDY_DATE_FLAG, this.StudyDate);
            OffScreenRenderXmlHelper.AppendChildNode(filmingPageTitleInfoNode, OffScreenRenderXmlHelper.HOSPITAL_INFO_FLAG, this.HospitalInfo != "1" && this.OperatorName != "1" ? "0" : "1");
            OffScreenRenderXmlHelper.AppendChildNode(filmingPageTitleInfoNode, OffScreenRenderXmlHelper.COMMENT, this.Comment);
            OffScreenRenderXmlHelper.AppendChildNode(filmingPageTitleInfoNode, OffScreenRenderXmlHelper.PAGE_NO,isShowPageNo?this.PageNo:"-1");
            OffScreenRenderXmlHelper.AppendChildNode(filmingPageTitleInfoNode, OffScreenRenderXmlHelper.ACCESSION_NO, this.AccessionNo);
            OffScreenRenderXmlHelper.AppendChildNode(filmingPageTitleInfoNode, OffScreenRenderXmlHelper.FILMING_STUDY_INSTANCE_UID, this.StudyInstanceUid);
            OffScreenRenderXmlHelper.AppendChildNode(filmingPageTitleInfoNode, OffScreenRenderXmlHelper.LOGO, this.Logo);
        }

       #endregion
    }

    [XmlRoot("Root")]
    public class FilmingPageInfo
    {
        public string PatientName;
        public string PatientID;
        public string Age;
        public string Sex;
        public string StudyDate;
        public string HospitalInfo;
        public string OperatorName;
        public string Comment;
        public string PageNo;
        public string AccessionNo;
        public string Logo;
        public double DisplayFont;
        public string DisplayPosition;
    }
}
