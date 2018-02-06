namespace com.game.client
{
	namespace network.gamescoket
	{
		public enum eErrCode
		{
			/** 错误 - 创建Socket */
			CreateSocket,

			/** 错误 - 服务器拒绝连接 */
			ConnectRefused,

			/** 错误 - 断开服务器 */
			DisconnectEx,

			/** 错误 - 连接状态异常，需要清理当前GameSocket状态为None */
			ConnectStateErr,

			/** 错误 - 连接超时 */
			ConnectOutTime,

			/** 错误 - 发送消息异常 */
			SendMsgEx,

			/** 错误 - 发送消息失败 */
			SendMsgFail,

			/** 错误 - 读取网络流数据长度错误 */
			ReadBuffLenErr,

			/** 错误 - 读取网络数据异常 */
			ReadBuffEx,
		}
	}
}