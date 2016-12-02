//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_dcm_print_config.h
///  \brief   the printer parameters init class
///
///  \version 1.0
///  \date    Oct. 20, 2011
///  
//////////////////////////////////////////////////////////////////////////

#ifndef MCSF_DCM_FILM_SESSEION_OBJECT_H_
#define MCSF_DCM_FILM_SESSEION_OBJECT_H_

#include <string>
#include <list>
using namespace std;

#include "mcsf_filming_config.h"

MCSF_FILMING_BEGIN_NAMESPACE

class Mcsf_Filming_Export McsfDcmFilmSessionObject
{
public:
    McsfDcmFilmSessionObject(std::string sPrinterAE,
        std::string sPrinterIP,
        unsigned int iPrinterPort,
        std::string sLayout);
    
    McsfDcmFilmSessionObject(void);
    ~McsfDcmFilmSessionObject(void);

//getter
    const std::string& GetPrinterID() const;
    const std::string& GetTargetHostname() const;
    const std::string& GetTargetAETitle() const;
    const std::string& GetImageDisplayFormat() const;
    unsigned int GetLayoutRowNumber() const;
    unsigned int GetLayoutColNumber() const;
    const std::string& GetPrinterDescription() const;
    const unsigned int GetPrinterPort() const;
    const std::string& GetPrinterIP() const;
    const unsigned int GetCopies() const;
    const unsigned long GetMaxPDU() const;
    const bool GetImplictOnly() const;
    const std::string& GetBorderDensity() const;
    const std::string& GetEmptyImageDensity() const;
    const std::string& GetFilmDestination() const;
    const std::string& GetFilmSize() const;
    const std::string& GetMagnification() const;
    const std::string& GetMediumType() const;
    bool GetDisableNewVRs() const;
	bool GetPresentationLUTinFilmSession() const;
    int GetMaxDensity() const;
    int GetMinDensity() const;

//setter
    void SetPrinterID                                 (const std::string&  sPrinterID                                 ) ;
    void SetTargetHostname                            (const std::string&  sTargetHostname                            ) ;
    void SetTargetAETitle                             (const std::string&  sTargetAETitle                             ) ;
    void SetImageDisplayFormat                        (const std::string&  sImageDisplayFormat                        ) ;
    /*void SetLayoutRowNumber                           (unsigned int        uiLayoutRowNumber                           ) ;
    void SetLayoutColNumber                           (unsigned int        uiLayoutColNumber                           ) ;*/
    void SetPrinterDescription                        (const std::string&  sPrinterDescription                        ) ;
    void SetPrinterPort                               (const unsigned int  uiPrinterPort                               ) ;
    void SetPrinterIP                                 (const std::string&  sPrinterIP                                 ) ;
    //void SetCopies                                    (const unsigned int  uiCopies                                    ) ;
    void SetMaxPDU                                    (const unsigned long ulMaxPDU                                    ) ;
    void SetImplictOnly                               (const bool          bImplictOnly                               ) ;

    void SetLutName                                   (const std::string&  sLutName                                   ) ;
    void SetLutShape                                  (const int           iLutShape                                  ) ;
    void SetUseInverseLut                             (const bool          bUseInverseLut                             ) ;
    void SetUseAnnotation                             (const bool          bUseAnnotation                             ) ;
    void SetUseannotationDatetime                     (const bool          bUseannotationDatetime                     ) ;
    void SetUseAnnotationPrinter                      (const bool          bUseAnnotationPrinter                      ) ;
    void SetUseAnnotationIllumination                 (const bool          bUseAnnotationIllumination                 ) ;
    void SetAnnotationString                          (const std::string&  sAnnotationString                          ) ;
    void SetImgPolarity                               (const std::string&  sImgPolarity                               ) ;
    void SetImgRequestSize                            (const std::string&  sImgRequestSize                            ) ;
    void SetImgMagnification                          (const std::string&  sImgMagnification                          ) ;
    void SetImgSmoothing                              (const std::string&  sImgSmoothing                              ) ;
    void SetImgConfiguration                          (const std::string&  sImgConfiguration                          ) ;
private:

	friend class PrinterConfig;

    /// filming communication parameters
    std::string m_sPrinterID;
    std::string m_sPrinterName;
    std::string m_sPrinterAE;
    std::string m_sPrinterIP;
    unsigned int m_iPrinterPort;
    std::string m_sPrinterDescription;
    std::string m_sOurAE;
    unsigned long m_lMaxPDU;
    bool m_bImplicitOnly;
    bool m_bDisableVRs;

    /// filming session parameters
    std::string m_sLayout;
    std::string m_sFilmSize;
    std::string m_sBorderDensity;
    std::string m_sEmptyImageDensity;
    std::string m_sMagnification;
    std::string m_sSmoothingType;
    std::string m_sMaxPrintResolution;
    std::string m_sMaxPreviewResolution;
    int m_iMaxDensity;
    int m_iMinDensity;
    std::string m_sFilmDestination;
    std::string m_sMediumType;
	bool m_bPresentationLUTinFilmSession;

};

MCSF_FILMING_END_NAMESPACE
#endif  //_MCSF_DCM_FILM_SESSEION_OBJECT_H_
