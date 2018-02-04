using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//战斗效果（技能 buff 通用）
public enum FIGHT_EF_TPYE
{
    NONE = 0,
    ACTION,      //触发表演
    HURT,       //伤害
    ADD_BUFF,       //添加buff
    RE_BUFF,        //移除buff
    ARRTIBUTE,      //属性修改(改变血量等 一次性修改类型)
}

public class FightEffectDefine
{
    public static void DoEffect(ICharaBattle battleInfo, FightEffect effect)
    {
        if (null == battleInfo)
            return;

        switch (effect.Info.EffectType)
        {
            case FIGHT_EF_TPYE.NONE:
                break;
            case FIGHT_EF_TPYE.HURT:
                CalculateHurt(battleInfo, effect);
                break;
            case FIGHT_EF_TPYE.ADD_BUFF:
                AddBuff(battleInfo, effect);
                break;
            case FIGHT_EF_TPYE.RE_BUFF:
                RemoveBuff(battleInfo, effect);
                break;
            case FIGHT_EF_TPYE.ACTION:
                DoAction(battleInfo, effect);
                break;
            case FIGHT_EF_TPYE.ARRTIBUTE:
                ChangeAttribute(battleInfo, effect);
                break;
        }
    }

    public static void ChangeAttribute(ICharaBattle battleInfo, FightEffect effect)
    {
        //改变的属性选择
        switch ((ATTRIBUTE)effect.Info.Param1)
        {
            case ATTRIBUTE.ATTACK:
                break;
            case ATTRIBUTE.HP:
                break;
        }

        //按以下选项修改上面属性
        switch ((ATT_ALTER_TYPE)effect.Info.Param2)
        {
            case ATT_ALTER_TYPE.VALUE:
                break;
            case ATT_ALTER_TYPE.PRECENT:
                break;
        }
    }

    public static void DoAction(ICharaBattle battleInfo, FightEffect effect)
    {
      
        ICharaBattle user = ZTSceneManager.GetInstance().GetCharaById(effect.UserId) as ICharaBattle;
        SkillCommand skill = effect.TakeParam as SkillCommand;
        if (null != skill && null != user)
        {
            user.SkillCommand(skill);
        }
    }

    public static void CalculateHurt(ICharaBattle battleInfo, FightEffect effect)
    {
        ICharaFight target = battleInfo as ICharaFight;
        ICharaFight user = ZTSceneManager.GetInstance().GetCharaById(effect.UserId) as ICharaFight;
        if (null == target || null == user) return;
        HurtInfo hurtInfo = new HurtInfo();
        hurtInfo.Type = HURT_TYPE.NORMAL;
        hurtInfo.Value = user.Attack;
        battleInfo.AddHurt(hurtInfo);
    }

    public static void AddBuff(ICharaBattle battleInfo, FightEffect effect)
    {
        if (effect.UserId <= 0) return;
        battleInfo.AddBuff(new BuffData(effect.Info.Param1, ZTSceneManager.GetInstance().SceneFrame, effect.UserId));
    }

    public static void RemoveBuff(ICharaBattle battleInfo, FightEffect effect)
    {
        switch (effect.Info.Param1)
        {
            case 0:
                battleInfo.RemoveBuff(effect.Info.Param2);
                break;
            case 1:
                battleInfo.RemoveBuffByType(effect.Info.Param2);
                break;
        }
    }

}
