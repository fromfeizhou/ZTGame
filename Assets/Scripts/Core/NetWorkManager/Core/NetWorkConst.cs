namespace com.game.client
{
	namespace network
	{
		public class NetWorkConst
		{
			/** 是否开启前后端通信 */
			public static bool IsOpenNetWork = true;

			//public const string Ip = "120.79.43.54";
			//public const int Port = 93;

			public const string Ip = "120.79.192.95";
			public const int Port = 9001;


			public const int ConnectTimeOut = 15;

			public const int ReConnectTimes = 3;

		    public const int MxSendMsgNum = 8;

		}
	}
}