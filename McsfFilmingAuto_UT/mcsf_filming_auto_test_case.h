//////////////////////////////////////////////////////////////////////////
///  Copyright (c) Shanghai United Imaging Healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Wang Hui   mailto:hui.wang@united-imaging.com
///
///  \file mcsf_filming_auto_test_case.h
/// 
///  \brief testing class for auto filming interface
///
///  \version 1.0
///  \date    DEC/26/2011
//////////////////////////////////////////////////////////////////////////

//Gtest Warning
//lint  --e{1511}   2   (Warning -- Member hides non-virtual member 'testing::Test::SetUpTestCase(void)' (line 359, file D:\MCSF\External\gtest\include\gtest\gtest.h) -- Effective C++ #37 & Eff. C++ 3rd Ed. item 33& Eff. C++ 3rd Ed. item 36)
//lint  --e{526}   for ERASE_LOG_INSTANCE
//lint  --e{628}   for ERASE_LOG_INSTANCE
//lint  --e{1055}   for ERASE_LOG_INSTANCE
#ifndef MCSF_FILMING_AUTO_TEST_CASE_H_
#define MCSF_FILMING_AUTO_TEST_CASE_H_

#include <string>
#include <iostream>

#include "gtest/gtest.h"

#include "mcsf_filming_auto_job_creater.h"

using namespace Mcsf;
using namespace std;
/// \class FilmingAutoTest
/// \brief Test suite class For testing DICOM auto Filming
///
class FilmingAutoTest : public testing::Test, public McsfFilmingAutoJobCreater
{
public:
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
    /// This static function erases the log client object
    static void TearDownTestCase()
    {
        testing::Test::TearDownTestCase();
        //		LOG_INFO("------------Test Fixture FilmingTest TearDown");
        
        //const bool bResult = /*lint -e(746)*/ERASE_LOG_INSTANCE(MCSF_FILMING_LOGGER_NAME);

        //if (!bResult)
        //{
        //    cout << "There is something wrong when Erasing Filming Log Instance" << endl;
        //}
    }
protected:
private:
};


#endif//MCSF_FILMING_AUTO_TEST_CASE_H_
