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

        private bool allComponentsAdded = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Loaded");
            Network = new Network();
            Machine = new Machine();

            addRow("Machine");
            addKeyLabel("Machine", true);

            addRow("IPv4");
            addKeyLabel("IPv4", false);
            addValueLabel("IPv4", Machine.IPv4);

            addRow("IPv6");
            addKeyLabel("IPv6", false);
            addValueLabel("IPv6", Machine.IPv6);

            addRow("MAC_Address");
            addKeyLabel("MAC_Address", false);
            addValueLabel("MAC_Address", Machine.MACAddress);

            addRow("Subnet_Mask");
            addKeyLabel("Subnet_Mask", false);
            addValueLabel("Subnet_Mask", Machine.SubnetMask);

            addRow("Machine_Name");
            addKeyLabel("Machine_Name", false);
            addValueLabel("Machine_Name", Machine.MachineName);

            addRow("Operating_System");
            addKeyLabel("Operating_System", false);
            addValueLabel("Operating_System", Machine.OperatingSystem);

            addRow("OS_Architecture");
            addKeyLabel("OS_Architecture", false);
            addValueLabel("OS_Architecture", Machine.OSArchitecture);

            addRow("OS_Manufacturer");
            addKeyLabel("OS_Manufacturer", false);
            addValueLabel("OS_Manufacturer", Machine.OSManufacturer);

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

            //foreach (double height in windowHeights)
            //{
            //    Console.WriteLine(height);
            //}


            //this.MinWidth = this.ActualWidth;
            //this.MinHeight = this.Height;
            //this.MaxHeight = this.Height;
            allComponentsAdded = true;
        }

        private void setValue()
        {

        }

        private void addRow(string name)
        {
            RowDefinition newRow = new RowDefinition();
            newRow.Height = new GridLength(1.0, GridUnitType.Auto);
            newRow.Name = name;

            GridMain.RowDefinitions.Add(newRow);
        }

        private void addValueLabel(string valueName, string value)
        {
            Label newLabel_val = new Label();
            newLabel_val.Content = "-";
            newLabel_val.Name = (valueName + "_val");

            Grid.SetRow(newLabel_val, (GridMain.RowDefinitions.Count - 1));
            Grid.SetColumn(newLabel_val, 1);

            newLabel_val.FontWeight = FontWeights.Light;
            newLabel_val.Foreground = Brushes.White;
            newLabel_val.HorizontalAlignment = HorizontalAlignment.Right;
            newLabel_val.VerticalAlignment = VerticalAlignment.Center;

            GridMain.Children.Add(newLabel_val);
        }

        private void addKeyLabel(string keyName, bool isHeader)
        {
            Label newLabel_key = new Label();
            newLabel_key.Name = (keyName + "_key");

            if (keyName.Contains("_"))
                keyName = keyName.Replace("_", " ");

            newLabel_key.Content = keyName;

            Grid.SetRow(newLabel_key, (GridMain.RowDefinitions.Count - 1));
            Grid.SetColumn(newLabel_key, 0);

            if (isHeader)
            {
                Grid.SetColumnSpan(newLabel_key, 2);
                newLabel_key.FontWeight = FontWeights.Heavy;
            } else
                newLabel_key.FontWeight = FontWeights.Light;

            newLabel_key.Foreground = Brushes.White;
            newLabel_key.HorizontalAlignment = HorizontalAlignment.Left;
            newLabel_key.VerticalAlignment = VerticalAlignment.Center;

            Thickness labelThick = new Thickness();
            labelThick.Bottom = 0;
            labelThick.Top = 0;
            labelThick.Left = 5;
            labelThick.Right = 5;

            newLabel_key.Padding = labelThick;

            GridMain.Children.Add(newLabel_key);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (allComponentsAdded)
            {
                this.MinHeight = this.ActualHeight;
                this.MaxHeight = this.ActualHeight;
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            Console.WriteLine("Initialized");
        }
    }
}
