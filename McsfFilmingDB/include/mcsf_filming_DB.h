//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_filming_DB.h
///  \brief   wrapper ODB API 
///
///  \version 1.0
///  \date    Oct. 26, 2011
///  
//////////////////////////////////////////////////////////////////////////

#ifndef MCSF_FILMING_DB_H_
#define MCSF_FILMING_DB_H_

#include <vector>

//configure file parser
#include "McsfFileParser/mcsf_file_parser_exports.h"
#include "McsfFileParser/mcsf_file_parser_factory.h"

//db wrapper
#include "McsfDatabase/mcsf_database_factory.h"
#include "McsfDatabase/mcsf_database_interface_object.h"

#include "mcsf_filming_DB_config.h"
#include "mcsf_print_job_object.h"

//#include "mcsf_filming_config.h"

MCSF_FILMING_BEGIN_NAMESPACE
class Mcsf_Filming_Export McsfFilmingDB
{
public:
    static McsfFilmingDB* GetInstance();

    /////////////////////////////////////////////////////////////////
    ///  \brief         Get all print Job in Job ID level.
    ///  \param[in]     none
    ///                 
    ///  \param[out]    std::vector<McsfPrintJobObject>*
    ///                 the PrintJobObject vector.
    ///  \return        int 
    ///                 if success,return 0
    ///                 if failed, return -1
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int GetAllPrintJob(/*out*/std::vector<McsfPrintJobObject>* pFilmingJobVector);

    /////////////////////////////////////////////////////////////////
    ///  \brief         Get all film sheet level job by a Job ID
    ///  \param[in]     unsigned int iJobID
    ///                 a job ID in DB
    ///                 
    ///  \param[out]    std::vector<McsfPrintJobObject>*
    ///                 the film sheet level PrintJobObject vector.
    ///  \return        int 
    ///                 if success,return 0
    ///                 if failed, return -1
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int GetPrintFilmJobByID(unsigned int iJobID, 
        /*out*/std::vector<McsfPrintJobObject>* pFilmingJobVector);

    /////////////////////////////////////////////////////////////////
    ///  \brief         insert print job info into DB table PrintJobTable
    ///  \param[in]     McsfPrintJobObject*    the print job object
    ///                 
    ///  \param[out]    None
    ///  \return        unsigned int 
    ///                 this job PK: JobID
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    unsigned int InsertPrintJobToDB(McsfPrintJobObject* pPrintJob);

    /////////////////////////////////////////////////////////////////
    ///  \brief         insert film box level info into DB table PrintFileTable
    ///  \param[in]     unsigned int iJobID    the FK of this record
    ///                 McsfPrintJobObject*    the print job object
    ///                 
    ///  \param[out]    None
    ///  \return        unsigned int 
    ///                 the film box level PK: FilmID.
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    unsigned int InsertPrintFilmToDB(unsigned int iJobID, 
        McsfPrintJobObject* pPrintJob); 
    
    /// \brief insert print images path to DB bind to a FilmID,
    /// if failed , return -1;
    int InsertPrintImagesToDB(unsigned int iFilmID, 
        const std::vector<std::string>& vPrintImagesVector);

    /////////////////////////////////////////////////////////////////
    ///  \brief         insert film box level info into DB table PrintFileTable
    ///  \param[in]     const unsigned int    the job ID
    ///                 const std::string&    the stored print object file path
    ///                 
    ///  \param[out]    None
    ///  \return        int 
    ///                 this record's FK: SPfileID.
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    //int InsertSPFilePathToDB(const unsigned int iJobID, 
    //    const std::string& sSPfilePath);

    //int InsertHGFilePathToDB(const int iSPfileID, 
    //    const std::string& sHGfilePath);

    //int GetSPFilePathFromDB(/*in*/const unsigned int iJobID,  /*out*/std::string* pSPfilePath);

    //int GetHGFilePathListFromDB(/*in*/const int iSPfileID,
    //    /*out*/std::vector<std::string>* pHGfilePathList);

    /////////////////////////////////////////////////////////////////
    ///  \brief         get the history print job list by identify conditions.
    ///  \param[in]     const std::string&    the where condition sql.
    ///                 
    ///  \param[out]    std::vector<McsfPrintJobObject>*
    ///                 the McsfPrintJobObject vector
    ///  \return        int 
    ///                 if success,this record's counts;
    ///                 if failed ,return -1;
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int GetHistoryPrintJob(const std::string& sConditionSql, 
        /*out*/std::vector<McsfPrintJobObject>* pFilmingJobVector);

    /////////////////////////////////////////////////////////////////
    ///  \brief         delete a job record by assign job ID.
    ///  \param[in]     unsigned int iJobID
    ///                 a job ID in DB
    ///                 
    ///  \param[out]    none
    ///  \return        int 
    ///                 if success,return 0
    ///                 if failed, return -1
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int DeleteJobByID(unsigned int iJobID);

    /////////////////////////////////////////////////////////////////
    ///  \brief         update print job in DB table PrintJobTable
    ///  \param[in]     McsfPrintJobObject&    the print job object
    ///                 
    ///  \param[out]    None
    ///  \return        int 
    ///                 return 0    indicating success
    ///                 return -1   indicating that there is something wrong
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    int UpdatePrintJobStatusInDB(const McsfPrintJobObject& printJobObject);

    /////////////////////////////////////////////////////////////////
    ///  \brief         Get Film amount since yesterday
    ///  \param[in]     None
    ///                 
    ///  \param[out]    None
    ///  \return        int 
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    //int GetFilmCountYesterday();

    /////////////////////////////////////////////////////////////////
    ///  \brief         check if a job record can be removed.
    ///  \param[in]     unsigned int iJobID
    ///                 a job ID in DB
    ///                 
    ///  \param[out]    none
    ///  \return        bool  
    ///                 if the job record can not be removed,return false
    ///                 else, return true
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    //bool IsJobRecordCanBeRemoved(unsigned int iJobID);

protected:

    McsfFilmingDB();

    void ConnectToDB( IDatabaseFactory::IDBPointer &pDBPointer, IDatabasePtr &pDatabase );

    ~McsfFilmingDB(void);

    //unsigned int GetNewJobID();

    //unsigned int GetNewFilmID();

    //int GetNewSPFileRecordID();

    //int GetNewHGFileRecordID();

    //int GetNewPrintImageRecordID();

    //bool IsDBAvailable();

private:
    std::string m_sUser;
    std::string m_sPassword;
    std::string m_sDatabaseName;
    std::string m_sHostName;
    unsigned int m_uiPort;
    unsigned int m_uiConnectionPoolFactory;
};

MCSF_FILMING_END_NAMESPACE

#endif      //MCSF_FILMING_DB_H_
