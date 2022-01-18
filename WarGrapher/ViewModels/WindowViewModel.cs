using System;
using System.Windows.Input;
using WarGrapher.ViewModels.Commands;
using FactoryAccessible = WarGrapher.ViewModels.ViewFactories.FactoryAccessibleAttribute;      //интерфейс взаимодействия с фабрикой

namespace WarGrapher.ViewModels
{
    #region REMARK
    //выделил HasView в отдельный интерфейс, т.к. 
    //периодически оно нужно во вспомогатательных интерфейсах различных ViewModel 
    //(напр. IErrorRecorder или IDataSelector) 
    #endregion
    internal interface IWindowViewModel
    {
        bool HasView { get; }
    }

    /// <summary>
    /// Represents a view-model that intended for using with an entire window as a view.
    /// </summary>
    public abstract class WindowViewModel : ViewModelBase, IWindowViewModel
    {
        /// <summary>
        /// Gets a command that executes closing a window associated with this view-model.
        /// </summary>
        public ICommand CloseViewCommand { get; }
        /// <summary>
        /// Gets a value that indicates whether the view-model is assigned to a window.
        /// </summary>
        public bool HasView { get; private set; }

        [FactoryAccessible("RequestCloseEvent")]
        private event EventHandler RequestClose;

        protected WindowViewModel()
        {
            CloseViewCommand =
                new RelayCommand(
                canExecute: CanCloseView,
                execute: OnRequestClose
                );
        }

        /// <summary>
        /// Allows derived classes to define a logic that will be executes when this view-model is assigned to a window.
        /// </summary>
        protected virtual void SetupViewHook() { }
        /// <summary>
        /// Allows derived classes to define a logic that will be executes when the associated window is closed.
        /// </summary>
        protected virtual void CloseViewHook() { }

        protected virtual bool CanCloseView() => HasView;
        private void OnRequestClose()
        {
            if (HasView)
            {
                EventHandler requestCloseHandler = RequestClose;
                requestCloseHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        [FactoryAccessible("ViewSetupNotificationMethod")]
        private void OnViewSetup()
        {
            HasView = true;
            SetupViewHook();
        }

        [FactoryAccessible("ViewClosedNotificationMethod")]
        private void OnViewClosed()
        {
            HasView = false;
            CloseViewHook();
        }
    }
}
