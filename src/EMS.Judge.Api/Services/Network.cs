﻿using EnduranceJudge.Core.ConventionalServices;
using System;
using System.Net;
using System.Net.Sockets;

namespace Endurance.Judge.Gateways.API.Services
{
    public class Network : INetwork
    {
        public string GetIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }

    public interface INetwork : ITransientService
    {
        string GetIpAddress();
    }
}
