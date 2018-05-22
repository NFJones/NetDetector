using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Diagnostics;
using System.Collections;

namespace NetDetector
{
    internal class Detector
    {
        public Detector(IPAddress address,
                        string[] connectCmd,
                        string[] disconnectCmd,
                        int pollRate,
                        int connectCount,
                        int disconnectCount)
        {
            this.address = address;
            this.connectCmd = connectCmd;
            this.disconnectCmd = disconnectCmd;
            this.pollRate = pollRate;
            this.connectCount = connectCount;
            this.disconnectCount = disconnectCount;
            connected = false;
            running = false;
            firstRun = true;
        }

        public void Detect()
        {
            Log($"Watching");

            while (running)
            {
                try
                {
                    var ping = new Ping();
                    var response = ping.Send(address);
                    if (response.Status == IPStatus.Success)
                    {
                        curConnectCount++;
                        curDisconnectCount = 0;

                        if ((!connected || firstRun) && curConnectCount >= connectCount)
                        {
                            firstRun = false;
                            connected = true;
                            Log($"Detected");
                            if (connectCmd.Length > 0)
                            {
                                Log($"Running: {ArgsToString(connectCmd)}");
                                RunProcess(connectCmd);
                            }
                        }
                    }
                    else
                    {
                        curDisconnectCount++;
                        curConnectCount = 0;

                        if ((connected || firstRun) && curDisconnectCount >= disconnectCount)
                        {
                            firstRun = false;
                            connected = false;
                            Log($"Not detected");
                            if (disconnectCmd.Length > 0)
                            {
                                Log($"Running: {ArgsToString(disconnectCmd)}");
                                RunProcess(disconnectCmd);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log(e.Message);
                }

                System.Threading.Thread.Sleep(1000 * pollRate);
            }
        }

        private static string ArgsToString(string[] args)
        {
            StringBuilder buffer = new StringBuilder();
            if (args.Length > 0)
            {
                foreach (var v in args)
                    buffer.Append($"{v.ToString()} ");
                buffer.Length -= 1;
            }

            return buffer.ToString();
        }

        private void RunProcess(string[] cmd)
        {
            try
            {
                var programName = cmd[0];

                ArrayList args = new ArrayList();

                for (var i = 1; i < cmd.Length; i++)
                    args.Add(cmd[i]);

                var p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.FileName = programName;
                p.StartInfo.Arguments = ArgsToString((string[])args.ToArray(typeof(string)));
                p.StartInfo.Environment.Add("NET_DETECTOR_ADDRESS", address.ToString());
                p.Start();
            }
            catch (Exception e)
            {
                Log($"Failed to run: {ArgsToString(cmd)} ({e.Message})");
            }
        }

        private void Log(string message)
        {
            Util.LogMessage($"({address}) - {message}");
        }

        public void Start()
        {
            running = true;
            Detect();
        }

        public void Stop()
        {
            running = false;
        }

        private IPAddress address { get; set; }
        private string[] connectCmd { get; set; }
        private string[] disconnectCmd { get; set; }
        private int pollRate { get; set; }
        private int connectCount { get; set; }
        private int disconnectCount { get; set; }
        private bool connected { get; set; }
        private bool running { get; set; }
        private bool firstRun { get; set; }
        private int curConnectCount { get; set; }
        private int curDisconnectCount { get; set; }
    }
}