using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using NativeWifi;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.NetworkInformation;

namespace IPCapture
{
    public class Network
    {
        public string DefaultGateway { get; set; }
        public string ExternalIP { get; set; }
        public string SSID { get; set; }
        public string ConnectionType { get; set; }

        public const string WIFI = "WiFi";
        public const string ETHERNET = "Ethernet";
        public const string NO_NETWORK_AVAILABLE = "No network available!";

        public Network()
        {
            this.ExternalIP = getExternalIP();
            this.DefaultGateway = getDefaultGateway();

            if (NetworkIsAvailable())
            {
                this.SSID = getSSID();

                if (this.SSID != null)
                    this.ConnectionType = WIFI;
                else
                    this.ConnectionType = ETHERNET;
            }
            else
            {
                this.SSID = null;
                this.ConnectionType = NO_NETWORK_AVAILABLE;
            }
        }

        private string getExternalIP()
        {
            try
            {
                string ExternalIP = null;
                ExternalIP = (new System.Net.WebClient()).DownloadString("http://checkip.dyndns.org/");
                ExternalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                             .Matches(ExternalIP)[0].ToString();
                return ExternalIP;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getDefaultGateway()
        {
            try
            {
                string DefaultGateway = null;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");

                foreach (ManagementObject mo in mc.Get())
                {
                    string[] gateways = (string[])mo["DefaultIPGateway"];
                    DefaultGateway = gateways[0];
                }
                return DefaultGateway;
            }
            catch (Exception Ex)
            {
                throw Ex;
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

        private string getSSID()
        {
            try
            {
                WlanClient wlan = new WlanClient();

                Collection<String> ConnectedSSIDs = new Collection<string>();

                foreach (WlanClient.WlanInterface wlanInterface in wlan.Interfaces)
                {
                    Wlan.Dot11Ssid ssid = wlanInterface.CurrentConnection.wlanAssociationAttributes.dot11Ssid;
                    ConnectedSSIDs.Add(new String(Encoding.ASCII.GetChars(ssid.SSID, 0, (int)ssid.SSIDLength)));
                }
                return ConnectedSSIDs[0];
            }
            catch (Win32Exception)
            {
                return null;
            }
            catch (Exception Ex)
            {
                throw Ex;
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
    }
}
