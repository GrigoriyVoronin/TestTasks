using System.ComponentModel;
using System.Runtime.CompilerServices;
using NovatorTestTask.Annotations;
using NovatorTestTask.Data;
using NovatorTestTask.Models;
using NovatorTestTask.Views;

namespace NovatorTestTask.VIewModels
{
    public sealed class AddWorkerVM : INotifyPropertyChanged
    {
        private RelayCommand addCommand;

        public AddWorkerVM()
        {
            Title = "Добавление";
            Worker = new Worker();
            WorkerContext = new WorkerContext();
            OnPropertyChanged(nameof(Title));
        }

        public WorkerContext WorkerContext { get; set; }

        public MainVM MainVM { get; set; }

        public string Title { get; set; }

        public Worker Worker { get; set; }

        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ?? (addCommand = new RelayCommand(obj =>
                    {
                        WorkerContext.Workers.Add(new Worker
                        {
                            FirstName = Worker.FirstName,
                            SecondName = Worker.SecondName,
                            Patronymic = Worker.Patronymic
                        });
                        WorkerContext.SaveChanges();
                        Worker = new Worker();
                        MainVM.ChangeUserControl(new Workers());
                        ((WorkersVM) MainVM.CurentControl.DataContext).MainVM = MainVM;
                        OnPropertyChanged();
                    },
                    obj => !(string.IsNullOrEmpty(Worker.FirstName)
                             || string.IsNullOrEmpty(Worker.SecondName)
                             || string.IsNullOrEmpty(Worker.Patronymic))));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}