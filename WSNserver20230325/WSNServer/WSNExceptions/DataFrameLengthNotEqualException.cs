using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSNServer.WSNExceptions
{
    public class DataFrameLengthNotEqualException : Exception
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


        public DataFrameLengthNotEqualException(byte dataLength, int realLength)
        {
            this.DataLength = dataLength;
            this.RealLength = realLength;
        }


        public override string Message
        {
            get
            {
                return "数据帧中的长度描述和实际长度不等";
            }
        }
    }
}
