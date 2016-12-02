namespace UIH.Mcsf.Filming
{
    public class Patient
    {

        public Patient()
        {
            PatientID = string.Empty;
            PatientName = string.Empty;
            PatientSex = string.Empty;
            PatientAge = string.Empty;
            AccessionNo = string.Empty;
            OperatorName = string.Empty;
            StudyID = string.Empty;
        }

        #region Properties

        public string PatientID
        {
            get { return _patientID; }
            set { _patientID = value ?? string.Empty; }
        }

        public string PatientName
        {
            get { return _patientName; }
            set { _patientName = value ?? string.Empty; }
        }

        public string PatientSex
        {
            get { return _patientSex; }
            set { _patientSex = value ?? string.Empty; }
        }

        public string PatientAge
        {
            get { return _patientAge; }
            set { _patientAge = value ?? string.Empty; }
        }

        public string AccessionNo
        {
            get { return _accessionNo; }
            set { _accessionNo = value ?? string.Empty; }
        }

        public string OperatorName
        {
            get { return _operatorName; }
            set { _operatorName = value ?? string.Empty; }
        }

        public string StudyID
        {
            get { return _studyID; }
            set { _studyID = value ?? string.Empty; }
        }

        #endregion  //Properties

        #region Fields

        private string _patientID = string.Empty;
        private string _patientName = string.Empty;
        private string _patientSex = string.Empty;
        private string _patientAge = string.Empty;
        private string _accessionNo = string.Empty;
        private string _operatorName = string.Empty;
        private string _studyID = string.Empty;
        #endregion  //Fields
    }

}

