namespace com.game.client
{
	namespace network
	{
		/** 聊天 */
		public partial class Module
		{
			public const byte chat = 6;
		}

		public partial class Command
		{
			/** 聊天请求，包含世界，场景，家族，队伍 */
			public const byte chat_req = 1;

			/** 私聊 */
			public const byte chat_private_chat = 2;

			/** 聊天内容推送 */
			public const byte chat_content_push = 3;

			/** 查看玩家展示信息 */
			public const byte chat_show_info = 4;

			/** 请求小喇叭队列长度 */
			public const byte chat_horn_length = 5;

			/** 系统公告 */
			public const byte chat_sys_anounce = 6;

			/** 传闻电视广播 */
			public const byte chat_rumor = 7;

			/** GM反馈 */
			public const byte chat_gm_advice = 8;

			/** 客户端提交错误 */
			public const byte chat_client_commit_error = 9;

			/** 请求私聊基本信息 */
			public const byte chat_basic_info_req = 10;

			/** 请求广播内容 */
			public const byte chat_brocast_info = 11;


		}
	}
}