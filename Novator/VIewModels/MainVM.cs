using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using NovatorTestTask.Annotations;
using NovatorTestTask.Views;

namespace NovatorTestTask.VIewModels
{
    public class MainVM : INotifyPropertyChanged
    {
        public MainVM()
        {
            var workers = new Workers();
            ((WorkersVM) workers.DataContext).MainVM = this;
            ChangeUserControl(workers);
        }

        public UserControl CurentControl { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void ChangeUserControl(UserControl userControl)
        {
            CurentControl = userControl;
            OnPropertyChanged(nameof(CurentControl));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}