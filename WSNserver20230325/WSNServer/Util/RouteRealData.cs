using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSNServer.Util
{

    //数据帧中路由表的Data部分（按照通信协议来）
    public class RouteRealData
    {

        public byte[] loopflag ={0xEE ,0xEE}; //每次循环开始的标志 
        public byte[] CollectTime = new byte[7]; //每次的采集时间
        public byte CountByte;//计数每个源节点有多少个“下一跳+目的”的路由表项
        public byte[] SourceNodeAddress = new byte[2];
        public byte[] Data;

        //获取原始的byte数组
        public byte[] GetRawBytes()
        {
            var bytes = new byte[2 + 7 + 1 + 2 + Data.Length];

            //每次循环开始的标志 
            bytes[0] = loopflag[0];
            bytes[1] = loopflag[1];
            
            bytes[2] = CollectTime[0];
            bytes[3] = CollectTime[1];
            bytes[4] = CollectTime[2];
            bytes[5] = CollectTime[3];
            bytes[6] = CollectTime[4];
            bytes[7] = CollectTime[5];
            bytes[8] = CollectTime[6];
         
            //count
            bytes[9] = CountByte;
            //源节点地址
            bytes[10] = SourceNodeAddress[0];
            bytes[11] = SourceNodeAddress[1];

            Array.Copy(Data, 0, bytes, 12, Data.Length);

            return bytes;
        }

    }

}
