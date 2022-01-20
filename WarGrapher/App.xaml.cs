using System;
using System.Windows;
using System.Windows.Threading;
using WarGrapher.ViewModels;
using WarGrapher.ViewModels.ViewFactories;
using WarGrapher.Views;

namespace WarGrapher
{
    /// <summary>
    /// Represents this application.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Gets the path to the application directory.
        /// </summary>
        public static string AppFolder { get; private set; }

        /// <summary>
        /// Handles the launch of the application.
        /// </summary>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppFolder = AppDomain.CurrentDomain.BaseDirectory;

            try
            {
                WindowFactory factory = new MainWindowFactory();
                WindowViewModel mainViewModel = factory.GetCommonViewModel();
                factory.CreateWindow(mainViewModel);
            }
            catch (Exception ex)
            {
                //notify about a critical error if the main window is not opened
                try
                {
                    CriticalErrorWindow errorWin = new CriticalErrorWindow();
                    errorWin.Closed += (obj, arg) => this.Shutdown();
                    errorWin.Show();

                    errorWin.SendError(ex);
                }
                catch
                {
                    this.Shutdown();
                }
            }
        }

        /// <summary>
        /// Handles unhandled exceptions that thrown from code of this application.
        /// Serves as the shared point for handling any unexpected error.
        /// </summary>
        private void HandleException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            WindowFactory errorViewFactory = new ErrorWindowFactory();
            IErrorRecorder errorVIewModel = (IErrorRecorder)errorViewFactory.GetCommonViewModel();
            errorVIewModel.SendError(ErrorType.UnhandledError, e.Exception);

            e.Handled = true;
        }
    }
}
