//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging Healthcare Inc., 2012
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///           
///
///  \file    mcsf_dicom_job_manager.cpp
///  \brief   implement the job manager to manage the job with function:ADD,
///     DELETE,PAUSE,RESUME,ChangePriority.
///
///  \version 1.0
///  \date    Apr. 6, 2012
///  
//////////////////////////////////////////////////////////////////////////

#include <sstream>
#include <exception>

#include "boost/lexical_cast.hpp"
#include <boost/thread/mutex.hpp>
#include "ace/Semaphore.h"

#include "mcsf_dicom_job_manager.h"

MCSF_FILMING_BEGIN_NAMESPACE

ACE_Message_Queue<ACE_MT_SYNCH>* McsfDicomJobManager::m_pMessageQueue = NULL;
McsfDicomJobManager::McsfDicomJobManager()
{
    m_pMessageQueue = new ACE_Message_Queue<ACE_MT_SYNCH>();
}

McsfDicomJobManager::~McsfDicomJobManager()
{
    try
    {
        if(NULL != m_pMessageQueue)
        {
            delete m_pMessageQueue;
            m_pMessageQueue = NULL;
        }
    }
    catch(...)
    {
    }
}

int McsfDicomJobManager::AddJob(unsigned int iJobID, unsigned long lPriority)
{
    //create a new message block specifying exactly how large
    //an underlying data block should be created.
    std::string sTemp = boost::lexical_cast<std::string>(iJobID);

    ACE_Message_Block *pTempMB = new ACE_Message_Block(sTemp.length()); 
    if (NULL == pTempMB) 
    {
        LOG_ERROR("create a new ACE_Message_Block failed!");
        return -1; 
    }

    //Insert data into the message block using the wr_ptr
    memcpy(pTempMB->wr_ptr(),sTemp.c_str(),sTemp.length());

    //Be careful to advance the wr_ptr by the necessary amount.
    //Note that the argument is of type "size_t" that is mapped to
    //bytes.
    pTempMB->wr_ptr(sTemp.length());
    pTempMB->msg_priority(lPriority);

    //Enqueue the print job message block onto the message queue
    if(-1 == m_pMessageQueue->enqueue_prio(pTempMB))
    {
        LOG_ERROR("\nCould not enqueue on to mq!!\n");
        return -1;
    }

    return 0;
}

int McsfDicomJobManager::GetNextJob(unsigned int iJobID)
{
    //TO DO...
    std::cout<<iJobID<<std::endl;
    return 0;
}

int McsfDicomJobManager::DeleteJob(unsigned int iJobID)
{
    ChangePriority(iJobID,0);
    return 0;
}

int McsfDicomJobManager::PauseJob(unsigned int iJobID)
{
    try
    {
        if (0 == iJobID)
        {
            LOG_INFO("Parameter error, jobid = 0 is perserved by backend");
            return -1;
        }

        ACE_Message_Block *mb=NULL;
        ACE_Message_Queue_Iterator<ACE_MT_SYNCH> it(*m_pMessageQueue);
        unsigned int id;
        while (!(it.done()))
        {   //lint -e534,  the return value will be test by it.done()
            it.next(mb);
            std::istringstream is(mb->rd_ptr());
            is >> id;
            //id = boost::lexical_cast<unsigned int>(mb->rd_ptr());
            if (id == iJobID)
            {
                LOG_INFO("Has found the job");

                //delete the origin job
                mb->msg_priority(0);

                LOG_INFO( 
                    string("Suspended a job, id is : ") 
                    + boost::lexical_cast<string>(iJobID));
                break;
            }

            it.advance();
            //lint +e534,  the return value will be test by it.done()
        }
        if (it.done())
        {
            LOG_INFO("Has not found the job, I can't tell whether the job is done or  exception");
            return -1;
        }
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
        return -1;
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
        return -1;
    }

    return 0;
}

int McsfDicomJobManager::ResumeJob(unsigned int iJobID, unsigned long lPriority)
{
    try
    {
        if (0 == iJobID)
        {
            LOG_INFO("Parameter error, jobid = 0 is perserved by backend");
            return -1;
        }

        //add the job back to the job queue
        //Enqueue the print job message block onto the message queue
        //create a new message block specifying exactly how large
        //an underlying data block should be created.
        std::string sTemp = boost::lexical_cast<std::string>(iJobID);
        ACE_Message_Block *pTempMB = new ACE_Message_Block(sTemp.length()); 
        if (NULL == pTempMB) 
        {
            LOG_ERROR("create a new ACE_Message_Block failed!");
            return -1; 
        }

        //Insert data into the message block using the wr_ptr
        memcpy(pTempMB->wr_ptr(),sTemp.c_str(),sTemp.length());

        //Be careful to advance the wr_ptr by the necessary amount.
        //Note that the argument is of type "size_t" that is mapped to
        //bytes.
        pTempMB->wr_ptr(sTemp.length());


        pTempMB->msg_priority(lPriority);
        if(-1 == m_pMessageQueue->enqueue_prio(pTempMB))
        {
            LOG_ERROR("\nCould not enqueue on to mq!!\n");
            return -1;
        }

        LOG_INFO("Succeeded to resume a Print job");
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
        return -1;
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
        return -1;
    }

    return 0;
}

int McsfDicomJobManager::ChangePriority(unsigned int iJobID, int lNewPriority)
{
    try
    {
        if (0 == iJobID)
        {
            LOG_INFO("Parameter error, jobid = 0 is perserved by backend");
            return -1;
        }

        unsigned int id = 0;
        ACE_Message_Block *mb=NULL;
        ACE_Message_Queue_Iterator<ACE_MT_SYNCH> it(*m_pMessageQueue);
        while (!(it.done()))
        {   //lint -e534,  the return value will be test by it.done()
            it.next(mb);
            //deserializing mb to get jobid;
            std::istringstream is(mb->rd_ptr());
            is >> id;
            //id = boost::lexical_cast<unsigned int>(mb->rd_ptr());
            if (id == iJobID && mb->msg_priority()) //need to check if the job is discarded
            {
                mb->msg_priority(lNewPriority);
                break;
            }

            it.advance();
            //lint +e534
        }
        if(it.done())
        {
            LOG_WARN(
                string("Don't find this Job:")
                + boost::lexical_cast<string>(iJobID));
            return -1;
        }

        LOG_INFO( "It's priority has been changed");
    }
    catch (...)
    {   //lint -e665
        LOG_ERROR(
            string("Exception when adjust priority of a job, job id is ") 
            + boost::lexical_cast<string>(iJobID) 
            + " New Priority is "
            + boost::lexical_cast<string>(lNewPriority));
        //lint +e665
        return -1;
    }

    return 0;
}

int McsfDicomJobManager::DoJob(McsfJobDelegateCallback jobDelegateCallback)
{
    try
    {
        ACE_Message_Block* pTempMB = NULL;

        while(true)
        {
            pTempMB = NULL;
            unsigned int iJobID = 0;
            //dequeue the head of the message queue until no more messages are
            //left. If there is no message_block in m_pMessageQueue, app will hung
            //in calling dequeue_prio() function.
            {
                bool  bRightJob = true;


                if(bRightJob && -1==m_pMessageQueue->dequeue_prio(pTempMB))
                {
                    LOG_WARN("Some exception happened, when dequeue job");
                    bRightJob = false;
                    continue;
                }

                if (bRightJob && NULL == pTempMB)
                {
                    LOG_ERROR("Can't get job information, when dequeue job");
                    continue;
                }

                if (bRightJob && !pTempMB->msg_priority())
                {
                    LOG_INFO("it's abandoned job");
                    bRightJob = false;
                    if (pTempMB->release())
                    {
                        LOG_WARN("There is something wrong when release ACE_MESSAGE_BLOCK.");
                    }
                    //delete pTempMB;
                    continue;
                }

                char* pDataArray = new char[pTempMB->length()+1];
                memset(pDataArray,0,pTempMB->length()+1);
                strncpy_s(pDataArray,pTempMB->length()+1,pTempMB->rd_ptr(),pTempMB->length());

                try
                {
                    iJobID = boost::lexical_cast<int>(pDataArray);

                    if(pDataArray != NULL)
                    {
                        delete pDataArray;
                        pDataArray = NULL;
                    }
                    if( bRightJob && (!iJobID))
                    {
                        LOG_WARN(
                            string("Can't find the job in job list, the id is ") 
                            + boost::lexical_cast<string>(iJobID));
                        bRightJob = false;
                    }
                }
                catch(...)
                {
                    if(pDataArray != NULL)
                    {
                        delete pDataArray;
                        pDataArray = NULL;
                    }

                    LOG_ERROR("Error occured when get jobID from ACE_MESSAGE_BLOCK!");
                    if (pTempMB->release())
                    {
                        LOG_WARN("There is something wrong when release ACE_MESSAGE_BLOCK.");
                    }
                    continue;
                }

                if (!bRightJob)
                {
                    if (pTempMB->release())
                    {
                        LOG_WARN("There is something wrong when release ACE_MESSAGE_BLOCK.");
                    }
                    continue;
                }
                //lint -e665
                LOG_WARN(
                    string("Begin to do job: " ) 
                    + boost::lexical_cast<string>(iJobID));  //<< " priority = " << pTempMB->msg_priority();
            }

            //Release the memory associated with the message block
            if (pTempMB->release())
            {
                LOG_WARN("There is something wrong when release ACE_MESSAGE_BLOCK.");
            }

            jobDelegateCallback(iJobID);
            
            LOG_WARN(
                string("End to do job: "  ) 
                + boost::lexical_cast<string>(iJobID));
            //lint +e665
        }
        delete pTempMB;
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
        return -1;
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
        return -1;
    }

    return 0;
}

MCSF_FILMING_END_NAMESPACE

