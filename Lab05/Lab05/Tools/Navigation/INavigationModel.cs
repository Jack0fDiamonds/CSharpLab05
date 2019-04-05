using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab05.Tools.Navigation
{
    internal enum ViewType
    {
        TaskManager,
        ProcessDetails
    }

    interface INavigationModel
    {
        void Navigate(ViewType viewType);
    }
}
