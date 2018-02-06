namespace com.game.client
{
	namespace network
	{
		/** 邮件系统 */
		public partial class Module
		{
			public const byte mail = 8;
		}

		public partial class Command
		{
			/** 获取邮件列表 */
			public const byte mail_get_box_list = 1;

			/** 删除邮件 */
			public const byte mail_del_mail = 2;

			/** 提取邮件附件 */
			public const byte mail_take_attach = 3;

			/** 新邮件通知 */
			public const byte mail_new_mail_inform = 4;

			/** 发送邮件 */
			public const byte mail_send = 5;

			/** 阅读邮件 */
			public const byte mail_read_mail = 6;

			/** 一键提取附件 */
			public const byte mail_take_attach_all = 7;


		}
	}
}