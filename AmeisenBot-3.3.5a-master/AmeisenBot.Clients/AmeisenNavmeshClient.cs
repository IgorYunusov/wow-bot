using AmeisenBotUtilities;
using AmeisenBotUtilities.Structs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AmeisenBot.Clients
{
    public class AmeisenNavmeshClient
    {
        public AmeisenNavmeshClient(string ip, int port)
        {
            Ip = ip;
            Port = port;

            TcpClient = new TcpClient();
            try
            {
                TcpClient.Connect(IPAddress.Parse(ip), port);
            }
            catch { }
        }

        public List<Vector3> RequestPath(PathRequest pathRequest)
        {
            List<Vector3> path = new List<Vector3>();

            if (!TcpClient.Connected)
            {
                try { TcpClient.Connect(Ip, Port); } catch { return null; }
                if (!TcpClient.Connected)
                {
                    return null;
                }
            }

            StreamReader sReader = new StreamReader(TcpClient.GetStream(), Encoding.ASCII);
            StreamWriter sWriter = new StreamWriter(TcpClient.GetStream(), Encoding.ASCII);

            bool isConnected = true;
            string pathJson = "";

            while (isConnected)
            {
                sWriter.WriteLine(JsonConvert.SerializeObject(pathRequest) + " &gt;");
                sWriter.Flush();

                pathJson = sReader.ReadLine().Replace("&gt;", "");
                path = JsonConvert.DeserializeObject<List<Vector3>>(pathJson);
                return path;
            }

            return path;
        }

        ~AmeisenNavmeshClient()
        {
            TcpClient.Close();
            TcpClient.Dispose();
        }

        private string Ip { get; set; }
        private int Port { get; set; }
        private TcpClient TcpClient { get; set; }
    }
}