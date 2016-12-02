using System.Collections.Generic;

namespace UIH.Mcsf.Filming.DataModel
{
    public class SelectablePageList : SelectableList<Page>
    {
        public SelectablePageList(IEnumerable<Page> pages) : base(pages)
        {
        }
    }
}