using System;
using System.Net;
using System.Net.NetworkInformation;

namespace NetDetector
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var parser = new SimpleCli.Parser(args, version: "0.0.2", maxWidth: 65);
                parser.Add("addr", "The IP or MAC address to detect.");
                parser.Add("connect", 
                           'c', 
                           "The command to run when the MAC address is detected on the network.", 
                           "",
                           SimpleCli.Arg.Type.SINGLE);
                parser.Add("disconnect", 
                           'd', 
                           "The command to run when the MAC address is no longer detected on the network.", 
                           "",
                           SimpleCli.Arg.Type.SINGLE);
                parser.Add("rate", 
                           'r', 
                           "The poll rate in seconds.", 
                           "60",
                           SimpleCli.Arg.Type.SINGLE);
                parser.Add("connect-count", 
                           'C', 
                           "The number of consecutive connection detections that must elapse before the connect command is run.", 
                           "1",
                           SimpleCli.Arg.Type.SINGLE);
                parser.Add("disconnect-count", 
                           'D', 
                           "The number of consecutive disconnection detections that must elapse before the disconnect command is run.", 
                           "1",
                           SimpleCli.Arg.Type.SINGLE);
                parser.Add("interface", 
                           'i', 
                           "The network interface to use as the scope identifier.", 
                           "1",
                           SimpleCli.Arg.Type.SINGLE);
                parser.Parse();

                var connect = parser["connect"].GetString();
                var disconnect = parser["disconnect"].GetString();
                var rate = parser["rate"].GetInt();
                var connectCount = parser["connect-count"].GetInt();
                var disconnectCount = parser["disconnect-count"].GetInt();
                var iface = parser["interface"].GetString();
                var addr = parser["addr"].Get<IPAddress>((arg) => {return Util.ParseIp(arg, iface);});

                var detector = new Detector(addr,
                                            connect, 
                                            disconnect, 
                                            rate, 
                                            connectCount, 
                                            disconnectCount);
                detector.Start();

                return 0;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
        }
    }
}
