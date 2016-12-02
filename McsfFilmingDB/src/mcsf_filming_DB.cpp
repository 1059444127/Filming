//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_filming_DB.cpp
///  \brief   filming database
///
///  \version 1.0
///  \date    Oct. 26, 2011
///  
//////////////////////////////////////////////////////////////////////////

#include "mcsf_filming_DB.h"

#include <boost/date_time/posix_time/posix_time.hpp>
#include <boost/filesystem.hpp>//create directory

// configure file
#include "Common/McsfSystemEnvironmentConfig/mcsf_systemenvironment_factory.h"

MCSF_FILMING_BEGIN_NAMESPACE

McsfFilmingDB::McsfFilmingDB():
m_sUser(EMPTY_STRING),
m_sPassword(EMPTY_STRING),
m_sDatabaseName(EMPTY_STRING),
m_sHostName(EMPTY_STRING),
m_uiPort(0),
m_uiConnectionPoolFactory(0)
{

}

McsfFilmingDB::~McsfFilmingDB(void)
{
}

McsfFilmingDB* McsfFilmingDB::GetInstance()
{
    static McsfFilmingDB filmDB;

    return &filmDB;
}

//int McsfFilmingDB::GetSPFilePathFromDB( 
//    /*in*/const unsigned int iJobID, 
//    /*out*/std::string* pSPfilePath )
//{
//    try
//    {
//        typedef odb::query<SPfileTable> querySPfileTable;
//        typedef odb::result<SPfileTable> resultSPfileTable;
//
//        if ( !IsDBAvailable())  return -1;
//        
//        odb::core::transaction trans(m_pMySqlDB->begin ());
//
//        resultSPfileTable rSPfileTable(m_pMySqlDB->query<SPfileTable> (
//            querySPfileTable::JobID == iJobID));
//        trans.commit ();
//
//        if (0 == rSPfileTable.size())
//        {
//            return -1;//;
//        }
//
//        for (resultSPfileTable::iterator i=rSPfileTable.begin(); i != rSPfileTable.end (); ++i)
//        {     
//            *pSPfilePath = (*it)->GetSP_FilePath();
//
//        }
//        
//    }
//    catch (const odb::exception& e)
//    {
//        printf("%s \n", e.what());
//        std::string sDescription("error occur with: ");
//        sDescription += e.what();
//        LOG_ERROR_DB(sDescription);
//        return -1;//;
//    }
//    catch (...)
//    {
//        LOG_ERROR_DB("fatal error! in GetSPFilePathFromDB.");
//        return  -1;//;
//    }
//
//    return 0;//;
//}
//
//int McsfFilmingDB::GetHGFilePathListFromDB(/*in*/const int iSPfileID,
//    /*out*/std::vector<std::string>* pHGfilePathList)
//{
//    try
//    {
//        typedef odb::query<HGfileTable> queryHGfileTable;
//        typedef odb::result<HGfileTable> resultHGfileTable;
//
//        if ( !IsDBAvailable())  return -1;
//        
//        odb::core::transaction trans(m_pMySqlDB->begin ());
//
//        resultHGfileTable rHGfileTable(m_pMySqlDB->query<HGfileTable> (
//            queryHGfileTable::SP_FileID == iSPfileID));
//        trans.commit ();
//
//        if (0 == rHGfileTable.size())
//        {
//            return -1;//;
//        }
//
//        for (resultHGfileTable::iterator i=rHGfileTable.begin(); i != rHGfileTable.end (); ++i)
//        {     
//            std::string tempHGfilePath = (*it)->GetHG_FIlePath();
//
//            pHGfilePathList->push_back(tempHGfilePath);
//        }
//    }
//    catch (const odb::exception& e)
//    {
//        printf("%s \n", e.what());
//        std::string sDescription("error occur in GetHGFilePathListFromDB with: ");
//        sDescription += e.what();
//        LOG_ERROR_DB(sDescription);
//        return -1;//;
//    }
//    catch (...)
//    {
//        LOG_ERROR_DB("fatal error! in GetHGFilePathListFromDB");
//        return  -1;//;
//    }
//
//    return 0;//;
//}


//        iJobID = 0;//return invalid job ID
//    }
//    catch (...)
//    {
//        LOG_ERROR_DB("fatal error! in GetJobID!");
//        iJobID = 0;//;
//    }
//    return iJobID;
//}

//unsigned int McsfFilmingDB::GetNewFilmID()
//{
//    unsigned int iFilmID = 0;
    //try
    //{
    //    typedef odb::query<PrintFilmTable> queryPrintFilmTable;
    //    typedef odb::result<PrintFilmTable> resultPrintFilmTable;

    //    if ( !IsDBAvailable())  return 0;
    //    
    //    odb::core::transaction trans(m_pMySqlDB->begin ());

    //    //get the maximum FilmID
    //    resultPrintFilmTable rPrintFilmTable(m_pMySqlDB->query<PrintFilmTable> (
    //        "1=1 order by FilmID desc limit 1"));
    //    trans.commit();

    //    if (0 == rPrintFilmTable.size())
    //    {
    //        //LOG_WARN_DB("no record in table: PrintFilmTable.assign a new FilmID from 1.");
    //        return 1;//the first film id, start from 1.
    //    }

    //    for (resultPrintFilmTable::iterator i=rPrintFilmTable.begin(); i != rPrintFilmTable.end (); ++i)
    //    {     
    //        iFilmID = (*it)->GetFilmID();
    //        iFilmID++;
    //    }
    //    
    //}
    //catch (const odb::exception& e)
    //{
    //    printf("%s \n", e.what());
    //    std::string sDescription("error occur in GetNewFilmID with: ");
    //    sDescription += e.what();
    //    LOG_ERROR_DB(sDescription);
    //    iFilmID = 0;//return invalid job ID
    //}
    //catch (...)
    //{
    //    LOG_ERROR_DB("fatal error! in GetNewFilmID!");
    //    iFilmID = 0;//;
    //}
//    return iFilmID;
//}

unsigned int McsfFilmingDB::InsertPrintJobToDB(McsfPrintJobObject* pPrintJob)
{
    unsigned int iJobID = 0;
    if(NULL == pPrintJob)
    {
        LOG_ERROR_DB("invalid print job object!");
        return 0;
    }

    try
    {

        IDatabaseFactory::IDBPointer pDBPointer;
        IDatabasePtr pDatabase;
        ConnectToDB(pDBPointer, pDatabase);

        //if this print job(film box) has existed in DB, just insert low level table
        if(pPrintJob->GetJobID() != 0)
        {
            iJobID = pPrintJob->GetJobID();
        }
        else// (!pPrintJob->GetJobID())
        {
            Mcsf::IFilmingJobPtr pFilmingJob= pDBPointer->CreateFilmingJob();
    
            const boost::posix_time::ptime ptimeLocalTimeInstance= boost::posix_time::microsec_clock::local_time();
            const boost::gregorian::date dtDateInstance =ptimeLocalTimeInstance.date();
            const boost::posix_time::time_duration tdTimeInstance=ptimeLocalTimeInstance.time_of_day();
            //17 parameters
            //pFilmingJob->SetJobID(g_iJobID);//Job ID
            pFilmingJob->SetFilmingDate(dtDateInstance);//FilmingDate
            pFilmingJob->SetFilmingTime(tdTimeInstance);//FilmingTime
            pFilmingJob->SetJobStatus(pPrintJob->GetJobStatus());//JobStatus
            pFilmingJob->SetJobPriority(pPrintJob->GetJobPriority());//JobPriority
            pFilmingJob->SetFilmAmount(pPrintJob->GetFilmAmount());//the number of total film
            pFilmingJob->SetRefPatientID(pPrintJob->GetRefPatientID());//patient ID
            pFilmingJob->SetRefPatientName(pPrintJob->GetRefPatientName());//patient name
            pFilmingJob->SetRefPatientAge(pPrintJob->GetRefPatientAge());//patient age
            pFilmingJob->SetRefPatientSex(pPrintJob->GetRefPatientSex());//patient sex
            pFilmingJob->SetOperatorName(pPrintJob->GetOperatorName());//operator doctor name
            pFilmingJob->SetRefAccessionNo(pPrintJob->GetAccessionNo());//accession Number
            pFilmingJob->SetOurAE(pPrintJob->GetMyAETitle());//our workstation ae
            pFilmingJob->SetPrinterAE(pPrintJob->GetTargetAETitle());//printer ae
            pFilmingJob->SetPrinterIP(pPrintJob->GetPrinterIP());//printer ip or hostname
            pFilmingJob->SetPrinterPort(pPrintJob->GetTargetPort());//listen port
            pFilmingJob->SetMaxPDU(pPrintJob->GetMaxPDU());//max PDU
            pFilmingJob->SetImplicitOnly(pPrintJob->GetTargetImplicitOnly());//ImplicitOnly
            pFilmingJob->SetSupportsPLUT(pPrintJob->GetTargetSupportsPLUT());//SupportsPLUT
            pFilmingJob->SetSupportsAnnotation(pPrintJob->GetTargetSupportsAnnotation());//SupportsAnnotation
            pFilmingJob->SetSupports12bit(pPrintJob->GetTargetSupports12bit());//Supports12bit
            pFilmingJob->SetPLUTinFilmSession((int)pPrintJob->GetTargetPLUTinFilmSession());//PLUTinFilmSession
            pFilmingJob->SetRerquiresMatchingLUT(pPrintJob->GetTargetRequiresMatchingLUT());//RerquiresMatchingLUT
            pFilmingJob->SetPreferSCPLUTRendering(pPrintJob->GetTargetPreferSCPLUTRendering());//PreferSCPLUTRendering
            pFilmingJob->SetOrientation(pPrintJob->GetOrientation());//Orientation
            int iDBInfo = pDatabase->InsertFilmingJobObject(pFilmingJob);
            if(ERROR_DB_NULL != iDBInfo )
            {
                string sDBInfo = boost::lexical_cast<string>(iDBInfo);
                LOG_WARN_DB("fail to insert a job to DB, DBInfo: " + sDBInfo);
            }
            else
            {
                iJobID = (unsigned int)pFilmingJob->GetJobID();
                if(!InsertPrintFilmToDB(iJobID,pPrintJob))
                {   //lint -e665
                    LOG_WARN_DB(
                        string("something wrong when InsertPrintFilmToDB, the jobid is ") 
                        + boost::lexical_cast<string>(iJobID));
                }   //lint +e665
            }
        }
    }
    catch (std::exception& e)
    {
        LOG_FUNC_EXCEPTION_DB(e.what());
    }
    catch (...)
    {
        LOG_FUNC_EXCEPTION_DB("general exception");
    }

    return iJobID;
}

unsigned int McsfFilmingDB::InsertPrintFilmToDB(unsigned int iJobID, McsfPrintJobObject* pPrintJob)
{
    unsigned int iFilmID = 0;

    if(iJobID < 1)
    {
        LOG_ERROR_DB("invalid job ID in InsertPrintFilmToDB!");
        return 0;
    }

    if(NULL == pPrintJob)
    {
        LOG_ERROR_DB("invalid print job object in InsertPrintFilmToDB!");
        return 0;
    }

    try
    {
        IDatabaseFactory::IDBPointer pDBPointer;
        IDatabasePtr pDatabase;
        ConnectToDB(pDBPointer, pDatabase);

        Mcsf::IPrintFilmPtr pPrintFilm= pDBPointer->CreatePrintFilm();

        const boost::posix_time::ptime ptimeLocalTimeInstance= boost::posix_time::microsec_clock::local_time();
        const boost::gregorian::date dtDateInstance =ptimeLocalTimeInstance.date();
        const boost::posix_time::time_duration tdTimeInstance=ptimeLocalTimeInstance.time_of_day();

        //pPrintFilm->SetFilmID(0);//FilmID
        pPrintFilm->SetJobIDFk(iJobID);//bigint not null
        pPrintFilm->SetFilmAmount(pPrintJob->GetCopies());//the number of total film
        pPrintFilm->SetLayout(pPrintJob->GetLayout());//display format
        pPrintFilm->SetFilmSize(pPrintJob->GetFilmSize());//film size
        pPrintFilm->SetCreateDate(dtDateInstance);//CreateDate
        pPrintFilm->SetCreateTime(tdTimeInstance);//CreateTime
        pPrintFilm->SetHavePrinted(0);//mark this film have been printed

        int iDBInfo = pDatabase->InsertPrintFilmObject(pPrintFilm);
        if(ERROR_DB_NULL != iDBInfo )
        {
            string sDBInfo = boost::lexical_cast<string>(iDBInfo);
            LOG_WARN_DB("Fail to insert a film to DB, DBInfo: " + sDBInfo);
        }
        else
        {
            iFilmID = (unsigned int)pPrintFilm->GetFilmID();
            //when insert print film to DB with success. then insert images to DB
            if(!InsertPrintImagesToDB(iFilmID,pPrintJob->GetFileNameList()))
            {   //lint -e665
                LOG_WARN_DB(
                    string("something wrong when InsertPrintImagesToDB, the film id is " ) 
                    + boost::lexical_cast<string>(iFilmID));
            }   //lint +e665
        }

    }
    catch (const std::exception& e)
    {
        LOG_FUNC_EXCEPTION_DB(e.what());
        return  0;
    }
    catch (...)
    {
        LOG_FUNC_EXCEPTION_DB("general exception");
        return  0;
    }

    return iFilmID;
}

int McsfFilmingDB::InsertPrintImagesToDB(
    unsigned int iFilmID, 
    const std::vector<std::string>& vPrintImagesVector)
{
    int iRet = 0;
    try
    {
        IDatabaseFactory::IDBPointer pDBPointer;
        IDatabasePtr pDatabase;
        ConnectToDB(pDBPointer, pDatabase);

        if(vPrintImagesVector.size() <= 0)
        {
            LOG_ERROR_DB("invalid print images vector!");
            return -1;
        }

        if(iFilmID < 1)
        {
            LOG_ERROR_DB("invalid job ID!");
            return -1;
        }

        for(size_t i = 0; i< vPrintImagesVector.size();i++)
        {
            if(vPrintImagesVector[i].empty())
            {
                LOG_WARN_DB("empty file path!");
                continue;
            }

            Mcsf::IPrintImagePtr pPrintImage= pDBPointer->CreatePrintImage();

            const boost::posix_time::ptime ptimeLocalTimeInstance= boost::posix_time::microsec_clock::local_time();
            const boost::gregorian::date dtDateInstance =ptimeLocalTimeInstance.date();
            const boost::posix_time::time_duration tdTimeInstance=ptimeLocalTimeInstance.time_of_day();

            //pPrintImage->SetPrintImageID(0);//Pk
            pPrintImage->SetFilmIDFk(iFilmID);//FilmID
            pPrintImage->SetPath(vPrintImagesVector[i]);//Path
            pPrintImage->SetPrintImagePosition(0);//ImagePosotion
            pPrintImage->SetCreateDate(dtDateInstance);//CreateDate
            pPrintImage->SetCreateTime(tdTimeInstance);//CreateTime
            pPrintImage->SetHaveUsed(0);//mark this film have been printed

            
            int iDBInfo = pDatabase->InsertPrintImageObject(pPrintImage);
            if(ERROR_DB_NULL != iDBInfo )
            {
                string sDBInfo = boost::lexical_cast<string>(iDBInfo);
                LOG_WARN_DB("Fail to insert a image to DB, DBInfo: " + sDBInfo);
                iRet = -1;
            }
            else
            {
                iRet = (int)pPrintImage->GetPrintImageID();
            }
        
        }
    }
    catch( const std::exception& e)
    {
        LOG_FUNC_EXCEPTION_DB(e.what());
        iRet = -1;
    }
    catch(...)
    {
        LOG_FUNC_EXCEPTION_DB("general exception");
        iRet = -1;
    }

    return iRet;
}

int McsfFilmingDB::GetPrintFilmJobByID(unsigned int iJobID, 
    /*out*/std::vector<McsfPrintJobObject>* pFilmingJobVector)
{
    try
    {
        IDatabaseFactory::IDBPointer pDBPointer;
        IDatabasePtr pDatabase;
        ConnectToDB(pDBPointer, pDatabase);

        Mcsf::IFilmingJobPtr pFilmingJob= pDBPointer->CreateFilmingJob();
        int iDBInfo = pDatabase->GetFilmingJobObjectByID(iJobID, pFilmingJob);
        if(ERROR_DB_NULL != iDBInfo )
        {
            string sDBInfo = boost::lexical_cast<string>(iDBInfo);
            string sDescription = string("Fail to get Filming job from DB, of which job id is : ") 
                + boost::lexical_cast<string>(iJobID); 
            LOG_ERROR_DB(sDescription + " DBInfo: " +sDBInfo);
            return -1;
        }
        
        std::vector<IPrintFilmPtr> PrintFilmPtrArray;
        const std::string strSQL = string("JobIDFk = ") 
            + boost::lexical_cast<string>(iJobID)
            + " ORDER BY FilmID";
        if (ERROR_DB_NULL != pDatabase->GetPrintFilmListBySQL(strSQL, PrintFilmPtrArray))
        {
            string sDescription = string("Fail to get Films from DB, of which job id is : ") 
                + boost::lexical_cast<string>(iJobID); 
            LOG_ERROR_DB(sDescription);
            return -1;
        }
        
        for (auto itFilm = PrintFilmPtrArray.begin(); itFilm != PrintFilmPtrArray.end(); itFilm++)
        {

            int iFilmID = (int)(*itFilm)->GetFilmID();

            std::vector<IPrintImagePtr> PrintImagePtrArray;
            const std::string strSQL= "FilmIDFk=" +
                boost::lexical_cast<std::string>(iFilmID) + " ORDER BY PrintImageID";
            int iDBInfo = pDatabase->GetPrintImageListBySQL(strSQL, PrintImagePtrArray);
            if(ERROR_DB_NULL != iDBInfo )
            {
                string sDBInfo = boost::lexical_cast<string>(iDBInfo);
                string sDescription = string("Fail to get images from DB, of which job id is : ") 
                    + boost::lexical_cast<string>(iJobID);
                LOG_ERROR_DB(sDescription + " DBInfo: " + sDBInfo);
                return -1;
            }

            McsfPrintJobObject tempPrintJob;

            //Print job
            tempPrintJob.SetJobID(iJobID);
            tempPrintJob.SetMyAETitle(pFilmingJob->GetOurAE());
            tempPrintJob.SetTargetHostName(pFilmingJob->GetPrinterIP());
            tempPrintJob.SetTargetAETitle(pFilmingJob->GetPrinterAE());
            tempPrintJob.SetTargetPort(static_cast<unsigned short>(pFilmingJob->GetPrinterPort()));
            tempPrintJob.SetTargetMaxPDU(static_cast<unsigned long>(pFilmingJob->GetMaxPDU()));
            tempPrintJob.SetTargetImplicitOnly(pFilmingJob->GetImplicitOnly());
            tempPrintJob.SetTargetSupportsPLUT(pFilmingJob->GetSupportsPLUT());
            tempPrintJob.SetTargetSupportsAnnotation(pFilmingJob->GetSupportsAnnotation());
            tempPrintJob.SetTargetSupports12bit(pFilmingJob->GetSupports12bit());
            tempPrintJob.SetTargetPLUTinFilmSession(pFilmingJob->GetPLUTinFilmSession());
            tempPrintJob.SetTargetRequiresMatchingLUT(pFilmingJob->GetRerquiresMatchingLUT());
            tempPrintJob.SetTargetPreferSCPLUTRendering(pFilmingJob->GetPreferSCPLUTRendering());
            tempPrintJob.SetPriority(pFilmingJob->GetJobPriority());
            tempPrintJob.SetOrientation(pFilmingJob->GetOrientation());
            //tempPrintJob.SetJobStatus(pFilmingJob->GetJobStatus());
            //tempPrintJob.SetJobPriority(pFilmingJob->GetJobPriority());
            //tempPrintJob.SetFilmAmount(pFilmingJob->GetFilmAmount());
            tempPrintJob.SetRefPatientID(pFilmingJob->GetRefPatientID());
            tempPrintJob.SetRefPatientName(pFilmingJob->GetRefPatientName());
            tempPrintJob.SetRefPatientAge(pFilmingJob->GetRefPatientAge());
            tempPrintJob.SetRefPatientSex(pFilmingJob->GetRefPatientSex());
            tempPrintJob.SetOperatorName(pFilmingJob->GetOperatorName());
            tempPrintJob.SetAccessionNo(pFilmingJob->GetRefAccessionNo());

            //Print Film
            tempPrintJob.SetLayout((*itFilm)->GetLayout());
            tempPrintJob.SetFilmSize((*itFilm)->GetFilmSize());
            
            //Print Image
            std::vector<std::string> imagePathVector;
            for (auto itImageBox = PrintImagePtrArray.begin(); 
                itImageBox != PrintImagePtrArray.end(); itImageBox++)
            {
                imagePathVector.push_back((*itImageBox)->GetPath());
            }
            if (!tempPrintJob.SetFileNameList(imagePathVector))
            {
                LOG_WARN_DB("warning on tempPrintJob.SetFileNameList");
            }

            pFilmingJobVector->push_back(tempPrintJob);

        }
    }
    catch (const std::exception& e)
    {
        LOG_FUNC_EXCEPTION_DB(e.what());
        return -1;//;
    }
    catch (...)
    {
        LOG_FUNC_EXCEPTION_DB("general exception");
        return -1;//;
    }

    return 0;//;
}

int McsfFilmingDB::GetHistoryPrintJob(const std::string& sConditionSql, 
    /*out*/std::vector<McsfPrintJobObject>* pFilmingJobVector)
{
    
    int iRet = 0;
    try
    {
        IDatabaseFactory::IDBPointer pDBPointer;
        IDatabasePtr pDatabase;
        ConnectToDB(pDBPointer, pDatabase);

        std::vector<IFilmingJobPtr> FilmingJobPtrArray;
        int iDBInfo = pDatabase->GetFilmingJobListBySQL(sConditionSql, FilmingJobPtrArray);
        if(ERROR_DB_NULL != iDBInfo )
        {
            string sDBInfo = boost::lexical_cast<string>(iDBInfo);
            LOG_ERROR_DB(string("Fail to get Filming job from DB, condition sql is: ") + sConditionSql + " DBInfo is " + sDBInfo);
            return -1;
        }
        
        iRet = static_cast<int>(FilmingJobPtrArray.size());

        for (auto it=FilmingJobPtrArray.begin(); it != FilmingJobPtrArray.end (); ++it)
        {
            McsfPrintJobObject tempPrintJob;
            tempPrintJob.SetJobID((unsigned int)(*it)->GetJobID());
            tempPrintJob.SetJobStatus((*it)->GetJobStatus());
            tempPrintJob.SetMyAETitle((*it)->GetOurAE());
            tempPrintJob.SetTargetHostName((*it)->GetPrinterIP());
            tempPrintJob.SetTargetAETitle((*it)->GetPrinterAE());
            tempPrintJob.SetTargetPort(static_cast<unsigned short>((*it)->GetPrinterPort()));
            tempPrintJob.SetTargetMaxPDU(static_cast<unsigned long>((*it)->GetMaxPDU()));
            tempPrintJob.SetTargetImplicitOnly((*it)->GetImplicitOnly());
            tempPrintJob.SetTargetSupportsPLUT((*it)->GetSupportsPLUT());
            tempPrintJob.SetTargetSupportsAnnotation((*it)->GetSupportsAnnotation());
            tempPrintJob.SetTargetSupports12bit((*it)->GetSupports12bit());
            tempPrintJob.SetTargetPLUTinFilmSession((*it)->GetPLUTinFilmSession());
            tempPrintJob.SetTargetRequiresMatchingLUT((*it)->GetRerquiresMatchingLUT());
            tempPrintJob.SetTargetPreferSCPLUTRendering((*it)->GetPreferSCPLUTRendering());
            tempPrintJob.SetPriority((*it)->GetJobPriority());
            tempPrintJob.SetOrientation((*it)->GetOrientation());

            tempPrintJob.SetJobStatus((*it)->GetJobStatus());
            tempPrintJob.SetJobPriority((*it)->GetJobPriority());
            //tempPrintJob.SetFilmAmount((*it)->GetFilmAmount());
            tempPrintJob.SetRefPatientID((*it)->GetRefPatientID());
            tempPrintJob.SetRefPatientName((*it)->GetRefPatientName());
            tempPrintJob.SetRefPatientAge((*it)->GetRefPatientAge());
            tempPrintJob.SetRefPatientSex((*it)->GetRefPatientSex());
            tempPrintJob.SetOperatorName((*it)->GetOperatorName());
            tempPrintJob.SetAccessionNo((*it)->GetRefAccessionNo());


            pFilmingJobVector->push_back(tempPrintJob);
        }
    }
    catch (const std::exception& e)
    {
        LOG_FUNC_EXCEPTION_DB(e.what());
        return -1;//;
    }
    catch (...)
    {
        LOG_FUNC_EXCEPTION_DB("general exception");
        return  -1;//;
    }

    return iRet;
}

int McsfFilmingDB::GetAllPrintJob(
    /*out*/std::vector<McsfPrintJobObject>* pFilmingJobVector)
{
    return GetHistoryPrintJob(string("1=1"), pFilmingJobVector);
}

int McsfFilmingDB::DeleteJobByID( unsigned int iJobID )
{
    LOG_FUNC_BEGIN_DB 
    try
    {
        IDatabaseFactory::IDBPointer pDBPointer;
        IDatabasePtr pDatabase;
        ConnectToDB(pDBPointer, pDatabase);

        int iDBInfo = pDatabase->EraseFilmingJobObject(iJobID);
        if(ERROR_DB_NULL != iDBInfo )
        {
            string sDBInfo = boost::lexical_cast<string>(iDBInfo);
            string sDescription = string("Fail to erase Filming job from DB, of which id is : ") 
                + boost::lexical_cast<string>(iJobID); 
            LOG_ERROR_DB(sDescription + "DBInfo: " + sDBInfo);
            return -1;
        }
    }
    catch (const std::exception& e)
    {
        LOG_FUNC_EXCEPTION_DB(e.what());
        return -1;
    }
    catch (...)
    {
        LOG_FUNC_EXCEPTION_DB("unknown error occur!");
        return -1;
    }

    LOG_FUNC_END_DB
    return 0;
}

//int McsfFilmingDB::GetFilmCountYesterday()
//{
//    try
//    {
//        LOG_FUNC_BEGIN_DB 
//
//        const boost::posix_time::ptime ptimeLocalTimeInstance= boost::posix_time::microsec_clock::local_time();
//        const boost::posix_time::ptime yester_ptime = ptimeLocalTimeInstance - boost::gregorian::days(1);
//        const boost::gregorian::date dtDateInstance =yester_ptime.date();
//
//        IDatabaseFactory::IDBPointer pDBPointer;
//        IDatabasePtr pDatabase;
//        ConnectToDB(pDBPointer, pDatabase);
//
//        //todo: day to string 
//        const std::string sConditionSql = "FilmingDate='" + boost::gregorian::to_iso_extended_string(dtDateInstance)+"'";
//        LOG_INFO_DB(sConditionSql);
//
//        std::vector<IFilmingJobPtr> FilmingJobPtrArray;
//        int iDBInfo = pDatabase->GetFilmingJobListBySQL(sConditionSql, FilmingJobPtrArray);
//        if(ERROR_DB_NULL != iDBInfo )
//        {
//            string sDBInfo = boost::lexical_cast<string>(iDBInfo);
//            LOG_ERROR_DB(string("Fail to get Filming job from DB, condition sql is: ") + sConditionSql + " DBInfo is " + sDBInfo);
//            return 0;
//        }
//
//        int filmCount = 0;
//        for (auto it=FilmingJobPtrArray.begin(); it != FilmingJobPtrArray.end (); ++it)
//        {
//            filmCount += (*it)->GetFilmAmount();
//        }
//
//        LOG_FUNC_END_DB
//        return filmCount;
//    }
//    catch (const std::exception& e)
//    {
//        LOG_FUNC_EXCEPTION_DB(e.what());
//        return 0;
//    }
//    catch (...)
//    {
//        LOG_FUNC_EXCEPTION_DB("general exception");
//        return 0;
//    } 
//}

/// \fn int McsfFilmingDB::UpdatePrintJobStatusInDB( const McsfPrintJobObject& printJobObject )
/// <key> \n
/// PRA:yes \n
/// Traced from:SSFS_PRA_Filming_PrintFlag \n
/// Description:update print job status in DB \n
/// Short Description:UpdatePrintJobStatusInDB \n
/// Component:Filming \n
/// </key> \n
///
int McsfFilmingDB::UpdatePrintJobStatusInDB( const McsfPrintJobObject& printJobObject )
{
    try
    {
        LOG_FUNC_BEGIN_DB 
            
        IDatabaseFactory::IDBPointer pDBPointer;
        IDatabasePtr pDatabase;
        ConnectToDB(pDBPointer, pDatabase);

        const std::string sSQLConditon = "JobID=" + boost::lexical_cast<std::string>(printJobObject.GetJobID()); 
        const std::string sSQLStatement = 
            "JobStatus=" + boost::lexical_cast<string>(printJobObject.GetJobStatus())
            +   ", FilmAmount=" + boost::lexical_cast<string>(printJobObject.GetFilmAmount());
        int iDBInfo = pDatabase->UpdateFilmingJobListBySQL(sSQLConditon, sSQLStatement);

        string sDBInfo = boost::lexical_cast<string>(iDBInfo);
        string sDescription = string("Fail to update Filming job from DB, of which job id is : ") 
	              + boost::lexical_cast<string>(printJobObject.GetJobID()); 
        LOG_WARN_DB(sDescription + " DBInfo: " + sDBInfo);

		LOG_FUNC_END_DB
    }
    catch (const std::exception& e)
    {
        LOG_FUNC_EXCEPTION_DB(e.what());
        return -1;
    }
    catch (...)
    {
        LOG_FUNC_EXCEPTION_DB("general exception");
        return -1;
    }

    LOG_FUNC_END_DB
        return 0;   
}

void McsfFilmingDB::ConnectToDB( IDatabaseFactory::IDBPointer &pDBPointer, IDatabasePtr &pDatabase )
{
    pDBPointer = IDatabaseFactory::Instance();
    pDatabase = pDBPointer->CreateDBWrapper();

    if(! pDatabase->Initialize())//Initialize
    {
        LOG_ERROR_DB("Fail to Connect to DB!");
        throw std::exception("Fail to Connect to DB!");
    }
}


//bool McsfFilmingDB::IsJobRecordCanBeRemoved( unsigned int iJobID )
//{
//    LOG_FUNC_BEGIN_DB()
//    try
//    {
//        if ( !IsDBAvailable())  return false;
//        odb::core::transaction trans (m_pMySqlDB->begin ());
//
//        typedef odb::query<FilmingJobTable> queryFilmingJob;
//        typedef odb::result<FilmingJobTable> resultFilmingJob;
//        resultFilmingJob rFilmingJob (m_pMySqlDB->query<FilmingJobTable> (
//            queryFilmingJob::JobID ==  iJobID && queryFilmingJob::JobStatus == ));
//        if (0 == rFilmingJob.size())
//        {
//            std::string sError ("there is no Job ID:");
//            sError += boost::lexical_cast<std::string>(iJobID);
//            LOG_FUNC_EXCEPTION_DB(sError);
//            return false;
//        }
//
//        m_pMySqlDB->erase<FilmingJobTable> (iJobID);
//        trans.commit();
//
//        std::string sDescription = 
//            "delete a filming Job by job ID:" + boost::lexical_cast<std::string>(iJobID);
//        LOG_INFO_DB(sDescription);
//    }
//    catch (const odb::exception& e)
//    {
//        printf("%s \n", e.what());
//        std::string sDescription("error occur: ");
//        sDescription += e.what();
//        LOG_FUNC_EXCEPTION_DB(sDescription);
//        return -1;
//    }
//    catch (...)
//    {
//        LOG_FUNC_EXCEPTION_DB("unknown error occur!");
//        return  -1;
//    }
//
//    LOG_FUNC_END_DB()
//        return 0;
//    return false;
//}

MCSF_FILMING_END_NAMESPACE
