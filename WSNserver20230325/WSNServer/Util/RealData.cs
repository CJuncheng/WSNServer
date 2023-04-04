using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSNServer.Util
{
    //数据帧中采集数据中的Data部分（按照通信协议来）
    public class RealData
    {  
        public byte Count;
        public byte[] NodeAddress = new byte[2];
        public byte[] Data;


        //获取原始的byte数组
        public byte[] GetRawBytes()
        {
            var bytes = new byte[1+2+Data.Length];
           
            bytes[0] = Count;

            bytes[1] = NodeAddress[0];
            bytes[2] = NodeAddress[1];

            Array.Copy(Data, 0, bytes, 3, Data.Length);

            return bytes;
        }
    }
}
