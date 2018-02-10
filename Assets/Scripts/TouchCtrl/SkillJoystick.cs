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
        if (ZTSceneManager.GetInstance().MyPlayer.CanUseSkill())
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

        int distance = 6;

        //---------------test------------------------//

		SkillId = FightModule.GetInstance().CurSkillId;
        if (ZTSceneManager.GetInstance().MyPlayer.ActivateSkillId > 0)
        {
            SkillId = ZTSceneManager.GetInstance().MyPlayer.ActivateSkillId;
            ZTSceneManager.GetInstance().MyPlayer.ActivateSkillId = -1;
        }

        Vector3 targetPos =ZTSceneManager.GetInstance().MyPlayer.MovePos + new Vector3(distance * deltaVec.x, 0, distance * deltaVec.y);
        Vector3 dir = new Vector3(deltaVec.x, 0, deltaVec.y).normalized;

        //选择最近目标
        uint targetId =0;
        if (SkillId == 1001)
        {
            ICharaBattle battleInfo = SkillMethod.GetNearestTarget(ZTSceneManager.GetInstance().MyPlayer, SkillDefine.ColliderTarget.ENEMY);
            if (null == battleInfo || Vector3.Distance(ZTSceneManager.GetInstance().MyPlayer.MovePos, battleInfo.MovePos) > 6) return;

            targetId = battleInfo.BattleId;
            dir = (battleInfo.MovePos - ZTSceneManager.GetInstance().MyPlayer.MovePos).normalized;
        }
        BattleProtocol.GetInstance().SendSkillCommand(ZTSceneManager.GetInstance().MyPlayer.BattleId, SkillId, dir, targetPos, targetId);
        //---------------test------------------------//
        
        //SkillCommand command = FightDefine.GetSkillCommand(ZTSceneManager.GetInstance().MyPlayer.BattleId,ZTSceneManager.GetInstance().SceneFrame, SkillId, dir, targetPos, targetId);
        //SceneEvent.GetInstance().dispatchEvent(SCENE_EVENT.ADD_COMMAND, new Notification(command));
    }
}