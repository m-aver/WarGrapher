using WarGrapher.Views;

namespace WarGrapher.ViewModels.ViewFactories
{
    class GraphWindowFactory : WindowFactory
    {
        protected override WindowViewBase CreateNewWindow()
        {
            return new GraphWindowView();
        }

        protected override WindowViewModel CreateViewModel()
        {
            return new GraphWindowViewModel();
        }
    }
}
