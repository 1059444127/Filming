//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_filming_auto_job_creater.h
///  \brief   define the filming job instance initialize function
///
///  \version 1.0
///  \date    Dec. 26, 2011
///  
//////////////////////////////////////////////////////////////////////////
#ifndef MCSF_FILMING_AUTO_JOB_CREATER_H_
#define MCSF_FILMING_AUTO_JOB_CREATER_H_

#include "McsfContainee/mcsf_containee_interface.h"
#include "McsfNetBase/mcsf_netbase_interface.h"

#include "mcsf_filming_common.h"

MCSF_FILMING_BEGIN_NAMESPACE

    /// \class  McsfFilmingAutoJobCreater mcsf_filming_auto_job_creater.h
    /// \brief  this class is used for init a auto filming job instance.
    /// you just need to do 4 steps thing to use this lib.
    /// step1: define class instance
    /// step2: use Set* method, init instance
    /// step3: create filmingJobInstance
    /// step4: define CommandContext,init it, and then send to FilmingBE
class Mcsf_Filming_Export McsfFilmingAutoJobCreater
{
public:
    /////////////////////////////////////////////////////////////////
    ///  \brief constructor
    ///
    ///  \param[in]    None
    ///  \param[out]   None
    ///  \return       None
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    McsfFilmingAutoJobCreater();

    /////////////////////////////////////////////////////////////////
    ///  \brief desconstructor
    ///
    ///  \param[in]    None
    ///  \param[out]   None
    ///  \return       None
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    ~McsfFilmingAutoJobCreater();

    /////////////////////////////////////////////////////////////////
    ///  \brief get the print job's priority.
    ///
    ///  \param[in]    None
    ///  \param[out]   None
    ///  \return       FilmingPrintJob_PrintPriority
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    PRINT_PRIORITY_ENUM GetPrintPriority() const {return m_ePrintPriority;}

    /////////////////////////////////////////////////////////////////
    ///  \brief set the print job's priority(HIGH, MEDIUM, LOW)
    ///
    ///  \param[in]    FilmingPrintJob_PrintPriority
    ///  \param[out]   None
    ///  \return       void
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void SetPrintPriority(PRINT_PRIORITY_ENUM ePrintPriority) {m_ePrintPriority = ePrintPriority;}

    /////////////////////////////////////////////////////////////////
    ///  \brief set the print job's linked patient ID.
    ///
    ///  \param[in]    const std::string& sPatientID
    ///  \param[out]   None
    ///  \return       void
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void SetPatientID(const std::string& sPatientID) { m_sPatientID = sPatientID; }

    /////////////////////////////////////////////////////////////////
    ///  \brief set the print job's linked patient Name.
    ///
    ///  \param[in]    const std::string& sPatientName
    ///  \param[out]   None
    ///  \return       void
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void SetPatientName(const std::string& sPatientName) { m_sPatientName = sPatientName; }

    /////////////////////////////////////////////////////////////////
    ///  \brief set the print job's linked patient Sex.
    ///
    ///  \param[in]    const std::string& sPatientSex
    ///  \param[out]   None
    ///  \return       void
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void SetPatientSex(const std::string& sPatientSex) { m_sPatientSex = sPatientSex; }

    /////////////////////////////////////////////////////////////////
    ///  \brief set the print job's linked patient Age.
    ///
    ///  \param[in]    const std::string& sPatientAge
    ///  \param[out]   None
    ///  \return       void
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void SetPatientAge(const std::string& sPatientAge) { m_sPatientAge = sPatientAge; }

    /////////////////////////////////////////////////////////////////
    ///  \brief set the print job's linked operatorName
    ///
    ///  \param[in]    const std::string& sOperatorName
    ///  \param[out]   None
    ///  \return       void
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void SetOperatorName(const std::string& sOperatorName) { m_sOperatorName = sOperatorName; }

    /////////////////////////////////////////////////////////////////
    ///  \brief set the print job's linked AccessionNo
    ///
    ///  \param[in]    const std::string& sAccessionNo
    ///  \param[out]   None
    ///  \return       void
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void SetAccessionNo(const std::string& sAccessionNo) { m_sAccessionNo = sAccessionNo; }

    /////////////////////////////////////////////////////////////////
    ///  \brief get the print job's copies. it means that the same film sheet count will
    ///  be printed as this function set.
    ///
    ///  \param[in]    None
    ///  \param[out]   None
    ///  \return       int
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int GetCopies() const { return m_iCopies; }

    /////////////////////////////////////////////////////////////////
    ///  \brief set the print job's copies. it means that the same film sheet count will
    ///  be printed as this function set.
    ///
    ///  \param[in]    int iCopies
    ///  \param[out]   None
    ///  \return       void
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void SetCopies(int iCopies) { m_iCopies = iCopies; }

    /////////////////////////////////////////////////////////////////
    ///  \brief set the film sheet display format.
    ///
    ///  \param[in]    LAYOUT_TYPE_ENUM eLayoutType  : STANDARD,ROW,COL
    ///                int iFirtNo  : Column count
    ///                int iSecondNo  : Row count
    ///  \param[out]   None
    ///  \return       void
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void SetLayout(LAYOUT_TYPE_ENUM eLayoutType, int iFirtNo, int iSecondNo);

    /////////////////////////////////////////////////////////////////
    ///  \brief get the dicom files' path vector
    ///
    ///  \param[in]    None
    ///  \param[out]   None
    ///  \return       std::vector<std::string>
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    const std::vector<std::string>& GetDicomFilePathVector() const { return m_vcDicomFilePathVector; }

    /////////////////////////////////////////////////////////////////
    ///  \brief get the dicom files' path vector
    ///
    ///  \param[in]    const std::vector<std::string> : the printed dicom images'path vector
    ///  \param[out]   None
    ///  \return       void
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void SetDicomFilePathVector(const std::vector<std::string>& val) { m_vcDicomFilePathVector = val; }

    /////////////////////////////////////////////////////////////////
    ///  \brief send filming command to filming module
    ///
    ///  \param[in]    ICommunicationProxy& proxy:  communication proxy of the filming command sender
    ///  \return       void
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void SendFilmingJobCommand(ICommunicationProxy& proxy);

    /////////////////////////////////////////////////////////////////
    ///  \brief send suspend all jobs filming command to filming module
    ///
    ///  \param[in]    ICommunicationProxy& proxy:  communication proxy of the filming command sender
    ///  \return       void
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void SuspendAllFilmingJob(ICommunicationProxy& proxy);

    /////////////////////////////////////////////////////////////////
    ///  \brief send resume all jobs filming command to filming module
    ///
    ///  \param[in]    ICommunicationProxy& proxy:  communication proxy of the filming command sender
    ///  \return       void
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void ResumeAllFilmingJob(ICommunicationProxy& proxy);

private:
    void Init();

    MCSF_FILMING_DISALLOW_COPY_AND_ASSIGN(McsfFilmingAutoJobCreater);
    
private:
    //FilmingPrintJob filmingPrintJob;

    std::string m_sPrinterAE;

    std::string m_sOurAE;

    std::string m_sPrinterIP;

    unsigned int m_iPrinterPort;

    PRINT_PRIORITY_ENUM m_ePrintPriority;

    std::string m_sPatientID;

    std::string m_sPatientName;

    std::string m_sPatientSex;

    std::string m_sPatientAge;

    std::string m_sOperatorName;

    std::string m_sAccessionNo;

    int m_iCopies;

    std::string m_sLayout;

    std::vector<std::string> m_vcDicomFilePathVector;

    int m_iSheetImageCount;

    /////////////////////////////////////////////////////////////////
    ///  \brief after call Set* methods init instance, then call this 
    ///  function to create a ProtoBuffer series object which will send
    ///  to FilmingBE.
    ///
    ///  \param[in]    sSerializedJob
    ///  \param[out]   sSerializedJob
    ///  \return       void
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void CreateFilmingJobInstance(std::string& sSerializedJob);

protected:
    /////////////////////////////////////////////////////////////////
    ///  \brief uplayer user define and put in a CommandContext instance,
    ///  the CommandContext's sReceiver,iCommandId,sSerializeObject will 
    ///  init here.
    ///
    ///  \param[in]    None
    ///  \param[out]   CommandContext& commandContext
    ///  \return       void
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void CreateFilmingJobCommandContext(CommandContext& commandContext);
};
#endif //MCSF_FILMING_AUTO_JOB_CREATER_H_
MCSF_FILMING_END_NAMESPACE
