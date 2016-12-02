//////////////////////////////////////////////////////////////////////////
/// 
///  Copyright, (c) Shanghai United Imaging healthcare Inc., 2011
///  All rights reserved.
///
///  \author  Mu PengXuan  pengxun.mu@united-imaging.com
///
///  \file    mcsf_dcm_print_config.cpp
///  \brief   the print parameters init class
///
///  \version 1.0
///  \date    Oct. 20, 2011
///  
//////////////////////////////////////////////////////////////////////////

#include "mcsf_dcm_film_session_object.h"

MCSF_FILMING_BEGIN_NAMESPACE

McsfDcmFilmSessionObject::McsfDcmFilmSessionObject(std::string sPrinterAE,
    std::string sPrinterIP,
    unsigned int iPrinterPort,
    std::string sLayout):
    m_iPrinterPort(0),
    m_lMaxPDU(32768),
    m_bImplicitOnly(false),
    m_bDisableVRs(false),
    m_iMaxDensity(320),
    m_iMinDensity(20)
{
    if(sPrinterAE.length()>0)
    {
        m_sPrinterAE = sPrinterAE;
    }
    else
    {
        m_sPrinterAE = "UNKNOWAE";
    }

    m_sPrinterIP = sPrinterIP;
    m_iPrinterPort = iPrinterPort;
    m_sLayout = sLayout;
}

McsfDcmFilmSessionObject::McsfDcmFilmSessionObject(void)
{

}

McsfDcmFilmSessionObject::~McsfDcmFilmSessionObject(void)
{
}

const std::string& McsfDcmFilmSessionObject::GetTargetHostname() const
{
    if(this->m_sPrinterIP.length()>0)
    {
        return this->m_sPrinterIP;
    }
    //Error
    return EMPTY_STRING;
    
}

const std::string& McsfDcmFilmSessionObject::GetMagnification() const
{
    return m_sMagnification;
}

bool McsfDcmFilmSessionObject::GetDisableNewVRs() const
{
    return this->m_bDisableVRs;
}

bool McsfDcmFilmSessionObject::GetPresentationLUTinFilmSession() const
{
	return this->m_bPresentationLUTinFilmSession;
}

int McsfDcmFilmSessionObject::GetMaxDensity() const
{
    return this->m_iMaxDensity;
}

int McsfDcmFilmSessionObject::GetMinDensity() const
{
    return this->m_iMinDensity;
}

const std::string& McsfDcmFilmSessionObject::GetTargetAETitle() const
{
    if(this->m_sPrinterAE.length()>0)
    {
        return this->m_sPrinterAE;
    }
    //error
    return DEFAULT_AE;
}

const std::string& McsfDcmFilmSessionObject::GetImageDisplayFormat() const
{
    if(this->m_sLayout.length()>0)
    {
        return this->m_sLayout;
    }
    //warning
    return DEFAULT_LAYOUT;//default??
    
}

const std::string& McsfDcmFilmSessionObject::GetPrinterDescription() const
{
    return this->m_sPrinterDescription;
}

const unsigned int McsfDcmFilmSessionObject::GetPrinterPort() const
{
    return this->m_iPrinterPort;
}

const std::string& McsfDcmFilmSessionObject::GetPrinterIP() const
{
    return this->m_sPrinterIP;
}

const unsigned long McsfDcmFilmSessionObject::GetMaxPDU() const
{
    return this->m_lMaxPDU;
}

const bool McsfDcmFilmSessionObject::GetImplictOnly() const
{
    return this->m_bImplicitOnly;
}

void McsfDcmFilmSessionObject::SetPrinterID( const std::string& sPrinterID )
{
    this->m_sPrinterID = sPrinterID;
}

void McsfDcmFilmSessionObject::SetTargetHostname( const std::string& sTargetHostname )
{
    this->m_sPrinterIP = sTargetHostname;
}

void McsfDcmFilmSessionObject::SetTargetAETitle( const std::string& sTargetAETitle )
{
    this->m_sPrinterAE = sTargetAETitle;
}

void McsfDcmFilmSessionObject::SetImageDisplayFormat( const std::string& sImageDisplayFormat )
{
    m_sLayout = sImageDisplayFormat;
}

void McsfDcmFilmSessionObject::SetPrinterDescription( const std::string& sPrinterDescription )
{
    this->m_sPrinterDescription = sPrinterDescription;
}

void McsfDcmFilmSessionObject::SetPrinterPort( const unsigned int uiPrinterPort )
{
    this->m_iPrinterPort = uiPrinterPort;
}

void McsfDcmFilmSessionObject::SetPrinterIP( const std::string& sPrinterIP )
{
    this->m_sPrinterIP = sPrinterIP;
}

void McsfDcmFilmSessionObject::SetMaxPDU( const unsigned long ulMaxPDU )
{
    this->m_lMaxPDU = ulMaxPDU;
}

void McsfDcmFilmSessionObject::SetImplictOnly( const bool bImplictOnly )
{
    this->m_bImplicitOnly = bImplictOnly;
}

const std::string& McsfDcmFilmSessionObject::GetPrinterID() const
{
    return m_sPrinterID;
}

const std::string& McsfDcmFilmSessionObject::GetBorderDensity() const
{
    return this->m_sBorderDensity;
}

const std::string& McsfDcmFilmSessionObject::GetEmptyImageDensity() const
{
    return this->m_sEmptyImageDensity;
}

const std::string& McsfDcmFilmSessionObject::GetFilmDestination() const
{
    return EMPTY_STRING;
}

const std::string& McsfDcmFilmSessionObject::GetFilmSize() const
{
    return this->m_sFilmSize;
}

const std::string& McsfDcmFilmSessionObject::GetMediumType() const
{
    return this->m_sMediumType;
}

MCSF_FILMING_END_NAMESPACE
