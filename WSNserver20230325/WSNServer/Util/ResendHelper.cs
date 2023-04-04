using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSNServer.Util
{
    public class ResendHelper
    {
        //给缓冲区添加一帧回发错误的数据
        public static void AddResendData(byte[] data, ResendType type, string sessionID)
        {
            //将重发的数据添加入重发数据缓存
            var resendCacheModel = new ResendCacheModel();
            resendCacheModel.Data = data;
            resendCacheModel.Type = type;
            resendCacheModel.SequenceNumber = -1;
            CacheMap.ResendDataCache[sessionID].Add(resendCacheModel);
        }
        //给缓冲区添加一帧回发错误的数据（带序列号）
        public static void AddResendData(byte[] data, ResendType type, string sessionID, byte frameSequenceNumber)
        {
            //将重发的数据添加入重发数据缓存
            var resendCacheModel = new ResendCacheModel();
            resendCacheModel.Data = data;
            resendCacheModel.Type = type;
            resendCacheModel.SequenceNumber = (int)frameSequenceNumber;
            CacheMap.ResendDataCache[sessionID].Add(resendCacheModel);
        }
        //获取当前重发出现错误的帧的次数
        public static int GetCurrentResendCount(string sessionID)
        {
            //已经重发的次数
            var hasResendCount = (from item in CacheMap.ResendDataCache[sessionID]
                                  select item).Count();
            return hasResendCount;
        }

        //重置重发缓存区
        public static void ResetResendCacheCount(string sessionID)
        {
            CacheMap.ResendDataCache[sessionID].Clear();
        }
    }
}
