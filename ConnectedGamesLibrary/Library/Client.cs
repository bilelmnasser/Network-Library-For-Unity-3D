using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace ConnectedGames.Library
{
    class Client : NetworkInformation
    {

        internal uint Packet_Counter = 0;

        public TcpClient TcpClient;
        public UdpClient UdpClient;

        public Thread ClientThread;



        public Client()
        {
            Server_IP = IPAddress.Parse("192.168.1.92");
            UDP_Reading_Loop();

        }

        public void TCP_Connect()
        {
            TcpClient = new TcpClient();
            TcpClient.Connect(Server_IP, Server_TCP_Port);
            Console.WriteLine("Connecting to " + TcpClient.Client.RemoteEndPoint);

            ClientThread = new Thread(TCP_Reading_Loop);
            ClientThread.Start();

        }
        private void TCP_Reading_Loop()
        {
            while (TcpClient.Connected)
            {


                if (TcpClient.GetStream().DataAvailable)
                {
                    Byte[] pL = new Byte[4];
                    Byte[] pID = new Byte[4];

                    TcpClient.GetStream().Read(pL, 0, 4);
                    TcpClient.GetStream().Read(pID, 0, 4);
                    Byte[] pDATA = new Byte[BitConverter.ToUInt32(pL, 0)];
                    TcpClient.GetStream().Read(pDATA, 0, (int)BitConverter.ToUInt32(pL, 0));

                    NetworkPacket np = new NetworkPacket(BitConverter.ToUInt32(pL, 0));

                    np.Packet_Length = BitConverter.ToUInt32(pL, 0);

                    np.Packet_Id = BitConverter.ToUInt32(pID, 0);
                    np.Packet_Data = pDATA;
                    np.ToString();


                }









            }



        }
        public void TCP_Send(NetworkPacket np)
        {
            np.Packet_Id = Packet_Counter;

            TcpClient.GetStream().Write(np.ToByteArray(), 0, 8 + (int)np.Packet_Length);
            Packet_Counter++;

        }


        public void UDP_Reading_Loop()
        {
            UdpClient = new UdpClient(new IPEndPoint(Local_IP, Client_Port));
            Console.WriteLine("Starting UDP CLient at iP = " + UdpClient.Client.LocalEndPoint.ToString());

            UdpState s = new UdpState();
            s.e = new IPEndPoint(Local_IP, Server_UDP_Port);
            s.u = UdpClient;

            UdpClient.BeginReceive(BeginReceive_UDP_Client_Callback, s);


        }

        private void BeginReceive_UDP_Client_Callback(IAsyncResult ar)
        {
            // End the operation and display the received data on 
            // the console.

            UdpClient u = ((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

            // UdpClient client =

            NetworkPacket NewP = new NetworkPacket();
            NewP.FromByteArray(UdpClient.EndReceive(ar, ref e));
            NewP.ToString();

            Console.Write("New Packet Received From " + e.ToString() + "   ");
            NewP.ToString();




            // Continue Reading Packets
            UdpState s = new UdpState();
            s.e = new IPEndPoint(Local_IP, Client_Port);
            s.u = UdpClient;

            UdpClient.BeginReceive(BeginReceive_UDP_Client_Callback, s);

        }
        public void UDP_Send(NetworkPacket np)
        {
            np.Packet_Id = Packet_Counter;

            UdpClient.Send(np.ToByteArray(), (int)np.Packet_Length + 8, new IPEndPoint(Server_IP, Server_UDP_Port));

            Packet_Counter++;
        }



        public PingReply Ping()
        {

            using (Ping p = new Ping())
            {
                return p.Send(Server_IP);



            }

        }
    }
}
