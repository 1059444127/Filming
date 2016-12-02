//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.ComponentModel;

//using UIH.Mcsf.Database;

//namespace UIH.Mcsf.Filming
//{
//    public enum ReviewDataStatus
//    {
//        Initializing,
//        Initialized,
//        Unloaded,
//    }

//    public class DataRepositry
//    {
//        private static readonly DataRepositry _instance= new DataRepositry();

//        private List<StudyData> _studys;

//        private List<Study> _studies;

//        public static DataRepositry Instance
//        {
//            get 
//            {
//                return _instance;
//            }
//        }

//        private DataRepositry()
//        {
//            _studys = new List<StudyData>();
//            _studies = new List<Study>();
//        }

//        public List<StudyData> Studys
//        {
//            get { return _studys; }
//        }

//        public List<Study> studies
//        {
//            get { return _studies; }

//        }

//    }
    
//}
