using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSNServer.WSNExceptions
{
    public class FrameTotalLengthNotEqualException : Exception
    {
        public byte DataLength
        {
            set;
            get;
        }
        public int RealLength
        {

            set;
            get;
        }
        public FrameTotalLengthNotEqualException(byte dataLength, int realLength)
        {
            this.DataLength = dataLength;
            this.RealLength = realLength;
        }
        public override string Message
        {
            get
            {
                return "一帧数据总长度描述和实际长度不等";
            }
        }
    }
}
