using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace ConnectedGames.Library
{


    class Server : NetworkInformation
    {



        public TcpListener Tcp_Server;
        public UdpClient Udp_Server;




        public static HashSet<RequestProcessor> Clients_List;
        // public static HashSet<UDP_RequestProcessor> UDP_Clients_List;



        public Server()
        {
            Clients_List = new HashSet<RequestProcessor>();



        }


        #region TCP Server Side Implementation

        public void Start_TCP_Server()
        {
            Tcp_Server = new TcpListener(Local_IP, Server_TCP_Port);
            Console.WriteLine("Starting TCP Server at iP =" + Tcp_Server.LocalEndpoint.ToString());
            Tcp_Server.Start();
            Tcp_Server.BeginAcceptTcpClient(Begin_Accept_Tcp_Client_Callback, Tcp_Server);


        }

        private void Begin_Accept_Tcp_Client_Callback(IAsyncResult ar)
        {
            // End the operation and display the received data on 
            // the console.


            TcpClient client = Tcp_Server.EndAcceptTcpClient(ar);

            Console.WriteLine("New Client Connected" + client.Client.RemoteEndPoint.ToString());

            IPEndPoint DistanceCLientAddress = (IPEndPoint)client.Client.RemoteEndPoint;



            IEnumerable<RequestProcessor> query = Clients_List.Where(RequestProcessor => RequestProcessor.DistanceCLientAddress == DistanceCLientAddress);


            if (query.Count() == 0)
            {
                RequestProcessor rp = new RequestProcessor();
                rp.instance = this;
                rp.DistanceCLientAddress = DistanceCLientAddress;
                rp.client = client;
                rp.Proccess();
                Clients_List.Add(rp);
            }
            else
            {

                RequestProcessor rp = query.First();
                rp.instance = this;
                rp.client = client;
                rp.Proccess();

            }



            // Process the connection here. (Add the client to a
            // server table, read data, etc.)

            Console.WriteLine("Client connected completed");
            Tcp_Server.BeginAcceptTcpClient(Begin_Accept_Tcp_Client_Callback, Tcp_Server);

        }

        public static void Broadcast_TCP(NetworkPacket np, RequestProcessor tcp_RequestProcessor)
        {
            if (Clients_List != null)
                foreach (RequestProcessor rp in Clients_List)
                {
                    if (tcp_RequestProcessor != rp)
                        rp.Send_reliable_Message(np);

                }


        }


        #endregion

        #region UDP Server Side Implementation

        public void Start_UDP_Server()
        {
            Udp_Server = new UdpClient(new IPEndPoint(Local_IP, Server_UDP_Port));
            Console.WriteLine("Starting UDP Server at iP = " + Udp_Server.Client.LocalEndPoint.ToString());

            UdpState s = new UdpState();
            s.e = new IPEndPoint(Local_IP, Server_UDP_Port);
            s.u = Udp_Server;

            Udp_Server.BeginReceive(BeginReceive_UDP_Client_Callback, s);


        }

        private void BeginReceive_UDP_Client_Callback(IAsyncResult ar)
        {
            // End the operation and display the received data on 
            // the console.

            UdpClient u = ((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

            // UdpClient client =

            NetworkPacket NewP = new NetworkPacket();
            NewP.FromByteArray(Udp_Server.EndReceive(ar, ref e));
            // NewP.ToString();

            Console.Write("New Packet Received From " + e.ToString() + " ==> ");
            NewP.ToString();


            IEnumerable<RequestProcessor> query = Clients_List.Where(RequestProcessor => RequestProcessor.DistanceCLientAddress == e);



            if (query.Count() == 0)
            {
                RequestProcessor rp = new RequestProcessor();
                rp.DistanceCLientAddress = e;
                rp.instance = this;
                Clients_List.Add(rp);
            }
            else
            {

                /* RequestProcessor rp = query.First();
                 rp.instance = this;
                 rp.client = client;
                 rp.Proccess();
                 */

            }








            // Continue Reading Packets
            UdpState s = new UdpState();
            s.e = new IPEndPoint(Local_IP, Server_UDP_Port);
            s.u = Udp_Server;

            Udp_Server.BeginReceive(BeginReceive_UDP_Client_Callback, s);

        }

        public static void Broadcast_UDP(NetworkPacket np, RequestProcessor udp_RequestProcessor)
        {
            if (Clients_List != null)
                foreach (RequestProcessor rp in Clients_List)
                {
                    if (udp_RequestProcessor != rp)
                        rp.Send_unreliable_Message(np);

                }


        }

        #endregion

















    }
}
public struct UdpState
{
    public UdpClient u;
    public IPEndPoint e;
}