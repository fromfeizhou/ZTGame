namespace com.game.client
{
	namespace network
	{
		/** 设置 */
		public partial class Module
		{
			public const byte setting = 7;
		}

		public partial class Command
		{
			/** 获取设置 */
			public const byte setting_get_all = 1;

			/** 设置内容 */
			public const byte setting_set_one_setting = 2;

			/** 禁止协议 */
			public const byte setting_proto_forbid = 3;


		}
	}
}