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
using System.Net.NetworkInformation;
using NativeWifi;
using System.Collections.ObjectModel;

namespace IPCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Machine Machine = new Machine();
        private static Network Network = new Network();

        public int RowCount = 0;
        public int ColumnCount = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            addMachineRow();

            addRow("IPv4");
            setIPv4();


            setIPv6();
            setMACAddress();
            setSubnetMask();
            setMachineName();
            setOperatingSystem();
            setOSArchitecture();
            setOSManufacturer();

            setSSID();
            setExternalIP();
            //setDefaultGateway();

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

        private void setIPv4()
        {
            try
            {
                IPHostEntry IPhostEntry = Dns.GetHostEntry(Dns.GetHostName());
                string IPv4 = null;
                foreach (IPAddress ip in IPhostEntry.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        IPv4 = ip.ToString();
                    }
                }
                Machine.IPv4 = IPv4;
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        private void setIPv6()
        {
            try
            {
                string IPv6 = null;
                IPHostEntry IPhostEntry = Dns.GetHostEntry(Dns.GetHostName());

                foreach (IPAddress ip in IPhostEntry.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        IPv6 = ip.ToString();
                    }
                }
                Machine.IPv6 = IPv6;
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        private void setExternalIP()
        {
            try
            {
                string ExternalIP = null;
                ExternalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                ExternalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                             .Matches(ExternalIP)[0].ToString();
                Network.ExternalIP = ExternalIP;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        private void setMACAddress()
        {
            try
            {
                string MACAddress = null;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");

                foreach (ManagementObject mo in mc.Get())
                {
                    MACAddress = (string)mo["MACAddress"];
                    Console.WriteLine(MACAddress);
                }
                Machine.MACAddress = MACAddress;
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        private void setDefaultGateway()
        {
            try
            {
                string DefaultGateway = null;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");

                foreach (ManagementObject mo in mc.Get())
                {
                    Console.WriteLine(mo["IPEnabled"].ToString());
                    string[] gateways = (string[])mo["DefaultIPGateway"];
                    DefaultGateway = gateways[0];
                    Console.WriteLine(DefaultGateway);
                }
                Console.WriteLine();
                Console.WriteLine("DefaultGateway:\t{0}", DefaultGateway);
                Network.DefaultGateway = DefaultGateway;
            }
            catch (NullReferenceException nrEx)
            {
                throw nrEx;
            }
        }

        private bool InternetIsAvailable()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private void setSSID()
        {
            WlanClient wlan = new WlanClient();

            Collection<String> connectedSsids = new Collection<string>();

            foreach (WlanClient.WlanInterface wlanInterface in wlan.Interfaces)
            {
                Wlan.Dot11Ssid ssid = wlanInterface.CurrentConnection.wlanAssociationAttributes.dot11Ssid;
                connectedSsids.Add(new String(Encoding.ASCII.GetChars(ssid.SSID, 0, (int)ssid.SSIDLength)));
            }

            foreach (var itme in connectedSsids)
            {
                Console.WriteLine(itme);
            }
        }

        private bool NetworkIsAvailable()
        {
            return NetworkIsAvailable(0);
        }

        private bool NetworkIsAvailable(long minimumSpeed)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // discard because of standard reasons
                if ((ni.OperationalStatus != OperationalStatus.Up) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel))
                    continue;

                // this allow to filter modems, serial, etc.
                // I use 10000000 as a minimum speed for most cases
                if (ni.Speed < minimumSpeed)
                    continue;

                // discard virtual cards (virtual box, virtual pc, etc.)
                if ((ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0))
                    continue;

                // discard "Microsoft Loopback Adapter", it will not show as NetworkInterfaceType.Loopback but as Ethernet Card.
                if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                    continue;

                return true;
            }
            return false;
        }

        private void setSubnetMask()
        {
            try
            {
                string SubnetMask = null;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");

                foreach (ManagementObject mo in mc.Get())
                {
                    string[] subnets = (string[])mo["IPSubnet"];
                    SubnetMask = subnets[0];
                }
                Machine.MACAddress = SubnetMask;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void setMachineName()
        {
            try
            {
                Machine.HostName = Environment.MachineName;
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        private void setOSArchitecture()
        {
            try
            {
                string OSArchitecture = null;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

                foreach (ManagementObject mo in mc.Get())
                {
                    OSArchitecture = (string)mo["OSArchitecture"];
                }
                Machine.OSArchitecture = OSArchitecture;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void setOperatingSystem()
        {
            try
            {
                string OperatingSystem = null;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

                foreach (ManagementObject mo in mc.Get())
                {
                    OperatingSystem = (string)mo["Caption"];
                }
                Machine.OperatingSystem = OperatingSystem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void setOSManufacturer()
        {
            try
            {
                string Manufacturer = null;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

                foreach (ManagementObject mo in mc.Get())
                {
                    Manufacturer = (string)mo["Manufacturer"];
                }
                Machine.OSManufacturer = Manufacturer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
