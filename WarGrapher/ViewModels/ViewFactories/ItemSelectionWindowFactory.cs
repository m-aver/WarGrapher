using WarGrapher.Views;

namespace WarGrapher.ViewModels.ViewFactories
{
    class ItemSelectionWindowFactory : WindowFactory
    {
        protected override WindowViewBase CreateNewWindow()
        {
            return new ItemSelectionWindowView();
        }

        protected override WindowViewModel CreateViewModel()
        {
            return new ItemSelectionWindowViewModel<EquipItemViewModel>();
        }
    }
}
