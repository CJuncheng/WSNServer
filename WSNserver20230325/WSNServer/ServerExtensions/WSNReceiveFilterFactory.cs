using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase.Protocol;
using System.Windows.Forms;

namespace WSNServer.ServerExtensions
{
    public class WSNReceiveFilterFactory : DefaultReceiveFilterFactory<WSNReceiveFilter, BinaryRequestInfo>
    {

        public override IReceiveFilter<BinaryRequestInfo> CreateFilter(SuperSocket.SocketBase.IAppServer appServer, SuperSocket.SocketBase.IAppSession appSession, System.Net.IPEndPoint remoteEndPoint)
        {
            var filter = (WSNReceiveFilter)base.CreateFilter(appServer, appSession, remoteEndPoint);
            filter.Session = (WSNSession)appSession;
            return filter;
        }
    }
}
