//Gtest Info
//lint  --e{1684, 1722, 1928, 764, 744, 1961, 1715}   TEST MACRO

//Gtest Error
//lint	--e{36}		4	(line 32)	(Error -- redefining the storage class of symbol 'FilmingTest_Printer_Configure_File_Is_Null_Test::test_info_' (auto vs. static data), conflicts with line 32)
//lint  --e{63}     35  (line 243)  (Error -- Expected an lvalue)
//lint  --e{42}     35  (line 243)  (Error -- Expected a statement)
//lint  --e{107}    35  (line 323)  (Error -- Label 'gtest_label_testnothrow_323' (line 323) not defined)

//Gtest Warning
//lint  --e{665}	5	(line 28)	(Warning -- Unparenthesized parameter 1 in macro 'GTEST_PRED_FORMAT2_' is passed an expression)
//lint	--e{525}	5	(line 30)	(Warning -- Negative indentation from line 28)
//lint	--e{438}	4	(line 54)	(Warning -- Last value assigned to variable 'FilmingTest_Printer_Configure_File_Is_All_Right_Test::test_info_' (defined at line 54, file mcsf_filming_test_case.cpp) not used)
//lint  --e{527}    54  (line 243)  (Warning -- Unreachable code at token
//lint  --e{534}    47  (line 244)  error 534: (Warning -- Ignoring return value of function 'Mcsf::McsfFilmingCommandHandler::HandleCommand
//lint  --e{529, 550}    variable not reference or access
#include "mcsf_filming_containee_config.h"
//
//#include "mcsf_filming_command_handler.h"
//#include "..\McsfFilmingContainee\src\mcsf_filming_command_handler.cpp"

#include "mcsf_filming_containee.h"

#include "mcsf_filming_containee_test_case.h"

using namespace Mcsf;

#ifdef NOT_REFACTORING

McsfFilmingImageCommandHandler* FilmingContaineeTest::m_pImageCmdHandler = NULL;

bool FilmingContaineeTest::TestFuncSplitBorderDensity()
{
    FilmingPrinterList_Printer printer;
    std::string sArrayString = "1\\2\\3";
    SplitBorderDensity(&printer, sArrayString);
    return printer.border_density().size() == 3;
}

bool FilmingContaineeTest::TestFuncSplitDisplayFormat()
{
    FilmingPrinterList_Printer printer;
    std::string sArrayString = "1\\2\\3";
    SplitDisplayFormat(&printer, sArrayString);
    return printer.layout().size() == 3;
}

bool FilmingContaineeTest::TestFuncSplitEmptyImageDensity()
{
    FilmingPrinterList_Printer printer;
    std::string sArrayString = "1\\2\\3";
    SplitEmptyImageDensity(&printer, sArrayString);
    return printer.emptyimagedensity().size() == 3;
}

bool FilmingContaineeTest::TestFuncSplitFilmDestination()
{
    FilmingPrinterList_Printer printer;
    std::string sArrayString = "1\\2\\3";
    SplitFilmDestination(&printer, sArrayString);
    return printer.film_destination().size() == 3;
}

bool FilmingContaineeTest::TestFuncSplitFilmSize()
{
    FilmingPrinterList_Printer printer;
    std::string sArrayString = "1\\2\\3";
    SplitFilmSize(&printer, sArrayString);
    return printer.film_size().size() == 3;
}

bool FilmingContaineeTest::TestFuncSplitMagnificationType()
{
    FilmingPrinterList_Printer printer;
    std::string sArrayString = "1\\2\\3";
    SplitMagnificationType(&printer, sArrayString);
    return printer.magnification_type().size() == 3;
}

bool FilmingContaineeTest::TestFuncSplitMediumType()
{
    FilmingPrinterList_Printer printer;
    std::string sArrayString = "1\\2\\3";
    SplitMediumType(&printer, sArrayString);
    return printer.medium_type().size() == 3;
}

//bool FilmingContaineeTest::TestFuncSplitWrongFormatBorderDensity()
//{
//    FilmingPrinterList_Printer printer;
//    std::string sArrayString = "";
//    return true;
//}
//
//bool FilmingContaineeTest::TestFuncSplitWrongFormatDisplayFormat()
//{
//    return true;
//}
//
//bool FilmingContaineeTest::TestFuncSplitWrongFormatEmptyImageDensity()
//{
//    return true;
//}
//
//bool FilmingContaineeTest::TestFuncSplitWrongFormatFilmDestination()
//{
//    return true;
//}
//
//bool FilmingContaineeTest::TestFuncSplitWrongFormatFilmSize()
//{
//    return true;
//}
//
//bool FilmingContaineeTest::TestFuncSplitWrongFormatMagnificationType()
//{
//    return true;
//}
//
//bool FilmingContaineeTest::TestFuncSplitWrongFormatMediumType()
//{
//    return true;
//}

TEST_F(FilmingContaineeTest, Init_Filming_Containee_Config)
{
    std::string sFilmingContaineeConfigFilePath = InitFilmingContaineeConfig();
    EXPECT_NE("",sFilmingContaineeConfigFilePath);
    //EXPECT_EQ("McsfFilmingLog.xml",sFilmingContaineeConfigFilePath);   
    
}

//TEST_F(FilmingContaineeTest, Init_Mcsf_Print_Job)
//{
//    const FilmingPrintJob filmingPrintJob;
//    std::vector<McsfPrintJobObject> mcsfPrintJobVector;
//    this->InitMcsfPrintJob(filmingPrintJob, &mcsfPrintJobVector);
//
//    EXPECT_EQ(filmingPrintJob.our_ae(), mcsfPrintJobVector[0].GetMyAETitle());
//    EXPECT_EQ(filmingPrintJob.printer_ae(), mcsfPrintJobVector[0].GetTargetAETitle());
//    EXPECT_EQ(filmingPrintJob.printer_ip(), mcsfPrintJobVector[0].GetPrinterIP());
//    EXPECT_EQ(filmingPrintJob.port(), mcsfPrintJobVector[0].GetPrinterPort());
//    EXPECT_EQ(filmingPrintJob.copies(), mcsfPrintJobVector[0].GetCopies());
//    EXPECT_EQ(filmingPrintJob.film_box(0).layout(), mcsfPrintJobVector[0].GetLayout());
//
//    std::vector<std::string> vsImagePathVector = mcsfPrintJobVector[0].GetFileNameList();
//    for(int i = 0; i<filmingPrintJob.film_box(0).image_box_size(); i++)
//    {
//        EXPECT_EQ(filmingPrintJob.film_box(0).image_box(i).image_path(), vsImagePathVector.at(i));
//    }
//
//    
//}


TEST_F(FilmingContaineeTest, Split_Border_Density)
{
    bool bTestFuncSplitBorderDensity = this->TestFuncSplitBorderDensity();
    EXPECT_TRUE(bTestFuncSplitBorderDensity);  
}

TEST_F(FilmingContaineeTest, Split_Display_Format)
{
    bool bTestFuncSplitDisplayFormat = this->TestFuncSplitDisplayFormat();
    EXPECT_TRUE(bTestFuncSplitDisplayFormat);    
}

TEST_F(FilmingContaineeTest, Split_Empty_Image_Density)
{
    bool bTestFuncSplitEmptyImageDensity =  this->TestFuncSplitEmptyImageDensity();
    EXPECT_TRUE(bTestFuncSplitEmptyImageDensity);    
}

TEST_F(FilmingContaineeTest, Split_Film_Destination)
{
    bool bTestFuncSplitFilmDestination = this->TestFuncSplitFilmDestination();
    EXPECT_TRUE(bTestFuncSplitFilmDestination);    
}

TEST_F(FilmingContaineeTest, Split_Film_Size)
{
    bool bTestFuncSplitFilmSize = this->TestFuncSplitFilmSize();
    EXPECT_TRUE(bTestFuncSplitFilmSize);   
}

TEST_F(FilmingContaineeTest, Split_Magnification_Type)
{
    bool bTestFuncSplitMagnificationType = this->TestFuncSplitMagnificationType();
    EXPECT_TRUE(bTestFuncSplitMagnificationType);   
}

TEST_F(FilmingContaineeTest, Split_Medium_Type)
{
    bool bTestFuncSplitMediumType = this->TestFuncSplitMediumType();
    EXPECT_TRUE(bTestFuncSplitMediumType);    
}


//TEST_F(FilmingContaineeTest, Split_Wrong_Format_Border_Density)
//{
//    EXPECT_TRUE(this->TestFuncSplitWrongFormatBorderDensity());
//    
//}
//
//TEST_F(FilmingContaineeTest, Split_Wrong_Format_Display_Format)
//{
//    EXPECT_TRUE(this->TestFuncSplitWrongFormatDisplayFormat());
//    
//}
//
//TEST_F(FilmingContaineeTest, Split_Wrong_Format_Empty_Image_Density)
//{
//    EXPECT_TRUE(this->TestFuncSplitWrongFormatEmptyImageDensity());
//    
//}
//
//TEST_F(FilmingContaineeTest, Split_Wrong_Format_Film_Destination)
//{
//    EXPECT_TRUE(this->TestFuncSplitWrongFormatFilmDestination());
//    
//}
//
//TEST_F(FilmingContaineeTest, Split_Wrong_Format_Film_Size)
//{
//    EXPECT_TRUE(this->TestFuncSplitWrongFormatFilmSize());
//    
//}
//
//TEST_F(FilmingContaineeTest, Split_Wrong_Format_Magnification_Type)
//{
//    EXPECT_TRUE(this->TestFuncSplitWrongFormatMagnificationType());
//    
//}
//
//TEST_F(FilmingContaineeTest, Split_Wrong_Format_Medium_Type)
//{
//    EXPECT_TRUE(this->TestFuncSplitWrongFormatMediumType());
//    
//}

TEST_F(FilmingContaineeTest, PrintThread)
{
    //EXPECT_NO_FATAL_FAILURE(this->OpenPrintThread());
    //EXPECT_NO_THROW(this->ClosePrintThread());
}

TEST_F(FilmingContaineeTest, Handle_Command_Adjusting_Priority_Of_A_Print_Job_Of_Which_ID_Is_Zero)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(ADJUST_PRIORITY_OF_PRINT_JOB_COMMAND);
    std::ostringstream os;
    unsigned int uiJobID = 0;
    os << uiJobID << " " << static_cast<unsigned int>(FilmingPrintJob_PrintPriority_HIGH);
    context.sSerializeObject = os.str();

    int iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

    FilmingJobStatus jobStatus;
    jobStatus.ParseFromString(ReplyObject);
    EXPECT_EQ(-1, jobStatus.return_value());
    EXPECT_EQ(JobStatus::FAILURE, jobStatus.job_status());

    
}

TEST_F(FilmingContaineeTest, Handle_Command_Adjusting_Priority_Of_A_Print_Job_Which_Is_Not_Exist)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(ADJUST_PRIORITY_OF_PRINT_JOB_COMMAND);
    std::ostringstream os;
    unsigned int uiJobID = static_cast<unsigned int>(-1);
    os << uiJobID << " " << static_cast<unsigned int>(FilmingPrintJob_PrintPriority_HIGH);
    context.sSerializeObject = os.str();

    int iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

    FilmingJobStatus jobStatus;
    jobStatus.ParseFromString(ReplyObject);
    EXPECT_EQ(-1, jobStatus.return_value());
    EXPECT_EQ(JobStatus::FAILURE, jobStatus.job_status());

    
}

TEST_F(FilmingContaineeTest, Handle_Command_Adjusting_Priority_Of_A_Print_Job_Which_Is_Exist)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(ADJUST_PRIORITY_OF_PRINT_JOB_COMMAND);
    std::ostringstream os;
    unsigned int uiJobID = kuiJobIDForTest;
    os << uiJobID << " " << static_cast<unsigned int>(FilmingPrintJob_PrintPriority_HIGH);
    context.sSerializeObject = os.str();

    int iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

    //FilmingJobStatus jobStatus;
    //jobStatus.ParseFromString(ReplyObject);
    //EXPECT_EQ(-1, jobStatus.return_value());
    //EXPECT_EQ(JobStatus::FAILURE, jobStatus.job_status());

}


TEST_F(FilmingContaineeTest, Handle_Command_Getting_Print_Config)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(GET_PRINTER_CONFIG_COMMAND);

    int iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

}

TEST_F(FilmingContaineeTest, Handle_Command_Querying_Historic_Print_Jobs)
{
    FilmingHistoryJobQueryCondition historyJobQueryCondition;
    historyJobQueryCondition.set_study_range("2011/09/01-2011/12/13");
    historyJobQueryCondition.set_patient_id("123456");
    historyJobQueryCondition.set_accession_no("7654321");
    historyJobQueryCondition.set_job_status(JobStatus::PENDING);//pending
    
    std::string sSerializedCondition = historyJobQueryCondition.SerializeAsString();

    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(QUERY_HISTORY_PRINT_JOB_COMMAND);
    context.sSerializeObject = sSerializedCondition;

    int iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

    
}

TEST_F(FilmingContaineeTest, Handle_Command_Querying_Historic_Print_Jobs_With_Wrong_Date_Range)
{
    FilmingHistoryJobQueryCondition historyJobQueryCondition;
    historyJobQueryCondition.set_study_range("2011/09/01--2011/12/13");// two '-' is wrong
    historyJobQueryCondition.set_patient_id("123456");
    historyJobQueryCondition.set_accession_no("7654321");
    historyJobQueryCondition.set_job_status(JobStatus::PENDING);//pending

    std::string sSerializedCondition = historyJobQueryCondition.SerializeAsString();

    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(QUERY_HISTORY_PRINT_JOB_COMMAND);
    context.sSerializeObject = sSerializedCondition;

    int iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

    ////////////////the case 2
    historyJobQueryCondition.set_study_range("2011/09/01-2011/12/13-2011/12/24");// three range is wrong
    sSerializedCondition = historyJobQueryCondition.SerializeAsString();

    string ReplyObject2;
    CommandContext context2;
    context2.iCommandId = static_cast<int>(QUERY_HISTORY_PRINT_JOB_COMMAND);
    context2.sSerializeObject = sSerializedCondition;

    iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context2, &ReplyObject2));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context2, &ReplyObject2));

    
}

TEST_F(FilmingContaineeTest, Handle_Command_RePrinting_Job_Of_Which_ID_Is_Zero)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(REPRINT_COMMAND);
    std::ostringstream os;
    unsigned int uiJobID = 0;
    os << uiJobID;
    context.sSerializeObject = os.str();

    int iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

    FilmingJobStatus jobStatus;
    jobStatus.ParseFromString(ReplyObject);
    EXPECT_EQ(-1, jobStatus.return_value());
    EXPECT_EQ(JobStatus::FAILURE, jobStatus.job_status());

    
}

TEST_F(FilmingContaineeTest, Handle_Command_RePrinting_Job_Which_Is_Not_Exist)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(REPRINT_COMMAND);
    std::ostringstream os;
    unsigned int uiJobID = static_cast<unsigned int>(-1);
    os << uiJobID;
    context.sSerializeObject = os.str();

    int iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

    FilmingJobStatus jobStatus;
    jobStatus.ParseFromString(ReplyObject);
    EXPECT_EQ(-1, jobStatus.return_value());
    EXPECT_EQ(JobStatus::FAILURE, jobStatus.job_status());

    
}

TEST_F(FilmingContaineeTest, Handle_Command_Pausing_A_Print_Job_Of_Which_ID_Is_Zero)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(PAUSE_PRINT_JOB_COMMAND);
    std::ostringstream os;
    unsigned int uiJobID = 0;
    os << uiJobID;
    context.sSerializeObject = os.str();

    EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

    FilmingJobStatus jobStatus;
    jobStatus.ParseFromString(ReplyObject);
    EXPECT_EQ(-1, jobStatus.return_value());
    EXPECT_EQ(JobStatus::FAILURE, jobStatus.job_status());    
}

TEST_F(FilmingContaineeTest, Handle_Command_Pausing_A_Print_Job_Which_Is_Not_Exist)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(PAUSE_PRINT_JOB_COMMAND);
    std::ostringstream os;
    unsigned int uiJobID = static_cast<unsigned int>(-1);
    os << uiJobID;
    context.sSerializeObject = os.str();

    int iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

    FilmingJobStatus jobStatus;
    jobStatus.ParseFromString(ReplyObject);
    EXPECT_EQ(-1, jobStatus.return_value());
    EXPECT_EQ(JobStatus::FAILURE, jobStatus.job_status());

}

TEST_F(FilmingContaineeTest, Handle_Command_Pausing_A_Print_Job_Which_Is_Exist)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(PAUSE_PRINT_JOB_COMMAND);
    std::ostringstream os;
    unsigned int uiJobID = kuiJobIDForTest;
    os << uiJobID;
    context.sSerializeObject = os.str();

    int iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

    //FilmingJobStatus jobStatus;
    //jobStatus.ParseFromString(ReplyObject);
    //EXPECT_EQ(-1, jobStatus.return_value());
    //EXPECT_EQ(JobStatus::FAILURE, jobStatus.job_status());

}


TEST_F(FilmingContaineeTest, Handle_Command_Resuming_A_Print_Job_Of_Which_ID_Is_Zero)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(RESUME_PRINT_JOB_COMMAND);
    std::ostringstream os;
    unsigned int uiJobID = 0;
    os << uiJobID;
    context.sSerializeObject = os.str();

    int iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

    FilmingJobStatus jobStatus;
    jobStatus.ParseFromString(ReplyObject);
    EXPECT_EQ(-1, jobStatus.return_value());
    EXPECT_EQ(JobStatus::FAILURE, jobStatus.job_status());

    
}

TEST_F(FilmingContaineeTest, Handle_Command_Resuming_A_Print_Job_Which_Is_Not_Exist)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(RESUME_PRINT_JOB_COMMAND);
    std::ostringstream os;
    unsigned int uiJobID = static_cast<unsigned int>(-1);
    os << uiJobID;
    context.sSerializeObject = os.str();

    int iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

    FilmingJobStatus jobStatus;
    jobStatus.ParseFromString(ReplyObject);
    EXPECT_EQ(-1, jobStatus.return_value());
    EXPECT_EQ(JobStatus::FAILURE, jobStatus.job_status());    
}

TEST_F(FilmingContaineeTest, Handle_Command_Resuming_A_Print_Job_Which_Is_Exist)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(RESUME_PRINT_JOB_COMMAND);
    std::ostringstream os;
    unsigned int uiJobID = kuiJobIDForTest;
    os << uiJobID;
    context.sSerializeObject = os.str();

    int iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

    //FilmingJobStatus jobStatus;
    //jobStatus.ParseFromString(ReplyObject);
    //EXPECT_EQ(-1, jobStatus.return_value());
    //EXPECT_EQ(JobStatus::FAILURE, jobStatus.job_status());    
}

TEST_F(FilmingContaineeTest, Handle_Command_Adding_A_Print_Job)
{
    string ReplyObject;
    CommandContext context;
    this->AddPrintJob(context);
    context.iCommandId = static_cast<int>(ADD_PRINT_JOB_COMMAND);

    int iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

}

TEST_F(FilmingContaineeTest, Handle_Command_Querying_Current_Print_Jobs)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(QUERY_CURRENT_PRINT_JOBS_COMMAND);

    int iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

}

void FilmingContaineeTest::AddPrintJob(CommandContext& context)
{
    FilmingPrintJob filmingPrintJob;
    FilmingPrintJob_FilmBox* pFilmBox = NULL;
    FilmingPrintJob_ImageBox* pImageBox = NULL;

    filmingPrintJob.set_printer_ae("LEAD_SERVER");//root["sCalledAE"] = "LEAD_SERVER";
    filmingPrintJob.set_our_ae("MWLSCU");//root["sCallingAE"] = "MWLSCU";
    filmingPrintJob.set_printer_ip("10.1.4.155");//root["sIP"] = "10.1.4.233";
    filmingPrintJob.set_port(10006);//root["iPort"] = 104;

    for(int i=0; i<2; i++)
    {
        pFilmBox = filmingPrintJob.add_film_box();

        if(i == 0)
            pFilmBox->set_layout("STANDARD\\1,1");
        else if(i == 1)
            pFilmBox->set_layout("STANDARD\\1,2");

        pImageBox = pFilmBox->add_image_box();
        pImageBox->set_image_path("//10.1.2.13/Public/DicomData/MR_STU00040/IMG00001");
        pImageBox = pFilmBox->add_image_box();
        pImageBox->set_image_path("//10.1.2.13/Public/DicomData/MR_STU00040/IMG00002");
        pImageBox = pFilmBox->add_image_box();
        pImageBox->set_image_path("//10.1.2.13/Public/DicomData/MR_STU00040/IMG00003");
        pImageBox = pFilmBox->add_image_box();
        pImageBox->set_image_path("//10.1.2.13/Public/DicomData/MR_STU00040/IMG00004");
        pImageBox = pFilmBox->add_image_box();
        pImageBox->set_image_path("//10.1.2.13/Public/DicomData/MR_STU00040/IMG00005");
        pImageBox = pFilmBox->add_image_box();
        pImageBox->set_image_path("//10.1.2.13/Public/DicomData/MR_STU00040/IMG00006");
        pImageBox = pFilmBox->add_image_box();
        pImageBox->set_image_path("//10.1.2.13/Public/DicomData/MR_STU00040/IMG00007");
        pImageBox = pFilmBox->add_image_box();
        pImageBox->set_image_path("//10.1.2.13/Public/DicomData/MR_STU00040/IMG00008");
    }

    std::string sSerialized = filmingPrintJob.SerializeAsString();
    context.sSerializeObject = sSerialized;
}


TEST_F(FilmingContaineeTest, Handle_Command_Deleting_A_Print_Job_Of_Which_ID_Is_Zero)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(DELETE_PRINT_JOB_COMMAND);
    std::ostringstream os;
    unsigned int uiJobID = 0;
    os << uiJobID;
    context.sSerializeObject = os.str();

    int iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

    FilmingJobStatus jobStatus;
    jobStatus.ParseFromString(ReplyObject);
    EXPECT_EQ(-1, jobStatus.return_value());
    EXPECT_EQ(JobStatus::FAILURE, jobStatus.job_status());

}

TEST_F(FilmingContaineeTest, Handle_Command_Deleting_A_Print_Job_Which_Is_Not_Exist)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(DELETE_PRINT_JOB_COMMAND);
    std::ostringstream os;
    unsigned int uiJobID = static_cast<unsigned int>(-1);
    os << uiJobID;
    context.sSerializeObject = os.str();

    int iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

    FilmingJobStatus jobStatus;
    jobStatus.ParseFromString(ReplyObject);
    EXPECT_EQ(-1, jobStatus.return_value());
    EXPECT_EQ(JobStatus::FAILURE, jobStatus.job_status());
}

TEST_F(FilmingContaineeTest, Handle_Command_Deleting_A_Print_Job_Which_Is_Exist)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(DELETE_PRINT_JOB_COMMAND);
    std::ostringstream os;
    unsigned int uiJobID = static_cast<unsigned int>(-1);
    os << uiJobID;
    context.sSerializeObject = os.str();

    int iRet = this->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

    //EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));

    //the test environment need to setup
    //FilmingJobStatus jobStatus;
    //jobStatus.ParseFromString(ReplyObject);
    //EXPECT_EQ(-1, jobStatus.return_value());
    //EXPECT_EQ(JobStatus::FAILURE, jobStatus.job_status());
}


TEST_F(FilmingContaineeTest, MCSF_Filming_Containee_Test)
{
    McsfFilmingContainee containee;

    containee.SetName("McsfFilmingBE");
    std::string sContaineeName =  containee.GetName();
    EXPECT_EQ("McsfFilmingBE", sContaineeName);

    containee.SetCommunicationProxy(NULL);
    EXPECT_NO_FATAL_FAILURE(containee.SetCommunicationProxy(NULL));
    EXPECT_NO_THROW(containee.SetCommunicationProxy(NULL));

    containee.SetCustomConfigFile("ContainerFilming.xml");
    EXPECT_NO_FATAL_FAILURE(containee.SetCustomConfigFile("ContainerFilming.xml"));
    EXPECT_NO_THROW(containee.SetCustomConfigFile("ContainerFilming.xml"));

    containee.Startup();
    EXPECT_NO_FATAL_FAILURE(containee.Startup());
    EXPECT_NO_THROW(containee.Startup());

    bool bReadyForShutdown = containee.IsReadyForShutdown();
    EXPECT_TRUE(bReadyForShutdown);
    bool bShutDownAble = containee.IsShutDownAble();
    EXPECT_TRUE(bShutDownAble);
    bool bShutDown = containee.Shutdown();
    EXPECT_FALSE(bShutDown);
    bool bWaitForShutdown = containee.WaitForShutdown();
    EXPECT_FALSE(bWaitForShutdown);   
}

//for coverage of register command
TEST_F(FilmingContaineeTest, MCSF_Filming_Containee_Do_Work)
{
    McsfFilmingContainee containee;
    ICommunicationProxy* pCommProxy = new CommunicationProxy();
    containee.SetCommunicationProxy(pCommProxy);
    EXPECT_NO_FATAL_FAILURE(containee.DoWork());
    delete pCommProxy;
}

TEST_F(FilmingContaineeTest, HANDLE_COMMAND_LOAD_IMAGES_BY_PATHS_HAVE_SOMETHING_WRONG)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(LOAD_IMAGE_COMMAND);
    context.sSerializeObject = "Not exist image file";
         
    //EXPECT_NO_THROW(m_pImageCmdHandler->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(m_pImageCmdHandler->HandleCommand(&context, &ReplyObject));

    int iRet = m_pImageCmdHandler->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

       
}

TEST_F(FilmingContaineeTest, HANDLE_COMMAND_LOAD_IMAGES_BY_PATHS)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(LOAD_IMAGE_COMMAND);
    context.sSerializeObject = "//10.1.2.13/Public/DicomData/MR_STU00040/IMG00001;//10.1.2.13/Public/DicomData/MR_STU00040/IMG00002";

    //EXPECT_NO_THROW(m_pImageCmdHandler->HandleCommand(&context, &ReplyObject));
    EXPECT_NO_FATAL_FAILURE(m_pImageCmdHandler->HandleCommand(&context, &ReplyObject));

    //int iRet = m_pImageCmdHandler->HandleCommand(&context, &ReplyObject);
    //EXPECT_EQ(0, iRet);
      
}

TEST_F(FilmingContaineeTest, HANDLE_COMMAND_REMOVE_ALL)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(REMOVE_ALL_COMMAND);
    context.sSerializeObject = "";

    //EXPECT_NO_THROW(m_pImageCmdHandler->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(m_pImageCmdHandler->HandleCommand(&context, &ReplyObject));

    int iRet = m_pImageCmdHandler->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(-1, iRet);      
}


TEST_F(FilmingContaineeTest, HANDLE_COMMAND_SAVE_IMAGES_Lack_of_Parameters)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(SAVE_IMAGE_COMMAND);
    context.sSerializeObject = "";

    //EXPECT_NO_THROW(m_pImageCmdHandler->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(m_pImageCmdHandler->HandleCommand(&context, &ReplyObject));

    int iRet = m_pImageCmdHandler->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);

       
}

TEST_F(FilmingContaineeTest, HANDLE_COMMAND_SAVE_IMAGES_File_Path_Not_exist)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(SAVE_IMAGE_COMMAND);
    context.sSerializeObject = "0;0;Not_existed_file_path";

    //EXPECT_NO_THROW(m_pImageCmdHandler->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(m_pImageCmdHandler->HandleCommand(&context, &ReplyObject));

    int iRet = m_pImageCmdHandler->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);
      
}


TEST_F(FilmingContaineeTest, HANDLE_COMMAND_SAVE_IMAGES_File_Path_exist)
{
    string ReplyObject;
    CommandContext context;
    context.iCommandId = static_cast<int>(SAVE_IMAGE_COMMAND);
    context.sSerializeObject = "0;0;c:/1";

    //EXPECT_NO_THROW(m_pImageCmdHandler->HandleCommand(&context, &ReplyObject));
    //EXPECT_NO_FATAL_FAILURE(m_pImageCmdHandler->HandleCommand(&context, &ReplyObject));

    int iRet = m_pImageCmdHandler->HandleCommand(&context, &ReplyObject);
    EXPECT_EQ(0, iRet);
}

//TEST_F(FilmingContaineeTest, HANDLE_COMMAND_LOAD_STUDY)
//{
//    string ReplyObject;
//    CommandContext context;
//    context.iCommandId = static_cast<int>(LOAD_STUDY_COMMAND);
//    context.sSerializeObject = "";
//
//    EXPECT_NO_THROW(this->HandleCommand(&context, &ReplyObject));
//    EXPECT_NO_FATAL_FAILURE(this->HandleCommand(&context, &ReplyObject));
//
//    int iRet = this->HandleCommand(&context, &ReplyObject);
//    EXPECT_EQ(0, iRet);
//       
//}

#endif //NOT_REFACTORING
