using JetBrains.Annotations;
using Lab05.Models;
using Lab05.Tools.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Lab05.VIewModels
{
    class TaskManagerViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<MyProcess> _processes;
        private CancellationToken _token;
        private CancellationTokenSource _tokenSource;
        private Task _updateProcessesTask;
        private readonly long TotalRAM;

        public ObservableCollection<MyProcess> Processes
        {
            get => _processes;
            private set
            {
                _processes = value;
                OnPropertyChanged();
            }
        }

        internal TaskManagerViewModel()
        {
            GetPhysicallyInstalledSystemMemory(out TotalRAM);
            GetCpuUsages(Process.GetProcesses());
            GetProcesses();
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            StartUpdateProcessesTask();
            StationManager.StopThreads += StopUpdateProcessesTask;
        }

        private void GetProcesses()
        {
            Process[] processes = Process.GetProcesses();

            List<MyProcess> myProcesses = new List<MyProcess>();
            Dictionary<int, double> idMemoryPairs = GetIdMemoryPairs();
            GetCpuUsages(processes);
            float totalCpuUsage = cpuUsages[-1].NextValue();
            int i = 0;
            foreach (Process process in processes)
            {
                myProcesses.Add(new MyProcess(process));
                if (idMemoryPairs.ContainsKey(myProcesses[i].ID))
                {
                    myProcesses[i].Memory = idMemoryPairs[myProcesses[i].ID];
                    myProcesses[i].MemoryPercentageString = String.Format("{0:0.00}", myProcesses[i].Memory / TotalRAM * 100) + "%";
                    myProcesses[i].CPU = cpuUsages[process.Id].NextValue() / totalCpuUsage * 100;
                }
                i++;
            }
            var sortedProcesses = myProcesses.OrderByDescending(p => p.Name);
            switch ((StationManager.Test)StationManager.SortBy)
            {
                case StationManager.Test.Name:
                    sortedProcesses = myProcesses.OrderBy(p => p.Name);
                    break;
                case StationManager.Test.ID:
                    sortedProcesses = myProcesses.OrderBy(p => p.ID);
                    break;
                case StationManager.Test.Working:
                    sortedProcesses = myProcesses.OrderBy(p => p.State);
                    break;
                case StationManager.Test.CPU:
                    sortedProcesses = myProcesses.OrderByDescending(p => p.CPU);
                    break;
                case StationManager.Test.Memory:
                    sortedProcesses = myProcesses.OrderByDescending(p => p.Memory);
                    break;
                case StationManager.Test.Threads:
                    sortedProcesses = myProcesses.OrderByDescending(p => p.Threads);
                    break;
                case StationManager.Test.UserName:
                    sortedProcesses = myProcesses.OrderBy(p => p.UserName);
                    break;
                case StationManager.Test.StartTime:
                    sortedProcesses = myProcesses.OrderByDescending(p => p.StartTime);
                    break;
                case StationManager.Test.FilePath:
                    sortedProcesses = myProcesses.OrderBy(p => p.FilePath);
                    break;
                default:
                    sortedProcesses = myProcesses.OrderBy(p => p.Name);
                    break;
            }
            Processes = new ObservableCollection<MyProcess>(sortedProcesses);
        }

        private void StartUpdateProcessesTask()
        {
            _updateProcessesTask = Task.Factory.StartNew(UpdateProcessesTaskProcess, TaskCreationOptions.LongRunning);
        }

        private void UpdateProcessesTaskProcess()
        {
            while (!_token.IsCancellationRequested)
            {
                GetProcesses();
                    for (int i = 0; i < 4; ++i)
                    {
                        Thread.Sleep(250);
                        if(_token.IsCancellationRequested)
                            break;
                    }
            }
        }

        private void StopUpdateProcessesTask()
        {
            _tokenSource.Cancel();
            _updateProcessesTask.Wait(1000);
            _updateProcessesTask.Dispose();
            _updateProcessesTask = null;
        }

        private Dictionary<int, double> GetIdMemoryPairs()
        {
            Dictionary<int, double> idMemoryPairs = new Dictionary<int, double>();
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT * FROM Win32_PerfFormattedData_PerfProc_Process");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    if(!idMemoryPairs.ContainsKey(Convert.ToInt32(queryObj["IDProcess"])))
                        idMemoryPairs.Add(Convert.ToInt32(queryObj["IDProcess"]), Convert.ToInt64(queryObj["WorkingSetPrivate"].ToString()) / 1024);
                }

                return idMemoryPairs;
            }
            catch (Exception){}

            return idMemoryPairs;
        }



        private Dictionary<int, PerformanceCounter> cpuUsages = new Dictionary<int, PerformanceCounter>();
        private bool firstCpuUsagesCall = true;

        private void GetCpuUsages(Process[] processes)
        {
            if (firstCpuUsagesCall)
            {
                firstCpuUsagesCall = false;
                cpuUsages.Add(-1, new PerformanceCounter("Process", "% Processor Time", "_Total"));
            }

            foreach (Process process in processes)
            {
                if (!cpuUsages.ContainsKey(process.Id))
                {
                    cpuUsages.Add(process.Id, new PerformanceCounter("Process", "% Processor Time", process.ProcessName));
                }
            }
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}