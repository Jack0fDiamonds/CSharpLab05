using Lab05.Models;
using Lab05.Tools.Managers;
using Lab05.Tools.Navigation;
using Lab05.VIewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Lab05.Views
{
    /// <summary>
    /// Interaction logic for TaskManagerView.xaml
    /// </summary>
    public partial class TaskManagerView : UserControl, INavigatable
    {
        public TaskManagerView()
        {
            InitializeComponent();
            DataContext = new TaskManagerViewModel();
        }

        private void SelectedProcessesChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = sender as DataGrid;
                MyProcess selectedProcesses = dataGrid.SelectedItem as MyProcess;
                if(dataGrid.SelectedCells == null || dataGrid.SelectedCells.Count == 0)
                {
                    int index = 0;
                    foreach (var dataGridItem in dataGrid.Items)
                    {
                        MyProcess item = dataGridItem as MyProcess;
                        if (item.ID == StationManager.CurrentProcessId)
                        {
                            dataGrid.SelectedItem = dataGridItem;
                            dataGrid.Focus();
                            break;
                        }
                        index++;
                    }
                }
                else
                {
                    StationManager.CurrentProcessId = selectedProcesses.ID;
                }
            }
            catch (Exception){}
        }
    }
}

