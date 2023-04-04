using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSNServer.Views;
using WSNServer.WSNExceptions;

namespace WSNServer.Util
{
    //更新界面帮助类
    public static class ViewUpdateHelper
    {
        public static void UpdateView(UpdateParams updateParams)
        {
            lock (MainView.Lock)
            {
                if (MainView.View != null)
                {
                    MainView.View.Invoke(new UICallBack(() =>
                    {
                        ((IUpdateView)MainView.View).UpdateView(updateParams);
                    }));
                }
                else
                {
                    //没有需要更新的界面 或者界面还未初始化
                }
            }
        }


        //添加ListViewMessage
        public static void AddListViewMessage(ListViewMessageType type, string message)
        {
            var updateParams = new UpdateParams();
            updateParams.UpdateType = UpdateType.AddListViewMessage;
            ListViewMessage listViewMessage = new ListViewMessage();
            listViewMessage.Type = type;
            listViewMessage.Message = message;
            updateParams.Message = listViewMessage;
            UpdateView(updateParams);
        }
    }
}
