using System;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.Filming.Card.ViewModel;
using UIH.Mcsf.MedDataManagement;

namespace UIH.Mcsf.Filming.Card.Model
{
    public class ImageModel : ObservableObject
    {
         public ImageModel()
         {
             try
             {
                 Logger.LogFuncUp();

                 //DataManagement for separation UI with data
                 var studyTree = new StudyTree();
                 _dataLoader = DataLoaderFactory.Instance().CreateLoader(studyTree, DBWrapperHelper.DBWrapper);
                 _dataLoader.SopLoadedHandler += OnImageDataLoaded;

                 Logger.LogFuncDown();
             }
             catch (Exception ex)
             {
                 Logger.LogFuncException(ex.Message+ex.StackTrace);
                 throw;
             }
         }

         private void OnImageDataLoaded(object sender, DataLoaderArgs e)
         {
             try
             {
                 Logger.LogFuncUp();

                 var sop = e.Target as Sop;
                 if (sop == null)
                 {
                     throw new Exception("Image Data Loading mistake");
                 }
                 Logger.LogInfo(sop.ToString());

                 Logger.LogFuncDown();
             }
             catch (Exception ex)
             {
                 Logger.LogFuncException(ex.Message+ex.StackTrace);
             }
         }

         private readonly IDataLoader _dataLoader;
    }
}