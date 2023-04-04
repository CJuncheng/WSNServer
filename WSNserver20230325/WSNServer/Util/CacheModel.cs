using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSNServer.Util
{

    public class ResendCacheModel
    {
        public byte[] Data;
        public int SequenceNumber;
        public ResendType Type;
    }

    public enum ResendType
    {
        FCSIncorrect = 0,
        FrameTotalLengthNotEqual,
        DataLengthTooShort,
        HeadAndFoot,
        UnKnown
    }

    public class CacheModel
    {
        public byte[] CollectData;

        public int CompleteFlag;

        public int DataSign;

        public int DataPosition;

        public int SequenceNumber;


        public DateTime CreateTime { get; set; }
    }
}
