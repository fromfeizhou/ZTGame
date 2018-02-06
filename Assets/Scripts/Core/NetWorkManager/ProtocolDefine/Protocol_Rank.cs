namespace com.game.client
{
	namespace network
	{
		/** 设置 */
		public partial class Module
		{
			public const byte rank = 10;
		}

		public partial class Command
		{
			/** 排行榜信息 */
			public const byte rank_info = 1;

			/** 玩家的排行榜进度 */
			public const byte rank_role_rank_info = 2;


		}
	}
}