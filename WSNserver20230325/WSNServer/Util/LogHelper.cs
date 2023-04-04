using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WSNServer.Util
{
    public static class LogHelper
    {

        public static object CommonLogLock = new object();
        public static object ExceptionLogLock = new object();
        public static object SendMessageLogLock = new object();

        public static string CommonLogTemplate = "";
        public static string ExcepitonLogTemplate = "";
        public static string SendMessageLogTemplate = "";

        static LogHelper()
        {
            CommonLogTemplate += "**********************{0}**********************\r\n";
            CommonLogTemplate += "**********************{1}***************************\r\n";
            CommonLogTemplate += "{2}\r\n";
            CommonLogTemplate += "**********************以上是本次收到的原始数据包***************\r\n";
            CommonLogTemplate += "\r\n\r\n";


            ExcepitonLogTemplate += "**********************{0}**********************\r\n";
            ExcepitonLogTemplate += "引发异常的原始数据:{1}\r\n";
            ExcepitonLogTemplate += "异常信息:{2}\r\n";
            ExcepitonLogTemplate += "**********************以上是本次出现的异常信息***************\r\n";
            ExcepitonLogTemplate += "\r\n\r\n";



            SendMessageLogTemplate += "**********************{0}**********************\r\n";
            SendMessageLogTemplate += "发送的数据:{1}\r\n";
            SendMessageLogTemplate += "发送的IP和端口号:{2}\r\n";
            SendMessageLogTemplate += "**********************以上是本次发送的数据***************\r\n";
            SendMessageLogTemplate += "\r\n\r\n";

        }


        public static void SaveLog(DateTime saveTime, string endPoint, string data)
        {
            lock (CommonLogLock)
            {
                //保存日志的路径
                var saveDir = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";

                //如果目录不存在则创建
                if (!Directory.Exists(saveDir))
                {
                    Directory.CreateDirectory(saveDir);
                }

                var savePath = saveDir + "\\Logs.txt";
                //需要存入的字符串
                string saveStr = string.Format(CommonLogTemplate, saveTime.ToString("yyyy-MM-dd HH:mm:ss"), endPoint, data);

                using (var fs = new FileStream(savePath, FileMode.Append))
                {
                    using (var sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.Write(saveStr);
                    }
                }
            }

        }


        public static void SaveExceptionLog(DateTime saveTime,string rawData,string exceptionInfo)
        {
            lock (ExceptionLogLock)
            {
                //保存日志的路径
                var saveDir = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";

                //如果目录不存在则创建
                if (!Directory.Exists(saveDir))
                {
                    Directory.CreateDirectory(saveDir);
                }

                var savePath = saveDir + "\\ExceptionLogs.txt";
                //需要存入的字符串
                string saveStr = string.Format(ExcepitonLogTemplate, saveTime.ToString("yyyy-MM-dd HH:mm:ss"), rawData, exceptionInfo);

                using (var fs = new FileStream(savePath, FileMode.Append))
                {
                    using (var sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.Write(saveStr);
                    }
                }
            }
        
        }


        public static void SaveSendMessageLog(DateTime saveTime, string sendData, string endPoint)
        {
            lock (SendMessageLogLock)
            {
                //保存日志的路径
                var saveDir = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";

                //如果目录不存在则创建
                if (!Directory.Exists(saveDir))
                {
                    Directory.CreateDirectory(saveDir);
                }

                var savePath = saveDir + "\\SendMessageLogs.txt";
                //需要存入的字符串
                string saveStr = string.Format(SendMessageLogTemplate, saveTime.ToString("yyyy-MM-dd HH:mm:ss"), sendData, endPoint);

                using (var fs = new FileStream(savePath, FileMode.Append))
                {
                    using (var sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.Write(saveStr);
                    }
                }
            }

        }

    }
}
