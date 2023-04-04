using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SuperSocket.SocketBase;
using WSNServer.ServerExtensions;
using WSNServer.Util;
using WSNServer.WSNExceptions;
using System.Threading;


namespace WSNServer.Views
{
    public partial class MainView : Form, IUpdateView
    {

        //线程锁,用于多个不同线程的session同时更新ui界面时避免冲突,应使用该锁标记后，再更新界面
        public static object Lock = new object();

        //声明静态变量便于全局访问
        public static Form View = null;

        //服务实例
        public WSNSuperSocketServer appServer = new WSNSuperSocketServer();

        public MainView()
        {

            //该静态变量为当前的视图对象
            View = this;

            //构造组件
            InitializeComponent();

            //初始化服务监听
            InitServices();

            //一运行程序就启动服务器
            StartServices();


        }

        //增加一个ListView Message
        public void AddListViewMessage(ListViewMessageType type, string message)
        {
            int iconIndex = (int)type;
            lvMessage.BeginUpdate();
            if (lvMessage.Items.Count > 200)
            {
                lvMessage.Items.Clear();
            }
            var item = new ListViewItem();
            item.ImageIndex = iconIndex;
            item.SubItems.Add(((lvMessage.Items.Count + 1).ToString()));
            item.SubItems.Add(message);
            item.SubItems.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            lvMessage.Items.Add(item);
            lvMessage.EndUpdate();

        }


        //初始化服务监听端口
        private void InitServices()
        {
            //监听2020端口
            if (!appServer.Setup(2020))
            {
                //启动端口失败
                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "监听2020端口失败");
            }

        }

        //启动服务
        private void StartServices()
        {

            //尝试启动服务
            if (!appServer.Start())
            {
                //启动服务失败
                this.Text = "服务器打开失败";
            }
            else
            {
                //服务器启动成功
                this.Text = "服务器已经开启";
            }

            //注册新客户端新连接事件
            appServer.NewSessionConnected += new SessionHandler<WSNSession>(appServer_NewSessionConnected);

            //接收到未经处理的原始数据的事件
            ((IRawDataProcessor<WSNSession>)appServer).RawDataReceived += new Func<WSNSession, byte[], int, int, bool>(MainView_RawDataReceived);

            //注册客户端数据发送事件
            appServer.NewRequestReceived += new RequestHandler<WSNSession, SuperSocket.SocketBase.Protocol.BinaryRequestInfo>(appServer_NewRequestReceived);

            //注册客户端连接关闭的事件
            appServer.SessionClosed += new SessionHandler<WSNSession, SuperSocket.SocketBase.CloseReason>(appServer_SessionClosed);


            //启动垃圾回收机制,否则会造成内存泄漏 最后解决
            GCHandler handler = new GCHandler(appServer);
            Thread gc = new Thread(handler.GCHandle);
            gc.IsBackground = true;
            gc.Start();

        }





        bool MainView_RawDataReceived(WSNSession session, byte[] receive, int offset, int length)
        {
            if (!CacheMap.CanLogCache.ContainsKey(session.SessionID))
            {
                return false;
            }
            var canLog = CacheMap.CanLogCache[session.SessionID].Value;
            if (canLog) // rest=0时进入
            {
                //处理发送过来的原始数据
                var handleRawDataRequest = new HandleRawDataRequest(session, receive, offset, length);
                var isContainHeadAndFoot = handleRawDataRequest.Handle();
                return isContainHeadAndFoot;
            }



            //返回true代表需要继续处理该原始数据返回false则停止处理该原始数据
            return true;
        }



        void appServer_NewRequestReceived(WSNSession session, SuperSocket.SocketBase.Protocol.BinaryRequestInfo requestInfo)
        { //新建线程处理客户端请求

            //这里本身就是非UI线程，如果要更新UI 需要使用线程安全的回掉         

            var neighborDataCache = CacheMap.NeighborDataCache[session.SessionID];//邻居表数据缓冲区
            var routeDataCache = CacheMap.RouteDataCache[session.SessionID];//路由表数据缓冲区
            var sensorDataCache = CacheMap.SensorDataCache[session.SessionID];  //采集数据缓冲区
            var LaiDataCache = CacheMap.LaiDataCache[session.SessionID];        //Lai数据缓冲区
            var resendDataCache = CacheMap.ResendDataCache[session.SessionID];  //重发数据缓冲区

            HandleRequest handle = new HandleRequest(session, requestInfo, neighborDataCache, routeDataCache, sensorDataCache, LaiDataCache, resendDataCache);

            handle.Handle();
            //这里不应使用多线程，否则会影包的解析顺序
            //Thread thread = new Thread(handle.Handle);
            //thread.Start();

        }




        void appServer_NewSessionConnected(WSNSession session)
        {
            //客户端连接一次，就新创建一个相应的数据缓冲区，防止不同的客户端共用缓存区，导致错误
            lock (CacheMap.Lock)
            {
                CacheMap.NeighborDataCache.Add(session.SessionID, new CacheCollection(DateTime.Now));
                CacheMap.RouteDataCache.Add(session.SessionID, new CacheCollection(DateTime.Now));
                CacheMap.SensorDataCache.Add(session.SessionID, new CacheCollection(DateTime.Now));
                CacheMap.LaiDataCache.Add(session.SessionID, new CacheCollection(DateTime.Now));
                CacheMap.ResendDataCache.Add(session.SessionID, new ResendCacheCollection(DateTime.Now));
                CacheMap.RawDataCache.Add(session.SessionID, new RawDataCache(DateTime.Now, string.Empty));
                CacheMap.CanLogCache.Add(session.SessionID, new CanLogCacheModel(DateTime.Now, true));
            }



            string sessionID = session.SessionID;
            string ipAndPort = session.RemoteEndPoint.ToString();


            //向主界面发送添加tabPage的更新界面请求
            var updateParams = new UpdateParams();
            updateParams.UpdateType = UpdateType.AddTabPage;
            updateParams.Message = string.Join(",", new string[] { sessionID, ipAndPort });
            ViewUpdateHelper.UpdateView(updateParams);

            //添加一个客户端连接的消息
            ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, string.Format("{0}已经连接", ipAndPort));

        }

        void appServer_SessionClosed(WSNSession session, SuperSocket.SocketBase.CloseReason value)
        {

            //客户端断开连接时删除对应的tabPage
            var updateParams = new UpdateParams();
            updateParams.UpdateType = UpdateType.RemoveTabPage;
            updateParams.Message = session.SessionID;
            ViewUpdateHelper.UpdateView(updateParams);

            var x16 = CacheMap.RawDataCache[session.SessionID].Value;
            //记录日志
            LogHelper.SaveLog(DateTime.Now, session.RemoteEndPoint.ToString(), x16);

            //回收数据缓冲区(客户端如果断开了，就把这个里面对应的缓存区清空,避免内存泄露)//不能删除此处如果删除则会造成消息解析不全,应由系统自动进行回收
            //session关闭之后session对象并不会被立即回收
            //CacheMap.NeighborDataCache.Remove(session.SessionID);
            //CacheMap.RouteDataCache.Remove(session.SessionID);
            //CacheMap.SensorDataCache.Remove(session.SessionID);
            //CacheMap.ResendDataCache.Remove(session.SessionID);
            //CacheMap.RawDataCache.Remove(session.SessionID);
            //CacheMap.CanLogCache.Remove(session.SessionID);


            //添加一个客户端连接的消息
            ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, string.Format("{0}已经下线", session.RemoteEndPoint.ToString()));



        }


        public void AddTabPage(string sessionID, string ipAndPort)
        {

            //TabPage
            TabPage tabPage = new TabPage();
            tabPage.Location = new System.Drawing.Point(4, 22);
            tabPage.Name = "tabPage" + sessionID.Replace("-", "");
            tabPage.Padding = new System.Windows.Forms.Padding(3);
            tabPage.Size = new System.Drawing.Size(661, 174);
            tabPage.TabIndex = 0;
            tabPage.Text = ipAndPort;
            tabPage.UseVisualStyleBackColor = true;
            tabPage.SuspendLayout();

            //Text
            TextBox textBox = new TextBox();
            textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            textBox.Location = new System.Drawing.Point(3, 3);
            textBox.Multiline = true;
            textBox.Name = "txtData" + sessionID.Replace("-", "");
            textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            textBox.Size = new System.Drawing.Size(655, 168);
            textBox.TabIndex = 0;
            textBox.SuspendLayout();


            tabPage.Controls.Add(textBox);
            tabMain.TabPages.Add(tabPage);
            tabMain.SelectTab(tabPage);


        }


        public void RemoveTabPage(string sessionID)
        {
            var tabPage = (TabPage)tabMain.Controls["tabPage" + sessionID.Replace("-", "")];
            if (tabMain.SelectedTab != tabPage)
            {
                tabMain.Controls.Remove(tabPage);
            }
            else
            {
                //选择前一个或后一个tabPage
                int preSelectIndex = tabMain.SelectedIndex - 1;
                int nextSelectIndex = tabMain.SelectedIndex + 1;

                if (preSelectIndex >= 0)
                {
                    tabMain.SelectTab(preSelectIndex);
                }
                else
                {
                    if (nextSelectIndex <= (tabMain.TabCount - 1))
                    {
                        tabMain.SelectTab(nextSelectIndex);
                    }
                }

                tabMain.Controls.Remove(tabPage);
            }
        }


        public void UpdateTabPageData(string sessionID, string message)
        {
            var tabPage = (TabPage)tabMain.Controls["tabPage" + sessionID.Replace("-", "")];
            if (tabPage != null)
            {
                var txtData = (TextBox)tabPage.Controls["txtData" + sessionID.Replace("-", "")];
                var x16 = message;
                txtData.Text += x16;
                tabMain.SelectTab(tabPage);
            }
        }

        private void StopServices()
        {
            //注销事件
            appServer.NewSessionConnected -= appServer_NewSessionConnected;
            ((IRawDataProcessor<WSNSession>)appServer).RawDataReceived -= MainView_RawDataReceived;
            appServer.NewRequestReceived -= appServer_NewRequestReceived;
            appServer.SessionClosed -= appServer_SessionClosed;

            //停止服务
            appServer.Stop();
        }

        public void UpdateView(UpdateParams updateParams)
        {
            switch (updateParams.UpdateType)
            {
                case UpdateType.AddTabPage:
                    //获取参数
                    string[] addTabPageParams = updateParams.Message.ToString().Split(',');
                    //调用更新界面方法
                    AddTabPage(addTabPageParams[0], addTabPageParams[1]);
                    break;
                case UpdateType.UpdateTabPageData:

                    string[] updateTabPageDataParams = updateParams.Message.ToString().Split(',');
                    //调用更新对应tabPage中的数据的方法
                    UpdateTabPageData(updateTabPageDataParams[0], updateTabPageDataParams[1]);
                    break;

                case UpdateType.RemoveTabPage:
                    //调用方法移除对应的tabPage
                    string removeTabPageParams = updateParams.Message.ToString();
                    RemoveTabPage(removeTabPageParams);
                    break;

                //添加一条消息到ListView
                case UpdateType.AddListViewMessage:
                    var addListViewMessageParams = (ListViewMessage)updateParams.Message;
                    //调用添加消息的方法
                    AddListViewMessage(addListViewMessageParams.Type, addListViewMessageParams.Message);
                    break;


                default:
                    break;
            }

        }

        //private void btnRefresh_Click(object sender, EventArgs e)
        //{
        //    lvMessage..Items.Refresh();
        //}

        private void btnClear_Click(object sender, EventArgs e)
        {
            lvMessage.Items.Clear();
        }
    }

}
