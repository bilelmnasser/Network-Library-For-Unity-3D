using ConnectedGames.Server;
using ConnectedGames.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace ConnectedGames.Server
{
    public delegate void OnMessageReceivedFromPlayer(PlayerNetworkCommunication player, NetworkPacket packet);
    public delegate void OnPlayerDisconnect(PlayerNetworkCommunication player);
    public delegate void OnPlayerConnect(PlayerNetworkCommunication player);


    class Server : NetworkInformation
    {

        public  static OnMessageReceivedFromPlayer _OnMessageReceivedFromPlayer;
        public static OnPlayerDisconnect _OnPlayerDisconnect;
        public static OnPlayerConnect _OnPlayerConnect;

        public int MaximumPlayersCount=150;

        public TcpListener Tcp_Server;
        public UdpClient Udp_Server;




        public static Dictionary<int,PlayerNetworkCommunication> Clients_List;



        public Server(int PlayerCount)
        {
            MaximumPlayersCount = PlayerCount;
            Clients_List = new  Dictionary<int, PlayerNetworkCommunication>(MaximumPlayersCount);

            for (int i = 0; i < MaximumPlayersCount; i++)
                Clients_List[i] = null;


           
        }

        public void Start()
        {
            Start_TCP_Server();
            Start_UDP_Server();

        }
        #region TCP Server Side Implementation

        private void Start_TCP_Server()
        {
            Tcp_Server = new TcpListener(IPAddress.Any, Server_Port);
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



           // IEnumerable<PlayerNetworkCommunication> query = Clients_List.Where(RequestProcessor => RequestProcessor.DistanceCLientAddress == DistanceCLientAddress);


          //  if (query.Count() == 0)
          //  {
          // Prepare new player to join him to Game Server
                PlayerNetworkCommunication rp = new PlayerNetworkCommunication();
                rp.instance = this;
                rp.CLientAddress = DistanceCLientAddress;
                rp.client = client;
                bool Serverisfull = true;
                for (int i = 0; i < MaximumPlayersCount; i++)
                {
                    if (Clients_List[i] == null)
                    {
                    rp.PlayerIdentifier = i;

                    Serverisfull = false;
                    Clients_List[i] = rp;
                    rp.Tcp_Proccess();
                    break;
                    }

                }
                if(Serverisfull)
                {
                    rp.Disconnect();

                }
         

            // Process the connection here. (Add the client to a
            // server table, read data, etc.)

            Tcp_Server.BeginAcceptTcpClient(Begin_Accept_Tcp_Client_Callback, Tcp_Server);

        }

        public static void Broadcast_TCP(NetworkPacket np, PlayerNetworkCommunication RequestProcessor)
        {
            if (Clients_List != null)
                foreach (PlayerNetworkCommunication rp in Clients_List.Values)
                {
                    if (RequestProcessor != rp && rp!=null)
                        rp.Send_reliable_Message(np);

                }


        }


        #endregion

        #region UDP Server Side Implementation

        private void Start_UDP_Server()
        {
            Udp_Server = new UdpClient(new IPEndPoint(IPAddress.Any, Server_Port));
            Console.WriteLine("Starting UDP Server at iP = " + Udp_Server.Client.LocalEndPoint.ToString());

            UdpState s = new UdpState();
            s.e = new IPEndPoint(Local_IP, Server_Port);
            s.u = Udp_Server;

            Udp_Server.BeginReceive(BeginReceive_UDP_Client_Callback, s);


        }

        private void BeginReceive_UDP_Client_Callback(IAsyncResult ar)
        {
            // End the operation and display the received data on 
            // the console.

            UdpClient u = ((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

           
            NetworkPacket NewP = new NetworkPacket();
            NewP.FromByteArray(Udp_Server.EndReceive(ar, ref e));

            foreach (PlayerNetworkCommunication rp in Clients_List.Values)
            {
                if( rp!=null&&rp.CLientAddress.Address.ToString()==e.Address.ToString()&& rp.CLientAddress.Port == e.Port)
                {


                    if (Server._OnMessageReceivedFromPlayer != null)
                    {
                        Server._OnMessageReceivedFromPlayer.Invoke(rp, NewP);
                    }
                    Console.WriteLine("New Packet Received From " + e.ToString() + " ==> " + NewP.ToString());
                    Server.Broadcast_TCP(NewP, rp);

                   rp.CheckLostnotReceivedPacket(NewP.Packet_Id);

                    Console.WriteLine(" Packet Loss Percentage : " + rp.PacketLossPercentage() + " % ");

                }
            }





            // Continue Reading Packets
            UdpState s = new UdpState();
            s.e = new IPEndPoint(Local_IP, Server_Port);
            s.u = Udp_Server;

            Udp_Server.BeginReceive(BeginReceive_UDP_Client_Callback, s);

        }

        public static void Broadcast_UDP(NetworkPacket np, PlayerNetworkCommunication RequestProcessor)
        {
            if (Clients_List != null)
                foreach (PlayerNetworkCommunication rp in Clients_List.Values)
                {
                    if (RequestProcessor != rp && rp != null)
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