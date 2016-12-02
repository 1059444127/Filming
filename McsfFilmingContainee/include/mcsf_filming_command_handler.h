//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///           Wang Hui     hui.wang@united-imaging.com
///
///  \file    mcsf_filming_command_handler.h
///  \brief   define the filming containee command callback function and
///           process function
///
///  \version 1.0
///  \date    Nev. 2, 2011
///  
//////////////////////////////////////////////////////////////////////////

#ifndef MCSF_FILMING_COMMAND_HANDLER_H_
#define MCSF_FILMING_COMMAND_HANDLER_H_

#include <map>

//ace include files
#include "ace/Message_Queue.h"
#include "ace/Null_Condition.h"
#include "ace/Null_Mutex.h"
#include "ace/OS_NS_unistd.h"
#include "ace/Thread_Manager.h"
#include "ace/OS_NS_string.h"

//google proto buffer files
#include "McsfAppToolkit/McsfAppSaveFilming/mcsf_save_filming_cmd_context.pb.h"
//#include "PrinterConfig.pb.h"

//container interface include files
#include "McsfNetBase/mcsf_communication_proxy.h"
#include "McsfNetBase/mcsf_netbase_interface.h"


//DBWrapper interface include files
#include "McsfDatabase/mcsf_database_factory.h"
#include "McsfDatabase/mcsf_database_interface.h"

//filming libary include files
#include "mcsf_filming_libary_interface.h"
#include "mcsf_dcm_printer_config.h"
#include "mcsf_print_job_object.h"
#include "mcsf_filming_libary_factory.h"

//inner include files
#include "mcsf_filming_containee_config.h"
#include "mcsf_dicom_job_manager.h"

//status bar notify
#include "McsfMHC/mcsf_mhc_statusinfo_handler.h"
#include "McsfMHC/mcsf_mhc_msginfo_handler.h"

MCSF_FILMING_BEGIN_NAMESPACE
class McsfFilmingCommandHandler : public ICommandHandler
{
public:
    /////////////////////////////////////////////////////////////////
    ///  \brief     HandleCommand
    ///
    ///  \param[in]    const CommandContext* pContext
    ///  \param[out]   std::string* pReplyObject
    ///  \return       int
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    virtual int HandleCommand(const CommandContext* pContext,std::string* pReplyObject);

    /////////////////////////////////////////////////////////////////
    ///  \brief     constructor
    ///
    ///  \param[in]    ICommunicationProxy* pCommProxy
    ///		
    ///  \param[out]   None
    ///  \return       None
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    McsfFilmingCommandHandler(ICommunicationProxy* pCommProxy);

    /////////////////////////////////////////////////////////////////
    ///  \brief     destructor
    ///
    ///  \param[in]    None
    ///  \param[out]   None
    ///  \return       None
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    virtual ~McsfFilmingCommandHandler(void);

    /////////////////////////////////////////////////////////////////
    ///  \brief     start a consumer thread for filming
    ///
    ///  \param[in]    None
    ///  \param[out]   None
    ///  \return       None
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void OpenPrintThread();

    /////////////////////////////////////////////////////////////////
    ///  \brief     close the consumer thread for filming
    ///
    ///  \param[in]    None
    ///  \param[out]   None
    ///  \return       None
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void ClosePrintThread();

	/////////////////////////////////////////////////////////////////
	///  \brief     set the Retry Connect Printer Times
	///
	///  \param[in]    times
	///  \param[out]   None
	///  \return       None
	///  \pre \e  
	/////////////////////////////////////////////////////////////////
	void SetRetryConnectPrinterTimes(int);

	/////////////////////////////////////////////////////////////////
	///  \brief     set the Set Film box timeout Time
	///
	///  \param[in]    time
	///  \param[out]   None
	///  \return       None
	///  \pre \e  
	/////////////////////////////////////////////////////////////////
	void SetSetFilmBoxTimeOutTime(int);
	/////////////////////////////////////////////////////////////////
	///  \brief     Evaluate how much time (unit: seconds) left for all filming job done
	///  \return    work load
	///  \pre \e  
	/////////////////////////////////////////////////////////////////
	static int EvaluateRemainingWorkload();

	void SetArchivedSeriesDescription(const std::string& description ) {m_sArchivedSeriesDescription=description;}
    void SetAutoArchivingFlag(bool bAutoArchiving) {m_bAutoArchiving = bAutoArchiving;}

private:

	/////////////////////////////////////////////////////////////////
	///  \brief     adding a print job
	///
	///  \param[in]    printJob				print job object deserialized from Common Save
	///  \param[out]   pJobStatus           command executed results, function HandleCommand ensure it's not null
	///  \return       void         
	///  \pre \e  
	/////////////////////////////////////////////////////////////////
	void AddPrintJob(McsfCommonSave::FilmingPrintJob filmingPrintJob, McsfCommonSave::FilmingJobStatus *pJobStatus);

	/////////////////////////////////////////////////////////////////
	///  \brief     adding a print job
	///
	///  \param[in]    sSerializeObject     serialized print job information from FE
	///  \param[out]   pJobStatus           command executed results, function HandleCommand ensure it's not null
	///  \return       void         
	///  \pre \e  
	/////////////////////////////////////////////////////////////////
	void AddPrintJob(const std::string& sSerializeObject, McsfCommonSave::FilmingJobStatus *pJobStatus);
	
	void AutoArchiving( string seriesInstanceUid, string studyInstanceUid );

    /////////////////////////////////////////////////////////////////
    ///  \brief     A command function for adding a print job to job queue
    ///
    ///  \param[in]    sSerializeObject     serialized print job information from FE
    ///  \param[in]    uiJobID              id of the print job
    ///  \param[out]   pJobStatus           command executed results, function HandleCommand ensure it's not null
    ///  \return       None
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    //void AddPrintJobToQueue(const std::string& sSerializeObject, unsigned int uiJobID, FilmingJobStatus *pJobStatus);

    /////////////////////////////////////////////////////////////////
    ///  \brief     adding a print job to DB
    ///
    ///  \param[in]    sSerializeObject     serialized print job information from FE
    ///  \return       unsigned int         a new id of the job
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    //unsigned int AddPrintJobToDB(const std::string& sSerializeObject);

    /////////////////////////////////////////////////////////////////
    ///  \brief     A command function for Querying all print jobs from job queue
    ///
    ///  \param[out]    sSerializeObject     serialized print job information from FE
    ///  \return       None
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void QueryCurrentPrintJobs(/*out*/std::string* pReplyObject);

    /////////////////////////////////////////////////////////////////
    ///  \brief     A command function for Querying all history print jobs from filming DB
    ///
    ///  \param[in]    sSerializeObject     serialized print job information from FE
    ///  \return       None
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void QueryHistoryPrintJobs(const std::string& sSerializeObject,
        /*out*/std::string* pReplyObject);

    /////////////////////////////////////////////////////////////////
    ///  \brief     A command function for adjusting priority of a print job
    ///
    ///  \param[in]    sSerializeObject     serialized print job information from FE
    ///  \param[out]   pJobStatus           command executed results, function HandleCommand ensure it's not null
    ///  \return       None
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void AdjustPriorityOfPrintJob(const std::string& sSerializedJobInfo, McsfCommonSave::FilmingJobStatus *pJobStatus);

    /////////////////////////////////////////////////////////////////
    ///  \brief     A command function for Transfer To Printer of a print job
    ///
    ///  \param[in]    sSerializeObject     serialized print job information from FE
    ///  \param[out]   pJobStatus           command executed results, function HandleCommand ensure it's not null
    ///  \return       None
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void TransferToPrinterOfPrintJob(const std::string& sSerializedJobInfo, McsfCommonSave::FilmingJobStatus *pJobStatus);

    void McsfFilmingCommandHandler::TransferToPrinterOfPrintJob( unsigned int uiJobID, 
        string sPrinterAE, string sOurAE,string sPrinterIP,unsigned iPrinterPort,
        McsfCommonSave::FilmingJobStatus *pJobStatus );
    /////////////////////////////////////////////////////////////////
    ///  \brief     A command function for deleting a print job
    ///
    ///  \param[in]    sSerializeObject     serialized print job information from FE
    ///  \param[out]   pJobStatus           command executed results
    ///  \return       None
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void DeletePrintJob(const std::string& sSerializedJobID, McsfCommonSave::FilmingJobStatus *pJobStatus);

	static void DeleteEFilms(const McsfCommonSave::FilmingPrintJob& job );

    /////////////////////////////////////////////////////////////////
    ///  \brief adjust priority of a print job in job queue
    ///
    ///  \param[in]    uiJobID           job id of the job to be modified
    ///  \param[in]   ulNewPriority     new priority to be changed to
    ///  \param[out]   pJobStatus           command executed results, function HandleCommand ensure it's not null
    ///  \
    /////////////////////////////////////////////////////////////////
    void AdjustPriorityOfPrintJob(unsigned int uiJobID, unsigned long ulNewPriority, McsfCommonSave::FilmingJobStatus *pJobStatus);

    /////////////////////////////////////////////////////////////////
    ///  \brief     A command function for suspending a print job in job queue
    ///
    ///  \param[in]    sSerializeObject     serialized print job information from FE
    ///  \param[out]   pJobStatus           command executed results, function HandleCommand ensure it's not null
    ///  \return       None
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void PausePrintJob(const std::string& sSerializedJobID, McsfCommonSave::FilmingJobStatus *pJobStatus);

    /////////////////////////////////////////////////////////////////
    ///  \brief     A command function for resuming a suspended print job
    ///
    ///  \param[in]    sSerializeObject     serialized print job information from FE
    ///  \param[out]   pJobStatus           command executed results, function HandleCommand ensure it's not null
    ///  \return       None
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void ResumePrintJob(const std::string& sSerializedJobID, McsfCommonSave::FilmingJobStatus *pJobStatus);

    /////////////////////////////////////////////////////////////////
    ///  \brief     A command function for reprinting a historic print job
    ///
    ///  \param[in]    sSerializeObject     serialized print job information from FE
    ///  \param[out]   pJobStatus           command executed results, function HandleCommand ensure it's not null
    ///  \return       None
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void RePrintJob(const std::string& sSerializedJobID, McsfCommonSave::FilmingJobStatus *pJobStatus);

    /////////////////////////////////////////////////////////////////
    ///  \brief     A command function for reprinting a historic print job or resume a paused(stopped) job
    ///
    ///  \param[in]    sSerializeObject     serialized print job information from FE
    ///  \param[out]   pJobStatus           command executed results, function HandleCommand ensure it's not null
    ///  \return       None
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void RestartPrintJob(const std::string& sSerializedJobID, McsfCommonSave::FilmingJobStatus *pJobStatus);

    /////////////////////////////////////////////////////////////////
    ///  \brief     A command function for Querying the printer config list
    ///
    ///  \param[in]    sSerializeObject     serialized print job information from FE
    ///  \return       None
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    void ReplyPrinterConfig(std::string* pReplyObject);

	/////////////////////////////////////////////////////////////////
	///  \brief     A command function for Querying the printer config list
	///
	///  \param[in]    sSerializeObject     serialized print job information from FE
	///  \return       None
	///  \pre \e  
	/////////////////////////////////////////////////////////////////
    static void RePlyJobInfoToMainFrame();
/////////////////////////////////////////////////////////////////
    ///  \brief     向mainfram发送打印进度
    ///
    ///  \param[in]    uiJobID     正在打印的任务ID
	///  \param[in]    iTotalNum   当前任务内打印胶片总数
	///  \param[in]    iFinishedNum     当前任务内打印完成的胶片数
	///  \param[in]    iErrorNum     当前任务内打印失败的胶片数，由于目前实现中一旦打印失败就任务终止，所以该参数最多为1
    ///  \param[out]   无
    ///  \return       None
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    static void RePlyProgressInfoToMainFrame(unsigned int uiJobID,int iTotalNum, int iFinishedNum, int iErrorNum);
    /// \brief do print
    static void DoPrint();

    /// \brief regular collect statistical info, and send to service center
    //static void StatisticalServe();

    /// \brief do real print
    //static bool DoRealPrint(const char* pPrintJobStream, int iJobStreamSize);

    /// \brief do real print
    static bool DoRealPrint(const unsigned int uiJobID);

	static void UpdatePrintStatus(McsfCommonSave::FilmingPrintJob &filmingPrintJob, bool bResult );

    /// \brief initialization,  now can load "pending" jobs from db to print
    void InitialJobsFromDB();

    /// \brief set filming job status
    void SetFilmingJobStatus(McsfCommonSave::FilmingJobStatus *pJobStatus, McsfCommonSave::JobStatus eJobStatus,
        int iReturnValue, std::string sReturnMeaning);

	/// \brief calculate max priority in m_PrintJobMap, the smaller of the value, the higher of the priority
	unsigned long CalculateMaxPriority();

	// Current Command Sender
	std::string m_sCurrentCommandSender;
	
	////DB pointer just for sending db update event;
	IDatabasePtr m_pDBNotifier;

    /// \brief communication proxy instance, initialized by Container.
    static ICommunicationProxy* m_pCommProxy;

    /// \brief define a mysql DB implement class
    static McsfFilmingDB* m_pFilmingDB;
    
    static IDatabasePtr m_pDatabase;

    static std::map<unsigned int, McsfCommonSave::FilmingPrintJob>  m_PrintJobMap;

	std::string m_sArchivedSeriesDescription;
    bool m_bAutoArchiving;

	static int m_iRetryConnectPrinterTimes;

	static int m_iSetFilmBoxTimeOutTime;

protected:
    ///// \brief split border density string:150\20\BLACK\WHITE
    //void SplitBorderDensity(FilmingPrinterList_Printer* pPrinter, std::string sArrayString);

    ///// \brief split display format to array layout:1,1\1,2\2,2\2,3\3,3\3,4\3,5\4,4\4,5
    //void SplitDisplayFormat(FilmingPrinterList_Printer* pPrinter, std::string sArrayString);

    ///// \brief split empty image density:20\BLACK\WHITE
    //void SplitEmptyImageDensity(FilmingPrinterList_Printer* pPrinter, std::string sArrayString);

    ///// \brief split film destination:MAGAZINE\PROCESSOR\BIN_1\BIN_2
    //void SplitFilmDestination(FilmingPrinterList_Printer* pPrinter, std::string sArrayString);

    ///// \brief split film size id: 8INX10IN\10INX12IN\10INX14IN\11INX14IN\14INX14IN\14INX17IN\24CMX24CM\24CMX30CM
    //void SplitFilmSize(FilmingPrinterList_Printer* pPrinter, std::string sArrayString);

    ///// \brief split magnification: REPLICATE\BILINEAR\CUBIC\NONE
    //void SplitMagnificationType(FilmingPrinterList_Printer* pPrinter, std::string sArrayString);

    ///// \brief split medium type: PAPER\CLEAR FILM\BLUE FILM
    //void SplitMediumType(FilmingPrinterList_Printer* pPrinter, std::string sArrayString);

    /// \brief init a McsfPrintJobObject with protobuffer class FilmingPrintJob
    static void InitMcsfPrintJob(const McsfCommonSave::FilmingPrintJob& filmingPrintJob,
        /*out*/std::vector<McsfPrintJobObject>* pMcsfPrintJobVector);

    /// \brief export a McsfPrintJobObjectVector to protobuffer class FilmingPrintJob
    static void ExportMcsfPrintJob(const vector<McsfPrintJobObject>& mcsfPrintJobVector, 
        /*out*/McsfCommonSave::FilmingPrintJob* pFilmingPrintJob);

    /////////////////////////////////////////////////////////////////
    ///  \brief     A command function for save electronic films
    ///
    ///  \param[in]    sSerializePara     file path separated by ";"
    ///  \param[out]   pReplyObject       trace info
    ///  \return       indicate complete status, 0 indicating success, -1 indicating that there is something wrong
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int SaveEFilms( const std::string& sSerializedPara, std::string *pReplyObject );

    /////////////////////////////////////////////////////////////////
    ///  \brief     A command function for save electronic films
    ///
    ///  \param[in]    imagePathVector,     file path separated by List
    ///  \param[out]   pReplyObject       trace info
    ///  \return       indicate complete status, 0 indicating success, -1 indicating that there is something wrong
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int SaveEFilms( const vector<string>& imagePathVector, std::string *pReplyObject );

    /////////////////////////////////////////////////////////////////
    ///  \brief     A command function for save electronic films
    ///
    ///  \param[in]    job     filming print job
    ///  \param[out]   pReplyObject       trace info
    ///  \return       indicate complete status, 0 indicating success, -1 indicating that there is something wrong
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int SaveEFilms(const McsfCommonSave::FilmingPrintJob& job, std::string *pReplyObject);

	/////////////////////////////////////////////////////////////////
	///  \brief     sendout db update event(study imported)
	///
	///  \param[in]    sStudyInstanceUIDs     study instance uids  separated by ";"
	///  \return       N/A
	///  \pre \e  
	/////////////////////////////////////////////////////////////////
	void McsfFilmingCommandHandler::NotifyStudyImported(string sStudyInstanceUIDs);

	///////////////////////////////////////////////////////////////////
	/////  \brief     sendout db update event(study imported)
	/////
	/////  \param[in]    studyInstanceUIDSet     study instance uid set 
	/////  \return       N/A
	/////  \pre \e  
	///////////////////////////////////////////////////////////////////
	//void McsfFilmingCommandHandler::NotifyStudyImported(set<string> studyInstanceUIDSet);
	/// monitor thread id
    ACE_thread_t m_iThreadId;
    /// stastical server thread id
    //ACE_thread_t m_iServerThreadId;

    static McsfDicomJobManager* m_pDicomJobManager;
};
MCSF_FILMING_END_NAMESPACE

#endif          //MCSF_FILMING_COMMAND_HANDLER_H_



