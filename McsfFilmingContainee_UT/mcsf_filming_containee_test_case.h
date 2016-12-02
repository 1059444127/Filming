//////////////////////////////////////////////////////////////////////////
///  Copyright (c) Shanghai United Imaging Healthcare Inc., 2011
///  All rights reserved.
///
///  \author  MU Pengxuan   mailto:pengxuan.mu@united-imaging.com
///
///  \file mcsf_filming_containee_test_case.h
/// 
///  \brief testing class for DICOM filming containee
///
///  \version 1.0
///  \date    NOV/8/2011
//////////////////////////////////////////////////////////////////////////

//Gtest Warning
//lint  --e{1511}   2   (Warning -- Member hides non-virtual member 'testing::Test::SetUpTestCase(void)' (line 359, file D:\MCSF\External\gtest\include\gtest\gtest.h) -- Effective C++ #37 & Eff. C++ 3rd Ed. item 33& Eff. C++ 3rd Ed. item 36)

#ifndef MCSF_FILMING_CONTAINEE_TEST_CASE_H_
#define MCSF_FILMING_CONTAINEE_TEST_CASE_H_

#include <string>
#include <iostream>
#include <limits>

#include "McsfMedViewer/mcsf_med_viewer_controller_interface.h"
#include "McsfMedViewer/mcsf_med_viewer_controller_factory.h"

#include "gtest/gtest.h"

#include "mcsf_filming_command_handler.h"

using namespace Mcsf;
using namespace std;
//lint --e{1524, 429}
static const unsigned int kuiJobIDForTest = UINT_MAX-1;
/// \class FilmingTest
/// \brief Test suite class For testing DICOM Filming
///
class FilmingContaineeTest : public testing::Test//, public McsfFilmingCommandHandler
{
public:
    FilmingContaineeTest() //: McsfFilmingCommandHandler(new CommunicationProxy())
    {

    }

    virtual ~FilmingContaineeTest()
    {

    }

    ///\brief	initial test environment for the whole test suite
    ///
    /// This static function can be used to initial test environment for the whole test suite
    static void SetUpTestCase()
    {
        testing::Test::SetUpTestCase();
        //LOG_INFO("------------Test Fixture FilmingTest Setup");

        ////get system config file path
        //Mcsf::ISystemEnvironmentConfig* pSystemEnviromentConfig = 
        //    Mcsf::ConfigSystemEnvironmentFactory::Instance()->GetSystemEnvironment();

        //std::string sConfigFilePath = 
        //    pSystemEnviromentConfig->GetApplicationPath(
        //    MCSF_FILMING_CONFIG_PATH_TAG_IN_SYSTEM_ENVIRONMENT);
        //wstring wsFilePath; 

        ////lint -e534
        //wsFilePath.assign(sConfigFilePath.begin(), sConfigFilePath.end());
        ////lint +e534
        //IMedViewerController* pMedViewerController = MedViewerControllerFactory::Instance()->GetController(0, wsFilePath);

        //m_pImageCmdHandler = new McsfFilmingImageCommandHandler(new CommunicationProxy(), NULL);

    }
    ///\brief	clear test environment for the whole test suite
    ///
    /// This static function erases the logclient object
    static void TearDownTestCase()
    {
        testing::Test::TearDownTestCase();
		//LOG_INFO("++++++++++++Test Fixture FilmingTest TearDown");

        //gFilmingServiceLogger.EraseLogger();

        //bool bResult = ERASE_LOG_INSTANCE(MCSF_FILMING_SEVICE_LOGGER_NAME);
        //if (!bResult)
        //{
        //    cout << "There is something wrong when Erasing Filming Containee Log Instance" << endl;
        //}
        //bResult = ERASE_LOG_INSTANCE(MCSF_FILMING_BE_LOGGER_NAME); 
        //if (!bResult)
        //{
        //    cout << "There is something wrong when Erasing Filming Log Instance" << endl;
        //}
        //bResult = ERASE_LOG_INSTANCE(MCSF_FILMING_DB_LOGGER_NAME);
        //if (!bResult)
        //{
        //    cout << "There is something wrong when Erasing Filming DB Log Instance" << endl;
        //}

        //if (NULL != m_pImageCmdHandler)
        //{
        //    delete m_pImageCmdHandler;
        //}
    }

    //virtual void SetUp()
    //{
    //    LOG_INFO("-------Test case Setup");

    //    ostringstream os;
    //    os << kuiJobIDForTest;
    //    std::string sTemp = os.str();
    //    ACE_Message_Block *pTempMB = new ACE_Message_Block(sTemp.length()); 
    //    if (NULL == pTempMB) 
    //    {
    //        LOG_CONTAINEE_ERROR("create a new ACE_Message_Block failed!");
    //        return; 
    //    }

    //    //Insert data into the message block using the wr_ptr
    //    memcpy(pTempMB->wr_ptr(),sTemp.c_str(),sTemp.length());

    //    //Be careful to advance the wr_ptr by the necessary amount.
    //    //Note that the argument is of type "size_t" that is mapped to
    //    //bytes.
    //    pTempMB->wr_ptr(sTemp.length());
    //    pTempMB->msg_priority( static_cast<unsigned long>( MEDIUM ) );

    //    if(-1 == m_pMessageQueue->enqueue_prio(pTempMB))
    //    {
    //        LOG_CONTAINEE_ERROR("\nCould not enqueue on to mq!!\n");
    //        return ;
    //    }
    //}

    //virtual void TearDown()
    //{
    //    LOG_INFO("+++++++Test case TearDown");

    //    ACE_Message_Block* pTempMB = NULL;
    //    m_pMessageQueue->dequeue_prio(pTempMB);
    //    if (NULL == pTempMB || pTempMB->release())
    //    {
    //        LOG_CONTAINEE_WARN("something wrong when release ACE_MESSAGE_BLOCK in DoRealPrint");
    //    }
    //}

public:
    //static McsfFilmingImageCommandHandler* m_pImageCmdHandler;
    //McsfFilmingContaineeTest m_filmingContaineeTest;

    /// \brief Test McsfFilmingCommandHandler::SplitBorderDensity
    //bool TestFuncSplitBorderDensity();

    ///// \brief Test McsfFilmingCommandHandler::SplitDisplayFormat
    //bool TestFuncSplitDisplayFormat();

    ///// \brief Test McsfFilmingCommandHandler::SplitEmptyImageDensity
    //bool TestFuncSplitEmptyImageDensity();

    ///// \brief Test McsfFilmingCommandHandler::SplitFilmDestination
    //bool TestFuncSplitFilmDestination();

    ///// \brief Test McsfFilmingCommandHandler::SplitFilmSize
    //bool TestFuncSplitFilmSize();

    ///// \brief Test McsfFilmingCommandHandler::SplitMagnificationType
    //bool TestFuncSplitMagnificationType();

    ///// \brief Test McsfFilmingCommandHandler::cSplitMediumType
    //bool TestFuncSplitMediumType();

    //void AddPrintJob(CommandContext& context);
    //bool TestFuncSplitWrongFormatBorderDensity();
    //bool TestFuncSplitWrongFormatDisplayFormat();
    //bool TestFuncSplitWrongFormatEmptyImageDensity();
    //bool TestFuncSplitWrongFormatFilmDestination();
    //bool TestFuncSplitWrongFormatFilmSize();
    //bool TestFuncSplitWrongFormatMagnificationType();
    //bool TestFuncSplitWrongFormatMediumType();

protected:
private:
};


#endif//MCSF_FILMING_CONTAINEE_TEST_CASE_H_
