using System;
using System.Text;
using System.Management;
using System.Net;
using NativeWifi;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace IPCapture
{
    public class NetworkActivities
    {
        private const string WIFI = "WIFI";
        private const string ETHERNET_ONLY = "ETHERNET ONLY";
        private const string EMPTY = "-";
        private const bool TRUE = true;
        private const bool FALSE = false;

        public NetworkActivities()
        {}

        /// <summary>
        /// These are the methods that get the appropriate values via different resources
        /// </summary>
        /// <returns>
        /// Adequate values. Either a FOUND value if found (i.e. DefaultGateway)...
        /// ...or EMPTY, which will infer the required value cannot be found or, in this case, the device may not be connected to a Network
        /// </returns>
        public string checkSSID(string SSID)
        {
            try
            {
                return SSID != EMPTY ? WIFI : ETHERNET_ONLY;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getExternalIP()
        {
            try
            {
                return new WebClient().DownloadString("https://api.ipify.org");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getDefaultGateway()
        {
            try
            {
                string DefaultGateway = EMPTY;

                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in adapters)
                {
                    if (DefaultGateway != EMPTY)
                        break;

                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    GatewayIPAddressInformationCollection addresses = adapterProperties.GatewayAddresses;
                    if (addresses.Count > 0)
                    {
                        foreach (GatewayIPAddressInformation address in addresses)
                            DefaultGateway = address.Address.ToString();
                    }
                }
                return DefaultGateway;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public bool IsInternetAvailable()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return TRUE;
                }
            }
            catch
            {
                return FALSE;
            }
        }

        public string getSSID()
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
                return EMPTY;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public bool IsNetworkAvailable(long minimumSpeed)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return FALSE;

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

                return TRUE;
            }
            return FALSE;
        }
    }
}
