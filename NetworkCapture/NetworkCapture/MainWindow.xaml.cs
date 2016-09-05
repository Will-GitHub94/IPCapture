using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;

namespace NetworkCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Machine Machine { get; set; }
        private static Network Network { get; set; }

        private const string EMPTY = "-";
        private const bool TRUE = true;
        private const bool FALSE = false;

        private const string NETWORK = "Network";
        private const string MACHINE = "Machine";

        // Network member variables
        private const string DEFAULT_GATEWAY = "DefaultGateway";
        private const string EXTERNAL_IP = "ExternalIP";
        private const string ISP = "ISP";
        private const string SSID = "SSID";
        private const string NETWORK_CONNECTION = "NetworkConnection";
        private const string NETWORK_CONNECTION_TYPE = "NetworkConnectionType";
        private const string INTERNET_CONNECTION = "InternetConnection";
        private const string DNS_SUFFIX = "DNSSuffix";
        private const string DOWNLOAD_SPEED = "DownloadSpeed";
        private const string UPLOAD_SPEED = "UploadSpeed";

        // Machine member variables
        private const string IPV4 = "IPv4";
        private const string IPV6 = "IPv6";
        private const string MAC_ADDRESS = "MACAddress";
        private const string SUBNET_MASK = "SubnetMask";
        private const string MACHINE_NAME = "MachineName";
        private const string OPERATING_SYSTEM = "OperatingSystem";
        private const string OS_ARCHITECTURE = "OSArchitecture";
        private const string OS_MANUFACTURER = "OSManufacturer";

        public MainWindow()
        {
            Network = new Network();
            Machine = new Machine();

            Network.PropertyChanged += new PropertyChangedEventHandler(this.NetworkPropertyChanged);
            Machine.PropertyChanged += new PropertyChangedEventHandler(this.MachinePropertyChanged);

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //------------------------------------------------------
            //---------------------MACHINE--------------------------
            //------------------------------------------------------

            addRow(MACHINE);
            addKeyLabel(MACHINE, true);

            addKeyValuePairRow(MACHINE, IPV4);
            addKeyValuePairRow(MACHINE, IPV6);
            addKeyValuePairRow(MACHINE, MAC_ADDRESS);
            addKeyValuePairRow(MACHINE, SUBNET_MASK);
            addKeyValuePairRow(MACHINE, MACHINE_NAME);
            addKeyValuePairRow(MACHINE, OPERATING_SYSTEM);
            addKeyValuePairRow(MACHINE, OS_ARCHITECTURE);
            addKeyValuePairRow(MACHINE, OS_MANUFACTURER);

            //------------------------------------------------------
            //---------------------NETWORK--------------------------
            //------------------------------------------------------

            addRow(NETWORK);
            addKeyLabel(NETWORK, true);

            addKeyValuePairRow(NETWORK, NETWORK_CONNECTION);
            addKeyValuePairRow(NETWORK, NETWORK_CONNECTION_TYPE);
            addKeyValuePairRow(NETWORK, SSID);
            addKeyValuePairRow(NETWORK, DEFAULT_GATEWAY);
            addKeyValuePairRow(NETWORK, INTERNET_CONNECTION);
            addKeyValuePairRow(NETWORK, EXTERNAL_IP);
            addKeyValuePairRow(NETWORK, DNS_SUFFIX);
            addKeyValuePairRow(NETWORK, ISP);
            addKeyValuePairRow(NETWORK, DOWNLOAD_SPEED);
        }

        private void addKeyValuePairRow(string whichClass, string item)
        {
            addRow(item);
            addKeyLabel(item, false);
            addValueLabel(item);

            if (whichClass == MACHINE)
                setValueByLastAdded(Machine.GetType().GetProperty(item).GetValue(Machine, null).ToString());
            else
                setValueByLastAdded(Network.GetType().GetProperty(item).GetValue(Network, null).ToString());
        }

        private void checkIfValueIsError(Label valueLabel, string value)
        {
            if (value.Contains("-----"))
            {
                valueLabel.Foreground = Brushes.Black;
                valueLabel.FontStyle = FontStyles.Italic;
            } else
            {
                valueLabel.Foreground = Brushes.White;
                valueLabel.FontStyle = FontStyles.Normal;
            }
        }

        private void setValueByLastAdded(string value)
        {
            Label valueLabel = GridMain.Children
                .Cast<Label>()
                .First(e => Grid.GetRow(e) == (GridMain.RowDefinitions.Count - 1) && Grid.GetColumn(e) == 1);

            valueLabel.Content = value;

            checkIfValueIsError(valueLabel, value);
        }

        private void setValueByLabelName(string propertyName, string propertyVal)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                foreach (Label valueLabel in GridMain.Children.Cast<Label>())
                {
                    if (valueLabel.Name == (propertyName + "_val"))
                    {
                        valueLabel.Content = propertyVal;
                        checkIfValueIsError(valueLabel, propertyVal);
                        break;
                    }
                }
            }));
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

        private string getKeyName(string key)
        {
            switch (key)
            {
                case MAC_ADDRESS:
                    return "MAC Address";
                case SUBNET_MASK:
                    return "Subnet Mask";
                case MACHINE_NAME:
                    return "Machine Name";
                case OPERATING_SYSTEM:
                    return "Operating System";
                case OS_ARCHITECTURE:
                    return "OS Architecture";
                case OS_MANUFACTURER:
                    return "OS Manufacturer";
                case NETWORK_CONNECTION:
                    return "Network State";
                case NETWORK_CONNECTION_TYPE:
                    return "Connection Type(s)";
                case DEFAULT_GATEWAY:
                    return "Default Gateway";
                case INTERNET_CONNECTION:
                    return "Internet State";
                case EXTERNAL_IP:
                    return "External IP";
                case DNS_SUFFIX:
                    return "DNS Suffix";
                case DOWNLOAD_SPEED:
                    return "Download (MB/s)";
                case UPLOAD_SPEED:
                    return "Upload";
                default:
                    return key;
            }
        }

        private void addKeyLabel(string keyName, bool isHeader)
        {
            Label newLabel_key = new Label();
            newLabel_key.Name = (keyName + "_key");

            Grid.SetRow(newLabel_key, (GridMain.RowDefinitions.Count - 1));
            Grid.SetColumn(newLabel_key, 0);

            Thickness labelThick = new Thickness();

            switch (isHeader)
            {
                case TRUE:
                    Grid.SetColumnSpan(newLabel_key, 2);
                    newLabel_key.FontWeight = FontWeights.ExtraBold;
                    newLabel_key.FontSize = 15;

                    labelThick.Bottom = 3;
                    labelThick.Top = 3;

                    newLabel_key.Content = "----- " + keyName + " -----";
                    break;
                case FALSE:
                    newLabel_key.FontWeight = FontWeights.UltraLight;
                    newLabel_key.FontSize = 12;

                    labelThick.Bottom = 1;
                    labelThick.Top = 1;

                    newLabel_key.Content = getKeyName(keyName);
                    break;
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

        public void NetworkPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case DEFAULT_GATEWAY:
                    setValueByLabelName(DEFAULT_GATEWAY, Network.DefaultGateway);
                    break;
                case EXTERNAL_IP:
                    setValueByLabelName(EXTERNAL_IP, Network.ExternalIP);
                    break;
                case SSID:
                    setValueByLabelName(SSID, Network.SSID);
                    break;
                case NETWORK_CONNECTION:
                    setValueByLabelName(NETWORK_CONNECTION, Network.NetworkConnection);
                    break;
                case NETWORK_CONNECTION_TYPE:
                    setValueByLabelName(NETWORK_CONNECTION_TYPE, Network.NetworkConnectionType);
                    break;
                case INTERNET_CONNECTION:
                    setValueByLabelName(INTERNET_CONNECTION, Network.InternetConnection);
                    break;
                case DNS_SUFFIX:
                    setValueByLabelName(DNS_SUFFIX, Network.DNSSuffix);
                    break;
                case ISP:
                    setValueByLabelName(ISP, Network.ISP);
                    break;
                case DOWNLOAD_SPEED:
                    setValueByLabelName(DOWNLOAD_SPEED, Network.DownloadSpeed);
                    break;
            }
        }

        public void MachinePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Commented cases will never change while app is running

            switch (e.PropertyName)
            {
                case IPV4:
                    setValueByLabelName(IPV4, Machine.IPv4);
                    break;
                case IPV6:
                    setValueByLabelName(IPV6, Machine.IPv6);
                    break;
                case MAC_ADDRESS:
                    setValueByLabelName(MAC_ADDRESS, Machine.MACAddress);
                    break;
                case SUBNET_MASK:
                    setValueByLabelName(SUBNET_MASK, Machine.SubnetMask);
                    break;
                //case MACHINE_NAME:
                //    setValueByLabelName(MACHINE_NAME, Machine.MachineName);
                //    break;
                //case OPERATING_SYSTEM:
                //    setValueByLabelName(OPERATING_SYSTEM, Machine.OperatingSystem);
                //    break;
                //case OS_ARCHITECTURE:
                //    setValueByLabelName(OS_ARCHITECTURE, Machine.OSArchitecture);
                //    break;
                //case OS_MANUFACTURER:
                //    setValueByLabelName(OS_MANUFACTURER, Machine.OSManufacturer);
                //    break;
            }
        }
    }
}
