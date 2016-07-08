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
using System.Reflection;

namespace IPCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Machine Machine;
        private static Network Network;

        private const string EMPTY = "-";
        private double MinWindowWidth = 0.0;

        public MainWindow()
        {
            Network = new Network();
            Machine = new Machine();

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //------------------------------------------------------
            //---------------------MACHINE--------------------------
            //------------------------------------------------------

            addRow("Machine");
            addKeyLabel("Machine", true);

            addKeyValuePairRow("Machine", "IPv4");
            addKeyValuePairRow("Machine", "IPv6");
            addKeyValuePairRow("Machine", "MAC_Address");
            addKeyValuePairRow("Machine", "Subnet_Mask");
            addKeyValuePairRow("Machine", "Machine_Name");
            addKeyValuePairRow("Machine", "Operating_System");
            addKeyValuePairRow("Machine", "OS_Architecture");
            addKeyValuePairRow("Machine", "OS_Manufacturer");

            //------------------------------------------------------
            //---------------------NETWORK--------------------------
            //------------------------------------------------------

            addRow("Network");
            addKeyLabel("Network", true);

            addKeyValuePairRow("Network", "Network_Connection");
            addKeyValuePairRow("Network", "Network_Connection_Type");
            addKeyValuePairRow("Network", "SSID");
            addKeyValuePairRow("Network", "Default_Gateway");
            addKeyValuePairRow("Network", "Internet_Connection");
            addKeyValuePairRow("Network", "External_IP");
        }

        private void addKeyValuePairRow(string whichClass, string item)
        {
            addRow(item);
            addKeyLabel(item, false);
            addValueLabel(item);

            if (item.Contains("_"))
                item = item.Replace("_", "");

            if (whichClass == "Machine")
                setValue(Machine.GetType().GetProperty(item).GetValue(Machine, null).ToString());
            else
                setValue(Network.GetType().GetProperty(item).GetValue(Network, null).ToString());
        }

        private void setValue(string value)
        {
            Label valueLabel = GridMain.Children
                .Cast<Label>()
                .First(e => Grid.GetRow(e) == (GridMain.RowDefinitions.Count - 1) && Grid.GetColumn(e) == 1);

            valueLabel.Content = value;
         }

        private void addRow(string name)
        {
            RowDefinition newRow = new RowDefinition();
            newRow.Height = new GridLength(1.0, GridUnitType.Auto);
            newRow.Name = name;

            GridMain.RowDefinitions.Add(newRow);
        }

        private void addValueLabel(string valueName)
        {
            Label newLabel_val = new Label();
            newLabel_val.Content = EMPTY;
            newLabel_val.Name = (valueName + "_val");

            Grid.SetRow(newLabel_val, (GridMain.RowDefinitions.Count - 1));
            Grid.SetColumn(newLabel_val, 1);

            newLabel_val.FontWeight = FontWeights.UltraLight;
            newLabel_val.FontSize = 12;
            newLabel_val.Foreground = Brushes.White;
            newLabel_val.HorizontalAlignment = HorizontalAlignment.Right;
            newLabel_val.VerticalAlignment = VerticalAlignment.Center;

            Thickness labelThick = new Thickness();
            labelThick.Bottom = 1;
            labelThick.Top = 1;
            labelThick.Left = 0;
            labelThick.Right = 5;

            newLabel_val.Padding = labelThick;

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

            Thickness labelThick = new Thickness();
            if (isHeader)
            {
                Grid.SetColumnSpan(newLabel_key, 2);
                newLabel_key.FontWeight = FontWeights.Bold;
                newLabel_key.FontSize = 13;

                labelThick.Bottom = 3;
                labelThick.Top = 3;
                
            } else
            {
                newLabel_key.FontWeight = FontWeights.UltraLight;
                newLabel_key.FontSize = 12;

                labelThick.Bottom = 1;
                labelThick.Top = 1;
            }

            labelThick.Left = 5;
            labelThick.Right = 0;

            newLabel_key.Foreground = Brushes.White;
            newLabel_key.HorizontalAlignment = HorizontalAlignment.Left;
            newLabel_key.VerticalAlignment = VerticalAlignment.Center;

            newLabel_key.Padding = labelThick;

            GridMain.Children.Add(newLabel_key);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            this.MinWidth = this.ActualWidth;
            this.MinHeight = this.ActualHeight;
            this.MaxHeight = this.ActualHeight;
        }
    }
}
