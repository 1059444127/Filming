//======================================================================
//
//        Copyright (C) 2013 Shanghai United Imaging Healthcare Inc.    
//        All rights reserved
//
//        filename :ImageJobManager
//        description : Image job manager implement
//
//        created by MU Pengxuan at 2013-5-6 15:45:12
//        pengxuan.mu@united-imaging.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Threading;

namespace UIH.Mcsf.Filming.ImageManager
{
    public class ImageJobManager:IDisposable
    {
        #region [--Constructors--]

        public ImageJobManager()
        {
        }

        public ImageJobManager(IWorkFlow realWorkFlow)
        {
            _realWorkFlow = realWorkFlow;
        }

        #endregion [--Constructors--]


        #region [--Jobs Info （Synchronized）--]

        public IWorkFlow JobWorkFlow
        {
            get
            {
                return _realWorkFlow;
            }

            set
            {
                _realWorkFlow = value;
            }
        }

        public List<object> OriginalJobQueue
        {
            get { return _originalJobQueue; }
        }

        public int OriginalJobCount
        {
            get
            {
                lock (_originalJobLock)
                {
                    return OriginalJobQueue.Count;
                }
            }
        }

        public List<object> ProcessedJobQueue
        {
            get
            {
                return _processedJobQueue;
            }
        }

        public int ProcessedJobCount
        {
            get
            {
                lock (_processedJobLock)
                {
                    return ProcessedJobQueue.Count;
                }
            }
        }

        public bool IsStoppingService { get; set; }

        #endregion [--Jobs Info （Synchronized）--]


        #region [--Job Management (Synchronized）--]
        public void PushOriginalJob(object originalJob)
        {
            lock (_originalJobLock)
            {
                if (IsStoppingService)
                {
                    Logger.LogWarning("Job manager is stopping service, don't receive new job!");
                    return;
                }

                OriginalJobQueue.Add(originalJob);

                Logger.LogInfo("Add Original Job!");
            }

            _originalManualResetEvent.Set();
        }

        public object PopOriginalJob()
        {
            lock (_originalJobLock)
            {
                var job = OriginalJobQueue[0];
                OriginalJobQueue.RemoveAt(0);
                return job;
            }
        }

        public void PushProcessedJob(object processedJob)
        {
            lock (_processedJobLock)
            {
                if (processedJob is ImageJobModel)
                {
                    var job = processedJob as ImageJobModel;
                    var id = job.FilmingIdentifier;
                    var batchJob =
                        ProcessedJobQueue.Find(j => j is BatchJob && (j as BatchJob).Id == id
                        && !(j as BatchJob).IsComplete)  as BatchJob;
                    if(batchJob != null)
                    {
                        batchJob.Add(job);
                    }
                    else
                    {
                        ProcessedJobQueue.Add(job);
                    }
                }
                else
                {
                    ProcessedJobQueue.Add(processedJob);
                }

                Logger.LogInfo("Add Processed Job!");
            }

            WakeUpWorkThread();
        }


        private readonly  object _busyLock = new object();

        public bool IsBusy
        {
            get
            {
                lock (_busyLock)
                {
                    return FilmingViewerContainee.IsBusy;
                }
            }
            set
            {
                lock (_busyLock)
                {
                    FilmingViewerContainee.IsBusy = value;
                }
            }
        }

        private readonly object _isLoadingLock = new object();

        public bool IsLoading
        {
            get
            {
                lock (_isLoadingLock)
                {
                    return FilmingViewerContainee.IsLoading;
                }
            }
            set
            {
                lock (_isLoadingLock)
                {
                    FilmingViewerContainee.IsLoading = value;
                }
            }
        }

        public void JobFinished()
        {
            lock (_busyLock)
            {
                FilmingViewerContainee.IsBusy = false;
                _processedManualResetEvent.Set();
                Logger.LogWarning("[[[[Job finished]]]]");
            }
        }

        public void WakeUpWorkThread()
        {
            lock (_busyLock)
            {
                if (!FilmingViewerContainee.IsBusy)
                {
                    _processedManualResetEvent.Set();
                    FilmingViewerContainee.IsBusy = true;
                }
            }
        }

        public void SleepWorkThread()
        {
            lock (_busyLock)
            {
                FilmingViewerContainee.IsBusy = true;
            }
        }

        public void SetWorkingFlag()
        {
            lock (_isLoadingLock)
            {
                FilmingViewerContainee.IsLoading = true;
            }
        }

        public void PopProcessedJob()
        {
            lock (_processedJobLock)
            {
                ProcessedJobQueue.RemoveAt(0);
            }
        }

        public object PeekProcessedJob()
        {
            lock (_processedJobLock)
            {
                var task = ProcessedJobQueue[0] as ACommandHandler;
                if (task == null)
                {
                    ProcessedJobQueue.RemoveAt(0);
                    return null;
                }

                var job = task.CurrentJob();

                if (task.IsReady)
                {
                    ProcessedJobQueue.RemoveAt(0);
                }

                return job;
            }
        }

        /// <summary>
        /// this function shall run in a new thread, and can trig by new job coming.
        /// if there is no job, this thread need hung for waiting new job.
        /// </summary>
        /// <param></param>
        public void StartWork()
        {
            _preprocessWorkThread = new Thread(DoPreprocessWork);
            _preprocessWorkThread.Start();

            _realWorkThread = new Thread(DoWork);
            _realWorkThread.Start();
        }

        /// <summary>
        /// Don't allowed to stop when there are some job in queue. because we don't have history
        /// job record, so if you stop. the job will lost!.
        /// </summary>
        public bool StopWork()
        {
            if (OriginalJobCount > 0 || ProcessedJobCount > 0)
            {
                Logger.LogWarning("Warning: There are some job in queue, stopping will lost job...");
            }

            _preprocessWorkThread.Abort();
            _realWorkThread.Abort();

            Logger.LogInfo("Stop job manager!");

            return true;
        }

        #endregion [--Job Management (Synchronized）--]



        #region [--Private Work Thread Methods--]

        private void DoPreprocessWork()
        {
            while (true)
            {
                _originalManualResetEvent.WaitOne();

                while (OriginalJobCount > 0)
                {
                    var job = PopOriginalJob();

                    var processedJob = _realWorkFlow.Preprocess(job);

                    if(processedJob != null)
                        PushProcessedJob(processedJob);

                    Thread.Sleep(3);
                }

                _originalManualResetEvent.Reset();
            }
        }

        /// <summary>
        /// Note: This function never returns, you shall call function StopWork to stop this function.
        /// </summary>
        private void DoWork()
        {
            while (true)
            {
                IsLoading = false;
                bool hasGetSignal     = _processedManualResetEvent.WaitOne(FilmingUtility.JobTimeOutLength * 1000);
                _processedManualResetEvent.Reset();

                if(ProcessedJobCount <=0 ) continue;
                if (IsLoading && !hasGetSignal)
                {
                    continue;
                }
                object processedJob = new TimeOutJob();
                if (hasGetSignal)
                {
                    processedJob = PeekProcessedJob();
                }
                else
                {
                    PopProcessedJob();
                    Logger.Instance.LogDevWarning(FilmingUtility.FunctionTraceEnterFlag + "Throw a Time Out Job!");
                }

                if (processedJob != null)
                {
                    _realWorkFlow.DoRealWork(processedJob);
                }
                else
                {
                    Logger.LogWarning("Null Processed Job");
                    IsBusy = false;
                }

                    //Thread.Sleep(1);


                //_processedManualResetEvent.Reset();
            }
        }

        #endregion [--Private Work Thread Methods--]

        #region [--Fields--]

        private readonly object _originalJobLock = new object();

        private readonly object _processedJobLock = new object();

        private readonly ManualResetEvent _originalManualResetEvent = new ManualResetEvent(true);
        private readonly ManualResetEvent _processedManualResetEvent = new ManualResetEvent(true);

        private IWorkFlow _realWorkFlow;

        private readonly List<object> _originalJobQueue = new List<object>();
        private readonly List<object> _processedJobQueue = new List<object>();

        private Thread _preprocessWorkThread;

        private Thread _realWorkThread;

        #endregion [--Fields--]

        public void Dispose()
        {
            _originalManualResetEvent.Dispose();
            _processedManualResetEvent.Dispose();
        }

    }

}
