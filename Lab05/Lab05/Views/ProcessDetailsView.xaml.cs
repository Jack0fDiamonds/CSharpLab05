using Lab05.Tools.Navigation;
using Lab05.VIewModels;
using System.Windows.Controls;

namespace Lab05.Views
{
    /// <summary>
    /// Interaction logic for ProcessDetailsView.xaml
    /// </summary>
    public partial class ProcessDetailsView : UserControl, INavigatable
    {
        public ProcessDetailsView()
        {
            InitializeComponent();
            DataContext = new ProcessDetailsViewModel();
        }
    }
}
