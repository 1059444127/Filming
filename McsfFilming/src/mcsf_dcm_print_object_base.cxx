//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_dcm_print_object.cxx
///  \brief   wrapper DVInterface class of DCMTK. DICOM print object class. 
///           create stored print object and hardcopy grayscale object file.
///
///  \version 1.0
///  \date    Oct. 17, 2011
///  
//////////////////////////////////////////////////////////////////////////

#include "mcsf_dcm_print_object_base.h"
#include "mcsf_miscellaneous_config.h"

#include "dcmtk/config/osconfig.h"    /* make sure OS specific configuration is included first */
#include "dcmtk/dcmpstat/dviface.h"

#include "dcmtk/dcmpstat/dvpsdef.h"   /* for constants */
#include "dcmtk/ofstd/ofstring.h"     /* for class OFString */
#include "dcmtk/ofstd/ofbmanip.h"     /* for OFBitmanipTemplate */
#include "dcmtk/ofstd/ofdatime.h"     /* for OFDateTime */
#include "dcmtk/ofstd/oflist.h"       /* for class OFList */
#include "dcmtk/ofstd/ofstream.h"
#include "dcmtk/ofstd/ofcast.h"

#include "dcmtk/dcmimgle/digsdfn.h"   /* for DiGSDFunction */
#include "dcmtk/dcmimgle/diciefn.h"   /* for DiCIELABFunction */
#include "dcmtk/dcmnet/diutil.h"      /* for DU_getStringDOElement */
#include "dcmtk/dcmpstat/dvpssp.h"    /* for class DVPSStoredPrint */
#include "dcmtk/dcmpstat/dvpshlp.h"   /* for class DVPSHelper */
#include "dcmtk/dcmimgle/dcmimage.h"  /* for class DicomImage */
#include "dcmtk/dcmpstat/dvsighdl.h"  /* for class DVSignatureHandler */
#include "dcmtk/dcmsign/dcsignat.h"   /* for class DcmSignature */
#include "dcmtk/dcmsr/dsrdoc.h"       /* for class DSRDocument */
#include "dcmtk/dcmsr/dsrcodvl.h"     /* for class DSRCodedEntryValue */
#include "dcmtk/oflog/fileap.h"       /* for log4cplus::FileAppender */

#include "dcmtk/dcmpstat/dvpsib.h"    /* for DVPSImageBoxContent, needed by MSVC5 with STL */
#include "dcmtk/dcmpstat/dvpsab.h"    /* for DVPSAnnotationContent, needed by MSVC5 with STL */
#include "dcmtk/dcmpstat/dvpsov.h"    /* for DVPSOverlay, needed by MSVC5 with STL */
#include "dcmtk/dcmpstat/dvpsgl.h"    /* for DVPSGraphicLayer, needed by MSVC5 with STL */
#include "dcmtk/dcmpstat/dvpsal.h"    /* for DVPSOverlayCurveActivationLayer, needed by MSVC5 with STL */
#include "dcmtk/dcmpstat/dvpsga.h"    /* for DVPSGraphicAnnotation, needed by MSVC5 with STL */
#include "dcmtk/dcmpstat/dvpscu.h"    /* for DVPSCurve, needed by MSVC5 with STL */
#include "dcmtk/dcmpstat/dvpsvl.h"    /* for DVPSVOILUT, needed by MSVC5 with STL */
#include "dcmtk/dcmpstat/dvpsvw.h"    /* for DVPSVOIWindow, needed by MSVC5 with STL */
#include "dcmtk/dcmpstat/dvpsda.h"    /* for DVPSDisplayedArea, needed by MSVC5 with STL */
#include "dcmtk/dcmpstat/dvpssv.h"    /* for DVPSSoftcopyVOI, needed by MSVC5 with STL */
#include "dcmtk/dcmpstat/dvpsrs.h"    /* for DVPSReferencedSeries, needed by MSVC5 with STL */
#include "dcmtk/dcmpstat/dvpstx.h"    /* for DVPSTextObject, needed by MSVC5 with STL */
#include "dcmtk/dcmpstat/dvpsgr.h"    /* for DVPSGraphicObject, needed by MSVC5 with STL */
#include "dcmtk/dcmpstat/dvpsri.h"    /* for DVPSReferencedImage, needed by MSVC5 with STL */
#include "dcmtk/dcmqrdb/dcmqrdbi.h"   /* for DB_UpperMaxBytesPerStudy */
#include "dcmtk/dcmqrdb/dcmqrdbs.h"   /* for DcmQueryRetrieveDatabaseStatus */

#define INCLUDE_CSTDIO
#define INCLUDE_CCTYPE
#define INCLUDE_CMATH
#define INCLUDE_UNISTD
#include "dcmtk/ofstd/ofstdinc.h"

BEGIN_EXTERN_C
#ifdef HAVE_SYS_TYPES_H
#include <sys/types.h>   /* for fork */
#endif
#ifdef HAVE_SYS_WAIT_H
#include <sys/wait.h>    /* for waitpid */
#endif
#ifdef HAVE_SYS_TIME_H
#include <sys/time.h>    /* for wait3 */
#endif
#ifdef HAVE_SYS_RESOURCE_H
#include <sys/resource.h> /* for wait3 */
#endif
#ifdef HAVE_SYS_STAT_H
#include <sys/stat.h>    /* for stat, fstat */
#endif
#ifdef HAVE_SYS_UTIME_H
#include <sys/utime.h>   /* for utime */
#endif
#ifdef HAVE_UTIME_H
#include <utime.h>       /* for utime */
#endif
    END_EXTERN_C

#ifdef HAVE_WINDOWS_H
#include <windows.h>
#include <winbase.h>     /* for CreateProcess */
#endif

#ifdef WITH_OPENSSL
#include "dcmtk/dcmtls/tlstrans.h"
#include "dcmtk/dcmtls/tlslayer.h"

    BEGIN_EXTERN_C
#include <openssl/evp.h>
#include <openssl/x509.h>
#include <openssl/pem.h>
#include <openssl/err.h>
    END_EXTERN_C
#endif

#pragma warning(disable:4127)
#pragma warning(disable:4100)

    MCSF_FILMING_BEGIN_NAMESPACE

    McsfDcmPrintObject::McsfDcmPrintObject(McsfPrintJobObject* pPrintJobObject)
    : pPrint(NULL)
    , pState(NULL)
    , pStoredPState(NULL)
    , pDicomImage(NULL)
    , pDicomPState(NULL)
    , pHardcopyImage(NULL)
    , minimumPrintBitmapWidth(0)
    , minimumPrintBitmapHeight(0)
    , maximumPrintBitmapWidth(0)
    , maximumPrintBitmapHeight(0)
    , maximumPrintPreviewWidth(0)
    , maximumPrintPreviewHeight(0)
    , maximumPreviewImageWidth(0)
    , maximumPreviewImageHeight(0)
    , currentPrinter()
    , displayCurrentLUTID()
    , printCurrentLUTID()
    , printerMediumType()//"CLEAR FILM")
    , printerFilmDestination()//OFString("BIN_1"))
    , printerFilmSessionLabel()//OFString("PRINT"))
    , printerNumberOfCopies(1)
	, printerPriority()//OFString("MED"))
    , printerOwnerID()//OFString("UIH"))
    , activateAnnotation(OFFalse)
    , prependDateTime(OFTrue)
    , prependPrinterName(OFTrue)
    , prependLighting(OFTrue)
    , annotationText()
{
	MiscellaneousConfig miscellaneousConfig;
	//printerMediumType = OFString(miscellaneousConfig.GetMediaType().c_str());
	//printerFilmDestination = OFString(miscellaneousConfig.GetFilmDestination().c_str());
	printerFilmSessionLabel = OFString(miscellaneousConfig.GetFilmSessionLabel().c_str());
	printerPriority = OFString(miscellaneousConfig.GetFilmPriority().c_str());
	printerOwnerID = OFString(miscellaneousConfig.GetOwnerID().c_str());

	m_pPrintJobObject = pPrintJobObject;

	printerMediumType = OFString(m_pPrintJobObject->GetMediumType().c_str());
	printerFilmDestination = OFString(m_pPrintJobObject->GetDestination().c_str());

    /* initialize display transform (only on low-cost systems) */
    for (int i = DVPSD_first; i < DVPSD_max;i++)
        displayFunction[i] = NULL;

    minimumPrintBitmapWidth   = m_pPrintJobObject->GetMinPrintResolutionX();
    minimumPrintBitmapHeight  = m_pPrintJobObject->GetMinPrintResolutionY();
    maximumPrintBitmapWidth   = m_pPrintJobObject->GetMaxPrintResolutionX();
    maximumPrintBitmapHeight  = m_pPrintJobObject->GetMaxPrintResolutionY();
    maximumPreviewImageWidth  = m_pPrintJobObject->GetMaxPreviewResolutionX();
    maximumPreviewImageHeight = m_pPrintJobObject->GetMaxPreviewResolutionY();

    pPrint = new DVPSStoredPrint(
        m_pPrintJobObject->GetDefaultPrintIllumination(), 
        m_pPrintJobObject->GetDefaultPrintReflection(), 
        m_pPrintJobObject->GetTargetAETitle().c_str());
    pState = new DVPresentationState(OFstatic_cast(DiDisplayFunction **, displayFunction),
        minimumPrintBitmapWidth, minimumPrintBitmapHeight, maximumPrintBitmapWidth, maximumPrintBitmapHeight,
        maximumPreviewImageWidth, maximumPreviewImageHeight);
}

McsfDcmPrintObject::~McsfDcmPrintObject(void)
{
    if(pPrint != NULL)
    {
        delete pPrint;
        pPrint = NULL;
    }

    if(pState != NULL)
    {
        delete pState;
        pState = NULL;
    }

    if(pStoredPState != NULL)
    {
        delete pStoredPState;
        pStoredPState = NULL;
    }

    if(pDicomImage != NULL)
    {
        delete pDicomImage;
        pDicomImage = NULL;
    }

    if(pDicomPState != NULL)
    {
        delete pDicomPState;
        pDicomPState = NULL;
    }

    if(pHardcopyImage != NULL)
    {
        delete pHardcopyImage;
        pHardcopyImage = NULL;
    }
}

void McsfDcmPrintObject::UpdatePrintJobObject(McsfPrintJobObject* pPrintJobObject)
{
    m_pPrintJobObject = pPrintJobObject;

    printerMediumType = OFString(m_pPrintJobObject->GetMediumType().c_str());
    printerFilmDestination = OFString(m_pPrintJobObject->GetDestination().c_str());
}

OFCondition McsfDcmPrintObject::selectDisplayPresentationLUT(const char *lutID)
{
    OFCondition result = EC_IllegalCall;
    if (lutID && pState)
    {
        const char *lutfile = m_pPrintJobObject->GetLUTFilename(lutID).c_str();
        if (lutfile)
        {
            OFString filename = OFString(m_pPrintJobObject->GetLUTFolder().c_str()); // never NULL.
            filename += PATH_SEPARATOR;
            filename += lutfile;
            DcmFileFormat *fileformat = NULL;
            if ((result = DVPSHelper::loadFileFormat(filename.c_str(), fileformat)) == EC_Normal)
            {
                if (fileformat)
                {
                    DcmDataset *dataset = fileformat->getDataset();
                    if (dataset)
                        result = pState->setPresentationLookupTable(*dataset);
                    else
                        result = EC_IllegalCall;
                    if (EC_Normal == result)
                        displayCurrentLUTID = lutID;
                    else
                        displayCurrentLUTID.clear();
                } else result = EC_IllegalCall;
                if (result != EC_Normal)
                    LOG_ERROR_FILMING("Load display presentation LUT from file: invalid data structures");
            } else
                LOG_ERROR_FILMING("Load display presentation LUT from file: could not read fileformat");
            delete fileformat;
        } else
            LOG_ERROR_FILMING("Load display presentation LUT from file: not specified in config file");
    }
    return result;
}

OFCondition McsfDcmPrintObject::loadImage(const char *imgName)
{
    OFCondition status = EC_IllegalCall;
    DcmFileFormat *image = NULL;
    DVPresentationState *newState = new DVPresentationState(
        OFstatic_cast(DiDisplayFunction **, displayFunction),
        minimumPrintBitmapWidth, minimumPrintBitmapHeight, 
        maximumPrintBitmapWidth, maximumPrintBitmapHeight,
        maximumPreviewImageWidth, maximumPreviewImageHeight);
    if (newState==NULL)
    {
        LOG_ERROR_FILMING("Load image from file failed: memory exhausted");
        return EC_MemoryExhausted;
    }

    if ((status = DVPSHelper::loadFileFormat(imgName, image)) == EC_Normal)
    {
        if (image)
        {
            DcmDataset *dataset = image->getDataset();
            if (dataset)
            {
                //check the representation
                Uint16 iRepresentation=0;
                status = dataset->findAndGetUint16(DcmTagKey(DCM_PixelRepresentation),iRepresentation);
                //if the swap 8bit DICOM image has PixelRepresentation, then move it, 
                //it's wrong tag value from VIEWER save as generate
                if(status == EC_Normal && iRepresentation == 1)
                {
                    LOG_WARN_FILMING("remove the Representation mark!");
                    status = dataset->putAndInsertUint16(DcmTagKey(DCM_PixelRepresentation),0);
                    LOG_ERROR_FILMING("Replace Pixel Representation to 0 failed!");
                }

                if (EC_Normal == (status = newState->createFromImage(*dataset)))
                {
                    //TODO add for NanJing real printer test!
                    newState->setPresentationLabel("Shanghai United Imaging Healthcare Inc.");

                    status = newState->attachImage(image, OFFalse);
                }

                if (EC_Normal == status)
                {
                    exchangeImageAndPState(newState, image);
                }
            } 
            else 
            {
                status = EC_CorruptedData;
            }
        } 
        else 
        {
                status = EC_IllegalCall;
        }

        if (status != EC_Normal)
            LOG_ERROR_FILMING("Load image from file failed: invalid data structures");
    } 
    else
        LOG_ERROR_FILMING("Load image from file failed: could not read fileformat");

    if (status != EC_Normal)
    {
        delete newState;
        delete image;
    }
    return status;
}

OFCondition McsfDcmPrintObject::exchangeImageAndPState(
    DVPresentationState *newState, 
    DcmFileFormat *image, 
    DcmFileFormat *state)
{
    if (newState==NULL) return EC_IllegalCall;
    if (image==NULL) return EC_IllegalCall;
    if (pState != newState)
    {
        delete pState;
        delete pStoredPState;
        delete pDicomPState;
        pState = newState;
        pStoredPState = NULL;
        pDicomPState = state;
    }
    if (pDicomImage != image)
    {
        delete pDicomImage;       // delete only if different
        pDicomImage = image;
    }
    return EC_Normal;
}


OFCondition McsfDcmPrintObject::loadPState(const char *pstName,
    const char *imgName)
{
    OFCondition status = EC_IllegalCall;
    DcmFileFormat *pstate = NULL;
    DcmFileFormat *image = pDicomImage;     // default: do not replace image if image filename is NULL
    DVPresentationState *newState = new DVPresentationState(
        OFstatic_cast(DiDisplayFunction **, displayFunction),
        minimumPrintBitmapWidth, minimumPrintBitmapHeight, 
        maximumPrintBitmapWidth, maximumPrintBitmapHeight,
        maximumPreviewImageWidth, maximumPreviewImageHeight);
    if (newState==NULL)
    {
        LOG_ERROR_FILMING("Load presentation state from file failed: memory exhausted");
        return EC_MemoryExhausted;
    }

    if ((status = DVPSHelper::loadFileFormat(pstName, pstate)) == EC_Normal)
    {
        if ((imgName == NULL) || ((status = DVPSHelper::loadFileFormat(imgName, image)) == EC_Normal))
        {
            if ((pstate)&&(image))
            {
                DcmDataset *dataset = pstate->getDataset();
                if (dataset)
                {
                    if (EC_Normal == (status = newState->read(*dataset)))
                        status = newState->attachImage(image, OFFalse);
                    if (EC_Normal == status)
                    {
                        exchangeImageAndPState(newState, image, pstate);
                    }
                } else status = EC_CorruptedData;
            } else status = EC_IllegalCall;
            if (status != EC_Normal)
                LOG_ERROR_FILMING("Load presentation state from file failed: invalid data structures");
        } else
            LOG_ERROR_FILMING("Load presentation state from file failed: could not load image");
    } else
        LOG_ERROR_FILMING("Load presentation state from file failed: could not read fileformat");
    if (status != EC_Normal)
    {
        delete newState;
        if (image != pDicomImage)
            delete image;
        delete pstate;
    }
    return status;
}

OFCondition McsfDcmPrintObject::saveStoredPrint(
    const char *filename,
    OFBool writeRequestedImageSize,
    OFBool explicitVR,
    const char *instanceUID)
{
    if (pState == NULL) return EC_IllegalCall;
    if (pPrint == NULL) return EC_IllegalCall;
    if (filename == NULL) return EC_IllegalCall;

    OFCondition status = EC_Normal;
    DcmFileFormat *fileformat = new DcmFileFormat();
    DcmDataset *dataset = NULL;
    if (fileformat)
        dataset = fileformat->getDataset();

    char newuid[70];
    char buf[32];

    /* set annotation if active */
    if (activateAnnotation)
    {
        OFString text;
        OFString dummy;
        if (prependDateTime)
        {
            OFDateTime::getCurrentDateTime().getISOFormattedDateTime(text, OFFalse /*showSeconds*/);
        }

        if (prependPrinterName)
        {
            text += currentPrinter;
            text += " ";
        }

        if (prependLighting)
        {
            sprintf(buf, "%d/%d ", 
                pPrint->getPrintIllumination(), 
                pPrint->getPrintReflectedAmbientLight());
            text += buf;
        }

        text += annotationText;
        if (text.size() >64) text.erase(64); // limit to max annotation length

        if (m_pPrintJobObject->GetTargetPrinterSupportsAnnotationBoxSOPClass())
        {
            const char *displayformat = m_pPrintJobObject->GetTargetPrinterAnnotationDisplayFormatID().c_str();
            Uint16 position = m_pPrintJobObject->GetTargetPrinterAnnotationPosition();
            pPrint->setSingleAnnotation(displayformat, text.c_str(), position);
        } 
        else 
        {
            pPrint->deleteAnnotations();
        }

        if (m_pPrintJobObject->GetTargetPrinterSessionLabelAnnotation())
        {
            status = setPrinterFilmSessionLabel(text.c_str());
        }
    } 
    else 
    {
        pPrint->deleteAnnotations();
    }

    if (dataset)
    {
        if (instanceUID) status = pPrint->setInstanceUID(instanceUID); else
        {
            dcmGenerateUniqueIdentifier(newuid);
            status = pPrint->setInstanceUID(newuid);
        }
        if (EC_Normal == status) 
            status = pPrint->write(*dataset, writeRequestedImageSize, OFTrue, OFTrue, OFFalse);

        // save file
        if (EC_Normal == status) 
            status = DVPSHelper::saveFileFormat(filename, fileformat, explicitVR);

        if (status != EC_Normal)
            LOG_ERROR_FILMING("Save stored print to file failed: could not write fileformat");
    } 
    else 
    {
        LOG_ERROR_FILMING("Save stored print to file failed: memory exhausted");
        status = EC_MemoryExhausted;
    }

    delete fileformat;
    return status;
}

OFCondition McsfDcmPrintObject::makeNewStoreFileName(
    const char      *SOPClassUID,
    const char      * /* SOPInstanceUID */ ,
    char            *newImageFileName)
{

    OFString filename;
    char prefix[12];

    const char *m = dcmSOPClassUIDToModality(SOPClassUID);
    if (m==NULL) m = "XX";
    sprintf(prefix, "%s_", m);
    // unsigned int seed = fnamecreator.hashString(SOPInstanceUID);
    unsigned int seed = (unsigned int)time(NULL);
    newImageFileName[0]=0; // return empty string in case of error

    OFFilenameCreator fnamecreator;
    if (! fnamecreator.makeFilename(seed, this->m_sFileRootDir.c_str(), prefix, ".dcm", filename)) 
        return EC_IllegalParameter;

    strcpy(newImageFileName, filename.c_str());
    return EC_Normal;
}

OFCondition McsfDcmPrintObject::saveStoredPrint(OFBool writeRequestedImageSize)
{
    char uid[100];
    dcmGenerateUniqueIdentifier(uid);

    char imageFileName[MAXPATHLEN+1];
    OFCondition result=EC_Normal;

    OFFilenameCreator fnamecreator;

    if (makeNewStoreFileName(UID_RETIRED_StoredPrintStorage, uid, imageFileName).good())
    {
        // now store stored print object as filename
        result = saveStoredPrint(imageFileName, writeRequestedImageSize, OFTrue, uid);
        if(EC_Normal==result)
        {
            this->m_sSPfilePath = imageFileName;
        }
    }
    return result;
}

void McsfDcmPrintObject::setAnnotationText(const char *value)
{
    if (value) 
        annotationText=value; 
    else 
        annotationText.clear();
    return;
}

OFCondition McsfDcmPrintObject::setPrinterFilmSessionLabel(const char *value)
{
    if (value) printerFilmSessionLabel = value; else printerFilmSessionLabel.clear();
    return EC_Normal;
}

OFCondition McsfDcmPrintObject::saveHardcopyGrayscaleImage(
    const char *filename,
    const void *pixelData,
    unsigned long width,
    unsigned long height,
    double aspectRatio,
    OFBool explicitVR,
    const char *instanceUID)
{
    if (pState == NULL) return EC_IllegalCall;
    if (pPrint == NULL) return EC_IllegalCall;

    if ((width<1)||(width > 0xFFFF)) return EC_IllegalCall;
    if ((height<1)||(height > 0xFFFF)) return EC_IllegalCall;
    if (pixelData == NULL) return EC_IllegalCall;
    if (filename == NULL) return EC_IllegalCall;
    if (aspectRatio == 0.0) return EC_IllegalCall;

    Uint16 columns = OFstatic_cast(Uint16, width);
    Uint16 rows = OFstatic_cast(Uint16, height);
    OFCondition status = EC_Normal;
    DcmFileFormat *fileformat = new DcmFileFormat();
    DcmDataset *dataset = NULL;
    if (fileformat) dataset=fileformat->getDataset();
    char newuid[70];
    OFString aString;
    OFString theInstanceUID;

    if (dataset)
    {
        // write patient module
        if (EC_Normal==status) 
            status = pState->writeHardcopyImageAttributes(*dataset);
        // write general study and general series module
        if (EC_Normal==status) 
            status = pPrint->writeHardcopyImageAttributes(*dataset);

        // Hardcopy Equipment Module
        if (EC_Normal==status) 
            status = DVPSHelper::putStringValue(dataset, DCM_RETIRED_HardcopyDeviceManufacturer, "OFFIS");
        if (EC_Normal==status) 
            status = DVPSHelper::putStringValue(dataset, 
            DCM_RETIRED_HardcopyDeviceSoftwareVersion, 
            OFFIS_DTK_IMPLEMENTATION_VERSION_NAME);

        // General Image Module
        if (EC_Normal==status) 
            status = DVPSHelper::putStringValue(dataset, DCM_InstanceNumber);
        if (EC_Normal==status) 
            status = DVPSHelper::putStringValue(dataset, DCM_PatientOrientation);
        if (EC_Normal==status) 
            status = DVPSHelper::putStringValue(dataset, DCM_ImageType, "DERIVED\\SECONDARY");
        if (EC_Normal==status) 
            status = DVPSHelper::putStringValue(dataset, 
            DCM_DerivationDescription, 
            "Hardcopy rendered using Presentation State");
        // source image sequence is written in pState->writeHardcopyImageAttributes().

        // SOP Common Module
        if (EC_Normal==status)
        {
            status = DVPSHelper::putStringValue(dataset, 
            DCM_SOPClassUID, 
            UID_RETIRED_HardcopyGrayscaleImageStorage);
        }

        dcmGenerateUniqueIdentifier(newuid);
        theInstanceUID = (instanceUID ? instanceUID : newuid);
        if (EC_Normal==status) 
        {
            status = DVPSHelper::putStringValue(dataset, DCM_SOPInstanceUID, theInstanceUID.c_str());
        }
        DVPSHelper::currentDate(aString);
        if (EC_Normal==status) 
        {
            status = DVPSHelper::putStringValue(dataset, DCM_InstanceCreationDate, aString.c_str());
        }
        DVPSHelper::currentTime(aString);
        if (EC_Normal==status) 
        {
                status = DVPSHelper::putStringValue(dataset, DCM_InstanceCreationTime, aString.c_str());
        }

        // Hardcopy Grayscale Image Module
        if (EC_Normal==status) status = DVPSHelper::putStringValue(dataset, DCM_PhotometricInterpretation, "MONOCHROME2");
        if (EC_Normal==status) status = DVPSHelper::putUint16Value(dataset, DCM_SamplesPerPixel, 1);
        if (EC_Normal==status) status = DVPSHelper::putUint16Value(dataset, DCM_Rows, rows);
        if (EC_Normal==status) status = DVPSHelper::putUint16Value(dataset, DCM_Columns, columns);
        if (EC_Normal==status) status = DVPSHelper::putUint16Value(dataset, DCM_BitsAllocated, 16);
        if (EC_Normal==status) status = DVPSHelper::putUint16Value(dataset, DCM_BitsStored, 12);
        if (EC_Normal==status) status = DVPSHelper::putUint16Value(dataset, DCM_HighBit, 11);
        if (EC_Normal==status) status = DVPSHelper::putUint16Value(dataset, DCM_PixelRepresentation, 0);
        if ((EC_Normal==status)&&(aspectRatio != 1.0))
        {
            sprintf(newuid, "%ld\\%ld", 1000L, OFstatic_cast(long, aspectRatio*1000.0));
            status = DVPSHelper::putStringValue(dataset, DCM_PixelAspectRatio, newuid);
        }

        DcmPolymorphOBOW *pxData = new DcmPolymorphOBOW(DCM_PixelData);
        if (pxData)
        {
            status = pxData->putUint16Array(OFstatic_cast(Uint16 *, OFconst_cast(void *, pixelData)), OFstatic_cast(unsigned long, width*height));
            if (EC_Normal==status) status = dataset->insert(pxData, OFTrue /*replaceOld*/); else delete pxData;
        } 
        else 
            status = EC_MemoryExhausted;

        // add Presentation LUT to hardcopy file if present, making it a Standard Extended SOP Class
        if ((EC_Normal==status)&&(pState->getPresentationLUT() == DVPSP_table))
        {
            status = pState->writePresentationLUTforPrint(*dataset);
        }

        if (status != EC_Normal)
            LOG_ERROR_FILMING("Save hardcopy grayscale image to file failed: invalid data structures");

        // save image file
        if (EC_Normal == status)
        {
            status = DVPSHelper::saveFileFormat(filename, fileformat, explicitVR);
            if (status != EC_Normal)
                LOG_ERROR_FILMING("Save hardcopy grayscale image to file failed: could not write fileformat");
        }
    } 
    else 
    {
        status = EC_MemoryExhausted;
        LOG_ERROR_FILMING("Save hardcopy grayscale image to file failed: memory exhausted");
    }

    if (EC_Normal == status)
    {
        OFString reqImageTmp;
        const char *reqImageSize = NULL;
        DVPSPresentationLUT *presLUT = pState->getPresentationLUTData();

        if (EC_Normal == pState->getPrintBitmapRequestedImageSize(reqImageTmp)) reqImageSize = reqImageTmp.c_str();
        /* we don't pass the patient ID (available as pState->getPatientID()) here because then
        * we could end up with multiple images being part of one study and one series, but having
        * different patient IDs. This might confuse archives using the patient root query model.
        */

		string targetAETitle = m_pPrintJobObject->GetTargetAETitle();

		string targetAETitleInfo = "@@@@TargetAETitle=" + targetAETitle + "@@@@";

		LOG_INFO_FILMING(targetAETitleInfo);

        status = pPrint->addImageBox(
            targetAETitle.c_str(), 
            theInstanceUID.c_str(), 
            reqImageSize, 
            NULL, 
            presLUT, 
            pState->isMonochrome1Image());
    }

    delete fileformat;
    return status;
}

OFCondition McsfDcmPrintObject::saveColorImage(
    const char *filename,
    const void *pixelData,
    unsigned long width,
    unsigned long height,
    double aspectRatio,
    OFBool explicitVR,
    const char *instanceUID)
{
    if (pState == NULL) return EC_IllegalCall;
    if (pPrint == NULL) return EC_IllegalCall;

    if ((width<1)||(width > 0xFFFF)) return EC_IllegalCall;
    if ((height<1)||(height > 0xFFFF)) return EC_IllegalCall;
    if (pixelData == NULL) return EC_IllegalCall;
    if (filename == NULL) return EC_IllegalCall;
    if (aspectRatio == 0.0) return EC_IllegalCall;

    Uint16 columns = OFstatic_cast(Uint16, width);
    Uint16 rows = OFstatic_cast(Uint16, height);
    OFCondition status = EC_Normal;
    DcmFileFormat *fileformat = new DcmFileFormat();
    DcmDataset *dataset = NULL;
    if (fileformat) dataset=fileformat->getDataset();
    char newuid[70];
    OFString aString;
    OFString theInstanceUID;

    if (dataset)
    {
        // write patient module
        if (EC_Normal==status) 
            status = pState->writeHardcopyImageAttributes(*dataset);
        // write general study and general series module
        if (EC_Normal==status) 
            status = pPrint->writeHardcopyImageAttributes(*dataset);

        // Hardcopy Equipment Module
        if (EC_Normal==status) 
            status = DVPSHelper::putStringValue(dataset, DCM_RETIRED_HardcopyDeviceManufacturer, "OFFIS");
        if (EC_Normal==status) 
            status = DVPSHelper::putStringValue(dataset, 
            DCM_RETIRED_HardcopyDeviceSoftwareVersion, 
            OFFIS_DTK_IMPLEMENTATION_VERSION_NAME);

        // General Image Module
        if (EC_Normal==status) 
            status = DVPSHelper::putStringValue(dataset, DCM_InstanceNumber);
        if (EC_Normal==status) 
            status = DVPSHelper::putStringValue(dataset, DCM_PatientOrientation);
        if (EC_Normal==status) 
            status = DVPSHelper::putStringValue(dataset, DCM_ImageType, "DERIVED\\SECONDARY");
        if (EC_Normal==status) 
            status = DVPSHelper::putStringValue(dataset, 
            DCM_DerivationDescription, 
            "Hardcopy rendered using Presentation State");
        // source image sequence is written in pState->writeHardcopyImageAttributes().

        // SOP Common Module
        if (EC_Normal==status)
        {
            status = DVPSHelper::putStringValue(dataset, 
            DCM_SOPClassUID, 
            UID_RETIRED_HardcopyGrayscaleImageStorage);
        }

        dcmGenerateUniqueIdentifier(newuid);
        theInstanceUID = (instanceUID ? instanceUID : newuid);
        if (EC_Normal==status) 
        {
            status = DVPSHelper::putStringValue(dataset, DCM_SOPInstanceUID, theInstanceUID.c_str());
        }
        DVPSHelper::currentDate(aString);
        if (EC_Normal==status) 
        {
            status = DVPSHelper::putStringValue(dataset, DCM_InstanceCreationDate, aString.c_str());
        }
        DVPSHelper::currentTime(aString);
        if (EC_Normal==status) 
        {
                status = DVPSHelper::putStringValue(dataset, DCM_InstanceCreationTime, aString.c_str());
        }

        // Hardcopy Grayscale Image Module
        if (EC_Normal==status) status = DVPSHelper::putStringValue(dataset, DCM_PhotometricInterpretation, "RGB");
		if (EC_Normal==status) status = DVPSHelper::putStringValue(dataset, DCM_PlanarConfiguration, "0");
        if (EC_Normal==status) status = DVPSHelper::putUint16Value(dataset, DCM_SamplesPerPixel, 3);
        if (EC_Normal==status) status = DVPSHelper::putUint16Value(dataset, DCM_Rows, rows);
        if (EC_Normal==status) status = DVPSHelper::putUint16Value(dataset, DCM_Columns, columns);
        if (EC_Normal==status) status = DVPSHelper::putUint16Value(dataset, DCM_BitsAllocated, 8);
        if (EC_Normal==status) status = DVPSHelper::putUint16Value(dataset, DCM_BitsStored, 8);
        if (EC_Normal==status) status = DVPSHelper::putUint16Value(dataset, DCM_HighBit, 7);
        if (EC_Normal==status) status = DVPSHelper::putUint16Value(dataset, DCM_PixelRepresentation, 0);
        if ((EC_Normal==status)&&(aspectRatio != 1.0))
        {
            sprintf(newuid, "%ld\\%ld", 1000L, OFstatic_cast(long, aspectRatio*1000.0));
            status = DVPSHelper::putStringValue(dataset, DCM_PixelAspectRatio, newuid);
        }

        DcmPolymorphOBOW *pxData = new DcmPolymorphOBOW(DCM_PixelData);
        if (pxData)
        {
            status = pxData->putUint8Array(OFstatic_cast(Uint8 *, OFconst_cast(void *, pixelData)), OFstatic_cast(unsigned long, width*height));
            if (EC_Normal==status) status = dataset->insert(pxData, OFTrue /*replaceOld*/); else delete pxData;
        } 
        else 
            status = EC_MemoryExhausted;

        // add Presentation LUT to hardcopy file if present, making it a Standard Extended SOP Class
        if ((EC_Normal==status)&&(pState->getPresentationLUT() == DVPSP_table))
        {
            status = pState->writePresentationLUTforPrint(*dataset);
        }

        if (status != EC_Normal)
            LOG_ERROR_FILMING("Save hardcopy grayscale image to file failed: invalid data structures");

        // save image file
        if (EC_Normal == status)
        {
            status = DVPSHelper::saveFileFormat(filename, fileformat, explicitVR);
            if (status != EC_Normal)
                LOG_ERROR_FILMING("Save hardcopy grayscale image to file failed: could not write fileformat");
        }
    } 
    else 
    {
        status = EC_MemoryExhausted;
        LOG_ERROR_FILMING("Save hardcopy grayscale image to file failed: memory exhausted");
    }

    if (EC_Normal == status)
    {
        OFString reqImageTmp;
        const char *reqImageSize = NULL;
        DVPSPresentationLUT *presLUT = pState->getPresentationLUTData();

        if (EC_Normal == pState->getPrintBitmapRequestedImageSize(reqImageTmp)) reqImageSize = reqImageTmp.c_str();
        /* we don't pass the patient ID (available as pState->getPatientID()) here because then
        * we could end up with multiple images being part of one study and one series, but having
        * different patient IDs. This might confuse archives using the patient root query model.
        */

		string targetAETitle = m_pPrintJobObject->GetTargetAETitle();

		string targetAETitleInfo = "@@@@TargetAETitle=" + targetAETitle + "@@@@";

		LOG_INFO_FILMING(targetAETitleInfo);

        status = pPrint->addImageBox(
            targetAETitle.c_str(), 
            theInstanceUID.c_str(), 
            reqImageSize, 
            NULL, 
            presLUT, 
            pState->isMonochrome1Image());
    }

    delete fileformat;
    return status;
}

OFCondition McsfDcmPrintObject::HandlePrintingImage(
    const char *instanceUID)
{
    OFCondition status = EC_Normal;
    char newuid[70];
    OFString theInstanceUID;
    OFString reqImageTmp;
    const char *reqImageSize = NULL;

    dcmGenerateUniqueIdentifier(newuid);
    theInstanceUID = (instanceUID ? instanceUID : newuid);
    DVPSPresentationLUT *presLUT = pState->getPresentationLUTData();
    if (EC_Normal == pState->getPrintBitmapRequestedImageSize(reqImageTmp)) 
        reqImageSize = reqImageTmp.c_str();

    string targetAETitle = m_pPrintJobObject->GetTargetAETitle();

    status = pPrint->addImageBox(
        targetAETitle.c_str(), 
        theInstanceUID.c_str(), 
        reqImageSize, 
        NULL, 
        presLUT, 
        pState->isMonochrome1Image());
    return status;
}

OFCondition McsfDcmPrintObject::HandlePrintingImageSetInfo(const char *instanceUID)
{
    if (pState == NULL) return EC_IllegalCall;
    if (pPrint == NULL) return EC_IllegalCall;

    OFCondition status = EC_Normal;
    DcmFileFormat *fileformat = new DcmFileFormat();
    DcmDataset *dataset = NULL;
    if (fileformat)
        dataset = fileformat->getDataset();

    char buf[32];

    /* set annotation if active */
    if (activateAnnotation)
    {
        OFString text;
        OFString dummy;
        if (prependDateTime)
        {
            OFDateTime::getCurrentDateTime().getISOFormattedDateTime(text, OFFalse /*showSeconds*/);
        }
        if (prependPrinterName)
        {
            text += currentPrinter;
            text += " ";
        } 
        if (prependLighting)
        {
            sprintf(buf, "%d/%d ", 
                pPrint->getPrintIllumination(), 
                pPrint->getPrintReflectedAmbientLight());
            text += buf;
        } 
        text += annotationText;
        if (text.size() >64) text.erase(64); // limit to max annotation length

        if (m_pPrintJobObject->GetTargetPrinterSupportsAnnotationBoxSOPClass())
        {
            const char *displayformat = m_pPrintJobObject->GetTargetPrinterAnnotationDisplayFormatID().c_str();
            Uint16 position = m_pPrintJobObject->GetTargetPrinterAnnotationPosition();
            pPrint->setSingleAnnotation(displayformat, text.c_str(), position);
        } 
        else 
        {
            pPrint->deleteAnnotations();
        }

        if (m_pPrintJobObject->GetTargetPrinterSessionLabelAnnotation())
        {
            status = setPrinterFilmSessionLabel(text.c_str());
        }
    } 
    else 
    {
        pPrint->deleteAnnotations();
    }

    if (dataset)
    {
        if (EC_Normal == status) 
            status = pPrint->write(*dataset, OFTrue, OFTrue, OFTrue, OFFalse);
    } 
    else 
    {
        LOG_ERROR_FILMING("Save stored print to file failed: memory exhausted");
        status = EC_MemoryExhausted;
    }

    delete fileformat;
    return status;
}

OFCondition McsfDcmPrintObject::saveHardcopyGrayscaleImage(
    const void *pixelData,
    unsigned long width,
    unsigned long height,
    double aspectRatio)
{
    char uid[100];
    dcmGenerateUniqueIdentifier(uid);

    char imageFileName[MAXPATHLEN+1];

    OFCondition result=EC_Normal;

    if (makeNewStoreFileName(UID_RETIRED_HardcopyGrayscaleImageStorage, uid, imageFileName).good())
    {
        result = saveHardcopyGrayscaleImage(imageFileName, pixelData, width, height, aspectRatio, OFTrue, uid);
        if (EC_Normal==result)
        {
            this->m_sHGfilePathList.push_back(imageFileName);
            LOG_INFO_FILMING(string("save HG file to:") + imageFileName);
        }
    }
    return result;
}

OFCondition McsfDcmPrintObject::saveColorImage(
    const void *pixelData,
    unsigned long width,
    unsigned long height,
    double aspectRatio)
{
	char uid[100];
    dcmGenerateUniqueIdentifier(uid);

    char imageFileName[MAXPATHLEN+1];

    OFCondition result=EC_Normal;

    if (makeNewStoreFileName(UID_RETIRED_HardcopyColorImageStorage, uid, imageFileName).good())
    {
        result = saveColorImage(imageFileName, pixelData, width, height, aspectRatio, OFTrue, uid);
        if (EC_Normal==result)
        {
            this->m_sHGfilePathList.push_back(imageFileName);
            LOG_INFO_FILMING(string("save HG file to:") + imageFileName);
        }
    }
    return result;
}

const char *McsfDcmPrintObject::getCurrentPrinter()
{
    return currentPrinter.c_str();
}

OFCondition McsfDcmPrintObject::setCurrentPrinter(const char *targetID)
{
    if (targetID == NULL) 
        return EC_IllegalCall;

    if (m_pPrintJobObject->GetTargetHostName().length() <= 0) 
        return EC_IllegalCall; // Printer seems to be unknown

    activateAnnotation = m_pPrintJobObject->GetTargetPrinterSupportsAnnotation();
    if (pPrint != NULL)
    {
        pPrint->setPrinterName(targetID);
        pPrint->setDestination(m_pPrintJobObject->GetTargetAETitle().c_str());
    }
    currentPrinter = targetID;
    return EC_Normal;
}

OFCondition McsfDcmPrintObject::printSCUcreateBasicFilmSession(DVPSPrintMessageHandler& printHandler, OFBool plutInSession)
{
    if (!pPrint) return EC_IllegalCall;
    OFCondition result = EC_Normal;
    DcmDataset dset;
    DcmElement *delem = NULL;
    char buf[20];

    if ((EC_Normal==result)&&(printerMediumType.size() > 0))
    {
        delem = new DcmCodeString(DCM_MediumType);
        if (delem) result = delem->putString(printerMediumType.c_str()); else result=EC_IllegalCall;
        if (EC_Normal==result) result = dset.insert(delem, OFTrue /*replaceOld*/);
    }

    if ((EC_Normal==result)&&(printerFilmDestination.size() > 0))
    {
        delem = new DcmCodeString(DCM_FilmDestination);
        if (delem) result = delem->putString(printerFilmDestination.c_str()); else result=EC_IllegalCall;
        if (EC_Normal==result) result = dset.insert(delem, OFTrue /*replaceOld*/);
    }

    if ((EC_Normal==result)&&(printerFilmSessionLabel.size() > 0))
    {
        delem = new DcmLongString(DCM_FilmSessionLabel);
        if (delem) result = delem->putString(printerFilmSessionLabel.c_str()); else result=EC_IllegalCall;
        if (EC_Normal==result) result = dset.insert(delem, OFTrue /*replaceOld*/);
    }

    if ((EC_Normal==result)&&(printerPriority.size() > 0))
    {
        delem = new DcmCodeString(DCM_PrintPriority);
        if (delem) result = delem->putString(printerPriority.c_str()); else result=EC_IllegalCall;
        if (EC_Normal==result) result = dset.insert(delem, OFTrue /*replaceOld*/);
    }

    if ((EC_Normal==result)&&(printerOwnerID.size() > 0))
    {
        delem = new DcmShortString(DCM_OwnerID);
        if (delem) result = delem->putString(printerOwnerID.c_str()); else result=EC_IllegalCall;
        if (EC_Normal==result) result = dset.insert(delem, OFTrue /*replaceOld*/);
    }

    if ((EC_Normal==result)&&(printerNumberOfCopies > 0))
    {
        sprintf(buf, "%lu", printerNumberOfCopies);
        delem = new DcmIntegerString(DCM_NumberOfCopies);
        if (delem) result = delem->putString(buf); else result=EC_IllegalCall;
        if (EC_Normal==result) result = dset.insert(delem, OFTrue /*replaceOld*/);
    }

    if (EC_Normal==result) 
        result = pPrint->printSCUcreateBasicFilmSession(printHandler, dset, plutInSession);
    return result;
}

const std::vector<std::string>& McsfDcmPrintObject::GetHGfilePathList() const
{
    return this->m_sHGfilePathList;
}

const std::string& McsfDcmPrintObject::GetSPfilePath() const
{
    return this->m_sSPfilePath;
}

MCSF_FILMING_END_NAMESPACE

