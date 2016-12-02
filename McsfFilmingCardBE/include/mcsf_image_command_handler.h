//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging Healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Wang Hui   (mailto:hui.wang@united-imaging.com)
///           
///  \file    mcsf_image_command_handler.h
///  \brief   define the filming containee command callback functions and
///           process functions for processing images
///
///  \version 1.0
///  \date    Nov. 23, 2011
///  
//////////////////////////////////////////////////////////////////////////
#ifndef MCSF_IMAGE_COMMAND_HANDLER_H__
#define MCSF_IMAGE_COMMAND_HANDLER_H__

//container interface include files
#include "McsfNetBase/mcsf_communication_proxy.h"
#include "McsfNetBase/mcsf_netbase_interface.h"

//inner include files
#include "mcsf_filmingcard_config.h"

using namespace std;

MCSF_FILMING_BEGIN_NAMESPACE

// foward declaration
class ICommunicationProxy;
class IMedViewerController;

class McsfFilmingImageCommandHandler : public ICommandHandler
{
public:
    /// \fn     LoadImageByFilePathCmdHndler
    /// \brief  Constructor
    McsfFilmingImageCommandHandler(
        ICommunicationProxy     *pCommProxy
        );

    /// \fn     ~LoadImageByFilePathCmdHndler
    /// \brief  destructor
    virtual ~McsfFilmingImageCommandHandler();

    /// \fn	   HandleCommand(const Mcsf::CommandContext* pContext, Mcsf::IMarshalObject** pReplyObject)
    /// \brief        handle command of command_id equaled to 6000 ,notify the back-end to 
    ///               load image by stringUID;if LoadImage successfully,send reply
    ///               message 1,else 0;
    virtual int HandleCommand(
        const Mcsf::CommandContext* pContext, 
        std::string* pReplyObject);

private:

    wstring GetViewerControlConfigPath();

    /////////////////////////////////////////////////////////////////
    ///  \brief     filter invalid file path from a file path vector 
    ///
    ///  \param[in]    const std::vector<std::string>& filePathVector
    ///                     the file path vector to be filtered      
    ///  \return       std::vector<std::string>
    ///                     the filtered file path vector
    ///  \pre \e  
    /////////////////////////////////////////////////////////////////
    std::vector<std::string> FilterInvalidPath(const std::vector<std::string>& filePathVector);

    int LoadImages(const std::string& sSerializedPara, std::string *pReplyObject);

    //int SaveImage(std::string sSerializedPara);

    int SaveImages(const std::string& sSerializedPara, std::string *pReplyObject);

    int SaveFilms(const std::string& sSerializedPara, std::string *pReplyObject);

    int SaveEFilms(const std::string& sSerializedPara, std::string *pReplyObject);

    int RemoveAll(const std::string& sSerializedPara, std::string *pReplyObject);

    int CreateNewViewrController(const std::string& sSerializedPara);

    //int LoadImage(std::string sSerializedPara);

    //int LoadImageByPath(std::string sSerializedPara);

    //int LoadStudy(std::string sSerializedPara);

    //int RemoveImage(std::string sSerializedPara);

    //int RemoveCell(const std::string& sSerializedPara);

    //int SavePSInfo(std::string sSerializedPara);

    //int SetLayout(std::string sSerializedPara);

    //int UpdateMemory(std::string sSerializedPara);

    void AskFilmingFEToShowOnTop();

    void AskFilmingFEToUpdatePagesInfo();

    void InformFilmingFECountOfImageLoading(unsigned int uiCount);

    /// \brief pointer to the controller instance
    IMedViewerController *m_pMedViewerController;

    /// \brief pointer to the communication proxy
    ICommunicationProxy *m_pCommunicationProxy;

    wstring m_wsViewerControllerConfigurePath;

    MCSF_FILMING_DISALLOW_COPY_AND_ASSIGN(McsfFilmingImageCommandHandler);
};

MCSF_FILMING_END_NAMESPACE

#endif // MCSF_IMAGE_COMMAND_HANDLER_H__
