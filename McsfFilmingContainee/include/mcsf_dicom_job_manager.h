//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging Healthcare Inc., 2012
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///           
///
///  \file    mcsf_dicom_job_manager.h
///  \brief   define the job manager to manage the job with function:ADD,
///     DELETE,PAUSE,RESUME,ChangePriority,GetNextJob.
///
///  \version 1.0
///  \date    Apr. 6, 2012
///  
//////////////////////////////////////////////////////////////////////////

#ifndef MCSF_DICOM_JOB_MANAGER_H_
#define MCSF_DICOM_JOB_MANAGER_H_

//ace include files
#include "ace/Message_Queue.h"
#include "ace/Null_Condition.h"
#include "ace/Null_Mutex.h"
#include "ace/OS_NS_unistd.h"
#include "ace/Thread_Manager.h"
#include "ace/OS_NS_string.h"

//inner include files
#include "mcsf_filming_containee_config.h"

MCSF_FILMING_BEGIN_NAMESPACE

    typedef bool (*McsfJobDelegateCallback)(
    const unsigned int iJobID
    );

typedef int (*McsfFindJobCallback)(
    const unsigned int iJobID
    );

class McsfDicomJobManager
{
public:
    McsfDicomJobManager();
    ~McsfDicomJobManager();

    /////////////////////////////////////////////////////////////////
    ///  \brief     AddJob
    ///
    ///  \param[in]    unsigned int iJobID
    ///                unsigned long lPriority
    ///  \param[out]   None
    ///  \return       int      success:0
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int AddJob(unsigned int iJobID, unsigned long lPriority);

    /////////////////////////////////////////////////////////////////
    ///  \brief     GetNextJob
    ///  get next job ID from job queue, the default start index is head.
    ///
    ///  \param[in]    unsigned int iJobID=0
    ///  \param[out]   None
    ///  \return       int      success:0
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int GetNextJob(unsigned int iJobID = 0);

    /////////////////////////////////////////////////////////////////
    ///  \brief     DeleteJob
    ///
    ///  \param[in]    unsigned int iJobID
    ///  \param[out]   None
    ///  \return       int      success:0
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int DeleteJob(unsigned int iJobID);

    /////////////////////////////////////////////////////////////////
    ///  \brief     DeleteAllJob
    ///
    ///  \param[in]    None
    ///  \param[out]   None
    ///  \return       int      success:0
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int DeleteAllJob();

    /////////////////////////////////////////////////////////////////
    ///  \brief     PauseJob
    ///
    ///  \param[in]    unsigned int iJobID
    ///  \param[out]   None
    ///  \return       int      success:0
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int PauseJob(unsigned int iJobID);

    /////////////////////////////////////////////////////////////////
    ///  \brief     PauseAllJobs
    ///
    ///  \param[in]    None
    ///  \param[out]   None
    ///  \return       int      success:0
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int PauseAllJobs();

    /////////////////////////////////////////////////////////////////
    ///  \brief     ResumeJob
    ///
    ///  \param[in]    unsigned int iJobID
    ///  \param[out]   None
    ///  \return       int      success:0
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int ResumeJob(unsigned int iJobID, unsigned long lPriority);

    /////////////////////////////////////////////////////////////////
    ///  \brief     ResumeAllJobs
    ///
    ///  \param[in]    None
    ///  \param[out]   None
    ///  \return       int      success:0
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int ResumeAllJobs();

    /////////////////////////////////////////////////////////////////
    ///  \brief     ChangePriority
    ///  Change one job's priority, the default priority is MEDIUM, you
    ///  can reset to LOW or HIGH.
    ///
    ///  \param[in]    unsigned int iJobID
    ///                 int iNewPriority
    ///  \param[out]   None
    ///  \return       int      success:0
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int ChangePriority(unsigned int iJobID, int iNewPriority);

    /////////////////////////////////////////////////////////////////
    ///  \brief     DoJob
    ///
    ///  \param[in]    McsfJobDelegateCallback jobDelegateCallback
    ///  The job delegate callback function pointer do the real worker
    ///  \param[out]   None
    ///  \return       int      success:0
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int DoJob(McsfJobDelegateCallback jobDelegateCallback);

private:
    /// \brief define a ACE_Message_Queue,used to store job.
    static ACE_Message_Queue<ACE_MT_SYNCH>* m_pMessageQueue;
};

MCSF_FILMING_END_NAMESPACE
#endif      //MCSF_DICOM_JOB_MANAGER_H_
