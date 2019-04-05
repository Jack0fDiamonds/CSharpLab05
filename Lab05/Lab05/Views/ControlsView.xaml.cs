using Lab05.Tools.Managers;
using Lab05.VIewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Lab05.Views
{
    /// <summary>
    /// Interaction logic for ControlsView.xaml
    /// </summary>
    public partial class ControlsView : UserControl
    {
        public ControlsView()
        {
            InitializeComponent();
            DataContext = new ControlsViewModel();
        }

        private void SortingSelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboBox comboBox = sender as ComboBox;
                if (comboBox == null)
                    return;
                ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;
                if (selectedItem == null)
                    return;
                string sortBy = selectedItem.Content.ToString();
                int index = 0;
                foreach (var comboBoxItem in comboBox.Items)
                {
                    ComboBoxItem item = (ComboBoxItem)comboBoxItem;
                    if (item.Content.ToString().Equals(sortBy))
                    {
                        StationManager.SortBy = index;
                        return;
                    }
                    index++;
                }
            }
            catch (Exception){}
        }
    }
}
