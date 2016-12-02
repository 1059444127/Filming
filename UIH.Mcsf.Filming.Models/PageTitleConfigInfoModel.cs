using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace UIH.Mcsf.Filming.Models
{
   public class PageTitleConfigInfoModel
   {

       #region [---Constructors---]
       #endregion



       #region [---Properties---]

       public string DisplayFont { get; private set; }
       public string DisplayPosition { get; set; } 
       public string PatientNameFlag { get; private set; }
       public string PatientIdFlag { get; private set; }
       public string AgeFlag { get; private set; }
       public string SexFlag { get; private set; }
       public string StudyDateFlag { get; private set; }
       public string HospitalInfoFlag { get; private set; }
       public string Comment { get; private set; }
       public string PageNo { get; private set; }
       public string AccessionNoFlag { get; private set; }
       public string StudyInstanceUid { get; private set; }
       public string Logo { get; private set; }
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
               this.PatientNameFlag = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.PATIENT_NAME_FLAG);
               this.PatientIdFlag = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.PATIENT_ID_FLAG);
               this.AgeFlag = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.AGE_FLAG);
               this.SexFlag = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.SEX_FLAG);
               this.StudyDateFlag = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.STUDY_DATE_FLAG);
               this.HospitalInfoFlag = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.HOSPITAL_INFO_FLAG);
               this.Comment = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.COMMENT);
               this.PageNo = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.PAGE_NO);
               this.Logo = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.LOGO);
               this.AccessionNoFlag = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.ACCESSION_NO);
               this.StudyInstanceUid = OffScreenRenderXmlHelper.GetChildNodeValue(currNode, OffScreenRenderXmlHelper.FILMING_STUDY_INSTANCE_UID);
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
