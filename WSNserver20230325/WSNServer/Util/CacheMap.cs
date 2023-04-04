using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSNServer.Util
{
    public class CacheMap
    {
        public static object Lock = new object();
        public static IDictionary<string, CacheCollection> NeighborDataCache = new Dictionary<string, CacheCollection>();
        public static IDictionary<string, CacheCollection> RouteDataCache = new Dictionary<string, CacheCollection>();
        public static IDictionary<string, CacheCollection> SensorDataCache = new Dictionary<string, CacheCollection>();
        public static IDictionary<string, CacheCollection> LaiDataCache = new Dictionary<string, CacheCollection>();
        public static IDictionary<string, ResendCacheCollection> ResendDataCache = new Dictionary<string, ResendCacheCollection>();
        public static IDictionary<string, RawDataCache> RawDataCache = new Dictionary<string, RawDataCache>();
        public static IDictionary<string, CanLogCacheModel> CanLogCache = new Dictionary<string, CanLogCacheModel>();

    }

    public class RawDataCache
    {

        public string Value
        {
            set;
            get;
        }
        public DateTime CreateTime
        {
            set;
            get;
        }
        public RawDataCache(DateTime createTime, string value)
        {
            this.Value = value;
            this.CreateTime = createTime;
        }

    }

    public class CanLogCacheModel
    {

        public bool Value
        {
            set;
            get;
        }
        public DateTime CreateTime
        {
            set;
            get;
        }
        public CanLogCacheModel(DateTime createTime, bool value)
        {
            this.Value = value;
            this.CreateTime = createTime;
        }

    }

}
