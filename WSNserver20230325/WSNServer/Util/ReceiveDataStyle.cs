using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WSNServer.Util
{
    //接收的每一帧数据符合的总的信息格式
    public class ReceiveDataStyle
    {
        public byte[] FrameHeader;
        public byte FrameLength;
        public byte FrameControl;
        public byte FrameSequenceNumber;
        public byte[] FramePayload;
        public byte FCS;
        public byte[] FrameEnd;
    }
}
