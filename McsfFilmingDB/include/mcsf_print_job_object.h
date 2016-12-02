//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_print_job_object.h
///  \brief   get a print job and contain needed parameters
///
///  \version 1.0
///  \date    Oct. 24, 2011
///  
//////////////////////////////////////////////////////////////////////////

#ifndef MCSF_PRINT_JOB_OBJECT_H_
#define MCSF_PRINT_JOB_OBJECT_H_

#include<string>
#include<vector>

#include "mcsf_filming_DB_config.h"

MCSF_FILMING_BEGIN_NAMESPACE

 class Mcsf_Filming_Export McsfPrintJobObject
{
public:
    McsfPrintJobObject(void);
    ~McsfPrintJobObject(void);

    void SetJobID(const unsigned int iJobID);
    const unsigned int GetJobID() const;

    const std::string& GetMyAETitle() const;
    void SetMyAETitle(const std::string& sMyAETitle);

    const std::string& GetTargetHostName() const;
    void SetTargetHostName(const std::string& sTargetName);

    //const std::string& GetTargetDescription() const;
    //void SetTargetDescription(const std::string& sTargetDescription);

    const std::string& GetTargetAETitle() const;
    void SetTargetAETitle(const std::string& sTargetAETitle);

    const unsigned short GetTargetPort() const;
    void SetTargetPort(const unsigned short iPort);

    const std::string& GetLayout() const;
    void SetLayout(const std::string& sLayout);

    const std::string& GetFilmSize() const;
    void SetFilmSize(const std::string& sFilmSize);

    const unsigned long GetTargetMaxPDU() const;
    void SetTargetMaxPDU(unsigned long lMaxPDU);

    const bool GetTargetImplicitOnly() const;
    void SetTargetImplicitOnly(const bool bTargetImplicitOnly);

    //const bool GetTargetDisableNewVRs() const;
    //void SetTargetDisableNewVRs(const bool bTargetDisableNewVRs);

    const bool GetTargetSupportsPLUT() const;
    void SetTargetSupportsPLUT(const bool bTargetSupportPLUT);

    const bool GetTargetSupportsAnnotation() const;
    void SetTargetSupportsAnnotation(const bool bTargetSupportAnnotation);

    const bool GetTargetSupports12bit() const;
    void SetTargetSupports12bit(const bool bTargetSupports12bit);

    const bool GetTargetPLUTinFilmSession() const;
    void SetTargetPLUTinFilmSession(const bool bTargetPLUTinFilmSession);

    const bool GetTargetRequiresMatchingLUT() const;
    void SetTargetRequiresMatchingLUT(const bool bTargetRequiresMatchingLUT);

    const bool GetTargetPreferSCPLUTRendering() const;
    void SetTargetPreferSCPLUTRendering(const bool bTargetPreferSCPLUTRendering);

    const bool GetSessionPrint() const;
    void SetSessionPrint(const bool bSessionPrint);

    const bool GetMonochrome1() const;
    void SetMonochrome1(const bool bMonochrome1);

    //brief Note: 0---portrait; 1---landscape
    const int GetOrientation() const;
    void SetOrientation(int iOrientation);

    //const std::string& GetPrinterID() const {return this->m_sPrinterID;}
    //void SetPrinterID(const std::string& sPrinterID);

	const std::string& GetMediumType() const;
	void SetMediumType(const std::string& sMediumType);

	const std::string& GetDestination() const;
	void SetDestination(const std::string& sDestination);

    //const std::string& GetSessionLable() const;
    //void SetSessionLable(const std::string& sSessionLable);

    const unsigned int& GetPriority() const;
    void SetPriority(const unsigned int& sPriority);
    
    //const std::string& GetOwnerID() const;
    //void SetOwnerID(const std::string& sOwnerID);

    //const std::string& GetSpoolPrefix() const;
    //void SetSpoolPrefix(const std::string& sSpoolPrefix);

    //const unsigned int GetSleepTime() const;
    //void SetSleepTime(const unsigned int iSleepTime);

    const unsigned int GetCopies() const;
    void SetCopies(const unsigned int iCopies);

	const bool GetIfColorPrint() const;
	void SetColorPrint(const bool bColorPrint);

    const std::string& GetSPfilePath() const;
    const std::vector<std::string>& GetHGfilePathList() const;

    bool  SetSPfilePath(const std::string& sSPfilePath);

    bool  SetHGfileList(const std::vector<std::string>& sHGfilePathList);

    const std::vector<std::string>& GetOriginSOPInstanceUIDList() const { return m_sOriginSOPInstanceUIDList; }
    void SetOriginSOPInstanceUIDList(const std::vector<std::string>& val) { m_sOriginSOPInstanceUIDList = val; }

    const std::vector<std::string>& GetFileNameList() const;
    bool  SetFileNameList(const std::vector<std::string>& sFileNameList);

    const std::string& GetStorageRootPath() const;
    bool SetStorageRootPath(const std::string& sStorageRootPath);

    const int GetJobStatus() const ;
    void SetJobStatus(int iJobStatus);

    const int GetJobPriority() const;
    void SetJobPriority(int iJobPriority);

    const int GetFilmAmount() const;
    void SetFilmAmount(int iFilmAmount);

    //patient information---------------down
    const std::string& GetRefPatientID() const;
    void SetRefPatientID(const std::string& sRefPatientID);

    const std::string& GetRefPatientName() const;
    void SetRefPatientName(const std::string& sRefPatientName);

    //M, F
    const std::string& GetRefPatientSex() const;
    void SetRefPatientSex(const std::string& sRefPatientSex);

    //60Y (years), 10M (months), so define with 'string' type
    const std::string& GetRefPatientAge() const;
    void SetRefPatientAge(const std::string& sRefPatientAge);

    const std::string& GetOperatorName() const;
    void SetOperatorName(const std::string& sOperatorName);

    const std::string& GetAccessionNo() const;
    void SetAccessionNo(const std::string& sAccessionNo);
    
    ///////////////////////////////////////20111102
    const std::string& GetTargetHostname() const;
    //const std::string& GetImageDisplayFormat() const;
    const unsigned int GetLayoutRowNumber() const;
    const unsigned int GetLayoutColNumber() const;
    //const std::string& GetPrinterDescription() const;
    const unsigned int GetPrinterPort() const;
    const std::string& GetPrinterIP() const;
    const unsigned long GetMaxPDU() const;
    //const bool GetImplictOnly() const;

    const std::string& GetLutFile() const { return m_sLutFile; }
    void SetLutFile(const std::string& val) { m_sLutFile = val; }

    void SetLutName(const std::string& sLutName);
    const std::string& GetLutName() const;
    const int GetLutShape() const {return this->m_iLUTshape;}
    void SetLutShape(const int val){this->m_iLUTshape = val;}
    void SetMatchingLUT(const bool val){this->m_bTargetRequiresMatchingLUT = val;}
    void SetPreferSCPLUTRendering(const bool val){this->m_bTargetPreferSCPLUTRendering = val;}
    const bool GetUseInverseLut() const {return this->m_bInverseLUT;}
    const bool GetUseAnnotation() const {return this->m_bAnnotation;}
    const bool GetUseannotationDatetime() const {return this->m_bAnnotationDateTime;}
    const bool GetUseAnnotationPrinter() const {return this->m_bAnnotationPrinter;}
    const bool GetUseAnnotationIllumination() const {return this->m_bAnnotationIllumination;}
    const std::string& GetAnnotationString() const;
    const std::string& GetImgPolarity() const;
    const std::string& GetImgRequestSize() const;
    const std::string& GetImgMagnification() const;
    const std::string& GetImgSmoothing() const;
    const std::string& GetImgConfiguration() const;

    const bool GetTargetPrinterSupportsAnnotation() const;
    const unsigned long GetMinPrintResolutionX() const;
    const unsigned long GetMinPrintResolutionY() const;
    const unsigned long GetMaxPrintResolutionX() const;
    const unsigned long GetMaxPrintResolutionY() const;
    const unsigned long GetMaxPreviewResolutionX() const;
    const unsigned long GetMaxPreviewResolutionY() const;
    const unsigned short GetDefaultPrintIllumination() const;
    void SetDefaultPrintIllumination(const unsigned short usIllumination);
    const unsigned short GetDefaultPrintReflection() const;
    void SetDefaultPrintReflection(const unsigned short usPrintReflection);
    //const std::string& GetNetworkAETitle() const;
    const std::string& GetLUTFilename(const std::string& lutID) const;
    const std::string& GetLUTFolder() const;
    const bool GetTargetPrinterSupportsAnnotationBoxSOPClass() const;
    const std::string& GetTargetPrinterAnnotationDisplayFormatID() const;
    const unsigned short GetTargetPrinterAnnotationPosition() const;
    const bool GetTargetPrinterSessionLabelAnnotation() const;

    bool           m_bNoPrint;

private:
    /// print parameters, taken from DB print job table
    unsigned int m_iJobID;
    int m_iFilmAmount;
    int m_iJobStatus;
    unsigned int m_iPriority;
    std::string m_sPrinterID;
    std::string m_sMediumType;
    std::string m_sDestination;
    std::string m_sSessionLabel;
    std::string m_sOwnerID;
    std::string m_sSpoolPrefix;
    unsigned int m_iSleep;
    unsigned int m_iCopies;
	bool			m_bColorPrint;
    bool           m_bSessionPrint;             /* Basic Film Session N-ACTION? */
    bool           m_bMonochrome1;             /* send images in MONOCHROME 1? */
    std::string m_sMyAETitle;
    std::string m_sTargetHostName;
    std::string m_sTargetDescription;
    std::string m_sTargetAETitle;
    unsigned short m_iTargetPort;
    unsigned long  m_lTargetMaxPDU;
    bool         m_bTargetImplicitOnly;
    bool         m_bTargetDisableNewVRs;
    bool         m_bTargetSupportsPLUT;
    bool         m_bTargetSupportsAnnotation;
    bool         m_bTargetSupports12bit;
    bool         m_bTargetPLUTinFilmSession;
    bool         m_bTargetRequiresMatchingLUT;
    bool         m_bTargetPreferSCPLUTRendering;
    int          m_iOrientation;
    /////////////////20111102/////////////////////////

    unsigned short m_iIllumination;
    unsigned short m_iReflection;
    std::string m_sMaxPreviewResolution;
    std::string m_sMaxPrintResolution;
    std::string m_sMinPrintResolution;
    std::string m_sLutFolder;
    std::string m_sLutName;
    std::string m_sLutFile;
    bool m_bInverseLUT;
    int m_iLUTshape;
    std::string m_sAnnotationString;
    bool m_bAnnotationIllumination;
    bool m_bAnnotationPrinter;
    bool m_bAnnotationDateTime;
    bool m_bAnnotation;
    std::string m_sLayout;
    std::string m_sFilmSize;

    //SP and HG file path
    std::vector<std::string> m_sOriginSOPInstanceUIDList;
    std::vector<std::string>           m_sFileNameList;
    std::string m_sSPfilePath;
    std::vector<std::string> m_sHGfilePathList;
    std::string m_sStorageRootPath;

    // patient information
    std::string m_sRefPatientID;
    std::string m_sRefPatientName;
    std::string m_sRefPatientSex;
    std::string m_sRefPatientAge;
    std::string m_sOperatorName;
    std::string m_sAccessionNo;
};

MCSF_FILMING_END_NAMESPACE
#endif  //MCSF_PRINT_JOB_OBJECT_H_
