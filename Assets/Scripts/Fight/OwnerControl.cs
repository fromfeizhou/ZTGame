using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnerControl : Singleton<OwnerControl>
{
    public JoystickBase _moveJoy;
    private bool _sendMove = false;
    private bool _sendMoveEvent = false;

    public override void Init()
    {
        base.Init();
        _moveJoy = GameObject.Find("UIManager/Canvas/MainPanel/MoveJoystick").GetComponent<JoystickBase>();
        if (null != _moveJoy)
        {
            InitEvent();
        }
    }

    public override void Destroy()
    {
        base.Destroy();
        if (null != _moveJoy)
        {
            RemoveEvent();
            _moveJoy = null;
        }
    }

    // Update is called once per frame
    public void Update()
    {
        //_playerBase.Update();通过场景刷新
#if UNITY_EDITOR
        KeyboardControl();
#endif

    }

    // Update is called once per frame
    private void InitEvent()
    {
        _moveJoy.onJoystickDownEvent += OnMoveDownEvent;
        _moveJoy.onJoystickMoveEvent += OnMoveMoveEvent;
        _moveJoy.onJoystickUpEvent += OnMoveUpEvent;

    }

    private void RemoveEvent()
    {
        _moveJoy.onJoystickDownEvent += OnMoveDownEvent;
        _moveJoy.onJoystickMoveEvent += OnMoveMoveEvent;
        _moveJoy.onJoystickUpEvent += OnMoveUpEvent;
        
    }


    void OnMoveDownEvent(Vector2 deltaVec)
    {
    }

    void OnMoveMoveEvent(Vector2 deltaVec)
    {
        float angle = Mathf.Atan2(deltaVec.y, deltaVec.x) * Mathf.Rad2Deg;
        if (angle >= -22.5 && angle < 22.5)
        {
            SendMoveCommond(MOVE_DIR.RIGHT);
        }
        else if (angle >= 22.5 && angle < 67.5)
        {
            SendMoveCommond(MOVE_DIR.UP_RIGHT);
        }
        else if (angle >= 67.5 && angle < 112.5)
        {
            SendMoveCommond(MOVE_DIR.UP);
        }
        else if (angle >= 112.5 && angle < 157.5)
        {
            SendMoveCommond(MOVE_DIR.UP_LEFT);
        }
        else if (angle >= -157.5 && angle < -112.5)
        {
            SendMoveCommond(MOVE_DIR.DOWN_LEFT);
        }
        else if (angle >= -112.5 && angle < -67.5)
        {
            SendMoveCommond(MOVE_DIR.DOWN);
        }
        else if (angle >= -67.5 && angle < -22.5)
        {
            SendMoveCommond(MOVE_DIR.DOWN_RIGHT);
        }
        else
        {
            SendMoveCommond(MOVE_DIR.LEFT);
        }
        _sendMoveEvent = true;
    }

    void OnMoveUpEvent(Vector2 deltaVec)
    {
        if (_sendMoveEvent)
        {
            SendMoveCommond(MOVE_DIR.NONE);
            _sendMoveEvent = false;
        }
    }

    //发送操作指令
    private void SendMoveCommond(MOVE_DIR dir)
    {
        int battleId = ZTSceneManager.GetInstance().MyPlayer.BattleId;
        if (battleId > 0)
        {
            MoveCommand command = FightDefine.GetMoveCommand(battleId, dir);
            SceneEvent.GetInstance().dispatchEvent(SceneEvents.ADD_COMMAND, new Notification(command));
        }
    }
  
    private void KeyboardControl()
    {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            SendMoveCommond(MOVE_DIR.UP_RIGHT);
            _sendMove = true;
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            SendMoveCommond(MOVE_DIR.UP_LEFT);
            _sendMove = true;
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            SendMoveCommond(MOVE_DIR.DOWN_LEFT);
            _sendMove = true;
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            SendMoveCommond(MOVE_DIR.DOWN_RIGHT);
            _sendMove = true;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            SendMoveCommond(MOVE_DIR.UP);
            _sendMove = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            SendMoveCommond(MOVE_DIR.DOWN);
            _sendMove = true;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            SendMoveCommond(MOVE_DIR.LEFT);
            _sendMove = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            SendMoveCommond(MOVE_DIR.RIGHT);
            _sendMove = true;
        }
        else if (_sendMove)
        {
            SendMoveCommond(MOVE_DIR.NONE);
            _sendMove = false;
        }
    }
}
