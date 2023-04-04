using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSNServer.WSNExceptions
{
    public class DataLengthTooShortException : Exception
    {
        public int RealLength
        {
            set;
            get;
        }

        public DataLengthTooShortException(int realLength)
        {
            this.RealLength = realLength;
        }

        public override string Message
        {
            get
            {
                return "帧长度太短";
            }
        }
    }
}
