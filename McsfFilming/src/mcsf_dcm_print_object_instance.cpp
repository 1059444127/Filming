//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_dcm_print_object_interface.cpp
///  \brief   parameter init, use McsfDcmPrintObject class to create  
///           hardcopy grayscale object file. and insert DB.
///
///  \version 1.0
///  \date    Oct. 17, 2011
///  
//////////////////////////////////////////////////////////////////////////

#include "boost/filesystem.hpp" // for boost filesystem
#include "Common/McsfSystemEnvironmentConfig/mcsf_systemenvironment_factory.h"
#include "mcsf_filming_DB.h"

#include "mcsf_dcm_print_object_instance.h"
#include "dcmtk/dcmimage/diregist.h" 
#include "boost/lexical_cast.hpp"
#pragma warning(disable:4127)

MCSF_FILMING_BEGIN_NAMESPACE

    McsfDcmPrintObjectInstance::McsfDcmPrintObjectInstance(
    McsfPrintJobObject* pPrintJobObject,
    McsfFilmingDB* pFilmingDB)
    :IFilmingLibary(/*pPrintJobObject,pFilmingDB*/),
    McsfDcmPrintObject(pPrintJobObject),
    m_pPrintJobObject(NULL),
    m_pFilmingDB(NULL)
{
    try
    { 
		Initialize(pPrintJobObject, pFilmingDB);


    }
    catch (std::exception& e)
    {
        LOG_ERROR_FILMING(e.what());
    }
    catch(...) 
    {
        LOG_ERROR_FILMING("general exception");
    }
}


McsfDcmPrintObjectInstance::~McsfDcmPrintObjectInstance(void)
{
    //no need to delete the m_pPrintJobObject, because this transfer from outside.
    //so we can't free it!!
    //try
    //{
    //    if(NULL != m_pPrintJobObject)
    //    {
    //        delete m_pPrintJobObject;
    //        m_pPrintJobObject = NULL;
    //    }
    //}
    //catch (...)
    //{
    //    //for pc-lint, don't allow log in catch, that means this macro
    //    //will occur a new exception.
    //    //LOG_ERROR_FILMING("deconstructor McsfDcmPrintObjectInstance error!");
    //}
}

void McsfDcmPrintObjectInstance::GetStorageRootPath()
{
    try
    {
        //step1: get the FilmingConfig.xml full path
        std::string sFilePath("");
        ISystemEnvironmentConfig *pSysConfig = 
            ConfigSystemEnvironmentFactory::Instance()->GetSystemEnvironment();
        if (NULL != pSysConfig)
        {
            sFilePath = pSysConfig->GetApplicationPath("FilmingConfigPath");
        }

        if(sFilePath.empty())
        {
            LOG_ERROR_FILMING("GetStorageRootPath function get null path by using \
                              GetApplicationPath('FilmingConfigPath'),please check!!");
            return ;
        }

        sFilePath += MCSF_FILMING_CONFIG_FILE_NAME;
        LOG_INFO_FILMING("filming config file path is :" + sFilePath);

        //step2: parse the FilmingConfig.xml, get the 'PrintObjectStoragePath' value
        // and init the m_sFileRootDir.
        ParseFilmingConfig(sFilePath);
        //check directory path validity
        //if (false == boost::filesystem::is_directory(boost::filesystem::path(m_sFileRootDir)))
        //{
        //    LOG_WARN_FILMING("not get a valid print object root storage path, use \"D:/temp\" instead");
        //    if( (false ==boost::filesystem::is_directory(boost::filesystem::path("D:/temp")) ) && 
        //        (false == boost::filesystem::create_directory("D:/temp")) )
        //    {
        //        LOG_ERROR_FILMING("can't get a valid print object root storage path");
        //        return;
        //    }
        //    m_sFileRootDir = "c:\temp";
        //    return;
        //}

        delete pSysConfig;
        pSysConfig = NULL;
    }
    catch (std::exception& e)
    {
        LOG_ERROR_FILMING(e.what());
    }
    catch(...) 
    {
        LOG_ERROR_FILMING("general exception");
    }
}

void McsfDcmPrintObjectInstance::ParseFilmingConfig(const std::string& sFilmingConfigPath)
{
    try
    {
        Mcsf::IFileParser* m_pFileParser = Mcsf::ConfigParserFactory::Instance()->GetXmlFileParser();
        if (NULL == m_pFileParser)
        {
            LOG_ERROR_FILMING("Can't get FileParser instance");
            return;
        }
        m_pFileParser->Initialize();

        std::wstring wsConfigFilePath = m_pFileParser->ToWString(sFilmingConfigPath);

        std::string sLogStr(EMPTY_STRING);
        if (false == m_pFileParser->Validate(wsConfigFilePath))
        {
            sLogStr = "Printer Config File " + sFilmingConfigPath + " is not valid";
            LOG_WARN_FILMING(sLogStr);
            return ;
        }

        if (false == m_pFileParser->ParseByURI(wsConfigFilePath))
        {
            sLogStr = "Printer Config File " + sFilmingConfigPath + " has been parsed failed";
            LOG_WARN_FILMING(sLogStr);
            return ;
        }

        std::wstring wsPrintObjectStorageRootPath;

        //parse <PrintObjectStoragePath>
        if (false == m_pFileParser->GetStringValueByTag(L"PrintObjectStoragePath", &wsPrintObjectStorageRootPath) )
        {
            LOG_ERROR_FILMING(" error when parsing Tag  \"PrintObjectStoragePath\" ");
        }
        else
        {
            this->m_sFileRootDir = m_pFileParser->FromWString(wsPrintObjectStorageRootPath);
            if ( (false == boost::filesystem::exists(boost::filesystem::path(m_sFileRootDir)))
                && false == boost::filesystem::create_directory(boost::filesystem::path(m_sFileRootDir)))
            {
                LOG_ERROR_FILMING(string("can't create path: ") + m_sFileRootDir) ;
            }
            sLogStr = "stored print object root storage path is :" + m_sFileRootDir;
            LOG_INFO_FILMING(sLogStr);
        }

        m_pFileParser->Terminate();
    }
    catch (std::exception& e)
    {
        LOG_WARN_FILMING(e.what());
    }
    catch (...)
    {
        LOG_WARN_FILMING("Exception when Getting temporary printing object path");
    }
}

OFCondition McsfDcmPrintObjectInstance::LoadPrintFile(const char* psFilePath,
    const char* imageFilePath)
{
    std::string sLogString("");
    OFCondition status = EC_Normal;

    if (NULL != psFilePath)
    {
        LOG_INFO_FILMING("loading image file '"+std::string(imageFilePath)+
            "' with presentation state '"+std::string(psFilePath)+"'");

        status = loadPState(psFilePath, imageFilePath);
        if (EC_Normal != status)
        {
            LOG_ERROR_FILMING("loading image file '"+std::string(imageFilePath)+
                "' with presentation state '"+psFilePath+"' failed.");

            return status;
        }
    }
    else
    {
        sLogString = "loading image file '" + std::string(imageFilePath)+ "'";
        LOG_INFO_FILMING(sLogString);

        status = loadImage(imageFilePath);
        if (EC_Normal != status)
        {
            sLogString = "loading image file '"+std::string(imageFilePath)+"' failed.";
            LOG_ERROR_FILMING(sLogString);
            return status;
        }
    }

    return status;
}

int McsfDcmPrintObjectInstance::SaveHGfile()
{
    void *pixelData = NULL;
    unsigned long width = 0;
    unsigned long height = 0;
    unsigned long bitmapSize = 0;
    double pixelAspectRatio;
    // save grayscale hardcopy image.
    bitmapSize = getCurrentPState().getPrintBitmapSize();
    pixelData = new char[bitmapSize];
    if (pixelData)
    {
        if (EC_Normal != getCurrentPState().getPrintBitmapWidthHeight(width, height))
        {
            LOG_ERROR_FILMING("can't determine bitmap size");
            return 10;
        }
        if (EC_Normal != getCurrentPState().getPrintBitmap(pixelData, bitmapSize, m_pPrintJobObject->GetUseInverseLut()))
        {
            LOG_ERROR_FILMING("can't create print bitmap");
            return 10;
        }
        pixelAspectRatio = getCurrentPState().getPrintBitmapPixelAspectRatio();

        /*if (cmd.findOption("--overlay", 0, OFCommandLine::FOM_First))
        {
        do {
        const char *fn = NULL;
        OFCmdUnsignedInt x, y;
        app.checkValue(cmd.getValue(fn));
        app.checkValue(cmd.getValue(x));
        app.checkValue(cmd.getValue(y));
        if (fn != NULL)
        addOverlay(fn, x, y, OFstatic_cast(Uint16 *, pixelData), width, height, OFstatic_cast(unsigned int, opt_ovl_graylevel));
        } while (cmd.findOption("--overlay", 0, OFCommandLine::FOM_Next));
        }*/
		if(m_pPrintJobObject->GetIfColorPrint())
		{
			LOG_INFO_FILMING("writing DICOM color hardcopy image file.");
			if (EC_Normal != saveColorImage(pixelData, width, height, pixelAspectRatio))
			{
				LOG_ERROR_FILMING("error during creation of DICOM grayscale hardcopy image file");
				return 10;
			}
		}
		else
		{
			LOG_INFO_FILMING("writing DICOM grayscale hardcopy image file.");
			if (EC_Normal != saveHardcopyGrayscaleImage(pixelData, width, height, pixelAspectRatio))
			{
				LOG_ERROR_FILMING("error during creation of DICOM grayscale hardcopy image file");
				return 10;
			}
		}

        delete[] OFstatic_cast(char *, pixelData);
    } 
    else 
    {
        LOG_ERROR_FILMING("out of memory error: cannot allocate print bitmap");
        return 10;
    }
    return 0;
}

void McsfDcmPrintObjectInstance::SetLut()
{
    std::string sLogString("");

    if (this->m_pPrintJobObject->GetLutName().length()>0)
    {
        if (EC_Normal != selectDisplayPresentationLUT(this->m_pPrintJobObject->GetLutName().c_str()))
        {
            sLogString = "cannot set requested presentation LUT '"+
                this->m_pPrintJobObject->GetLutName()+"', ignoring.";
            LOG_WARN_FILMING(sLogString);
        }
    } 
    else 
    {
        // in the case of a Presentation LUT Shape, we set the shape inside
        // the GSPS object to default (corresponding to IDENTITY for MONOCHROME2
        // and INVERSE for MONOCHROME1). This will leave our image data unaltered.
        // The LIN OD shape is only activated in the print handler, not the GSPS.
        if ((m_pPrintJobObject->GetLutShape() == 1) || (m_pPrintJobObject->GetLutShape() == 2))
        {
            if (getCurrentPState().setDefaultPresentationLUTShape().bad())
            {
                LOG_WARN_FILMING("cannot set presentation LUT shape, ignoring.");
            }
            if((m_pPrintJobObject->GetLutShape() == 2))
            {
                if (getCurrentPState().setCurrentPresentationLUT(DVPSP_lin_od).bad())
                {
                    LOG_WARN_FILMING("cannot set presentation LUT shape, ignoring.");
                }
            }
        }
    }
}

void McsfDcmPrintObjectInstance::SetAnnotation()
{
    std::string sLogString("");
    const char *currentPrinter = getCurrentPrinter();
    // set annotations

    if (m_pPrintJobObject->GetUseAnnotation())
    {
        if (m_pPrintJobObject->GetTargetPrinterSupportsAnnotation())
        {
            setActiveAnnotation(OFTrue);
            setPrependDateTime(m_pPrintJobObject->GetUseannotationDatetime());
            setPrependPrinterName(m_pPrintJobObject->GetUseAnnotationPrinter());
            setPrependLighting(m_pPrintJobObject->GetUseAnnotationIllumination());
            setAnnotationText(m_pPrintJobObject->GetAnnotationString().c_str());
        } 
        else 
        {
            sLogString = "printer '" + std::string(currentPrinter) + 
                "' does not support annotations, ignoring.";
            LOG_WARN_FILMING(sLogString);
            setActiveAnnotation(OFFalse);
        }
    } else 
    {
        setActiveAnnotation(OFFalse);
    }
}
OFCondition McsfDcmPrintObjectInstance::SetPrintHandlerInfo()
{
    OFCondition status = EC_Normal;
    size_t numImages = getPrintHandler().getNumberOfImages();
    for (size_t i=0; i<numImages; i++)
    {
        if(m_pPrintJobObject->GetOrientation()==0)
        {
            status = getPrintHandler().setFilmOrientation(DVPSF_portrait);
        }
        else if(m_pPrintJobObject->GetOrientation()==1)
        {
            status = getPrintHandler().setFilmOrientation(DVPSF_landscape);
        }
        else
        {
            status = getPrintHandler().setFilmOrientation(DVPSF_default);
        }
    }
    if (numImages > 0)
    {
        // save the SP file to file system
        LOG_INFO_FILMING("writing DICOM stored print object to file system.");
        if (EC_Normal != HandlePrintingImageSetInfo())
        {
            LOG_ERROR_FILMING("error during creation of DICOM stored print object");
        }
    }

    return status;
}
OFCondition McsfDcmPrintObjectInstance::SaveSPfile()
{
    OFCondition status = EC_Normal;

    size_t numImages = getPrintHandler().getNumberOfImages();
    for (size_t i=0; i<numImages; i++)
    {
        if(m_pPrintJobObject->GetOrientation()==0)
        {
            status = getPrintHandler().setFilmOrientation(DVPSF_portrait);
        }
        else if(m_pPrintJobObject->GetOrientation()==1)
        {
            status = getPrintHandler().setFilmOrientation(DVPSF_landscape);
        }
        else
        {
            status = getPrintHandler().setFilmOrientation(DVPSF_default);
        }
    }

    if (numImages > 0)
    {
        // save the SP file to file system
        LOG_INFO_FILMING("writing DICOM stored print object to file system.");
        if (EC_Normal != saveStoredPrint(true))
        {
            LOG_ERROR_FILMING("error during creation of DICOM stored print object");
        }
    }

    return status;
}

int McsfDcmPrintObjectInstance::CreatePrintObject()
{
    std::string sLogString("");
    unsigned long lColumns = static_cast<unsigned long>(m_pPrintJobObject->GetLayoutColNumber());
    unsigned long lRows = static_cast<unsigned long>(m_pPrintJobObject->GetLayoutRowNumber());
    getPrintHandler().setImageDisplayFormat(lColumns,lRows);
    getPrintHandler().setFilmSizeID(m_pPrintJobObject->GetFilmSize().c_str());

    std::string sCurrentImage = EMPTY_STRING;

    OFCondition status = EC_Normal;

    int iFileCount = static_cast<int>(m_FileNameList.size());
    for(int i=0; i<iFileCount;i++)
    {
        sCurrentImage = m_FileNameList[i];

        if (!sCurrentImage.empty())
        {
            status = LoadPrintFile(NULL,sCurrentImage.c_str());
            if(status != EC_Normal)
            {
                return -1;
            }
            
            SetLut();
			HandlePrintingImage();
        } 
        else 
        {
            LOG_WARN_FILMING("internal error - odd number of filenames");
            continue;
        }
    }

    if(status == EC_Normal)
    {
        SetAnnotation();
    }
    if (status == EC_Normal)
    {
        status = SetPrintHandlerInfo();
    }
    return (status != EC_Normal) ? -1 : 0;

}

void McsfDcmPrintObjectInstance::DeletePrintObjectFile()
{
    try
    {
        std::string sLogStr(EMPTY_STRING);
        bool bRet = false;

        sLogStr = "SP file"+m_sSPfilePath+" don't exists!";
        if (boost::filesystem::exists(this->m_sSPfilePath))
        {
            sLogStr = "delete SP file"+m_sSPfilePath+" failed!";
            bRet = boost::filesystem::remove(this->m_sSPfilePath);
            if(bRet != true)
            {
                LOG_ERROR_FILMING(sLogStr);
            }
            else
            {
                sLogStr = "delete SP file"+m_sSPfilePath+" success!";
                LOG_INFO_FILMING(sLogStr);
            }
        }
        else
        {
            LOG_WARN_FILMING(sLogStr);
        }

        if(this->m_sHGfilePathList.size()>0)
        {
            for(int i=0; i<m_sHGfilePathList.size();i++)
            {
                sLogStr = "HG file"+m_sHGfilePathList[i]+" don't exists!";
                if(boost::filesystem::exists(m_sHGfilePathList[i]))
                {
                    sLogStr = "delete HG file"+m_sHGfilePathList[i]+" failed!";
                    bRet = boost::filesystem::remove(m_sHGfilePathList[i]);
                    if(bRet != true)
                    {
                        LOG_ERROR_FILMING(sLogStr);
                    }
                    else
                    {
                        sLogStr = "delete HG file"+m_sHGfilePathList[i]+" success!";
                        LOG_INFO_FILMING(sLogStr);
                    }
                }
                else
                {
                    LOG_WARN_FILMING(sLogStr);
                }
            }
        }
        else
        {
            LOG_WARN_FILMING("there is no HG file to delete!");
        }
    }
    catch(...)
    {
        LOG_ERROR_FILMING("exception occur: delete SP, HG file.");
    }
}

//bool McsfDcmPrintObjectInstance::InsertPrintObjectToDB()
//{
//    try
//    {
//        //should be create outside, if NULL ,just return
//        if(NULL == m_pFilmingDB)
//        {
//            return false;
//        }
//
//        if(NULL != m_pFilmingDB)
//        {
//            // step 1: insert SP file to DB
//            int iSPfileID = m_pFilmingDB->InsertSPFilePathToDB(m_pPrintJobObject->GetJobID(),
//                this->GetSPfilePath());
//
//            if(iSPfileID < 0)
//            {
//                LOG_ERROR_FILMING("insert sp file to DB failed!");
//                return false;
//            }
//
//            // step 2: insert HG file to DB
//            int iHgFileCounts = this->GetHGfilePathList().size();
//            for(int i = 0; i < iHgFileCounts; i++)
//            {
//                int iRet = 0;
//                iRet = m_pFilmingDB->InsertHGFilePathToDB(iSPfileID,this->GetHGfilePathList()[i]);
//                if(iRet != 0)
//                {
//                    //don't return ,continue loop ...
//                    std::string sLogString = "insert file"+this->GetHGfilePathList()[i]+" to db failed!";
//                    LOG_ERROR_FILMING(sLogString);
//                }
//            }
//        }
//        else
//        {
//            LOG_ERROR_FILMING("NULL m_pFilmingDB!!");
//            return false;
//        }
//    }
//    catch (...)
//    {
//        LOG_ERROR_FILMING("exception occur:insert sp file to DB failed!");
//        return false;
//    }
//
//    return true;
//}

//////////////////////////////////////////print-..//////////////////////
OFCondition McsfDcmPrintObjectInstance::LoadSPfile(std::string sSPfilePath, DVPSStoredPrint& stprint)
{
    std::string sLogString(EMPTY_STRING);

    DcmFileFormat *ffile = NULL;
    DcmDataset *dset = NULL;

    if (m_sSPfilePath.empty())
    {
        LOG_WARN_FILMING("sp file path is null!");
        return EC_IllegalCall;
    }

    OFCondition result = DVPSHelper::loadFileFormat(m_sSPfilePath.c_str(), ffile);
    if (EC_Normal != result)
    {
        sLogString = "filming: unable to load file '" + std::string(m_sSPfilePath) + "'";
        LOG_ERROR_FILMING(sLogString);
    }
    if (ffile) 
        dset = ffile->getDataset();

    if (EC_Normal == result)
    {
        if (dset) 
            result = stprint.read(*dset); 
        else 
            result = EC_IllegalCall;
    }
    if (EC_Normal != result)
    {
        sLogString = "file '" + std::string(m_sSPfilePath) +
            "' is not a valid Stored Print object";
        LOG_ERROR_FILMING(sLogString);
    }
    delete ffile;

    return result;
}

OFCondition McsfDcmPrintObjectInstance::ConnectToPrinter()
{
    std::string sLogString(EMPTY_STRING);
    OFCondition result = EC_Normal;
    LOG_INFO_FILMING("Connect to Printer with:");
    sLogString = "IP:"+m_pPrintJobObject->GetTargetHostName();
    LOG_INFO_FILMING(sLogString);
    sLogString = "Port:"+m_pPrintJobObject->GetTargetPort();
    LOG_INFO_FILMING(sLogString);
    sLogString = "AE:"+m_pPrintJobObject->GetTargetAETitle();
    LOG_INFO_FILMING(sLogString);

	int iNegResult(4);    //0_BasicGrayscalePrintManagementSopClassOnly,1_BasicColorPrintManagementSopClassOnly,2_BothGrayScaleAndColor,3_NeitherGrayscaleNorColor 
    int iBasicGrayscaleColorControl(0);
    if(m_pPrintJobObject->GetIfColorPrint())
        iBasicGrayscaleColorControl = 1;
    result = printHandler.negotiateAssociation(
        NULL, m_pPrintJobObject->GetMyAETitle().c_str(),
        m_pPrintJobObject->GetTargetAETitle().c_str(), 
        m_pPrintJobObject->GetTargetHostName().c_str(), 
        m_pPrintJobObject->GetTargetPort(), 
        m_pPrintJobObject->GetTargetMaxPDU(),
        m_pPrintJobObject->GetTargetSupportsPLUT(), 
        m_pPrintJobObject->GetTargetSupportsAnnotation(),
        m_pPrintJobObject->GetTargetImplicitOnly(), 
        iBasicGrayscaleColorControl,
        &iNegResult);
    if(result.bad())
    {
        ReleaseAssociation();
        result = printHandler.negotiateAssociation(
            NULL, m_pPrintJobObject->GetMyAETitle().c_str(),
            m_pPrintJobObject->GetTargetAETitle().c_str(), 
            m_pPrintJobObject->GetTargetHostName().c_str(), 
            m_pPrintJobObject->GetTargetPort(), 
            m_pPrintJobObject->GetTargetMaxPDU(),
            m_pPrintJobObject->GetTargetSupportsPLUT(), 
            m_pPrintJobObject->GetTargetSupportsAnnotation(),
            m_pPrintJobObject->GetTargetImplicitOnly(), 
            iBasicGrayscaleColorControl,
            &iNegResult);
    }
    if(iNegResult == 4) LOG_ERROR_FILMING("Function negotiateAssociation get support color failed!");
    if(!result.bad())
    {
	    if(m_pPrintJobObject->GetIfColorPrint())
	    {
            printHandler.setUseGrayscaleSopClass(false);
            if(iNegResult == 1||iNegResult == 2)
            {
                std::string sLogStringFailed = "printer support color print\n";
                LOG_INFO_FILMING(sLogStringFailed);
            }
            else
            {
                std::string sLogStringFailed = "printer does not support color print\n ";
                LOG_ERROR_FILMING(sLogStringFailed);
                result = EC_IllegalParameter;
                return result;
            }
	    }
	    else
	    {
	    	printHandler.setUseGrayscaleSopClass(true);
            if(iNegResult == 0||iNegResult == 2)
            {
                std::string sLogStringFailed = "printer support gray print\n";
                LOG_INFO_FILMING(sLogStringFailed);
            }
	    	else
	    	{
                std::string sLogStringFailed = "printer does not support gray print\n";
                LOG_ERROR_FILMING(sLogStringFailed);
                result = EC_IllegalParameter;
                return result;
	    	}
	    }
    }
    if (result.bad())
    {
        OFString temp_str;
        OFString ofsDump = DimseCondition::dump(temp_str, result);
        std::string sLogStringFailed = "connection setup with printer failed\n"+
            std::string(temp_str.c_str());
        LOG_ERROR_FILMING(sLogStringFailed);
    }
    else
    {
        sLogString = "connection to printer: "+sLogString;
        LOG_INFO_FILMING(sLogString);
    }

    return result;
}

OFCondition McsfDcmPrintObjectInstance::CreateSession(DVPSStoredPrint& stprint)
{
    OFCondition result = EC_Normal;

    result = stprint.printSCUpreparePresentationLUT(
        printHandler, 
        m_pPrintJobObject->GetTargetRequiresMatchingLUT(), 
        m_pPrintJobObject->GetTargetPreferSCPLUTRendering(),
        m_pPrintJobObject->GetTargetSupports12bit());

    if (EC_Normal != result)
        LOG_ERROR_FILMING("printer communication failed, unable to create presentation LUT");

    if (EC_Normal==result) 
    {
        result = printSCUcreateBasicFilmSession(
            printHandler, 
            m_pPrintJobObject->GetTargetPLUTinFilmSession());

        if (EC_Normal != result)
            LOG_ERROR_FILMING("printer communication failed, unable to create basic film session");
    }

    //if (EC_Normal==result) 
    //{
    //    result = stprint.printSCUcreateBasicFilmBox(
    //        printHandler, 
    //        m_pPrintJobObject->GetTargetPLUTinFilmSession());

    //    if (EC_Normal != result)
    //        LOG_ERROR_FILMING("printer communication failed, unable to create basic film box");
    //}

    return result;
}

OFCondition McsfDcmPrintObjectInstance::SetFilmBox(DVPSStoredPrint& stprint)
{
    std::string sLogString(EMPTY_STRING);
    OFCondition result = EC_Normal;

    size_t numberOfImages = 1;// stprint.getNumberOfImages();
    const char *studyUID = NULL;
    const char *seriesUID = NULL;
    const char *instanceUID = NULL;
    const char *imagefile = NULL;
    OFString theFilename;
    DicomImage *dcmimage = NULL;
    for (size_t currentImage=0; currentImage<numberOfImages; currentImage++)
    {
        result = stprint.getImageReference(currentImage, studyUID, seriesUID, instanceUID);

        if (studyUID && seriesUID && instanceUID)
        {
            imagefile = this->m_FileNameList[currentImage].c_str();

            if (imagefile)
                theFilename = imagefile; 
            else 
                theFilename.clear();

            if (theFilename.size() > 0)
            {
                dcmimage = new DicomImage(theFilename.c_str());
                //if (dcmimage && (EIS_Normal == dcmimage->getStatus()))
				if(dcmimage)
                {
                    // N-SET basic image box
                    result = stprint.printSCUsetBasicImageBox(
                        printHandler, 
                        currentImage, 
                        *dcmimage, 
                        m_pPrintJobObject->GetMonochrome1());
                    if (EC_Normal != result)
                    {
                        OFString temp_str;
                        OFString ofsDump = DimseCondition::dump(temp_str, result);
                        std::string sLogStringFailed = "printer communication failed,unable to transmit basic grayscale image box"+
                            std::string(temp_str.c_str());
                        LOG_ERROR_FILMING(sLogStringFailed);
                    }
                } 
                else 
                {
					result = EC_IllegalCall;
					sLogString = "unable to load image file '"+
						std::string(theFilename.c_str())+ "'";
					LOG_ERROR_FILMING(sLogString);
                }
                delete dcmimage;
            } 
            else 
            {
                result = EC_IllegalCall;
                LOG_ERROR_FILMING("unable to locate image file in database");
            }
        } 
        else 
            result = EC_IllegalCall;
    }

    return result;
}

OFCondition McsfDcmPrintObjectInstance::SetAnnotationBox(DVPSStoredPrint& stprint)
{
    OFCondition result = EC_Normal;

    size_t numberOfAnnotations = stprint.getNumberOfAnnotations();
    for (size_t currentAnnotation=0; currentAnnotation<numberOfAnnotations; currentAnnotation++)
    {
        // N-SET basic annotation box
        result = stprint.printSCUsetBasicAnnotationBox(printHandler, currentAnnotation);
        if (EC_Normal != result)
        {
            LOG_ERROR_FILMING("printer communication failed, unable to transmit basic annotation box");
        }
    }

    return result;
}

OFCondition McsfDcmPrintObjectInstance::PrintFilmSession(DVPSStoredPrint& stprint)
{
    OFCondition result = EC_Normal;

    if (!m_pPrintJobObject->m_bNoPrint)
    {
        //if (m_pPrintJobObject->GetSessionPrint())
        //{
        //    result = stprint.printSCUprintBasicFilmSession(printHandler);
        //    if (EC_Normal != result)
        //    {
        //        LOG_ERROR_FILMING("printer communication failed, unable to print (at film session level)");
        //    }
        //} 
        //else 
        //{
        //    result = stprint.printSCUprintBasicFilmBox(printHandler);
        //    if (EC_Normal != result)
        //    {
        //        LOG_ERROR_FILMING("printer communication failed, unable to print");
        //    }
        //}

		result = stprint.printSCUprintBasicFilmBox(printHandler);
		if (EC_Normal != result)
		{
			LOG_ERROR_FILMING("printer communication failed, unable to print");
		}
    }

    return result;
}

OFCondition McsfDcmPrintObjectInstance::DeleteFilmSession(DVPSStoredPrint& stprint)
{
    OFCondition result = EC_Normal;

    result = stprint.printSCUdelete(printHandler);
    if (EC_Normal != result)
    {
        LOG_ERROR_FILMING("printer communication failed, unable to delete print objects");
    }

    return result;
}

OFCondition McsfDcmPrintObjectInstance::ReleaseAssociation(/*DVPSStoredPrint& stprint*/)
{
    std::string sLogString(EMPTY_STRING);
    OFCondition result = EC_Normal;

    result = printHandler.releaseAssociation();
    if (result.bad())
    {
        OFString temp_str;
        OFString ofsDump = DimseCondition::dump(temp_str, result);
        sLogString = "release of connection to printer failed\n"+
            std::string(temp_str.c_str());
        LOG_ERROR_FILMING( sLogString );

        if (EC_Normal == result) 
        {
            result = EC_IllegalCall;
        }
    }

    return result;
}

/// \fn int McsfDcmPrintObjectInstance::DoPrint()
/// <key> \n
/// PRA:Yes \n
/// Traced from:SSFS_PRA_Filming_PostproFilmPrintError \n
/// Description:Print workflow \n
/// Short Description:DoPrint \n
/// Component:Filming \n
/// </key> \n
///
int McsfDcmPrintObjectInstance::DoPrint()
{
    try
    {
        DVPSStoredPrint& stprint = getPrintHandler();

        //---------------------Connect Printer And Create Session-------------------------
        OFCondition result = EC_Normal;
        // Creat Image box
        
        if (EC_Normal==result) 
        {
            stprint.filmBoxInstanceUID = "";
            result = stprint.printSCUcreateBasicFilmBox(
                printHandler, 
                m_pPrintJobObject->GetTargetPLUTinFilmSession());
            if (EC_Normal != result)
            {
                LOG_ERROR_FILMING("printer communication failed, unable to create basic film box");
                LOG_SVC_ERROR_LOGUID(SVCLOGUID_SOURCE, SVCLOGUID_ERROR_COMMUNICATION, "printer communication failed, unable to create basic film box");
                stprint.filmBoxInstanceUID.clear();
                return -1;
            }
        }

        // Process images
        if(EC_Normal != SetFilmBox(stprint))
        {
            LOG_ERROR_FILMING("SetFilmBox Failure");
            LOG_SVC_ERROR_LOGUID(SVCLOGUID_SOURCE, SVCLOGUID_ERROR_COMMUNICATION, "SetFilmBox Failure");
            stprint.filmBoxInstanceUID.clear();
            return -1;
        }
        else
        {
            LOG_INFO_FILMING("Set Film Box Success");
        }

        //set annotation box
        if(EC_Normal != SetAnnotationBox(stprint))
        {
            LOG_ERROR_FILMING("SetAnnotationBox Failure");
            LOG_SVC_ERROR_LOGUID(SVCLOGUID_SOURCE, SVCLOGUID_ERROR_COMMUNICATION, "SetAnnotationBox Failure");
            stprint.filmBoxInstanceUID.clear();
            return -1;
        }
        else
        {
            LOG_INFO_FILMING("Set Annotation Box Success");
        }

        //print film session
        if(EC_Normal != PrintFilmSession(stprint))
        {
            LOG_ERROR_FILMING("PrintFilmSession Failure");
            LOG_SVC_ERROR_LOGUID(SVCLOGUID_SOURCE, SVCLOGUID_ERROR_COMMUNICATION, "PrintFilmSession Failure");
            stprint.filmBoxInstanceUID.clear();
            return -1;
        }
        else
        {
            LOG_INFO_FILMING("Print Film Session Success");
        }

	    // delete basic film box
	    if (stprint.filmBoxInstanceUID.size() > 0)
	    {
	    	Uint16 status=0;

            OFCondition cond = printHandler.deleteRQ(UID_BasicFilmBoxSOPClass, stprint.filmBoxInstanceUID.c_str(), status);
            if ((cond.bad())||((status!=0)&&((status & 0xf000)!=0xb000))) 
            {
                stprint.filmBoxInstanceUID.clear();
                return -1;
            }
            stprint.filmBoxInstanceUID.clear();
        }

        return 0;
    }
    catch (...)
    {
        
        return -1;
    }


}

int McsfDcmPrintObjectInstance::DisConnectPrinter()
{
	try
	{
		DVPSStoredPrint& stprint = getPrintHandler();
	
		//delete the print job 
		if (EC_Normal != DeleteFilmSession(stprint))
		{
			LOG_ERROR_FILMING("DeleteFilmSession Failure");
			//return -1;
		}
		else
		{
			LOG_INFO_FILMING("Delete Film Session Success");
		}
	
		//release association
		if (EC_Normal != ReleaseAssociation())
		{
			LOG_ERROR_FILMING("ReleaseAssociation Failure");
			//return -1;
		}
		else
		{
			LOG_INFO_FILMING("Release Association Success");
		}
	
		return 0;
	}
	catch (...)
	{
		return -1;
	}
}

void McsfDcmPrintObjectInstance::SetSetFilmBoxTimeOut(int time)
{
	//设置连接超时机制 单位为s
    printHandler.setTimeout(DIMSE_NONBLOCKING,time);
	LOG_INFO_FILMING("Set Film Box TimeOut : "+ boost::lexical_cast<std::string>(time));
}
int McsfDcmPrintObjectInstance::ConnectPrinter()
{
	DVPSStoredPrint& stprint = getPrintHandler();

	if (ConnectToPrinter().bad())
	{
		LOG_ERROR_FILMING("Can't connect to printer");
		stringstream s;
		s << "Can't connect to printer "
			<< m_pPrintJobObject->GetTargetAETitle()
			<< "(IP: "
			<< m_pPrintJobObject->GetPrinterIP()
			<< " , Port: "
			<< m_pPrintJobObject->GetPrinterPort() 
			<< ")";
		//LOG_SVC_WARN_FILMING(s.str());
		LOG_SVC_ERROR_LOGUID(SVCLOGUID_SOURCE, SVCLOGUID_ERROR_CONNECTPRINTER, s.str());
		return -1;
	}
	else
	{
		LOG_INFO_FILMING("Connect to Printer Success");
	}

	if (EC_Normal != stprint.printSCUgetPrinterInstance(printHandler))
	{
		LOG_ERROR_FILMING("printer communication failed, unable to request printer settings");
		ReleaseAssociation();
		return -1;
	}

	//create session....
    if (EC_Normal != CreateSession(stprint))
    {
        LOG_ERROR_FILMING("CreateSession Failure");
        //LOG_SVC_ERROR_FILMING("CreateSession Failure, Maybe selected film size not supported");
        ReleaseAssociation();
        return -1;
	}
	else
	{
		LOG_INFO_FILMING("Create Session Success");
	}

	return 0;
}

void McsfDcmPrintObjectInstance::Initialize( McsfPrintJobObject* pPrintJobObject, McsfFilmingDB* pFilmingDB )
{
	this->m_pPrintJobObject = pPrintJobObject;
	this->m_pFilmingDB = pFilmingDB;

	//set SP,HG file storage root path
	//this->m_sFileRootDir = pPrintJobObject->GetStorageRootPath();
	GetStorageRootPath();

	//dicom image file list
	this->m_FileNameList = pPrintJobObject->GetFileNameList();

	//SP and HG file path
	this->m_sSPfilePath = this->m_pPrintJobObject->GetSPfilePath();
	m_sHGfilePathList = this->m_pPrintJobObject->GetHGfilePathList();


	std::string sLogString("");
	DVPSStoredPrint& stprint = getPrintHandler();
	if(EC_Normal != LoadSPfile(m_sSPfilePath,stprint) )
	{
		LOG_ERROR_FILMING("LoadSFile fail");
		//return -1;
	}
}


MCSF_FILMING_END_NAMESPACE
