using System;
using System.Text;
using System.Net;
using NativeWifi;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics;
using System.Threading;

namespace NetworkCapture
{
    public class NetworkActivities
    {
        private ExceptionHandling ExceptionHandling;

        private const string WIFI = "WIFI";
        private const string ETHERNET = "ETHERNET";
        private const string WIFI_AND_ETHERNET = "WIFI & ETHERNET";
        private const string EMPTY = "-";
        private const string NOTHING = "";
        private const bool TRUE = true;
        private const bool FALSE = false;

        public NetworkActivities()
        {
            ExceptionHandling = new ExceptionHandling();
        }

        /// <summary>
        /// These are the methods that get the appropriate values via different resources
        /// </summary>
        /// <returns>
        /// Adequate values. Either a FOUND value if found (i.e. DefaultGateway)...
        /// ...or EMPTY, which will infer the required value cannot be found or, in this case, the device may not be connected to a Network
        /// </returns>
        ///

        public string getExternalIP()
        {
            try
            {
                return new WebClient().DownloadString("https://api.ipify.org");
            }
            catch (Exception ex)
            {
                return ExceptionHandling.getExceptionMessage(ex);
            }
        }

        public string getNetworkConnectionType()
        {
            try
            {
                bool WLAN_STATE = false;
                bool LAN_STATE = false;

                var WLAN_Process = new Process
                {
                    StartInfo =
                      {
                          FileName = "netsh.exe",
                          Arguments = "wlan show interfaces ",
                          UseShellExecute = false,
                          RedirectStandardOutput = true,
                          CreateNoWindow = true
                      }
                };
                WLAN_Process.Start();
                var WLAN_Output = WLAN_Process.StandardOutput.ReadToEnd();

                var LAN_Process = new Process
                {
                    StartInfo =
                      {
                          FileName = "netsh.exe",
                          Arguments = "interface show interface name=\"Ethernet\" ",
                          UseShellExecute = false,
                          RedirectStandardOutput = true,
                          CreateNoWindow = true
                      }
                };
                LAN_Process.Start();
                var LAN_Output = LAN_Process.StandardOutput.ReadToEnd();

                var LAN_State = LAN_Output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(l => l.Contains("Connect state"));
                var WLAN_State = WLAN_Output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(l => l.Contains("State"));

                if (WLAN_State != null)
                    WLAN_STATE = !WLAN_State.Contains("disconnected");
                
                if (LAN_State != null)
                    LAN_STATE = !LAN_State.Contains("Disconnected");

                if (WLAN_STATE && !LAN_STATE)
                    return WIFI;
                else if (!WLAN_STATE && LAN_STATE)
                    return ETHERNET;
                else if (WLAN_STATE && LAN_STATE)
                    return WIFI_AND_ETHERNET;
                else
                    return EMPTY;

            } catch (Exception ex)
            {
                return ExceptionHandling.getExceptionMessage(ex);
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
            catch (Exception ex)
            {
                return ExceptionHandling.getExceptionMessage(ex);
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

        public string[] getSSID()
        {
            string[] wirelessInfo = new string[2];
            wirelessInfo[0] = EMPTY;
            wirelessInfo[1] = EMPTY;

            try
            {
                WlanClient wlan = new WlanClient();
                Collection<String> ConnectedSSIDs = new Collection<string>();

                foreach (WlanClient.WlanInterface wlanInterface in wlan.Interfaces)
                {
                    wirelessInfo[0] = wlanInterface.InterfaceDescription;
                    Wlan.Dot11Ssid ssid = wlanInterface.CurrentConnection.wlanAssociationAttributes.dot11Ssid;
                    ConnectedSSIDs.Add(new String(Encoding.ASCII.GetChars(ssid.SSID, 0, (int)ssid.SSIDLength)));
                }
                wirelessInfo[1] = ConnectedSSIDs[0];
            }
            catch (Exception ex)
            {
                wirelessInfo[1] = ExceptionHandling.getExceptionMessage(ex);
            }
            return wirelessInfo;
        }

        public string getDownloadSpeed(string adapterDescription)
        {
            try
            {
                string DownloadSpeed = EMPTY;

                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in adapters)
                {
                    if (adapter.Description == adapterDescription)
                    {
                        DownloadSpeed = ((adapter.GetIPv4Statistics().BytesReceived / 1024f) / 1024f).ToString("n2");
                        Thread.Sleep(1000);
                    }
                }
                return DownloadSpeed;
            } catch (Exception ex)
            {
                return ExceptionHandling.getExceptionMessage(ex);
            }
        }

        public string[] getDNSSuffix()  
        {
            string[] wiredInfo = new string[2];
            wiredInfo[0] = EMPTY;
            wiredInfo[1] = EMPTY;

            try
            {
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in adapters)
                {
                    if (wiredInfo[1] != EMPTY)
                        break;

                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    if (properties.DnsSuffix != NOTHING)
                    {
                        wiredInfo[0] = adapter.Description;
                        wiredInfo[1] = properties.DnsSuffix;
                    }
                }
            } catch (Exception ex)
            {
                wiredInfo[1] = ExceptionHandling.getExceptionMessage(ex);
            }
            return wiredInfo;
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

        public string getISP(string externalIP)
        {
            try
            {
                string whoIsMyISP = new WebClient().DownloadString("https://www.whoismyisp.org/ip/" + externalIP);
                int start = whoIsMyISP.IndexOf("<h1>") + "<h1>".Length;
                int end = whoIsMyISP.IndexOf("</h1>");

                return whoIsMyISP.Substring(start, end - start);
            }
            catch (Exception ex)
            {
                return ExceptionHandling.getExceptionMessage(ex);
            }
        }
    }
}
