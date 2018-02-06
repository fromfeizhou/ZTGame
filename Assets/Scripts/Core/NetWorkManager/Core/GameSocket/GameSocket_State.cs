namespace com.game.client
{
	namespace network.gamescoket
	{
		public enum eConnectState
		{
			/** 连接拒绝 */
			ConnectionRefused = -1,

			/** 闲置 */
			None = 0,

			/** 连接中 */
			Connecting = 1,

			/** 连接后 */
			Connected = 2,
		}
	}
}