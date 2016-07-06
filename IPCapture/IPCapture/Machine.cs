using System;
using System.Management;
using System.Net;
using System.Net.Sockets;

namespace IPCapture
{
    public class Machine
    {
        public string IPv4 { get; set; }
        public string IPv6 { get; set; }
        public string MACAddress { get; set; }
        public string MachineName { get; set; }
        public string SubnetMask { get; set; }
        public string OperatingSystem { get; set; }
        public string OSArchitecture { get; set; }
        public string OSManufacturer { get; set; }

        public Machine()
        {
            this.IPv4 = getIPv4();
            this.IPv6 = getIPv6();
            this.MACAddress = getMACAddress();
            this.MachineName = getMachineName();
            this.SubnetMask = getSubnetMask();
            this.OperatingSystem = getOperatingSystem();
            this.OSArchitecture = getOSArchitecture();
            this.OSManufacturer = getOSManufacturer();
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
            }
            catch (Exception ex)
            {
                throw ex;
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getMACAddress()
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
                return MACAddress;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getSubnetMask()
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
                return SubnetMask;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getMachineName()
        {
            try
            {
                return Environment.MachineName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getOSArchitecture()
        {
            try
            {
                string OSArchitecture = null;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

                foreach (ManagementObject mo in mc.Get())
                {
                    OSArchitecture = (string)mo["OSArchitecture"];
                }
                return OSArchitecture;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getOperatingSystem()
        {
            try
            {
                string OperatingSystem = null;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

                foreach (ManagementObject mo in mc.Get())
                {
                    OperatingSystem = (string)mo["Caption"];
                }
                return OperatingSystem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getOSManufacturer()
        {
            try
            {
                string Manufacturer = null;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

                foreach (ManagementObject mo in mc.Get())
                {
                    Manufacturer = (string)mo["Manufacturer"];
                }
                return Manufacturer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
