//======================================================================
//
//        Copyright (C) 2013 Shanghai United Imaging Healthcare Inc.    
//        All rights reserved
//
//        filename :ImageJobWorker
//        description : Image job worker implement
//
//        created by MU Pengxuan at 2013-5-6 16:09:24
//        pengxuan.mu@united-imaging.com
//
//======================================================================

using System.Text;
using UIH.Mcsf.Pipeline.Data;
using System;
using UIH.Mcsf.Pipeline.Dictionary;

namespace UIH.Mcsf.Filming.ImageManager
{
    using MedDataManagement;
    using Viewer;

    /// <summary>
    /// DataHeader Job Worker
    /// this worker like a custom to pre-process image, then show in UI
    /// </summary>
    public class DataHeaderJobWorker : IWorkFlow
    {
        #region [--Implemented IJobWorker Interface Methods--]

        /// <summary>
        /// Covert a raw job (dataheader) to a filming job (ImageJobModel)
        /// </summary>
        /// <param name="job">raw job</param>
        /// <returns>filming job</returns>
        public object Preprocess(object job)
        {
            try
            {
                if(job == null)
                    Logger.Instance.LogDevWarning(FilmingUtility.FunctionTraceEnterFlag + "Job is null!!" );

                var dataHeaderBuffer = job as byte[];
                if (dataHeaderBuffer != null)
                {
                    if (dataHeaderBuffer.Length < FilmingUtility.MAX_NUMBER_LENGTH)
                    {
                        Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag +"ImageCount:"+ dataHeaderBuffer);
                        return new DataHeaderBatchJob(Convert.ToInt32(Encoding.UTF8.GetString(dataHeaderBuffer)));
                    }

                    var dataHeader = DicomAttributeCollection.Deserialize(dataHeaderBuffer);

                    if (dataHeader != null)
                    {
                        return CreateImageJobModel(dataHeader);
                    }
                    Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "LayoutCommand");
                    return new SetLayoutCommandHandler(new LayoutCommandInfo(Encoding.UTF8.GetString(dataHeaderBuffer)));
                }

                var dataHeaderJob = job as DicomAttributeCollection;
                if (dataHeaderJob != null)
                {
                    Logger.LogInfo("Preprocess data header.");

                    return CreateImageJobModel(dataHeaderJob);
                }


                Logger.LogError("Invalid data header.");

                Logger.LogFuncDown();

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }

            return null;
        }

        /// <summary>
        /// dataheader filming job, add a cell in filmingcard
        /// </summary>
        /// <param name="job">filming job</param>
        public void DoRealWork(object job)
        {
            var imageJobModel = job as ACommandHandler;
            if (imageJobModel != null)
            {
                imageJobModel.HandleCommand();
            }
            else
            {
                Logger.LogError("Invalid image job model.");
            }
        }

        //public void CancelWork()
        //{
        //}

        #endregion [--Implemented IJobWorker Interface Methods--]


        #region [--Private Helper Methods--]

        private ImageJobModel CreateImageJobModel(DicomAttributeCollection dataHeader)
        {
            try
            {
                if (dataHeader != null)
                {
                    var modality = GetTagValueFrom(dataHeader, Tag.Modality);
                    var seriesDescription = GetTagValueFrom(dataHeader, Tag.SeriesDescription);
                    if (seriesDescription.Contains(FilmingUtility.EFilmDescriptionHeader)) modality = FilmingUtility.EFilmModality;


                    var seriesInstanceUid = GetTagValueFrom(dataHeader, Tag.SeriesInstanceUid);

                    var cell = new MedViewerControlCell();

                    var imageJobModel = new ImageJobModel { Modality = modality, ImageCell = cell, SeriesInstanceUid = seriesInstanceUid, DataHeader = dataHeader };


                    var privateTag0X00613102String = GetTagValueFrom(dataHeader, 0X00613102);

                    var tags = privateTag0X00613102String.Split(ComponentModel.Chars._1);

                    if (tags.Length > 0)
                    {
                        try
                        {
                            imageJobModel.ImageTextFileContext = tags[0];
                            imageJobModel.TextItemFileContext = tags[1];
                            imageJobModel.SerializedImageText = tags[2];
                            imageJobModel.FilmingIdentifier = tags[3];
                            imageJobModel.FilmIndex = Convert.ToInt32(tags[4]);
                            imageJobModel.CellIndex = Convert.ToInt32(tags[5]);
                            if (tags.Length >= 7 && !string.IsNullOrEmpty(tags[6]))
                            {
                                imageJobModel.UserInfo = tags[6];
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogWarning(ex.StackTrace);
                        }
                    }


                    return imageJobModel;
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
                throw;
            }
        }

        private string GetTagValueFrom(DicomAttributeCollection dataHeader, Tag tag)
        {
            try
            {
                if (!dataHeader.Contains(tag)) return string.Empty;
                var attr = dataHeader[tag];
                string value;
                attr.GetString(0, out value);
                return value ?? string.Empty;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
            return string.Empty;
        }

        private string GetTagValueFrom(DicomAttributeCollection dataHeader, uint tag)
        {
            try
            {
                if (!dataHeader.Contains(tag)) return string.Empty;
                var attr = dataHeader[tag];

                byte[] bytePath;
                attr.GetBytes(0, out bytePath);

                string value = Encoding.UTF8.GetString(bytePath);

                return value;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            return string.Empty;
        }

        #endregion [--Private Helper Methods--]


    }
}
