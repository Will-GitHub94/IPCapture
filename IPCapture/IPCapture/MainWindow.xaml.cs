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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Management;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;
using NativeWifi;

namespace IPCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Machine Machine;
        private static Network Network;

        public int RowCount = 0;
        public int ColumnCount = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Network = new Network();
            Machine = new Machine();

            addMachineRow();

            addRow("IPv4");

            //----------------------------------------------------------

            //IPv4_val.Content = Machine.IPv4;
            //IPv6_val.Content = Machine.IPv6;
            //MAC_val.Content = Machine.MACAddress;
            //Subnet_val.Content = Machine.SubnetMask;
            //HostName_val.Content = Machine.HostName;
            //OperatingSystem_val.Content = Machine.OperatingSystem;
            //OSArchitecture_val.Content = Machine.OSArchitecture;
            //OSManufacturer_val.Content = Machine.OSManufacturer;

            //ExternalIP_val.Content = Network.ExternalIP;
            //DefaultGateway_val.Content = Network.DefaultGateway;
        }

        private void addRow(string valueKey)
        {
            GridMain.RowDefinitions.Add(new RowDefinition());
            GridMain.RowDefinitions[(GridMain.RowDefinitions.Count - 1)].Height = new GridLength(1, GridUnitType.Star);

            addKeyValuePair(valueKey);
        }

        private void addKeyValuePair(string valueKey)
        {
            // add key
            Label newLabel_key = new Label();
            newLabel_key.Content = (valueKey + ":");
            newLabel_key.Name = (valueKey + "_key");

            Grid.SetRow(newLabel_key, (GridMain.RowDefinitions.Count - 1));
            Grid.SetColumn(newLabel_key, 0);


            // add val
            Label newLabel_val = new Label();
            newLabel_val.Content = "-";
            newLabel_val.Name = (valueKey + "_val");

            Grid.SetRow(newLabel_val, (GridMain.RowDefinitions.Count - 1));
            Grid.SetColumn(newLabel_val, 1);
        }

        private void addMachineRow()
        {
            // adds initial row for "Machine"
            GridMain.RowDefinitions.Insert(0, new RowDefinition());

            // sets percentage height
            GridMain.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);

            //creates label "Machine"
            Label Machine_label = new Label();
            Machine_label.Content = "Machine";

            // adds label to grid
            Grid.SetRow(Machine_label, 0);
            Grid.SetColumn(Machine_label, 0);
            Grid.SetColumnSpan(Machine_label, 2);
        }

        private void addNetworkRow()
        {

        }
    }
}
