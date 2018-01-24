using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleInfo : CharaPlayerInfo, ICharaBattle
{

    //**===================接口实现=======================**//
    //战斗id
    public int BattleId { get; set; }

    //角色状态
    private BATTLE_STATE _battleState;
    public BATTLE_STATE BattleState {
        set
        {
            if (_battleState == value) return;
            switch(value)
            {
                case BATTLE_STATE.NONE:
                    this.PlayAction(PlayerActionName.IDLE);
                    _battleState = value;
                    break;
                case BATTLE_STATE.SKILL:
                    if(CanUseSkill()) _battleState = value;
                    break;
                case BATTLE_STATE.MOVE:
                    if(CanMove()) _battleState = value;
                    this.PlayAction(PlayerActionName.RUN);
                    break;
                default:
                    _battleState = value;
                    break;
            }

        }
        get { return _battleState; }
    }


    //移动方向
    MOVE_DIR _moveDir;
    public MOVE_DIR MoveDir
    {
        get { return _moveDir; }
        set { if (_moveDir != value) _moveDir = value; }
    }
    public float Speed { get; set; }

    Vector3 _charaPos;
    public Vector3 MovePos
    {
        get { return _charaPos; }
        set
        {
            if (!_charaPos.Equals(value))
            {
                _charaPos = value;
                this.UpdatePos(_charaPos);
            }
        }
    }

    //阵营
    public int Camp { get; set; }
    //碰撞框
    public CollRadius Collider { get; set; }

    //状态改变
    public void ChangeState(BATTLE_STATE state)
    {
        _battleState = state;
        switch (state)
        {
            case BATTLE_STATE.MOVE:
                this.PlayAction(PlayerActionName.RUN);
                break;
            case BATTLE_STATE.DIE:
                this.PlayAction(PlayerActionName.DIE);
                break;
            case BATTLE_STATE.NONE:
                this.PlayAction(PlayerActionName.IDLE);
                break;
        }
    }

    //每帧更新
    public override void UpdateFrame()
    {
        base.UpdateFrame();
        this.UpdateMoveState();
    }

    private eMapBlockType _mapBlockType;
    private Vector3 _hitPos;
    //根据状态刷新(位置)
    public void UpdateMoveState()
    {
        if (BattleState == BATTLE_STATE.MOVE && MoveDir != MOVE_DIR.NONE)
        {
            this.ChangeRotate(CharaDefine.GetDirVec(MoveDir));
            _hitPos = MovePos + CharaDefine.GetDirVec(MoveDir) * Speed;
            _mapBlockType = MapManager.GetInstance().GetFloorColl(_hitPos);
            if (_mapBlockType == eMapBlockType.Collect) return;
            MovePos = _hitPos;
        }
    }

    //使用技能
    public void SkillCommand(SkillCommand command)
    {
        if (CanUseSkill())
        {
            //BattleState = BATTLE_STATE.SKILL;
        }
    }

    public void MoveCommand(MoveCommand command)
    {
        if (CanMove())
        {
            MoveDir = command.MoveDir;
            if (MoveDir == MOVE_DIR.NONE)
            {
                BattleState = BATTLE_STATE.NONE;
            }
            else
            {
                BattleState = BATTLE_STATE.MOVE;
            }
        }
    }

    //能否使用技能
    public bool CanUseSkill()
    {
        if (BattleState == BATTLE_STATE.MOVE || BattleState == BATTLE_STATE.NONE) return true;
        return false;
    }
    //能否移动
    public bool CanMove()
    {
        if (BattleState == BATTLE_STATE.MOVE || BattleState == BATTLE_STATE.NONE) return true;
        return false;
    }

    public void SetBattleInfo(int battleId = 0, int camp = 0,Vector3 pos = default(Vector3))
    {
        BattleId = battleId;
        Camp = camp;
        MoveDir = MOVE_DIR.NONE;
        Speed = CharaDefine.PLAYER_SPEED;
        Collider = new CollRadius(0, 0, 0, CharaDefine.PLAYER_RADIUS);
        MovePos = pos;
        Debug.Log(MovePos);
        ChangeState(BATTLE_STATE.NONE);
    }

    //**===================接口实现=======================**//
    public PlayerBattleInfo(int animaId, CHARA_TYPE type)
        : base(animaId, CHARA_TYPE.PLAYER)
    {
    }

    public override void Destroy()
    {
        base.Destroy();
    }
}
