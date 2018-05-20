using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace NetDetector
{
    internal class Util
    {
        public static IPAddress MacToIPv6LinkLocal(PhysicalAddress mac)
        {
            var bytes = new byte[16];

            mac.GetAddressBytes().CopyTo(bytes, 10);
            for (var i = 8; i < 11; i++)
                bytes[i] = bytes[i + 2];
            bytes[11] = 0xff;
            bytes[12] = 0xfe;
            bytes[8] ^= 0x02;
            bytes[0] = 0xfe;
            bytes[1] = 0x80;

            return new IPAddress(bytes);
        }

        public static PhysicalAddress ParseMac(string mac)
        {
            mac = mac.Replace(':', '-');
            mac = mac.ToUpper();
            return PhysicalAddress.Parse(mac);
        }

        public static IPAddress ParseIp(string addr, string iface)
        {
            try
            {
                var mac = ParseMac(addr);
                var ret = MacToIPv6LinkLocal(mac);
                if (iface != "")
                    ret.ScopeId = GetInterfaceIndex(iface);
                return ret;
            }
            catch
            {
                var ret = IPAddress.Parse(addr);
                if (ret.IsIPv6LinkLocal && iface != "")
                    ret.ScopeId = GetInterfaceIndex(iface);
                return ret;
            }
        }

        public static int GetInterfaceIndex(string iface)
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            var properties = IPGlobalProperties.GetIPGlobalProperties();

            foreach (var i in interfaces)
            {
                if (i.Supports(NetworkInterfaceComponent.IPv4) == false ||
                    !i.Description.Equals(iface, StringComparison.OrdinalIgnoreCase))
                    continue;

                var p = i.GetIPProperties().GetIPv4Properties();

                if (p == null)
                    continue;

                return p.Index;
            }

            throw new Exception($"Could not determine the index of interface {iface}");
        }

        public static string MacToString(PhysicalAddress mac)
        {
            StringBuilder buffer = new StringBuilder();

            foreach (var b in mac.GetAddressBytes())
                buffer.Append($"{String.Format("{0:X2}", b)}:");

            buffer.Length--;
            return buffer.ToString();
        }

        public static void LogMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}