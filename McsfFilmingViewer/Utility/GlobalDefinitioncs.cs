namespace UIH.Mcsf.Filming.Utility
{
    public enum LocalDragDropEffects
    {
        InsertBefore,
        InsertAfter,
        Forbit,
        None
    }
    public enum RepackMode
    {
        RepackDefault = 0,
        RepackCut = 1,
        RepackDelete = 2,
        RepackPaste = 3,
        RepackLoad = 4,
        RepackDrag = 5,
        RepackMenu = 6
    }
    public enum FilmPageType
    {
        NormalFilmPage,
        BatchFilmPage,
        SingleSeriesCompareFilmPage,
        MultiSeriesCompareFilmPage,
        BreakFilmPage
    }

    //filming正在操作状态
    public enum FilmingRunState
    {
        Default, //默认
        Loading, //加载图片
        ChangeLayout, //切换布局
        Delete, //删除图片
        Paste,
        LocalImageRefrence, //添加定位像
        InsertImageRefrence //添加参考线组

    }
}
