using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSNServer.WSNExceptions;

namespace WSNServer.Util
{
    //数据帧
    public class DataFrame
    {
        public byte[] DataOptions = new byte[2];
        public byte Length;
        public byte[] Data;
    }



}
