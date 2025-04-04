using Caliburn.Micro;
using System.Windows;
using System.Windows.Interop;

namespace TranslateRESX.Dialog
{
    public class DialogViewModel : PropertyChangedBase, IDialogView
    {
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyOfPropertyChange();
            }
        }

        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                NotifyOfPropertyChange();
            }
        }

        private string _boldMessage;
        public string BoldMessage
        {
            get => _boldMessage;
            set
            {
                _boldMessage = value;
                NotifyOfPropertyChange();
            }
        }

        private bool _error;
        public bool Error
        {
            get => _error;
            set
            {
                _error = value;
                NotifyOfPropertyChange();
            }
        }

        public void OkCommand(Window window)
        {
            if (ComponentDispatcher.IsThreadModal)
                window.DialogResult = true;
            window.Close();
        }
    }
}
