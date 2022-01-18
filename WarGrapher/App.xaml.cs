using System;
using System.Windows;
using System.Windows.Threading;
using WarGrapher.ViewModels;
using WarGrapher.ViewModels.ViewFactories;
using WarGrapher.Views;

#region REMARK
/*
 * были добавлены пользовательские настройки в .csproj файл с целью удаления папок локализации System.Windows.Interactivity из bin/Debug
 * также, ссылка на System.Windows.Interactivity устанавливалась с помощью NuGet, а не дефолтного Reference Manager
 * по пути была удалена ссылка на лишнюю библиотеку, поставляемую NuGet вместе с System.Windows.Interactivity
 * и добавлена кастомная настройка в .csproj файл, удаляющая дополнительные файлы, поставляемые вместе с .dll сборками, из папки bin/Debug,
 * которая нужна для фильтрации .xml файлов докуметации поставляемой NuGet вместе со сборками
 */
//SOURCES:
/*
 * https://stackoverflow.com/questions/20342061/disable-dll-culture-folders-on-compile
 * https://stackoverflow.com/questions/64273946/do-not-copy-xml-documentation-files-from-nuget-packages-into-bin
 */
#endregion

namespace WarGrapher
{
    /// <summary>
    /// Represents this application.
    /// </summary>
    public partial class App : Application
    {
        #region REMARK
        /*
        инициализация полей/свойств должна происходить в Application_Startup(), а не в конструкторе или в строке объявления
        т.к. если заглянут в автоматически сгенерированную часть класса, то можно увидеть, что считывание App.xaml (в InitializeComponent) происходит после создания экземпляра App и перед запуском приложения (Application_Startup)
        из-за этого в конструкторе App не будут доступны, например, ресурсы приложения, которые считываются из XAML файла, что может привести к ошибке, если создавать в констукторе окно, ссылающееся на эти ресурсы
        */
        #endregion

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
            #region REMARK
            /*
             * При запуске приложения из внешнего кода с помощью Process.Start()
             * Directory.GetCurrentDirectory() возвращает директорию внешнего приложения, которое инициирует запуск
             * а AppDomain.CurrentDomain.BaseDirectory возвращает родную директорию запускаемого приложения
             */
            #endregion

            try
            {
                WindowFactory factory = new MainWindowFactory();
                WindowViewModel mainViewModel = factory.GetCommonViewModel();
                factory.CreateWindow(mainViewModel);

                #region REMARK
                /*
                 интересно, что при ошибке создания экземпляра главного окна (т.е. исключение вылетело в коде конструктора)
                 создание экземпляра не отменяется, как можно было бы подумать, и поле this.MainWindow не остается null, а хранит ссылку на некоторый объект MainWindow
                 однако при попытке отобразить этот объект, на экране появляется только полностью черное окно
                 видимо поле this.MainWindow автоматически заполняется объектом главного окна, которое еще не полностью скомпановано (т.е. до генерации исключения)
                 это может приводить к очень неприятным вещам, например, если установлен режим остановки приложения после закрытия главного окна, то процесс приложения останется висеть в ОС, т.к. главное окно даже не появится, чтобы его можно было закрыть из интерфейса, придется вырубать приложение через диспетчер задач
                */
                #endregion
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
