using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace UIH.Mcsf.Filming.Models
{
    public class PageTitleInfoModel
    {

        #region [---Properties----]

        public string DisplayFont { get; set; }
        public string DisplayPosition { get; set; }
        public string PatientName { get; private set; }
        public string PatientId { get; private set; }
        public string Age { get; private set; }
        public string Sex { get; private set; }
        public string StudyDate { get; private set; }
        public string HospitalInfo { get; private set; }
        public string Comment { get; private set; }
        public string PageNo { get; private set; }
        public string AccessionNo { get; private set; }
        public string StudyInstanceUid { get; private set; }
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

                this.DisplayFont = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.DISPLAY_FONT);
                this.DisplayPosition = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.DISPLAY_POSITION);
                this.PatientName = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.PATIENT_NAME_FLAG);
                this.PatientId = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.PATIENT_ID_FLAG);
                this.Age = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.AGE_FLAG);
                this.Sex = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.SEX_FLAG);
                this.StudyDate = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.STUDY_DATE_FLAG);
                this.HospitalInfo = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.HOSPITAL_INFO_FLAG);
                this.Comment = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.COMMENT);
                this.PageNo = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.PAGE_NO);
                this.AccessionNo = OffScreenRenderXmlHelper.GetChildNodeValue(currNode,
                                                                              OffScreenRenderXmlHelper.ACCESSION_NO);
                this.StudyInstanceUid = OffScreenRenderXmlHelper.GetChildNodeValue(currNode,
                                                                             OffScreenRenderXmlHelper.FILMING_STUDY_INSTANCE_UID);
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
