using WarGrapher.Views;

namespace WarGrapher.ViewModels.ViewFactories
{
    class MainWindowFactory : WindowFactory
    {
        protected override WindowViewBase CreateNewWindow()
        {
            return new MainWindowView();
        }

        protected override WindowViewModel CreateViewModel()
        {
            return new MainWindowViewModel();
        }
    }
}
