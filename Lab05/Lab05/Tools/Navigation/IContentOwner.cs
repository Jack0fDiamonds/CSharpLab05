using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Lab05.Tools.Navigation
{
    interface IContentOwner
    {
        ContentControl ContentControl { get; }
    }
}
