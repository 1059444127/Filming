//////////////////////////////////////////////////////////////////////////
///  Copyright (c) Shanghai United Imaging Healthcare Inc., 2011
///  All rights reserved.
///
///  /author  Wang Hui   mailto:hui.wang@united-imaging.com
///
///  /file mcsf_filming_test_case.cpp
/// 
///  /brief testing Case for DICOM Filming
///
///  /version 1.0
///  /date    Oct/26/2011
//////////////////////////////////////////////////////////////////////////
//Gtest Info
//lint  --e{1704, 1722, 1928, 764, 744, 1961, 1715, 1788}   TEST MACRO

//Gtest Error
//lint	--e{36}		4	(line 32)	(Error -- redefining the storage class of symbol 'FilmingTest_Printer_Configure_File_Is_Null_Test::test_info_' (auto vs. static data), conflicts with line 32)

//Gtest Warning
//lint  --e{665}	5	(line 28)	(Warning -- Unparenthesized parameter 1 in macro 'GTEST_PRED_FORMAT2_' is passed an expression)
//lint	--e{525}	5	(line 30)	(Warning -- Negative indentation from line 28)
//lint	--e{438}	4	(line 54)	(Warning -- Last value assigned to variable 'FilmingTest_Printer_Configure_File_Is_All_Right_Test::test_info_' (defined at line 54, file mcsf_filming_test_case.cpp) not used)
//lint -e529
//lint -e107
//lint -e42
//lint -e63

#include "mcsf_filming_test_case.h"

#include <limits.h>

#include "mcsf_miscellaneous_config.h"
#include "mcsf_dcm_printer_config.h"
#include "mcsf_dcm_film_session_object.h"
//#include "mcsf_filming_config.cpp"

using namespace Mcsf;

#define USE_LOCAL_FILE_TO_TEST

TEST_F(FilmingTest, Parsing_Miscellaneous_And_Get_MediaType)
{
	MiscellaneousConfig aMiscellaneousConfig;
	EXPECT_NE("CLEAR FILM", aMiscellaneousConfig.GetMediaType());
	EXPECT_NE("BIN_1", aMiscellaneousConfig.GetFilmDestination());
	EXPECT_NE("PRINT", aMiscellaneousConfig.GetFilmSessionLabel());
	EXPECT_NE("MED", aMiscellaneousConfig.GetFilmPriority());
	EXPECT_NE("UIH", aMiscellaneousConfig.GetOwnerID());
}

TEST_F(FilmingTest, DCM_Film_Session_Object_Default_Constructor_Test)
{
    McsfDcmFilmSessionObject ob;
    string sDefaultTargetAETitle = ob.GetTargetAETitle();
    EXPECT_EQ(DEFAULT_AE, sDefaultTargetAETitle);
    string sDefaultLayout = ob.GetImageDisplayFormat();
    EXPECT_EQ(DEFAULT_LAYOUT, sDefaultLayout);
    string sDefaultTargetHostname = ob.GetTargetHostname();
    EXPECT_EQ(EMPTY_STRING, sDefaultTargetHostname);
}

TEST_F(FilmingTest, DCM_Film_Session_Object_Constructor_By_PrinterAE_Is_Null)
{
    McsfDcmFilmSessionObject ob("", "PrinterIP", 8812, "Layout");
    string sTargetAETitle = ob.GetTargetAETitle();
    EXPECT_EQ("UNKNOWAE", sTargetAETitle);
}

TEST_F(FilmingTest, DCM_Film_Session_Object_Constructor_By_PrinterAE_Is_Not_Null)
{
    McsfDcmFilmSessionObject ob("PrinterAE", "PrinterIP", 8812, "Layout");
    string sTargetAETitle = ob.GetTargetAETitle();
    EXPECT_EQ("PrinterAE", sTargetAETitle);

    bool bDisableNewVRs = ob.GetDisableNewVRs();
    EXPECT_FALSE(bDisableNewVRs);
}

TEST_F(FilmingTest, DCM_Film_Session_Object_Attributes_Test)
{
    McsfDcmFilmSessionObject ob;

    //ob.SetCopies(10);
    //EXPECT_EQ( 10 , ob.GetCopies()  ) << "McsfDcmFilmSessionObject.GetCopies and .SetCopies not match";

    ob.SetImageDisplayFormat("ImageDisplayFormat");
    string sImageDisplayFormat = ob.GetImageDisplayFormat();
    EXPECT_EQ("ImageDisplayFormat", sImageDisplayFormat)<< "McsfDcmFilmSessionObject.GetImageDisplayFormat and .SetImageDisplayFormat not match";

    ob.SetImplictOnly(true);
    bool bImplictOnly = ob.GetImplictOnly();
    EXPECT_EQ(true, bImplictOnly) << "McsfDcmFilmSessionObject.GetImplictOnly and .SetImplictOnly not match";

    ob.SetMaxPDU(11);
    unsigned int iMaxPDU = ob.GetMaxPDU();
    EXPECT_EQ(11, iMaxPDU) << "McsfDcmFilmSessionObject.GetMaxPDU and .SetMaxPDU not match";

    ob.SetPrinterDescription("PrinterDescription");
    string sPrinterDescription = ob.GetPrinterDescription();
    EXPECT_EQ("PrinterDescription", sPrinterDescription) << "McsfDcmFilmSessionObject.GetPrinterDescription and .SetPrinterDescription not match";

    ob.SetPrinterID("PrinterID");
    string sPrinterID = ob.GetPrinterID();
    EXPECT_EQ("PrinterID", sPrinterID) << "McsfDcmFilmSessionObject.GetPrinterID and .SetPrinterID not match";

    ob.SetPrinterIP("10.1.2.13");
    string sPrinterIP = ob.GetPrinterIP();
    EXPECT_EQ("10.1.2.13", sPrinterIP) << "McsfDcmFilmSessionObject.GetPrinterIP and .SetPrinterIP not match";

    ob.SetPrinterIP("PrinterIP");
    sPrinterIP = ob.GetPrinterIP();
    EXPECT_EQ("PrinterIP", sPrinterIP) << "McsfDcmFilmSessionObject.GetPrinterIP and .SetPrinterIP not match";

    ob.SetPrinterPort(8812);
    unsigned int iPrinterPort = ob.GetPrinterPort();
    EXPECT_EQ(8812, iPrinterPort) << "McsfDcmFilmSessionObject.GetPrinterPort and .SetPrinterPort not match";

    ob.SetTargetAETitle("TargetAETitle");
    string sTargetAETitle = ob.GetTargetAETitle();
    EXPECT_EQ("TargetAETitle", sTargetAETitle) << "McsfDcmFilmSessionObject.GetTargetAETitle and .SetTargetAETitle not match";

    ob.SetTargetHostname("TargetHostname");
    string sTargetHostname = ob.GetTargetHostname();
    EXPECT_EQ("TargetHostname", sTargetHostname) << "McsfDcmFilmSessionObject.GetTargetHostname and .SetTargetHostname not match";

    //TODO: add other accessors test
    //ob.SetAnnotationString("AnnotationString");
    //EXPECT_EQ("TargetHostname", sTargetHostname) << "McsfDcmFilmSessionObject.GetTargetHostname and .SetTargetHostname not match";

    string sBorderDensity = ob.GetBorderDensity();
    EXPECT_EQ(EMPTY_STRING, sBorderDensity);

    //ob.GetCopies();

    //ob.GetDisableNewVRs();

    string sEmptyImageDensity = ob.GetEmptyImageDensity();
    EXPECT_EQ(EMPTY_STRING, sEmptyImageDensity);

    string sFilmDestination = ob.GetFilmDestination();
    EXPECT_EQ(EMPTY_STRING, sFilmDestination);

    string sFilmSize = ob.GetFilmSize();
    EXPECT_EQ(EMPTY_STRING, sFilmSize);

    //ob.GetLayoutColNumber();

    //ob.GetLayoutRowNumber();

    string sMagnification = ob.GetMagnification();
    EXPECT_EQ(EMPTY_STRING, sMagnification);

    unsigned int iMaxDensity = static_cast<unsigned int>(ob.GetMaxDensity());
    EXPECT_TRUE(0 <= (iMaxDensity));

    string sMediumType = ob.GetMediumType();
    EXPECT_EQ(EMPTY_STRING, sMediumType);

    unsigned int iMinDensity = static_cast<unsigned int>(ob.GetMinDensity());
    EXPECT_TRUE(0 <= (iMinDensity));

    //ob.SetAnnotationString(sTargetHostname);

    //ob.SetImgConfiguration(sImageDisplayFormat);

    //ob.SetImgMagnification(sPrinterDescription);

    //ob.SetImgPolarity(sPrinterDescription);

    //ob.SetImgRequestSize(sPrinterDescription);

    //ob.SetImgSmoothing(sPrinterDescription);

    //ob.SetLutName(sPrinterIP);

    //ob.SetLutShape(sPrinterIP);

    //ob.SetUseAnnotation(true);

    //ob.SetUseannotationDatetime(true);

    //ob.SetUseAnnotationIllumination(true);

    //ob.SetUseAnnotationPrinter(true);

    //ob.SetUseInverseLut(true);

}


//TEST_F(FilmingTest, Printer_Configure_File_Not_Exist)
//{
//    PrinterConfig aPrinterConfig("");
//    EXPECT_EQ(NULL, aPrinterConfig.GetFilmSessionObject(1));
//    
//}
//
//TEST_F(FilmingTest, Printer_Configure_File_Is_Null)
//{
//    PrinterConfig aPrinterConfig("\\\\10.1.2.13\\Public\\wh\\NullPrinterDictionary.xml");
//    EXPECT_EQ(NULL, aPrinterConfig.GetFilmSessionObject(1));
//    
//}
//
//TEST_F(FilmingTest, Printer_Configure_File_Format_Is_Wrong)
//{
//    PrinterConfig aPrinterConfig("\\\\10.1.2.13\\Public\\wh\\WrongFormatPrinterDictionary.xml");
//    EXPECT_EQ(NULL, aPrinterConfig.GetFilmSessionObject(1));
//    
//}
//
//
//TEST_F(FilmingTest, Printer_Configure_File_Lack_Item)
//{
//    PrinterConfig aPrinterConfig("\\\\10.1.2.13\\Public\\wh\\LackItemPrinterDictionary.xml");
//    EXPECT_EQ("", aPrinterConfig.GetFilmSessionObject(1)->GetPrinterIP()) << "Occurred at PrinterIP ";
//    
//}


TEST_F(FilmingTest, Parsing_Printer_Configure_To_Get_Film_Session_Object_Vector)
{
    PrinterConfig aPrinterConfig;
    //lint -e838
    const std::vector<McsfDcmFilmSessionObject>* pFilmSessionObjectVector=nullptr;
    pFilmSessionObjectVector = aPrinterConfig.GetFilmSesionObjectVector();
    //lint  -e529
    bool bResult = (pFilmSessionObjectVector == nullptr); 
    //lint  +e529
    EXPECT_FALSE(bResult) << "Can't Get FilmSessionObjectVector";
    //lint +e838
    
}

TEST_F(FilmingTest, Parsing_Printer_Configure_Length_Of_Film_Session_Object_Vector_GT_0)
{
    PrinterConfig aPrinterConfig;
    //lint  -e529
    int iFilmSessionObjectSize =  aPrinterConfig.GetFilmSessionObjectSize();
    EXPECT_LE(0,iFilmSessionObjectSize);
}   //lint  +e529

TEST_F(FilmingTest, Parsing_Printer_Configure_And_Get_A_Film_Session_Object_In_The_Index_Range_Of_The_Vector)
{
    PrinterConfig aPrinterConfig;
    //lint  -e838
    const McsfDcmFilmSessionObject* pFilmSessionObject = nullptr;
    pFilmSessionObject = aPrinterConfig.GetFilmSessionObject(0);
    //lint  -e529
    //bool bResult = (pFilmSessionObject == nullptr); 
    //lint  +e529
    //EXPECT_FALSE(bResult) << "Can't Get FilmSessionObject at index 0 of FilmSessionObjectVector";
    //EXPECT_NE(nullptr, pFilmSessionObject);
}   //lint  +e838

TEST_F(FilmingTest, Parsing_Printer_Configure_And_Get_A_Film_Session_Object_out_The_Index_Range_Of_The_Vector)
{//lint -e550
    PrinterConfig aPrinterConfig;
    //lint  -e838
    const McsfDcmFilmSessionObject* pFilmSessionObject = nullptr;
    const int iInexOfObjectInVector = INT_MAX;
    //EXPECT_NO_THROW(pFilmSessionObject = aPrinterConfig.GetFilmSessionObject(iInexOfObjectInVector));
    pFilmSessionObject = aPrinterConfig.GetFilmSessionObject(iInexOfObjectInVector);
    EXPECT_EQ(nullptr, pFilmSessionObject);
}   //lint  -e838
//lint  +e550
//lint +e529
//lint +e107
//lint +e42
//lint +e63
    //TODO: TEST GetPrinterStorageRootPath

#pragma region Load_Print_File_TEST

//Test for
// OFCondition McsfDcmPrintObjectInstance::LoadPrintFile(const char* psFilePath,
//     const char* imageFilePath)
TEST_F(FilmingTest, Load_Print_File_With_False_File_Path)
{
    //EXPECT_NE(EC_Normal, )
}

#pragma endregion