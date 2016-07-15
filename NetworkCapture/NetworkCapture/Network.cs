using System;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Threading;

namespace NetworkCapture
{
    /// <summary>
	/// Stores all the information required of the network that the user is currently connected to.
	/// </summary>
	/// <remarks>
	/// This class is instantiated when the app is first run (before the GUI is initialized)
	/// </remarks>
    public class Network : INotifyPropertyChanged
    {
        // Classes
        private static NetworkActivities NetworkActivities;

        // Constants
        private const string EMPTY = "-";
        private const bool TRUE = true;
        private const bool FALSE = false;
        private const string INACTIVE = "INACTIVE";
        private const string ACTIVE = "ACTIVE";
        private const string WIFI = "WIFI";
        private const string ETHERNET_ONLY = "ETHERNET ONLY";

        // Member variables
        private string _DefaultGateway = EMPTY;
        private string _ExternalIP = EMPTY;
        private string _SSID = EMPTY;
        private string _DNSSuffix = EMPTY;
        private string _NetworkConnection = EMPTY;
        private string _NetworkConnectionType = EMPTY;
        private string _InternetConnection = EMPTY;

        private Timer updateTimer;

        // Events
        public event PropertyChangedEventHandler PropertyChanged;
        private object _lock = new object();

        public Network()
        {
            NetworkActivities = new NetworkActivities();

            // initial check of network.
            this.CheckNetworkAvailability();

            NetworkChange.NetworkAddressChanged += NetworkAddressChanged;
        }

        private void CheckNetworkAvailability()
        {
            switch (NetworkActivities.IsNetworkAvailable(0))
            {
                case TRUE:
                    NetworkIsActive();
                    break;
                case FALSE:
                    NetworkIsInactive();
                    break;
            }
        }

        private void NetworkIsActive()
        {
            this.DefaultGateway = NetworkActivities.getDefaultGateway();
            this.SSID = NetworkActivities.getSSID();
            this.NetworkConnectionType = NetworkActivities.checkSSID(this.SSID);

            switch (this.NetworkConnectionType)
            {
                case ETHERNET_ONLY:
                    this.DNSSuffix = NetworkActivities.getDNSSuffix();
                    break;
                case WIFI:
                    /* do nothing */
                    break;
            }
            this.NetworkConnection = ACTIVE;

            try
            {
                updateTimer = new Timer(this.CheckInternetConnection, null, TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(-1));
            }
            catch (ArgumentOutOfRangeException Ex)
            {
                throw Ex;
            }
            catch (ArgumentNullException NEx)
            {
                throw NEx;
            }
            catch (Exception x)
            {
                throw x;
            }
        }

        private void CheckInternetConnection(object state)
        {
            switch (NetworkActivities.IsInternetAvailable())
            {
                case TRUE:
                    this.ExternalIP = NetworkActivities.getExternalIP();
                    this.InternetConnection = ACTIVE;
                    break;
                case FALSE:
                    this.ExternalIP = EMPTY;
                    this.InternetConnection = INACTIVE;
                    break;
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

        public string DNSSuffix
        {
            get { return this._DNSSuffix; }
            set { setter(value, "DNSSuffix", ref this._DNSSuffix); }
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
                    OnPropertyChanged(propertyName);
                }
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Called if the device detects a change in the network status
        private void NetworkAddressChanged(object sender, EventArgs e)
        {
            this.CheckNetworkAvailability();
        }
    }
}
