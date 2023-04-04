using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WSNServer.Util
{
    //CityID判断帮助类
    public class CityHelper
    {

        #region  邻居表的子父节点判断CityID

        //根据邻居表的子父节点判断CityID，解析出的数据判断属于哪个程序,如果返回CityID=-1则说明未找到该数据所属城市则不插入数据
        public static int GetNeighborCityIDByData(byte[] contentData)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "\\CityMaps\\WSNNodeManagement.xml";
            XDocument doc = XDocument.Load(path);

            int cityID = -1;

            var citysetting = (from setting in doc.Root.Elements("Setting")
                               where setting.Element("title").Value == "网络节点"
                               select setting).FirstOrDefault();

            if (citysetting != null)
            {
                //获取所有城市的wsn列表
                var wsns = from wsn in citysetting.Elements("WSN") select wsn;
                //如果数据长度为0,直接返回CityID=-1
                if (contentData.Length != 0)
                {
                    int count = (contentData.Length / 4);
                    for (int i = 0; i < count; i++)
                    {
                        var childNode = new byte[] { contentData[i * 4], contentData[i * 4 + 1] };
                        var fatherNode = new byte[] { contentData[i * 4 + 2], contentData[i * 4 + 3] };

                        var strChildNode = ParseHelper.ShowX16(childNode).Replace(" ", "");
                        var strFatherNode = ParseHelper.ShowX16(fatherNode).Replace(" ", "");

                        //判断如父节点且子节点属于xml中的某个城市节点则返回 某个城市的ID
                        //选取父子节点都被包含的wsn
                        var wsnContain = (from wsn in wsns
                                          where wsn.Elements("Node").Any(n => n.Value == strChildNode) &&
                                                wsn.Elements("Node").Any(n => n.Value == strFatherNode)
                                          select wsn).FirstOrDefault();

                        //如果存在该wsn节点则 给 cityID赋值并退出循环
                        if (wsnContain != null)
                        {
                            //取WSN的I,便于扩展，如果增加城市只要增加对应的XML节点并设置WSN节点的ID为数据库中的CITYID即可
                            cityID = Convert.ToInt32(wsnContain.Attribute("ID").Value);
                            break;
                        }
                    }
                }
            }


            return cityID;
        }

        #endregion


        #region  路由表中源节点/下一节点地址和目的地址来判断CityID

        //根据路由表中源节点/下一节点地址和目的地址来判断数据来源于那个城市
        public static int GetRouteCityIDBySourceNode(byte[] sourceNode, byte[] contentData)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "\\CityMaps\\WSNNodeManagement.xml";
            XDocument doc = XDocument.Load(path);

            int cityID = -1;
            var citysetting = (from setting in doc.Root.Elements("Setting")
                               where setting.Element("title").Value == "网络节点"
                               select setting).FirstOrDefault();



            if (citysetting != null)
            {
                //获取所有城市的wsn列表
                var wsns = from wsn in citysetting.Elements("WSN") select wsn;
                int count = (contentData.Length / 4);
                if (count == 0)
                {
                    var strSourceNode = ParseHelper.ShowX16(sourceNode).Replace(" ", "");
                    //判断源节点、下一跳地址和目的地址属于xml中的某个城市节点则返回 某个城市的ID
                    var wsnContain = (from wsn in wsns
                                      where wsn.Elements("Node").Any(n => n.Value == strSourceNode)
                                      select wsn).FirstOrDefault();

                    //如果存在该wsn节点则 给 cityID赋值并退出循环
                    if (wsnContain != null)
                    {
                        //取WSN的I,便于扩展，如果增加城市只要增加对应的XML节点并设置WSN节点的ID为数据库中的CITYID即可
                        cityID = Convert.ToInt32(wsnContain.Attribute("ID").Value);

                    }
                }
                else if (count != 0)
                {
                    for (int i = 0; i < count; i++)
                    {

                        var nextNode = new byte[] { contentData[i * 4], contentData[i * 4 + 1] };
                        var targetNode = new byte[] { contentData[i * 4 + 2], contentData[i * 4 + 3] };

                        var strSourceNode = ParseHelper.ShowX16(sourceNode).Replace(" ", "");
                        var strNextNode = ParseHelper.ShowX16(nextNode).Replace(" ", "");
                        var strTargetNode = ParseHelper.ShowX16(targetNode).Replace(" ", "");

                        //判断源节点、下一跳地址和目的地址属于xml中的某个城市节点则返回 某个城市的ID
                        var wsnContain = (from wsn in wsns
                                          where wsn.Elements("Node").Any(n => n.Value == strSourceNode) &&
                                                wsn.Elements("Node").Any(n => n.Value == strNextNode) &&
                                                wsn.Elements("Node").Any(n => n.Value == strTargetNode)
                                          select wsn).FirstOrDefault();

                        //如果存在该wsn节点则 给 cityID赋值并退出循环
                        if (wsnContain != null)
                        {
                            //取WSN的I,便于扩展，如果增加城市只要增加对应的XML节点并设置WSN节点的ID为数据库中的CITYID即可
                            cityID = Convert.ToInt32(wsnContain.Attribute("ID").Value);
                            break;
                        }
                    }
                }
            }
            return cityID;

        }
        #endregion


        #region  采集节点地址来判断CityID

        //根据采集节点地址来判断解析出的数据属于那个城市，如果没有找到属于那个城市就返回默认值-1
        public static int GetCollectCityIDByCollectNode(byte[] nodeAddress)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "\\CityMaps\\WSNNodeManagement.xml";
            XDocument doc = XDocument.Load(path);

            int cityID = -1;

            var citysetting = (from setting in doc.Root.Elements("Setting")
                               where setting.Element("title").Value == "网络节点"
                               select setting).FirstOrDefault();
            if (citysetting != null)
            {
                //获取所有城市的wsn列表
                var wsns = from wsn in citysetting.Elements("WSN") select wsn;

                var strCollectNode = ParseHelper.ShowX16(nodeAddress).Replace(" ", "");
                //判断源节点、下一跳地址和目的地址属于xml中的某个城市节点则返回 某个城市的ID
                var wsnContain = (from wsn in wsns
                                  where wsn.Elements("Node").Any(n => n.Value == strCollectNode)
                                  select wsn).FirstOrDefault();

                //如果存在该wsn节点则 给 cityID赋值并退出循环
                if (wsnContain != null)
                {
                    //取WSN的I,便于扩展，如果增加城市只要增加对应的XML节点并设置WSN节点的ID为数据库中的CITYID即可
                    cityID = Convert.ToInt32(wsnContain.Attribute("ID").Value);

                }
            }
            return cityID;

        }
        #endregion
    }

}
