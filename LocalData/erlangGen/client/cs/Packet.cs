/**
*Packet  自动生成,请勿编辑
*/

using System;
using System.Collections.Generic;
namespace Assets.Scripts.Com.Game.Config
{
    [Serializable]
    public class SysEquipment
    {
		public string unikey;
		public int id; //装备ID
		public string name; //名称
		public int type; //类型
		public string description; //描述
		public int professional; //职业
		public int sex; //性别
		public int superposition; //叠加
		public int hp; //生命
		public int damage_min; //最小攻击
		public int damage_max; //最大攻击
		public int defense; //防御
		public int attack_speed; //攻速
		public int life_back; //生命恢复
		public int energy_max; //能量上限
		public int lucky; //幸运值
		public int curse; //诅咒值
		public int move; //移动
		public int wound_deeper; //伤害加成
		public int damage_reduction; //伤害减免
		public int skills_cd; //技能冷却
    }
}
