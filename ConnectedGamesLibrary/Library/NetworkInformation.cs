using ConnectedGamesLibrary.Library;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ConnectedGames.Library
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
      /*  {
            set { }

            get
            {
                Router r = new Router();
                var k = r.Externel_IPAsync();
                return k.Result;
                
            }

        }
*/




        public IPAddress Server_IP;


        public int Server_TCP_Port = 1000;
        public int Server_UDP_Port = 1000;


        public int Client_Port = 1002;

      /*  public async Task ForwardingPortsForRouterAsync(string myGameName)
        {

            Router r = new Router();
            await r.Create_New_MapAsync(Client_Port, Client_Port, myGameName);



        }*/


    }
}
