using System.Collections.Concurrent;

namespace UIH.Mcsf.Filming.ImageManager
{
    public abstract class BatchJob : ACommandHandler
    {
        protected BatchJob(string id, int count)
        {
            _count = count;
            Id = id;
            Add(this);
        }

        public void Add(ACommandHandler job)
        {
            _queue.Enqueue(job);
            _jobInCount++;
        }

        public override ACommandHandler CurrentJob()
        {
            ACommandHandler job;
            if (_queue.TryDequeue(out job))
            {
                _jobOutCount++;
                return job;
            }
            return null;
        }

        public bool IsComplete
        {
            get { return _jobInCount == _count; }
        }

        public override  bool IsReady
        {
            get { return _jobOutCount == _count; }
        }

        public string Id { get; private set; }

        private ConcurrentQueue<ACommandHandler> _queue = new ConcurrentQueue<ACommandHandler>();
        private readonly int _count;
        private int _jobInCount;
        private int _jobOutCount;
    }
}
