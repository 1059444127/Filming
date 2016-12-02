//////////////////////////////////////////////////////////////////////////
///  Copyright (c) Shanghai United Imaging Healthcare Inc., 2011
///  All rights reserved.
///
///  /author  MU Pengxuan   mailto:pengxuan.mu@united-imaging.com
///           WANG Hui      mailto:hui.wang@united-imaging.com
///
///  /file mcsf_filming_test_case.cpp
/// 
///  /brief testing Case for DICOM Filming
///
///  /version 1.0
///  /date    Oct/26/2011
//////////////////////////////////////////////////////////////////////////
//Gtest Info
//lint  --e{1704, 1722, 1928, 764, 744, 1961, 1715}   TEST MACRO

//Gtest Error
//lint	--e{36}		4	(line 32)	(Error -- redefining the storage class of symbol 'FilmingTest_Printer_Configure_File_Is_Null_Test::test_info_' (auto vs. static data), conflicts with line 32)

//Gtest Warning
//lint  --e{666}
//lint  --e{529}
//lint  --e{528}
//lint  --e{665}	5	(line 28)	(Warning -- Unparenthesized parameter 1 in macro 'GTEST_PRED_FORMAT2_' is passed an expression)
//lint	--e{525}	5	(line 30)	(Warning -- Negative indentation from line 28)
//lint	--e{438}	4	(line 54)	(Warning -- Last value assigned to variable 'FilmingTest_Printer_Configure_File_Is_All_Right_Test::test_info_' (defined at line 54, file mcsf_filming_test_case.cpp) not used)

#include <algorithm>
#include "mcsf_filming_db_test_case.h"

using namespace Mcsf;

static /*lint  -e843*/  unsigned int iTestJobID = 0;

//#define DISABLE_UT

#ifndef DISABLE_UTk

//TEST_F(FilmingDBTest, Init_FilmingDB_Log_Config)
//{
//    const string sFilePath = InitFilmingDBLogConfig();
//    EXPECT_NE("", sFilePath);
//}
//
//
//TEST_F(FilmingDBTest, Init_FilmingDB_Instance)
//{//lint --e{1715}  GTEST MACRO
//    Mcsf::McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
//    EXPECT_TRUE(NULL!=pFilmingDB);
//}
//
//TEST_F(FilmingDBTest, Test_InsertPrintJobToDB)
//{
//    Mcsf::McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
//    EXPECT_TRUE(NULL!=pFilmingDB);
//
//    McsfPrintJobObject printObject;
//    printObject.SetJobStatus(0);//pending
//    printObject.SetCopies(1);
//    printObject.SetFilmSize("8INx10IN");
//    printObject.SetJobPriority(2);//medium
//    printObject.SetLayout("STANDARD\\2,2");
//    printObject.SetMyAETitle("UI_Filming");
//    printObject.SetTargetAETitle("Printer");
//    printObject.SetTargetHostName("10.1.3.194");
//    printObject.SetTargetPort(3333);
//    printObject.SetTargetMaxPDU(65536);
//    printObject.SetTargetSupports12bit(true);
//    printObject.SetRefPatientID("123456");
//    printObject.SetRefPatientName("ZhangSan");
//    printObject.SetRefPatientAge("25Y");
//    printObject.SetRefPatientSex("M");
//    printObject.SetSessionPrint("654321");
//    printObject.SetOperatorName("Doctor LiLi");
//
//    std::vector<std::string> sImagePathVector;
//    bool bRet = printObject.SetFileNameList(sImagePathVector);
//    EXPECT_FALSE(bRet);
//
//    sImagePathVector.push_back("//10.1.2.13/Public/DicomData/MR_STU00040/IMG00001");
//    sImagePathVector.push_back("//10.1.2.13/Public/DicomData/MR_STU00040/IMG00002");
//    sImagePathVector.push_back("//10.1.2.13/Public/DicomData/MR_STU00040/IMG00003");
//    sImagePathVector.push_back("//10.1.2.13/Public/DicomData/MR_STU00040/IMG00004");
//    bRet = printObject.SetFileNameList(sImagePathVector);
//    EXPECT_TRUE(bRet);
//
//    unsigned int uiJobID = pFilmingDB->InsertPrintJobToDB(&printObject);
//
//    iTestJobID = uiJobID;
//    
//    //EXPECT_GE(uiJobID,(unsigned int)1);
//
//    
//}
//
//TEST_F(FilmingDBTest, InsertPrintImagesToDB_Boundary_Condition_Test)
//{
//    Mcsf::McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
//    EXPECT_TRUE(NULL!=pFilmingDB);
//
//    std::vector<std::string> sVector;
//    int iRet = pFilmingDB->InsertPrintImagesToDB(1, sVector);
//    EXPECT_EQ(-1, iRet);
//
//    sVector.push_back(EMPTY_STRING);
//    iRet = pFilmingDB->InsertPrintImagesToDB(0, sVector);
//    EXPECT_EQ(-1, iRet);
//
//    iRet = pFilmingDB->InsertPrintImagesToDB(2, sVector);
//    EXPECT_EQ(0, iRet);
//
//}
//
//
//TEST_F(FilmingDBTest, InsertPrintFilmToDB_Boundary_Condition_Test)
//{
//    Mcsf::McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
//    EXPECT_TRUE(NULL!=pFilmingDB);
//
//    int iRet = pFilmingDB->InsertPrintFilmToDB(0, NULL);
//    EXPECT_EQ(0, iRet);
//    iRet = pFilmingDB->InsertPrintFilmToDB(1, NULL);
//    EXPECT_EQ(0, iRet);
//}
//
//TEST_F(FilmingDBTest, InsertPrintJobToDB_Boundary_Condition_Test)
//{
//    Mcsf::McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
//    EXPECT_TRUE(NULL!=pFilmingDB);
//
//    const unsigned int iRet = pFilmingDB->InsertPrintJobToDB(NULL);
//    EXPECT_EQ(0, iRet);
//}
//
//TEST_F(FilmingDBTest, Test_GetAllPrintJob)
//{
//    Mcsf::McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
//    EXPECT_TRUE(NULL!=pFilmingDB);
//
//    std::vector<McsfPrintJobObject> printJobObjectVector;
//    pFilmingDB->GetAllPrintJob(&printJobObjectVector);
//
//    size_t iCount = printJobObjectVector.size();
//    EXPECT_GE(iCount,1);
//
//    
//}
//
//TEST_F(FilmingDBTest, Test_GetPrintFilmJobByID)
//{
//    Mcsf::McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
//    EXPECT_TRUE(NULL!=pFilmingDB);
//
//    std::vector<McsfPrintJobObject> printJobObjectVector;
//    pFilmingDB->GetPrintFilmJobByID(iTestJobID,&printJobObjectVector);
//
//    size_t iCount = printJobObjectVector.size();
//    ASSERT_GE(iCount,1);
//
//    EXPECT_EQ(printJobObjectVector[0].GetJobID(),iTestJobID);
//    EXPECT_EQ(printJobObjectVector[0].GetJobStatus(),0);//pending
//    EXPECT_EQ(printJobObjectVector[0].GetCopies(),1);
//    EXPECT_EQ(printJobObjectVector[0].GetFilmSize(),"8INx10IN");
//    EXPECT_EQ(printJobObjectVector[0].GetJobPriority(),2);//medium
//    EXPECT_EQ(printJobObjectVector[0].GetLayout(),"STANDARD\\2,2");
//    EXPECT_EQ(printJobObjectVector[0].GetMyAETitle(),"UI_Filming");
//    EXPECT_EQ(printJobObjectVector[0].GetTargetAETitle(),"Printer");
//    EXPECT_EQ(printJobObjectVector[0].GetTargetHostName(),"10.1.3.194");
//    EXPECT_EQ(printJobObjectVector[0].GetTargetPort(),3333);
//    EXPECT_EQ(printJobObjectVector[0].GetTargetMaxPDU(),65536);
//    EXPECT_EQ(printJobObjectVector[0].GetTargetSupports12bit(),true);
//
//    
//}
//
//TEST_F(FilmingDBTest, Test_GetHistoryPrintJob)
//{
//    Mcsf::McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
//    EXPECT_TRUE(NULL!=pFilmingDB);
//
//    std::vector<McsfPrintJobObject> printJobObjectVector;
//    std::string sQueryConditionSql = "JobStatus = 0";
//    int iCount = pFilmingDB->GetHistoryPrintJob(sQueryConditionSql,&printJobObjectVector);
//
//    EXPECT_GE(iCount,1);
//
//    
//}
//
//TEST_F(FilmingDBTest, Update_Print_Job_Status_In_DB_By_ID_Is_Not_valid)
//{
//    Mcsf::McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
//    EXPECT_TRUE(NULL!=pFilmingDB);
//
//    McsfPrintJobObject printJobObject;
//    printJobObject.SetJobID(0);
//    
//    int iResult = pFilmingDB->UpdatePrintJobStatusInDB(printJobObject);
//    EXPECT_EQ(0,  iResult);
//
//    
//}
//
//TEST_F(FilmingDBTest, Update_Print_Job_Status_In_DB_By_ID_Is_Not_Existed)
//{
//    Mcsf::McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
//    EXPECT_TRUE(NULL!=pFilmingDB);
//
//    unsigned int uiUnExistedJobID = static_cast<unsigned int> (-1);
//    McsfPrintJobObject printJobObject;
//    printJobObject.SetJobID(uiUnExistedJobID);
//    
//    int iResult = pFilmingDB->UpdatePrintJobStatusInDB(printJobObject);
//    EXPECT_EQ(0,  iResult);
//
//    
//}
//
///// \fn TEST_F(FilmingDBTest, Update_Print_Job_Status_In_DB_By_ID_Is_Existed)
///// <key> \n
///// PRA:Yes \n
///// Tests:DS_PRA_Filming_UpdatePrintJobStatusInDB \n
///// Description:test update print job status in db with an existed job ID \n
///// Short Description:UpdatePrintJobStatusInDB \n
///// Component:Filming \n
///// </key> \n
/////
//TEST_F(FilmingDBTest, Update_Print_Job_Status_In_DB_By_ID_Is_Existed)
//{
//    Mcsf::McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
//    EXPECT_TRUE(NULL!=pFilmingDB);
//
//    McsfPrintJobObject printJobObject;
//    printJobObject.SetJobID(iTestJobID);
//    int iResult = pFilmingDB->UpdatePrintJobStatusInDB(printJobObject);
//    EXPECT_EQ(0,  iResult);
//
//}
//
//TEST_F(FilmingDBTest, Update_Print_Job_Status_In_DB_NOT_Existed)
//{
//	//rename db config file
//	string dbConfigPath = "D:\\UIH\\appdata\\sysconfig\\McsfDatabaseConfig.xml";
//	string tempDBConfigPath = dbConfigPath + "1";
//
//	//rename
//	rename(dbConfigPath.c_str(), tempDBConfigPath.c_str());
//
//	//test
//	Mcsf::McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
//	EXPECT_TRUE(NULL!=pFilmingDB);
//
//	unsigned int uiUnExistedJobID = static_cast<unsigned int> (-1);
//	McsfPrintJobObject printJobObject;
//	printJobObject.SetJobID(uiUnExistedJobID);
//
//	int iResult = pFilmingDB->UpdatePrintJobStatusInDB(printJobObject);
//	EXPECT_EQ(0,  iResult);
//
//
//	//restore db config file
//	rename(tempDBConfigPath.c_str(), dbConfigPath.c_str());
//}
//
//TEST_F(FilmingDBTest, Test_DeleteJobByID)
//{
//    Mcsf::McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
//    EXPECT_TRUE(NULL!=pFilmingDB);
//
//    int iRet = pFilmingDB->DeleteJobByID(iTestJobID);
//    EXPECT_EQ(iRet,0);
//
//    
//}
//
//TEST_F(FilmingDBTest, Test_DeleteJobByID_No_Exists)
//{
//    Mcsf::McsfFilmingDB* pFilmingDB = McsfFilmingDB::GetInstance();
//    EXPECT_TRUE(NULL!=pFilmingDB);
//
//    int iRet = pFilmingDB->DeleteJobByID(0);
//    EXPECT_EQ(iRet,0);
//
//}
//
//
//TEST_F(FilmingDBTest, PrinterJobObject_Abbtributes_Test)
//{
//    McsfPrintJobObject jobObj;
//
//    jobObj.SetAccessionNo("AccessionNo");
//    const string sAccessionNo = jobObj.GetAccessionNo();
//    EXPECT_EQ("AccessionNo", sAccessionNo);
//
//
//    //jobObj.SetAnnotationString("AnnotationString");
//    const string sAnnotationString = jobObj.GetAnnotationString();
//    EXPECT_EQ(EMPTY_STRING, sAnnotationString);
//
//    jobObj.SetCopies(3);
//    const unsigned int uiCopies = jobObj.GetCopies();
//    EXPECT_EQ(3, uiCopies);
//
//    jobObj.SetDefaultPrintIllumination(0);
//    unsigned short sDefaultPrintIllumination = jobObj.GetDefaultPrintIllumination();
//    EXPECT_EQ(2000, sDefaultPrintIllumination);
//    jobObj.SetDefaultPrintIllumination(100);
//    sDefaultPrintIllumination = jobObj.GetDefaultPrintIllumination();
//    EXPECT_EQ(100, sDefaultPrintIllumination);
//
//
//    jobObj.SetDefaultPrintReflection(0);
//    unsigned short sDefaultPrintReflection = jobObj.GetDefaultPrintReflection();
//    EXPECT_EQ(10, sDefaultPrintReflection);
//
//    jobObj.SetDefaultPrintReflection(11);
//    sDefaultPrintReflection = jobObj.GetDefaultPrintReflection();
//    EXPECT_EQ(11, sDefaultPrintReflection);
//    
//    //no definition
//    //jobObj.SetDestination("Destination");
//    //const string sDestination = jobObj.GetDestination();
//    //EXPECT_EQ("Destination", sDestination);
//
//
//    vector<string> ExpectedFileNameList, ResultFileNameList;
//    ExpectedFileNameList.push_back("FileName1");
//    ExpectedFileNameList.push_back("FileName2");
//    jobObj.SetFileNameList(ExpectedFileNameList);
//    ResultFileNameList = jobObj.GetFileNameList();
//    const bool bFileNameList = 
//        equal(  ExpectedFileNameList.begin(), ExpectedFileNameList.end(),
//                ResultFileNameList.begin()  );
//    EXPECT_TRUE(bFileNameList);
//
//
//    jobObj.SetFilmAmount(3);
//    const int iFilmAmount = jobObj.GetFilmAmount();
//    EXPECT_EQ(3, iFilmAmount);
//
//
//    jobObj.SetFilmSize("FilmSize");
//    const string sFilmSize = jobObj.GetFilmSize();
//    EXPECT_EQ("FilmSize", sFilmSize);
//
//    //no definition
//    //jobObj.SetGUIConfigEntryBool("sGUIConfigEntryBool", false);
//    //const bool bGUIConfigEntryBool = 
//    //    jobObj.GetGUIConfigEntryBool("GUIConfigEntryBool", false);
//    //EXPECT_TRUE(bGUIConfigEntryBool);
//
//
//    vector<string> ExpectedHGfilePathList, ResultHGfilePathList;
//
//    bool bSetHGfileList = jobObj.SetHGfileList(ExpectedHGfilePathList);
//    EXPECT_FALSE(bSetHGfileList);
//
//    ExpectedHGfilePathList.push_back("FileName1");
//    ExpectedHGfilePathList.push_back("FileName2");
//    //TODO: getter & setter name not opposite
//    bSetHGfileList = jobObj.SetHGfileList(ExpectedHGfilePathList);
//    EXPECT_TRUE(bSetHGfileList);
//
//    ResultHGfilePathList = jobObj.GetHGfilePathList();
//    const bool bHGfilePathList = 
//        equal(  ExpectedHGfilePathList.begin(), ExpectedHGfilePathList.end(),
//        ResultHGfilePathList.begin()  );
//    EXPECT_TRUE(bHGfilePathList);
//
//    jobObj.SetJobID(3);
//    const unsigned int uiJobID = jobObj.GetJobID();
//    EXPECT_EQ(3, uiJobID);
//
//
//    jobObj.SetJobPriority(1);
//    const int iJobPriority = jobObj.GetJobPriority();
//    EXPECT_EQ(1, iJobPriority);
//
//
//    jobObj.SetJobStatus(1);
//    const int iJobStatus = jobObj.GetJobStatus();
//    EXPECT_EQ(1, iJobStatus);
//
//
//    jobObj.SetLayout("Layout");
//    const string sLayout = jobObj.GetLayout();
//    EXPECT_EQ("Layout", sLayout);
//
//
//    //jobObj.SetLayoutColNumber("LayoutColNumber");
//    const unsigned int uiLayoutColNumber = jobObj.GetLayoutColNumber();
//    EXPECT_EQ(0, uiLayoutColNumber);
//
//
//    //jobObj.SetLayoutRowNumber("LayoutRowNumber");
//    const unsigned int uiLayoutRowNumber = jobObj.GetLayoutRowNumber();
//    EXPECT_EQ(0, uiLayoutRowNumber);
//
//
//    jobObj.SetLutName(EMPTY_STRING);
//    string  sLUTFilename = jobObj.GetLUTFilename(EMPTY_STRING);
//    EXPECT_EQ(EMPTY_STRING, sLUTFilename);
//
//    sLUTFilename = jobObj.GetLUTFilename("lutID");
//    EXPECT_EQ(EMPTY_STRING, sLUTFilename);
//
//    jobObj.SetLutName("LutName");
//    sLUTFilename = jobObj.GetLUTFilename("lutID");
//    EXPECT_EQ("LutName", sLUTFilename);
//
//    //jobObj.SetLUTFolder("LUTFolder");
//    const string sLUTFolder = jobObj.GetLUTFolder();
//    EXPECT_EQ(EMPTY_STRING, sLUTFolder);
//
//    jobObj.SetLutFile("LUTFile");
//    const string sLUTFile = jobObj.GetLutFile();
//    EXPECT_EQ("LUTFile", sLUTFile);
//
//    jobObj.SetLutName("LUTName");
//    const string sLUTName = jobObj.GetLutName();
//    EXPECT_EQ("LUTName", sLUTName);
//
//
//    //jobObj.SetLutShape("LutShape");
//    //const int iLutShape = jobObj.GetLutShape();
//    EXPECT_NO_THROW(jobObj.GetLutShape());
//
//
//    //jobObj.SetMaxPDU("MaxPDU");
//    const unsigned long ulMaxPDU = jobObj.GetMaxPDU();
//    EXPECT_EQ(32768, ulMaxPDU);
//
//
//    //jobObj.SetMaxPreviewResolutonX("MaxPreviewResolutonX");
//    const unsigned long ulMaxPreviewResolutionX = jobObj.GetMaxPreviewResolutionX();
//    EXPECT_EQ(256, ulMaxPreviewResolutionX);
//
//
//    //jobObj.SetMaxPreviewResolutonY("MaxPreviewResolutonY");
//    const unsigned long ulMaxPreviewResolutionY = jobObj.GetMaxPreviewResolutionY();
//    EXPECT_EQ(256, ulMaxPreviewResolutionY);
//
//
//    //jobObj.SetMaxPrintResolutonX("MaxPrintResolutonX");
//    const unsigned long ulMaxPrintResolutionX = jobObj.GetMaxPrintResolutionX();
//    EXPECT_EQ(8192, ulMaxPrintResolutionX);
//
//
//    //jobObj.SetMaxPrintResolutonY("MaxPrintResolutonY");
//    const unsigned long ulMaxPrintResolutionY = jobObj.GetMaxPrintResolutionY();
//    EXPECT_EQ(8192, ulMaxPrintResolutionY);
//
//
//    //jobObj.SetMediumType("MediumType");
//    //const string sMediumType = jobObj.GetMediumType();
//    //EXPECT_EQ("MediumType", sMediumType);
//
//
//    //jobObj.SetMinPrintResolutonX("MinPrintResolutonX");
//    const unsigned long ulMinPrintResolutionX = jobObj.GetMinPrintResolutionX();
//    EXPECT_EQ(1024, ulMinPrintResolutionX);
//
//
//    //jobObj.SetMinPrintResolutonY("MinPrintResolutonY");
//    const unsigned long ulMinPrintResolutionY = jobObj.GetMinPrintResolutionY();
//    EXPECT_EQ(1024, ulMinPrintResolutionY);
//
//
//    //jobObj.SetMonitorCharacteristicsFile("MonitorCharacteristicsFile");
//    //const string sMonitorCharacteristicsFile = jobObj.GetMonitorCharacteristicsFile();
//    //EXPECT_EQ("MonitorCharacteristicsFile", sMonitorCharacteristicsFile);
//
//
//    jobObj.SetMonochrome1(false);
//    const bool bMonochrome1 = jobObj.GetMonochrome1();
//    EXPECT_EQ(false, bMonochrome1);
//
//
//    jobObj.SetMyAETitle("MyAETitle");
//    const string sMyAETitle = jobObj.GetMyAETitle();
//    EXPECT_EQ("MyAETitle", sMyAETitle);
//
//
//    //jobObj.SetNetworkAETitle("NetworkAETitle");
//    //const string sNetworkAETitle = jobObj.GetNetworkAETitle();
//    //EXPECT_EQ("NetworkAETitle", sNetworkAETitle);
//
//
//    jobObj.SetOperatorName("OperatorName");
//    const string sOperatorName = jobObj.GetOperatorName();
//    EXPECT_EQ("OperatorName", sOperatorName);
//
//
//    //jobObj.SetOwnerID("OwnerID");
//    //const string sOwnerID = jobObj.GetOwnerID();
//    //EXPECT_EQ("OwnerID", sOwnerID);
//
//
//    vector<string> ExpectedOriginSOPInstanceUIDList, ResultOriginSOPInstanceUIDList;
//    ExpectedOriginSOPInstanceUIDList.push_back("FileName1");
//    ExpectedOriginSOPInstanceUIDList.push_back("FileName2");
//    jobObj.SetOriginSOPInstanceUIDList(ExpectedOriginSOPInstanceUIDList);
//    ResultOriginSOPInstanceUIDList = jobObj.GetOriginSOPInstanceUIDList();
//    const bool bOriginSOPInstanceUIDList = 
//        equal(  ExpectedOriginSOPInstanceUIDList.begin(), ExpectedOriginSOPInstanceUIDList.end(),
//        ResultOriginSOPInstanceUIDList.begin()  );
//    EXPECT_TRUE(bOriginSOPInstanceUIDList);
//
//
//    //jobObj.SetPrinterDescription("PrinterDescription");
//    //const string sPrinterDescription = jobObj.GetPrinterDescription();
//    //EXPECT_EQ("PrinterDescription", sPrinterDescription);
//
//
//    //jobObj.SetPrinterID("PrinterID");
//    //const string sPrinterID = jobObj.GetPrinterID();
//    //EXPECT_EQ("PrinterID", sPrinterID);
//
//
//    //jobObj.SetPrinterIP("PrinterIP");
//    const string sPrinterIP = jobObj.GetPrinterIP();
//    EXPECT_NE(EMPTY_STRING, sPrinterIP);
//
//
//    //jobObj.SetPrinterPort(3);
//    const unsigned int uiPrinterPort = jobObj.GetPrinterPort();
//    EXPECT_EQ(10006, uiPrinterPort);
//
//
//    jobObj.SetPriority(1);
//    const unsigned int uiPriority = jobObj.GetPriority();
//    EXPECT_EQ(1, uiPriority);
//
//
//    jobObj.SetRefPatientAge("RefPatientAge");
//    const string sRefPatientAge = jobObj.GetRefPatientAge();
//    EXPECT_EQ("RefPatientAge", sRefPatientAge);
//
//
//    jobObj.SetRefPatientID("RefPatientID");
//    const string sRefPatientID = jobObj.GetRefPatientID();
//    EXPECT_EQ("RefPatientID", sRefPatientID);
//
//
//    jobObj.SetRefPatientName("RefPatientName");
//    const string sRefPatientName = jobObj.GetRefPatientName();
//    EXPECT_EQ("RefPatientName", sRefPatientName);
//
//
//    jobObj.SetRefPatientSex("RefPatientSex");
//    const string sRefPatientSex = jobObj.GetRefPatientSex();
//    EXPECT_EQ("RefPatientSex", sRefPatientSex);
//
//
//    //jobObj.SetSessionLable("SessionLable");
//    //const string sSessionLable = jobObj.GetSessionLable();
//    //EXPECT_EQ("SessionLable", sSessionLable);
//
//
//    jobObj.SetSessionPrint(false);
//    const bool bSessionPrint = jobObj.GetSessionPrint();
//    EXPECT_EQ(false, bSessionPrint);
//
//
//    //jobObj.SetSleepTime(1);
//    //const unsigned int uiSleepTime = jobObj.GetSleepTime();
//    //EXPECT_EQ(1, uiSleepTime);
//
//
//    bool bRightSPfilePath = jobObj.SetSPfilePath("SPfilePath");
//    EXPECT_TRUE(bRightSPfilePath);
//    const string sSPfilePath = jobObj.GetSPfilePath();
//    EXPECT_EQ("SPfilePath", sSPfilePath);
//
//    bRightSPfilePath = jobObj.SetSPfilePath(EMPTY_STRING);
//    EXPECT_FALSE(bRightSPfilePath);
//    
//
//
//    //jobObj.SetSpoolPrefix("SpoolPrefix");
//    //const string sSpoolPrefix = jobObj.GetSpoolPrefix();
//    //EXPECT_EQ("SpoolPrefix", sSpoolPrefix);
//
//    bool bSetStorageRootPath = jobObj.SetStorageRootPath(EMPTY_STRING);
//    EXPECT_FALSE(bSetStorageRootPath);
//
//    bSetStorageRootPath = jobObj.SetStorageRootPath("StorageRootPath");
//    const string sStorageRootPath = jobObj.GetStorageRootPath();
//    EXPECT_EQ("StorageRootPath", sStorageRootPath);
//    EXPECT_TRUE(bSetStorageRootPath);
//    
//
//
//    jobObj.SetTargetAETitle("TargetAETitle");
//    const string sTargetAETitle = jobObj.GetTargetAETitle();
//    EXPECT_EQ("TargetAETitle", sTargetAETitle);
//
//
//    //jobObj.SetTargetDescription("TargetDescription");
//    //const string sTargetDescription = jobObj.GetTargetDescription();
//    //EXPECT_EQ("TargetDescription", sTargetDescription);
//
//
//    //jobObj.SetTargetDisableNewVRs(false);
//    //const bool bTargetDisableNewVRs = jobObj.GetTargetDisableNewVRs();
//    //EXPECT_EQ(false, bTargetDisableNewVRs);
//
//
//    //jobObj.SetTargetHostname("TargetHostname");
//    const string sTargetHostname = jobObj.GetTargetHostname();
//    EXPECT_NE(EMPTY_STRING, sTargetHostname);
//
//
//    jobObj.SetTargetHostName("TargetHostName");
//    const string sTargetHostName = jobObj.GetTargetHostName();
//    EXPECT_EQ("TargetHostName", sTargetHostName);
//
//
//    jobObj.SetTargetImplicitOnly(false);
//    const bool bTargetImplicitOnly = jobObj.GetTargetImplicitOnly();
//    EXPECT_EQ(false, bTargetImplicitOnly);
//
//
//    jobObj.SetTargetMaxPDU(3);
//    const unsigned long ulTargetMaxPDU = jobObj.GetTargetMaxPDU();
//    EXPECT_EQ(3, ulTargetMaxPDU);
//
//
//    jobObj.SetTargetPLUTinFilmSession(false);
//    const bool bTargetPLUTinFilmSession = jobObj.GetTargetPLUTinFilmSession();
//    EXPECT_EQ(false, bTargetPLUTinFilmSession);
//
//
//    jobObj.SetTargetPort(10006);
//    const unsigned short usTargetPort = jobObj.GetTargetPort();
//    EXPECT_EQ(10006, usTargetPort);
//
//
//    jobObj.SetTargetPreferSCPLUTRendering(false);
//    const bool bTargetPreferSCPLUTRendering = jobObj.GetTargetPreferSCPLUTRendering();
//    EXPECT_EQ(false, bTargetPreferSCPLUTRendering);
//
//
//    //jobObj.SetTargetPrinterAnnotationDisplayFormatID("TargetPrinterAnnotationDisplayFormatID");
//    const string sTargetPrinterAnnotationDisplayFormatID = jobObj.GetTargetPrinterAnnotationDisplayFormatID();
//    EXPECT_EQ(EMPTY_STRING, sTargetPrinterAnnotationDisplayFormatID);
//
//
//    //jobObj.SetTargetPrinterAnnotationPosition("TargetPrinterAnnotationPosition");
//    const unsigned short usTargetPrinterAnnotationPosition = jobObj.GetTargetPrinterAnnotationPosition();
//    EXPECT_EQ(1, usTargetPrinterAnnotationPosition);
//
//
//    //jobObj.SetTargetPrinterSessionLabelAnnotation("TargetPrinterSessionLabelAnnotation");
//    const bool bTargetPrinterSessionLabelAnnotation = jobObj.GetTargetPrinterSessionLabelAnnotation();
//    EXPECT_EQ(true, bTargetPrinterSessionLabelAnnotation);
//
//
//    //jobObj.SetTargetPrinterSupportsAnnotation("TargetPrinterSupportsAnnotation");
//    const bool bTargetPrinterSupportsAnnotation = jobObj.GetTargetPrinterSupportsAnnotation();
//    EXPECT_EQ(false, bTargetPrinterSupportsAnnotation);
//
//
//    //jobObj.SetTargetPrinterSupportsAnnotationBoxSOPClass("TargetSupportsAnnotationBoxSOPClass");
//    const bool bTargetSupportsAnnotationBoxSOPClass = jobObj.GetTargetPrinterSupportsAnnotationBoxSOPClass();
//    EXPECT_EQ(true, bTargetSupportsAnnotationBoxSOPClass);
//
//
//    //jobObj.SetTargetPrinterSupportsRequestedImageSize("TargetPrinterSupportsRequestedImageSize");
//    //const bool bTargetPrinterSupportsRequestedImageSize = jobObj.GetTargetPrinterSupportsRequestedImageSize();
//    //EXPECT_EQ(false, bTargetPrinterSupportsRequestedImageSize);
//
//    jobObj.SetTargetRequiresMatchingLUT(false);
//    const bool bTargetRequiresMatchingLUT = jobObj.GetTargetRequiresMatchingLUT();
//    EXPECT_EQ(false, bTargetRequiresMatchingLUT);
//
//
//    jobObj.SetTargetSupports12bit(false);
//    const bool bTargetSupports12bit = jobObj.GetTargetSupports12bit();
//    EXPECT_EQ(false, bTargetSupports12bit);
//
//
//    jobObj.SetTargetSupportsAnnotation(false);
//    const bool bTargetSupportsAnnotation = jobObj.GetTargetSupportsAnnotation();
//    EXPECT_EQ(false, bTargetSupportsAnnotation);
//
//
//    jobObj.SetTargetSupportsPLUT(false);
//    const bool bTargetSupportsPLUT = jobObj.GetTargetSupportsPLUT();
//    EXPECT_EQ(false, bTargetSupportsPLUT);
//
//    jobObj.SetOrientation(1);
//    const int iOrientation =  jobObj.GetOrientation();
//    EXPECT_EQ(1, iOrientation);
//
//    //jobObj.SetUseAnnotation(false);
//    //const bool bUseAnnotation = jobObj.GetUseAnnotation();
//    EXPECT_NO_FATAL_FAILURE(jobObj.GetUseAnnotation());
//
//
//    //jobObj.SetUseannotationDatetime(false);
//    //const bool bUseannotationDatetime = jobObj.GetUseannotationDatetime();
//    EXPECT_NO_FATAL_FAILURE(jobObj.GetUseannotationDatetime());
//
//
//    //jobObj.SetUseAnnotationIllumination(false);
//    //const bool bUseAnnotationIllumination = jobObj.GetUseAnnotationIllumination();
//    EXPECT_NO_FATAL_FAILURE(jobObj.GetUseAnnotationIllumination());
//
//
//    //jobObj.SetUseAnnotationPrinter(false);
//    //const bool bUseAnnotationPrinter = jobObj.GetUseAnnotationPrinter();
//    EXPECT_NO_FATAL_FAILURE(jobObj.GetUseAnnotationPrinter());
//
//
//    //jobObj.SetUseInverseLut(false);
//    //const bool bUseInverseLut = jobObj.GetUseInverseLut();
//    EXPECT_NO_FATAL_FAILURE(jobObj.GetUseInverseLut());
//
//}
#endif//    DISABLE_UT
