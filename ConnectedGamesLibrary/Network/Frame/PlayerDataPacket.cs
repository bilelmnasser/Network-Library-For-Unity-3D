using ConnectedGames.Frame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ConnectedGamesLibrary.Network.Frame
{
    public enum GameObjectType:uint
    {

        Player = 0,
        AI = 1,
        Projectile = 2,
      





    }
    public enum DataType:uint
    {





        GameObjectPosition = 0,
        GameObjectRotation = 1,
        GameObjectScale = 2,        




    }
    public class GamePacket
    {

        public uint GameObjectID;

        public GameObjectType gameobjectType;

        public DataType dataType = DataType.GameObjectID;

        public byte[] Value;


        public GamePacket (uint id, GameObjectType type, DataType datatype, GameObject unityGameObject)
        {
            GameObjectID = id;
            gameobjectType = type;
            dataType =  datatype;



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



    }


}
