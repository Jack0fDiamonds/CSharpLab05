using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab05.VIewModels;
using Lab05.Views;

namespace Lab05.Tools.Navigation
{
    class InitializationNavigationModel : BaseNavigationModel
    {
        public InitializationNavigationModel(IContentOwner contentOwner) : base(contentOwner)
        {

        }

        protected override void InitializeView(ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.TaskManager:
                    ViewsDictionary.Add(viewType, new TaskManagerView());
                    break;
                case ViewType.ProcessDetails:
                    ViewsDictionary.Add(viewType, new ProcessDetailsView());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(viewType), viewType, null);
            }
        }
    }
}
