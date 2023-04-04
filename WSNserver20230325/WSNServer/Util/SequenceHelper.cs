using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSNServer.Util
{
    //维护上位机自己生成的序号类
    public static class SequenceHelper
    {

        public static byte random = CreateRandom();

        private static byte CreateRandom()
        {
            byte randomByte;
            Random random = new Random();
            randomByte = (byte)random.Next(0, 255);
            return randomByte;
        }

    }
}
