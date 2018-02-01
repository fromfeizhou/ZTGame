using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SkillJoystick : JoystickBase
{

    public int SkillId;

    private bool _skillDownState;

    public override void Start()
    {
        base.Start();
        SkillId = 10001;
        this.onJoystickDownEvent += OnJoystickDownEvent;
        this.onJoystickUpEvent += OnJoystickUpEvent;

        _skillDownState = false;
    }

    public override void OnDestroy()
    {
        this.onJoystickDownEvent -= OnJoystickDownEvent;
        this.onJoystickUpEvent += OnJoystickUpEvent;
        base.OnDestroy();
    }

    void OnJoystickDownEvent(Vector2 deltaVec)
    {
        if (ZTSceneManager.GetInstance().MyPlayer.BattleState == BATTLE_STATE.NONE || ZTSceneManager.GetInstance().MyPlayer.BattleState == BATTLE_STATE.MOVE)
        {
            _skillDownState = true;
        }
    }

    void OnJoystickUpEvent(Vector2 deltaVec)
    {
        if (!_skillDownState)
        {
            return;
        }
        _skillDownState = false;

        int distance = 10;

        //---------------test------------------------//
        SkillId = SkillSelected.SelectIndex;
        //---------------test------------------------//
        Vector3 targetPos = new Vector3(distance * deltaVec.x, 0, distance * deltaVec.y);
        Vector3 dir = new Vector3(deltaVec.x, 0, deltaVec.y).normalized;
        SkillCommand command = FightDefine.GetSkillCommand(ZTSceneManager.GetInstance().MyPlayer.BattleId,SkillId,dir,targetPos);
        SceneEvent.GetInstance().dispatchEvent(SCENE_EVENT.ADD_COMMAND, new Notification(command));
    }
}