//////////////////////////////////////////////////////////////////
///  Copyright (c) Shanghai United Imaging Healthcare Inc., 2011
///  All rights reserved.
/// 
///  \author    qingzhen.ma  (mailto:qingzhen.ma@united-imaging.com)
/// 
///  \file      mcsf_display_info_accessor.h
/// 
///  \brief     class DisplayInfoAccessor declaration 
/// 
///  \version   1.0
/// 
///  \date      11/11/2013/
////////////////////////////////////////////////////////////////

#ifndef MCSF_EFILM_INFO_ACCESSOR_H
#define MCSF_EFILM_INFO_ACCESSOR_H

#include <memory>

#include "boost/shared_ptr.hpp"

#include "McsfAppCommon/mcsf_application_common.h"
#include "McsfDataHeader/mcsf_data_header_element_map_interface.h"

#include "mcsf_app_image_info_accessor_interface.h"

MCSF_APPLICATION_BEGIN_NAMESPACE   // begin namespace Mcsf

class IDatabase;
typedef boost::shared_ptr<IDatabase> IDatabasePtr;
//////////////////////////////////////////////////////////////////////////
/// \class  DisplayInfoAccessor
/// \brief      
//////////////////////////////////////////////////////////////////////////
class EFilmInfoAccessor : public ImageInfoAccessorInterface
{
public:

    //////////////////////////////////////////////////////////////////////
    /// \brief  Constructor
    //////////////////////////////////////////////////////////////////////
    EFilmInfoAccessor(IDatabasePtr pDatabase);

    //////////////////////////////////////////////////////////////////////
    /// \brief  Destructor
    //////////////////////////////////////////////////////////////////////
    virtual ~EFilmInfoAccessor();

    virtual IDataHeaderElementMap::Ptr GetPixelData(const int iCellIndex, const std::string& sSOPInstanceUID, const std::string& ksPS);

    virtual IDataHeaderElementMap::Ptr GetHeaderInfo(const int iCellIndex, const std::string& sSOPInstanceUID);

private:
    IDatabasePtr                               m_pDatabase;
    IDataHeaderElementMap::Ptr                 m_pOriHeaderInfo;

private:
    DISALLOW_COPY_AND_ASSIGN(EFilmInfoAccessor);

};

MCSF_APPLICATION_END_NAMESPACE  // end namespace Mcsf

#endif