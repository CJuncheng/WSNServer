using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using WSNServer.WSNExceptions;
using System.Diagnostics;

namespace WSNServer.Util
{



    //将传过来的byte序列转换为命令实体
    public class ParseHelper
    {

        #region   通过帧头帧尾校验的数据是否能通过FCS校验

        //将一条二进制命令转换为实体
        public static ReceiveDataStyle ParseAll(byte[] checkdata)
        {
            //长度太短
            if (checkdata.Count() < 9)
            {
                throw new DataLengthTooShortException(checkdata.Count());
            }

            //总长度
            var totalength = (int)checkdata[2];
            if (totalength != (checkdata.Count() - 4))
            {
                throw new FrameTotalLengthNotEqualException(checkdata[2], (checkdata.Count() - 4));
            }

            //首先进行fcs校验,看上位机校验的结果和下位机传过来的fcs位是否相同
            var fcsbit = new byte[1];
            //取得下位机传过来的fcs位
            Array.Copy(checkdata, (checkdata.Length - 3), fcsbit, 0, 1);

            //需要计算fcs的数据部分长度为减去 2bit帧头 2bit阵尾 和 1 帧 fcs码 总共减5
            var needcomputefcsframebytes = new byte[checkdata.Length - 5];
            //上位机根据下位机传过来的数据计算出对比的fcs码
            Array.Copy(checkdata, 2, needcomputefcsframebytes, 0, (checkdata.Length - 5));

            var computefcsbit = CommonHelper.XorCheckForBytes(needcomputefcsframebytes);

            //zry:将计算出来的结果与fcs码进行对比
            if (fcsbit[0] == computefcsbit)
            {

                ReceiveDataStyle data = ParseAllData(checkdata);

                //fcs重发缓冲区归零 由于每次会话序号唯一,所以即使不归0 一旦解析正确，客户端也不可能再使用同一个序号
                //所以此处可以不填充逻辑

                return data;
            }
            else
            {
                var FrameSequenceNumber = (int)checkdata[4];
                //fcs校验失败,发送给客户端fcs校验失败的应答
                throw new FCSCodeIncorrectException(FrameSequenceNumber);

            }

        }

        #endregion

        //私有函数 将一条二进制命令转换为实体
        private static ReceiveDataStyle ParseAllData(byte[] checkdata)
        {
            //将checkdata中的内容放到data中
            ReceiveDataStyle data = new ReceiveDataStyle();
            data.FrameHeader = new byte[2];
            data.FrameHeader[0] = checkdata[0];
            data.FrameHeader[1] = checkdata[1];
            data.FrameLength = checkdata[2];
            data.FrameControl = checkdata[3];
            data.FrameSequenceNumber = checkdata[4];

            int payloadlength = (ushort)data.FrameLength - 4;
            data.FramePayload = new byte[payloadlength];

            //拷贝负载帧
            Array.Copy(checkdata, 5, data.FramePayload, 0, payloadlength);

            data.FCS = checkdata[5 + payloadlength];
            data.FrameEnd = new byte[2];
            data.FrameEnd[0] = checkdata[6 + payloadlength];
            data.FrameEnd[1] = checkdata[7 + payloadlength];

            return data;

        }


        //将数据帧的payload部分转换为对应实体
        public static DataFrame ParseDataFrame(byte[] payload)
        {
            var dataframe = new DataFrame();
            dataframe.DataOptions[0] = payload[0];
            dataframe.DataOptions[1] = payload[1];
            dataframe.Length = payload[2];

            //if ((int)dataframe.Length != (payload.Length - 3))
            //{
            //    throw new DataFrameLengthNotEqualException(dataframe.Length, (payload.Length - 3));
            //}

            dataframe.Data = new byte[payload.Length - 3];
            Array.Copy(payload, 3, dataframe.Data, 0, payload.Length - 3);
            return dataframe;
        }


        #region  获取下位机发过来的邻居表的具体数据

        //递归解析数据帧当中的邻居表的具体数据
        public static List<NeighborRealData> ParseNeighborRealData(byte[] neighborrealdata)
        {
            // byte序列转换成char数组
            var chars = neighborrealdata.ToList().ConvertAll(m => (char)m).ToArray();
            // string 就是char类型的数组，然后把转换后的char数组装到一个string里面 
            var splitstring = new string(chars);
            //选择数据分割的标志,利用string的split方法，这个方法是系统自带的分割方法，把数据分段
            //分隔符
            string separator = new string(new char[] { (char)0xEE, (char)0xEE });
            //分隔方法（EE EE 数据 EE EE 数据。。。。，）用spilt方法用EE EE做数据分隔符的话就多了最开始的EE EE，要去掉
            var chasrarray = splitstring.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);

            var list = new List<NeighborRealData>();

            foreach (var item in chasrarray)
            {
                var bytes = item.ToList().ConvertAll(m => (byte)m).ToArray();
                NeighborRealData data = new NeighborRealData();
                //加上开始标志EE EE
                data.Data = (new byte[] { 0xEE, 0xEE }).Concat(bytes).ToArray();
                list.Add(data);
            }

            return list;
        }
        ////递归数据帧当中的邻居表的解析
        //private static void ParseNeighborRealData(byte[] neighborrealdata, int offset, List<NeighborRealData> list)
        //{

        //    //命名大部分和通信协议上一致
        //    var data = new NeighborRealData();

        //    if (neighborrealdata.ToList<byte>().Contains(0xEE))
        //    {
        //        for (var i = 9; i < neighborrealdata.Length; i++)
        //        {
        //            //邻居表数据开始的标志
        //            if ((neighborrealdata[offset + 0] == data.loopflag[0]) && (neighborrealdata[offset + 1] == data.loopflag[1]))//查找开始标志
        //            {
        //                //采集时间
        //                data.CollectTime[0] = neighborrealdata[offset + 2];
        //                data.CollectTime[1] = neighborrealdata[offset + 3];
        //                data.CollectTime[2] = neighborrealdata[offset + 4];
        //                data.CollectTime[3] = neighborrealdata[offset + 5];
        //                data.CollectTime[4] = neighborrealdata[offset + 6];
        //                data.CollectTime[5] = neighborrealdata[offset + 7];
        //                data.CollectTime[6] = neighborrealdata[offset + 8];

        //                if ((neighborrealdata[offset + i] == data.loopflag[0]) && (neighborrealdata[offset + i] == data.loopflag[1]))//查找下一次的开始标志
        //                {

        //                    var newoffset = offset + i;//计算新偏移量

        //                    var currentdatalength = newoffset - 7 - 2; //计算当前数据的长度

        //                    //填充数据
        //                    data.Data = new byte[currentdatalength];
        //                    Array.Copy(neighborrealdata, (newoffset + 9), data.Data, 0, currentdatalength);


        //                    //将解析后构造的数据实体添加到列表
        //                    list.Add(data);


        //                    //递归调用数据解析
        //                    if (newoffset < neighborrealdata.Length)
        //                    {
        //                        //try
        //                        //{
        //                        ParseNeighborRealData(neighborrealdata, newoffset, list);
        //                        //}
        //                        //catch (Exception ex)
        //                        //{
        //                        //    Debug.WriteLine(string.Format("本帧数据解析失败:原始数据为:{0}", ShowX16(realdata)));
        //                        //    Debug.WriteLine("偏移量为:" + newoffset);
        //                        //    Debug.WriteLine("错误信息为:" + ex.Message);
        //                        //    //容错处理,即使解析出现错误，则忽略本次解析直接返回，错误之前的数据解析仍然有效


        //                        //}
        //                    }
        //                    i--;//循环自加，故先自减

        //                    break;
        //                }


        //            }


        //        }


        //    }

        //}

        #endregion



        #region  获取下位机发过来的路由表的具体数据

        //递归解析数据帧当中的路由表的具体数据
        public static List<RouteRealData> ParseRouteRealData(byte[] routerealdata)
        {
            var list = new List<RouteRealData>();
            int offset = 0;
            ParseRouteRealData(routerealdata, offset, list);
            return list;
        }

        //递归数据帧当中的路由表的解析
        private static void ParseRouteRealData(byte[] routerealdata, int offset, List<RouteRealData> list)
        {
            //命名大部分和通信协议上一致
            var data = new RouteRealData();

            //路由表数据开始的标志
            if ((routerealdata[offset + 0] == data.loopflag[0]) && (routerealdata[offset + 1] == data.loopflag[1]))
            {

                //采集时间
                data.CollectTime[0] = routerealdata[offset + 2];
                data.CollectTime[1] = routerealdata[offset + 3];
                data.CollectTime[2] = routerealdata[offset + 4];
                data.CollectTime[3] = routerealdata[offset + 5];
                data.CollectTime[4] = routerealdata[offset + 6];
                data.CollectTime[5] = routerealdata[offset + 7];
                data.CollectTime[6] = routerealdata[offset + 8];

                //count
                data.CountByte = routerealdata[offset + 9];

                //数据组数
                int n = data.CountByte;

                //源节点地址
                data.SourceNodeAddress[0] = routerealdata[offset + 10];
                data.SourceNodeAddress[1] = routerealdata[offset + 11];

                ////计算当前数据的长度
                var currentdatalength = 4 * n;

                //填充数据
                data.Data = new byte[currentdatalength];
                Array.Copy(routerealdata, (offset + 12), data.Data, 0, currentdatalength);


                //将解析后构造的数据实体添加到列表
                list.Add(data);

                //计算新的偏移量

                var newoffset = offset + 2 + 7 + 1 + 2 + currentdatalength;

                //递归调用数据解析
                if (newoffset <= (routerealdata.Length - 14))
                {
                    //try
                    //{
                    ParseRouteRealData(routerealdata, newoffset, list);
                    //}
                    //catch (Exception ex)
                    //{
                    //    Debug.WriteLine(string.Format("本帧数据解析失败:原始数据为:{0}", ShowX16(realdata)));
                    //    Debug.WriteLine("偏移量为:" + newoffset);
                    //    Debug.WriteLine("错误信息为:" + ex.Message);
                    //    //容错处理,即使解析出现错误，则忽略本次解析直接返回，错误之前的数据解析仍然有效


                    //}
                }


            }



        }

        #endregion



        #region  获取下位机发过来的采集数据的具体数据


        //递归解析数据帧当中的采集数据的具体数据
        public static List<RealData> ParseRealData(byte[] realdata)
        {
            var list = new List<RealData>();
            int offset = 0;
            ParseRealData(realdata, offset, list);
            return list;
        }


        //递归数据帧当中的采集数据的解析
        private static void ParseRealData(byte[] realdata, int offset, List<RealData> list)
        {
            //命名大部分和通信协议上一致
            var data = new RealData();
            //Count位
            data.Count = realdata[offset + 0];
            //NodeAddress位
            data.NodeAddress[0] = realdata[offset + 1];
            data.NodeAddress[1] = realdata[offset + 2];

            //传感器个数, 高4位
            int ns = (data.Count & 0xF0) >> 4;

            //数据组数,低四位
            int n = data.Count & 0x0F;

            ////计算当前数据的长度
            var currentdatalength = (n * ((3 * ns) + 7));

            //填充数据
            data.Data = new byte[currentdatalength];
            Array.Copy(realdata, (offset + 3), data.Data, 0, currentdatalength);


            //将解析后构造的数据实体添加到列表
            list.Add(data);

            //计算新的偏移量

            var newoffset = offset + currentdatalength + 3;

            //递归调用数据解析
            if (newoffset <= (realdata.Length - 3))
            {
                //try
                //{
                ParseRealData(realdata, newoffset, list);
                //}
                //catch (Exception ex)
                //{
                //    Debug.WriteLine(string.Format("本帧数据解析失败:原始数据为:{0}", ShowX16(realdata)));
                //    Debug.WriteLine("偏移量为:" + newoffset);
                //    Debug.WriteLine("错误信息为:" + ex.Message);
                //    //容错处理,即使解析出现错误，则忽略本次解析直接返回，错误之前的数据解析仍然有效


                //}
            }

        }
        #endregion



        //得到byte数组的16进制显示
        public static string ShowX16(byte[] bytes)
        {
            //空格隔开16进制显示
            return string.Join(" ", bytes.ToList()
                .ConvertAll(m => m.ToString("X2")).ToArray());

        }

    }
}
