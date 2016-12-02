namespace UIH.Mcsf.Filming.Wrappers
{
    public class FilmingNlsFactory
    {
        public static string WarningTitle
        {
            get
            {
                var warningTitle = "Warning message";
                if (FilmingViewerContainee.FilmingResourceDict.Contains("UID_Filming_Warning_Title"))
                {
                    warningTitle =
                    (string)FilmingViewerContainee.FilmingResourceDict[
                        "UID_Filming_Warning_Title"];
                }

                return warningTitle;
            }
        }

        public static string WarningViewportLossImages
        {
            get
            {
                var warningString = "Warning: Selected view port will loss images!";
                if (FilmingViewerContainee.FilmingResourceDict.Contains("UID_Filming_Warning_ViewportLossImages"))
                {
                    warningString =
                    (string)FilmingViewerContainee.FilmingResourceDict[
                        "UID_Filming_Warning_ViewportLossImages"];
                }

                return warningString;
            }
        }

        public static string WarningCorrectNumber
        {
            get
            {
                var warningString = "You should input a float number!";
                if (FilmingViewerContainee.FilmingResourceDict.Contains("UID_Filming_Warning_CorrectNumber"))
                {
                    warningString =
                    (string)FilmingViewerContainee.FilmingResourceDict[
                        "UID_Filming_Warning_CorrectNumber"];
                }

                return warningString;
            }
        }

        public static string WarningInsertEmptyCellToUnStandardLayout
        {
            get
            {
                var warningString = "Can't insert empty cell in un-standard layout page!";
                if (FilmingViewerContainee.FilmingResourceDict.Contains("UID_Filming_Warning_InsertEmptyCellToUnStandardLayout"))
                {
                    warningString =
                    (string)FilmingViewerContainee.FilmingResourceDict[
                        "UID_Filming_Warning_InsertEmptyCellToUnStandardLayout"];
                }

                return warningString;
            }
        }

        public static string WarningNoEmptyCellForPaste
        {
            get
            {
                var warningString = "No empty cell for paste!";
                if (FilmingViewerContainee.FilmingResourceDict.Contains("UID_Filming_Warning_NoEmptyCellForPaste"))
                {
                    warningString =
                    (string)FilmingViewerContainee.FilmingResourceDict[
                        "UID_Filming_Warning_NoEmptyCellForPaste"];
                }

                return warningString;
            }
        }

        public static string WarningCellLossImages
        {
            get
            {
                var warningString = "Warning: Can't change the cell layout, will be lost image!";
                if (FilmingViewerContainee.FilmingResourceDict.Contains("UID_Filming_Warning_CellLossImages"))
                {
                    warningString =
                    (string)FilmingViewerContainee.FilmingResourceDict[
                        "UID_Filming_Warning_CellLossImages"];
                }

                return warningString;
            }
        }

        //<sys:String x:Key="UID_Filming_Warning_HaveNoEmptyCell">Have no empty cell!</sys:String>
        public static string WarningHaveNoEmptyCell
        {
            get
            {
                var warningString = "Have no empty cell!";
                if (FilmingViewerContainee.FilmingResourceDict.Contains("UID_Filming_Warning_HaveNoEmptyCell"))
                {
                    warningString =
                    (string)FilmingViewerContainee.FilmingResourceDict[
                        "UID_Filming_Warning_HaveNoEmptyCell"];
                }

                return warningString;
            }
        }

        //<sys:String x:Key="UID_Filming_Warning_AddDifferentPatientToSameFilmCard">Add different patients' images in one film card ?</sys:String>
        public static string WarningAddDifferentPatientToSameFilmCard
        {
            get
            {
                var warningString = "Add different patients' images in one film card ?";
                if (FilmingViewerContainee.FilmingResourceDict.Contains("UID_Filming_Warning_AddDifferentPatientToSameFilmCard"))
                {
                    warningString =
                    (string)FilmingViewerContainee.FilmingResourceDict[
                        "UID_Filming_Warning_AddDifferentPatientToSameFilmCard"];
                }

                return warningString;
            }
        }

        //<sys:String x:key="UID_Filming_Warning_LoadingImagesWhichMayBeSlowForCountExceedThreshold">This filming loading will take a long time. Do you want to continue ?</sys:String>
        public static string WarningLoadingImagesWhichMayBeSlowForCountExceedThreshold
        {
            get 
            {
                var warningString = "This filming loading will take a long time. Do you want to continue ?";
                if (FilmingViewerContainee.FilmingResourceDict.Contains("UID_Filming_Warning_LoadingImagesWhichMayBeSlowForCountExceedThreshold"))
                {
                    warningString =
                    (string)FilmingViewerContainee.FilmingResourceDict[
                        "UID_Filming_Warning_LoadingImagesWhichMayBeSlowForCountExceedThreshold"];
                }

                return warningString;
            }
        }
    }
}
