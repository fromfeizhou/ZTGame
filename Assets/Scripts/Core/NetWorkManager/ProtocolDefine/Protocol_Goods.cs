namespace com.game.client
{
	namespace network
	{
		/** 物品 */
		public partial class Module
		{
			public const byte goods = 5;
		}

		public partial class Command
		{
			/** 物品信息 */
			public const byte goods_info = 1;

			/** 删除物品(支持多个) */
			public const byte goods_delete = 2;

			/** 使用物品 */
			public const byte goods_use = 3;

			/** 穿装备 */
			public const byte goods_puton = 4;

			/** 卸装备 */
			public const byte goods_takeoff = 5;

			/** 批量使用 */
			public const byte goods_batch_use = 6;

			/** 装备续费 */
			public const byte goods_renew_weapon = 7;

			/** 过期物品提示(前端检测) */
			public const byte goods_outimes_tips = 8;

			/** 出售物品 */
			public const byte goods_sale_goods = 9;

			/** 设置装备生效 */
			public const byte goods_set_valid = 10;

			/** 查看物品 */
			public const byte goods_look = 11;

			/** 装备替换(只限于已经装备的装备互相替换) */
			public const byte goods_replace = 12;


		}
	}
}