using System.Collections.Generic;
using System.Threading;

namespace UIH.Mcsf.Filming.ImageManager
{
    public abstract class AbstractJobManager
    {
        #region [--Constructors--]



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

        public Queue<object> OriginalJobQueue
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

        public Queue<object> ProcessedJobQueue
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

        public bool CanOutput
        {
            get
            {
                lock (_outputLock)
                {
                    return _canOutput;
                }
            }

        }

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

                OriginalJobQueue.Enqueue(originalJob);

                Logger.LogInfo("Add Original Job!");
            }

            _originalManualResetEvent.Set();
        }

        public object PopOriginalJob()
        {
            lock (_originalJobLock)
            {
                return OriginalJobQueue.Dequeue();
            }
        }

        public void PushProcessedJob(object processedJob)
        {
            lock (_processedJobLock)
            {
                ProcessedJobQueue.Enqueue(processedJob);

                Logger.LogInfo("Add Processed Job!");
            }

            _processedManualResetEvent.Set();
        }

        public object PopProcessedJob()
        {
            lock (_processedJobLock)
            {
                return ProcessedJobQueue.Dequeue();
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

        public void EnableOutput(bool able)
        {
            lock(_outputLock)
            {
                _canOutput = able;
            }
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

                    PushProcessedJob(processedJob);

                    _processedManualResetEvent.Set();

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
                _processedManualResetEvent.WaitOne();

                while (ProcessedJobCount > 0 && CanOutput)
                {
                    var processedJob = PopProcessedJob();

                    _realWorkFlow.DoRealWork(processedJob);

                    Thread.Sleep(1);
                }

                _processedManualResetEvent.Reset();
            }
        }

        #endregion [--Private Work Thread Methods--]

        #region [--Fields--]

        private readonly object _originalJobLock = new object();

        private readonly object _processedJobLock = new object();

        private readonly object _outputLock = new object();
        private bool _canOutput = true;

        private readonly ManualResetEvent _originalManualResetEvent = new ManualResetEvent(true);
        private readonly ManualResetEvent _processedManualResetEvent = new ManualResetEvent(true);

        private static IWorkFlow _realWorkFlow;

        private readonly Queue<object> _originalJobQueue = new Queue<object>();
        private readonly Queue<object> _processedJobQueue = new Queue<object>();

        private Thread _preprocessWorkThread;

        private Thread _realWorkThread;

        #endregion [--Fields--]

    }
}