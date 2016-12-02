//////////////////////////////////////////////////////////////////////////
///  Copyright (c) Shanghai United Imaging Healthcare Inc., 2011
///  All rights reserved.
///
///  \author  MU Pengxuan   mailto:pengxuan.mu@united-imaging.com
///
///  \file mcsf_filming_test_case.h
/// 
///  \brief testing class for DICOM filming
///
///  \version 1.0
///  \date    Nov/8/2011
//////////////////////////////////////////////////////////////////////////

//Gtest Warning
//lint  --e{1511}   2   (Warning -- Member hides non-virtual member 'testing::Test::SetUpTestCase(void)' (line 359, file D:\MCSF\External\gtest\include\gtest\gtest.h) -- Effective C++ #37 & Eff. C++ 3rd Ed. item 33& Eff. C++ 3rd Ed. item 36)

#ifndef MCSF_FILMING_CONTAINEE_TEST_CASE_H_
#define MCSF_FILMING_CONTAINEE_TEST_CASE_H_

#include <string>
#include <iostream>
using namespace std;

#include "gtest/gtest.h"

#include "mcsf_filming_DB.h"
using namespace Mcsf;

/// \class FilmingTest
/// \brief Test suite class For testing DICOM Filming
///
class FilmingDBTest : public testing::Test
{
public:
    ///\brief	initial test environment for the whole test suite
    ///
    /// This static function can be used to initial test environment for the whole test suite
    static void SetUpTestCase()
    {
        testing::Test::SetUpTestCase();
    }
    ///\brief	clear test environment for the whole test suite
    ///
    /// This static function erases the logclient object
    static void TearDownTestCase()
    {
        testing::Test::TearDownTestCase();
//		LOG_INFO("------------Test Fixture FilmingTest TearDown");
        /*const bool bResult = ERASE_LOG_INSTANCE(MCSF_FILMING_DB_LOGGER_NAME); 
        if (!bResult)
        {
            cout << "There is something wrong when Erasing Filming Log Instance" << endl;
        }*/
    }
protected:
private:
};


#endif//MCSF_FILMING_CONTAINEE_TEST_CASE_H_
