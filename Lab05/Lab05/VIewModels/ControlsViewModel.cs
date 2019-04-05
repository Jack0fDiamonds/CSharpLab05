using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Lab05.Tools;
using Lab05.Tools.Managers;
using Lab05.Tools.Navigation;
using System.Diagnostics;
using System.IO;

namespace Lab05.VIewModels
{
    class ControlsViewModel : INotifyPropertyChanged
    {
        private string _detailsButtonText = "Details";

        public string DetailsButtonText
        {
            get => _detailsButtonText;
            set
            {
                _detailsButtonText = value;
                OnPropertyChanged();
            }
        }

        private bool _controlsAvailable = true;

        public bool ControlsAvailable
        {
            get => _controlsAvailable;
            private set
            {
                _controlsAvailable = value;
                OnPropertyChanged();
            }
        }

        private ICommand _buttonClickCommand;
        private ICommand _terminateCommand;
        private ICommand _openFolderCommand;

        public ICommand DetailsButtonClickCommand
        {
            get
            {
                return _buttonClickCommand ??
                       (_buttonClickCommand = new RelayCommand<object>(DetailsButtonClickImplementation));
            }
        }

        public ICommand TerminateCommand
        {
            get
            {
                return _terminateCommand ??
                       (_terminateCommand = new RelayCommand<object>(TerminateCommandImplementation, CanExecuteControlsCommands));
            }
        }

        public ICommand OpenFolderCommand
        {
            get
            {
                return _openFolderCommand ??
                       (_openFolderCommand = new RelayCommand<object>(OpenFolderCommandImplementation, CanExecuteControlsCommands));
            }
        }

        private bool CanExecuteControlsCommands(object obj)
        {
            return ControlsAvailable && StationManager.CurrentProcessId > -1;
        }

        private bool IsDetailsButtonAvailable(object obj)
        {
            return StationManager.CurrentProcessId > -1;
        }

        private void DetailsButtonClickImplementation(object obj)
        {
            switch (StationManager.CurrentView)
            {
                case ViewType.TaskManager:
                    DetailsButtonText = "Back";
                    NavigationManager.Instance.Navigate(ViewType.ProcessDetails);
                    StationManager.CurrentView = ViewType.ProcessDetails;
                    ControlsAvailable = false;
                    break;
                case ViewType.ProcessDetails:
                    DetailsButtonText = "Details";
                    NavigationManager.Instance.Navigate(ViewType.TaskManager);
                    StationManager.CurrentView = ViewType.TaskManager;
                    ControlsAvailable = true;
                    break;
            }
        }

        private void TerminateCommandImplementation(object obj)
        {
            if (StationManager.CurrentProcessId > 0)
            {
                try
                {
                    Process.GetProcessById(StationManager.CurrentProcessId).Kill();
                    StationManager.CurrentProcessId = -1;
                    DetailsButtonText = "Details";
                    NavigationManager.Instance.Navigate(ViewType.TaskManager);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not teminate process. Reason:\n" + ex.Message);
                }
            }
        }

        private void OpenFolderCommandImplementation(object obj)
        {
            if (StationManager.CurrentProcessId > 0)
            {
                try
                {
                    Process.Start("explorer.exe",
                        Path.GetDirectoryName(Process.GetProcessById(StationManager.CurrentProcessId).MainModule.FileName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not open folder. Reason:\n" + ex.Message);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
