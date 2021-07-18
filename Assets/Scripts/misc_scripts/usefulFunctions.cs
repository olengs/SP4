using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

namespace usefulFunctions
{
    namespace JCFunctions
    {
        public class JCFunctions
        {
            public static string LocalIPAddress()
            {
                IPHostEntry host;
                string localIP = "0.0.0.0";
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
                return localIP;
            }
        }

        public class IPManager
        {
            public static string GetIP(ADDRESSFAM Addfam)
            {
                //Return null if ADDRESSFAM is Ipv6 but Os does not support it
                if (Addfam == ADDRESSFAM.IPv6 && !Socket.OSSupportsIPv6)
                {
                    return null;
                }

                string output = "0.0.0.0";

                foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                {
                    #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                    NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
                    NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;

                    if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
                    #endif
                    {
                        foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                        {
                            //IPv4
                            if (Addfam == ADDRESSFAM.IPv4)
                            {
                                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                                {
                                    if (ip.Address.ToString() != "127.0.0.1")
                                        output = ip.Address.ToString();
                                }
                            }

                            //IPv6
                            else if (Addfam == ADDRESSFAM.IPv6)
                            {
                                if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                                {
                                    if (ip.Address.ToString() != "127.0.0.1")
                                        output = ip.Address.ToString();
                                }
                            }
                        }
                    }
                }
                return output;
            }
        }

        public enum ADDRESSFAM
        {
            IPv4, IPv6
        }
    }
    //add ur own namespace if you wan
}
