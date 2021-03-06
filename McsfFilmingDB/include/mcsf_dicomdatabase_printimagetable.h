//////////////////////////////////////////////////////////////////////////
/// \defgroup McsfDatabase of Common Software Business Unit
///  Copyright, (c) Shanghai United Imaging Healthcare Inc., 2011
///  All rights reserved.
///
///  \author  ZHOU qiangqiang  qiangqiang.zhou@united-imaging.com
///
///  \file       mcsf_dicomdatabase_printimagetable.h
///  \brief   This file was generated by CodeGenerater.exe 
///              which translates database script into classes supported by ODB
///
///  \version 1.0
///  \date    11/14/2011
///  \{
//////////////////////////////////////////////////////////////////////////

#ifndef MCSF_DICOMDATABASE_PRINTIMAGETABLE_H
#define MCSF_DICOMDATABASE_PRINTIMAGETABLE_H
#include <string>
#include <odb/core.hxx>

#include <boost/date_time/gregorian/gregorian_types.hpp>// for posix_time::ptime --DATETIME
#include <boost/date_time/posix_time/posix_time_types.hpp>

#pragma db object
class PrintImageTable
{
public:
    typedef boost::posix_time::ptime DATETIME_BOOST;
    typedef boost::posix_time::time_duration TIME_BOOST;
    typedef boost::gregorian::date DATE_BOOST;
PrintImageTable (
const std::string& Pk,
const int& FilmID,
const std::string& Path,
const int& ImagePosotion,
const DATE_BOOST& CreateDate,
const TIME_BOOST& CreateTime,
const int& HaveUsed
):
m_Pk(Pk),
m_FilmID(FilmID),
m_Path(Path),
m_ImagePosotion(ImagePosotion),
m_CreateDate(CreateDate),
m_CreateTime(CreateTime),
m_HaveUsed(HaveUsed)
{
}

const std::string& GetPk () const 
{
	return m_Pk;
}
void SetPk (const std::string& Pk)
{
	m_Pk = Pk;
}

const int& GetFilmID () const 
{
	return m_FilmID;
}
void SetFilmID (const int& FilmID)
{
	m_FilmID = FilmID;
}

const std::string& GetPath () const 
{
	return m_Path;
}
void SetPath (const std::string& Path)
{
	m_Path = Path;
}

const int& GetImagePosotion () const 
{
	return m_ImagePosotion;
}
void SetImagePosotion (const int& ImagePosotion)
{
	m_ImagePosotion = ImagePosotion;
}

const DATE_BOOST& GetCreateDate () const 
{
	return m_CreateDate;
}
void SetCreateDate (const DATE_BOOST& CreateDate)
{
	m_CreateDate = CreateDate;
}

const TIME_BOOST& GetCreateTime () const 
{
	return m_CreateTime;
}
void SetCreateTime (const TIME_BOOST& CreateTime)
{
	m_CreateTime = CreateTime;
}

const int& GetHaveUsed () const 
{
	return m_HaveUsed;
}
void SetHaveUsed (const int& HaveUsed)
{
	m_HaveUsed = HaveUsed;
}

private:
friend class odb::access;

PrintImageTable () {}

//7 parameters


#pragma db id type("varchar(64)")
std::string m_Pk;
//comment

#pragma db type("int not null")
int m_FilmID;
//comment

#pragma db type("varchar(256)")
std::string m_Path;
//comment

#pragma db type("int")
int m_ImagePosotion;
//comment

#pragma db type("date")
DATE_BOOST m_CreateDate;
//comment

#pragma db type("time")
TIME_BOOST m_CreateTime;
//comment

#pragma db type("int")
int m_HaveUsed;
//comment
};

#endif // MCSF_DICOMDATABASE_PRINTIMAGETABLE_H
