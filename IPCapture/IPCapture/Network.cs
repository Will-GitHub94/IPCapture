using System;
using System.Management;
using System.Net;
using NativeWifi;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace IPCapture
{
    public class Network
    {
        private const string WIFI = "WIFI";
        private const string ETHERNET_ONLY = "ETHERNET ONLY";
        private const string INACTIVE = "INACTIVE";
        private const string ACTIVE = "ACTIVE";
        private const string EMPTY = "-";

        private const bool TRUE = true;
        private const bool FALSE = false;

        public string DefaultGateway { get; set; }
        public string ExternalIP { get; set; }
        public string SSID { get; set; }
        public string NetworkConnection { get; set; }
        public string NetworkConnectionType { get; set; }
        public string InternetConnection { get; set; }

        public Network()
        {
            switch (NetworkIsAvailable())
            {
                case TRUE:
                    //this.DefaultGateway = getDefaultGateway();
                    this.SSID = getSSID();
                    this.NetworkConnectionType = checkSSID();
                    this.NetworkConnection = ACTIVE;

                    if (InternetIsAvailable())
                    {
                        this.ExternalIP = getExternalIP();
                        this.InternetConnection = ACTIVE;
                    }
                    else
                    {
                        this.ExternalIP = EMPTY;
                        this.InternetConnection = INACTIVE;
                    }
                    break;
                case FALSE:
                    this.ExternalIP = EMPTY;
                    this.DefaultGateway = EMPTY;
                    this.SSID = EMPTY;
                    this.NetworkConnectionType = EMPTY;

                    this.NetworkConnection = INACTIVE;
                    this.InternetConnection = INACTIVE;
                    break;
            }
        }

        private string checkSSID()
        {
            return this.SSID != EMPTY ? WIFI : ETHERNET_ONLY;
        }

        private string getExternalIP()
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

        //private string getDefaultGateway()
        //{
        //    try
        //    {
        //        string DefaultGateway = EMPTY;
        //        ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");

        //        foreach (ManagementObject mo in mc.Get())
        //        {
        //            string[] gateways = (string[])mo["DefaultIPGateway"];
        //            DefaultGateway = gateways[0];
        //            break;
        //        }
        //        return DefaultGateway;
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex;
        //    }
        //}

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
                return EMPTY;
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
