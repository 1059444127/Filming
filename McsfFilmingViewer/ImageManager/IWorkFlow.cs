namespace UIH.Mcsf.Filming.ImageManager
{
    /// <summary>
    /// The worker like a customer, provide the job process method.
    /// </summary>
    public interface IWorkFlow
    {
        object Preprocess(object originalJob);

        void DoRealWork(object processedJob);

        /*
                void CancelWork();
        */
    }
}
