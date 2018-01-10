using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PLAYERSTATE
{
    NONE = 0,
    STAND,
    MOVE,
    ATTACK,
    HIT,
    SKILL,
    STUN,
    DODGE,
    REPEL,
    DIE
}



//test
public class PlayerBaseData
{
    public int Id { get; set; }     //玩家id
    public int Camp { get; set; }       //玩家阵营
    public Vector3 PlayerPos { get; set; }
    public PlayerBaseData(int id, int camp, Vector3 pos)
    {
        Id = id;
        Camp = camp;
        PlayerPos = pos;
    }
}


//英雄基础类，保存英雄基础属性，并进行基础操作
public class PlayerBase : NotificationDelegate, IMove
{
    public static float PLAYER_SPEED = 0.05f;      //移动速度
    public static float PLAYER_RADIUS = 1f;       //碰撞半径
    //----------------------------------------------------------------------------------------------------------------------//
    //------------------------------------------------基本属性Begin-----------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------//
    public GameObject Target;//挂钩对象

    public int Id { get; set; }     //玩家id
    public int Camp { get; set; }       //玩家阵营
    public CollRadius Collider = new CollRadius(0, 0, 0, PlayerBase.PLAYER_RADIUS);      //玩家碰撞
    public float MoveSpeed { get; set; }         //每帧移动速度
    public FightDefine.PLAYERDIR MoveDir { get; set; }        //移动方向

    private PLAYERSTATE _state;     //玩家状态
    private Vector3 _playerPos = new Vector3(0, 0, 0);      //位置

    //移动接口实现
    public Vector3 MovePos { get { return _playerPos; } set { PlayerPos = value; } }

    private List<SkillActionParser> _skillParserList;

    //数据赋值
    public void SetPlayerBaseData(PlayerBaseData data)
    {
        Id = data.Id;
        Camp = data.Camp;
        PlayerPos = data.PlayerPos;
    }
    //----------------------------------------------------------------------------------------------------------------------//
    //------------------------------------------------基本属性End-----------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------//
    public PLAYERSTATE PlayerState
    {
        get { return _state; }
        set
        {
            if (_state != value)
            {
                if (value == PLAYERSTATE.MOVE)
                {
                    this.dispatchEvent(PlayerAnimEvents.PLAY, new Notification(PlayerActionName.RUN));
                }
                else if (value == PLAYERSTATE.ATTACK)
                {
                    this.dispatchEvent(PlayerAnimEvents.PLAY, new Notification(PlayerActionName.ATTACK_1));
                }
                else
                {
                    this.dispatchEvent(PlayerAnimEvents.PLAY, new Notification(PlayerActionName.IDLE_1));
                }
            }
            _state = value;
        }
    }

    public Vector3 PlayerPos
    {
        get
        {
            return _playerPos;
        }
        set
        {
            if (!_playerPos.Equals(value))
            {
                MapManager.GetInstance().SetMapCenterPos(new Vector3(value.x, value.y, value.z));
                _playerPos = value;
                //碰撞跟随移动
                Collider.MovePos = value;
                this.dispatchEvent(PlayerAnimEvents.UPDATE_POS);
            }
        }
    }

    //----------------------------------------------------------------------------------------------------------------------//
    //------------------------------------------------Public Func-----------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------//

    /// 构建函数
    public PlayerBase(int id = 0)
    {
        Id = id;
        PlayerState = PLAYERSTATE.NONE;
        MoveSpeed = PLAYER_SPEED;
        _skillParserList = new List<SkillActionParser>();
        InitEvent();
    }

    private void InitEvent()
    {
        this.addEventListener(FightDefine.FightOperaEvents.PLAY_SKILL, PlayerUsetSkill);
    }
    private void RemoveEvent()
    {
        this.removeEventListener(FightDefine.FightOperaEvents.PLAY_SKILL, PlayerUsetSkill);
    }


    private void PlayerUsetSkill(Notification data)
    {
        SkillOpera opera = data.param as SkillOpera;
        if (null != opera)
        {
            SkillActionParser skillParser = new SkillActionParser(this, opera);
            _skillParserList.Add(skillParser);
        }
    }

    public void Destroy()
    {
        RemoveEvent();
    }

    //每帧刷新
    public void Update()
    {
        if (PlayerState == PLAYERSTATE.MOVE)
        {
            UpdatePos();
        }

        for (int i = 0; i < _skillParserList.Count; i++)
        {
            _skillParserList[i].UpdateAction();
        }

    }

    /// 刷新坐标
    public void UpdatePos()
    {
        if (MoveDir != FightDefine.PLAYERDIR.NONE)
        {
            PlayerPos += FightDefine.GetDirVec(MoveDir) * MoveSpeed;
        }
    }

    /// 移动
    public void StartMove(FightDefine.PLAYERDIR moveDir = FightDefine.PLAYERDIR.NONE)
    {
        if (CanMove())
        {
            if (moveDir == FightDefine.PLAYERDIR.NONE)
            {
                StopMove();
            }
            else
            {
                PlayerState = PLAYERSTATE.MOVE;
                MoveDir = moveDir;
            }
        }
    }

    public void StopMove()
    {
        if (PlayerState == PLAYERSTATE.MOVE)
        {
            PlayerState = PLAYERSTATE.NONE;
        }
    }



    //----------------------------------------------------------------------------------------------------------------------//
    //------------------------------------------------Private Func-----------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------//

    /// <summary>
    /// 能否移动
    /// </summary>
    /// <returns></returns>
    private bool CanMove()
    {
        if (PlayerState == PLAYERSTATE.NONE || PlayerState == PLAYERSTATE.STAND || PlayerState == PLAYERSTATE.MOVE)
        {
            return true;
        }
        return false;
    }
}
