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

namespace IPCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IPv4_val.Content = getIPv4();
            IPv6_val.Content = getIPv6();
            ExternalIP_val.Content = getExternalIP();
            MAC_val.Content = getMACAddress();
            DefaultGateway_val.Content = getDefaultGateway();
        }

        private string getIPv4()
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
                return IPv4;
            } catch
            {
                return null;
            }
        }

        private string getIPv6()
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
                return IPv6;
            } catch
            {
                return null;
            }
        }

        private string getExternalIP()
        {
            try
            {
                string externalIP = null;
                externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                             .Matches(externalIP)[0].ToString();
                return externalIP;
            }
            catch {
                return null;
            }
        }

        private string getMACAddress()
        {
            try
            {
                string MACAddress = null;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");

                foreach (var mo in mc.Get())
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        MACAddress = mo["MACAddress"].ToString();
                    }
                }
                return MACAddress;
            } catch
            {
                return null;
            }
        }

        public string getDefaultGateway()
        {
            try
            {
                string DefaultGateway = "";
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in adapters)
                {
                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    GatewayIPAddressInformationCollection addresses = adapterProperties.GatewayAddresses;
                    if (addresses.Count > 0)
                    {
                        foreach (GatewayIPAddressInformation address in addresses)
                        {
                            DefaultGateway = address.Address.ToString();
                        }
                    }
                }
                return DefaultGateway;
            } catch
            {
                return null;
            }
        }
    }
}
