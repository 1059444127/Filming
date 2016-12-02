//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging Healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Wang Hui  (mailto:hui.wang@united-imaging.com)
///           
///  \file    mcsf_image_command_handler.h
///  \brief   define the filming containee command callback functions and
///           process functions for processing images
///
///  \version 1.0
///  \date    Nov. 23, 2011
///  
//////////////////////////////////////////////////////////////////////////

// C/C++ headers
#include <vector>
#include <sstream>

// 3rd party headers
#include <boost/filesystem.hpp>
#include <boost/algorithm/string.hpp>
#include <boost/lexical_cast.hpp>
#include "ace/OS_NS_unistd.h"

#include "Common/McsfSystemEnvironmentConfig/mcsf_systemenvironment_factory.h"
// header files in other MCSF component
// header files in viewer
//#include "mcsf_med_viewer_internal_config.h"
#include "McsfNetBase/mcsf_communication_node_name.h"
#include "McsfMedViewer/mcsf_med_viewer_controller_interface.h"
#include "McsfMedViewer/mcsf_med_viewer_controller_factory.h"

//DB-importer
#include "McsfDicomConvertor/mcsf_dbimporter_interface.h"

#include "FilmsInfo.pb.h"

#include "mcsf_image_command_handler.h"

MCSF_FILMING_BEGIN_NAMESPACE

/// \brief Constructor
McsfFilmingImageCommandHandler::McsfFilmingImageCommandHandler(
    ICommunicationProxy     *pCommProxy)
: m_pCommunicationProxy(pCommProxy)
{
    //TODO: initiate m_pMedViewerController
    m_wsViewerControllerConfigurePath = GetViewerControlConfigPath();
    m_pMedViewerController = NULL;
    m_pMedViewerController = MedViewerControllerFactory::Instance()->
        GetController(m_pCommunicationProxy->GetName(), m_wsViewerControllerConfigurePath,0);
}

/// \brief Destructor
McsfFilmingImageCommandHandler::~McsfFilmingImageCommandHandler()
{
    try
    {
        if(NULL != m_pMedViewerController)
        {
            delete m_pMedViewerController;
            m_pMedViewerController = NULL;
        }

    }
    catch(...)
    {

    }
}

/// \brief command handling function, override function of ICommandHandler interface
/// \param[in]      pContext        context of command
/// \param[in]      pReplyObject    pointer to the replied object for the command
/// \return         -1  means failed, 
///                 0   means success
int McsfFilmingImageCommandHandler::HandleCommand(
    const Mcsf::CommandContext* pContext, 
    std::string* pReplyObject)
{
    try
    {
        if (!pContext || !pReplyObject)
        {
            LOG_ERROR("Command Parameter is not valid");
            return -1;
        }

        //if (NULL == m_pMedViewerController)
        //{
        //    LOG_ERROR("No pMedViewerController to process image");
        //    return -1;
        //}
        int iRet;
        switch(pContext->iCommandId)
        {
        //case LOAD_IMAGE_BYPATH_COMMAND://5003
        //    LOG_INFO("begin to load image by path");
        //    return LoadImage(pContext->sSerializeObject);
        //    //LOG_INFO("end to load image by path");
        //    //break;
        case LOAD_IMAGE_COMMAND://6872
            LOG_INFO("begin to load image");
            AskFilmingFEToShowOnTop();
            return LoadImages(pContext->sSerializeObject, pReplyObject);
            //LOG_INFO("end to load image");
            //break;
      //  case LOAD_STUDY_COMMAND://6871
      //      LOG_INFO("begin to load study");
      //      AskFilmingFEToShowOnTop();
            //return LoadStudy(pContext->sSerializeObject);
            //LOG_INFO("end to load study");
            //break;
      //  case REMOVE_CELL_COMMAND://6875
      //      LOG_INFO("begin to remove cell");
            //return RemoveCell(pContext->sSerializeObject);
      //      //LOG_INFO("end to remove cell");
      //      //break;
      //  case REMOVE_IMAGE_COMMAND://5006
      //      LOG_INFO("begin to remove image");
            //return RemoveImage(pContext->sSerializeObject);
      //      //LOG_INFO("end to remove image");
      //      //break;
        case SAVE_IMAGE_COMMAND://5004
            LOG_INFO("begin to save image");
            return SaveImages(pContext->sSerializeObject, pReplyObject);
            //LOG_INFO("end to save image");
            //break;
        case SAVE_FILMS_COMMAND://7079
            LOG_INFO("begin to save films");
            iRet = SaveFilms(pContext->sSerializeObject, pReplyObject);
            LOG_INFO("end to save films");
            return iRet;
        case SAVE_EFILMS_COMMAND://7078
            LOG_INFO("begin to save e-films");
            iRet = SaveEFilms(pContext->sSerializeObject, pReplyObject);
            LOG_INFO("end to save e-films");
            return iRet;
      //  case SAVE_PS_COMMAND://5005
      //      LOG_INFO("begin to save PS information");
            //return SavePSInfo(pContext->sSerializeObject);
      //      //LOG_INFO("end to save PS information");
      //      //break;
      //  case SET_LAYOUT_COMMAND://5001
      //      LOG_INFO("begin to set layout");
            //return SetLayout(pContext->sSerializeObject);
      //      //LOG_INFO("end to set layout");
      //      //break;
      //  case UPDATE_MEMORY_COMMAND://11002
      //      LOG_INFO("begin to update memory");
            //return UpdateMemory(pContext->sSerializeObject);
      //      //LOG_INFO("end to update memory");
      //      //break;
        case REMOVE_ALL_COMMAND://6874
            LOG_INFO("begin to remove all images");
            return RemoveAll(pContext->sSerializeObject, pReplyObject);
        case CREATE_NEW_VIEWER_CONTROLLER:
            return CreateNewViewrController(pContext->sSerializeObject);
        default:
            LOG_WARN("command id is not valid for the filming-image-command-handler");
        }
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
    return -1;
}

std::vector<std::string> McsfFilmingImageCommandHandler::FilterInvalidPath(const std::vector<std::string>& filePathVector)
{
    std::vector<std::string> filteredFilePathVector;
    for (std::vector<std::string>::const_iterator it = filePathVector.begin(); it != filePathVector.end(); it++)
    {
        try
        {
            if (false == boost::filesystem::exists(boost::filesystem::path(*it)))
            {
                //lint -e665
                LOG_ERROR(string("the path which is : \"" ) 
                    + *it + "\" is not exist");
                //lint +e665
                continue;
            }

            filteredFilePathVector.push_back(*it);
        }
        catch (std::exception& e)
        {
            LOG_ERROR(e.what());
        }
        catch (...)
        {
            LOG_ERROR("general exception");
        }
    }
    return filteredFilePathVector;
}

int McsfFilmingImageCommandHandler::LoadImages( const std::string& sSerializedPara, std::string *pReplyObject )
{
    std::string sPath("");
    try
    {
        LOG_INFO(sSerializedPara);

        std::vector<std::string> imagePathVector;

        std::istringstream isInput(sSerializedPara);
        std:: string sTemp;
        while (getline(isInput, sTemp, ';'))
        {
            imagePathVector.push_back(sTemp);
        }

        imagePathVector = FilterInvalidPath(imagePathVector);

        //InformFilmingFECountOfImageLoading(imagePathVector.size());

        for (std::vector<std::string>::const_iterator it = imagePathVector.begin(); it != imagePathVector.end(); it++)
        {
            try
            {
                sPath = *it;
                //lint -e665
                LOG_INFO(string("begin to load image file : " ) + sPath);
                //lint +e665
    
                if (NULL == m_pMedViewerController)
                {
                    throw "m_pMedViewerController is NULL";
                }
                m_pMedViewerController->LoadImageByFilePath(sPath);
    
                //lint -e665
                LOG_INFO(string("end to load image file : ") + sPath);
                //lint +e665
            }
            catch (std::exception& e)
            {
                *pReplyObject = e.what();
                *pReplyObject += " When Loading image ";
                *pReplyObject +=  sPath;
                LOG_ERROR(*pReplyObject);
                //AskFilmingFEToUpdatePagesInfo();
                return -1;
            }
            catch (...)
            {//lint -e665
                *pReplyObject = "general exception";
                *pReplyObject += " When Loading image ";
                *pReplyObject +=  sPath;
                LOG_ERROR(*pReplyObject);
                //lint +e665
                //AskFilmingFEToUpdatePagesInfo();
                return -1;
            }
        }
        //AskFilmingFEToUpdatePagesInfo();
        Sleep(1000);
        *pReplyObject = "0";
        return 0;
    }
    catch(std::exception& e)
    {
        LOG_ERROR(e.what());
        //AskFilmingFEToUpdatePagesInfo();
        return -1;
    }
    catch (...)
    {
        LOG_ERROR("general exception");
        return -1;
    }
}

//int McsfFilmingImageCommandHandler::LoadImage( std::string sSerializedPara )
//{
//    try
//    {
//        std::vector<std::string> vecCmdParas;
//        //lint -e534
//        boost::split(vecCmdParas, sSerializedPara, boost::is_any_of(";"));
//        //lint +e534
//        // split the command string
//        if (3 != vecCmdParas.size())
//        {
//            std::string sError = "Invalid command parameter! [" + sSerializedPara + "].";
//            LOG_ERROR(sError);
//            return -1;
//        }
//
//        // 1st is file path
//        std::string sParam = vecCmdParas[1];
//        // 2nd is cell postion
//        std::string sCellIndex = vecCmdParas[2];
//        // todo: rcc, replace with boost::lexical_cast in the future
//        int iCellIndex = atoi(sCellIndex.c_str());
//        std::ostringstream os;
//        os << "Command parameter is :" << sParam << ", cellIndex:" << iCellIndex;
//        LOG_INFO(os.str());
//
//        if ("1" == vecCmdParas[0])
//        {
//            m_pMedViewerController->LoadImageByFilePath(sParam,iCellIndex);
//            LOG_INFO("Succeed to call LoadImageByPath of controller");
//        }
//        else
//        {
//            m_pMedViewerController->LoadImage(sParam,iCellIndex);
//        }
//
//
//        return 0;
//    }
//    catch(std::exception& e)
//    {
//        LOG_ERROR(e.what());
//        return -1;
//    }
//}

//int McsfFilmingImageCommandHandler::LoadImageByPath( std::string sSerializedPara )
//{
//    return -1;
//}

//int McsfFilmingImageCommandHandler::LoadStudy(std::string sSerializedPara)
//{
//    // Loading
//    try
//    {
//        std::string sStudyUID = sSerializedPara;
//        //notify the back-end to load image by stringUID
//        std::string sInfo = "studyUID is   ___________________________________>"
//            + sStudyUID;
//        LOG_INFO(sInfo);
//
//        m_pMedViewerController->LoadStudy(sStudyUID);
//
//        LOG_INFO("Succeed to load image in LoadImageCommandHandler::"
//            "HandleCommand of be containee.");
//        return 0;
//    }
//    catch(...)
//    {
//        LOG_ERROR("Exception happened in LoadImageCommandHandler::"
//            "HandleCommand of be containee.")
//            return -1;
//    }
//}

//int McsfFilmingImageCommandHandler::RemoveImage( std::string sSerializedPara )
//{
//    try
//    {
//        // pares the parameter from the command string
//        std::vector<std::string> vecStringPara;
//        //lint -e534
//        boost::split(vecStringPara, sSerializedPara, boost::is_any_of(";"));
//        //lint +e534
//        if (vecStringPara.size() != 2)
//        {
//            throw std::exception("Invalid command parameters received!");
//        }
//
//        // convert the parameter
//        int iCellIndex = boost::lexical_cast<int>(vecStringPara[0].c_str());
//        int iStackIndex  = boost::lexical_cast<int>(vecStringPara[1].c_str());
//
//        m_pMedViewerController->RemoveImage(iCellIndex, iStackIndex);
//
//        std::ostringstream os;
//        os << "Succeeded to remove image in cell index: " << iCellIndex
//            << ", stack index: " << iStackIndex;
//        LOG_INFO(os.str());
//
//        return 0;
//    }
//    catch(std::exception &exp)
//    {
//        LOG_ERROR(exp.what());
//        return -1;
//    }
//}

//int McsfFilmingImageCommandHandler::RemoveCell( const std::string& sSerializedPara )
//{
//    try
//    {
//        unsigned int uiCellIndex, uiCount;
//        std::istringstream is(sSerializedPara);
//        is >> uiCellIndex >> uiCount;
//
//        for(unsigned int i=0; i<uiCount; i++)
//        {
//            m_pMedViewerController->RemoveCell(uiCellIndex);
//        }
//        AskFilmingFEToUpdatePagesInfo();
//    }
//    catch (std::exception &exp)
//    {
//        LOG_ERROR(exp.what());
//        //AskFilmingFEToUpdatePagesInfo();
//        return -1;
//    }
//    catch (...)
//    {
//        LOG_ERROR("general exception");
//    }
//    return 0;
//}


int McsfFilmingImageCommandHandler::SaveFilms( const std::string& sSerializedPara, std::string *pReplyObject )
{
    //FilmsInfo filmsInfo;
    //try
    //{
    //    filmsInfo.ParseFromString(sSerializedPara);
    //}
    //catch (std::exception& e)
    //{
    //    *pReplyObject = string("Exception when parse serialized films info: ") 
    //       + ", " + e.what();
    //    LOG_ERROR(*pReplyObject);
    //    return 0;
    //}
    //catch (...)
    //{
    //    *pReplyObject = string("Exception when save film ") 
    //        + ", " + "general exception";
    //    LOG_ERROR(*pReplyObject);
    //    return 0;
    //}

    ////films
    //for (int i=0; i<filmsInfo.film_size(); i++)
    //{
    //    try
    //    {
    //        //add communication proxy name for multi instance
    //        FilmsInfo_FilmInfo film = filmsInfo.film(i);
    //        IMedViewerController* pMedViewerController = 
    //            MedViewerControllerFactory::Instance()->
    //            GetController(m_pCommunicationProxy->GetName(), m_wsViewerControllerConfigurePath,film.film_index());
    //        //std::cout<<"CommunicationProxyName=:"<<m_pCommunicationProxy->GetName()<<std::endl;
    //        //TRACE<<"CommunicationProxyName=:"<<m_pCommunicationProxy->GetName();
    //            //GetController(film.film_index(), m_wsViewerControllerConfigurePath);
    //        
    //        //images in a film
    //        for (int j = 0; j<film.image_size(); j++)
    //        {
    //            std::ostringstream os;
    //            try
    //            {
    //                //an image
    //                FilmsInfo_ImageInfo image = film.image(j);
    //                int iStackIndex = image.stack_index();
    //                int iCellIndex = image.cell_index();
    //                string sPath   = image.file();
    //                os << "(ImagePath,FilmIndex,ControllerIndex):("
    //                    <<sPath<<","<<i<< "," << film.film_index() <<")." <<endl;
    //                LOG_INFO(os.str());
    //    
    //                pMedViewerController->SaveImageByPath(iCellIndex, iStackIndex, sPath);
    //                os.clear();
    //            }
    //            catch (std::exception& e)
    //            {
    //                *pReplyObject = string("Exception when save image at ") 
    //                    + os.str() + ", " +e.what();
    //                //lint -e665
    //                LOG_ERROR(*pReplyObject);
    //                //lint +e665
    //                return 0;
    //            }
    //            catch (...)
    //            {
    //                *pReplyObject = string("Exception when save image at ")
    //                    + os.str() + ", " + "general exception";
    //                //lint -e665
    //                LOG_ERROR(*pReplyObject);
    //                //lint +e665
    //                return 0;
    //            }
    //    
    //        }
    //    }
    //    catch (std::exception& e)
    //    {
    //        *pReplyObject = string("Exception when save film ") 
    //            + boost::lexical_cast<string>(i) + " " + e.what();
    //        LOG_ERROR(*pReplyObject);
    //        return 0;
    //    }
    //    catch (...)
    //    {
    //        *pReplyObject = string("Exception when save film ") 
    //            + boost::lexical_cast<string>(i) + " " + "general exception";
    //        LOG_ERROR(*pReplyObject);
    //        return 0;
    //    }
    //}
    //Sleep(1000);
    //*pReplyObject = "0";
	*pReplyObject = sSerializedPara;
    return 0;
}

int McsfFilmingImageCommandHandler::SaveImages( const std::string& sSerializedPara, std::string *pReplyObject )
{

        //// pares the parameter from the command string
        //std::vector<std::string> vecStringPara;
        //std::istringstream isInput(sSerializedPara);
        //std:: string sTemp;
        //while (getline(isInput, sTemp, ';'))
        //{
        //    vecStringPara.push_back(sTemp);
        //}
        //////lint -e534
        ////vecStringPara =  boost::split(vecStringPara, sSerializedPara, boost::is_any_of(";"));
        //////lint +e534
        //if (vecStringPara.size() < 3)
        //{
        //    LOG_ERROR("Invalid command parameters received!");
        //    *pReplyObject = "Invalid command parameters received: ";
        //    *pReplyObject += sSerializedPara;
        //    return 0;
        //}

        //// convert the parameter
        //int iStackIndex;  
        //int iCellIndex;   
        //std::string sPath;
        //std::ostringstream os;
        //for (std::vector<std::string>::const_iterator it = vecStringPara.begin(); it!=vecStringPara.end(); it++)
        //{
        //    try
        //    {
        //        iStackIndex     = atoi(it->c_str());
        //        it++;  if(it == vecStringPara.end()) break;
        //        iCellIndex      = atoi(it->c_str());
        //        it++;  if(it == vecStringPara.end()) break;
        //        sPath   = *it;
        //        os << "(StackIndex,CellIndex,ImagePath):("<<iStackIndex<<","<<iCellIndex<< "," << sPath <<").";
        //        LOG_INFO(os.str());

        //        if (NULL == m_pMedViewerController)
        //        {
        //            throw "m_pMedViewerController is NULL";
        //        }

        //        //if (false == boost::filesystem::exists(boost::filesystem::path(*it)))
        //        //{
        //        //    std::string sLog = "the path which is : \"" ;
        //        //    sLog += *it;
        //        //    sLog += "\" is not exist";
        //        //    LOG_ERROR(sLog);
        //        //    throw sLog;
        //        //}
        //        m_pMedViewerController->SaveImageByPath(iStackIndex, iCellIndex, sPath);
    
        //        LOG_INFO("Succeeded to save a image in SaveImageCmdHandler::"
        //            "HandleCommand of viewer be containee.");
        //    }
        //    catch (std::exception& e)
        //    {
        //        *pReplyObject = "Exception when save image at ";
        //        *pReplyObject += os.str();
        //        *pReplyObject += " ";
        //        *pReplyObject += e.what();
        //        //lint -e665
        //        LOG_ERROR(*pReplyObject);
        //        //lint +e665
        //        return 0;
        //    }
        //    catch (...)
        //    {
        //        *pReplyObject = "Exception when save image at ";
        //        *pReplyObject += os.str();
        //        *pReplyObject += " ";
        //        *pReplyObject += "general exception";
        //        //lint -e665
        //        LOG_ERROR(*pReplyObject);
        //        //lint +e665
        //        return 0;
        //    }

        //}//lint +e1013 auto
        //Sleep(1000);
        //*pReplyObject = "0";
		*pReplyObject = sSerializedPara;
        return 0;
}


//int McsfFilmingImageCommandHandler::SaveImage(std::string sSerializedPara)
//{
//	try
//	{
//		// pares the parameter from the command string
//		std::vector<std::string> vecStringPara;
//        //lint -e534
//		boost::split(vecStringPara, sSerializedPara, boost::is_any_of(";"));
//        //lint +e534
//		if (vecStringPara.size() < 3)
//		{
//			LOG_ERROR("Invalid command parameters received!");
//			return -1;
//		}
//
//		// convert the parameter
//		int iStackIndex = atoi(vecStringPara[0].c_str());
//		int iCellIndex  = atoi(vecStringPara[1].c_str());
//        std::string sPath = vecStringPara[2];
//        std::ostringstream os;
//        os << "(StackIndex,CellIndex,ImagePath):("<<iStackIndex<<","<<iCellIndex<< "," << sPath <<").";
//		LOG_INFO(os.str());
//
//        m_pMedViewerController->SaveImageByPath(iStackIndex, iCellIndex, sPath);
//
//		LOG_INFO("Succeeded to save image in SaveImageCmdHandler::"
//			"HandleCommand of viewer be containee.");
//
//        return 0;
//    }
//    catch(...)
//    {
//        LOG_ERROR("Exception happened in SaveImageCmdHandler::"
//            "HandleCommand of viewer be containee.");
//        return -1;
//    }
//}

//int McsfFilmingImageCommandHandler::SavePSInfo(std::string sSerializedPara)
//{
//	try
//	{
//		// pares the parameter from the command string
//		std::vector<std::string> vecStringPara;
//        //lint -e534
//		boost::split(vecStringPara, sSerializedPara, boost::is_any_of(";"));
//        //lint +e534
//		if (vecStringPara.size() != 2)
//		{
//			LOG_ERROR("Invalid command parameters received!");
//			return -1;
//		}
//
//		// convert the parameter
//		int iStackIndex = atoi(vecStringPara[0].c_str());
//		int iCellIndex  = atoi(vecStringPara[1].c_str());
//        std::ostringstream os;
//        os << "(StackIndex,CellIndex):("<<iStackIndex<<","<<iCellIndex<<").";
//		LOG_INFO(os.str());
//
//		m_pMedViewerController->SavePS(iCellIndex,iStackIndex);
//
//		return 0;
//	}
//	catch(std::exception& e)
//	{
//		LOG_ERROR(e.what());
//		return -1;
//	}
//}

//int McsfFilmingImageCommandHandler::SetLayout(std::string sSerializedPara)
//{
//	try
//	{
//		size_t uPos1 = sSerializedPara.find_first_of (',');
//		std::string sFirstStr = sSerializedPara.substr(0,uPos1-0);
//		size_t uPos2 = sSerializedPara.find_first_of (',' , uPos1+1);
//		std::string sSecondStr = sSerializedPara.substr(uPos1+1,(uPos2-uPos1-1));
//		int row = 0;   
//		std::stringstream ssData1(sFirstStr);
//		ssData1 >> row;
//		int col = 0;
//		std::stringstream ssData2(sSecondStr);
//		ssData2 >> col;
//		m_pMedViewerController->SetLayout(row,col);
//
//		return 0; 
//	}
//	catch (std::exception &exp)
//	{
//		LOG_ERROR(exp.what());
//		return -1;
//	}
//}

//int McsfFilmingImageCommandHandler::UpdateMemory(std::string sSerializedPara)
//{
//	int iWidth = 512;
//	int iHeight = 512;
//	void *pData0 = NULL, *pData1 = NULL, *pData2 = NULL, *pData3 = NULL;
//	m_pMedViewerController->CreateCellImageBuffer(iWidth, iHeight, BGRA32, 0, pData0);
//	m_pMedViewerController->CreateCellImageBuffer(iWidth, iHeight, BGRA32, 1, pData1);
//	m_pMedViewerController->CreateCellImageBuffer(iWidth, iHeight, BGRA32, 2, pData2);
//	m_pMedViewerController->CreateCellImageBuffer(iWidth, iHeight, BGRA32, 3, pData3);
//	//if (!m_pMedViewerController->CreateCellImageBuffer(iWidth,iHeight,BGRA32,0,pData))
//	//{
//	//    throw std::exception("Fail to create cell image buffer");
//	//}
//	//::Sleep(20);
//
//	try
//	{
//		unsigned int colors[] = {0xFF0000FF, 0xFF00FF00, 0xFFFF0000};
//		int dex = 0;
//		for (int iIndex = 0; iIndex < 9; iIndex++)
//		{
//			for (int iHeight = 0; iHeight < 512; iHeight++)
//			{
//				for (int iWidth = 0; iWidth < 512; iWidth++)
//				{   //lint -e679
//					dex = iHeight*512 + iWidth;
//					(static_cast<int*>(pData0))[dex] = colors[iIndex % 3];
//					(static_cast<int*>(pData1))[dex] = colors[(iIndex + 1) % 3];
//					(static_cast<int*>(pData2))[dex] = colors[(iIndex + 2) % 3];
//					(static_cast<int*>(pData3))[dex] = colors[(iIndex + 3) % 3];
//				}//lint +e679
//			}
//			m_pMedViewerController->UpdateCellImageBuffer(0);
//			m_pMedViewerController->UpdateCellImageBuffer(1);
//			m_pMedViewerController->UpdateCellImageBuffer(2);
//			m_pMedViewerController->UpdateCellImageBuffer(3);
//            //lint -e534
//			ACE_OS::sleep(1);
//		}   //lint +e534
//	
//		m_pMedViewerController->ReleaseCellImageBuffer(0);
//		m_pMedViewerController->ReleaseCellImageBuffer(1);
//		m_pMedViewerController->ReleaseCellImageBuffer(2);
//		m_pMedViewerController->ReleaseCellImageBuffer(3);
//	
//		return 0;
//	}
//    catch(std::exception& e)
//    {
//    	LOG_ERROR(e.what());
//    	return -1;
//    }
//}

int McsfFilmingImageCommandHandler::CreateNewViewrController(const std::string& sSerializedPara)
{
    std::cout<<"CreateNewViewerController"<<std::endl;

    int iIndex = boost::lexical_cast<int>(sSerializedPara);

    //modify for support multi instance
    //MedViewerControllerFactory::Instance()->GetController(iIndex, m_wsViewerControllerConfigurePath);
    MedViewerControllerFactory::Instance()->GetController(m_pCommunicationProxy->GetName(),m_wsViewerControllerConfigurePath, iIndex);

    return 0;
}

int McsfFilmingImageCommandHandler::RemoveAll( const std::string& sSerializedPara, std::string *pReplyObject )
{
    try
    {
        std::istringstream is(sSerializedPara);
        int iSleep = 1;
        is >> iSleep;

        if (NULL == m_pMedViewerController)
        {
            throw "m_pMedViewerController is NULL";
        }
        m_pMedViewerController->RemoveAll();
        LOG_INFO("Succeed to remove all images");

        Sleep(iSleep*1000);

        AskFilmingFEToUpdatePagesInfo();
        *pReplyObject = "0";

        return 0;
    }
    catch (std::exception &exp)
    {
        *pReplyObject = "Exception When Removing all images : ";
        *pReplyObject += exp.what();
        LOG_ERROR(*pReplyObject);
        return -1;
    }
    catch (...)
    {//lint -e665
        *pReplyObject = "Exception When Removing all images : ";
        *pReplyObject += "general exception";
        LOG_ERROR(*pReplyObject);
        return -1;
    }//lint +e665
}

void McsfFilmingImageCommandHandler::AskFilmingFEToShowOnTop()
{
    try
    {
        LOG_INFO("Filming Viewer FE , please show up ");
        CommandContext filmingCmdContext;
        //filmingCmdContext.sReceiver = FILMING_FE_CMD_RECEIVER;
        filmingCmdContext.sReceiver = CommunicationNodeName::GetPeerCommunicationProxyName(m_pCommunicationProxy->GetName(), "FE");
        filmingCmdContext.iCommandId = static_cast<int>(SHOW_ON_TOP_COMMAND);
        std::string sSerializedString = "";
        if(-1 == m_pCommunicationProxy->SyncSendCommand(&filmingCmdContext, sSerializedString))
        {
            LOG_ERROR("Not asking FilmingFEToShowOnTop");
        }
        LOG_INFO("Asked FilmingFE to show on top");
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

void McsfFilmingImageCommandHandler::AskFilmingFEToUpdatePagesInfo()
{
    try
    {
        LOG_INFO("Filming Viewer FE , please update image page info ");
        CommandContext filmingCmdContext;
        //filmingCmdContext.sReceiver = FILMING_FE_CMD_RECEIVER;
        filmingCmdContext.sReceiver = CommunicationNodeName::GetPeerCommunicationProxyName(m_pCommunicationProxy->GetName(), "FE");
        filmingCmdContext.iCommandId = static_cast<int>(REMOVING_ALL_IMAGES_COMMAND);
        filmingCmdContext.pCommandCallback = NULL;
       // filmingCmdContext.bServiceAsyncDispatch = false;
        if(-1 == m_pCommunicationProxy->AsyncSendCommand(&filmingCmdContext))
        {
            LOG_ERROR("Ask FilmingFEToUpdateImagePageInfo failure");
        }
        LOG_INFO("Asked FilmingFE to update image page info");
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

void McsfFilmingImageCommandHandler::InformFilmingFECountOfImageLoading( unsigned int uiCount )
{
    try
    {
        LOG_INFO(string("Count of Image Loading is ") 
            + boost::lexical_cast<string>(uiCount));

        CommandContext filmingCmdContext;
        //filmingCmdContext.sReceiver = FILMING_FE_CMD_RECEIVER;
        filmingCmdContext.sReceiver = CommunicationNodeName::GetPeerCommunicationProxyName(m_pCommunicationProxy->GetName(), "FE");
        filmingCmdContext.iCommandId = static_cast<int>(COUNT_OF_IMAGES_LOADING_COMMAND);
       // filmingCmdContext.bServiceAsyncDispatch = true;
        filmingCmdContext.pCommandCallback = NULL;

        std::ostringstream os;
        os << uiCount;
        filmingCmdContext.sSerializeObject = os.str();

        if(-1 == m_pCommunicationProxy->AsyncSendCommand(&filmingCmdContext))
        {
            LOG_ERROR("inform FilmingFE failure");
        }
        LOG_INFO("informed FilmingFE");
    }
    catch (std::exception& e)
    {
        LOG_ERROR(e.what());
    }
    catch(...) 
    {
        LOG_ERROR("general exception");
    }
}

wstring McsfFilmingImageCommandHandler::GetViewerControlConfigPath()
{
    //get system config file path
    Mcsf::ISystemEnvironmentConfig* pSystemEnviromentConfig = 
        Mcsf::ConfigSystemEnvironmentFactory::Instance()->GetSystemEnvironment();

    std::string sConfigFilePath = 
        pSystemEnviromentConfig->GetApplicationPath(
        MCSF_FILMING_CONFIG_PATH_TAG_IN_SYSTEM_ENVIRONMENT);
    wstring wsFilePath; 

    //lint -e534
    wsFilePath.assign(sConfigFilePath.begin(), sConfigFilePath.end());
    //lint +e534

    delete pSystemEnviromentConfig;
    pSystemEnviromentConfig = NULL;

    return wsFilePath;
}

int McsfFilmingImageCommandHandler::SaveEFilms( const std::string& sSerializedPara, std::string *pReplyObject )
{
    std::string sPath("");
    try
    {
        if(!DBImporterInit())
        {
            LOG_ERROR("DB importer now is not usable");
            *pReplyObject = "DB importer now is not usable";
            return 0;
        }

        LOG_INFO(sSerializedPara);

        std::vector<std::string> imagePathVector;

        std::istringstream isInput(sSerializedPara);
        std:: string sTemp;
        while (getline(isInput, sTemp, ';'))
        {
            imagePathVector.push_back(sTemp);
        }

        for (std::vector<std::string>::const_iterator it = imagePathVector.begin(); it != imagePathVector.end(); it++)
        {
            try
            {
                sPath = *it;

#ifdef _DEBUG
                std::string sSOPInstanceUID("");
                if(!ImportDicomFileToDBWithUID(sPath,sSOPInstanceUID))
                {
                    LOG_ERROR("import file " + sPath + " failure");
                    *pReplyObject = "import file " + sPath + " failure";
                    return 0;
                }
                else
                {
                    LOG_INFO("import file " + sPath + " success, SOPInstanceUID is " + sSOPInstanceUID);
                }
#else
                if(!ImportDicomFileToDB(sPath))
                {
                    LOG_ERROR("import file " + sPath + " failure");
                    *pReplyObject = "import file " + sPath + " failure";
                    return 0;
                }
#endif
            }
            catch (std::exception& e)
            {
                *pReplyObject = e.what();
                *pReplyObject += " When Saving E-films";
                *pReplyObject +=  sPath;
                LOG_ERROR(*pReplyObject);
                //AskFilmingFEToUpdatePagesInfo();
                return -1;
            }
            catch (...)
            {//lint -e665
                *pReplyObject = "general exception";
                *pReplyObject += " When Saving E-films";
                *pReplyObject +=  sPath;
                LOG_ERROR(*pReplyObject);
                //lint +e665
                //AskFilmingFEToUpdatePagesInfo();
                return -1;
            }
        }
        //AskFilmingFEToUpdatePagesInfo();
        Sleep(1000);
        *pReplyObject = "0";
        return 0;
    }
    catch(std::exception& e)
    {
        LOG_ERROR(e.what());
        //AskFilmingFEToUpdatePagesInfo();
        return -1;
    }
    catch (...)
    {
        LOG_ERROR("general exception");
        return -1;
    }
}

MCSF_FILMING_END_NAMESPACE
