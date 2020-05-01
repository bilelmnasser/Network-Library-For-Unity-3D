using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConnectedGames.Library
{
    internal class RequestProcessor
    {

        internal uint Packet_Counter = 0;
        public IPEndPoint DistanceCLientAddress;



        #region TCP client processing
        internal TcpClient client;
        internal Server instance;

        internal Thread clientThread;


        public RequestProcessor()
        {
            Packet_Counter = 0;
        }


        internal void Proccess()
        {

            clientThread = new Thread(Reading_Loop);
            clientThread.Start();

        }


        internal void Reading_Loop()
        {
            // client = c;
            try
            {
                var networkStream = client.GetStream();
                var binaryReader = new BinaryReader(networkStream);
                while (true)
                {

                    try
                    {
                        uint packetLength = binaryReader.ReadUInt32();
                        NetworkPacket packet = new NetworkPacket(packetLength);
                        packet.Packet_Id = binaryReader.ReadUInt32();
                        packet.Packet_Data = binaryReader.ReadBytes((int)packetLength);

                        Console.Write("New Packet Received From " + client.Client.RemoteEndPoint.ToString() + " ==> ");
                        packet.ToString();

                        Server.Broadcast_TCP(packet, this);


                    }
                    catch (Exception ex)
                    {
                        binaryReader.Close();
                        networkStream.Close();
                        Console.WriteLine(ex.Message);
                        return;
                    }
                }

            }
            finally
            {

            }
        }


        public void Send_reliable_Message(NetworkPacket np)
        {


            np.Packet_Id = Packet_Counter;
            client.GetStream().Write(np.ToByteArray(), 0, (int)np.Packet_Length + 8);
            Packet_Counter++;
        }
        #endregion


        #region UDP client processing



        internal void Send_unreliable_Message(NetworkPacket np)
        {
            np.Packet_Id = Packet_Counter;

            instance.Udp_Server.Send(np.ToByteArray(), (int)np.Packet_Length + 8, (IPEndPoint)DistanceCLientAddress);

            Packet_Counter++;
        }
        #endregion




    }
}