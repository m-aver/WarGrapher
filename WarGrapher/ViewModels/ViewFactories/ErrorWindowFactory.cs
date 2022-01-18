using WarGrapher.Views;

namespace WarGrapher.ViewModels.ViewFactories
{
    class ErrorWindowFactory : WindowFactory
    {
        protected override WindowViewBase CreateNewWindow()
        {
            return new ErrorWindowView();
        }

        protected override WindowViewModel CreateViewModel()
        {
            return new ErrorWindowViewModel();
        }
    }
}
