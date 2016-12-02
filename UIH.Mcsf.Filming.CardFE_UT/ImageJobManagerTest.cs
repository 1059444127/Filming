using UIH.Mcsf.Filming.ImageManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    using UIH.Mcsf.Filming.ImageManager;

    /// <summary>
    ///This is a test class for ImageJobManagerTest and is intended
    ///to contain all ImageJobManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ImageJobManagerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for OriginalJobCount
        ///</summary>
        [TestMethod()]
        public void OriginalJobCountTest()
        {
            ImageJobManager target = new ImageJobManager();
            target.PushOriginalJob(new object());
            int actual = 1;
            actual = target.OriginalJobCount;
            Assert.AreEqual(1, actual);
        }

        /// <summary>
        ///A test for JobWorker
        ///</summary>
        [TestMethod()]
        public void JobWorkerTest()
        {
            ImageJobManager target = new ImageJobManager();
            IWorkFlow expected = new DataHeaderJobWorker();
            target.JobWorkFlow = expected;
            IWorkFlow actual;
            actual = target.JobWorkFlow;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsStoppingService
        ///</summary>
        [TestMethod()]
        public void IsStoppingServiceTest()
        {
            ImageJobManager target = new ImageJobManager();
            bool expected = false;
            bool actual;
            target.IsStoppingService = expected;
            actual = target.IsStoppingService;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for StopWork
        ///</summary>
        [TestMethod()]
        public void StopWorkTest()
        {
            ImageJobManager target = new ImageJobManager();
            target.StartWork();
            bool expected = true;
            bool actual;
            actual = target.StopWork();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for StartWork
        ///</summary>
        [TestMethod()]
        public void StartWorkTest()
        {
            ImageJobManager target = new ImageJobManager();
            target.StartWork();
            bool isStopping = target.IsStoppingService;
            bool expected = false;
            Assert.AreEqual(expected, isStopping);
        }

        /// <summary>
        ///A test for PushProcessedJob
        ///</summary>
        [TestMethod()]
        public void PushProcessedJobTest()
        {
            try
            {
                ImageJobManager target = new ImageJobManager();
                object processedJob = new object();
                target.PushProcessedJob(processedJob);

                Assert.AreEqual(1, target.ProcessedJobCount);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
        }

        /// <summary>
        ///A test for PushOriginalJob
        ///</summary>
        [TestMethod()]
        public void PushOriginalJobTest()
        {
            try
            {
                ImageJobManager target = new ImageJobManager();
                object originalJob = new object();
                target.PushOriginalJob(originalJob);

                Assert.AreEqual(1, target.OriginalJobCount);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
        }

        /// <summary>
        ///A test for PopProcessedJob
        ///</summary>
        [TestMethod()]
        public void PopProcessedJobTest()
        {
            try
            {
                ImageJobManager target = new ImageJobManager();
                object expected = new object();
                target.PushProcessedJob(expected);
                object actual;
                actual = target.PeekProcessedJob();
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
            //Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PopOriginalJob
        ///</summary>
        [TestMethod()]
        public void PopOriginalJobTest()
        {
            try
            {
                ImageJobManager target = new ImageJobManager();
                object expected = new object();
                target.PushOriginalJob(expected);
                object actual;
                actual = target.PopOriginalJob();
                Assert.AreEqual(expected, actual);
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError(e.Message + e.StackTrace);
            }
        }

        /// <summary>
        ///A test for ImageJobManager Constructor
        ///</summary>
        [TestMethod()]
        public void ImageJobManagerConstructorTest()
        {
            IWorkFlow realWorker = new DataHeaderJobWorker();
            ImageJobManager target = new ImageJobManager(realWorker);

            var actual = target.JobWorkFlow is DataHeaderJobWorker;
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        ///A test for ImageJobManager Constructor
        ///</summary>
        [TestMethod()]
        public void ImageJobManagerConstructorTest1()
        {
            ImageJobManager target = new ImageJobManager();
            Assert.AreNotEqual(null, target);
        }

        /// <summary>
        ///A test for OriginalJobQueue
        ///</summary>
        [TestMethod()]
        public void OriginalJobQueueTest()
        {
            ImageJobManager target = new ImageJobManager();
            List<object> actual;
            actual = target.OriginalJobQueue;
            Assert.AreNotEqual(null, actual);
        }

        /// <summary>
        ///A test for ProcessedJobCount
        ///</summary>
        [TestMethod()]
        public void ProcessedJobCountTest()
        {
            ImageJobManager target = new ImageJobManager();
            int actual;
            actual = target.ProcessedJobCount;
            Assert.AreEqual(0, actual);
        }

        /// <summary>
        ///A test for ProcessedJobQueue
        ///</summary>
        [TestMethod()]
        public void ProcessedJobQueueTest()
        {
            ImageJobManager target = new ImageJobManager();
            List<object> actual;
            actual = target.ProcessedJobQueue;
            Assert.AreEqual(0, actual.Count);
        }
    }
}
