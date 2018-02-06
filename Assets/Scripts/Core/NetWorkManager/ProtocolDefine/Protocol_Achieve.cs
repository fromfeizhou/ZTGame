namespace com.game.client
{
	namespace network
	{
		/** 设置 */
		public partial class Module
		{
			public const byte achieve = 9;
		}

		public partial class Command
		{
			/** 当前成就列表 */
			public const byte achieve_list = 1;

			/** 达成条件列表 */
			public const byte achieve_reach_condition = 2;

			/** 领成就奖励 */
			public const byte achieve_reward = 3;

			/** 更新通知 */
			public const byte achieve_update = 4;


		}
	}
}