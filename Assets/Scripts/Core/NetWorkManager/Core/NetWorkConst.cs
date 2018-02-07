namespace com.game.client
{
	namespace network
	{
		public class NetWorkConst
		{
			#if UNITY_EDITOR
			public const bool CMDebug = true;
			#endif

			public const string Ip = "120.79.43.54";
			public const int Port = 92;
		    public const float ConnectTimeOut = 15.0f;

			public const int ReConnectTimes = 3;
			public const bool HeartSwitch = false;
            public const float Heart_Time = 1.0f;

		    public const int MxSendMsgNum = 8;

		}
	}
}