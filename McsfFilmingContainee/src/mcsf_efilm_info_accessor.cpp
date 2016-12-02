//////////////////////////////////////////////////////////////////
///  Copyright (c) Shanghai United Imaging Healthcare Inc., 2011
///  All rights reserved.
/// 
///  \author    qingzhen.ma  (mailto:qingzhen.ma@united-imaging.com)
/// 
///  \file      mcsf_display_info_accessor.cpp
/// 
///  \brief     class DisplayInfoAccessor implementation 
/// 
///  \version   1.0
/// 
///  \date      11/11/2013/
////////////////////////////////////////////////////////////////

#include "mcsf_efilm_info_accessor.h"

#include "McsfDataHeader/mcsf_data_header_element_map_interface.h"

#include "McsfDatabase/mcsf_database_interface.h"

#include "McsfDicomConvertor/mcsf_dicom_convertor_factory.h"
#include "McsfDicomConvertor/mcsf_dicom_convertor.h"

#include "McsfLogger/mcsf_logger.h"

MCSF_APPLICATION_BEGIN_NAMESPACE   // begin namespace Mcsf

EFilmInfoAccessor::EFilmInfoAccessor(IDatabasePtr pDatabase)
{
    m_pDatabase = pDatabase;
}

EFilmInfoAccessor::~EFilmInfoAccessor()
{

}

IDataHeaderElementMap::Ptr EFilmInfoAccessor::GetPixelData(const int iCellIndex, const std::string& sSOPInstanceUID, const std::string& ksPS)
{
    iCellIndex;
    sSOPInstanceUID;

    try
    {
        std::string sPhotometricInterpretation = std::string("MONOCHROME2");
        unsigned int uiSamplesPerPixel = 1;

        unsigned int uiBitsAllocated = 8;
        unsigned int uiBitsStored = 8;
        unsigned int uiHighBit = 7;

        // 0 -- unsigned integer; 1 -- signed integer
        unsigned int uiPixelRepresentation(0);        

        IDataHeaderElementMap::Ptr pImage(IDataHeaderElementMap::CreateDataHeader());

		IImageBasePtr pImageBase;
		int iStatus = m_pDatabase->GetImageObjectByUID(sSOPInstanceUID, pImageBase);
		if (ERROR_DB_NULL != iStatus && iCellIndex != -1)
		{
			//LOG_DEV_ERROR_2(AppSaveFilmingModule(), AdvAppLogUID()) << "Fail to get image"
			//    " object from database. The SOPInstance UID is: " << sSOPInstanceUID.c_str();
			//return nullptr;

			//for saving original image for xiongke hospital
			//return pImage;
			uiSamplesPerPixel = iCellIndex;
		}  

		// samples per pixel
		//if (!pImage->InsertUInt16ValueByTag(kTagDcm_SamplesPerPixel, static_cast<UInt16>(uiSamplesPerPixel)))
		//{
		//	LOG_DEV_ERROR_2(AppSaveFilmingModule(), AdvAppLogUID()) << "Fail to "
		//		" insert TagDcm_SamplesPerPixel tag into DataHeader. Value is: " << uiSamplesPerPixel;
		//	return false;
		//}

		if(ERROR_DB_NULL != iStatus && iCellIndex != -1)
		{
			//for saving original image for xiongke hospital
			return pImage;
		}

        // sPhotometricInterpretation
        int iLength = static_cast<int>(std::strlen(sPhotometricInterpretation.c_str()));
        //if (!pImage->InsertStringByTag(kTagDcm_PhotometricInterpretation, sPhotometricInterpretation.c_str(), iLength))
        //{
        //   LOG_DEV_ERROR_2(AppSaveFilmingModule(), AdvAppLogUID()) << "Fail to "
        //        " insert TagDcm_PhotometricInterpretation tag into DataHeader. Value is: " 
        //        << sPhotometricInterpretation.c_str();
        //    return false;
        //}        

        // bits allocated
        //if (!pImage->InsertUInt16ValueByTag(kTagDcm_BitsAllocated, static_cast<UInt16>(uiBitsAllocated)))
        //{
        //    LOG_DEV_ERROR_2(AppSaveFilmingModule(), AdvAppLogUID()) << "Fail to "
        //        " insert Dcm_BitsAllocated tag into DataHeader. Value is: " << uiBitsAllocated;
        //    return false;
        //}

        // bits stored
        //if (!pImage->InsertUInt16ValueByTag(kTagDcm_BitsStored, static_cast<UInt16>(uiBitsStored)))
        //{
        //    LOG_DEV_ERROR_2(AppSaveFilmingModule(), AdvAppLogUID()) << "Fail to "
        //        " insert Dcm_BitsStored tag into DataHeader. Value is: " << uiBitsStored;
        //    return false;
        //}

        // high bit
        //if (!pImage->InsertUInt16ValueByTag(kTagDcm_HighBit, static_cast<UInt16>(uiHighBit)))
        //{
        //    LOG_DEV_ERROR_2(AppSaveFilmingModule(), AdvAppLogUID()) << "Fail to "
        //        " insert Dcm_HighBit tag into DataHeader. Value is: " << uiHighBit;
        //    return false;
        //}

        // pixel representation
        //if (!pImage->InsertUInt16ValueByTag(kTagDcm_PixelRepresentation, static_cast<UInt16>(uiPixelRepresentation)))
        //{
        //    LOG_DEV_ERROR_2(AppSaveFilmingModule(), AdvAppLogUID()) << "Fail to "
        //        " insert Dcm_PixelRepresentation tag into DataHeader. Value is: " << uiPixelRepresentation;
        //    return false;
        //}

        return pImage;
    }
    catch (const std::exception& ke)
    {
        LOG_DEV_ERROR_2(AppSaveFilmingModule(), AdvAppLogUID()) << "Fail to GetPixelData"
            " with error: " << ke.what();
        return nullptr;
    }
}

IDataHeaderElementMap::Ptr EFilmInfoAccessor::GetHeaderInfo(const int iCellIndex, const std::string& sSOPInstanceUID)
{
    try
    {
        iCellIndex;  // used

        ConstString pString(nullptr);
        int         iSize(0);
        if ((!m_pOriHeaderInfo) ||
            (!m_pOriHeaderInfo->GetStringByTag(kTagDcm_SOPInstanceUID, &pString, &iSize)))
        {
            m_pOriHeaderInfo.reset(IDataHeaderElementMap::CreateDataHeader());
        }

        if ((nullptr != pString) 
            && (0 != iSize)
            && (0 == sSOPInstanceUID.compare(pString)))
        {
            return m_pOriHeaderInfo;
        }

        IImageBasePtr pImage;
        int iStatus = m_pDatabase->GetImageObjectByUID(sSOPInstanceUID, pImage);
        if (ERROR_DB_NULL != iStatus)
        {
            //LOG_DEV_ERROR_2(AppSaveFilmingModule(), AdvAppLogUID()) << "Fail to get image"
            //    " object from database. The SOPInstance UID is: " << sSOPInstanceUID.c_str();
            //return nullptr;

			//for saving original image for xiongke hospital
			return m_pOriHeaderInfo;
        }

        std::string sFilePath = pImage->GetFilePath();
        auto pDICOMConvertor = DICOMConvertorFactory::Instance()->CreateConvertor(m_pDatabase);
        if (!pDICOMConvertor)
        {
            LOG_DEV_ERROR_2(AppSaveFilmingModule(), AdvAppLogUID()) << "Fail to create DICOMConvertor.";
            return nullptr;
        }
        if (!pDICOMConvertor->LoadFile(sFilePath, m_pOriHeaderInfo.get()))
        {
            LOG_DEV_ERROR_2(AppSaveFilmingModule(), AdvAppLogUID()) << "DICOMConvertor cannot load file:"
                << sFilePath.c_str();
            return nullptr;
        }

        if (!m_pOriHeaderInfo->RemoveElementByTag(kTagDcm_PixelData))
        {
            LOG_DEV_WARNING_2(AppSaveFilmingModule(), AdvAppLogUID()) << "Fail to "
                << " remove Dcm_PixelData tag from DataHeader.";
        }

        return m_pOriHeaderInfo;
    }
    catch (const std::exception& ke)
    {
        LOG_DEV_ERROR_2(AppSaveFilmingModule(), AdvAppLogUID()) << "Fail to GetHeaderInfo for"
            " SOPInstanceUID: " << sSOPInstanceUID.c_str() << ". Error is: " << ke.what();
        return nullptr;
    }
}

MCSF_APPLICATION_END_NAMESPACE  // end namespace Mcsf