using System;
using UIH.Mcsf.Database;
using System.Collections.Generic;
using System.Diagnostics;
using UIH.Mcsf.App.Common;

namespace UIH.Mcsf.Filming.Wrappers
{
    public class FilmingDbOperation
    {
        private readonly DBWrapper _dbWrapper = DBWrapperHelper.DBWrapper;//new DBWrapper();
        public DBWrapper FilmingDbWrapper
        {
            get 
            {
                return _dbWrapper; 
            }
        }

        private static FilmingDbOperation _instance;

        //private DataRepositry _dataRepositry = DataRepositry.Instance;
        //public DataRepositry FilmingDataRepositry
        //{
        //    get { return _dataRepositry; }
        //    set { _dataRepositry = value; }
        //}

        public static FilmingDbOperation Instance
        {
            get { return _instance ?? (_instance = new FilmingDbOperation()); }
        }

        private FilmingDbOperation()
        {
            //bool bInit = _dbWrapper.Initialize();
            //_dbWrapper.SetAutoNotifyOn(ComProxyManager.GetCurrentProxy());
            //if (!bInit)
            //{
            //    throw new Exception("DBWrapper.Initialize failed!");
            //}
            Debug.Assert(_dbWrapper != null);
        }

        //public void LoadAllStudy()
        //{
        //    //step 1: get patient level and study level information
        //    var studyList = _dbWrapper.GetAllStudyList();
        //    FilmingDataRepositry.Studys.Clear();
        //    foreach (var study in studyList)
        //    {
        //        var studyData = new StudyData();
        //        var patient = _dbWrapper.GetPatientByPatientUID(study.PatientUIDFk);
        //        if (patient == null)
        //        {
        //            return;
        //        }

        //        if (study.StudyConfirmStatus != 0)
        //        {
        //            continue;
        //        }

        //        studyData.PatientID = patient.PatientID;
        //        studyData.PatientName = patient.PatientName;
        //        studyData.PatientBirthday = patient.PatientBirthDate.ToString();
        //        studyData.PatientStudyAge = study.PatientAge;
        //        studyData.PatientSex = patient.PatientSex;
        //        studyData.StudyDate = study.StudyDate;
        //        studyData.AccessionNumber = study.AccessionNumber;
        //        studyData.StudyUid = study.StudyInstanceUID;
        //        studyData.Description = study.StudyDescription;
        //        studyData.Modality = study.ModalitiesInStudy;

        //        //step 2: get series level information
        //        var seriesList = _dbWrapper.GetSeriesListByStudyInstanceUID(study.StudyInstanceUID);
        //        if (seriesList == null)
        //        {
        //            Logger.LogError("NUll series list for studyUID:!" + study.StudyInstanceUID);
        //            return;
        //        }

        //        foreach (var seriesBase in seriesList)
        //        {
        //            if (seriesBase.SeriesConfirmStatus != 0)
        //            {
        //                continue;
        //            }

        //            var seriesData = new SeriesData(seriesBase.SeriesInstanceUID)
        //                                 {
        //                                     SeriesInstanceUID = seriesBase.SeriesInstanceUID,
        //                                     SeriesNumber = seriesBase.SeriesNumber.ToString(CultureInfo.InvariantCulture),
        //                                     Modality = seriesBase.Modality,
        //                                     StudyInstanceUID = study.StudyInstanceUID,
        //                                     PatientID = patient.PatientID
        //                                 };

        //            //step 3: get image level information
        //            var imageList = _dbWrapper.GetImageListBySeriesInstanceUID(seriesBase.SeriesInstanceUID).OrderBy(n=>n.InstanceNumber);
        //            foreach (var imageBase in imageList)
        //            {
        //                if (imageBase.ImageConfirmStatus != 0)
        //                {
        //                    continue;
        //                }

        //                var imageData = new ImageData(imageBase.SOPInstanceUID);
        //                imageData.Description = imageBase.ImageComments;
        //                imageData.SOPInstanceUID = imageBase.SOPInstanceUID;
        //                imageData.FilePath = imageBase.FilePath;
        //                //TO DO...
        //                //_dbWrapper.GetGSPSByImageUID();
        //                //imageData.GSPSFilePath = imageBase.
        //                imageData.ThumbnailFilePath = imageBase.ThumbnailFilePath;
        //                seriesData.Images.Add(imageData);
        //            }
        //            studyData.SeriesItems.Add(seriesData);
        //        }

        //        //step 4: transform the data to UI
        //        FilmingDataRepositry.Studys.Add(studyData);
        //    }
            
        //}

        //public void LoadStudies(IList<string> studyInstanceUIDList)
        //{
        //    FilmingDataRepositry.Studys.Clear();
        //    foreach (var studyUID in studyInstanceUIDList)
        //    {
        //        LoadStudy(studyUID);
        //    }
        //}

        //public void LoadStudy(string studyInstanceUID)
        //{
        //    try
        //    {
        //        //step 0: lock studyInstanceUID
        //        if (!FilmingDataRepositry.Studys.Any(n => n.StudyUid==studyInstanceUID) 
        //            && !_dbWrapper.Lock(studyInstanceUID, FILMING_LOCK_OWNER, DBLockLevel.Study, "filming lock"))
        //        {
        //            throw new Exception("Can't lock study:" + studyInstanceUID);
        //        }
        //        Logger.LogInfo("has locked study: " + studyInstanceUID + " by " + FILMING_LOCK_OWNER);

        //        //step 1: get patient level and study level information
        //        var studyData = new StudyData();
        //        var newStudy = _dbWrapper.GetStudyByStudyInstanceUID(studyInstanceUID);
        //        if (newStudy == null)
        //        {
        //            throw new Exception("Null study list for StudyUID:" + studyInstanceUID);
        //        }
        //        var newPatient = _dbWrapper.GetPatientByPatientUID(newStudy.PatientUIDFk);

        //        if (newStudy.StudyConfirmStatus != 0)
        //        {
        //            throw new Exception("not comfirmed Study:" + studyInstanceUID);
        //        }

        //        studyData.PatientID = newPatient.PatientID;
        //        studyData.PatientName = newPatient.PatientName;
        //        studyData.PatientBirthday = newPatient.PatientBirthDate.ToString();
        //        studyData.PatientStudyAge = newStudy.PatientAge;
        //        studyData.StudyDate = newStudy.StudyDate;
        //        studyData.PatientSex = newPatient.PatientSex;
        //        studyData.AccessionNumber = newStudy.AccessionNumber;
        //        studyData.StudyUid = newStudy.StudyInstanceUID;
        //        studyData.Description = newStudy.StudyDescription;
        //        studyData.Modality = newStudy.ModalitiesInStudy;

        //        //step 2: get series level information
        //        var seriesList = _dbWrapper.GetSeriesListByStudyInstanceUID(studyInstanceUID);
        //        if (seriesList == null)
        //        {
        //            throw new Exception("NUll series list for studyUID:!" + studyInstanceUID);
        //        }

        //        seriesList.OrderBy((series) => series.SeriesNumber);

        //        foreach (var seriesBase in seriesList)
        //        {
        //            if (seriesBase.SeriesConfirmStatus != 0)
        //            {
        //                continue;
        //            }

        //            var seriesData = new SeriesData(seriesBase.SeriesInstanceUID)
        //                                 {
        //                                     SeriesInstanceUID = seriesBase.SeriesInstanceUID,
        //                                     SeriesNumber = seriesBase.SeriesNumber.ToString(CultureInfo.InvariantCulture),
        //                                     Modality = seriesBase.Modality,
        //                                     StudyInstanceUID = newStudy.StudyInstanceUID,
        //                                     PatientID = newPatient.PatientID,
        //                                     SeriesDescription = seriesBase.SeriesDescription
        //                                 };

        //            //step 3: get image level information
        //            //Order by instance number will be occur exception when the instance number is null.
        //            var imageList = _dbWrapper.GetImageListByConditionWithOrder("SeriesInstanceUIDFk='" + seriesBase.SeriesInstanceUID +"'", "InstanceNumber");//.OrderBy(n => n.InstanceNumber);
        //            if (imageList == null)
        //            {
        //                Logger.LogError("This series has no image! seriesIUID:" + seriesBase.SeriesInstanceUID);
        //            }
        //            else
        //            {
        //                foreach (var imageBase in imageList)
        //                {
        //                    if (imageBase.ImageConfirmStatus != 0)
        //                    {
        //                        continue;
        //                    }

        //                    var imageData = new ImageData(imageBase.SOPInstanceUID);
        //                    imageData.Description = imageBase.ImageComments;
        //                    imageData.SOPInstanceUID = imageBase.SOPInstanceUID;
        //                    imageData.FilePath = imageBase.FilePath;
        //                    imageData.ThumbnailFilePath = imageBase.ThumbnailFilePath;
        //                    seriesData.Images.Add(imageData);
        //                }
        //            }
                    
        //            studyData.SeriesItems.Add(seriesData);
        //        }
                
        //        //step 4: transform the data to UI
        //        FilmingDataRepositry.Studys.RemoveAll(n => n.StudyUid == studyInstanceUID);
        //        FilmingDataRepositry.Studys.Add(studyData);

        //        FilmingDataRepositry.studies.RemoveAll(n => n.StudyInstanceUID == studyInstanceUID);
        //        FilmingDataRepositry.studies.Add(newStudy);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //        //throw;
        //        //unlock
        //        _dbWrapper.UnLock(studyInstanceUID, FILMING_LOCK_OWNER);
        //        Logger.LogInfo("has unlocked study: " + studyInstanceUID + " by " + FILMING_LOCK_OWNER);
        //    }
        //}

        private readonly string FilmingLockOwner = null == FilmingViewerContainee.Main ? "FilmingCard@FE@" : FilmingViewerContainee.Main.GetName();
        public void UnLock()
        {
            try
            {
                _dbWrapper.UnLock(FilmingLockOwner);
                Logger.LogInfo("has unlocked all studies locked" + " by " + FilmingLockOwner);
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        private readonly IList<string> _lockedStudyUIDList = new List<string>();

        public IList<string> LockedStudyUidList
        {
            get { return _lockedStudyUIDList; }
        }

        public void Lock(string studyInstanceUid)
        {
            try
            {
                if (!_lockedStudyUIDList.Contains(studyInstanceUid))
                {
                    _dbWrapper.Lock(studyInstanceUid, FilmingLockOwner, DBLockLevel.Study, "filming locking");
                    Logger.LogInfo("has locked study " + studyInstanceUid + " by " + FilmingLockOwner);
                    _lockedStudyUIDList.Add(studyInstanceUid);
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
    
        }
    }
}
