using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleInfo : CharaPlayerInfo, ICharaBattle
{

    //**===================接口实现=======================**//
    //战斗id
    public uint BattleId { get; set; }

    //角色状态
    private BATTLE_STATE _battleState;
    public BATTLE_STATE BattleState
    {

        get { return _battleState; }
    }


    //移动方向
    MOVE_DIR _moveDir;
    public MOVE_DIR MoveDir
    {
        get { return _moveDir; }
        set { if (_moveDir != value)  _moveDir = value; }
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
                Collider.MovePos = _charaPos;
                this.UpdatePos(_charaPos);
            }
        }
    }

    //阵营
    public int Camp { get; set; }
    //碰撞框
    public CollRadius Collider { get; set; }

    private int _grassId;
    //草丛id
    public int GrassId
    {
        get { return _grassId; }
        set
        {
            if (_grassId != value)
            {
                _grassId = value;
                SceneEvent.GetInstance().dispatchEvent(SCENE_EVENT.UPDATE_GRASS_ID,new Notification(BattleId));
            }
        }
    }

    //状态改变
    public void ChangeState(BATTLE_STATE state)
    {
        if (_battleState == state) return;
        switch (state)
        {
            case BATTLE_STATE.NONE:
                _battleState = state;
                break;
            case BATTLE_STATE.SKILL:
                if (CanUseSkill()) _battleState = state;
                break;
            case BATTLE_STATE.MOVE:
                if (CanMove()) _battleState = state;
                this.PlayAction(PLAYER_AC_NAME.RUN);
                break;
            case BATTLE_STATE.DIE:
                this.PlayAction(PLAYER_AC_NAME.DIE);
                _battleState = state;
                break;
            default:
                _battleState = state;
                break;
        }

    }

    //每帧更新
    public override void UpdateFrame()
    {
        base.UpdateFrame();
        this.UpdateMoveState();
        if (null != BattleBuffCouner)
        {
            BattleBuffCouner.Update();
        }
    }

    private MapBlockData _mapBlockData;
    private Vector3 _hitPos;
    //根据状态刷新(位置)
    public void UpdateMoveState()
    {
        if (BattleState == BATTLE_STATE.MOVE && MoveDir != MOVE_DIR.NONE)
        {
            this.ChangeRotate(CharaDefine.GetDirVec(MoveDir));
            List<Vector3> list = CharaDefine.GetDirMoveVecs(MoveDir);
            for (int i = 0; i < list.Count; i++)
            {
                bool isMove = false;
                _hitPos = MovePos + list[i] * Speed;
                _mapBlockData = MapManager.GetInstance().GetCurMapBlock(_hitPos);
                if (null != _mapBlockData)
                {
                    //草丛判断
                    if (_mapBlockData.type == eMapBlockType.Hide)
                    {
                        GrassId = int.Parse(_mapBlockData.param);
                    }
                    else
                    {
                        GrassId = -1;
                    }

                    //碰撞判断
                    if (_mapBlockData.type != eMapBlockType.Collect)
                    {
                        MovePos = _hitPos;
                        isMove = true;
                    }
                }
                else
                {
                    MovePos = _hitPos;
                    GrassId = -1;
                    isMove = true;
                }
                if (isMove) return;
            }
        }
    }

    //使用技能
    public void SkillCommand(SkillCommand command)
    {
        if (CanUseSkill() && null!= command)
        {
            //BattleState = BATTLE_STATE.SKILL;
            SceneEvent.GetInstance().dispatchEvent(SCENE_EVENT.ADD_SKILL_PARSER, new Notification(command));
        }
    }

    public void MoveCommand(MoveCommand command)
    {
        if (CanMove() && null != command)
        {
            MoveDir = command.MoveDir;
            if (MoveDir == MOVE_DIR.NONE)
            {
                this.PlayAction(PLAYER_AC_NAME.IDLE);
                ChangeState(BATTLE_STATE.NONE);
            }
            else
            {
                ChangeState(BATTLE_STATE.MOVE);
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

    //buff计数器 管理buff
    public BuffCounter BattleBuffCouner { get; set; }

    public void AddBuff(BuffData buffData)
    {
        if (null == buffData) return;

        Buff buffInfo = BuffDefine.GetBuffInfo(buffData);
        BattleBuffCouner.AddBuff(buffInfo);
        buffInfo.Start();
    }
    //移除buff
    public void RemoveBuff(int buffId)
    {
        BattleBuffCouner.RemoveBuffById(buffId);
    }
    public void RemoveBuffByType(int type)
    {
        BattleBuffCouner.RemoveBuffByType(type);
    }

    //临时使用
    public void ActivateSkill(int skillId)
    {
        ActivateSkillId = skillId;
    }
    public int ActivateSkillId { get; set; }

    public void SetDead(bool isDead) {
        if (isDead)
        {
            ChangeState(BATTLE_STATE.DIE);
        }
        else
        {
            ChangeState(BATTLE_STATE.NONE);
        }
    }
    //是否死亡 
    public bool IsDead()
    {
        return BattleState == BATTLE_STATE.DIE;
    }

    //复活
    public void Reborn()
    {
        ChangeState(BATTLE_STATE.NONE);
        ICharaFight info = this as ICharaFight;
        if (null != info) { info.Hp = MaxHp; }
    }

    public void SetBattleInfo(uint battleId = 0, int camp = 0, Vector3 pos = default(Vector3))
    {
        BattleId = battleId;
        Camp = camp;
        MoveDir = MOVE_DIR.NONE;
        Speed = CharaDefine.PLAYER_SPEED;
        Collider = new CollRadius(0, 0, 0, CharaDefine.PLAYER_RADIUS);
        MovePos = pos;
        BattleBuffCouner = new BuffCounter(this);

        ChangeState(BATTLE_STATE.NONE);
    }


    //**===================接口实现=======================**//
    public PlayerBattleInfo(int animaId, CHARA_TYPE type)
        : base(animaId, CHARA_TYPE.PLAYER)
    {
    }

    public override void Destroy()
    {
        Debug.Log("PlayerBattleInfo Destroy ");
        if (null != BattleBuffCouner)
        {
            BattleBuffCouner.Destroy();
            BattleBuffCouner = null;
        }
        base.Destroy();
    }
}
