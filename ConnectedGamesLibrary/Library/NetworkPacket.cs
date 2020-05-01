using System;
using System.IO;
using System.Text;

namespace ConnectedGames.Library
{
    class NetworkPacket
    {

        /// <summary>
        /// the Header of Any Packet in our Library is the Length and it's unsigned Integer we dont need negative values ,
        /// Any Packet Sent or received , we first read 4 byte of the packet length
        /// </summary>
        internal uint Packet_Length;
        /// <summary>
        /// the Network ID of Packet in our Library is next in order, and it's unsigned Integer we dont need negative values 
        /// Any Packet Sent or received , we have 4 byte for the packet id
        /// </summary>
        public uint Packet_Id;
        /// <summary>
        /// the Network data of our Packet is next in order,
        /// and it's unknow size for the receiver,
        /// first we need the Packet_Length to be read by the receiver than we can know the size of data table
        /// </summary>
        public Byte[] Packet_Data;

        public NetworkPacket() { }
        public NetworkPacket(uint packet_Length)
        {
            Packet_Length = packet_Length;
            Packet_Data = new Byte[packet_Length];
        }
        public void ToString()
        {
            Console.WriteLine("Size : " + Packet_Length + "ID : " + Packet_Id + " Data : " + Encoding.UTF8.GetString(Packet_Data, 0, Packet_Data.Length));


        }

        public Byte[] ToByteArray()
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {
                    writer.Write(Packet_Length);
                    writer.Write(Packet_Id);
                    writer.Write(Packet_Data);

                }
                return m.ToArray();
            }

        }
        public void FromByteArray(Byte[] rawData)
        {


            NetworkPacket result = new NetworkPacket();
            using (MemoryStream m = new MemoryStream(rawData))
            {
                using (BinaryReader reader = new BinaryReader(m))
                {




                    Packet_Length = reader.ReadUInt32();
                    Packet_Id = reader.ReadUInt32();
                    Packet_Data = reader.ReadBytes((int)Packet_Length);

                }
            }

        }





        public byte[] Serialize()
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {
                    writer.Write(Packet_Length);
                    writer.Write(Packet_Id);
                    writer.Write(Packet_Data);

                }
                return m.ToArray();
            }
        }

        public void Desserialize(byte[] data)
        {
            NetworkPacket result = new NetworkPacket();
            using (MemoryStream m = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(m))
                {




                    Packet_Length = reader.ReadUInt32();
                    Packet_Id = reader.ReadUInt32();
                    Packet_Data = reader.ReadBytes((int)Packet_Length);

                }
            }
        }

    }
}
