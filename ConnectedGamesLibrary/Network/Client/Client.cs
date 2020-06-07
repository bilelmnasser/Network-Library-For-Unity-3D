using ConnectedGames.Utilities;
using ConnectedGames.Frame;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace ConnectedGames.Client
{
    public delegate void OnMessageReceivedFromServer( NetworkPacket packet);

    public class Client : NetworkInformation
    {

        internal uint Packet_Counter = 1;


        internal uint LastReceivedPackedID = 0;

        public List<uint> LostPacketsID;

        public void CheckLostnotReceivedPacket(uint packetCurrebtID)
        {
            LostPacketsID.Remove(packetCurrebtID);
            if (LastReceivedPackedID + 1 == packetCurrebtID)
            { 
                
                return;
            }
            else if (packetCurrebtID > LastReceivedPackedID + 1)
            {
                for (int i = (int)LastReceivedPackedID + 1;i< packetCurrebtID-1;i++ )
                    LostPacketsID.Add((uint)i);


            }

            LastReceivedPackedID = packetCurrebtID;


        }

        public int PacketLossPercentage()
        {
            if (LostPacketsID.Count != 0)
                return Convert.ToInt32((LostPacketsID.Count / (int)LastReceivedPackedID) * 100);
            else return 0;


        }

        public static OnMessageReceivedFromServer OnMessageReceived;

        private TcpClient TcpClient;
        private UdpClient UdpClient;
        private bool LoopTcpReading = false;
        private Thread ClientThread;

        public bool Connect(IPAddress server)
        {
            LastReceivedPackedID = 0;
            LostPacketsID.Clear();
               Server_IP = server;
            if (TCP_Connect(Server_IP))
            {

                UDP_Reading_Loop();
                return true;
            }
            else
                return false;


        }

                public Client()
        {
            LostPacketsID = new List<uint>();



        }

        public bool TCP_Connect(IPAddress server )
        {
            try
            {
                TcpClient = new TcpClient();
                TcpClient.Connect(server, Server_Port);
                Console.WriteLine("Connecting to " + TcpClient.Client.RemoteEndPoint);
                Client_Port = ((IPEndPoint)(TcpClient.Client.LocalEndPoint)).Port;
                LoopTcpReading = true;
                ClientThread = new Thread(TCP_Reading_Loop);
                ClientThread.Start();
                return true;
            }
            catch
            {
                return false;

            }
        }
        private void TCP_Reading_Loop()
        {
            try
            {


                while (LoopTcpReading)
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
                        CheckLostnotReceivedPacket(np.Packet_Id);

                        Console.WriteLine(" Packet Loss Percentage : " + PacketLossPercentage() + " % ");
                        Console.WriteLine(np.ToString()+"\n");

                        if (OnMessageReceived != null)
                            OnMessageReceived.Invoke(np);


                    }









                }
            }catch(Exception ex)
            {
               
                Console.WriteLine(ex.Message);
                Disconnect();
                return;
            }


        }
        public void TCP_Send(NetworkPacket np)
        {
            try
            {



                np.Packet_Id = Packet_Counter;

                TcpClient.GetStream().Write(np.ToByteArray(), 0, 8 + (int)np.Packet_Length);
                Packet_Counter++;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }

        }


        public void UDP_Reading_Loop()
        {
            UdpClient = new UdpClient( Client_Port);
            Console.WriteLine("Starting UDP CLient at iP = " + UdpClient.Client.LocalEndPoint.ToString());

            UdpState s = new UdpState();
            s.e = new IPEndPoint(Local_IP, Server_Port);
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


            CheckLostnotReceivedPacket(NewP.Packet_Id);
            Console.WriteLine(" Packet Loss Percentage : " + PacketLossPercentage() + " %");
            Console.WriteLine(NewP.ToString() + "\n");

          //  Console.WriteLine("New Packet Received From " + e.ToString() + "   " + NewP.ToString());

            if (OnMessageReceived!=null)
            OnMessageReceived.Invoke(NewP);

            // Continue Reading Packets
            UdpState s = new UdpState();
            s.e = new IPEndPoint(Local_IP, Client_Port);
            s.u = UdpClient;

            UdpClient.BeginReceive(BeginReceive_UDP_Client_Callback, s);

        }
        public void UDP_Send(NetworkPacket np)
        {
            np.Packet_Id = Packet_Counter;

            UdpClient.Send(np.ToByteArray(), (int)np.Packet_Length + 8, new IPEndPoint(Server_IP, Server_Port));

            Packet_Counter++;
        }



        public PingReply Ping()
        {

            using (Ping p = new Ping())
            {

                PingReply reply= p.Send(Server_IP);
                Console.WriteLine(reply.Status + " : " + reply.RoundtripTime + " ms");
                return reply;


            }

        }

        public void Disconnect()
        {
            LoopTcpReading = false;

            if (ClientThread != null)
                ClientThread.Abort();  
            
           // TcpClient.Close();
          //  UdpClient.Close();
            Console.WriteLine("Client Lost Connexion to Server");
        }
    }
}
