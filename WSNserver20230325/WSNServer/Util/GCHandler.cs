using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSNServer.ServerExtensions;
using System.Threading;

namespace WSNServer.Util
{
    //封装内存垃圾回收机制
    public class GCHandler
    {
        public WSNSuperSocketServer AppServer
        {
            set;
            get;
        }
        public GCHandler(WSNSuperSocketServer appServer)
        {

            this.AppServer = appServer;

        }
        //垃圾回收进程
        public void GCHandle()
        {
            while (true)
            {
                lock (CacheMap.Lock)
                {

                    //15分钟之前插入的删除
                    var NeighborDataCache = (from item in CacheMap.NeighborDataCache where (DateTime.Now - item.Value.CreateTime) >= TimeSpan.FromMinutes(15) select item).ToList();
                    var RouteDataCache = (from item in CacheMap.RouteDataCache where (DateTime.Now - item.Value.CreateTime) >= TimeSpan.FromMinutes(15) select item).ToList();
                    var SensorDataCache = (from item in CacheMap.SensorDataCache where (DateTime.Now - item.Value.CreateTime) >= TimeSpan.FromMinutes(15) select item).ToList();
                    var ResendDataCache = (from item in CacheMap.ResendDataCache where (DateTime.Now - item.Value.CreateTime) >= TimeSpan.FromMinutes(15) select item).ToList();
                    var RawDataCache = (from item in CacheMap.RawDataCache where (DateTime.Now - item.Value.CreateTime) >= TimeSpan.FromMinutes(15) select item).ToList();
                    var CanLogCache = (from item in CacheMap.CanLogCache where (DateTime.Now - item.Value.CreateTime) >= TimeSpan.FromMinutes(15) select item).ToList();

                    //遍历集合并删除
                    int NeighborDataCacheCount = NeighborDataCache.Count();
                    for (int i = 0; i < NeighborDataCacheCount; i++)
                    {
                        WSNSession session = AppServer.GetSessionByID(NeighborDataCache[i].Key);
                        if (session == null)
                        {
                            CacheMap.NeighborDataCache.Remove(NeighborDataCache[i]);
                        }
                        else
                        {
                            if (session.Connected)
                            {
                                NeighborDataCache[i].Value.CreateTime = DateTime.Now;
                            }
                            else
                            {
                                CacheMap.NeighborDataCache.Remove(NeighborDataCache[i]);
                            }
                        }
                    }

                    int RouteDataCacheCount = RouteDataCache.Count();
                    for (int i = 0; i < RouteDataCacheCount; i++)
                    {
                        WSNSession session = AppServer.GetSessionByID(RouteDataCache[i].Key);
                        if (session == null)
                        {
                            CacheMap.RouteDataCache.Remove(RouteDataCache[i]);
                        }
                        else
                        {
                            if (session.Connected)
                            {
                                RouteDataCache[i].Value.CreateTime = DateTime.Now;
                            }
                            else
                            {
                                CacheMap.RouteDataCache.Remove(RouteDataCache[i]);
                            }
                        }
                    }

                    int SensorDataCacheCount = SensorDataCache.Count();
                    for (int i = 0; i < SensorDataCacheCount; i++)
                    {
                        WSNSession session = AppServer.GetSessionByID(SensorDataCache[i].Key);
                        if (session == null)
                        {
                            CacheMap.SensorDataCache.Remove(SensorDataCache[i]);
                        }
                        else
                        {
                            if (session.Connected)
                            {
                                SensorDataCache[i].Value.CreateTime = DateTime.Now;
                            }
                            else
                            {
                                CacheMap.SensorDataCache.Remove(SensorDataCache[i]);
                            }
                        }
                    }



                    int ResendDataCacheCount = ResendDataCache.Count();
                    for (int i = 0; i < ResendDataCacheCount; i++)
                    {
                        WSNSession session = AppServer.GetSessionByID(ResendDataCache[i].Key);
                        if (session == null)
                        {
                            CacheMap.ResendDataCache.Remove(ResendDataCache[i]);
                        }
                        else
                        {
                            if (session.Connected)
                            {
                                ResendDataCache[i].Value.CreateTime = DateTime.Now;
                            }
                            else
                            {
                                CacheMap.ResendDataCache.Remove(ResendDataCache[i]);
                            }
                        }
                    }



                    int RawDataCacheCount = RawDataCache.Count();
                    for (int i = 0; i < RawDataCacheCount; i++)
                    {
                        WSNSession session = AppServer.GetSessionByID(RawDataCache[i].Key);
                        if (session == null)
                        {
                            CacheMap.RawDataCache.Remove(RawDataCache[i]);
                        }
                        else
                        {
                            if (session.Connected)
                            {
                                RawDataCache[i].Value.CreateTime = DateTime.Now;
                            }
                            else
                            {
                                CacheMap.RawDataCache.Remove(RawDataCache[i]);
                            }
                        }
                    }

                    int CanLogCacheCount = CanLogCache.Count();
                    for (int i = 0; i < CanLogCacheCount; i++)
                    {
                        WSNSession session = AppServer.GetSessionByID(CanLogCache[i].Key);
                        if (session == null)
                        {
                            CacheMap.CanLogCache.Remove(CanLogCache[i]);
                        }
                        else
                        {
                            if (session.Connected)
                            {
                                CanLogCache[i].Value.CreateTime = DateTime.Now;
                            }
                            else
                            {
                                CacheMap.CanLogCache.Remove(CanLogCache[i]);
                            }
                        }
                    }

                }

                Thread.Sleep(1000 * 60 * 2);//间隔2分钟
            }

        }
    }
}
