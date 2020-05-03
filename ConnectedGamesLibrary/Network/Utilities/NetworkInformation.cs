using ConnectedGamesLibrary.Library;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ConnectedGames.Utilities
{
    public class NetworkInformation
    {

        internal IPAddress Local_IP
        {
            set { }

            get
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        Console.WriteLine(ip.ToString());

                        return ip;
                    }
                }
                throw new Exception("No network adapters with an IPv4 address in the system!");


            }

        }

        public IPAddress Public_IP;

        public IPAddress Server_IP;


        public int Server_Port = 1000;
        public int Client_Port = 1002;


    }
}
