//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_print_job_object.cpp
///  \brief   filming print job object
///
///  \version 1.0
///  \date    Oct. 26, 2011
///  
//////////////////////////////////////////////////////////////////////////

#include "mcsf_print_job_object.h"

MCSF_FILMING_BEGIN_NAMESPACE
McsfPrintJobObject::McsfPrintJobObject(void):
    m_bNoPrint(false),
    m_bSessionPrint(false),
    m_bMonochrome1(false),
    m_sPrinterID(EMPTY_STRING),
    m_sMediumType(EMPTY_STRING),
    m_sDestination(EMPTY_STRING),
    m_sSessionLabel(EMPTY_STRING),
    m_iPriority(1),
    m_sOwnerID(EMPTY_STRING),
    m_sSpoolPrefix(EMPTY_STRING),
    m_iSleep(1),
    m_iCopies(1),
	m_bColorPrint(false),
    m_sMyAETitle(EMPTY_STRING),
    m_sTargetHostName(EMPTY_STRING),
    m_sTargetDescription(EMPTY_STRING),
    m_sTargetAETitle(EMPTY_STRING),
    m_iTargetPort(104),
    m_lTargetMaxPDU(32768),
    m_bTargetImplicitOnly(false),
    m_bTargetDisableNewVRs(false),
    m_bTargetSupportsPLUT(true),
    m_bTargetSupportsAnnotation(true),
    m_bTargetSupports12bit(true),
    m_bTargetPLUTinFilmSession(false),
    m_bTargetRequiresMatchingLUT(true),
    m_bTargetPreferSCPLUTRendering(false),/////////////20111102------add----------------
    m_iJobID(0),
    m_iJobStatus(0),
    m_sLayout("STANDARD\\1,1"),
    m_iIllumination(2000),
    m_iReflection(10),
    m_iFilmAmount(0),
    m_sRefPatientID(EMPTY_STRING),
    m_sRefPatientName(EMPTY_STRING),
    m_sRefPatientSex(EMPTY_STRING),
    m_sRefPatientAge(EMPTY_STRING),
    m_sOperatorName(EMPTY_STRING),
    m_sAccessionNo(EMPTY_STRING)
    /*m_sMaxPreviewResolution(),
    m_sMaxPrintResolution(),
    m_sMinPrintResolution(),
    m_sLutFolder(),
    m_sLutName(),
    m_bInverseLUT(),
    m_iLUTshape(),
    m_sAnnotationString(),
    m_bAnnotationIllumination(),
    m_bAnnotationPrinter(),
    m_bAnnotationDateTime(),
    m_bAnnotation(),              
    m_sImgPolarity(),              
    m_sImgRequestSize(),              
    m_sImgMagnification(),
    m_sImgSmoothing(),              
    m_sImgConfiguration()*/
{
    //these parameters should select from DB, now use hard code for test.
    this->m_sSPfilePath = "";
    m_sMyAETitle                   = "UIPrint";
    m_sTargetHostName              = "10.1.3.194";
    m_sTargetDescription           = "UI-Filming";
    m_sTargetAETitle               = "DICOMSCOPE";
    m_iTargetPort                  = 10006;
    m_lTargetMaxPDU                = 32768;
    m_bTargetImplicitOnly          = false;
    m_bTargetDisableNewVRs         = false;
    m_bTargetSupportsPLUT          = true;
    m_bTargetSupportsAnnotation    = false;
    m_bTargetSupports12bit         = true;
    m_bTargetPLUTinFilmSession     = false;
    m_bTargetRequiresMatchingLUT   = false;
    m_bTargetPreferSCPLUTRendering = false;
}


McsfPrintJobObject::~McsfPrintJobObject(void)
{
}

const std::vector<std::string>& McsfPrintJobObject::GetFileNameList() const
{
    return this->m_sFileNameList;
}

bool McsfPrintJobObject::SetFileNameList(const std::vector<std::string>& sFileNameList)
{
    if(sFileNameList.size()>0)
    {
        this->m_sFileNameList = sFileNameList;
        return true;
    }
        LOG_ERROR_DB("dicom file list is null!");
        return false;
}

const std::string& McsfPrintJobObject::GetSPfilePath() const
{
    return this->m_sSPfilePath;
}

const std::vector<std::string>& McsfPrintJobObject::GetHGfilePathList() const
{
    return this->m_sHGfilePathList;
}

void McsfPrintJobObject::SetJobID(const unsigned int iJobID)
{
    this->m_iJobID = iJobID;
}

const unsigned int McsfPrintJobObject::GetJobID() const
{
    return this->m_iJobID;
}

bool  McsfPrintJobObject::SetSPfilePath(const std::string& sSPfilePath)
{
    if(sSPfilePath.length()>0)
    {
        this->m_sSPfilePath = sSPfilePath;
        return true;
    }
    return false;
}

bool  McsfPrintJobObject::SetHGfileList(const std::vector<std::string>& sHGfilePathList)
{
    if(sHGfilePathList.size()>0)
    {
        this->m_sHGfilePathList = sHGfilePathList;
        return true;
    }
    return false;
}

const std::string& McsfPrintJobObject::GetLayout() const
{
    return this->m_sLayout;
}

void McsfPrintJobObject::SetLayout(const std::string& sLayout)
{
    m_sLayout = sLayout;
}

const std::string& McsfPrintJobObject::GetFilmSize() const
{
    return this->m_sFilmSize;
}

void McsfPrintJobObject::SetFilmSize(const std::string& sFilmSize)
{
    m_sFilmSize = sFilmSize;
}

const unsigned int McsfPrintJobObject::GetCopies() const
{
    return this->m_iCopies;
}

void McsfPrintJobObject::SetCopies(const unsigned int iCopies)
{
    this->m_iCopies = iCopies;
}

const bool McsfPrintJobObject::GetIfColorPrint() const
{
	return this->m_bColorPrint;
}

void McsfPrintJobObject::SetColorPrint(const bool bColorPrint)
{
	this->m_bColorPrint = bColorPrint;
}

const int McsfPrintJobObject::GetJobStatus() const 
{
    return this->m_iJobStatus;
}

void McsfPrintJobObject::SetJobStatus(int iJobStatus)
{
    this->m_iJobStatus = iJobStatus;
}

const int McsfPrintJobObject::GetJobPriority() const
{
    return this->m_iPriority;
}

const unsigned int McsfPrintJobObject::GetPrinterPort() const
{
    return this->m_iTargetPort;
}

const unsigned long McsfPrintJobObject::GetMaxPDU() const
{
    return this->m_lTargetMaxPDU;
}

const std::string& McsfPrintJobObject::GetPrinterIP() const
{
    return this->m_sTargetHostName;
}

void McsfPrintJobObject::SetJobPriority(int iJobPriority)
{
    this->m_iPriority = iJobPriority;
}

const bool McsfPrintJobObject::GetSessionPrint() const
{
    return this->m_bSessionPrint;
}

void McsfPrintJobObject::SetSessionPrint(const bool bSessionPrint)
{
    this->m_bSessionPrint = bSessionPrint;
}

const bool McsfPrintJobObject::GetMonochrome1() const
{
    return this->m_bMonochrome1;
}

void McsfPrintJobObject::SetMonochrome1(const bool bMonochrome1)
{
    this->m_bMonochrome1 = bMonochrome1;
}

const bool McsfPrintJobObject::GetTargetPLUTinFilmSession() const
{
    return this->m_bTargetPLUTinFilmSession;
}

void McsfPrintJobObject::SetTargetPLUTinFilmSession(const bool bTargetPLUTinFilmSession)
{
    this->m_bTargetPLUTinFilmSession = bTargetPLUTinFilmSession;
}

const bool McsfPrintJobObject::GetTargetRequiresMatchingLUT() const
{
    return this->m_bTargetRequiresMatchingLUT;
}

void McsfPrintJobObject::SetTargetRequiresMatchingLUT(const bool bTargetRequiresMatchingLUT)
{
    this->m_bTargetRequiresMatchingLUT = bTargetRequiresMatchingLUT;
}

const bool McsfPrintJobObject::GetTargetPreferSCPLUTRendering() const
{
    return this->m_bTargetPreferSCPLUTRendering;
}

void McsfPrintJobObject::SetTargetPreferSCPLUTRendering(const bool bTargetPreferSCPLUTRendering)
{
    this->m_bTargetPreferSCPLUTRendering = bTargetPreferSCPLUTRendering;
}

const bool McsfPrintJobObject::GetTargetSupports12bit() const
{
    return this->m_bTargetSupports12bit;
}

void McsfPrintJobObject::SetTargetSupports12bit(const bool bTargetSupports12bit)
{
    this->m_bTargetSupports12bit = bTargetSupports12bit;
}

const std::string& McsfPrintJobObject::GetMyAETitle() const
{
    return this->m_sMyAETitle;
}

void McsfPrintJobObject::SetMyAETitle(const std::string& sMyAETitle)
{
    this->m_sMyAETitle = sMyAETitle;
}

const std::string& McsfPrintJobObject::GetTargetAETitle() const
{
    return this->m_sTargetAETitle;
}

void McsfPrintJobObject::SetTargetAETitle(const std::string& sTargetAETitle)
{
    this->m_sTargetAETitle = sTargetAETitle;
}

const std::string& McsfPrintJobObject::GetTargetHostName() const
{
    return this->m_sTargetHostName;
}

void McsfPrintJobObject::SetTargetHostName(const std::string& sTargetName)
{
    this->m_sTargetHostName  = sTargetName;
}

const unsigned short McsfPrintJobObject::GetTargetPort() const
{
    return this->m_iTargetPort;
}

void McsfPrintJobObject::SetTargetPort(const unsigned short iPort)
{
    this->m_iTargetPort = iPort;
}

const unsigned long McsfPrintJobObject::GetTargetMaxPDU() const
{
    return this->m_lTargetMaxPDU;
}

void McsfPrintJobObject::SetTargetMaxPDU(unsigned long lMaxPDU)
{
    this->m_lTargetMaxPDU = lMaxPDU;
}

const bool McsfPrintJobObject::GetTargetSupportsPLUT() const
{
    return this->m_bTargetSupportsPLUT;
}

void McsfPrintJobObject::SetTargetSupportsPLUT(const bool bTargetSupportPLUT)
{
    this->m_bTargetSupportsPLUT = bTargetSupportPLUT;
}

const bool McsfPrintJobObject::GetTargetSupportsAnnotation() const
{
    return this->m_bTargetSupportsAnnotation;
}

void McsfPrintJobObject::SetTargetSupportsAnnotation(const bool bTargetSupportAnnotation)
{
    this->m_bTargetSupportsAnnotation = bTargetSupportAnnotation;
}

const bool McsfPrintJobObject::GetTargetImplicitOnly() const
{
    return this->m_bTargetImplicitOnly;
}

void McsfPrintJobObject::SetTargetImplicitOnly(const bool bTargetImplicitOnly)
{
    this->m_bTargetImplicitOnly = bTargetImplicitOnly;
}

const unsigned int& McsfPrintJobObject::GetPriority() const
{
    return this->m_iPriority;
}

void McsfPrintJobObject::SetPriority(const unsigned int& iPriority)
{
    this->m_iPriority = iPriority;
}

void McsfPrintJobObject::SetDefaultPrintIllumination( const unsigned short usIllumination )
{
    this->m_iIllumination = usIllumination;
}

const unsigned short McsfPrintJobObject::GetDefaultPrintIllumination()  const
{
    if(this->m_iIllumination>0)
        return this->m_iIllumination;
    //warning
    return 2000;//default...
}

void McsfPrintJobObject::SetDefaultPrintReflection(const unsigned short usPrintReflection)
{
    this->m_iReflection = usPrintReflection;
}

const unsigned short McsfPrintJobObject::GetDefaultPrintReflection()  const
{
    if(this->m_iReflection>0)
        return this->m_iReflection;
    return 10;
}

const unsigned long McsfPrintJobObject::GetMaxPreviewResolutionY()  const
{
    if (this->m_sMaxPreviewResolution.length()>0)
    {
        unsigned long result = 0;
        unsigned long dummy = 0;
        if (2 == sscanf_s(m_sMaxPreviewResolution.c_str(), "%lu\\%lu", &dummy, &result))
        {
            return static_cast<unsigned int>(result);
        }
        else
        {
            return 256;
        }
    }
    else
    {
        //warning....
        return 256;//default
    }
}

const unsigned long McsfPrintJobObject::GetMaxPreviewResolutionX()  const
{
    if (this->m_sMaxPreviewResolution.length()>0)
    {
        unsigned long result = 0;
        unsigned long dummy = 0;
        if (2 == sscanf_s(m_sMaxPreviewResolution.c_str(), "%lu\\%lu", &result, &dummy))
        {
            return (unsigned int)result;
        }
        else
        {
            return 256;
        }
    }
    else
    {
        //warning....
        return 256;//default
    }
}

const unsigned long McsfPrintJobObject::GetMaxPrintResolutionY()  const
{
    if (this->m_sMaxPrintResolution.length()>0)
    {
        unsigned long result = 0;
        unsigned long dummy = 0;
        if (2 == sscanf_s(m_sMaxPrintResolution.c_str(), "%lu\\%lu", &dummy, &result))
        {
            return (unsigned int)result;
        }
        else
        {
            return 8192;
        }
    }
    else
    {
        //warning....
        return 8192;//default
    }
}

const unsigned long McsfPrintJobObject::GetMaxPrintResolutionX()  const
{
    if (this->m_sMaxPrintResolution.length()>0)
    {
        unsigned long result = 0;
        unsigned long dummy = 0;
        if (2 == sscanf_s(m_sMaxPrintResolution.c_str(), "%lu\\%lu", &result, &dummy))
        {
            return (unsigned int)result;
        }
        else
        {
            return 8192;
        }
    }
    else
    {
        //warning....
        return 8192;//default
    }
}

const unsigned long McsfPrintJobObject::GetMinPrintResolutionX()  const
{
    if (this->m_sMinPrintResolution.length()>0)
    {
        unsigned long result = 0;
        unsigned long dummy = 0;
        if (2 == sscanf_s(m_sMinPrintResolution.c_str(), "%lu\\%lu", &result, &dummy))
        {
            return (unsigned int)result;
        }
        else
        {
            return 1024;
        }
    }
    else
    {
        //warning....
        return 1024;//default
    }
}

const unsigned long McsfPrintJobObject::GetMinPrintResolutionY()  const
{
    if (this->m_sMinPrintResolution.length()>0)
    {
        unsigned long result = 0;
        unsigned long dummy = 0;
        if (2 == sscanf_s(m_sMinPrintResolution.c_str(), "%lu\\%lu", &dummy, &result))
        {
            return (unsigned int)result;
        }
        else
        {
            return 1024;
        }
    }
    else
    {
        //warning....
        return 1024;//default
    }
}

const std::string& McsfPrintJobObject::GetLUTFolder() const
{
    //if(this->m_sLutFolder.length()>0)
    //{
        return this->m_sLutFolder;
    //}
    //else
    //{
    //    //error
    //    return EMPTY_STRING;
    //}
}

const std::string& McsfPrintJobObject::GetLUTFilename(const std::string& lutID) const
{
    if(lutID.empty() || m_sLutName.empty())
    {
        return EMPTY_STRING;
    }

    return this->m_sLutName;

}

const bool McsfPrintJobObject::GetTargetPrinterSessionLabelAnnotation()  const
{
    return true;
}

const unsigned short McsfPrintJobObject::GetTargetPrinterAnnotationPosition()  const
{
    return 1;
}

const std::string& McsfPrintJobObject::GetTargetPrinterAnnotationDisplayFormatID() const
{
    return EMPTY_STRING;
}

const bool McsfPrintJobObject::GetTargetPrinterSupportsAnnotationBoxSOPClass()  const
{
    return true;
}

const bool McsfPrintJobObject::GetTargetPrinterSupportsAnnotation() const
{
    return false;
}

const std::string& McsfPrintJobObject::GetTargetHostname() const
{
    //if(this->m_sTargetHostName.length()>0)
    //{
        return this->m_sTargetHostName;
    //}
    //return EMPTY_STRING;
}

const std::string& McsfPrintJobObject::GetLutName() const
{
    return this->m_sLutName;
}

void McsfPrintJobObject::SetLutName( const std::string& sLutName )
{
    m_sLutName = sLutName;
}

const std::string& McsfPrintJobObject::GetAnnotationString() const
{
    return this->m_sAnnotationString;
}

const unsigned int McsfPrintJobObject::GetLayoutColNumber() const
{
    unsigned int columns = 0;
    unsigned int rows = 0;

    //STANDARD\2,2
    if (this->m_sLayout.substr(0,9) == "STANDARD\\")
    {
        const char *format = m_sLayout.c_str() + 9;

        if (2==sscanf_s(format, "%d,%d", &columns, &rows))
        {
            if ((columns > 0)&&(rows > 0))
            {
                return columns;
            }
            else
            {
                //error
                return 1;//default
            }
        }
        else
        {
            //error
            return 1;//default
        }
    }
    else if(this->m_sLayout.substr(0,4) == "ROW\\")
    {
        //Error
        return 0;
    }
    else if(this->m_sLayout.substr(0,4) == "COL\\")
    {
        //Error
        return 0;
    }
    else
    {
        //Error
        return 0;
    }
}

const unsigned int McsfPrintJobObject::GetLayoutRowNumber() const
{
    unsigned int columns = 0;
    unsigned int rows = 0;

    //STANDARD\2,2
    if (this->m_sLayout.substr(0,9) == "STANDARD\\")
    {
        const char *format = m_sLayout.c_str() + 9;

        if (2==sscanf_s(format, "%d,%d", &columns, &rows))
        {
            if ((columns > 0)&&(rows > 0))
            {
                return rows;
            }
            else
            {
                //error
                return 1;//default
            }
        }
        else
        {
            //error
            return 1;//default
        }
    }
    else
    {
        //Error
        return 0;
    }
}

const std::string& McsfPrintJobObject::GetStorageRootPath() const
{
    return this->m_sStorageRootPath;
}

bool McsfPrintJobObject::SetStorageRootPath(const std::string& sStorageRootPath)
{
    if(!sStorageRootPath.empty())
    {
        this->m_sStorageRootPath = sStorageRootPath;
        return true;
    }
    return false;
}

const int McsfPrintJobObject::GetFilmAmount() const 
{
    return this->m_iFilmAmount;
}

void McsfPrintJobObject::SetFilmAmount(int iFilmAmount)
{
    m_iFilmAmount = iFilmAmount;
}

const std::string& McsfPrintJobObject::GetRefPatientID() const
{
    return m_sRefPatientID;
}

void McsfPrintJobObject::SetRefPatientID(const std::string& sRefPatientID)
{
    m_sRefPatientID = sRefPatientID;
}

const std::string& McsfPrintJobObject::GetRefPatientName() const
{
    return m_sRefPatientName;
}

void McsfPrintJobObject::SetRefPatientName(const std::string& sRefPatientName)
{
    m_sRefPatientName = sRefPatientName;
}

//M, F
const std::string& McsfPrintJobObject::GetRefPatientSex() const
{
    return m_sRefPatientSex;
}

void McsfPrintJobObject::SetRefPatientSex(const std::string& sRefPatientSex)
{
    m_sRefPatientSex = sRefPatientSex;
}

//60Y (years), 10M (months), so define with 'string' type
const std::string& McsfPrintJobObject::GetRefPatientAge() const
{
    return m_sRefPatientAge;
}

void McsfPrintJobObject::SetRefPatientAge(const std::string& sRefPatientAge)
{
    m_sRefPatientAge = sRefPatientAge;
}

const std::string& McsfPrintJobObject::GetOperatorName() const
{
    return m_sOperatorName;
}

void McsfPrintJobObject::SetOperatorName(const std::string& sOperatorName)
{
    m_sOperatorName = sOperatorName;
}

const std::string& McsfPrintJobObject::GetAccessionNo() const
{
    return m_sAccessionNo;
}

void McsfPrintJobObject::SetAccessionNo(const std::string& sAccessionNo)
{
    m_sAccessionNo = sAccessionNo;
}

void McsfPrintJobObject::SetOrientation(int iOrientation)
{
    m_iOrientation = iOrientation;
}

const int McsfPrintJobObject::GetOrientation() const
{
    return m_iOrientation;
}

const std::string& McsfPrintJobObject::GetMediumType() const
{
	return m_sMediumType;
}
void McsfPrintJobObject::SetMediumType(const std::string& sMediumType)
{
	m_sMediumType = sMediumType;
}

const std::string& McsfPrintJobObject::GetDestination() const
{
	return m_sDestination;
}
void McsfPrintJobObject::SetDestination(const std::string& sDestination)
{
	m_sDestination = sDestination;
}

MCSF_FILMING_END_NAMESPACE

