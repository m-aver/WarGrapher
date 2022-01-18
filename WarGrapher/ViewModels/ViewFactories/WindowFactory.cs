using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WarGrapher.Common;
using ViewModelValidationFailedException = WarGrapher.Views.ViewModelValidationFailedException;
using WindowViewBase = WarGrapher.Views.WindowViewBase;

namespace WarGrapher.ViewModels.ViewFactories
{
    /// <summary>
    /// Represents a factory of windows and associated view-models.
    /// </summary>
    abstract class WindowFactory
    {
        private static List<WindowViewModel> _commonVMs;    //for storing the shared instances of each view-model type

        private static MethodInfo _viewSetupNotificationMethod;
        private static MethodInfo _viewClosedNotificationMethod;
        private static EventInfo _requestCloseEvent;

        static WindowFactory()
        {
            _commonVMs = new List<WindowViewModel>();

            _requestCloseEvent = FindMemberByKey<EventInfo>("RequestCloseEvent");
            _viewClosedNotificationMethod = FindMemberByKey<MethodInfo>("ViewClosedNotificationMethod");
            _viewSetupNotificationMethod = FindMemberByKey<MethodInfo>("ViewSetupNotificationMethod");
        }

        private static T FindMemberByKey<T>(string memberKey) where T : MemberInfo
        {
            var targetType = typeof(WindowViewModel);
            var allMembers = targetType.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance);

            var resultKit = from member in allMembers
                            where typeof(T).IsAssignableFrom(member.GetType())
                            group member by member.GetCustomAttribute<FactoryAccessibleAttribute>() into membersAttrGroup
                            where membersAttrGroup.Key != null &&
                                  membersAttrGroup.Key.MemberKey == memberKey
                            select membersAttrGroup.AsEnumerable();

            var result = resultKit.SelectMany(e => e);
            if (result.Count() > 1)
                throw new Exception("FactoryAccessibleAttribute should have unique keys for members with same type");

            MemberInfo resultMember = result.SingleOrDefault();
            ValidateMemberTemplate(memberKey, resultMember);

            return (T)resultMember;
        }

        private static void ValidateMemberTemplate(string memberKey, MemberInfo member)
        {
            if (memberKey == "RequestCloseEvent" &&
                (member is EventInfo) &&
                (member as EventInfo).EventHandlerType != typeof(EventHandler))
            {
                throw new Exception(
                    @"member with the key ""RequestCloseEvent"" must be an event and based on the EventHandler delegate");
            }

            if ((memberKey == "ViewClosedNotificationMethod" ||
                 memberKey == "ViewSetupNotificationMethod") &&
                (member is MethodInfo) &&
                (member as MethodInfo).GetParameters().Length != 0)
            {
                throw new Exception(
                    @"members with the keys ""ViewClosedNotificationMethod"" and ""ViewSetupNotificationMethod"" must be parameterless methods");
            }
        }

        /// <summary>
        /// Creates and returns a view model instance from a specific implemenatiton of the factory
        /// </summary>
        protected abstract WindowViewModel CreateViewModel();
        /// <summary>
        /// Creates and returns a window instance from a specific implemenatiton of the factory
        /// </summary>        
        protected abstract WindowViewBase CreateNewWindow();

        /// <summary>
        /// Creates a window and returns a corresponding view model
        /// </summary>
        public WindowViewModel CreateWindow()
        {
            WindowViewModel viewModel = CreateViewModel();
            CreateWindow(viewModel);
            return viewModel;
        }

        /// <summary>
        /// Creates a new window for an existing view model
        /// </summary>
        /// <param name="viewModel">the view model for that a window will be created</param>
        /// <exception cref="ArgumentException">it's thrown if the passed view model already has a window</exception>
        /// <exception cref="ArgumentException">it's thrown if the passed view model failed a client validation</exception>
        /// <exception cref="ArgumentNullException">it's thrown if the passed view model is null</exception>
        public void CreateWindow(WindowViewModel viewModel)
        {
            WindowViewBase window = CreateNewWindow();

            ValidateComponents(viewModel, window);
            ConfigureComponents(viewModel, window);
        }

        /// <summary>
        /// Creates or retrieves a view model instance that is accessible to the entire application
        /// </summary>
        public WindowViewModel GetCommonViewModel()
        {
            WindowViewModel viewModel = CreateViewModel();
            ValidateViewModel(viewModel);

            WindowViewModel commonVM = _commonVMs.FirstOrDefault(vm => vm.GetType() == viewModel.GetType());
            if (commonVM == null)
            {
                commonVM = viewModel;
                _commonVMs.Add(commonVM);
            }

            return commonVM;
        }

        private void ValidateComponents(WindowViewModel viewModel, WindowViewBase window)
        {
            ValidateWindow(window);
            ValidateViewModel(viewModel);
        }

        private void ValidateWindow(WindowViewBase window)
        {
            if (window == null)
                throw new ArgumentNullException(nameof(window));
            if (window.IsClosed)
                throw new ArgumentException(
                    "a factory should return a new window, but it is revealed that the window already closed", nameof(window));
            if (window.IsLoaded)
                throw new ArgumentException(
                    "a factory should return a new window, but it is revealed that the window already loaded in the user interface", nameof(window));
        }

        private void ValidateViewModel(WindowViewModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            if (viewModel.HasView)
                throw new ArgumentException(
                    "a view model alreary has a window", nameof(viewModel));
        }

        private void ConfigureComponents(WindowViewModel viewModel, WindowViewBase window)
        {
            try
            {
                window.ViewModel = viewModel;
            }
            catch (ViewModelValidationFailedException ex)
            {
                window.Close();
                throw new ArgumentException("the view model failed a client validation", nameof(viewModel), ex);
            }
            window.Show();

            EventHandler requestCloseHandler =
                delegate (Object sender, EventArgs args)
                {
                    if (window != null &&
                        !window.IsClosed())
                    {
                        window.Close();
                        window = null;
                    }
                };

            _requestCloseEvent?.AddMethod.Invoke(viewModel, new[] { requestCloseHandler });

            window.Closed += (obj, arg) =>
            {
                _requestCloseEvent?.RemoveMethod.Invoke(viewModel, new[] { requestCloseHandler });
                _viewClosedNotificationMethod?.Invoke(viewModel, null);
            };

            _viewSetupNotificationMethod?.Invoke(viewModel, null);
        }
    }
}
