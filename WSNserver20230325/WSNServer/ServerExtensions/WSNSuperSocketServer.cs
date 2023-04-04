using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace WSNServer.ServerExtensions
{
    public class WSNSuperSocketServer : AppServer<WSNSession, BinaryRequestInfo>
    {
        public WSNSuperSocketServer()
            : base(new WSNReceiveFilterFactory()) //使用默认的接受过滤器工厂 (DefaultReceiveFilterFactory)
        {


        }

    }
}
