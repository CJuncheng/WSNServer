using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;
using WSNServer.Util;
using System.Windows.Forms;
using WSNServer.Views;

namespace WSNServer.ServerExtensions
{
    public class WSNReceiveFilter : WSNServer.ServerExtensions.BeginEndMarkReceiveFilter<BinaryRequestInfo>
    {
        //开始和结束标记也可以是两个或两个以上的字节
        private readonly static byte[] BeginMark = new byte[] { 0x7E, 0x7E };
        private readonly static byte[] EndMark = new byte[] { 0xAA, 0xAA };

        public WSNSession Session
        {
            set;
            get;

        }

        public WSNReceiveFilter()
            : base(BeginMark, EndMark) //传入开始标记和结束标记
        {

        }
        public override BinaryRequestInfo Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
        {
            var filter = base.Filter(readBuffer, offset, length, toBeCopied, out rest);

            if (rest == 0)
            {
                //本次数据发送结束将CanLog设置为true
                CacheMap.CanLogCache[Session.SessionID].Value = true;

            }
            else
            {
                //需要把CanLog设置为false禁止记录日志否则会产生重复
                CacheMap.CanLogCache[Session.SessionID].Value = false;
            }

            return filter;
        }

        protected override BinaryRequestInfo ProcessMatchedRequest(byte[] readBuffer, int offset, int length)
        {
            //已解包后的数据,返回一个二进制实体
            return new BinaryRequestInfo(Guid.NewGuid().ToString(), readBuffer);
        }
    }
}
