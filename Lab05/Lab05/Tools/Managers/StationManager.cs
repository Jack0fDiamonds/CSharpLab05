using System;
using Lab05.Tools.Navigation;

namespace Lab05.Tools.Managers
{
    static class StationManager
    {
        public static event Action StopThreads;

        public enum Test
        {
            Name,
            ID,
            Working,
            CPU,
            Memory,
            Threads,
            UserName,
            StartTime,
            FilePath
        };

        internal static int SortBy { get; set; }

        internal static int CurrentProcessId { get; set; }

        internal static ViewType CurrentView { get; set; }

        internal static void CloseApp()
        {
            StopThreads?.Invoke();
            Environment.Exit(1);
        }
    }
}
