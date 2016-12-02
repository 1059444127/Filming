using System.Collections.Generic;

namespace UIH.Mcsf.Filming.ImageManager
{
    //singleton in containee
    class JobDispatcher : AbstractJobManager, IWorkFlow
    {

        public JobDispatcher()
        {
            JobWorkFlow = this;
        }

        #region     [--Methods--]
        #endregion  [--Methods--]

        #region     [--Fields--]
        IList<ImageJobManager> Jobs = new List<ImageJobManager>();
        #endregion  [--Fields--]

        #region Implementation of IWorkFlow

        //将原始任务拆分成JobUID/Count/Index/Flag,  检查任务队列的状态, 删除完成的队列, 启用新的队列
        public object Preprocess(object originalJob)
        {
            throw new System.NotImplementedException();
        }

        //根据JobUID, 将分配到不同的ImageJobManager
        public void DoRealWork(object processedJob)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
