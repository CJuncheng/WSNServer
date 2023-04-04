using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSNServer.Util
{
    public class CacheCollection : List<CacheModel>
    {
        public DateTime CreateTime
        {
            set;
            get;
        }

        public CacheCollection(DateTime createTime)
        {
            this.CreateTime = createTime;
        }

    }

    public class ResendCacheCollection : List<ResendCacheModel>
    {
        public DateTime CreateTime
        {
            set;
            get;
        }

        public ResendCacheCollection(DateTime createTime)
        {
            this.CreateTime = createTime;
        }

    }
    
}
