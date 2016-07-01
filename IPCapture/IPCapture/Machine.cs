using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCapture
{
    public class Machine
    {
        public string IPv4 { get; set; }
        public string IPv6 { get; set; }
        public string MACAddress { get; set; }
        public string HostName { get; set; }
        public string SubnetMask { get; set; }
        public string OperatingSystem { get; set; }
        public string OSArchitecture { get; set; }
        public string OSManufacturer { get; set; }

        public Machine() { }
    }
}
