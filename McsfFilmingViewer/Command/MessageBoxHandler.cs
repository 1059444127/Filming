using System.Windows;
using UIH.Mcsf.MHC;

namespace UIH.Mcsf.Filming.Command
{
    public class MessageBoxHandler
    {
        public void ShowInfo(string key, MsgResponseHander msgResponseHandler = null, MessageBoxType messageBoxType = MessageBoxType.Ok, string titleKey = "UID_MessageBox_Title_Info")
        {
            //_messageHanlder.ShowSysModelDialogWithValue(messageBoxType, MessageBoxIcon.Information, titleKey, key);

            _messageHanlder.ShowChildAppModelDialog(messageBoxType, MessageBoxIcon.Information, titleKey, key);
        }

        public void ShowInfoOwnerWin(string key, Window ownerWindow = null, MsgResponseHander msgResponseHandler = null, MessageBoxType messageBoxType = MessageBoxType.Ok, string titleKey = "UID_MessageBox_Title_Info")
        {
            //_messageHanlder.ShowSysModelDialogWithValue(messageBoxType, MessageBoxIcon.Information, titleKey, key);

            _messageHanlder.ShowChildAppModelDialog(messageBoxType, MessageBoxIcon.Information, titleKey, key,ownerWindow);
        }

        public void ShowSysInfo(string key, MsgResponseHander msgResponseHandler = null, MessageBoxType messageBoxType = MessageBoxType.Ok, string titleKey = "UID_MessageBox_Title_Info")
        {
            //_messageHanlder.ShowSysModelDialogWithValue(messageBoxType, MessageBoxIcon.Information, titleKey, key);

            _messageHanlder.ShowSysModelDialogWithValue(messageBoxType, MessageBoxIcon.Information, titleKey, key);
        }

        //public void ShowInfo(string key, params object[] args)
        //{
        //    _messageHanlder.ShowChildAppModelDialog(MessageBoxType.Ok, MessageBoxIcon.Information, "UID_MessageBox_Title_Info", key, args);
          
        //  //  _messageHanlder.ShowSysModelessDialog(MessageBoxType.Ok, MessageBoxIcon.Information, "UID_MessageBox_Title_Info", key, args);
        //}

        public void ShowError(string key, MsgResponseHander msgResponseHandler = null, MessageBoxType messageBoxType = MessageBoxType.Ok, string titleKey = "UID_MessageBox_Title_Error")
        {
           // _messageHanlder.ShowSysModelDialogWithValue(messageBoxType, MessageBoxIcon.Error, titleKey, key);
            _messageHanlder.ShowChildAppModelDialog(messageBoxType, MessageBoxIcon.Error, titleKey, key);
           // _messageHanlder.ShowAppModelDialogWithValue(messageBoxType, MessageBoxIcon.Error, titleKey, key);
            FilmingViewerContainee.DataHeaderJobManagerInstance.JobFinished();
        }

        public MessageBoxResponse ShowQuestion(string key, MsgResponseHander msgResponseHandler, MessageBoxType messageBoxType = MessageBoxType.YesNo, string titleKey = "UID_MessageBox_Title_Info", Window ownerWindow = null)
        {
            var responseType = _messageHanlder.ShowChildAppModelDialog(messageBoxType, MessageBoxIcon.Question, titleKey,key, ownerWindow);
            switch(responseType)
            {
                case MessageBoxResponse.YES:
                    {
                        if(_messageHanlder!=null)
                           {
                               msgResponseHandler(MessageBoxResponse.YES);
                           }
                        break;
                    }
                case MessageBoxResponse.NO:
                    {
                        FilmingViewerContainee.DataHeaderJobManagerInstance.JobFinished();
                        break;
                    }
                case MessageBoxResponse.CANCEL:
                    {
                        break;
                    }
            }
          

            return responseType;
        }

        public MessageBoxResponse ShowSysQuestion(string key, MsgResponseHander msgResponseHandler, MessageBoxType messageBoxType = MessageBoxType.YesNo, string titleKey = "UID_MessageBox_Title_Info")
        {
            if (MessageBoxResponse.YES == _messageHanlder.ShowSysModelDialog(messageBoxType, MessageBoxIcon.Question, titleKey, key))
            {

                if (_messageHanlder != null)
                {
                    msgResponseHandler(MessageBoxResponse.YES);
                    return MessageBoxResponse.YES;
                }
            }
            else
            {
                FilmingViewerContainee.DataHeaderJobManagerInstance.JobFinished();
                return MessageBoxResponse.NO;
            }
            return MessageBoxResponse.NO;
        }
		
        //public void ShowQuestion(string key, MsgResponseHander msgResponseHandler, params object[] args)
        //{
        //    if (MessageBoxResponse.YES== _messageHanlder.ShowChildAppModelDialog(MessageBoxType.YesNo, MessageBoxIcon.Question,
        //                                             "UID_MessageBox_Title_Info",
        //                                             key, args))
        //    {
        //        msgResponseHandler(MessageBoxResponse.YES);
        //    }
        //    else
        //    {
        //        FilmingViewerContainee.DataHeaderJobManagerInstance.JobFinished();
        //    }
        //}

        public void ShowWarning(string key, MsgResponseHander msgResponseHandler = null, MessageBoxType messageBoxType = MessageBoxType.Ok, string titleKey = "UID_MessageBox_Title_Warning")
        {
            _messageHanlder.ShowChildAppModelDialog(messageBoxType, MessageBoxIcon.Warning, titleKey, key);
           // _messageHanlder.ShowSysModelDialogWithValue(messageBoxType, MessageBoxIcon.Warning, titleKey, key);
        }

        public void ShowSysWarning(string key, MsgResponseHander msgResponseHandler = null, MessageBoxType messageBoxType = MessageBoxType.Ok, string titleKey = "UID_MessageBox_Title_Warning")
        {
            _messageHanlder.ShowSysModelDialogWithValue(messageBoxType, MessageBoxIcon.Warning, titleKey, key);
            // _messageHanlder.ShowSysModelDialogWithValue(messageBoxType, MessageBoxIcon.Warning, titleKey, key);
        }

        public static MessageBoxHandler Instance
        {
            get { return _instance; }
        }

        private static readonly MessageBoxHandler _instance = new MessageBoxHandler();

        private static MessageHanlder _messageHanlder;

        public static MessageHanlder MessageHanlder
        {
            get { return MessageBoxHandler._messageHanlder; }
        }

        private MessageBoxHandler()
        {
            if (null == _messageHanlder)
            {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  

                if (null != FilmingViewerContainee.FilmingResourceDict)
                {
                    _messageHanlder = new MessageHanlder(FilmingViewerContainee.FilmingResourceDict,
                                                         null == FilmingViewerContainee.Main ? null : FilmingViewerContainee.Main.GetCommunicationProxy());
                }
            }

        }

        private void EnteringSecondaryUI()
        {
            FilmingViewerContainee.Main.OnEnterSecondaryUI();
        }

        private void ExitingSecondaryUI(MessageBoxResponse response)
        {
            FilmingViewerContainee.Main.OnExitSecondaryUI();
        }
    }
}
