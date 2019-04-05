using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows;

namespace Lab05.Models
{
    class MyProcess
    {
        private string _name;
        private int _id;
        private bool _state;
        private double _cpu;
        private double _memory;
        private int _threads;
        private string _userName;
        private string _filePath;
        private DateTime _startTime;

        public string Name { get => _name; }
        public int ID { get => _id; }
        public string State { get => _state ? "yes" : "no"; }
        public double CPU { get => _cpu; set => _cpu = value; }
        public string CPUPercentageString { get => String.Format("{0:0.00}", _cpu) + '%'; }
        public double Memory { get => _memory; set => _memory = value; }
        public string MemoryPercentageString { get; set; }
        public int Threads { get => _threads; }
        public string UserName { get => _userName; }
        public string FilePath { get => _filePath; }
        public string StartTime { get => _startTime.Equals(DateTime.MinValue) ? "unknown" : _startTime.ToShortDateString() + ' ' + _startTime.ToShortTimeString(); }
        
        public MyProcess(Process process)
        {
            _name = process.ProcessName;
            _id = process.Id;
            _state = process.Responding;
            _cpu = 0;
            _memory = 0;
            _threads = process.Threads.Count;
            _userName = GetUserName(process);
            _filePath = GetFilePath(process);
            _startTime = GetStartTime(process);
        }

        private string GetFilePath(Process process)
        {
            try
            {
                return process.MainModule.FileName;
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        private DateTime GetStartTime(Process process)
        {
            try
            {
                return process.StartTime;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        private static string GetUserName(Process process)
        {
            IntPtr processHandle = IntPtr.Zero;
            try
            {
                OpenProcessToken(process.Handle, 8, out processHandle);
                WindowsIdentity wi = new WindowsIdentity(processHandle);
                string user = wi.Name;
                return user.Contains(@"\") ? user.Substring(user.IndexOf(@"\") + 1) : user;
            }
            catch
            {
                return String.Empty;
            }
            finally
            {
                if (processHandle != IntPtr.Zero)
                {
                    CloseHandle(processHandle);
                }
            }
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);
    }
}