//////////////////////////////////////////////////////////////////////////
///  Copyright (c) Shanghai United Imaging Healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Wang Hui   mailto:hui.wang@united-imaging.com
///
///  \file mcsf_filming_test_case.h
/// 
///  \brief testing class for DICOM filming
///
///  \version 1.0
///  \date    OCT/26/2011
//////////////////////////////////////////////////////////////////////////

//Gtest Warning
//lint  --e{1511}   2   (Warning -- Member hides non-virtual member 'testing::Test::SetUpTestCase(void)' (line 359, file D:\MCSF\External\gtest\include\gtest\gtest.h) -- Effective C++ #37 & Eff. C++ 3rd Ed. item 33& Eff. C++ 3rd Ed. item 36)

#ifndef MCSF_FILMING_TEST_CASE_H_
#define MCSF_FILMING_TEST_CASE_H_

#include <string>
#include <iostream>

#include "gtest/gtest.h"

#include "mcsf_filming_libary_interface.h"
#include "mcsf_filming_libary_factory.h"

#include "mcsf_filming_config.h"

using namespace Mcsf;
using namespace std;

//#define USE_LOCAL_FILE_TO_TEST

#ifdef USE_LOCAL_FILE_TO_TEST
const string gsTestFileReadOnlyDir = "c://tmp/readonly/";
const string gsTestFileSwapDir = "c://tmp/swap/";
#else//USE_LOCAL_FILE_TO_TEST
const string gsTestFileReadOnlyDir = "//CM-FILESERVER01/ShareData/UTdata/filming/readonly/";
const string gsTestFileSwapDir = "  //CM-FILESERVER01/Swap/UT_Output_Data/filming/";
#endif//USE_LOCAL_FILE_TO_TEST

/// \class FilmingTest
/// \brief Test suite class For testing DICOM Filming
///
class FilmingTest : public testing::Test
{
public:

    FilmingTest()
    {

    }

    virtual ~FilmingTest()
    {

    }

    virtual void SetUp()
    {
    //    m_pPrintJob = new McsfPrintJobObject();
    //    m_pFilmingDB = McsfFilmingDB::GetInstance();
    //    McsfFilmingLibaryFactory* pFilming = McsfFilmingLibaryFactory::Instance();
    //    m_pIfilmingLibary = pFilming->CreateFilmingLibary(m_pPrintJob,m_pFilmingDB);
    //}

    //virtual void TearDown()
    //{
    //    if (NULL != m_pIfilmingLibary)
    //    {
    //        delete m_pIfilmingLibary;
    //    }
    //    if (NULL != m_pPrintJob)
    //    {
    //        delete m_pPrintJob;
    //    }
    }

    ///\brief	initial test environment for the whole test suite
    ///
    /// This static function can be used to initial test environment for the whole test suite
    static void SetUpTestCase()
    {
        testing::Test::SetUpTestCase();
//		LOG_INFO("------------Test Fixture FilmingTest Setup");
    }
    ///\brief	clear test environment for the whole test suite
    ///
    /// This static function erases the logclient object
    static void TearDownTestCase()
    {
        testing::Test::TearDownTestCase();
//		LOG_INFO("------------Test Fixture FilmingTest TearDown");
        /*const bool bResult = ERASE_LOG_INSTANCE(MCSF_FILMING_LOGGER_NAME); 
        if (!bResult)
        {
            cout << "There is something wrong when Erasing Filming Log Instance" << endl;
        }*/
    }

    //IFilmingLibary* m_pIfilmingLibary;

protected:
private:
    //McsfPrintJobObject* m_pPrintJob;
    //McsfFilmingDB* m_pFilmingDB;
};


#endif//MCSF_FILMING_TEST_CASE_H_
