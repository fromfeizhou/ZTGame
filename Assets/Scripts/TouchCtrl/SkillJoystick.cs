using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SkillJoystick : JoystickBase
{

    public int SkillId;
    private int SkillDis;

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
        if (ZTSceneManager.GetInstance().MyPlayer.PlayerState == PLAYERSTATE.NONE || ZTSceneManager.GetInstance().MyPlayer.PlayerState == PLAYERSTATE.MOVE)
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
        //记录操作
        int frame = ZTSceneManager.GetInstance().SceneFrame;

        Vector3 myPos = ZTSceneManager.GetInstance().MyPlayer.PlayerPos;
        int distance = 10;

        //---------------test------------------------//
        SkillId = SkillSelected.SelectIndex;
        //---------------test------------------------//

        Vector3 targetPos = new Vector3(myPos.x + distance * deltaVec.x / outerCircleRadius, 0, myPos.z + distance * deltaVec.y / outerCircleRadius);
        ZTSceneManager.GetInstance().PlayerUseSkill(1, new SkillOpera(SkillId, frame, new Vector3(deltaVec.x, 0, deltaVec.y).normalized, targetPos));
    }
}