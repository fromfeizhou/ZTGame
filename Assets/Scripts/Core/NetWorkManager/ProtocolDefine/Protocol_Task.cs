namespace com.game.client
{
	namespace network
	{
		/** 任务 */
		public partial class Module
		{
			public const byte task = 14;
		}

		public partial class Command
		{
			/** 任务列表 */
			public const byte task_list = 1;

			/** 领取任务奖励 */
			public const byte task_reward = 2;

			/** 批量更新任务 */
			public const byte task_update = 3;


		}
	}
}