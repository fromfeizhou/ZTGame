namespace com.game.client
{
	namespace network
	{
		/** 地图 */
		public partial class Module
		{
			public const byte map = 4;
		}

		public partial class Command
		{
			/** 请求场景切换(暂时没用) */
			public const byte map_switch = 1;

			/** 前端开始进入地图 */
			public const byte map_enter = 2;

			/** 视野信息 */
			public const byte map_sight = 3;

			/** 玩家进入视野 */
			public const byte map_role_enter = 4;

			/** 玩家离开视野 */
			public const byte map_role_leave = 5;

			/** 玩家移动 */
			public const byte map_move = 6;

			/** 怪物进入场景 */
			public const byte map_mon_enter = 7;

			/** 怪物离开场景 */
			public const byte map_mon_leave = 8;

			/** 怪物移动 */
			public const byte map_mon_move = 9;

			/** 更新p_map_role部分数据 */
			public const byte map_role_update = 10;

			/** 更新p_map_mon部分数据 */
			public const byte map_mon_update = 11;

			/** 怪物死亡 */
			public const byte map_mon_dead = 12;

			/** 角色死亡 */
			public const byte map_role_dead = 13;

			/** 通关 */
			public const byte map_finish = 14;

			/** 副本时间控制 */
			public const byte map_time_control = 15;

			/** 攻击 */
			public const byte map_attack = 16;

			/** buff点进入场景 */
			public const byte map_buff_point_enter = 17;

			/** buff点离开场景 */
			public const byte map_buff_point_leave = 18;

			/** 获得buff点 */
			public const byte map_buff_point_get = 19;

			/** debuff点进入场景 */
			public const byte map_debuff_point_enter = 20;

			/** debuff点离开场景 */
			public const byte map_debuff_point_leave = 21;

			/** 获得debuff点 */
			public const byte map_debuff_point_get = 22;

			/** 活动总信息 */
			public const byte map_bc_info = 23;


		}
	}
}