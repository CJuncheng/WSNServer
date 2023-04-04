using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSNServer.Util
{
    //共有的方法
    public class CommonHelper
    {
        /// <summary>
        /// fcs数组的异或校验方法.<br></br>
        /// </summary>
        /// <param name="forCheckBytes"></param>
        /// <returns></returns>
        public static byte XorCheckForBytes(byte[] forCheckBytes)
        {
            byte result = (byte)0;
            for (int index = 0; index < forCheckBytes.Length; index++)
            {
                result ^= (byte)forCheckBytes[index];

            }
            return result;
        }
    }
}
