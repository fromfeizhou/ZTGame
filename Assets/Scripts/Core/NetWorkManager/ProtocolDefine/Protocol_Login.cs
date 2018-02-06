namespace com.game.client
{
	namespace network
	{
		/** 登陆相关 */
		public partial class Module
		{
			public const byte login = 1;
		}

		public partial class Command
		{
			/** 心跳 */
			public const byte login_heart = 0;

			/** 登陆 */
			public const byte login_login = 1;

			/** 选择角色 */
			public const byte login_select_role = 2;

			/** 创建角色 */
			public const byte login_create_role = 3;

			/** 服务端主动断开链接 */
			public const byte login_reject = 4;

			/** 通知服务端将要断开 */
			public const byte login_stop = 5;

			/** 加密参数 */
			public const byte login_auth_key = 6;

			/** 重登陆 */
			public const byte login_relogin = 7;

			/** 排队信息 */
			public const byte login_wait_info = 8;

			/** 服务端配置 */
			public const byte login_config = 9;

			/** 分享的配置 */
			public const byte login_share_config = 10;


		}
	}
}