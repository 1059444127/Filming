//////////////////////////////////////////////////////////////////////////
///  Copyright (c) Shanghai United Imaging Healthcare Inc., 2011
///  All rights reserved.
///
///  \author  MU Pengxuan   mailto:pengxuan.mu@united-imaging.com
///  
///  \modified by   Wang Hui    mailto:hui.wang@united-imaging.com
///  \file mcsf_filming_workflow_test_case.cpp
/// 
///  \brief testing Case for DICOM Filming
///
///  \version 1.0
///  \date    Oct/27/2011
//////////////////////////////////////////////////////////////////////////
//Gtest Info
//lint  --e{1704, 1722, 1928, 764, 744, 1961, 1715, 1788}   TEST MACRO

//lint	--e{438}	4	(line 54)	(Warning -- Last value assigned to variable 'FilmingTest_Printer_Configure_File_Is_All_Right_Test::test_info_' (defined at line 54, file mcsf_filming_test_case.cpp) not used)
//lint --e{529}

#include "mcsf_dcm_printer_config.h"
#include "mcsf_print_job_object.h"

#include "mcsf_filming_test_case.h"

TEST_F(FilmingTest, Filming_Config_Parse)
{
    
}

TEST_F(FilmingTest, Interface_Filming_Libary)
{
    McsfPrintJobObject* pPrintJob = new McsfPrintJobObject();
    EXPECT_TRUE(NULL != pPrintJob);

    McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
    EXPECT_TRUE(NULL != pFilmingDB);

    McsfFilmingLibaryFactory* pFilming = McsfFilmingLibaryFactory::Instance();

    IFilmingLibary* pIfilmingLibary = pFilming->CreateFilmingLibary(pPrintJob,pFilmingDB);
    EXPECT_TRUE(NULL != pIfilmingLibary);

    if (nullptr != pPrintJob)
    {
        delete pPrintJob;
    }
    
    //delete pFilmingDB;
    if (nullptr != pIfilmingLibary)
    {//lint -e1043
    	delete pIfilmingLibary;
    }//lint +e1043
    
}

TEST_F(FilmingTest, Get_Print_Job_From_DB)
{
    McsfPrintJobObject* pPrintJob = new McsfPrintJobObject();
    EXPECT_TRUE(NULL != pPrintJob);

    //McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
    //EXPECT_TRUE(NULL != pFilmingDB);

    //McsfFilmingLibaryFactory* pFilming = McsfFilmingLibaryFactory::Instance();

    pPrintJob->SetJobID(0);
    pPrintJob->SetPriority(1);
    pPrintJob->SetJobStatus(0);
    pPrintJob->SetCopies(1);
    pPrintJob->SetMyAETitle("ui");
    pPrintJob->SetTargetAETitle("LEAD_SERVER");
    pPrintJob->SetTargetHostName("10.1.3.194");
    pPrintJob->SetTargetPort(10006);
    pPrintJob->SetLayout("STANDARD\\2,2");
    pPrintJob->SetFilmAmount(1);
    pPrintJob->SetAccessionNo("AccesstionNo");
    pPrintJob->SetDefaultPrintIllumination(2000);
    pPrintJob->SetDefaultPrintReflection(10);
    pPrintJob->SetFilmSize("14INx17IN");
    pPrintJob->SetLutName("AAAA");
    pPrintJob->SetOrientation(0);
    pPrintJob->SetPriority(0);
    std::vector<std::string> imagePathVector;
    std::vector<std::string> originSOPInstanceUIDVector;
    imagePathVector.push_back("//10.1.2.12/Public/images/UT/Filming/test1.dcm");
    originSOPInstanceUIDVector.push_back("");//1.3.46.670589.11.0.0.11.4.2.0.8743.5.3800.2006120117111076001
    pPrintJob->SetFileNameList(imagePathVector);
    pPrintJob->SetOriginSOPInstanceUIDList(originSOPInstanceUIDVector);

    //IFilmingLibary* pIfilmingLibary = pFilming->CreateFilmingLibary(pPrintJob,pFilmingDB);
    //EXPECT_TRUE(NULL != pIfilmingLibary);

    //pIfilmingLibary->CreatePrintObject();

    delete pPrintJob;
    //delete pFilmingDB; //don't need delete!
    //delete pIfilmingLibary;
}

/// \fn TEST_F(FilmingTest, Print_Object_Instance_Do_Print_With_No_Object)
/// <key> \n
/// PRA:Yes \n
/// Tests:DS_PRA_Filming_DoPrint \n
/// Description:test do print function with on print object \n
/// Short Description:DoPrintWithNoObject \n
/// Component:Filming \n
/// </key> \n
///
TEST_F(FilmingTest, Print_Object_Instance_Do_Print_With_No_Object)
{
 //   McsfPrintJobObject* pPrintJob = new McsfPrintJobObject();

 //   McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
 //   McsfFilmingLibaryFactory* pFilming = McsfFilmingLibaryFactory::Instance();
	//IFilmingLibary* pIfilmingLibary = pFilming->CreateFilmingLibary(pPrintJob,pFilmingDB);

	//pIfilmingLibary->CreatePrintObject();

	//const int iRet = pIfilmingLibary->DoPrint();

	//EXPECT_EQ(-1, iRet);

	//if (NULL != pIfilmingLibary)
	//{
	//	delete pIfilmingLibary;
	//}
	//if (NULL != pPrintJob)
	//{
	//	delete pPrintJob;
	//}
}

/// \fn TEST_F(FilmingTest, Print_Object_Instance_Do_Print)
/// <key> \n
/// PRA:Yes \n
/// Tests:DS_PRA_Filming_DoPrint \n
/// Description:Do Print with a Print Object \n
/// Short Description:DoPrint \n
/// Component:Filming \n
/// </key> \n
///
TEST_F(FilmingTest, Print_Object_Instance_Do_Print)
{
//    McsfPrintJobObject* pPrintJob = new McsfPrintJobObject();
//
//    pPrintJob->SetJobID(0);
//    pPrintJob->SetPriority(1);
//    pPrintJob->SetJobStatus(0);
//    pPrintJob->SetCopies(1);
//    pPrintJob->SetMyAETitle("ui");
//    pPrintJob->SetTargetAETitle("LEAD_SERVER");
//    pPrintJob->SetTargetHostName("10.1.3.194");
//    pPrintJob->SetTargetPort(10006);
//    pPrintJob->SetLayout("STANDARD\\2,2");
//    pPrintJob->SetFilmAmount(1);
//    std::vector<std::string> imagePathVector;
//    std::vector<std::string> originSOPInstanceUIDVector;
//    imagePathVector.push_back("//10.1.2.12/Public/images/UT/Filming/test1.dcm");
//    originSOPInstanceUIDVector.push_back("");//1.3.46.670589.11.0.0.11.4.2.0.8743.5.3800.2006120117111076001
//    pPrintJob->SetFileNameList(imagePathVector);
//    pPrintJob->SetOriginSOPInstanceUIDList(originSOPInstanceUIDVector);
//
//
//    McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
//    McsfFilmingLibaryFactory* pFilming = McsfFilmingLibaryFactory::Instance();
//    IFilmingLibary* pIfilmingLibary = pFilming->CreateFilmingLibary(pPrintJob,pFilmingDB);
//
//    pIfilmingLibary->CreatePrintObject();
//
//#ifdef USE_LOCAL_FILE_TO_TEST
//    const int iRet = pIfilmingLibary->DoPrint();
//    EXPECT_EQ(0, iRet) << "Printing file directory" << gsTestFileReadOnlyDir;
//#else
//    pIfilmingLibary->DoPrint();
//#endif
//
//	pIfilmingLibary->DeletePrintObjectFile();
//
//    if (NULL != pIfilmingLibary)
//    {
//        delete pIfilmingLibary;
//    }
//    if (NULL != pPrintJob)
//    {
//        delete pPrintJob;
//    }
}

//TODO: private - called by DoPrint
TEST_F(FilmingTest, Print_Object_Instance_LoadSPfile)
{
    
}

TEST_F(FilmingTest, Print_Object_Instance_ConnectToPrinter)
{
    McsfPrintJobObject printJob;

    printJob.SetJobID(0);
    printJob.SetPriority(1);
    printJob.SetJobStatus(0);
    printJob.SetCopies(1);
    printJob.SetMyAETitle("ui");
    printJob.SetTargetAETitle("LEAD_SERVER");
    printJob.SetTargetHostName("10.1.8.38");
    printJob.SetTargetPort(10006);
    printJob.SetLayout("STANDARD\\2,2");
    printJob.SetFilmAmount(1);
    std::vector<std::string> imagePathVector;
    std::vector<std::string> originSOPInstanceUIDVector;
    imagePathVector.push_back("//10.1.2.12/Public/images/UT/Filming/test1.dcm");
    originSOPInstanceUIDVector.push_back("");//1.3.46.670589.11.0.0.11.4.2.0.8743.5.3800.2006120117111076001
    printJob.SetFileNameList(imagePathVector);
    printJob.SetOriginSOPInstanceUIDList(originSOPInstanceUIDVector);

    McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();

    McsfDcmPrintObjectInstance* pPrintObjectInstance = new McsfDcmPrintObjectInstance(&printJob,pFilmingDB);

    OFCondition cond = pPrintObjectInstance->ConnectToPrinter();

    // the printer IP is wrong, so can't connect
    EXPECT_TRUE(cond.bad());

    SUCCEED();
}

TEST_F(FilmingTest, Print_Object_Instance_CreateSession)
{
    McsfPrintJobObject printJob;

    printJob.SetJobID(0);
    printJob.SetPriority(1);
    printJob.SetJobStatus(0);
    printJob.SetCopies(1);
    printJob.SetMyAETitle("ui");
    printJob.SetTargetAETitle("LEAD_SERVER");
    printJob.SetTargetHostName("10.1.8.38");
    printJob.SetTargetPort(10006);
    printJob.SetLayout("STANDARD\\2,2");
    printJob.SetFilmAmount(1);
    std::vector<std::string> imagePathVector;
    std::vector<std::string> originSOPInstanceUIDVector;
    imagePathVector.push_back("//10.1.2.12/Public/images/UT/Filming/test1.dcm");
    originSOPInstanceUIDVector.push_back("");//1.3.46.670589.11.0.0.11.4.2.0.8743.5.3800.2006120117111076001
    printJob.SetFileNameList(imagePathVector);
    printJob.SetOriginSOPInstanceUIDList(originSOPInstanceUIDVector);

    McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();

    McsfDcmPrintObjectInstance* pPrintObjectInstance = new McsfDcmPrintObjectInstance(&printJob,pFilmingDB);

    DVPSStoredPrint* pDVPSStoredPrint = new DVPSStoredPrint(
        printJob.GetDefaultPrintIllumination(), 
        printJob.GetDefaultPrintReflection(), 
        printJob.GetTargetAETitle().c_str());

    OFCondition cond = pPrintObjectInstance->CreateSession(*pDVPSStoredPrint);

    EXPECT_TRUE(cond.bad());

    SUCCEED();
}

TEST_F(FilmingTest, Print_Object_Instance_SetFilmBox)
{
    McsfPrintJobObject printJob;

    printJob.SetJobID(0);
    printJob.SetPriority(1);
    printJob.SetJobStatus(0);
    printJob.SetCopies(1);
    printJob.SetMyAETitle("ui");
    printJob.SetTargetAETitle("LEAD_SERVER");
    printJob.SetTargetHostName("10.1.8.38");
    printJob.SetTargetPort(10006);
    printJob.SetLayout("STANDARD\\2,2");
    printJob.SetFilmAmount(1);
    std::vector<std::string> imagePathVector;
    std::vector<std::string> originSOPInstanceUIDVector;
    imagePathVector.push_back("//10.1.2.12/Public/images/UT/Filming/test1.dcm");
    originSOPInstanceUIDVector.push_back("");//1.3.46.670589.11.0.0.11.4.2.0.8743.5.3800.2006120117111076001
    printJob.SetFileNameList(imagePathVector);
    printJob.SetOriginSOPInstanceUIDList(originSOPInstanceUIDVector);

    McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();

    McsfDcmPrintObjectInstance* pPrintObjectInstance = new McsfDcmPrintObjectInstance(&printJob,pFilmingDB);

    DVPSStoredPrint* pDVPSStoredPrint = new DVPSStoredPrint(
        printJob.GetDefaultPrintIllumination(), 
        printJob.GetDefaultPrintReflection(), 
        printJob.GetTargetAETitle().c_str());

    OFCondition cond = pPrintObjectInstance->SetFilmBox(*pDVPSStoredPrint);

    //EXPECT_TRUE(cond.good());

    SUCCEED();
}

//TODO: private - called by DoPrint
TEST_F(FilmingTest, Print_Object_Instance_SetAnnotationBox)
{

}

TEST_F(FilmingTest, Print_Object_Instance_PrintFilmSession)
{
    McsfPrintJobObject printJob;

    printJob.SetJobID(0);
    printJob.SetPriority(1);
    printJob.SetJobStatus(0);
    printJob.SetCopies(1);
    printJob.SetMyAETitle("ui");
    printJob.SetTargetAETitle("LEAD_SERVER");
    printJob.SetTargetHostName("10.1.8.38");
    printJob.SetTargetPort(10006);
    printJob.SetLayout("STANDARD\\2,2");
    printJob.SetFilmAmount(1);
    std::vector<std::string> imagePathVector;
    std::vector<std::string> originSOPInstanceUIDVector;
    imagePathVector.push_back("//10.1.2.12/Public/images/UT/Filming/test1.dcm");
    originSOPInstanceUIDVector.push_back("");
    printJob.SetFileNameList(imagePathVector);
    printJob.SetOriginSOPInstanceUIDList(originSOPInstanceUIDVector);

    McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();

    McsfDcmPrintObjectInstance* pPrintObjectInstance = new McsfDcmPrintObjectInstance(&printJob,pFilmingDB);

    DVPSStoredPrint* pDVPSStoredPrint = new DVPSStoredPrint(
        printJob.GetDefaultPrintIllumination(), 
        printJob.GetDefaultPrintReflection(), 
        printJob.GetTargetAETitle().c_str());

    OFCondition cond = pPrintObjectInstance->PrintFilmSession(*pDVPSStoredPrint);

    EXPECT_TRUE(cond.bad());

    SUCCEED();
}

TEST_F(FilmingTest, Print_Object_Instance_DeleteFilmSession)
{
    McsfPrintJobObject printJob;

    printJob.SetJobID(0);
    printJob.SetPriority(1);
    printJob.SetJobStatus(0);
    printJob.SetCopies(1);
    printJob.SetMyAETitle("ui");
    printJob.SetTargetAETitle("LEAD_SERVER");
    printJob.SetTargetHostName("10.1.8.38");
    printJob.SetTargetPort(10006);
    printJob.SetLayout("STANDARD\\2,2");
    printJob.SetFilmAmount(1);
    std::vector<std::string> imagePathVector;
    std::vector<std::string> originSOPInstanceUIDVector;
    imagePathVector.push_back("//10.1.2.12/Public/images/UT/Filming/test1.dcm");
    originSOPInstanceUIDVector.push_back("");//1.3.46.670589.11.0.0.11.4.2.0.8743.5.3800.2006120117111076001
    printJob.SetFileNameList(imagePathVector);
    printJob.SetOriginSOPInstanceUIDList(originSOPInstanceUIDVector);

    McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();

    McsfDcmPrintObjectInstance* pPrintObjectInstance = new McsfDcmPrintObjectInstance(&printJob,pFilmingDB);

    DVPSStoredPrint* pDVPSStoredPrint = new DVPSStoredPrint(
        printJob.GetDefaultPrintIllumination(), 
        printJob.GetDefaultPrintReflection(), 
        printJob.GetTargetAETitle().c_str());

    OFCondition cond = pPrintObjectInstance->DeleteFilmSession(*pDVPSStoredPrint);

    EXPECT_TRUE(cond.good());

    SUCCEED();
}

TEST_F(FilmingTest, Print_Object_Instance_ReleaseAssociation)
{
    McsfPrintJobObject printJob;

    printJob.SetJobID(0);
    printJob.SetPriority(1);
    printJob.SetJobStatus(0);
    printJob.SetCopies(1);
    printJob.SetMyAETitle("ui");
    printJob.SetTargetAETitle("LEAD_SERVER");
    printJob.SetTargetHostName("10.1.8.38");
    printJob.SetTargetPort(10006);
    printJob.SetLayout("STANDARD\\2,2");
    printJob.SetFilmAmount(1);
    std::vector<std::string> imagePathVector;
    std::vector<std::string> originSOPInstanceUIDVector;
    imagePathVector.push_back("//10.1.2.12/Public/images/UT/Filming/test1.dcm");
    originSOPInstanceUIDVector.push_back("");//1.3.46.670589.11.0.0.11.4.2.0.8743.5.3800.2006120117111076001
    printJob.SetFileNameList(imagePathVector);
    printJob.SetOriginSOPInstanceUIDList(originSOPInstanceUIDVector);

    McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();

    McsfDcmPrintObjectInstance* pPrintObjectInstance = new McsfDcmPrintObjectInstance(&printJob,pFilmingDB);

    OFCondition cond = pPrintObjectInstance->ReleaseAssociation();

    EXPECT_TRUE(cond.good());

    SUCCEED();
}


//////////////////////////////////////////////////////////////////////////
//public function to be test

TEST_F(FilmingTest, Print_Object_Instance_InsertPrintObjectToDB)
{

}

TEST_F(FilmingTest, Print_Object_Instance_LoadPrintFile)
{
    McsfPrintJobObject printJob;

    printJob.SetJobID(0);
    printJob.SetPriority(1);
    printJob.SetJobStatus(0);
    printJob.SetCopies(1);
    printJob.SetMyAETitle("ui");
    printJob.SetTargetAETitle("LEAD_SERVER");
    printJob.SetTargetHostName("10.1.8.38");
    printJob.SetTargetPort(10006);
    printJob.SetLayout("STANDARD\\2,2");
    printJob.SetFilmAmount(1);
    std::vector<std::string> imagePathVector;
    std::vector<std::string> originSOPInstanceUIDVector;
    imagePathVector.push_back("//10.1.2.12/Public/images/UT/Filming/test1.dcm");
    originSOPInstanceUIDVector.push_back("");
    printJob.SetFileNameList(imagePathVector);
    printJob.SetOriginSOPInstanceUIDList(originSOPInstanceUIDVector);

    McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();

    McsfDcmPrintObjectInstance* pPrintObjectInstance = new McsfDcmPrintObjectInstance(&printJob,pFilmingDB);

    OFCondition cond = pPrintObjectInstance->LoadPrintFile("","//10.1.2.12/Public/images/UT/Filming/test1.dcm");

    EXPECT_TRUE(cond.bad());

    SUCCEED();
}

TEST_F(FilmingTest, Print_Object_Instance_LoadPrintFile_WithWrongSP)
{
    std::string testFilePath = "//10.1.2.12/Public/images/UT/Filming/test1.dcm";
    McsfPrintJobObject printJob;

    printJob.SetJobID(0);
    printJob.SetPriority(1);
    printJob.SetJobStatus(0);
    printJob.SetCopies(1);
    printJob.SetMyAETitle("ui");
    printJob.SetTargetAETitle("LEAD_SERVER");
    printJob.SetTargetHostName("10.1.8.38");
    printJob.SetTargetPort(10006);
    printJob.SetLayout("STANDARD\\2,2");
    printJob.SetFilmAmount(1);
    std::vector<std::string> imagePathVector;
    std::vector<std::string> originSOPInstanceUIDVector;
    imagePathVector.push_back(testFilePath);
    originSOPInstanceUIDVector.push_back("");
    printJob.SetFileNameList(imagePathVector);
    printJob.SetOriginSOPInstanceUIDList(originSOPInstanceUIDVector);

    McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();

    McsfDcmPrintObjectInstance* pPrintObjectInstance = new McsfDcmPrintObjectInstance(&printJob,pFilmingDB);

    OFCondition cond = pPrintObjectInstance->LoadPrintFile(testFilePath.c_str(),testFilePath.c_str());

    EXPECT_TRUE(cond.bad());

    SUCCEED();
}

TEST_F(FilmingTest, Print_Object_Instance_SaveHGFile)
{
    McsfPrintJobObject printJob;

    printJob.SetJobID(0);
    printJob.SetPriority(1);
    printJob.SetJobStatus(0);
    printJob.SetCopies(1);
    printJob.SetMyAETitle("ui");
    printJob.SetTargetAETitle("LEAD_SERVER");
    printJob.SetTargetHostName("10.1.8.38");
    printJob.SetTargetPort(10006);
    printJob.SetLayout("STANDARD\\2,2");
    printJob.SetFilmAmount(1);
    std::vector<std::string> imagePathVector;
    std::vector<std::string> originSOPInstanceUIDVector;
    imagePathVector.push_back("//10.1.2.12/Public/images/UT/Filming/test1.dcm");
    originSOPInstanceUIDVector.push_back("");
    printJob.SetFileNameList(imagePathVector);
    printJob.SetOriginSOPInstanceUIDList(originSOPInstanceUIDVector);

    McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();

    McsfDcmPrintObjectInstance* pPrintObjectInstance = new McsfDcmPrintObjectInstance(&printJob,pFilmingDB);

    pPrintObjectInstance->SaveHGfile();

    SUCCEED();
}

//TODO: private
TEST_F(FilmingTest, Print_Object_Instance_SaveSPFile)
{

}

//TODO: private
TEST_F(FilmingTest, Print_Object_Instance_SetAnnotation)
{

}

//TODO: private
TEST_F(FilmingTest, Print_Object_Instance_SetLut)
{

}

