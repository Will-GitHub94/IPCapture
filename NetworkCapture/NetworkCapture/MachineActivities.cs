using System;
using System.Management;
using System.Net;
using System.Net.Sockets;

namespace NetworkCapture
{
    public class MachineActivities
    {
        private ExceptionHandling ExceptionHandling; 

        private const string EMPTY = "-";

        public MachineActivities()
        {
            ExceptionHandling = new ExceptionHandling();
        }

        public string getIPv4()
        {
            try
            {
                IPHostEntry IPhostEntry = Dns.GetHostEntry(Dns.GetHostName());
                string IPv4 = EMPTY;
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
                return ExceptionHandling.getExceptionMessage(ex);
            }
        }

        public string getIPv6()
        {
            try
            {
                string IPv6 = EMPTY;
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
                return ExceptionHandling.getExceptionMessage(ex);
            }
        }

        public string getMACAddress()
        {
            try
            {
                string MACAddress = EMPTY;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");

                foreach (ManagementObject mo in mc.Get())
                {
                    MACAddress = (string)mo["MACAddress"];
                    break;
                }
                return MACAddress;
            }
            catch (Exception ex)
            {
                return ExceptionHandling.getExceptionMessage(ex);
            }
        }

        public string getSubnetMask()
        {
            try
            {
                string SubnetMask = EMPTY;
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
                return ExceptionHandling.getExceptionMessage(ex);
            }
        }

        public string getMachineName()
        {
            try
            {
                return Environment.MachineName;
            }
            catch (Exception ex)
            {
                return ExceptionHandling.getExceptionMessage(ex);
            }
        }

        public string getOSArchitecture()
        {
            try
            {
                string OSArchitecture = EMPTY;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

                foreach (ManagementObject mo in mc.Get())
                {
                    OSArchitecture = (string)mo["OSArchitecture"];
                }
                return OSArchitecture;
            }
            catch (Exception ex)
            {
                return ExceptionHandling.getExceptionMessage(ex);
            }
        }

        public string getOperatingSystem()
        {
            try
            {
                string OperatingSystem = EMPTY;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

                foreach (ManagementObject mo in mc.Get())
                {
                    OperatingSystem = (string)mo["Caption"];
                }
                return OperatingSystem;
            }
            catch (Exception ex)
            {
                return ExceptionHandling.getExceptionMessage(ex);
            }
        }

        public string getOSManufacturer()
        {
            try
            {
                string Manufacturer = EMPTY;
                ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

                foreach (ManagementObject mo in mc.Get())
                {
                    Manufacturer = (string)mo["Manufacturer"];
                }
                return Manufacturer;
            }
            catch (Exception ex)
            {
                return ExceptionHandling.getExceptionMessage(ex);
            }
        }
    }
}
