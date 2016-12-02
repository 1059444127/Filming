using System;
using System.Windows.Input;
using UIH.Mcsf.Filming.Widgets;

namespace UIH.Mcsf.Filming.Card.ViewModel
{
    public class FilmViewModel : ObservableObject
    {
        #region [--Properties--]

        #endregion [--Properties--]


        #region [--Commands--]

        void LoadImageExecute()
        {
            try
            {
                Logger.LogFuncUp();



                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        private bool CanLoadImageExecute()
        {
            return true;
        }

        //public ICommand LoadImageCommand
        //{
        //    get { return new RelayCommand(LoadImageExecute, CanLoadImageExecute); }
        //}

        #endregion [--Commands--]

    }
}