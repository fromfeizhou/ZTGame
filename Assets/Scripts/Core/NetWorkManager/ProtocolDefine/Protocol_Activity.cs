namespace com.game.client
{
	namespace network
	{
		/** 设置 */
		public partial class Module
		{
			public const byte activity = 11;
		}

		public partial class Command
		{
			/** 活动列表 */
			public const byte activity_activity_list = 1;

			/** 领奖 */
			public const byte activity_get_activity_reward = 2;

			/** 活动信息 */
			public const byte activity_activity_info = 3;

			/** 所有活动开关 */
			public const byte activity_all_open_info = 4;

			/** 更新活动开关 */
			public const byte activity_update_open_info = 5;

			/** 更新活动状态 */
			public const byte activity_update_activity = 6;


		}
	}
}