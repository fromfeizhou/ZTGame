namespace com.game.client
{
	namespace network
	{
		/** 角色 */
		public partial class Module
		{
			public const byte role = 3;
		}

		public partial class Command
		{
			/** 角色信息 */
			public const byte role_info = 1;

			/** 手机系统信息 */
			public const byte role_mobile = 2;

			/** 其他人信息（角色属性面板） */
			public const byte role_info_other = 3;

			/** 属性更新 */
			public const byte role_attr = 4;

			/** 财富更新 */
			public const byte role_fortune = 5;

			/** 血蓝变化 */
			public const byte role_hp_mp_change = 6;

			/** 领取VIP礼包 */
			public const byte role_vip_gift = 7;

			/** 军功变化 */
			public const byte role_gongxun = 8;

			/** 获得经验 */
			public const byte role_exp_inc = 10;

			/** 升级 */
			public const byte role_upgrade = 11;

			/** 签到 */
			public const byte role_sign = 12;

			/** 签到信息 */
			public const byte role_sign_info = 13;

			/** 已完成的新手引导列表 */
			public const byte role_guide_list = 14;

			/** 最后完成的新手引导 */
			public const byte role_guide_latest = 15;

			/** 完成某新手引导 */
			public const byte role_guide_complete = 16;

			/** 充值信息 */
			public const byte role_pay_package_info = 17;

			/** 充值 */
			public const byte role_pay_package_buy = 18;

			/** 领取每日返还 */
			public const byte role_pay_package_refund = 19;

			/** 复活 */
			public const byte role_revive = 20;

			/** 使用CDKEY */
			public const byte role_use_cdkey = 21;

			/** 请求充值，获得订单号 */
			public const byte role_pay_package_req = 22;

			/** 角色在线时间 */
			public const byte role_online_time = 30;

			/** 购买体力 */
			public const byte role_vigour_buy = 40;

			/** 体力信息 */
			public const byte role_vigour_info = 41;

			/** 状态同步（服务端只广播） */
			public const byte role_sync = 44;

			/** 配件信息 */
			public const byte role_acc_info = 45;

			/** 购买金币 */
			public const byte role_buy_gold = 46;

			/** 购买金币信息查询 */
			public const byte role_buy_gold_info = 47;

			/** 消息错误 */
			public const byte role_error = 48;

			/** 购买活动道具 */
			public const byte role_buy_activity_goods = 49;

			/** 活动前置条件 */
			public const byte role_activity_condition_info = 50;

			/** 战力更新 */
			public const byte role_update_fight_point = 51;

			/** 活动总信息 */
			public const byte role_activity_all_info = 54;

			/** 活动总信息 */
			public const byte role_bc_info = 55;


		}
	}
}