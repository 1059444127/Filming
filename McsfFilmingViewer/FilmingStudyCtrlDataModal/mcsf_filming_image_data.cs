////////////////////////////////////////////////////////////////////////////
///// Copyright, (c) Shanghai United Imaging Healthcare Inc., 2012 
///// All rights reserved. 
///// 
///// \author  Juanyong.Yang  juanyong.yang@united-imaging.com
////
///// \file    mcsf_review_image_data.cs
/////
///// \brief   image data model
///// 
///// \version 1
///// \date    2012/02/22
///////////////////////////////////////////////////////////////////////////

//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Windows;
//using System.Windows.Interop;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Threading;
//using System.Collections.ObjectModel;
//using UIH.Mcsf.Viewer;
//using System.ComponentModel;

//namespace UIH.Mcsf.Filming
//{
//    public class ImageData
//    {
//        private ReviewDataStatus _status;
//        private DisplayData _displayData;
//        private bool _isImageLoaded;
//        private string _sopInstanceUID;
//        private string _description;
//        private string _filePath;
//        private string _gspsFilePath;
//        private string _thumbnailFilePath;

//        public string SOPInstanceUID
//        {
//            get { return _sopInstanceUID; }
//            set
//            {
//                _sopInstanceUID = value;
//            }
//        }

//        public bool IsDisplayDataCreated
//        {
//            get { return _isImageLoaded; }
//            set
//            {
//                _isImageLoaded = value;
//            }
//        }

//        public DisplayData DisplayData
//        {
//            get { return _displayData; }
//            set
//            {
//                _displayData = value;
//            }
//        }

//        public string FilePath
//        {
//            get { return _filePath; }
//            set { _filePath = value; }
//        }

//        public string GSPSFilePath
//        {
//            get { return _gspsFilePath; }
//            set { _gspsFilePath = value; }
//        }

//        public string ThumbnailFilePath
//        {
//            get { return _thumbnailFilePath; }
//            set { _thumbnailFilePath = value; }
//        }

//        public ImageSource ImageSource
//        {
//            get 
//            { 
//                if(string.IsNullOrEmpty(ThumbnailFilePath))
//                {
//                    return null;
//                }

//                if(!File.Exists(ThumbnailFilePath))
//                {
//                    return null;
//                }

//                try
//                {
//                    var bitmap = new Bitmap(ThumbnailFilePath);

//                    IntPtr hBitmap = bitmap.GetHbitmap();

//                    ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
//                        hBitmap,
//                        IntPtr.Zero,
//                        Int32Rect.Empty,
//                        BitmapSizeOptions.FromEmptyOptions());

//                    return wpfBitmap;
//                }
//                catch (Exception ex)
//                {
//                    Logger.LogError("Create thumbnail failed!"+ex.StackTrace+"! path:"+ThumbnailFilePath);
//                    return null;
//                }
//            }
//        }

//        public string SharedMemName
//        {
//            get;
//            set;
//        }

//        public ReviewDataStatus Status
//        {
//            get { return _status; }
//            set
//            {
//                _status = value;
//            }
//        }

//        public string Description
//        {
//            get { return _description; }
//            set 
//            { 
//                _description = value;
//            }
//        }

//        public ImageData(string sopInstanceUID)
//        {
//            this._sopInstanceUID = sopInstanceUID;
//            this.Status = ReviewDataStatus.Initializing;
//        }

//        public void CreateDisplayData(string sharedMemName)
//        {
//            DataAccessor accessor = new DataAccessor();
//            _displayData = accessor.CreateImageData(sharedMemName);

//            this.IsDisplayDataCreated = true;
//        }
//    }
//}
