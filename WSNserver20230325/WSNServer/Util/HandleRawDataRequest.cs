using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSNServer.ServerExtensions;

namespace WSNServer.Util
{
    public class HandleRawDataRequest
    {
        public WSNSession Session
        {
            set;
            get;
        }

        public byte[] Receive
        {
            set;
            get;
        }
        public int Offset
        {
            set;
            get;
        }
        public int Length
        {
            set;
            get;
        }

        public HandleRawDataRequest(WSNSession session, byte[] receive, int offset, int length)
        {
            this.Session = session;
            this.Receive = receive;
            this.Offset = offset;
            this.Length = length;
        }

        public bool Handle()
        {


            //本次收到的数据
            var receiveData = new byte[Length];
            Array.Copy(Receive, Offset, receiveData, 0, Length);
            //此处可以保存下位机发送的原始数据


            //收到数据的16进制字符串
            var x16 = ParseHelper.ShowX16(receiveData) + " ";

            var rawData = CacheMap.RawDataCache[Session.SessionID].Value + x16;
            CacheMap.RawDataCache[Session.SessionID].Value = rawData;



            //更新界面
            var updateParams = new UpdateParams();
            updateParams.UpdateType = UpdateType.UpdateTabPageData;
            updateParams.Message =
                string.Join(",", new string[] { Session.SessionID, x16 + " " });//完整数据显示在页面窗口上数据显示的格式,每段数据后空格
            ViewUpdateHelper.UpdateView(updateParams);


            //string head = new string(new char[] { (char)0x7E, (char)0x7E });
            //string foot = new string(new char[] { (char)0xAA, (char)0xAA });
            string head = "7E 7E";
            string foot = "AA AA";
            string strbytes = new string(rawData.ToList().ConvertAll(m => (char)m).ToArray());
            //strbytes.Contains(head) && strbytes.Contains(foot)
            //&& Receive[Receive.Length-2]==0xAA&& Receive[Receive.Length - 1]==0xAA
            if (strbytes.Contains(head) && strbytes.Contains(foot))
            {
                return true;
            }
            else
            {
                //已经重发的次数
                var hasResendCount = ResendHelper.GetCurrentResendCount(Session.SessionID);

                if (hasResendCount < SystemUtil.ResendTimes)
                {
                    //不包含帧头或帧尾校失败要求重发
                    //要求重发

                    //构建帧头帧尾校验不通过的重发命令
                    var resendAck = BuildHelper.BuildAllDataErrorAck();

                    //如果汇聚节点仍在线
                    if (Session.Connected)
                    {
                        //给汇聚节点回应
                        Session.Send(resendAck, 0, resendAck.Length);

                        //当前帧要求重发的错误次数加1
                        ResendHelper.AddResendData(receiveData, ResendType.HeadAndFoot, Session.SessionID);

                        //ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "已发送因帧头帧尾验证不通过导致的重发指令");
                        ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "未通过帧头帧尾校验要求重发");

                        //记录因帧头帧尾验证不通过导致的重发指令
                        var reSendOrderX16 = ParseHelper.ShowX16(resendAck);
                        LogHelper.SaveSendMessageLog(DateTime.Now, reSendOrderX16, Session.RemoteEndPoint.ToString());

                    }
                    else
                    {
                        ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, string.Format("要求重发时,汇聚已经断线"));
                        LogHelper.SaveExceptionLog(DateTime.Now, x16, "要求重发时,汇聚已经断线");
                    }


                }
                else
                {


                    //超过了所要求的最大重发次数不处理(此处可以记录日志)
                    ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, string.Format("连续重发{0}次数据仍然错误", SystemUtil.ResendTimes));
                    LogHelper.SaveExceptionLog(DateTime.Now, x16, "超过了要求回发的最大次数,数据仍然错误，停止要求重发");

                }
                //无须继续解析
                return false;

            }
        }
    }
}
