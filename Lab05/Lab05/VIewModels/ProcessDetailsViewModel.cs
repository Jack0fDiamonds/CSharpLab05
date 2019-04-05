using JetBrains.Annotations;
using Lab05.Models;
using Lab05.Tools.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Lab05.VIewModels
{
    class ProcessDetailsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<CustomModule> _modules;
        private ObservableCollection<CustomThread> _threads;
        private string _modulesHeaderText;
        private string _threadsHeaderText;

        private CancellationToken _token;
        private CancellationTokenSource _tokenSource;
        private Task _updateModulesAndThreadsTask;

        public ObservableCollection<CustomModule> Modules
        {
            get => _modules;
            private set
            {
                _modules = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CustomThread> Threads
        {
            get => _threads;
            private set
            {
                _threads = value;
                OnPropertyChanged();
            }
        }

        public string ModulesHeaderText
        {
            get => _modulesHeaderText;
            private set
            {
                _modulesHeaderText = value;
                OnPropertyChanged();
            }
        }

        public string ThreadsHeaderText
        {
            get => _threadsHeaderText;
            private set
            {
                _threadsHeaderText = value;
                OnPropertyChanged();
            }
        }

        internal ProcessDetailsViewModel()
        {
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            StartUpdateTask();
            StationManager.StopThreads += StopUpdateTask;
        }

        private void UpdateModules()
        {
            if (StationManager.CurrentProcessId > 0)
            {
                try
                {
                    Process currentProcess = Process.GetProcessById(StationManager.CurrentProcessId);
                    List<CustomModule> modules = new List<CustomModule>();
                    foreach (ProcessModule module in currentProcess.Modules)
                    {
                        modules.Add(new CustomModule(module.ModuleName, module.FileName));
                    }

                    ModulesHeaderText = "Modules(" + modules.Count + ")";
                    var sortedModules = modules.OrderBy(m => m.Name);
                    Modules = new ObservableCollection<CustomModule>(sortedModules);
                }
                catch (Exception){}
            }
        }

        private void UpdateThreads()
        {
            if (StationManager.CurrentProcessId > 0)
            {
                try
                {
                    Process currentProcess = Process.GetProcessById(StationManager.CurrentProcessId);
                    List<CustomThread> threads = new List<CustomThread>();
                    foreach (ProcessThread thread in currentProcess.Threads)
                    {
                        threads.Add(new CustomThread(thread.Id, thread.ThreadState.ToString(), thread.StartTime));
                    }

                    ThreadsHeaderText = "Threads(" + threads.Count + ")";
                    var sortedThreads = threads.OrderBy(t => t.ID);
                    Threads = new ObservableCollection<CustomThread>(sortedThreads);
                }
                catch (Exception){}
            }
        }

        private void StartUpdateTask()
        {
            _updateModulesAndThreadsTask = Task.Factory.StartNew(UpdateTaskProcess, TaskCreationOptions.LongRunning);
        }

        private void UpdateTaskProcess()
        {
            while (!_token.IsCancellationRequested)
            {
                UpdateModules();
                UpdateThreads();
                for (int i = 0; i < 4; ++i)
                {
                    Thread.Sleep(250);
                    if (_token.IsCancellationRequested)
                        break;
                }
            }
        }

        private void StopUpdateTask()
        {
            _tokenSource.Cancel();
            _updateModulesAndThreadsTask.Wait(1000);
            _updateModulesAndThreadsTask.Dispose();
            _updateModulesAndThreadsTask = null;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
