using System.Windows.Input;
using WarGrapher.Common;
using WarGrapher.Models;
using WarGrapher.ViewModels.Commands;

namespace WarGrapher.ViewModels
{
    /// <summary>
    /// Represents a view-model that provides commands and properties to a view for selection of a targed hitbox part 
    /// </summary>
    class BodyPartSelectionViewModel : ElementViewModel
    {
        private BodyPart _sentValue;
        /// <summary>
        /// Gets the hitbox part that is sent to the application data model      
        /// </summary>
        public BodyPart SentValue
        {
            get { return _sentValue; }
            private set
            {
                _sentValue = value;
                OnPropertyChanged(nameof(SentValue));
            }
        }

        /// <summary>
        /// Gets the command that executes sending of the selected hitbox part to the application data model
        /// </summary>
        public ICommand SendFocusedBodyPartCommand { get; }

        private IBodyPartDataProvider _model;

        public BodyPartSelectionViewModel()
        {
            _model = ModelFactory.ModelInstance;

            SendFocusedBodyPartCommand = new RelayCommand<BodyPart>(
                canExecute: CanSendBodyPart,
                execute: SendBodyPart);
        }

        private bool CanSendBodyPart(BodyPart focusedBodyPart) => focusedBodyPart != SentValue;
        private void SendBodyPart(BodyPart focusedBodyPart)
        {
            _model.FocusedBodyPart = focusedBodyPart;
            SentValue = focusedBodyPart;
        }
    }
}
