using System.ComponentModel;

namespace WarGrapher.ViewModels
{
    /// <summary>
    /// Serves as the base class of a view-model
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);
            PropertyChangedEventHandler propertyChangedHandler = this.PropertyChanged;
            propertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Verify that the property name matches a real, public, instance property on this object. 
        /// </summary>
        private void VerifyPropertyName(string propertyName)
        {
            if (this.GetType().GetProperty(propertyName) == null)       
            {
                string msg = "Invalid property name: " + propertyName;
                //throw new Exception(msg);
            }
        } 
    }
}
