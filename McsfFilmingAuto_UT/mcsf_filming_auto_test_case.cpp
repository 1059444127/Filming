//////////////////////////////////////////////////////////////////////////
///  Copyright (c) Shanghai United Imaging Healthcare Inc., 2011
///  All rights reserved.
///
///  /author  Wang Hui   mailto:hui.wang@united-imaging.com
///
///  /file mcsf_filming_test_case.cpp
/// 
///  /brief testing Case for auto Filming interface
///
///  /version 1.0
///  /date    Dec/27/2011
//////////////////////////////////////////////////////////////////////////

//Gtest Info
//lint  --e{1704, 1722, 1928, 764, 744, 1961, 1715}   TEST MACRO
//lint  --e{746, 526, 628}

//Gtest error
//lint  --e{36}     line 53 (Error -- redefining the storage class of symbol 'FilmingAutoTest_Setters_And_Getters_Test::test_info_' (auto vs. static data), conflicts with line 53)
//lint  --e{1013}   line 55 (Error -- Symbol 'SetAccessionNo' not a member of class 'FilmingAutoTest_Setters_And_Getters_Test')
//lint  --e{1055}   line 55 (Error -- Symbol 'SetAccessionNo' undeclared, assumed to return int)
//Gtest warning
//lint  --e{529}
//lint  --e{526}
//lint  --e{665}    line 34 (Warning -- Unparenthesized parameter 1 in macro 'GTEST_PRED_FORMAT2_' is passed an expression)
//lint  --e{525}    line 51 (Warning -- Negative indentation from line 34)
//lint  --e{666}    line 112(Warning -- Expression with side effects passed to repeated parameter 1 in macro 'EXPECT_EQ')
//lint  --e{438}    line 100(Warning -- Last value assigned to variable 'FilmingAutoTest_Set_Col_Layout_Test::test_info_' (defined at line 100, file mcsf_filming_auto_test_case.cpp) not used)
//lint  --e{628}    line 110(Warning -- no argument information provided for function 'SetLayout()' (line 110, file mcsf_filming_auto_test_case.cpp))
#include <sstream>

#include "McsfNetBase/mcsf_communication_proxy.h"

#include "mcsf_filming_auto_printer_parameters_config.h"
#include "mcsf_filming_auto_test_case.h"

using namespace Mcsf;
//#define DLL_INTERFACE_TEST

#ifndef DLL_INTERFACE_TEST
TEST_F(FilmingAutoTest, FilmingAutoJobCreater_Constructor)
{
    //EXPECT_EQ(MEDIUM, this->m_ePrintPriority);
    ////FilmingPrintJob expectedFilmingPrintJob;
    ////EXPECT_EQ(expectedFilmingPrintJob, this->m_filmingPrintJob);
    //EXPECT_EQ(1, this->m_iCopies);
    ////EXPECT_EQ(0, this->m_iPrinterPort);
    //EXPECT_EQ(1, this->m_iSheetImageCount);
    //EXPECT_EQ(EMPTY_STRING, this->m_sAccessionNo);
    //EXPECT_EQ(EMPTY_STRING, this->m_sLayout);
    //EXPECT_EQ(EMPTY_STRING, this->m_sOperatorName);
    //EXPECT_NE(EMPTY_STRING, this->m_sOurAE);
    //EXPECT_EQ(EMPTY_STRING, this->m_sPatientAge);
    //EXPECT_EQ(EMPTY_STRING, this->m_sPatientID);
    //EXPECT_EQ(EMPTY_STRING, this->m_sPatientName);
    //EXPECT_EQ(EMPTY_STRING, this->m_sPatientSex);
    //EXPECT_NE(EMPTY_STRING, this->m_sPrinterAE);
    ////EXPECT_EQ(EMPTY_STRING, this->m_sPrinterIP);
    //EXPECT_EQ(0, this->m_vcDicomFilePathVector.size());
}

TEST_F(FilmingAutoTest, Setters_And_Getters)
{
    (void)this->SetAccessionNo("AccessionNo");
    //EXPECT_EQ("AccessionNo", this->m_sAccessionNo);

    this->SetCopies(2);
    int iCopies = this->GetCopies();
    EXPECT_EQ(2, iCopies);

    vector<string> vectorToBeSet;
    vectorToBeSet.push_back("C:/1.png");
    vectorToBeSet.push_back("C:/2.png");
    vectorToBeSet.push_back("C:/1.png");
    vectorToBeSet.push_back("C:/2.png");
    vectorToBeSet.push_back("C:/1.png");
    vectorToBeSet.push_back("C:/2.png");
    vectorToBeSet.push_back("C:/1.png");
    this->SetDicomFilePathVector(vectorToBeSet);
    vector<string> vectorToBeGet = this->GetDicomFilePathVector();
    bool bDicomFilePathVectorEqual = (vectorToBeGet == vectorToBeSet);
    EXPECT_TRUE(bDicomFilePathVectorEqual);

    //this->SetLayout("Layout");
    //EXPECT_EQ("Layout", this->m_sLayout);

    this->SetOperatorName("OperatorName");
    //EXPECT_EQ("OperatorName", this->m_sOperatorName);

    this->SetPatientAge("PatientAge");
    //EXPECT_EQ("PatientAge", this->m_sPatientAge);

    this->SetPatientID("PatientID");
    //EXPECT_EQ("PatientID", this->m_sPatientID);

    this->SetPatientName("PatientName");
    //EXPECT_EQ("PatientName", this->m_sPatientName);

    this->SetPatientSex("PatientSex");
    //EXPECT_EQ("PatientSex", this->m_sPatientSex);

    this->SetPrintPriority(HIGH);
    PRINT_PRIORITY_ENUM expectedPriority = this->GetPrintPriority();
    EXPECT_EQ(HIGH,expectedPriority);
}
//lint -e529    for iSheetImageCount
//lint -e522    for LAYOUT_TYPE_ENUM
TEST_F(FilmingAutoTest, Set_Col_Layout)
{
    LAYOUT_TYPE_ENUM layout = COL;
    const int iFirst = 2;
    const int iSecond = 3;
    int iSheetImageCount = iFirst*iSecond;

    ostringstream os;
    os << "COL\\" << iFirst << "," << iSecond;

    (void)this->SetLayout(layout, iFirst, iSecond);

    //EXPECT_EQ(os.str(), this->m_sLayout);

    //EXPECT_EQ(iSheetImageCount, this->m_iSheetImageCount);

    SUCCEED();
}

TEST_F(FilmingAutoTest, Set_Row_Layout)
{
    LAYOUT_TYPE_ENUM layout = ROW;
    const int iFirst = 2;
    const int iSecond = 3;
    int iSheetImageCount = iFirst*iSecond;

    ostringstream os;
    os << "ROW\\" << iFirst << "," << iSecond;

    (void)this->SetLayout(layout, iFirst, iSecond);

    //EXPECT_EQ(os.str(), this->m_sLayout);
    //EXPECT_EQ(iSheetImageCount, this->m_iSheetImageCount);

    SUCCEED();
}

TEST_F(FilmingAutoTest, Set_Standard_Layout)
{
    LAYOUT_TYPE_ENUM layout = STANDARD;
    const int iFirst = 2;
    const int iSecond = 2;
    int iSheetImageCount = iFirst*iSecond;
    
    ostringstream os;
    os << "STANDARD\\" << iFirst << "," << iSecond;

    (void)this->SetLayout(layout, iFirst, iSecond);

    //EXPECT_EQ(os.str(), this->m_sLayout);
    //EXPECT_EQ(iSheetImageCount, this->m_iSheetImageCount);
    SUCCEED();
}//lint +e522   for LAYOUT_TYPE_ENUM
//lint  +e529   for iSheetImageCount
#endif  //DLL_INTERFACE_TEST

TEST_F(FilmingAutoTest, Create_Filming_Job_Command_Context)
{
    (void)this->SetAccessionNo("AccessionNo");
    //EXPECT_EQ("AccessionNo", this->m_sAccessionNo);

    this->SetCopies(2);
    int iCopies = this->GetCopies();
    EXPECT_EQ(2, iCopies);

    vector<string> vectorToBeSet;
    vectorToBeSet.push_back("C:/1.png");
    vectorToBeSet.push_back("C:/2.png");
    vectorToBeSet.push_back("C:/1.png");
    vectorToBeSet.push_back("C:/2.png");
    vectorToBeSet.push_back("C:/1.png");
    vectorToBeSet.push_back("C:/2.png");
    vectorToBeSet.push_back("C:/1.png");
    this->SetDicomFilePathVector(vectorToBeSet);
    vector<string> vectorToBeGet = this->GetDicomFilePathVector();
    bool bDicomFilePathVectorEqual = (vectorToBeGet == vectorToBeSet);
    EXPECT_TRUE(bDicomFilePathVectorEqual);

    //this->SetLayout("Layout");
    //EXPECT_EQ("Layout", this->m_sLayout);

    this->SetOperatorName("OperatorName");
    //EXPECT_EQ("OperatorName", this->m_sOperatorName);

    this->SetPatientAge("PatientAge");
    //EXPECT_EQ("PatientAge", this->m_sPatientAge);

    this->SetPatientID("PatientID");
    //EXPECT_EQ("PatientID", this->m_sPatientID);

    this->SetPatientName("PatientName");
    //EXPECT_EQ("PatientName", this->m_sPatientName);

    this->SetPatientSex("PatientSex");
    //EXPECT_EQ("PatientSex", this->m_sPatientSex);

    this->SetPrintPriority(HIGH);
    PRINT_PRIORITY_ENUM expectedPriority = this->GetPrintPriority();
    EXPECT_EQ(HIGH,expectedPriority);

    CommandContext commandContext;
    (void)this->CreateFilmingJobCommandContext(commandContext);

    SUCCEED();
}

TEST_F(FilmingAutoTest, Resume_all_filming_job)
{
    ICommunicationProxy* proxy = new CommunicationProxy();

    EXPECT_TRUE(proxy != NULL);

    (void)this->ResumeAllFilmingJob(*proxy);

    SUCCEED();
}

TEST_F(FilmingAutoTest, Send_Filming_Job_Command)
{
    ICommunicationProxy* proxy = new CommunicationProxy();

    EXPECT_TRUE(proxy != NULL);

    (void)this->SendFilmingJobCommand(*proxy);

    SUCCEED();
}

TEST_F(FilmingAutoTest, Suspend_All_Filming_Job)
{
    ICommunicationProxy* proxy = new CommunicationProxy();

    EXPECT_TRUE(proxy != NULL);

    (void)this->SuspendAllFilmingJob(*proxy);

    SUCCEED();
}

TEST_F(FilmingAutoTest, ParseFilmingConfigFile_NotExist)
{
    McsfPrinterParametersConfig printerParamtersConfig;

    bool bRet = printerParamtersConfig.ParseFilmingConfigFile("Z:\\McsfFilmingConfig.xml");

    EXPECT_TRUE(bRet == false);

    SUCCEED();
}

TEST_F(FilmingAutoTest, ParsePrinterConfigFile_NotExist)
{
    McsfPrinterParametersConfig printerParamtersConfig;

    bool bRet = printerParamtersConfig.ParsePrinterConfigFile("Z:\\McsfPrinterConfig.xml");

    EXPECT_TRUE(bRet == false);

    SUCCEED();
}

TEST_F(FilmingAutoTest, GetPacsNodeByAE_Unknown)
{
    McsfPrinterParametersConfig printerParamConfig;

    std::string sStringIP;
    unsigned int iPort;
    int iRet = printerParamConfig.GetPacsNodeParametersByAE("unknown",&sStringIP,&iPort);

    EXPECT_TRUE(iRet == -1);

    SUCCEED();
}
