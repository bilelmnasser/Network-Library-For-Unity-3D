using ConnectedGames.Library;
using ConnectedGamesLibrary.Library;
using Mono.Nat;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace ConnectedGames
{

    class Program
    {
     

       
        static void Main(string[] args)
        {

            // if Server 

            //      Server server = new Server();
            //      server.Start_UDP_Server();





            Client cl = new Client();
            string g = "";      
          //  Router r = new Router(cl);
          
            
            do
            {
                using (Ping p = new Ping())
                {
                    PingReply reply = p.Send(IPAddress.Parse("192.168.1.92"));
                    Console.WriteLine(reply.Status + " : " + reply.RoundtripTime + " ms");


                }
                g = Console.ReadLine();
                if (g != string.Empty)
                {
                    for (int k = 0; k < 150; k++)
                    {
                        byte[] data = Encoding.UTF8.GetBytes(g);
                        NetworkPacket np = new NetworkPacket((uint)data.Length);
                        np.Packet_Data = data;
                        cl.UDP_Send(np);
                    }

                }



            } while (g != string.Empty);










            Console.ReadLine();
        }



    }
}
