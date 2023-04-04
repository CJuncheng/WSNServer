using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSNServer.Util
{
    //构建byte序列的回应
    public class BuildHelper
    {


        #region 封装的数据上传默认参数命令

        //构建上位机To汇聚节点上传数据的命令(封装的数据上传默认参数命令.)
        public static byte[] BuildDefaultUploadDataCmd()
        {

            byte[] cmdDefaultUploadNowPara = new byte[12];

            cmdDefaultUploadNowPara[0] = 0x7E;
            cmdDefaultUploadNowPara[1] = 0x7E;
            cmdDefaultUploadNowPara[2] = (byte)8;//长度
            cmdDefaultUploadNowPara[3] = (byte)FrameControlType.Cmd;//命令帧
            cmdDefaultUploadNowPara[4] = SequenceHelper.random;//序列号
            cmdDefaultUploadNowPara[5] = (byte)CmdType.CmdOrderForData;//表示数据上传命令
            cmdDefaultUploadNowPara[6] = 0x01;//时间标志：无时间
            cmdDefaultUploadNowPara[7] = 0x00;//节点标志：所有节点
            cmdDefaultUploadNowPara[8] = 0x0F;//全部数据类型上传，若改为0x0E不包括节点状态表;

            //计算fcs用到的数据部分
            var fcscomputedata = new byte[7];
            Array.Copy(cmdDefaultUploadNowPara, 2, fcscomputedata, 0, 7);

            cmdDefaultUploadNowPara[9] = CommonHelper.XorCheckForBytes(fcscomputedata);

            cmdDefaultUploadNowPara[10] = 0xAA;
            cmdDefaultUploadNowPara[11] = 0xAA;
            SequenceHelper.random++;

            return cmdDefaultUploadNowPara;
        }
        #endregion


        #region 封装数据无错误应答帧

        public static byte[] BuildNoErrorDataAck()  // eg 7E7E05XX02FCSAAAA
        {

              byte[] ackNoErrorData = new byte[9];

              ackNoErrorData[0] = 0x7E;
              ackNoErrorData[1] = 0x7E;

              ackNoErrorData[2] = (byte)5;
              ackNoErrorData[3] = (byte)FrameControlType.Ack;//应答帧
              ackNoErrorData[4] = SequenceHelper.random;//上位机维护的FrameSeq
              ackNoErrorData[5] = (byte)AckOption.DataRight;//数据无错，继续下一块，无参数

              //计算fcs用到的数据部分
              var fcscomputedata = new byte[4];
              Array.Copy(ackNoErrorData, 2, fcscomputedata, 0, 4);

              ackNoErrorData[6] = CommonHelper.XorCheckForBytes(fcscomputedata);//校验位

              ackNoErrorData[7] = 0xAA;
              ackNoErrorData[8] = 0xAA;

              SequenceHelper.random++;
              return ackNoErrorData;

        }

        #endregion


        #region 封装当次发送数据全部重传，重传应答帧
        /// <summary>
        /// 封装当次发送数据全部出错，重传应答帧.<br></br>
        /// </summary>
        /// <returns></returns>
        //帧头帧尾校验不通过的回应帧
        //帧长度太短的回应帧
        //帧长度与实际不符合的回应帧
        public static byte[] BuildAllDataErrorAck()
        {

           byte[] ackAllDataError = new byte[9];

           ackAllDataError[0] = 0x7E;
           ackAllDataError[1] = 0x7E;

           ackAllDataError[2] = (byte)5;
           ackAllDataError[3] = (byte)FrameControlType.Ack;//应答帧
           ackAllDataError[4] = SequenceHelper.random;//上位机维护的FrameSeq
           ackAllDataError[5] = (byte)AckOption.AllError;//数据有错，当次所发的全部数据重传，无参数（多发生在帧头帧尾校验出错）

           //计算fcs用到的数据部分
           var fcscomputedata = new byte[4];
           Array.Copy(ackAllDataError, 2, fcscomputedata, 0, 4);

           ackAllDataError[6] = CommonHelper.XorCheckForBytes(fcscomputedata);//校验位

           ackAllDataError[7] = 0xAA;
           ackAllDataError[8] = 0xAA;

           SequenceHelper.random++;
           return ackAllDataError;
        }

        #endregion


        #region  封装命令无错应答帧
        //收到汇聚节点请求休眠的指令的回应帧
        //联网成功回应帧
        public static byte[] BuildCmdRightResponAck()
        {

            byte[] cmdRightResponFrame = new byte[9];
            cmdRightResponFrame[0] = 0x7E;
            cmdRightResponFrame[1] = 0x7E;

            cmdRightResponFrame[2] = (byte)5;
            cmdRightResponFrame[3] = (byte)FrameControlType.Ack;
            cmdRightResponFrame[4] = SequenceHelper.random;
            cmdRightResponFrame[5] = (byte)AckOption.CmdRight;//命令无错

            //计算fcs用到的数据部分
            var fcscomputedata = new byte[4];
            Array.Copy(cmdRightResponFrame, 2, fcscomputedata, 0, 4);

            //计算fcs码
            cmdRightResponFrame[6] = CommonHelper.XorCheckForBytes(fcscomputedata);

            cmdRightResponFrame[7] = 0xAA;
            cmdRightResponFrame[8] = 0xAA;

            SequenceHelper.random++;

            return cmdRightResponFrame;

        }

        #endregion


        #region 封装命令有错应答帧
        /// <summary>
        /// 封装命令有错应答帧.<br></br>
        /// </summary>
        /// <param name="frameSeq"></param>
        /// <returns>FCS校验失败的时候，记录该帧序列号，要求重发该帧</returns>
        public static byte[] BuildCmdErrorResponFrameAck(byte frameSeq) // eg 7E7E0603(random)0101(frameSeq)(FCS)AAAA
        {
            byte[] cmdErrorResponFrame = new byte[10];

            cmdErrorResponFrame[0] = 0x7E;
            cmdErrorResponFrame[1] = 0x7E;

            cmdErrorResponFrame[2] = (byte)6;
            cmdErrorResponFrame[3] = (byte)FrameControlType.Ack;//应答帧
            cmdErrorResponFrame[4] = SequenceHelper.random;//FrameSeq
            cmdErrorResponFrame[5] = (byte)AckOption.CmdError;//命令有错
            cmdErrorResponFrame[6] = frameSeq;

            //计算fcs用到的数据部分
            var fcscomputedata = new byte[5];
            Array.Copy(cmdErrorResponFrame, 2, fcscomputedata, 0, 5);

            cmdErrorResponFrame[7] = CommonHelper.XorCheckForBytes(fcscomputedata);//校验位

            cmdErrorResponFrame[8] = 0xAA;
            cmdErrorResponFrame[9] = 0xAA;

            SequenceHelper.random++;
            return cmdErrorResponFrame;
        }
        #endregion


        #region 封装的服务器下发休眠指令和拍照指令

       
        public static byte[] BuildSleepCmdAndTakingPhotoCmd(int CityID)
        {
            int sleeptime;
            int collettime;
            /* 0：不拍照
               0x1-0xA: 对应节点编号拍照
               0: 所有节点都拍照
            */
            int sensorID;

            //var CityID = CityHelper.GetCollectCityIDByCollectNode(nodeaddress);
            using (var db = new DB.WirelessSensorNetworkEntities())
            {
                var query = from item in db.WSNTimeSetTables
                            where item.CityID ==CityID
                            select item;
                var data = query.ToList();
                sleeptime = data[0].SleepTime;
                collettime = data[0].CollectTime;
                sensorID = (int)data[0].NodeID;
            }
            int minute = DateTime.Now.Minute+collettime;
            int hour = DateTime.Now.Hour;
            if (minute >= 60) {
                hour += minute / 60;
                minute = minute % 60;
            }
            byte[] cmdDefaultSleepPara = new byte[23];

            cmdDefaultSleepPara[0] = 0x7E;
            cmdDefaultSleepPara[1] = 0x7E;

            cmdDefaultSleepPara[2] = (byte)19;
            cmdDefaultSleepPara[3] = (byte)FrameControlType.Cmd;
            cmdDefaultSleepPara[4] = SequenceHelper.random;

            cmdDefaultSleepPara[5] = (byte)CmdType.CmdOrderSleep;  
            cmdDefaultSleepPara[6] = 0x00;
            cmdDefaultSleepPara[7] = (byte)collettime;   //默认采集时间  0x1E 30min
            cmdDefaultSleepPara[8] = 0x00;
            cmdDefaultSleepPara[9] = (byte)collettime;  //默认休眠时间   0x05 5min
            int yearh = DateTime.Now.Year / 100;
            cmdDefaultSleepPara[10] = (byte)yearh; //取年的高位
            int yearl = DateTime.Now.Year % 100;
            cmdDefaultSleepPara[11] = (byte)yearl;//取年的低位
            //cmdDefaultSleepPara[11] = (byte)((((Int16)DateTime.Now.Year) & (Int16)0x0F));//取年的低位
            cmdDefaultSleepPara[12] = (byte)DateTime.Now.Month;
            cmdDefaultSleepPara[13] = (byte)DateTime.Now.Day;
            cmdDefaultSleepPara[14] = (byte)DateTime.Now.Hour;
            cmdDefaultSleepPara[15] = (byte)DateTime.Now.Minute;
            cmdDefaultSleepPara[16] = (byte)DateTime.Now.Second;
            cmdDefaultSleepPara[17] = 0x2f;//传感器开关
            int lo = sensorID & 0xFF; // 采集器编号0-7位
            int hi = (sensorID>>8)&0xFF; // 采集器编号8-15位
            cmdDefaultSleepPara[18] = (byte)hi; // 采集器编号8-15位
            cmdDefaultSleepPara[19] = (byte)lo; // 采集器编号0-7位

            //计算fcs用到的数据部分
            var fcscomputedata = new byte[18];
            Array.Copy(cmdDefaultSleepPara, 2, fcscomputedata, 0, 18);

            cmdDefaultSleepPara[20] = CommonHelper.XorCheckForBytes(fcscomputedata);//校验位


            cmdDefaultSleepPara[21] = 0xAA;
            cmdDefaultSleepPara[22] = 0xAA;
            SequenceHelper.random++;
            return cmdDefaultSleepPara;
        }

        #endregion


    }
}
