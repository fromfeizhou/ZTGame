using System;
using System.Net.Sockets;

namespace com.game.client
{
	namespace network.gamescoket
	{
		public partial class GameSocket
		{
			public Action CallBack_OnConnect;
			public Action CallBack_OnSend;
			public Action<byte[]> CallBack_OnReceive;
			public Action CallBack_OnDisConnect;
			public Action<string> CallBack_OnError;

			/** 回调 连接完成 */
			private void Invoking_CallBack_OnConnect()
			{
				if (CallBack_OnConnect != null) {
					CallBack_OnConnect ();
				}
			}
		  
            /** 回调 发送完成 */
			private void Invoking_CallBack_OnSend()
			{
				if (CallBack_OnSend != null) {
					CallBack_OnSend ();
				}
			}
		
            /** 回调 收到一条完成数据 */
			private void Invoking_CallBack_OnReceive(byte[] data)
			{
				if (CallBack_OnReceive != null) {
					CallBack_OnReceive (data);
				}
			}
		 
            /** 回调 连接断开 */
			private void Invoking_CallBack_OnDisConnect()
			{
				if (CallBack_OnDisConnect != null) {
					CallBack_OnDisConnect ();
				}
			}

		    /** 回调 错误回调 */
			private void Invoking_CallBack_OnError(eErrCode errCode, Exception e)
			{
				string outputErrLog = string.Empty;
				outputErrLog += string.Format ("[{0}]", errCode);

				switch (errCode) {
				case eErrCode.ConnectStateErr:
					{
						outputErrLog += "Connect State is not eGameSocketState.None";
						break;
					}
				case eErrCode.ReadBuffLenErr:
					{
						outputErrLog += "Read NetworkStream readLen is 0";
						break;
					}
				case eErrCode.ConnectOutTime:
					{
						outputErrLog += "Connect Time Out";
						break;
					}
				default:
					{
						if (e is SocketException) {
							SocketException socketEx = (SocketException)e;
							outputErrLog += string.Format ("[SocketErrCode:{0}]", socketEx.ErrorCode);
						}
						
						outputErrLog += " msg:" + e.Message;
						outputErrLog += " stackTrace:" + e.StackTrace;
						break;
					}
				}

				if (CallBack_OnError != null) {
					CallBack_OnError (outputErrLog);
				}

				if (IsCloseClient (errCode)) {
					DisConnect ();
				}
			}

		    /** 这些错误是否断开客户端 */
		    private bool IsCloseClient(eErrCode errCode)
		    {
		        switch (errCode)
		        {
		            case eErrCode.ReadBuffLenErr:
		            {
		                return true;
		            }
		        }
		        return false;
		    }
        }
	}
}