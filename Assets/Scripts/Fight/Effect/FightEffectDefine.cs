using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//战斗效果（技能 buff 通用）
public enum FIGHT_EF_TPYE
{
    NONE = 0,
    SHARK,         //震屏
    ACTION,      //触发表演
    ACTIVATE,       //激活技能
    HURT,       //伤害
    ADD_BUFF,       //添加buff
    RE_BUFF,        //移除buff
    ARRTIBUTE,      //属性修改(改变血量等 一次性修改类型)
}

public enum FIGHT_EF_TARGET
{
    SELF = 0,       //自己
    TARGET,         //目标
}

public class FightEffectDefine
{
    public static float IntToFloat = 100.0f;
    /// <summary>
    /// 解析技能效果
    /// </summary>
    /// <param name="battleInfo">生效者id</param> 
    /// <param name="effect">效果信息</param>
    /// <param name="userId">使用者id</param>
    /// <param name="dir">方向（击退 等带方向效果附带参数）</param>
    /// <param name="takeParam">技能 等附带参数</param>
    public static void ParseEffect(ICharaBattle battleInfo, FightEffectInfo effect, uint userId = 0, Vector3 dir = default(Vector3), object takeParam = null)
    {
        if (null == battleInfo)
            return;

        switch (effect.EffectType)
        {
            case FIGHT_EF_TPYE.NONE:
                break;
            case FIGHT_EF_TPYE.SHARK:
                SharkScreen(battleInfo, effect, userId);
                break;
            case FIGHT_EF_TPYE.HURT:
                CalculateHurt(battleInfo, effect, userId);
                break;
            case FIGHT_EF_TPYE.ADD_BUFF:
                AddBuff(battleInfo, effect, userId);
                break;
            case FIGHT_EF_TPYE.RE_BUFF:
                RemoveBuff(battleInfo, effect);
                break;
            case FIGHT_EF_TPYE.ACTION:
                DoAction(battleInfo, effect, userId, dir, takeParam);
                break;
            case FIGHT_EF_TPYE.ARRTIBUTE:
                ChangeAttribute(battleInfo, effect);
                break;
            case FIGHT_EF_TPYE.ACTIVATE:
                ActivateSkill(battleInfo, effect);
                break;
        }
    }

    //激活技能
    private static void ActivateSkill(ICharaBattle battleInfo, FightEffectInfo effect)
    {
        int skillId = effect.Param1;
        if (skillId > 0)
        {
            battleInfo.ActivateSkillId = skillId;
        }
    }

    //震屏
    private static void SharkScreen(ICharaBattle battleInfo, FightEffectInfo effect, uint userId)
    {
        //非玩家自己 不需要震动
        if (battleInfo.BattleId == ZTSceneManager.GetInstance().MyPlayer.BattleId || userId == ZTSceneManager.GetInstance().MyPlayer.BattleId)
        {
            int time = effect.Param1;
            float offset = effect.Param2 / IntToFloat;
            ZTSceneManager.GetInstance().SharkScreen(time, offset);
        }
    }

    private static void ChangeAttribute(ICharaBattle battleInfo, FightEffectInfo effect)
    {
        ICharaFight target = battleInfo as ICharaFight;
        if (null == target) return;

        float multi = 0;//乘数
        int value = 0;//值

        //按以下选项修改上面属性
        switch ((ATT_ALTER_TYPE)effect.Param2)
        {
            case ATT_ALTER_TYPE.VALUE:
                value = effect.Param3;
                break;
            case ATT_ALTER_TYPE.PRECENT:
                multi = effect.Param3 / 100.0f;
                break;
        }

        //改变的属性选择
        switch ((ATTRIBUTE)effect.Param1)
        {
            case ATTRIBUTE.ATTACK:
                target.Attack += Mathf.CeilToInt(target.Attack * multi) + value;
                break;
            case ATTRIBUTE.HP:
                int result =  Mathf.CeilToInt(target.MaxHp * multi) + value;
                HurtInfo hurtInfo = new HurtInfo();
                hurtInfo.Type = HURT_TYPE.NORMAL;
                hurtInfo.BattleId = battleInfo.BattleId;
                hurtInfo.Pos = battleInfo.MovePos;
                hurtInfo.Value = result;
                target.AddHurt(hurtInfo);
                break;
        }
    }

    private static void DoAction(ICharaBattle battleInfo, FightEffectInfo effect, uint userId = 0, Vector3 dir = default(Vector3), object takeParam = null)
    {

        SkillCommand skill = takeParam as SkillCommand;
        if (null != skill && null != battleInfo)
        {
            battleInfo.SkillCommand(skill);
        }
    }

    private static void CalculateHurt(ICharaBattle battleInfo, FightEffectInfo effect, uint userId)
    {
        ICharaFight target = battleInfo as ICharaFight;
        ICharaFight user = ZTSceneManager.GetInstance().GetCharaById(userId) as ICharaFight;
        if (null == target || null == user) return;
        HurtInfo hurtInfo = new HurtInfo();
        hurtInfo.Type = HURT_TYPE.NORMAL;
        hurtInfo.BattleId = battleInfo.BattleId;
        hurtInfo.Pos = battleInfo.MovePos;
        hurtInfo.Value = -user.Attack;

        user.AddHurt(hurtInfo);
    }

    private static void AddBuff(ICharaBattle battleInfo, FightEffectInfo effect, uint userId)
    {
        if (userId <= 0) return;
        battleInfo.AddBuff(new BuffData(effect.Param1, ZTSceneManager.GetInstance().SceneFrame, userId));
    }

    private static void RemoveBuff(ICharaBattle battleInfo, FightEffectInfo effect)
    {
        switch ((BUFF_REMOVE_TYPE)effect.Param1)
        {
            case BUFF_REMOVE_TYPE.ID:
                battleInfo.RemoveBuff(effect.Param2);
                break;
            case BUFF_REMOVE_TYPE.TYPE:
                battleInfo.RemoveBuffByType(effect.Param2);
                break;
        }
    }

}
