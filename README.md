Windows 下基于 C# SuperSocket 搭建 TCP 服务器

• 定义 TCP 报文协议和 SQL Server 表，接收地面 ZigBee 无线传感网络客户端节点的实时数据 (包含图片)
• 实例化 AppServer 对象，自定义 RequestInfo 实体类型以接收和处理二进制字符流。在 appServer 的构
造函数中继承使用 RequestFilterFactory，并执行自定义的 ReceiveFilter 和 RequestInfo
• 解析 RequestInfo 数据，将数据写入 SQL Server 数据库，并应答客户端；读取数据库，下发控制指令