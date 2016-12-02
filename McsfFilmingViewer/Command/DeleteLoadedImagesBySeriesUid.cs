using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using UIH.Mcsf.Core;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.Filming.Views;

namespace UIH.Mcsf.Filming.Command
{
    class DeleteLoadedImagesBySeriesUids : ICLRCommandHandler
    {
        public override int HandleCommand(CommandContext cc, ISyncResult pSyncResult)
        {
            var encoding = new UTF8Encoding();
            string uids = encoding.GetString(cc.sSerializeObject);
            var seriesUidList = uids.Split('#').ToList();
            DeleteLoadedImages(seriesUidList);
            return 0;
        }

        private void DeleteLoadedImages(List<string> seriesUidList)
        {
            var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
            if (filmingCard == null)
            {
                throw new Exception("filming card is not available");
            }
            if (filmingCard.IfZoomWindowShowState)
            {
                var zoomViewer = filmingCard.zoomWindowGrid.Children[0] as FilmingZoomViewer;
                if (zoomViewer != null)
                {
                    zoomViewer.CloseDialog();
                }
            }
            
            foreach (var page in filmingCard.EntityFilmingPageList)
            {
                foreach (var cell in page.Cells)
                {
                    if (cell.Image == null)
                        continue;

                    var seriesUid = FilmPageUtil.GetSeriesUidFromImage(cell.Image);
                    if (!string.IsNullOrEmpty(seriesUid) && seriesUidList.Contains(seriesUid))
                    {
                        FilmPageUtil.ClearAllActions(cell);
                        cell.Image.Clear();
                        cell.IsSelected = false; //删除图片后应去除多选状态
                        cell.Refresh();
                    }
                }
            }
            if(filmingCard.IsEnableRepack)
                filmingCard.contextMenu.Repack();
            else
            {
                filmingCard.EntityFilmingPageList.UpdatePageLabel();
            }
        }
    }
}
