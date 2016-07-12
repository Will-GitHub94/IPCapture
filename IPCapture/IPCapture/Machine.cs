using System.ComponentModel;

namespace IPCapture
{
    /// <summary>
    /// Stores all the information required of the device the app is running on.
    /// </summary>
    /// <remarks>
    /// This class is instantiated when the app is first run (before the GUI is initialized)
    /// </remarks>
    public class Machine : INotifyPropertyChanged
    {
        // Classes
        private static MachineActivities MachineActivities;

        // Constants
        private const string EMPTY = "-";

        // Member Variables
        private string _IPv4 = EMPTY;
        private string _IPv6 = EMPTY;
        private string _MACAddress = EMPTY;
        private string _SubnetMask = EMPTY;
        private string _MachineName = EMPTY;
        private string _OperatingSystem = EMPTY;
        private string _OSArchitecture = EMPTY;
        private string _OSManufacturer = EMPTY;

        // Events
        public event PropertyChangedEventHandler PropertyChanged;
        private object _lock = new object();

        public Machine()
        {
            MachineActivities = new MachineActivities();

            this.IPv4 = MachineActivities.getIPv4();
            this.IPv6 = MachineActivities.getIPv6();
            this.MACAddress = MachineActivities.getMACAddress();
            this.SubnetMask = MachineActivities.getSubnetMask();
            this.MachineName = MachineActivities.getMachineName();
            this.OperatingSystem = MachineActivities.getOperatingSystem();
            this.OSArchitecture = MachineActivities.getOSArchitecture();
            this.OSManufacturer = MachineActivities.getOSManufacturer();
        }

        public string SubnetMask
        {
            get { return this._SubnetMask; }
            set { setter(value, "SubnetMask", ref this._SubnetMask); }
        }

        public string MACAddress
        {
            get { return this._MACAddress; }
            set { setter(value, "MACAddress", ref this._MACAddress); }
        }

        public string IPv6
        {
            get { return this._IPv6; }
            set { setter(value, "IPv6", ref this._IPv6); }
        }

        public string IPv4
        {
            get { return this._IPv4; }
            set { setter(value, "IPv4", ref this._IPv4); }
        }

        public string MachineName
        {
            get { return this._MachineName; }
            set { setter(value, "MachineName", ref this._MachineName); }
        }

        public string OperatingSystem
        {
            get { return this._OperatingSystem; }
            set { setter(value, "OperatingSystem", ref this._OperatingSystem); }
        }

        public string OSArchitecture
        {
            get { return this._OSArchitecture; }
            set { setter(value, "OSArchitecture", ref this._OSArchitecture); }
        }

        public string OSManufacturer
        {
            get { return this._OSManufacturer; }
            set { setter(value, "OSManufacturer", ref this._OSManufacturer); }
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
