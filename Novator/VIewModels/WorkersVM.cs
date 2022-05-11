using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using NovatorTestTask.Annotations;
using NovatorTestTask.Data;
using NovatorTestTask.Models;
using NovatorTestTask.Views;

namespace NovatorTestTask.VIewModels
{
    public sealed class WorkersVM : INotifyPropertyChanged
    {
        private RelayCommand addCommand;

        private RelayCommand deleteCommand;

        public WorkersVM()
        {
            WorkerContext = new WorkerContext();
            Title = "Работники";
            OnPropertyChanged(nameof(Workers));
        }

        public WorkerContext WorkerContext { get; set; }

        public MainVM MainVM { get; set; }

        public List<Worker> Workers => WorkerContext.Workers.ToList();

        public string Title { get; set; }

        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new RelayCommand(obj =>
                {
                    var workersView = obj as DataGrid;
                    var selectedItems = workersView.SelectedItems;
                    if (selectedItems.Count > 0)
                    {
                        foreach (var w in from
                                object item in selectedItems
                            select item as Worker)
                        {
                            WorkerContext.Workers.Remove(w);
                            WorkerContext.SaveChanges();
                        }

                        OnPropertyChanged(nameof(Workers));
                        MessageBox.Show("Пользователь успешно удалён", "Удаление сотрудника");
                    }
                    else
                    {
                        MessageBox.Show("Выберите хотя бы одного работника", "Удаление сотрудника");
                    }
                }));
            }
        }

        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ?? (addCommand = new RelayCommand(obj =>
                {
                    MainVM.ChangeUserControl(new AddWorker());
                    ((AddWorkerVM) MainVM.CurentControl.DataContext).MainVM = MainVM;
                    OnPropertyChanged();
                }));
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