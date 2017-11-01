using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;
using System.Configuration;

namespace NetworkCapture
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        private static MainWindow m_parent;

        public ConfigWindow(MainWindow parent)
        {
            m_parent = parent;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateNICsVal(GetNICs());
        }

        private void config_save_btn_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.NIC = nic_val.Text;
            Properties.Settings.Default.Save();
            m_parent.Show();       
            Close();
        }

        private void PopulateNICsVal(ObservableCollection<string> NICs)
        {
            nic_val.ItemsSource = NICs;
        }

        private ObservableCollection<string> GetNICs()
        {
            ObservableCollection<string> nics = new ObservableCollection<string>();

            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface nic in adapters)
            {
                nics.Add(nic.Name);
            }
            return nics;
        }
    }
}
