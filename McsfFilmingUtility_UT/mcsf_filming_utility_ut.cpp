//////////////////////////////////////////////////////////////////////////
///  Copyright (c) Shanghai United Imaging Healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Wang Hui   mailto:hui.wang@united-imaging.com
///
///  \file mcsf_filming_utility_ut.cpp
/// 
///  \brief testing fixture entry for common utility used by Filming module
///
///  \version 1.0
///  \date    Apr/10/2012
//////////////////////////////////////////////////////////////////////////

#include "gtest/gtest.h"

//xml output file of UT result 
static const std::string ksTestOutputFile = "xml:mcsf_filming_utility_ut_result.xml";

int main(int argc, char* argv[])
{

    testing::GTEST_FLAG(output)=ksTestOutputFile;
    testing::InitGoogleTest(&argc, argv);  

    (void)RUN_ALL_TESTS();

    //system("pause");

    return 0;
}