using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace ConnectedGamesLibrary.Network.Utilities
{
  
      
    
  public static class  Extentions
    {
        public static T[] ConcatArrays<T>(params T[][] list)
        {
            var result = new T[list.Sum(a => a.Length)];
            int offset = 0;
            for (int x = 0; x < list.Length; x++)
            {
                list[x].CopyTo(result, offset);
                offset += list[x].Length;
            }
            return result;
        }

        public static byte[] ToByteArray(this float Value)
        {
            return BitConverter.GetBytes(Value);
        }
        public static float ToFloat(this byte[] Value)
        {
            return BitConverter.ToSingle(Value,0);
        }

        public static byte[] ToByteArray(this Vector3 Value)
        {
            return ConcatArrays(Value.x.ToByteArray(), Value.y.ToByteArray(), Value.z.ToByteArray());
        }
        public static Vector3 ToVector3(this byte[] Value)
        {
            return new Vector3( BitConverter.ToSingle(Value, 0), BitConverter.ToSingle(Value, 4), BitConverter.ToSingle(Value, 8));

        }
        public static byte[] ToByteArray(this Quaternion Value)
        {
            return ConcatArrays(Value.x.ToByteArray(), Value.y.ToByteArray(), Value.z.ToByteArray(), Value.w.ToByteArray());
        }
        public static Quaternion ToQuaternion(this byte[] Value)
        {
            return new Quaternion(BitConverter.ToSingle(Value, 0), BitConverter.ToSingle(Value, 4), BitConverter.ToSingle(Value, 8), BitConverter.ToSingle(Value, 12));

        }




    }
}
