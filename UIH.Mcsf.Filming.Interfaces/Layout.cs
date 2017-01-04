using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Interfaces
{
    public abstract class Layout
    {
        public abstract int Capacity { get; }
        // TODO-New-Feature: Layout.ViewPort Layout Dependency Property
        // TODO-New-Feature: Layout.regularLayout & irregularLayout

        // TODO-Later: Layout.Setup(LayoutManager layoutManager) 使用桥接模式 解除对Viewer.Control.dll的依赖
        public abstract void Setup(LayoutManager layoutManager);

        // TODO-Later: Layout.Equals HashCode For Dependency Property ViewerControlAdapter.Layout

    }
}