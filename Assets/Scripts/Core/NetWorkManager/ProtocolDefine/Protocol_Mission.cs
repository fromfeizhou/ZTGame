namespace com.game.client
{
	namespace network
	{
		/** 设置 */
		public partial class Module
		{
			public const byte mission = 12;
		}

		public partial class Command
		{
			/** 日常任务列表 */
			public const byte mission_list = 1;

			/** 领取任务奖励 */
			public const byte mission_get_mission_reward = 2;

			/** 刷新任务列表 */
			public const byte mission_refresh_mission_list = 3;

			/** 更新任务 */
			public const byte mission_update_mission = 4;


		}
	}
}