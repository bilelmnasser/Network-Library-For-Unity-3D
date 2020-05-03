
using ConnectedGames.Utilities;
using ConnectedGames.Server;

using Mono.Nat;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using ConnectedGames.Client;

namespace ConnectedGames
{

    class Program
    {
     

       
        static void Main(string[] args)
        {

            // if Server 

            ConnectedGames.Server.Server server = new ConnectedGames.Server.Server(150);
              server.Start();

          /*
           *
           * ConnectedGames.Client.Client cl = new Client.Client();
          Console.WriteLine("Connect to server Return with " + cl.Connect(IPAddress.Parse("127.0.0.1")));

              string g = "";      


              do
              {
               cl.Ping();
                
                

                  
                  g = Console.ReadLine();
                  if (g != string.Empty)
                  {
                      for (int k = 0; k < 600; k++)
                      {
                          byte[] data = Encoding.UTF8.GetBytes(g);
                          NetworkPacket np = new NetworkPacket((uint)data.Length);
                          np.Packet_Data = data;
                          cl.UDP_Send(np);
                       // cl.TCP_Send(np);
                      //  Console.WriteLine();
                    }

                }



              } while (g != string.Empty);
              
            
            
            */






            Console.ReadLine();
        }



    }
}
