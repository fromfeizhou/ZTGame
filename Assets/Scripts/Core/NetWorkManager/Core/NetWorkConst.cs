using UnityEngine;

namespace com.game.client
{
	namespace network
	{
		public class NetWorkConst
		{
			/** 是否开启前后端通信 */
			public static bool IsOpenNetWork = true;

			/** 服务器列表-URL地址*/
			public static string ServerListPath{
				get{
					return "http://s1.game.gdxygm.com/info.txt";//开发服
				}
			}

			public const int ConnectTimeOut = 15;

			public const int ReConnectTimes = 3;

		    public const int MxSendMsgNum = 8;
		}
	}
}