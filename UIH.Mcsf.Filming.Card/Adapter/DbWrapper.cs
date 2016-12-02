using System;
using System.Collections.Generic;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.Database;

namespace UIH.Mcsf.Filming.Card.Adapter
{
    public class DbWrapper
    {
        #region [--Singleton--]

        public static DbWrapper Instance
        {
            get { return _instance ?? (_instance = new DbWrapper()); }
        }

        private DbWrapper()
        {
            //bool bInit = _dbWrapper.Initialize();
            //_dbWrapper.SetAutoNotifyOn(ComProxyManager.GetCurrentProxy());
            //if (!bInit)
            //{
            //    throw new Exception("DBWrapper.Initialize failed!");
            //}
            //Debug.Assert(_dbWrapper != null);
        }

        private readonly DBWrapper _dbWrapper = DBWrapperHelper.DBWrapper;//new DBWrapper();

        private static DbWrapper _instance;

        #endregion [--Singleton--]

        #region [--DB Lock--]

        public void UnLock()
        {
            try
            {
                _dbWrapper.UnLock(_filmingLockOwner);
                Logger.LogInfo("has unlocked all studies locked" + " by " + _filmingLockOwner);
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        public void Lock(string studyInstanceUid)
        {
            try
            {
                if (!_lockedStudyUidList.Contains(studyInstanceUid))
                {
                    _dbWrapper.Lock(studyInstanceUid, _filmingLockOwner, DBLockLevel.Study, "filming locking");
                    Logger.LogInfo("has locked study " + studyInstanceUid + " by " + _filmingLockOwner);
                    _lockedStudyUidList.Add(studyInstanceUid);
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
    
        }


        private readonly string _filmingLockOwner = null == Containee.Main ? "Filming@FE@" : Containee.Main.GetName();

        private readonly IList<string> _lockedStudyUidList = new List<string>();

        //public IList<string> LockedStudyUIDList
        //{
        //    get { return _lockedStudyUIDList; }
        //}

        #endregion [--DB Lock--]

    }
}

