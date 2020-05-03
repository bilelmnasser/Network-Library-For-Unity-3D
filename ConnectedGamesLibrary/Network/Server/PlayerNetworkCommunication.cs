using ConnectedGames.Utilities;
using ConnectedGames.Server;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace ConnectedGames.Server
{

    public class PlayerNetworkCommunication
    {
        public int PlayerIdentifier;
        public int PlayerName ;

        public uint Packet_Counter = 1;
        public IPEndPoint CLientAddress;
        internal Server instance;

        public PlayerNetworkCommunication()
        {
            Packet_Counter = 1;
            LastReceivedPackedID = 0;
            LostPacketsID = new List<uint>();
        }



        #region TCP client processing
        internal TcpClient client;
        internal Thread clientThread;

        private bool TcpReadingLoop=false;


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
                for (int i = (int)LastReceivedPackedID + 1; i < packetCurrebtID - 1; i++)
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

        internal void Tcp_Proccess()
        {
            TcpReadingLoop = true;
            clientThread = new Thread(Tcp_Reading_Loop);
            clientThread.Start();

        }


        internal void Tcp_Reading_Loop()
        {
            // client = c;
            try
            {
                var networkStream = client.GetStream();
                var binaryReader = new BinaryReader(networkStream);
                while (TcpReadingLoop)
                {

                    try
                    {
                        uint packetLength = binaryReader.ReadUInt32();
                        NetworkPacket packet = new NetworkPacket(packetLength);
                        packet.Packet_Id = binaryReader.ReadUInt32();
                        packet.Packet_Data = binaryReader.ReadBytes((int)packetLength);

                        Console.WriteLine("New Packet Received From " + client.Client.RemoteEndPoint.ToString() + " ==> "+ packet.ToString());
                        

                        if (Server._OnMessageReceivedFromPlayer != null)
                        {
                            Server._OnMessageReceivedFromPlayer.Invoke(this, packet);
                        }
                        CheckLostnotReceivedPacket(packet.Packet_Id);

                        Console.WriteLine(" Packet Loss Percentage : " + PacketLossPercentage() + " % ");
                        Server.Broadcast_TCP(packet, this);


                    }
                    catch (Exception ex)
                    {
                        binaryReader.Close();
                        networkStream.Close();
                      //  Console.WriteLine(ex.Message);
                     //   Disconnect();
                        return;
                    }
                }

            }
            finally
            {
                Disconnect();
            }
        }


        public void Send_reliable_Message(NetworkPacket np)
        {


            np.Packet_Id = Packet_Counter;
            client.GetStream().Write(np.ToByteArray(), 0, (int)np.Packet_Length + 8);
            Packet_Counter++;
        }


        public void DisonnectTcp()
        {
            TcpReadingLoop = false;
            if(clientThread!=null)
            clientThread.Abort();


        }

        #endregion


        #region UDP client processing



        internal void Send_unreliable_Message(NetworkPacket np)
        {
            np.Packet_Id = Packet_Counter;

            instance.Udp_Server.Send(np.ToByteArray(), (int)np.Packet_Length + 8, (IPEndPoint)CLientAddress);

            Packet_Counter++;
        }
        #endregion


        public void Disconnect()
        {
            Server.Clients_List[PlayerIdentifier] = null;
            Console.WriteLine("Player DIsconnecting : "+CLientAddress.ToString());
            DisonnectTcp();


        }


    }
}