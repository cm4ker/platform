using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace SqlPlusDbSync.Shared
{
    public class CommonHelper
    {
        public static string ServerChannelName = "server";
        public static string ServiceChannelName = "service";
        public static string InfoChannelName = "info";


        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }


}
