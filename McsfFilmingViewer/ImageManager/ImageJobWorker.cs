namespace UIH.Mcsf.Filming.ImageManager
{
    class ImageJobWorker : IWorkFlow
    {
        #region Implementation of IWorkFlow

        public object Preprocess(object originalJob)
        {
            //throw new System.NotImplementedException();
            return null;
        }

        public void DoRealWork(object processedJob)
        {
            //throw new System.NotImplementedException();
            ACommandHandler commandHandler = processedJob as ACommandHandler;
            if(commandHandler == null) Logger.LogFuncException("null command handler");
            else commandHandler.HandleCommand();
        }

        #endregion
    }
}