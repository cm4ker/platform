using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ZenPlatform.ServerClientShared.Network
{
    public static class NetworkUtility
    {
        public static IPEndPoint CreateIPEndPoint(string endPoint)
        {
            string[] ep = endPoint.Split(':');
            if (ep.Length < 2) throw new FormatException("Invalid endpoint format");
            IPAddress ip;
            if (ep.Length > 2)
            {
                if (!IPAddress.TryParse(string.Join(":", ep, 0, ep.Length - 1), out ip))
                {
                    throw new FormatException("Invalid ip-address");
                }
            }
            else
            {
                if (!IPAddress.TryParse(ep[0], out ip))
                {
                    throw new FormatException("Invalid ip-address");
                }
            }
            int port;
            if (!int.TryParse(ep[ep.Length - 1], System.Globalization.NumberStyles.None,
                System.Globalization.NumberFormatInfo.CurrentInfo, out port))
            {
                throw new FormatException("Invalid port");
            }
            return new IPEndPoint(ip, port);
        }
    }
}
