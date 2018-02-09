using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharaBattle:IMove
{
    //战斗id
    uint BattleId { get; set; }

    //当前状态
    BATTLE_STATE BattleState { get;}
  
    //移动方向
    MOVE_DIR MoveDir { get; set; }
    float Speed { get; set; }

    //阵营
    int Camp { get; set; }
    //碰撞框
    CollRadius Collider { get; set; }

    void SetBattleInfo(uint battleId = 0, int camp = 0,Vector3 pos = default(Vector3));

    //改变状态
    void ChangeState(BATTLE_STATE state);
    //根据状态刷新
    void UpdateMoveState();

    //使用技能
    void SkillCommand(SkillCommand command);

    //移动
    void MoveCommand(MoveCommand command);
    //能否使用技能
    bool CanUseSkill();
    //能否移动
    bool CanMove();

    BuffCounter BattleBuffCouner { get; set; }

    void AddBuff(BuffData buffData);
    void RemoveBuff(int buffId);
    void RemoveBuffByType(int type);

    void ActivateSkill(int skillId);
    //临时使用 ActivateSkillId
    int ActivateSkillId { get; set; }

    int GrassId { get; set; }

    void SetDead(bool isDead);
    bool IsDead();
    void Reborn();
}
