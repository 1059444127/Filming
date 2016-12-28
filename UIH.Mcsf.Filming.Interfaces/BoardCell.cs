namespace UIH.Mcsf.Filming.Interfaces
{
    public class BoardCell
    {
        // TODO: BoardCell.PageDataChanged Event
        private readonly PageModel _pageModel;
        // TODO-Later: Replace BoardCell(PageModel PageModel) with PageModel.setter
        public BoardCell(PageModel pageModel)
        {
            _pageModel = pageModel;
        }

        public BoardCell()
        {
            // TODO: Create Class NullLayout
            _pageModel = new PageModel();
        }

        public bool IsVisible
        {
            set { _pageModel.IsVisible = value; }
        }

        public PageModel PageModel
        {
            get { return _pageModel; }
        }
    }
}