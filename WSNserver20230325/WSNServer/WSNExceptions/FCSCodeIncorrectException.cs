using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSNServer.WSNExceptions
{
    public class FCSCodeIncorrectException : Exception
    {
        public int FrameSequenceNumber
        {
            set;
            get;
        }

        public FCSCodeIncorrectException(int frameSequenceNumber)
        {
            this.FrameSequenceNumber = frameSequenceNumber;
        }

        public override string Message
        {
            get
            {
                return "FCS码计算错误";
            }
        }
    }
}
