using Lab05.Tools.Managers;
using Lab05.Tools.Navigation;
using Lab05.VIewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Lab05
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IContentOwner
    {
        public ContentControl ContentControl
        {
            get { return _contentControl; }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            NavigationManager.Instance.Initialize(new InitializationNavigationModel(this));
            NavigationManager.Instance.Navigate(ViewType.TaskManager);
            StationManager.CurrentView = ViewType.TaskManager;
            StationManager.CurrentProcessId = -1;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            StationManager.CloseApp();
        }
    }
}
