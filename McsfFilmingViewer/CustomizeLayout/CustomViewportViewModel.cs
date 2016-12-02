using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UIH.Mcsf.Core;
using UIH.Mcsf.Filming.Command;
using UIH.Mcsf.Filming.Utility;

namespace UIH.Mcsf.Filming.CustomizeLayout
{
    public class CustomViewportViewModel : INotifyPropertyChanged
    {
        #region 初始化读取布局
        public CustomViewportViewModel()
        {
            SelectedViewportItem = null;
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                CustomViewportItemCollectionUser.Clear();
                CustomViewportItemCollectionOrigin.Clear();
                string entryPath = mcsf_clr_systemenvironment_config.GetApplicationPath("FilmingConfigPath");


                //用户定义布局加载
                string layoutRootPath = entryPath + @"\McsfMedViewerConfig\MedViewerLayouts\CustomerLayout\";
                if(!Directory.Exists(layoutRootPath))
                {
                    Directory.CreateDirectory(layoutRootPath);
                }
                string[] customViewportLayoutFilesArray = Directory.GetFiles(layoutRootPath);
                foreach (var customViewportLayoutFile in customViewportLayoutFilesArray)
                {
                    if (customViewportLayoutFile.Contains(".png"))
                    {
                        string layoutName = customViewportLayoutFile.Split('\\').Last();
                        layoutName = layoutName.Remove(layoutName.Length - ".png".Length);

                        string layoutXmlFullpath = customViewportLayoutFile.Replace("png", "xml");
                        CustomViewportItemCollectionUser.Add(new CustomViewportItem(layoutName, customViewportLayoutFile, layoutXmlFullpath));
                    }
                }

                //出厂定义布局加载
                string originlayoutRootPath = entryPath + @"\McsfMedViewerConfig\MedViewerLayouts\Origin\";
                if (!Directory.Exists(originlayoutRootPath))
                {
                    Directory.CreateDirectory(originlayoutRootPath);
                }
                string[] originViewportLayoutFilesArray = Directory.GetFiles(originlayoutRootPath);
                foreach (var originViewportLayoutFile in originViewportLayoutFilesArray)
                {
                    if (originViewportLayoutFile.Contains(".png"))
                    {
                        string layoutName = originViewportLayoutFile.Split('\\').Last();
                        layoutName = layoutName.Remove(layoutName.Length - ".png".Length);
                        string layoutXmlFullpath = originViewportLayoutFile.Replace("png", "xml");
                        CustomViewportItemCollectionOrigin.Add(new CustomViewportItem(layoutName, originViewportLayoutFile, layoutXmlFullpath));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception in Initialize: "+ex.StackTrace);
                throw;
            }
        }
        #endregion

        #region 集合记录
        //用户定义布局集合
        private ObservableCollection<CustomViewportItem> _customViewportItemCollectionUser = 
            new ObservableCollection<CustomViewportItem>();
        public ObservableCollection<CustomViewportItem> CustomViewportItemCollectionUser
        {
            get { return _customViewportItemCollectionUser; }
            set
            {
                _customViewportItemCollectionUser = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CustomViewportItemCollectionUser"));
            }
        }

        //出厂定义布局集合
        private ObservableCollection<CustomViewportItem> _customViewportItemCollectionOrigin =
            new ObservableCollection<CustomViewportItem>();
        public ObservableCollection<CustomViewportItem> CustomViewportItemCollectionOrigin
        {
            get { return _customViewportItemCollectionOrigin; }
            set
            {
                _customViewportItemCollectionOrigin = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CustomViewportItemCollectionUser"));
            }
        }
        #endregion

        #region 布局命名相关
        public bool ExistViewportHasSameNameWithActiveViewport()
        {
            var names = (from item in CustomViewportItemCollectionUser
                        select item.CustomViewportName).ToList();
            return names.Contains(CustomViewportName);
        }
        public bool IsCustomViewportNameValid()
        {
            if (CustomViewportName.Trim() == string.Empty) return false;
            Regex regEx = new Regex("[\\*\\\\/:?<>|\"]");
            return !regEx.IsMatch(CustomViewportName);
        }

        private string _customViewportName;
        public string CustomViewportName
        {
            get
            {
                return _customViewportName; 
            }

            set
            {
                _customViewportName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CustomViewportName"));
            }
        }
        #endregion


        private CustomViewportItem _selectedViewportItem = null;
        public CustomViewportItem SelectedViewportItem
        {
            get { return _selectedViewportItem; }
            set
            {
                _selectedViewportItem = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsEnableApply"));
                OnPropertyChanged(new PropertyChangedEventArgs("IsEnableDelete"));
            }
        }
        private CustomViewportItem _selectedViewportItemUser = null;
        public CustomViewportItem SelectedViewportItemUser
        {
            get { return _selectedViewportItemUser; }
            set
            {
                _selectedViewportItemUser = value;
                SelectedViewportItem = value;
            }
        }
        private CustomViewportItem _selectedViewportItemOrigin = null;
        public CustomViewportItem SelectedViewportItemOrigin
        {
            get { return _selectedViewportItemOrigin; }
            set
            {
                _selectedViewportItemOrigin = value;
                SelectedViewportItem = value;
            }
        }

        public bool IsEnableApply
        {
            get
            {
                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                var isEnableViewportLayoutManager = filmingCard == null || filmingCard.layoutCtrl.IsEnableViewportLayoutManager;
                return SelectedViewportItem != null && isEnableViewportLayoutManager;
            }
        }

        public bool IsEnableDelete
        {
            get
            {
                return SelectedViewportItem != null && !CustomViewportItemCollectionOrigin.Contains(SelectedViewportItem);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void Refresh()
        {
            Initialize();
        }
    }

    public class CustomViewportItem:INotifyPropertyChanged
    {
        public CustomViewportItem(string name, string thumbnailFullPath,string layoutStreamFullPath)
        {
            CustomViewportThumbnailImagePath = thumbnailFullPath;
            CustomViewportLayoutXmlPath = layoutStreamFullPath;

            LayoutType = LayoutTypeEnum.DefinedLayout;
            if (name.Contains(FilmLayout.RegularLayoutString))
            {
                LayoutType = LayoutTypeEnum.RegularLayout;
                name = name.Remove(name.Length - FilmLayout.RegularLayoutString.Length);
            }

            CustomViewportName = name;
        }

        private Visibility _indexLableVisibility = Visibility.Collapsed;
        public Visibility IndexLableVisibility
        {
            get { return _indexLableVisibility; }
            set
            {
                if(_indexLableVisibility == value) return;
                _indexLableVisibility = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IndexLableVisibility"));
            }
        }

        private int _indexLableContent = -1;
        public int IndexLabelContent
        {
            get { return _indexLableContent; }
            set
            {
                if (_indexLableContent == value) return;
                _indexLableContent = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IndexLabelContent"));
            }
        }


        public bool Destroy()
        {
            try
            {
                File.Delete(CustomViewportThumbnailImagePath);
                File.Delete(CustomViewportLayoutXmlPath); 
                return true;
            }
            catch (Exception exp)
            {
                Logger.LogError("Exception in Destroy: "+exp.Message);
                return false;
            }
        }

        private string _customViewportName;
        public string CustomViewportName
        {
            get
            {
                return _customViewportName;
            }
            set
            {
                if (_customViewportName != value)
                {
                    Regex regEx = new Regex("[\\*\\\\/:?<>|\"]");
                    if (value.Trim() == string.Empty || regEx.IsMatch(value))
                    {
                        MessageBoxHandler.Instance.ShowWarning("UID_Filming_CustomViewPortLayout_New_Warning_Invalid_Name");
                        return;
                    }
                    var oldName = _customViewportName;
                    _customViewportName = value;
                    ChangeCustomViewportFilesName(oldName, value);
                    OnPropertyChanged(new PropertyChangedEventArgs("CustomViewportName"));
                }
            }
        }

        private bool _nameIsReadOnly = true;
        public bool NameIsReadOnly
        {
            get { return _nameIsReadOnly; }
            set
            {
                if(_nameIsReadOnly == value) return;
                _nameIsReadOnly = value;
                OnPropertyChanged(new PropertyChangedEventArgs("NameIsReadOnly"));
            }
        }

        private string _selectedText;
        public string SelectedText
        {
            get { return _selectedText; }
            set
            {
                _selectedText = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SelectedText"));
            }
        }


        public void SetInputtingState()
        {
            
        }
        
        private string _customViewportThumbnailImagePath;
        public string CustomViewportThumbnailImagePath
        {
            get { return _customViewportThumbnailImagePath; }

            set
            {
                if (_customViewportThumbnailImagePath != value)
                {
                    _customViewportThumbnailImagePath = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CustomViewportThumbnailImagePath"));
                }
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public ImageSource CustomViewportThumbnailBitmapSource
        {
            get
            {
                if (string.IsNullOrEmpty(CustomViewportThumbnailImagePath))
                {
                    return null;
                }

                if (!File.Exists(CustomViewportThumbnailImagePath))
                {
                    return null;
                }

                try
                {
                    using (var bitmap = new Bitmap(CustomViewportThumbnailImagePath) )
                    {
                        IntPtr hBitmap = bitmap.GetHbitmap();

                        ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                            hBitmap,
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());

                        DeleteObject(hBitmap);
                        return wpfBitmap;                    
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Create thumbnail failed!" + ex.StackTrace + "! path:" + CustomViewportThumbnailImagePath);
                    return null;
                }
            }
        }

        private string _customViewportLayoutXmlPath;
        public LayoutTypeEnum LayoutType { get; private set;}

        public string CustomViewportLayoutXmlPath
        {
            get { return _customViewportLayoutXmlPath; }

            set
            {
                if (_customViewportLayoutXmlPath != value)
                {
                    _customViewportLayoutXmlPath = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CustomViewportLayoutXmlPath"));
                }
            }
        }

        private void ChangeCustomViewportFilesName(string oldName, string newName)
        {
            if (oldName == null || newName == null) return;

            try
            {
                if (LayoutType == LayoutTypeEnum.RegularLayout)
                {
                    oldName += FilmLayout.RegularLayoutString;
                    newName += FilmLayout.RegularLayoutString;
                }
                string entryPath = mcsf_clr_systemenvironment_config.GetApplicationPath("FilmingConfigPath");

                string layoutRootPath = entryPath + @"\McsfMedViewerConfig\MedViewerLayouts\CustomerLayout\";

                if (!Directory.Exists(layoutRootPath))
                {
                    Directory.CreateDirectory(layoutRootPath);
                }
                DirectoryInfo dir = new DirectoryInfo(layoutRootPath);
                FileInfo[] customViewportLayoutFilesArray = dir.GetFiles();
                foreach (var customViewportLayoutFile in customViewportLayoutFilesArray)
                {
                    string layoutFileName = customViewportLayoutFile.Name;
                    var layoutFileNameWithoutPostfix = layoutFileName.Substring(0, layoutFileName.Length - ".png".Length);
                    if (layoutFileNameWithoutPostfix.ToUpper() == newName.ToUpper())
                    {
                        MessageBoxHandler.Instance.ShowWarning("UID_Filming_CustomViewPortLayout_New_Warning_Invalid_Name");
                        _customViewportName = oldName;
                        return;
                    }
                }
                
                foreach (var customViewportLayoutFile in customViewportLayoutFilesArray)
                {
                    string layoutFileName = customViewportLayoutFile.Name;
                    var layoutFileNameWithoutPostfix = layoutFileName.Substring(0, layoutFileName.Length - ".png".Length);
                    if (layoutFileNameWithoutPostfix.Equals(oldName))
                    {
                        string postFix = layoutFileName.Substring(layoutFileName.IndexOf(@"."), layoutFileName.Length - layoutFileNameWithoutPostfix.Length);
                        string newPath = customViewportLayoutFile.DirectoryName + @"\" + newName + postFix;
                        customViewportLayoutFile.MoveTo(newPath);
                        if (postFix == ".png") CustomViewportThumbnailImagePath = newPath;
                        else CustomViewportLayoutXmlPath = newPath;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception in ChangeCustomViewportFilesName: " + ex.StackTrace);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
