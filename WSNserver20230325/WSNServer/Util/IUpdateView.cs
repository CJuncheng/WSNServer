using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSNServer.Util
{

    public delegate void UICallBack();

    public enum UpdateType
    {
        AddTabPage = 0,
        UpdateTabPageData = 1,
        RemoveTabPage = 3,
        AddListViewMessage = 4

    }


    public class UpdateParams
    {
        public object Message
        {
            set;
            get;
        }

        public UpdateType UpdateType
        {
            set;
            get;
        }
    }

    public interface IUpdateView
    {
        void UpdateView(UpdateParams updateParams);
    }


    public enum ListViewMessageType
    {

        Error = 0,
        Info = 1
    }

    public class ListViewMessage
    {

        public ListViewMessageType Type
        {

            set;
            get;
        }
        public string Message
        {
            set;
            get;
        }

    }
}
