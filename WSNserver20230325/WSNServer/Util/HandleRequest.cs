using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSNServer.ServerExtensions;
using SuperSocket.SocketBase.Protocol;
using System.Windows.Forms;
using System.Threading;
using WSNServer.WSNExceptions;
using System.Diagnostics;
using System.Data.Objects;
using System.Data.Common;
using System.Data;
using WSNServer.DB;

namespace WSNServer.Util
{
    //处理客户端请求的类
    public class HandleRequest
    {

        public static object DBLock = new object();

        public WSNSession Session
        {
            set;
            get;
        }
        public BinaryRequestInfo Data
        {
            set;
            get;
        }


        //邻居表数据缓存
        public List<CacheModel> NeighborDataCache
        {
            set;
            get;
        }

        //路由表数据缓存
        public List<CacheModel> RouteDataCache
        {
            set;
            get;
        }
        //采集数据缓存
        public List<CacheModel> SensorDataCache
        {
            set;
            get;
        }

        //Lai数据缓存
         public List<CacheModel> LaiDataCache
        {
            set;
            get;
        }
        //重发数据缓存
        public List<ResendCacheModel> ResendDataCache
        {
            set;
            get;
        }

        public HandleRequest(WSNSession session, BinaryRequestInfo data, List<CacheModel> neighborDataCache, List<CacheModel> routeDataCache, List<CacheModel> sensorDataCache, List<CacheModel> LaiDataCache, List<ResendCacheModel> resendDataCache)
        {
            this.Session = session;
            this.Data = data;
            this.SensorDataCache = sensorDataCache;
            this.NeighborDataCache = neighborDataCache;
            this.RouteDataCache = routeDataCache;
            this.LaiDataCache=LaiDataCache;
            this.ResendDataCache = resendDataCache;

        }
    


        public void Handle()
        {

            //收到数据 检验和解析
            try
            {
                ReceiveDataStyle data = ParseHelper.ParseAll(Data.Body);

                //重发错误次数归零
                ResendHelper.ResetResendCacheCount(Session.SessionID);

                var control = data.FrameControl;
                var payLoad = data.FramePayload;

                #region   命令帧

                //命令帧
                if ((control == (byte)FrameControlType.Cmd))
                {
                    //汇聚节点发送联网命令
                    if ((payLoad.Length == 1) &&
                    (payLoad[0] == (byte)CmdType.CmdConnectLine))
                    {

                        ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "汇聚节点联网成功");
                        //构建联网成功回应帧
                        var ack = BuildHelper.BuildCmdRightResponAck();

                        //如果汇聚节点仍在线
                        if (Session.Connected)
                        {
                            //给汇聚节点回应
                            Session.Send(ack, 0, ack.Length);
                            ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "已发送联网命令确认帧");

                            //记录上位机给汇聚节点发送联网命令应答帧
                            var x16 = ParseHelper.ShowX16(ack);
                            LogHelper.SaveSendMessageLog(DateTime.Now, x16, Session.RemoteEndPoint.ToString());
                        }
                        else
                        {

                            ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "联网命令确认帧发送失败！无线传感网络断开连接!");
                        }


                        //1秒后执行给下位机发送要求上传数据的命令
                        Thread.Sleep(1000);

                        //构建要求汇聚节点上传数据的帧
                        var uploadDataCmd = BuildHelper.BuildDefaultUploadDataCmd();

                        //如果下位机仍在线
                        if (Session.Connected)
                        {
                            //发送命令包
                            Session.Send(uploadDataCmd, 0, uploadDataCmd.Length);
                            ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "已发送默认数据上传参数：所有参数均设置为全选。");

                            //记录上位机给汇聚节点发送要求上传默认数据的命令帧
                            var x16 = ParseHelper.ShowX16(uploadDataCmd);
                            LogHelper.SaveSendMessageLog(DateTime.Now, x16, Session.RemoteEndPoint.ToString());
                        }
                        else
                        {

                            ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "要求上传数据的命令帧发送失败！无线传感网络断开连接!");
                        }

                    }


                    //汇聚节点请求上位机发送休眠指令
                    if ((payLoad.Length == 2) &&
                    (payLoad[0] == (byte)CmdType.CmdSleepRequest))
                    {
                        ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "汇聚节点请求上位机发送休眠参数");

                        //构建上位机收到汇聚节点请求休眠的指令的回应帧
                        var sleepRequestAck = BuildHelper.BuildCmdRightResponAck();

                        //如果汇聚节点仍在线
                        if (Session.Connected)
                        {
                            //给汇聚节点回应
                            Session.Send(sleepRequestAck, 0, sleepRequestAck.Length);
                            ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "已发送收到汇聚节点请求休眠的确认帧");

                            //记录上位机给汇聚节点发送休眠命令无错应答帧
                            var x16 = ParseHelper.ShowX16(sleepRequestAck);
                            LogHelper.SaveSendMessageLog(DateTime.Now, x16, Session.RemoteEndPoint.ToString());
                        }
                        else
                        {

                            ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "休眠命令确认帧发送失败！无线传感网络断开连接!");
                        }
                        //0.5秒后执行给汇聚节点发送休眠参数
                        Thread.Sleep(500);
                        byte CityID = payLoad[1];


                        /* Juncheng Chen, 20230223
                         * 将服务器向汇聚节点下发拍照的指令和下发休眠参数指令放一起
                         */
                        //默认的休眠命令的数据包
                        var sleepCmd = BuildHelper.BuildSleepCmdAndTakingPhotoCmd(CityID);

                        //如果下位机仍在线
                        if (Session.Connected)
                        {
                            //发送命令包
                            Session.Send(sleepCmd, 0, sleepCmd.Length);
                            ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "已发送默认的休眠参数及对应的拍照指令"); /*20230223修改*/

                            //记录上位机给汇聚节点下达的休眠指令
                            var x16 = ParseHelper.ShowX16(sleepCmd);
                            LogHelper.SaveSendMessageLog(DateTime.Now, x16, Session.RemoteEndPoint.ToString());
                        }
                        else
                        {

                            ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "上位机发送休眠参数及对应的拍照指令失败！无线传感网络断开连接!");  /*20230223修改*/
                        }
                    }



                }
                #endregion

                #region  应答帧
                //汇聚节点发送数据传完的命令
                if ((payLoad.Length == 1) &&
                (payLoad[0] == (byte)AckOption.CmdRight))
                {

                    ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "汇聚节点数据已传完应答帧已发送");
                }
                #endregion
                

                #region  图片回应帧

                //图片回应帧

                //汇聚节点发送数据传完的命令
                if ((control == (byte)FrameControlType.JpgState))
                {
                    byte CityID = payLoad[1];

                    if (payLoad[0] == (byte)FTPflag.False)
                    {
                        //图片发送失败

                        //默认的休眠命令的数据包
                        var sleepCmd = BuildHelper.BuildSleepCmdAndTakingPhotoCmd(CityID);

                        //如果下位机仍在线
                        if (Session.Connected)
                        {
                            //发送命令包
                            Session.Send(sleepCmd, 0, sleepCmd.Length);
                            ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "已再次发送默认的休眠参数及对应的拍照指令！");

                            //记录上位机给汇聚节点下达的休眠指令
                            var x16 = ParseHelper.ShowX16(sleepCmd);
                            LogHelper.SaveSendMessageLog(DateTime.Now, x16, Session.RemoteEndPoint.ToString());
                        }
                        else
                        {
                            ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "上位机发送休眠参数及对应的拍照指令失败！无线传感网络断开连接！");
                        }

                    }
                    else if (payLoad[0] == (byte)FTPflag.Scuccess)
                    {
                        //图片上传成功
                        ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "上位机成功收到上传的图片！");
                        
                        using (var db = new DB.WirelessSensorNetworkEntities())
                        {
                          
                            WSNTimeSetTable timesettable = db.WSNTimeSetTables.Where(m => m.CityID == CityID).FirstOrDefault();
                            timesettable.NodeID = 0;
                            db.SaveChanges();
                        }
                        using (var db = new DB.WirelessSensorNetworkEntities())
                        {

                            int nodeid = (int)(payLoad[2] * 256 + payLoad[3]);

                            var bytesTime = new byte[7];

                            //采集时间bytes
                            Array.Copy(payLoad, 4, bytesTime, 0, 7);

                            var strTime = string.Format("{0}{1}-{2}-{3} {4}:{5}:{6}",
                                (((bytesTime[0] & 0xF0) >> 4).ToString() + (bytesTime[0] & 0x0F).ToString()),
                                (((bytesTime[1] & 0xF0) >> 4).ToString() + (bytesTime[1] & 0x0F).ToString()),
                                (((bytesTime[2] & 0xF0) >> 4).ToString() + (bytesTime[2] & 0x0F).ToString()),
                                (((bytesTime[3] & 0xF0) >> 4).ToString() + (bytesTime[3] & 0x0F).ToString()),
                                (((bytesTime[4] & 0xF0) >> 4).ToString() + (bytesTime[4] & 0x0F).ToString()),
                                (((bytesTime[5] & 0xF0) >> 4).ToString() + (bytesTime[5] & 0x0F).ToString()),
                                (((bytesTime[6] & 0xF0) >> 4).ToString() + (bytesTime[6] & 0x0F).ToString()));
                            DateTime uploadtime = Convert.ToDateTime(strTime);
                            var timeString = string.Format("{0}{1}_{2}_{3}_{4}_{5}_{6}",
                                (((bytesTime[0] & 0xF0) >> 4).ToString() + (bytesTime[0] & 0x0F).ToString()),
                                (((bytesTime[1] & 0xF0) >> 4).ToString() + (bytesTime[1] & 0x0F).ToString()),
                                (((bytesTime[2] & 0xF0) >> 4).ToString() + (bytesTime[2] & 0x0F).ToString()),
                                (((bytesTime[3] & 0xF0) >> 4).ToString() + (bytesTime[3] & 0x0F).ToString()),
                                (((bytesTime[4] & 0xF0) >> 4).ToString() + (bytesTime[4] & 0x0F).ToString()),
                                (((bytesTime[5] & 0xF0) >> 4).ToString() + (bytesTime[5] & 0x0F).ToString()),
                                (((bytesTime[6] & 0xF0) >> 4).ToString() + (bytesTime[6] & 0x0F).ToString()));
                            var dbItemImagesPath = new DB.WSNImagesPath();
                            dbItemImagesPath.CityID = CityID;
                            dbItemImagesPath.NodeAddress=nodeid.ToString("X4");
                            var imageName = string.Format("{0}-{1}-{2}", CityID.ToString("X2"), nodeid.ToString("x4"), timeString);
                            dbItemImagesPath.ImageName = imageName;
                            dbItemImagesPath.UploadDate = uploadtime;
                            db.AddToWSNImagesPaths(dbItemImagesPath);
                            db.SaveChanges();
                        }

                    }
                   
                }

                #endregion

                #region  数据帧

                //数据帧
                if ((control == (byte)FrameControlType.Data))
                {

                    //将payload封装为数据帧
                    var dataFrame = ParseHelper.ParseDataFrame(payLoad);

                    var dataType = dataFrame.DataOptions[0];

                    //数据标识（暂时未用，以后可能用于补充重发逻辑）。
                    var dataFlag = dataFrame.DataOptions[1];

                    //是否传完标识 zry:dataFlag & 11000000 保留高二位的传完标志，00表示传完，01表示未完
                    var completeFlag = ((dataFlag & 0xC0) >> 6);

                    //是否是重发数据的标识 zry:dataFlag & 00110000 保留5、6位的数据标志，00表示第一次上传数据，01表示重传的数据
                    var dataSign = ((dataFlag & 0x30) >> 4);

                    //帧位置标志 zry:dataFlag & 00001111 保留低四位的帧位置标志
                    var dataPosition = ((dataFlag & 0x0F));


                    #region 数据帧中的邻居表

                    //邻居表
                    if (dataType == (byte)DataType.NeighborTable)
                    {
                        //邻居表数据未传完
                        if (completeFlag == 01)
                        {

                            //构建数据无错应答帧
                            var neighborAck = BuildHelper.BuildNoErrorDataAck();
                            //如果汇聚节点仍在线
                            if (Session.Connected)
                            {
                                //给汇聚节点回应
                                Session.Send(neighborAck, 0, neighborAck.Length);
                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "邻居表数据未传完，数据无错应答已发送，等待数据中……");

                                //记录上位机给汇聚节点发送邻居表数据未传完应答帧
                                var x16 = ParseHelper.ShowX16(neighborAck);
                                LogHelper.SaveSendMessageLog(DateTime.Now, x16, Session.RemoteEndPoint.ToString());

                            }
                            else
                            {
                                //状态更新——未编程
                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "邻居表数据未传完，网络错误或汇聚节点已断开！");
                            }


                            //添加到缓存
                            var cache = new CacheModel();
                            cache.CollectData = dataFrame.Data;
                            cache.CompleteFlag = completeFlag;
                            cache.CreateTime = DateTime.Now;
                            cache.DataPosition = dataPosition;
                            cache.DataSign = dataSign;
                            cache.SequenceNumber = (int)data.FrameSequenceNumber;
                            NeighborDataCache.Add(cache);

                        }
                        //邻居表数据已传完
                        if (completeFlag == 00)
                        {

                            //构建数据无错应答帧
                            var neighborAck = BuildHelper.BuildNoErrorDataAck();
                            //如果汇聚节点仍在线
                            if (Session.Connected)
                            {
                                //给汇聚节点回应
                                Session.Send(neighborAck, 0, neighborAck.Length);
                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "邻居表数据已传完，数据无错应答已发送");

                                //记录上位机给汇聚节点发送邻居表数据已传完应答帧
                                var x16 = ParseHelper.ShowX16(neighborAck);
                                LogHelper.SaveSendMessageLog(DateTime.Now, x16, Session.RemoteEndPoint.ToString());
                            }
                            else
                            {
                                //状态更新——未编程
                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "邻居表数据已传完，网络错误或汇聚节点已断开！");
                            }


                            //添加到缓存区
                            var cache = new CacheModel();
                            cache.CollectData = dataFrame.Data;
                            cache.CompleteFlag = completeFlag;
                            cache.CreateTime = DateTime.Now;
                            cache.DataPosition = dataPosition;
                            cache.DataSign = dataSign;
                            cache.SequenceNumber = (int)data.FrameSequenceNumber;
                            NeighborDataCache.Add(cache);


                            var prepareParseData = new List<byte>();
                            //对缓存区的每帧数据按帧标识位排序 ,如果出现重发帧则使用重发帧,否则使用非重发帧
                            var handleResend = from item in NeighborDataCache
                                               group item by item.DataPosition into defaultresendgroup
                                               select
                                               (from defaultresenditem in defaultresendgroup
                                                orderby defaultresenditem.DataSign descending
                                                orderby defaultresenditem.CreateTime descending
                                                select defaultresenditem)
                                               .FirstOrDefault();


                            //添加到预解析区
                            var cacheList = from item in handleResend
                                            orderby item.DataPosition descending
                                            select item;

                            foreach (var item in cacheList)
                            {
                                prepareParseData.AddRange(item.CollectData);
                            }

                            //解析
                            var dataList = ParseHelper.ParseNeighborRealData(prepareParseData.ToArray());

                            //逐条入库
                            foreach (var item in dataList)
                            {
                                var x16 = ParseHelper.ShowX16(item.Data);

                                //Debug.WriteLine(string.Format("解析出的邻居表数据为: {0}", x16));//调试窗口显示解析出的数据

                                var bytesTime = new byte[7];

                                var contentData = new byte[item.Data.Length - 2 - 7];
                                //时间bytes
                                Array.Copy(item.Data, 2, bytesTime, 0, 7);
                                //数据bytes
                                Array.Copy(item.Data, 9, contentData, 0, item.Data.Length - 2 - 7);

                                var strTime = string.Format("{0}{1}-{2}-{3} {4}:{5}:{6}",
                                  (int)bytesTime[0],
                                  (int)bytesTime[1],
                                  (int)bytesTime[2],
                                  (int)bytesTime[3],
                                  (int)bytesTime[4],
                                  (int)bytesTime[5],
                                  (int)bytesTime[6]);
                                var strDay = string.Format("{0}{1}-{2}-{3}",
                                  (int)bytesTime[0],
                                  (int)bytesTime[1],
                                  (int)bytesTime[2],
                                  (int)bytesTime[3]);
                                string strLimitTime1 = strDay + " 5:30:00";
                                string strLimitTime2 = strDay + " 18:30:00";
                                //string strLimitTime3 = strDay + " 18:00:00";
                                //string strLimitTime4 = strDay + " 20:00:00";
                                DateTime LimitTime1 = Convert.ToDateTime(strLimitTime1);
                                DateTime LimitTime2 = Convert.ToDateTime(strLimitTime2);
                                //DateTime LimitTime3 = Convert.ToDateTime(strLimitTime3);
                                //DateTime LimitTime4 = Convert.ToDateTime(strLimitTime4);
                                var t = Convert.ToDateTime(strTime);
                                bool flog = ((DateTime.Compare(t, LimitTime1) >= 0) && (DateTime.Compare(t, LimitTime2) <= 0));
                                flog = true;
                                if (flog)
                                {
                                    //计算总共多少对父子节点
                                    int count = (contentData.Length / 4);

                                    //根据解析出的数据判断属于哪个城市,如果返回-1则说明未找到该数据所属城市则不插入数据
                                    var CityID = CityHelper.GetNeighborCityIDByData(contentData);

                                    if (CityID != -1)
                                    {

                                        using (var db = new DB.WirelessSensorNetworkEntities())
                                        {
                                            var time = Convert.ToDateTime(strTime);

                                            Debug.WriteLine("邻居表时间:" + time.ToString("yyyy-MM-dd HH:mm:ss"));

                                            //初始化绘图表节点数组
                                            var drawTopologyArray = new List<string>();


                                            for (int i = 0; i < count; i++)
                                            {
                                                var childNode = new byte[] { contentData[i * 4], contentData[i * 4 + 1] };
                                                var fatherNode = new byte[] { contentData[i * 4 + 2], contentData[i * 4 + 3] };

                                                var strChildNode = ParseHelper.ShowX16(childNode).Replace(" ", "");
                                                var strFatherNode = ParseHelper.ShowX16(fatherNode).Replace(" ", "");

                                                Debug.WriteLine("解析到的子父节点为:" + strChildNode + "_" + strFatherNode);

                                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "邻居表时间:" + time.ToString("yyyy-MM-dd HH:mm:ss") + "解析到的子父节点为:" + strChildNode + "_" + strFatherNode);
                                                var dbItemNeighborTable = new DB.WSNNeighborTable();
                                                dbItemNeighborTable.ChildNode = strChildNode;
                                                dbItemNeighborTable.FatherNode = strFatherNode;
                                                dbItemNeighborTable.UploadTime = time;

                                                //需要计算城市id
                                                dbItemNeighborTable.CityID = CityID;
                                                db.AddToWSNNeighborTables(dbItemNeighborTable);

                                                drawTopologyArray.Add(string.Format("{0}{1}", strFatherNode, strChildNode));

                                            }


                                            var dbItemDrawTopologyTable = new DB.WSNDrawTopologyTable();
                                            dbItemDrawTopologyTable.UploadTime = time;
                                            dbItemDrawTopologyTable.CityID = CityID;

                                            dbItemDrawTopologyTable.TopologyString = string.Join("*", drawTopologyArray.ToArray());
                                            db.AddToWSNDrawTopologyTables(dbItemDrawTopologyTable);

                                            db.SaveChanges();
                                        }

                                    }
                                    else
                                    {
                                        //状态更新——未编程
                                        ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "此次上传的邻居表数据不属于任何一个监测地方");
                                    }

                                    ////更新界面
                                    //this.View.Invoke(new UICallBack(() =>
                                    //{
                                    //    var upateParams = new UpdateParams();
                                    //    upateParams.Message = x16 + " ";//邻居表数据显示在页面窗口上数据显示的格式
                                    //    ((IUpdateView)this.View).UpdateView(upateParams);
                                    //}));
                                }

                            }
                            //移除缓冲区
                            NeighborDataCache.Clear();


                        }
                    }
                    #endregion


                    #region 数据帧中的路由表
                    //路由表
                    if (dataType == (byte)DataType.RoutingTable)
                    {
                        //路由表数据未传完
                        if (completeFlag == 01)
                        {

                            //构建数据无错应答帧
                            var routeAck = BuildHelper.BuildNoErrorDataAck();
                            //如果汇聚节点仍在线
                            if (Session.Connected)
                            {
                                //给汇聚节点回应
                                Session.Send(routeAck, 0, routeAck.Length);
                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "路由表数据未传完，数据无错应答已发送，等待数据中……");


                                //记录上位机给汇聚节点发送路由表数据未传完应答帧
                                var x16 = ParseHelper.ShowX16(routeAck);
                                LogHelper.SaveSendMessageLog(DateTime.Now, x16, Session.RemoteEndPoint.ToString());
                            }
                            else
                            {
                                //状态更新——未编程
                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "路由表数据未传完，网络错误或汇聚节点已断开！");
                            }


                            //添加到路由表数据缓存
                            var cache = new CacheModel();
                            cache.CollectData = dataFrame.Data;
                            cache.CompleteFlag = completeFlag;
                            cache.CreateTime = DateTime.Now;
                            cache.DataPosition = dataPosition;
                            cache.DataSign = dataSign;
                            cache.SequenceNumber = (int)data.FrameSequenceNumber;
                            RouteDataCache.Add(cache);

                        }
                        //路由表数据已传完
                        if (completeFlag == 00)
                        {
                            //构建数据无错应答帧
                            var routeAck = BuildHelper.BuildNoErrorDataAck();
                            //如果汇聚节点仍在线
                            if (Session.Connected)
                            {
                                //给汇聚节点回应
                                Session.Send(routeAck, 0, routeAck.Length);
                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "路由表数据已传完，数据无错应答已发送");


                                //记录上位机给汇聚节点发送路由表数据已传完应答帧
                                var x16 = ParseHelper.ShowX16(routeAck);
                                LogHelper.SaveSendMessageLog(DateTime.Now, x16, Session.RemoteEndPoint.ToString());
                            }
                            else
                            {
                                //状态更新——未编程
                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "路由表数据已传完，网络错误或汇聚节点已断开！");
                            }

                            //添加到路由表数据缓存区
                            var cache = new CacheModel();
                            cache.CollectData = dataFrame.Data;//数据帧中的data部分（参照通信协议）
                            cache.CompleteFlag = completeFlag;//传完与否的标志
                            cache.CreateTime = DateTime.Now;//防止出错缓冲区存在不止一条的未传完数据
                            cache.DataPosition = dataPosition;//帧位置标志
                            cache.DataSign = dataSign;//数据标志（参照通信协议）
                            cache.SequenceNumber = (int)data.FrameSequenceNumber;
                            RouteDataCache.Add(cache);


                            var prepareParseData = new List<byte>();
                            //对缓存区的每帧数据按帧标识位排序 ,如果出现重发帧则使用重发帧,否则使用非重发帧
                            var handleResend = from item in RouteDataCache
                                               group item by item.DataPosition into defaultresendgroup
                                               select
                                               (from defaultresenditem in defaultresendgroup
                                                orderby defaultresenditem.DataSign descending
                                                orderby defaultresenditem.CreateTime descending
                                                select defaultresenditem)
                                               .FirstOrDefault();


                            //添加到路由表数据预解析区
                            var cacheList = from item in handleResend
                                            orderby item.DataPosition descending
                                            select item;

                            foreach (var item in cacheList)
                            {
                                prepareParseData.AddRange(item.CollectData);
                            }

                            //路由表数据解析
                            //数据帧传感器数据中的Count参考通讯协议
                            var dataList = ParseHelper.ParseRouteRealData(prepareParseData.ToArray());

                            //路由表数据逐条入库
                            foreach (var item in dataList)
                            {
                                var x16 = ParseHelper.ShowX16(item.GetRawBytes());
                                Debug.WriteLine(string.Format("解析出的路由表数据为: {0}", x16));//调试窗口显示解析出的数据

                                var bytestime = new byte[7];

                                //时间bytes
                                Array.Copy(item.CollectTime, 0, bytestime, 0, 7);

                                //路由表中的count(总共多少对下一跳地址和目的地址)
                                var routecount = (Int32)item.CountByte;

                                //路由表中的源节点地址
                                var sourcenode = new byte[2];
                                Array.Copy(item.SourceNodeAddress, 0, sourcenode, 0, 2);

                                //数据bytes
                                var contentdata = new byte[item.Data.Length];

                                Array.Copy(item.Data, 0, contentdata, 0, item.Data.Length);

                                var strtime = string.Format("{0}{1}-{2}-{3} {4}:{5}:{6}",
                                  (int)bytestime[0],
                                  (int)bytestime[1],
                                  (int)bytestime[2],
                                  (int)bytestime[3],
                                  (int)bytestime[4],
                                  (int)bytestime[5],
                                  (int)bytestime[6]);
                                var strDay = string.Format("{0}{1}-{2}-{3}",
                                  (int)bytestime[0],
                                  (int)bytestime[1],
                                  (int)bytestime[2],
                                  (int)bytestime[3]);
                                string strLimitTime1 = strDay + " 5:30:00";
                                string strLimitTime2 = strDay + " 18:30:00";
                                //string strLimitTime3 = strDay + " 18:00:00";
                                //string strLimitTime4 = strDay + " 20:00:00";
                                DateTime LimitTime1 = Convert.ToDateTime(strLimitTime1);
                                DateTime LimitTime2 = Convert.ToDateTime(strLimitTime2);
                                //DateTime LimitTime3 = Convert.ToDateTime(strLimitTime3);
                                //DateTime LimitTime4 = Convert.ToDateTime(strLimitTime4);
                                var t = Convert.ToDateTime(strtime);
                                bool flog = ((DateTime.Compare(t, LimitTime1) >= 0) && (DateTime.Compare(t, LimitTime2) <= 0));
                                flog = true;
                                if (flog)
                                {
                                    if (routecount != 0)
                                    {
                                        //根据解析出的数据判断属于哪个城市,如果返回-1则说明未找到该数据所属城市则不插入数据
                                        var CityID = CityHelper.GetRouteCityIDBySourceNode(sourcenode, contentdata);

                                        if (CityID != -1)
                                        {

                                            using (var db = new DB.WirelessSensorNetworkEntities())
                                            {
                                                DateTime time = Convert.ToDateTime(strtime);

                                                Debug.WriteLine("路由表时间:" + time.ToString("yyyy-MM-dd HH:mm:ss"));

                                                ////计算总共多少对下一跳地址和目的地址
                                                //int count = (contentdata.Length / 4);

                                                for (int i = 0; i < routecount; i++)
                                                {
                                                    var nextnode = new byte[] { contentdata[i * 4], contentdata[i * 4 + 1] };
                                                    var targetnode = new byte[] { contentdata[i * 4 + 2], contentdata[i * 4 + 3] };
                                                    //调试窗口显示
                                                    Debug.WriteLine("解析到的源节点地址为:" + ParseHelper.ShowX16(sourcenode).Replace(" ", ""));
                                                    Debug.WriteLine("解析到的下一跳地址和目的地址为:" + ParseHelper.ShowX16(nextnode).Replace(" ", "") + "_" + ParseHelper.ShowX16(targetnode).Replace(" ", ""));
                                                    ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info,
                                                                                        "路由表时间:" + time.ToString("yyyy-MM-dd HH:mm:ss")
                                                                                       + "解析到的源节点地址为:" + ParseHelper.ShowX16(sourcenode).Replace(" ", "")
                                                                                       + "解析到的下一跳地址和目的地址为:" + ParseHelper.ShowX16(nextnode).Replace(" ", "") + "_" + ParseHelper.ShowX16(targetnode).Replace(" ", ""));
                                                    var dbitemrt = new DB.WSNRouteTable();

                                                    dbitemrt.UploadTime = time;
                                                    dbitemrt.TargetNode = ParseHelper.ShowX16(targetnode).Replace(" ", "");
                                                    dbitemrt.NextNode = ParseHelper.ShowX16(nextnode).Replace(" ", "");
                                                    dbitemrt.SourceNode = ParseHelper.ShowX16(sourcenode).Replace(" ", "");

                                                    //需要计算城市id
                                                    dbitemrt.CityID = CityID;

                                                    db.AddToWSNRouteTables(dbitemrt);
                                                }


                                                db.SaveChanges();


                                            }

                                            ////更新界面
                                            //this.View.Invoke(new UICallBack(() =>
                                            //{
                                            //    var upateParams = new UpdateParams();
                                            //    upateParams.Message = x16 + " ";//邻居表数据显示在页面窗口上数据显示的格式
                                            //    ((IUpdateView)this.View).UpdateView(upateParams);
                                            //}));

                                        }
                                        else
                                        {

                                            ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "此次上传的路由表不属于任何一个检测的地方");
                                        }

                                    }
                                    else if (routecount == 0)
                                    {    //如果没有下一跳地址和目的地址
                                        //根据解析出的数据判断属于哪个城市,如果返回-1则说明未找到该数据所属城市则不插入数据
                                        var CityID = CityHelper.GetRouteCityIDBySourceNode(sourcenode, contentdata);

                                        if (CityID != -1)
                                        {

                                            using (var db = new DB.WirelessSensorNetworkEntities())
                                            {
                                                DateTime time = Convert.ToDateTime(strtime);

                                                Debug.WriteLine("路由表时间:" + time.ToString("yyyy-MM-dd HH:mm:ss"));

                                                var dbitemrt = new DB.WSNRouteTable();
                                                Debug.WriteLine("解析到的源节点地址为:" + ParseHelper.ShowX16(sourcenode).Replace(" ", ""));
                                                dbitemrt.UploadTime = time;
                                                dbitemrt.SourceNode = ParseHelper.ShowX16(sourcenode).Replace(" ", "");
                                                dbitemrt.TargetNode = "无";
                                                dbitemrt.NextNode = "无";

                                                //需要计算城市id
                                                dbitemrt.CityID = CityID;

                                                db.AddToWSNRouteTables(dbitemrt);

                                                db.SaveChanges();
                                            }
                                        }
                                        else
                                        {

                                            ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "此次上传的路由表不属于任何一个检测的地方");
                                        }

                                    }
                                    else
                                    {
                                        ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "此次没有路由表数据上传");
                                    }
                                }

                            }
                            //移除缓冲区
                            RouteDataCache.Clear();
                        }
                    }
                    #endregion


                    #region  数据帧中的采样数据表

                    //采样数据表(包括温湿度等数据)
                    if (dataType == (byte)DataType.SensorData)
                    {
                        //采集数据未传完
                        if (completeFlag == 01)
                        {

                            //构建数据无错应答帧
                            var sensorDataAck = BuildHelper.BuildNoErrorDataAck();
                            //如果汇聚节点仍在线
                            if (Session.Connected)
                            {
                                //给汇聚节点回应
                                Session.Send(sensorDataAck, 0, sensorDataAck.Length);
                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "采集数据未传完，数据无错应答已发送，等待数据中……");

                                //记录上位机给汇聚节点发送采集数据未传完应答帧
                                var x16 = ParseHelper.ShowX16(sensorDataAck);
                                LogHelper.SaveSendMessageLog(DateTime.Now, x16, Session.RemoteEndPoint.ToString());
                            }
                            else
                            {
                                //状态更新——未编程
                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "采集数据未传完，网络错误或汇聚节点已断开！");
                            }

                            //添加到采集数据的缓存
                            var cache = new CacheModel();
                            cache.CollectData = dataFrame.Data;
                            cache.CompleteFlag = completeFlag;
                            cache.CreateTime = DateTime.Now;
                            cache.DataPosition = dataPosition;
                            cache.DataSign = dataSign;
                            cache.SequenceNumber = (int)data.FrameSequenceNumber;
                            SensorDataCache.Add(cache);

                        }
                        //采集数据已传完
                        if (completeFlag == 00)
                        {

                            //构建数据无错应答帧
                            var sensorDataAck = BuildHelper.BuildNoErrorDataAck();
                            //如果下位机仍在线
                            if (Session.Connected)
                            {
                                //给下位机回应
                                Session.Send(sensorDataAck, 0, sensorDataAck.Length);
                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "采集数据已传完，数据无错应答已发送");

                                //记录上位机给汇聚节点发送采集数据已传完应答帧
                                var x16 = ParseHelper.ShowX16(sensorDataAck);
                                LogHelper.SaveSendMessageLog(DateTime.Now, x16, Session.RemoteEndPoint.ToString());
                            }
                            else
                            {
                                //状态更新——未编程
                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "采集数据已传完，网络错误或汇聚节点已断开！");
                            }

                            //添加到采集数据的缓存区
                            var cache = new CacheModel();
                            cache.CollectData = dataFrame.Data;
                            cache.CompleteFlag = completeFlag;
                            cache.CreateTime = DateTime.Now;
                            cache.DataPosition = dataPosition;
                            cache.DataSign = dataSign;
                            cache.SequenceNumber = (int)data.FrameSequenceNumber;
                            SensorDataCache.Add(cache);
                            //var drawTopologyArray = new List<string>();


                            var prepareParseData = new List<byte>();
                            //对缓存区的每帧数据按帧标识位排序 ,如果出现重发帧则使用重发帧,否则使用非重发帧
                            var handleResend = from item in SensorDataCache
                                               group item by item.DataPosition into defaultresendgroup
                                               select
                                               (from defaultresenditem in defaultresendgroup
                                                orderby defaultresenditem.DataSign descending
                                                orderby defaultresenditem.CreateTime descending
                                                select defaultresenditem)
                                               .FirstOrDefault();


                            //添加到采集数据的预解析区
                            var cacheList = from item in handleResend
                                            orderby item.DataPosition descending
                                            select item;

                            //int OldCityID = -1;
                            foreach (var item in cacheList)
                            {
                                prepareParseData.AddRange(item.CollectData);
                            }

                            //采集数据解析
                            //数据帧传感器数据中的Count参考通讯协议
                            var dataList = ParseHelper.ParseRealData(prepareParseData.ToArray());
                            var drawTopologyArray = new List<string>();
                            DateTime time = Convert.ToDateTime("00:00:00");
                            DateTime time1 = Convert.ToDateTime("00:00:00");
                            int Cid=-1;
                            int Cid1 = -1;
                            var TopologyArray = new String[1];
                             

                            //int Oldcity=-1;

                            //逐条入库
                            foreach (var item in dataList)
                            {
                                var x16 = ParseHelper.ShowX16(item.GetRawBytes());
                                Debug.WriteLine(string.Format("解析出的采集数据为: {0}", x16));//调试窗口显示解析出的数据



                                //数组变量个数
                                var collectcount = Convert.ToInt32(item.Count & 0x0F);

                                if (collectcount != 0)
                                {

                                    //传感器个数
                                    var sensorcount = Convert.ToInt32((item.Count & 0xF0) >> 4);

                                    decimal? soilTemperature = null;
                                    decimal? minusSoilTemperature = null;
                                    decimal? ci = null;
                                    decimal? lai = null;
                                    decimal? airTemperature = null;
                                    decimal? minusAirTemperature = null;
                                    decimal? soilHumidity = null;
                                    decimal? airHumidity = null;
                                    decimal? mta = null;
                                    decimal? mta_negative = null;
                                    decimal? cover = null;
                                    decimal? mLai = null;
                                    decimal? batA = null;
                                    decimal? batSys = null;
                                    decimal? rssi = null;
                                    decimal? difn = null;

                                    //采集节点地址
                                    var nodeaddress = new byte[2];
                                    Array.Copy(item.NodeAddress, 0, nodeaddress, 0, 2);

                                    var strChildNode = ParseHelper.ShowX16(nodeaddress).Replace(" ","");
                                    //drawTopologyArray.Add(string.Format("{0}{1}", "0000", strChildNode));

                                    //用于判断这个是属于哪个城市的数据
                                    var CityID = CityHelper.GetCollectCityIDByCollectNode(nodeaddress);
                                    if (CityID != -1)
                                    {

                                        using (var db = new DB.WirelessSensorNetworkEntities())
                                        {
                                            for (int i = 0; i < collectcount; i++)//数据组数循环
                                            {

                                                Debug.WriteLine("采集节点:" + ParseHelper.ShowX16(nodeaddress).Replace(" ", ""));
                                                var bytesTime = new byte[7];
                                                //采集时间bytes
                                                Array.Copy(item.Data, (3* sensorcount + 7) * i, bytesTime, 0, 7);

                                                var strTime = string.Format("{0}{1}-{2}-{3} {4}:{5}:{6}",
                                                  (((bytesTime[0] & 0xF0) >> 4).ToString() + (bytesTime[0] & 0x0F).ToString()),
                                                  (((bytesTime[1] & 0xF0) >> 4).ToString() + (bytesTime[1] & 0x0F).ToString()),
                                                  (((bytesTime[2] & 0xF0) >> 4).ToString() + (bytesTime[2] & 0x0F).ToString()),
                                                  (((bytesTime[3] & 0xF0) >> 4).ToString() + (bytesTime[3] & 0x0F).ToString()),
                                                  (((bytesTime[4] & 0xF0) >> 4).ToString() + (bytesTime[4] & 0x0F).ToString()),
                                                  (((bytesTime[5] & 0xF0) >> 4).ToString() + (bytesTime[5] & 0x0F).ToString()),
                                                  (((bytesTime[6] & 0xF0) >> 4).ToString() + (bytesTime[6] & 0x0F).ToString()));
                                                DateTime uploadtime = Convert.ToDateTime(strTime);
                                                var strDay = string.Format("{0}{1}-{2}-{3}",
                                                  (((bytesTime[0] & 0xF0) >> 4).ToString() + (bytesTime[0] & 0x0F).ToString()),
                                                  (((bytesTime[1] & 0xF0) >> 4).ToString() + (bytesTime[1] & 0x0F).ToString()),
                                                  (((bytesTime[2] & 0xF0) >> 4).ToString() + (bytesTime[2] & 0x0F).ToString()),
                                                  (((bytesTime[3] & 0xF0) >> 4).ToString() + (bytesTime[3] & 0x0F).ToString()));
                                                string strLimitTime1 = strDay + " 5:30:00";
                                                string strLimitTime2 = strDay + " 18:30:00";
                                                //string strLimitTime3 = strDay + " 18:00:00";
                                                //string strLimitTime4 = strDay + " 20:00:00";
                                                DateTime LimitTime1 = Convert.ToDateTime(strLimitTime1);
                                                DateTime LimitTime2 = Convert.ToDateTime(strLimitTime2);
                                                //DateTime LimitTime3 = Convert.ToDateTime(strLimitTime3);
                                                //DateTime LimitTime4 = Convert.ToDateTime(strLimitTime4);
                                                bool flog = ((DateTime.Compare(uploadtime, LimitTime1) >= 0) && (DateTime.Compare(uploadtime, LimitTime2) <= 0));
                                                Debug.WriteLine("采集时间：" + uploadtime.ToString("yyyy-MM-dd HH:mm:ss"));
                                                flog = true;
                                                if (flog)
                                                {
                                                    uint collectdata = 0;
                                                    //此循环是对每个传感器数据的循环，执行完sensorcount个循环之后，每个传感器的数据都填充完毕。
                                                    for (int j = 0; j < sensorcount; j++)
                                                    {
                                                        SensorKind sensorKind = (SensorKind)((item.Data[7 + (j * 3)] & 0xF0) >> 4);//获取传感器类型
                                                        collectdata = (uint)(item.Data[7 + (j * 3)] & 0x0F) * 10000 +
                                                        (uint)(item.Data[7 + (j * 3) + 1] >> 4) * 1000 + (uint)(item.Data[7 + (j * 3) + 1] & 0x0f) * 100 +
                                                        (uint)(item.Data[7 + (j * 3) + 2] >> 4) * 10 + (uint)(item.Data[7 + (j * 3) + 2] & 0x0f) * 1;
                                                        //用上面的方式计算是为了后面计算得到真实数值
                                                        //collectdata = (uint)((item.Data[7 + (j * 3)] & 0x0F) << 8) + (uint)(item.Data[7 + (j * 3) + 1]);
                                                        //collectdata = ((collectdata & 0x0F00) >> 8) * 100 + ((collectdata & 0x00F0) >> 4) * 10 + (collectdata & 0x0F);
                                                        switch (sensorKind)
                                                        {
                                                            case SensorKind.SoilTemperature:
                                                                {
                                                                    //土壤温度处理方法： 
                                                                    soilTemperature = (decimal?)collectdata / 10;
                                                                    Debug.WriteLine("土壤温度：" + soilTemperature);

                                                                }
                                                                break;
                                                            case SensorKind.minusSoilTemperature:
                                                                {
                                                                    //零下土壤温度处理方法：
                                                                    minusSoilTemperature = (decimal?)collectdata / 10;
                                                                    Debug.WriteLine("土壤温度：-" + minusSoilTemperature);

                                                                }
                                                                break;
                                                            case SensorKind.CI:
                                                                {
                                                                    //丛生系数处理方法：

                                                                    ci = (decimal?)collectdata / 1000;   //丛生系数保留三位小数

                                                                    Debug.WriteLine("丛生系数：" + airHumidity);


                                                                }
                                                                break;
                                                            case SensorKind.Lai:
                                                                {
                                                                    //LAI处理方法：
                                                                    lai = (decimal?)collectdata / 100;   //lai值保存小数点后两位

                                                                    Debug.WriteLine("LAI值：" + lai);

                                                                }
                                                                break;
                                                            case SensorKind.AirTemperature:
                                                                {
                                                                    //空气温度处理方法：
                                                                    airTemperature = (decimal?)collectdata / 10;  //温度单位是。C

                                                                    Debug.WriteLine("空气温度：" + airTemperature);

                                                                }
                                                                break;
                                                            case SensorKind.minusAirTemperature:
                                                                {
                                                                    //零下空气温度处理方法:
                                                                    minusAirTemperature = (decimal?)collectdata / 10;

                                                                    Debug.WriteLine("空气温度:-" + minusAirTemperature);
                                                                }
                                                                break;
                                                            case SensorKind.SoilHumidity:
                                                                {

                                                                    //土壤湿度处理方法：
                                                                    soilHumidity = (decimal?)collectdata / 100;   

                                                                    Debug.WriteLine("土壤湿度：" + soilHumidity);

                                                                }
                                                                break;
                                                            case SensorKind.AirHumidity:
                                                                {

                                                                    //空气湿度处理方法：
                                                                    airHumidity = (decimal?)collectdata / 100;

                                                                    Debug.WriteLine("空气湿度：" + airHumidity);

                                                                }
                                                                break;
                                                            case SensorKind.MTA:
                                                                {
                                                                    //平均叶倾角处理方法： 
                                                                    mta = (decimal?)collectdata / 100;//平均叶倾角保留两位小数
                                                                    Debug.WriteLine("平均叶倾角：" + mta);

                                                                }
                                                                break;
                                                            case SensorKind.MTA_negative:
                                                                {
                                                                    //平均叶倾角为负处理方法：
                                                                    mta_negative = (decimal?)collectdata / 100;
                                                                    Debug.WriteLine("平均叶倾角：-" + mta_negative);

                                                                }
                                                                break;
                                                            case SensorKind.Cover:
                                                                {
                                                                    //植被覆盖度处理方法：
                                                                    cover = (decimal?)collectdata / 1000;       //植被覆盖度保留三位小数

                                                                    Debug.WriteLine("植被覆盖度：" + cover);

                                                                }
                                                                break;
                                                            case SensorKind.MLAI:
                                                                {
                                                                    //MLAI处理方法：
                                                                    mLai = (decimal?)collectdata / 100;

                                                                    Debug.WriteLine("MLAI值：" + mLai);

                                                                }
                                                                break;
                                                            case SensorKind.BatA:
                                                                {
                                                                    //采集器电池电压处理方法：
                                                                    batA = (decimal?)collectdata / 100;       

                                                                    Debug.WriteLine("采集器电池电压：" + batA);

                                                                }
                                                                break;
                                                            case SensorKind.BatSys:
                                                                {
                                                                    //汇聚节点电池电压处理方法：
                                                                    batSys = (decimal?)collectdata / 100;

                                                                    Debug.WriteLine("汇聚节点电池电压：" + batSys);

                                                                }
                                                                break;
                                                            case SensorKind.Rssi:
                                                                {
                                                                    //RSSI处理方法：
                                                                    rssi = (decimal?)collectdata / 100;      

                                                                    Debug.WriteLine("RSSSI(信号强度)：" + rssi);

                                                                }
                                                                break;
                                                            case SensorKind.DIFN:
                                                                {
                                                                    //DIFN处理方法：
                                                                    difn = (decimal?)collectdata / 100;      

                                                                    Debug.WriteLine("系统电池电量：" + difn);

                                                                }
                                                                break;
                                                            default: break;
                                                        }
                                                    }
                                                    var dbitemud = new DB.WSNUploadData();
                                                    dbitemud.NodeAddress = ParseHelper.ShowX16(nodeaddress).Replace(" ", "");//将采集节点地址插入数据库中
                                                    dbitemud.UploadTime = uploadtime;//将采集时间插入数据库中
                                                    dbitemud.CityID = CityID;//将城市对应序号插入
                                                    if (minusSoilTemperature == null)
                                                    {
                                                        dbitemud.SoilTemperature = soilTemperature;//将土壤温度的值插入数据库中
                                                    }
                                                    else
                                                        dbitemud.SoilTemperature = -minusSoilTemperature;//将零下土壤温度的值插入数据库中
                                                    dbitemud.CI = ci;       //将丛生系数的值插入数据库中
                                                    dbitemud.Lai = lai;     //将LAI值插入数据库中
                                                    if (minusAirTemperature == null)
                                                    {
                                                        dbitemud.AirTemperature = airTemperature;//将空气温度得值插入数据库中
                                                    }
                                                    else
                                                        dbitemud.AirTemperature = -minusAirTemperature;//将零下空气温度得值插入数据库中
                                                    dbitemud.SoilHumidity = soilHumidity;//将土壤湿度的值插入数据库中
                                                    dbitemud.AirHumidity = airHumidity;//将空气湿度的值插入数据库中
                                                    if (mta_negative == null)
                                                    {
                                                        dbitemud.MTA = mta;//将平均叶倾角的值插入数据库中
                                                    }
                                                    else
                                                        dbitemud.MTA = -mta_negative;//将负的平均叶倾角的值插入数据库中
                                                    dbitemud.Rainfall = cover;//将植被覆盖度的值插入数据库中
                                                    dbitemud.MLai = mLai;//将MLAI值插入数据库中

                                                    dbitemud.APower = batA;//将采集器电池的电压值插入数据库中
                                                    dbitemud.SPower = batSys;//将汇聚节点的电压值插入数据库中
                                                    dbitemud.RSSI = rssi;//将RSSI(信号强度)值插入数据库中
                                                    dbitemud.DIFN = difn;//将difn的值插入数据库中


                                                    db.AddToWSNUploadDatas(dbitemud);

                                                    drawTopologyArray.Add(string.Format("{0}{1}", "0000", strChildNode));

                                                    time = uploadtime;
                                                    Cid = CityID;
                                               
                                                    
                                                    using (var dbitemdtt = new DB.WirelessSensorNetworkEntities())
                                                    {

                                                        var query = from message in dbitemdtt.WSNDrawTopologyTables
                                                                    where message.CityID == CityID
                                                                    where message.UploadTime == uploadtime
                                                                    select message;
                                                        //int data2 = query;
                                                        var data1 = query.ToList();

                                                        //var msg = from msg1 in db.WSNDrawTopologyTables
                                                        //          where msg1.CityID == CityID
                                                        //          where msg1.UploadTime == uploadtime
                                                        //          select msg1;

                                                        //db.DeleteObject(query);
                                                        //db.SaveChanges();

                                                        if (data1.Count != 0)
                                                        {
                                                            time1 = data1[0].UploadTime;
                                                            Cid1 = data1[0].CityID;
                                                            TopologyArray[0] = (data1[0].TopologyString);

                                                            if (time1 == time && Cid1 == Cid)
                                                                drawTopologyArray.Add(string.Format(TopologyArray[0]));

                                                          
                                                          //  using (var db1 = new DB.WirelessSensorNetworkEntities())
                                                           // {

                                                            var obj = dbitemdtt.WSNDrawTopologyTables.FirstOrDefault(x => x.UploadTime == time1 && x.CityID  == Cid1);
                                                                if (obj!=null )
                                                                {
                                                                    obj.TopologyString = string.Join("*", drawTopologyArray.ToArray());
                                                                    // dbItemDrawTopologyTable timesettable = db.WSNDrawTopologyTables.Single(m => m.CityID == CityID && m.UploadTime == uploadtime);
                                                                    //timesettable.TopologyString = drawTopologyArray;

                                                                    dbitemdtt.SaveChanges();
                                                                }
                                                          //  }

                                                            //db.WSNNodeLocationTables.DeleteObject(query);
                                                            //db.SaveChanges();
                                                        }
                                                        else
                                                        {
                                                            using (var db3 = new DB.WirelessSensorNetworkEntities())
                                                            {

                                                                var dbItemDrawTopologyTable = new DB.WSNDrawTopologyTable();
                                                                dbItemDrawTopologyTable.UploadTime = time;
                                                                dbItemDrawTopologyTable.CityID = Cid;

                                                                dbItemDrawTopologyTable.TopologyString = string.Join("*", drawTopologyArray.ToArray());
                                                                db.AddToWSNDrawTopologyTables(dbItemDrawTopologyTable);
                                                               
                                                                db.SaveChanges();

                                                            }
                                                        }
                                                       

                                                       
                                                    }
                                                 
                                                    


                                                    if (minusAirTemperature == null)
                                                    {
                                                        if (minusSoilTemperature == null)
                                                        {
                                                            if (mta_negative == null)
                                                            {
                                                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info,
                                                                                "采集节点:" + ParseHelper.ShowX16(nodeaddress).Replace(" ", "")
                                                                               + "采集时间：" + uploadtime.ToString("yyyy-MM-dd HH:mm:ss")
                                                                               + "土壤温度:" + soilTemperature + "℃,"
                                                                               + "丛生系数：" + ci + ","
                                                                               + "LAI：" + lai + ","
                                                                               + "空气温度：" + airTemperature + "℃,"
                                                                               + "土壤湿度：" + soilHumidity + "%,"
                                                                               + "空气湿度：" + airHumidity + "%,"
                                                                               + "平均叶倾角：" + mta + "°,"
                                                                               + "植被覆盖度：" + cover + ","
                                                                               + "MLAI：" + mLai + ","
                                                                               + "采集器电池电压：" + batA + "V,"
                                                                               + "汇聚节点电池电压：" + batSys + "V,"
                                                                               + "RSSI(信号强度)" + rssi + "V,"
                                                                               + "DIFN:" + difn 
                                                                                );
                                                            }
                                                            else
                                                            {
                                                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info,
                                                                                "采集节点:" + ParseHelper.ShowX16(nodeaddress).Replace(" ", "")
                                                                               + "采集时间：" + uploadtime.ToString("yyyy-MM-dd HH:mm:ss")
                                                                               + "土壤温度:" + soilTemperature + "℃,"
                                                                               + "丛生系数：" + ci + ","
                                                                               + "LAI：" + lai + ","
                                                                               + "空气温度：" + airTemperature + "℃,"
                                                                               + "土壤湿度：" + soilHumidity + "%,"
                                                                               + "空气湿度：" + airHumidity + "%,"
                                                                               + "平均叶倾角-：" + mta_negative + "°,"
                                                                               + "植被覆盖度：" + cover + ","
                                                                               + "MLAI：" + mLai + ","
                                                                               + "采集器电池电压：" + batA + "V,"
                                                                               + "汇聚节点电池电压：" + batSys + "V,"
                                                                               + "RSSI(信号强度)" + rssi + "V,"
                                                                               + "DIFN:" + difn
                                                                                );
                                                            }
                                                                
                                                        }
                                                        else
                                                        {
                                                            if (mta_negative == null)
                                                            {
                                                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info,
                                                                                "采集节点:" + ParseHelper.ShowX16(nodeaddress).Replace(" ", "")
                                                                               + "采集时间：" + uploadtime.ToString("yyyy-MM-dd HH:mm:ss")
                                                                               + "土壤温度:" + -minusSoilTemperature + "℃,"
                                                                               + "丛生系数：" + ci + ","
                                                                               + "LAI：" + lai + ","
                                                                               + "空气温度：" + airTemperature + "℃,"
                                                                               + "土壤湿度：" + soilHumidity + "%,"
                                                                               + "空气湿度：" + airHumidity + "%,"
                                                                               + "平均叶倾角：" + mta + "°,"
                                                                               + "植被覆盖度：" + cover + ","
                                                                               + "MLAI：" + mLai + ","
                                                                               + "采集器电池电压：" + batA + "V,"
                                                                               + "汇聚节点电池电压：" + batSys + "V,"
                                                                               + "RSSI(信号强度)" + rssi + "V,"
                                                                               + "DIFN:" + difn
                                                                                );
                                                            }
                                                            else
                                                            {
                                                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info,
                                                                                "采集节点:" + ParseHelper.ShowX16(nodeaddress).Replace(" ", "")
                                                                               + "采集时间：" + uploadtime.ToString("yyyy-MM-dd HH:mm:ss")
                                                                               + "土壤温度:-" + -minusSoilTemperature + "℃,"
                                                                               + "丛生系数：" + ci + ","
                                                                               + "LAI：" + lai + ","
                                                                               + "空气温度：" + airTemperature + "℃,"
                                                                               + "土壤湿度：" + soilHumidity + "%,"
                                                                               + "空气湿度：" + airHumidity + "%,"
                                                                               + "平均叶倾角-：" + mta_negative + "°,"
                                                                               + "植被覆盖度：" + cover + ","
                                                                               + "MLAI：" + mLai + ","
                                                                               + "采集器电池电压：" + batA + "V,"
                                                                               + "汇聚节点电池电压：" + batSys + "V,"
                                                                               + "RSSI(信号强度)" + rssi + "V,"
                                                                               + "DIFN:" + difn
                                                                                );
                                                            }
                                                        }
                                                             
                                                     }
                                                 
                                                    else
                                                    { 
                                                       if (minusSoilTemperature == null)
                                                        {
                                                            if (mta_negative == null)
                                                            {
                                                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info,
                                                                                "采集节点:" + ParseHelper.ShowX16(nodeaddress).Replace(" ", "")
                                                                               + "采集时间：" + uploadtime.ToString("yyyy-MM-dd HH:mm:ss")
                                                                               + "土壤温度:" + soilTemperature + "℃,"
                                                                               + "丛生系数：" + ci + ","
                                                                               + "LAI：" + lai + ","
                                                                               + "空气温度：-" + minusAirTemperature + "℃,"
                                                                               + "土壤湿度：" + soilHumidity + "%,"
                                                                               + "空气湿度：" + airHumidity + "%,"
                                                                               + "平均叶倾角：" + mta + "°,"
                                                                               + "植被覆盖度：" + cover + ","
                                                                               + "MLAI：" + mLai + ","
                                                                               + "采集器电池电压：" + batA + "V,"
                                                                               + "汇聚节点电池电压：" + batSys + "V,"
                                                                               + "RSSI(信号强度)" + rssi + "V,"
                                                                               + "DIFN:" + difn
                                                                                );
                                                            }
                                                            else
                                                            {
                                                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info,
                                                                                "采集节点:" + ParseHelper.ShowX16(nodeaddress).Replace(" ", "")
                                                                               + "采集时间：" + uploadtime.ToString("yyyy-MM-dd HH:mm:ss")
                                                                               + "土壤温度:" + soilTemperature + "℃,"
                                                                               + "丛生系数：" + ci + ","
                                                                               + "LAI：" + lai + ","
                                                                               + "空气温度：-" + minusAirTemperature + "℃,"
                                                                               + "土壤湿度：" + soilHumidity + "%,"
                                                                               + "空气湿度：" + airHumidity + "%,"
                                                                               + "平均叶倾角-：" + mta_negative + "°,"
                                                                               + "植被覆盖度：" + cover + ","
                                                                               + "MLAI：" + mLai + ","
                                                                               + "采集器电池电压：" + batA + "V,"
                                                                               + "汇聚节点电池电压：" + batSys + "V,"
                                                                               + "RSSI(信号强度)" + rssi + "V,"
                                                                               + "DIFN:" + difn
                                                                                );
                                                            }

                                                        }
                                                        else
                                                        {
                                                            if (mta_negative == null)
                                                            {
                                                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info,
                                                                                "采集节点:" + ParseHelper.ShowX16(nodeaddress).Replace(" ", "")
                                                                               + "采集时间：" + uploadtime.ToString("yyyy-MM-dd HH:mm:ss")
                                                                               + "土壤温度:" + -minusSoilTemperature + ","
                                                                               + "丛生系数：" + ci + ","
                                                                               + "LAI：" + lai + ","
                                                                               + "空气温度：" + airTemperature + "℃,"
                                                                               + "土壤湿度：" + soilHumidity + "%,"
                                                                               + "空气湿度：" + airHumidity + "%,"
                                                                               + "平均叶倾角：" + mta + "°,"
                                                                               + "植被覆盖度：" + cover + ","
                                                                               + "MLAI：" + mLai + ","
                                                                               + "采集器电池电压：" + batA + "V,"
                                                                               + "汇聚节点电池电压：" + batSys + "V,"
                                                                               + "RSSI(信号强度)" + rssi + "V,"
                                                                               + "DIFN:" + difn
                                                                                );
                                                            }
                                                            else
                                                            {
                                                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info,
                                                                                "采集节点:" + ParseHelper.ShowX16(nodeaddress).Replace(" ", "")
                                                                               + "采集时间：" + uploadtime.ToString("yyyy-MM-dd HH:mm:ss")
                                                                               + "土壤温度:-" + -minusSoilTemperature + ","
                                                                               + "丛生系数：" + ci + ","
                                                                               + "LAI：" + lai + ","
                                                                               + "空气温度：" + airTemperature + "℃,"
                                                                               + "土壤湿度：" + soilHumidity + "%,"
                                                                               + "空气湿度：" + airHumidity + "%,"
                                                                               + "平均叶倾角-：" + mta_negative + "°,"
                                                                               + "植被覆盖度：" + cover + ","
                                                                               + "MLAI：" + mLai + ","
                                                                               + "采集器电池电压：" + batA + "V,"
                                                                               + "汇聚节点电池电压：" + batSys + "V,"
                                                                               + "RSSI(信号强度)" + rssi + "V,"
                                                                               + "DIFN:" + difn
                                                                                );
                                                            }
                                                        }

                                                    }
                                                }
                                                else
                                                    ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "此次采集数据不在采集时间段内");
                                            }


                                            db.SaveChanges();

                                        }
                                    }
                                    else
                                    {
                                        //CityID=-1的时候的处理情况
                                        ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "此次采集数据不属于任何一个检测的地方");

                                    }



                                }
                                else if (collectcount == 0)
                                {
                                    // collectcount表示在采集数据表中（采集时间+采集数据+...）这一段重复0次的处理情况，也就是该节点没有上传采集数据
                                    ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "此次该节点没有上传采集数据");
                                }

                                ////更新界面
                                //this.View.Invoke(new UICallBack(() =>
                                //{
                                //    var upateParams = new UpdateParams();
                                //    upateParams.Message = x16 + " ";//采集数据显示在页面窗口上数据显示的格式
                                //    ((IUpdateView)this.View).UpdateView(upateParams);
                                //}));

                            }
                         
                          

                            //移除采集数据缓冲区
                            SensorDataCache.Clear();

                        }
                    }

                    #endregion
                    #region 数据帧中的Lai数据表
                    if (dataType == (byte)DataType.LaiData)
                    {
                        if (completeFlag == 01)
                        {

                            //构建数据无错应答帧
                            var LaiDataAck = BuildHelper.BuildNoErrorDataAck();
                            //如果汇聚节点仍在线
                            if (Session.Connected)
                            {
                                //给汇聚节点回应
                                Session.Send(LaiDataAck, 0, LaiDataAck.Length);
                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "Lai数据未传完，数据无错应答已发送，等待数据中……");

                                //记录上位机给汇聚节点发送Lai数据未传完应答帧
                                var x16 = ParseHelper.ShowX16(LaiDataAck);
                                LogHelper.SaveSendMessageLog(DateTime.Now, x16, Session.RemoteEndPoint.ToString());
                            }
                            else
                            {
                                //状态更新——未编程
                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "Lai数据未传完，网络错误或汇聚节点已断开！");
                            }

                            //添加到Lai数据的缓存
                            var cache = new CacheModel();
                            cache.CollectData = dataFrame.Data;
                            cache.CompleteFlag = completeFlag;
                            cache.CreateTime = DateTime.Now;
                            cache.DataPosition = dataPosition;
                            cache.DataSign = dataSign;
                            cache.SequenceNumber = (int)data.FrameSequenceNumber;
                            SensorDataCache.Add(cache);

                        }
                        //采集数据已传完
                        if (completeFlag == 00)
                        {

                            //构建数据无错应答帧
                            var LaiDataAck = BuildHelper.BuildNoErrorDataAck();
                            //如果下位机仍在线
                            if (Session.Connected)
                            {
                                //给下位机回应
                                Session.Send(LaiDataAck, 0, LaiDataAck.Length);
                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "Lai数据已传完，数据无错应答已发送");

                                //记录上位机给汇聚节点发送Lai数据已传完应答帧
                                var x16 = ParseHelper.ShowX16(LaiDataAck);
                                LogHelper.SaveSendMessageLog(DateTime.Now, x16, Session.RemoteEndPoint.ToString());
                            }
                            else
                            {
                                //状态更新——未编程
                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "采集数据已传完，网络错误或汇聚节点已断开！");
                            }

                            //添加到Lai数据的缓存区
                            var cache = new CacheModel();
                            cache.CollectData = dataFrame.Data;
                            cache.CompleteFlag = completeFlag;
                            cache.CreateTime = DateTime.Now;
                            cache.DataPosition = dataPosition;
                            cache.DataSign = dataSign;
                            cache.SequenceNumber = (int)data.FrameSequenceNumber;
                            SensorDataCache.Add(cache);


                            var prepareParseData = new List<byte>();
                            //对缓存区的每帧数据按帧标识位排序 ,如果出现重发帧则使用重发帧,否则使用非重发帧
                            var handleResend = from item in SensorDataCache
                                               group item by item.DataPosition into defaultresendgroup
                                               select
                                               (from defaultresenditem in defaultresendgroup
                                                orderby defaultresenditem.DataSign descending
                                                orderby defaultresenditem.CreateTime descending
                                                select defaultresenditem)
                                               .FirstOrDefault();


                            //添加到采集数据的预解析区
                            var cacheList = from item in handleResend
                                            orderby item.DataPosition descending
                                            select item;

                            foreach (var item in cacheList)
                            {
                                prepareParseData.AddRange(item.CollectData);
                            }

                            var dataList = ParseHelper.ParseRealData(prepareParseData.ToArray());
                            foreach (var item in dataList)
                            {
                                var x16 = ParseHelper.ShowX16(item.GetRawBytes());
                                Debug.WriteLine(string.Format("解析出的Lai数据为: {0}", x16));//调试窗口显示解析出的数据


                                //数组变量个数
                                var collectcount = Convert.ToInt32(item.Count & 0x0F);
                                if (collectcount != 0)
                                {
                                    var parametercount = Convert.ToInt32((item.Count & 0xF0) >> 4);

                                    decimal? lai = null;
                                    var nodeaddress = new byte[2];
                                    Array.Copy(item.NodeAddress, 0, nodeaddress, 0, 2);

                                    //用于判断这个是属于哪个城市的数据
                                    var CityID = CityHelper.GetCollectCityIDByCollectNode(nodeaddress);
                                    if (CityID != -1)
                                    {
                                        using (var db = new DB.WirelessSensorNetworkEntities())
                                        {
                                            for (int i = 0; i < collectcount; i++)//数据组数循环
                                            {
                                                var dbitemud = new DB.WSNLAI();
                                                dbitemud.NodeAddress = ParseHelper.ShowX16(nodeaddress).Replace(" ", "");//将采集节点地址插入数据库中
                                                Debug.WriteLine("采集节点:" + ParseHelper.ShowX16(nodeaddress).Replace(" ", ""));

                                                var bytesTime = new byte[7];
                                                //采集时间bytes
                                                Array.Copy(item.Data, (2 * parametercount + 7) * i, bytesTime, 0, 7);

                                                var strTime = string.Format("{0}{1}-{2}-{3} {4}:{5}:{6}",
                                                  (((bytesTime[0] & 0xF0) >> 4).ToString() + (bytesTime[0] & 0x0F).ToString()),
                                                  (((bytesTime[1] & 0xF0) >> 4).ToString() + (bytesTime[1] & 0x0F).ToString()),
                                                  (((bytesTime[2] & 0xF0) >> 4).ToString() + (bytesTime[2] & 0x0F).ToString()),
                                                  (((bytesTime[3] & 0xF0) >> 4).ToString() + (bytesTime[3] & 0x0F).ToString()),
                                                  (((bytesTime[4] & 0xF0) >> 4).ToString() + (bytesTime[4] & 0x0F).ToString()),
                                                  (((bytesTime[5] & 0xF0) >> 4).ToString() + (bytesTime[5] & 0x0F).ToString()),
                                                  (((bytesTime[6] & 0xF0) >> 4).ToString() + (bytesTime[6] & 0x0F).ToString()));
                                                DateTime uploadtime = Convert.ToDateTime(strTime);
                                                dbitemud.Updatatime = uploadtime;//将采集时间插入数据库中
                                                Debug.WriteLine("采集时间：" + uploadtime.ToString("yyyy-MM-dd HH:mm:ss"));

                                                //每个节点每次采集时间的数据
                                                uint collectdata = 0;

                                                //此循环是对每个传感器数据的循环，执行完sensorcount个循环之后，每个传感器的数据都填充完毕。
                                                for (int j = 0; j < parametercount; j++)
                                                {
                                                    ParameterKind sensorKind = (ParameterKind)((item.Data[7 + (j * 2)] & 0xF0) >> 4);//获取参数类型   
                                                    collectdata = (uint)((item.Data[7 + (j * 2)] & 0x0F) << 8) + (uint)(item.Data[7 + (j * 2) + 1]);
                                                    collectdata = ((collectdata & 0x0F00) >> 8) * 100 + ((collectdata & 0x00F0) >> 4) * 10 + (collectdata & 0x0F);
                                                    switch (sensorKind)
                                                    {
                                                        case ParameterKind.Lai:
                                                            {
                                                                //空气湿度处理方法：

                                                                lai = (decimal?)collectdata / 10;   //湿度单位改成的是%

                                                                Debug.WriteLine("LAI值：" + lai);


                                                            }
                                                            break;
                                                        default: break;
                                                    }
                                                }
                                                dbitemud.Lai = lai;//将空气湿度的值插入数据库中
                                                dbitemud.CityID = CityID;
                                                db.AddToWSNLAIs(dbitemud);

                                                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info,
                                                                                 "采集节点:" + ParseHelper.ShowX16(nodeaddress).Replace(" ", "")
                                                                                + "采集时间：" + uploadtime.ToString("yyyy-MM-dd HH:mm:ss")
                                                                                + "LAI值：" + lai + ","
                                                                                 );

                                            }

                                            db.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        //CityID=-1的时候的处理情况
                                        ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "此次采集数据不属于任何一个检测的地方");

                                    }
                                }
                                else if (collectcount == 0)
                                {
                                    // collectcount表示在采集数据表中（采集时间+采集数据+...）这一段重复0次的处理情况，也就是该节点没有上传采集数据
                                    ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "此次该节点没有上传采集数据");
                                }

                                ////更新界面
                                //this.View.Invoke(new UICallBack(() =>
                                //{
                                //    var upateParams = new UpdateParams();
                                //    upateParams.Message = x16 + " ";//采集数据显示在页面窗口上数据显示的格式
                                //    ((IUpdateView)this.View).UpdateView(upateParams);
                                //}));

                            }

                            //移除采集数据缓冲区
                            SensorDataCache.Clear();
                        }
                    }
                    #endregion
                }

                #endregion

            }
            catch (DataLengthTooShortException ex)
            {
                //SensorDataCache.Clear();
                //NeighborDataCache.Clear();
                //RouteDataCache.Clear();
                //ResendDataCache.Clear();

                //帧长度太短
                Debug.WriteLine("帧长度太短校验失败");

                //添加帧长度校验的消息
                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "帧长度太短校验失败!");

                //记录异常日志
                var x16 = ParseHelper.ShowX16(Data.Body);
                LogHelper.SaveExceptionLog(DateTime.Now, x16, ex.ToString());

                //已经重发的次数
                var hasResendCount = ResendHelper.GetCurrentResendCount(Session.SessionID);

                if (hasResendCount < SystemUtil.ResendTimes)
                {
                    //构建因发送过来的帧长度与实际不符合的重发应答
                    var resendAck = BuildHelper.BuildAllDataErrorAck();

                    //如果汇聚节点仍在线
                    if (Session.Connected)
                    {

                        //给汇聚节点回应
                        Session.Send(resendAck, 0, resendAck.Length);

                        //当前帧要求重发的错误次数加1
                        ResendHelper.AddResendData(Data.Body, ResendType.DataLengthTooShort, Session.SessionID);

                        ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "已发送因帧长度太短的数据全部重发的指令");

                        //记录因帧头帧尾验证不通过导致的重发指令
                        var reSendOrderX16 = ParseHelper.ShowX16(resendAck);
                        LogHelper.SaveSendMessageLog(DateTime.Now, reSendOrderX16, Session.RemoteEndPoint.ToString());
                        ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "未通过帧长度太短校验要求重发");
                    }
                }
                else
                {
                    //超过了所要求的最大重发次数不处理(此处可以记录日志)
                    ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, string.Format("连续重发{0}次数据仍然错误", SystemUtil.ResendTimes));
                    LogHelper.SaveExceptionLog(DateTime.Now, x16, "超过了要求回发的最大次数,数据仍然错误 停止要求重发");
                }



                //无须继续解析

            }
            catch (FrameTotalLengthNotEqualException ex)
            {
                //SensorDataCache.Clear();
                //NeighborDataCache.Clear();
                //RouteDataCache.Clear();
                //ResendDataCache.Clear();

                //帧标记的长度和实际长度不一致
                Debug.WriteLine("帧长度校验失败");

                //添加帧长度校验的消息
                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "帧长度校验失败!");

                //记录异常日志
                var x16 = ParseHelper.ShowX16(Data.Body);
                LogHelper.SaveExceptionLog(DateTime.Now, x16, ex.ToString());

                //已经重发的次数
                var hasResendCount = ResendHelper.GetCurrentResendCount(Session.SessionID);

                if (hasResendCount < SystemUtil.ResendTimes)
                {
                    //构建因发送过来的帧长度与实际不符合的重发应答
                    var resendAck = BuildHelper.BuildAllDataErrorAck();

                    //如果汇聚节点仍在线
                    if (Session.Connected)
                    {
                        //给汇聚节点回应
                        Session.Send(resendAck, 0, resendAck.Length);

                        //当前帧要求重发的错误次数加1
                        ResendHelper.AddResendData(Data.Body, ResendType.FrameTotalLengthNotEqual, Session.SessionID);

                        ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "已发送因帧长度与实际不符的数据全部重发的指令");

                        //记录因帧头帧尾验证不通过导致的重发指令
                        var reSendOrderX16 = ParseHelper.ShowX16(resendAck);
                        LogHelper.SaveSendMessageLog(DateTime.Now, reSendOrderX16, Session.RemoteEndPoint.ToString());
                        ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "未通过帧长度与实际不符校验要求重发");
                    }
                }
                else
                {
                    //超过了所要求的最大重发次数不处理(此处可以记录日志)
                    ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, string.Format("连续重发{0}次数据仍然错误", SystemUtil.ResendTimes));
                    LogHelper.SaveExceptionLog(DateTime.Now, x16, "超过了要求回发的最大次数,数据仍然错误 停止要求重发");
                }



                //无须继续解析

            }
            catch (FCSCodeIncorrectException ex)
            {
                //fcs码校验失败
                Debug.WriteLine("fcs校验失败");


                //添加fcs校验的消息
                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "fcs校验失败!");

                byte FrameSequenceNumber = (byte)ex.FrameSequenceNumber;

                //记录异常日志
                var x16 = ParseHelper.ShowX16(Data.Body);
                LogHelper.SaveExceptionLog(DateTime.Now, x16, ex.ToString());

                //已经重发的次数
                var hasResendCount = ResendHelper.GetCurrentResendCount(Session.SessionID);

                //小于等于最大要求重发次数
                if (hasResendCount < SystemUtil.ResendTimes)
                {
                    //回发要求下位机重发并记录其序列号
                    //构建因发送过来的帧fcs校验失败的重发应答
                    var resendAck = BuildHelper.BuildCmdErrorResponFrameAck(FrameSequenceNumber);

                    //如果汇聚节点仍在线
                    if (Session.Connected)
                    {
                        //给汇聚节点回应
                        Session.Send(resendAck, 0, resendAck.Length);

                        //将重发的数据添加入重发数据缓存
                        ResendHelper.AddResendData(Data.Body, ResendType.FCSIncorrect, Session.SessionID, FrameSequenceNumber);

                        ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "已发送FCS校验出错重发帧");

                        //记录因帧头帧尾验证不通过导致的重发指令
                        var reSendOrderX16 = ParseHelper.ShowX16(resendAck);
                        LogHelper.SaveSendMessageLog(DateTime.Now, reSendOrderX16, Session.RemoteEndPoint.ToString());
                    }
                    else
                    {
                        ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, string.Format("要求重发时客户端已断线"));
                        LogHelper.SaveExceptionLog(DateTime.Now, ParseHelper.ShowX16(Data.Body), "要求重发时客户端已断线");
                    }
                }
                else
                {

                    //超过了所要求的最大重发次数不处理(此处可以记录日志)
                    ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, string.Format("连续重发{0}次数据仍然错误", SystemUtil.ResendTimes));
                    LogHelper.SaveExceptionLog(DateTime.Now, ParseHelper.ShowX16(Data.Body), "超过了要求回发的最大次数,数据仍然错误 停止要求重发");
                }

                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "未通过FCS校验要求重发该帧");
                //无须继续解析


            }
            catch (DataFrameLengthNotEqualException ex)
            {
                //数据帧标记的长度和实际长度不一致
                Debug.WriteLine("数据帧长度校验失败");

                //添加数据帧长度校验的消息
                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "数据帧长度校验失败!");

                //记录异常日志
                var x16 = ParseHelper.ShowX16(Data.Body);
                LogHelper.SaveExceptionLog(DateTime.Now, x16, ex.ToString());

                ////构建因发送过来的帧长度与实际不符合的重发应答
                //var resendAck = BuildHelper.BuildAllDataErrorAck();

                ////如果汇聚节点仍在线
                //if (Session.Connected)
                //{
                //    //给汇聚节点回应
                //    Session.Send(resendAck, 0, resendAck.Length);
                //    ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Info, "已发送因帧长度与实际不符的数据全部重发的指令");

                //    //记录因帧头帧尾验证不通过导致的重发指令
                //    var reSendOrderX16 = ParseHelper.ShowX16(resendAck);
                //    LogHelper.SaveSendMessageLog(DateTime.Now, reSendOrderX16, Session.RemoteEndPoint.ToString());
                //}


                //ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "未通过帧长度与实际不符校验要求重发");
                //无须继续解析


            }

            catch (Exception ex)
            {

                //解析出现未知错误  
                Debug.WriteLine("解析出现未知错误，原因：" + ex.ToString());

                //添加解析出现未知错误的消息
                ViewUpdateHelper.AddListViewMessage(ListViewMessageType.Error, "解析出现未知错误!");

                //记录异常日志
                var x16 = ParseHelper.ShowX16(Data.Body);
                LogHelper.SaveExceptionLog(DateTime.Now, x16, ex.ToString());

            }
        }
    }
}