using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSNServer.Util
{
    //系统配置常量
    public class SystemUtil {
        //重发次数常量
        public const int ResendTimes = 3;
    }

    /// <summary>
    /// 帧控制位的取值.<br></br>
    /// </summary>
    public enum FrameControlType
    {
        //帧控制
        Data = 0x01, //数据帧
        Cmd = 0x02,  //命令帧
        Ack = 0x03,  //应答帧
        JpgState=0x04 //图片回应帧
    }

    /// <summary>
    /// 命令位取值.<br></br>
    /// </summary>
    public enum CmdType
    {
        //命令帧
        CmdOrderForData = 0x01,//上位机发送的数据上传命令       (上位机发到汇聚节点)
        CmdOrderSleep = 0x02,  //上位机发送的休眠命令           (上位机发到汇聚节点)
        CmdConnectLine = 0x03,//ARM发送的联网命令              (汇聚节点发到上位机)
        CmdSleepRequest = 0x04, //ARM请求上位机发送休眠命令     (汇聚节点发到上位机)
    }

    /// <summary>
    /// 上传数据类型.<br></br>
    /// </summary>
    public enum DataType
    {
        //数据帧
        RoutingTable = 0x01, //路由表
        NeighborTable = 0x02,//邻居表
        SensorData = 0x03,   //采集数据
        LaiData=0x04,   //Lai数据
    }

    /// <summary>
    /// 续传标志位.<br></br>
    /// </summary>
    public enum FlagOver
    {
        Over = 0,
        Continue = 1,
    }

    /// <summary>
    /// 数据是否重传属性.<br></br>
    /// </summary>
    public enum DataFlag
    {
        FreshData = 0,  //第一次上传的数据
        RetransData = 1,//重传的数据
    }

    /// <summary>
    /// 采集数据中传感器类型取值.<br></br>
    /// </summary>
    public enum SensorKind
    {
        //传感器类型
        /**
         *  0000   0 GndTemp 土壤温度（预留）
            0001	1 GndTemp_negative土壤温度为负数（预留） 
            0010	2 CI 丛生系数
            0011   3 LAI值
            0100	4 AirTemp 空气温度     
            0101  	5 AirTemp_negative 空气温度为负数    
            0110	6 GndHumi  土壤湿度（预留）
            0111   7 AirHumi空气湿度（预留）
            1000   8 MTA 平均叶倾角     
            1001  	9 MTA_negative 平均叶倾角为负数 
            1010   A Cover 植被覆盖度
            1011	B MLAI 
            1100   C BatA 采集器电池电压
            1101	D BatSys  汇聚节点电池电压
            1110   E Rssi
            1111   F DIFN
        
            CI和Cover保留3位小数，温度1位小数，其余的2位小数
         */
        SoilTemperature = 0, //土壤温度（预留）

        minusSoilTemperature = 1, //GndTemp_negative土壤温度为负数（预留）

        CI = 2,//CI 丛生系数

        Lai = 3,

        AirTemperature = 4,//AirTemp 空气温度

        minusAirTemperature = 5,//AirTemp_negative 空气温度为负数

        SoilHumidity = 6,//GndHumi  土壤湿度（预留）

        AirHumidity = 7,//AirHumi空气湿度（预留）

        MTA = 8,//MTA 平均叶倾角

        MTA_negative = 9,//MTA_negative 平均叶倾角为负数

        Cover  = 10,//植被覆盖度

        MLAI = 11,

        BatA=12, //采集器电池电压

        BatSys =13,// 汇聚节点电池电压

        Rssi=14,

        DIFN=15,
    }
    public enum ParameterKind
    {
        Lai=0,
        lightTransmission=1,

    }

    /// <summary>
    /// 应答选项.<br></br>
    /// </summary>
    public enum AckOption
    {
        //应答帧
        CmdRight = 0x00,//命令无错
        CmdError = 0x01,//命令有错
        DataRight = 0x02,//数据无错，继续下一块，无参数
        AllError = 0x03,//数据有错，当次所发的全部数据重传，无参数（多发生在帧头帧尾校验出错）
        SomeDataError = 0x04,//部分出错，有参数，重传部分（多发生在FCS校验出错）
    }

    public enum Uploadflag
    {
        //图片是否需要上传标志
        Unnecessary= 0x00,
        Necessary = 0x01
    }
    public enum FTPflag
    {
        //图片是否上传成功标志
        Scuccess = 0x00,
        False=0x01
    }
}
