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
    /// <summary>
	/// Stores all the information required of the network that the user is currently connected to.
	/// </summary>
	/// <remarks>
	/// This class is instantiated when the app is first run (before the GUI is initialized)
	/// </remarks>
    public class Network : INotifyPropertyChanged
    {
        private const string WIFI = "WIFI";
        private const string ETHERNET_ONLY = "ETHERNET ONLY";
        private const string INACTIVE = "INACTIVE";
        private const string ACTIVE = "ACTIVE";
        private const string EMPTY = "-";

        private const bool TRUE = true;
        private const bool FALSE = false;

        public event PropertyChangedEventHandler PropertyChanged;
        private object _lock = new object();

        private string _DefaultGateway = EMPTY;
        private string _ExternalIP = EMPTY;
        private string _SSID = EMPTY;
        private string _NetworkConnection = EMPTY;
        private string _NetworkConnectionType = EMPTY;
        private string _InternetConnection = EMPTY;

        public Network()
        {
            // initial check of network.
            CheckNetworkAvailability();

            NetworkChange.NetworkAddressChanged += NetworkAddressChanged;
        }

        private void NetworkIsActive()
        {
            this.DefaultGateway = getDefaultGateway();
            this.SSID = getSSID();
            this.NetworkConnectionType = checkSSID();
            this.NetworkConnection = ACTIVE;

            if (IsInternetAvailable())
            {
                this.ExternalIP = getExternalIP();
                this.InternetConnection = ACTIVE;
            }
            else
            {
                this.ExternalIP = EMPTY;
                this.InternetConnection = INACTIVE;
            }
        }

        private void NetworkIsInactive()
        {
            this.ExternalIP = EMPTY;
            this.DefaultGateway = EMPTY;
            this.SSID = EMPTY;
            this.NetworkConnectionType = EMPTY;

            this.NetworkConnection = INACTIVE;
            this.InternetConnection = INACTIVE;
        }

        private void CheckNetworkAvailability()
        {
            switch (IsNetworkAvailable(0))
            {
                case TRUE:
                    NetworkIsActive();
                    break;
                case FALSE:
                    NetworkIsInactive();
                    break;
            }
        }

        /// <summary>
        /// These are the methods that get the appropriate values via different resources
        /// </summary>
        /// <returns>
        /// Adequate values. Either a FOUND value if found (i.e. DefaultGateway)...
        /// ...or EMPTY, which will infer the required value cannot be found or, in this case, the device may not be connected to a Network
        /// </returns>
        private string checkSSID()
        {
            return this._SSID != EMPTY ? WIFI : ETHERNET_ONLY;
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

        private string getDefaultGateway()
        {
            try
            {
                string DefaultGateway = EMPTY;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");

                foreach (ManagementObject mo in mc.Get())
                {
                    string[] gateways = (string[])mo["DefaultIPGateway"];
                    DefaultGateway = gateways[0];
                    break;
                }
                return DefaultGateway;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        private bool IsInternetAvailable()
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

        private bool IsNetworkAvailable(long minimumSpeed)
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

        // Called if the device detects a change in the network status
        private void NetworkAddressChanged(object sender, EventArgs e)
        {
            CheckNetworkAvailability();
        }

        /// <summary>
        /// Getter/setter methods that will (hopfully) update the property when changed
        /// </summary>
        public string DefaultGateway
        {
            get { return this._DefaultGateway; }
            set { setter(value, "DefaultGateway", ref this._DefaultGateway); }
        }

        public string ExternalIP
        {
            get { return this._ExternalIP; }
            set { setter(value, "ExternalIP", ref this._ExternalIP); }
        }

        public string SSID
        {
            get { return this._SSID; }
            set { setter(value, "SSID", ref this._SSID); }
        }

        public string NetworkConnection
        {
            get { return this._NetworkConnection; }
            set { setter(value, "NetworkConnection", ref this._NetworkConnection); }
        }

        public string NetworkConnectionType
        {
            get { return this._NetworkConnectionType; }
            set { setter(value, "NetworkConnectionType", ref this._NetworkConnectionType); }
        }

        public string InternetConnection
        {
            get { return this._InternetConnection; }
            set { setter(value, "InternetConnection", ref this._InternetConnection); }
        }

        private void setter(string val, string propertyName, ref string propertyVal)
        {
            lock (_lock)
            {
                if (val != propertyVal)
                {
                    propertyVal = val;
                    NotifyPropertyChanged(propertyName);
                }
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
