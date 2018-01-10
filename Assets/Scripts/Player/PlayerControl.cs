using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class PlayerControl : MonoBehaviour
{
    public PlayerBase _playerBase = null;

    public Animation _playerAnimation = null;
    private bool _sendMove = false;
    void Awake()
    {
        //_playerBase = new PlayerBase();
        //_playerAnimation.GetComponent<PlayerAnimCtrl>().SetPlayerData(_playerBase);
      
    }
    // Use this for initialization

    public void SetPlayerData(PlayerBase playerData)
    {
        _playerBase = playerData;
        _playerBase.Target = gameObject;
        gameObject.GetComponent<PlayerAnimCtrl>().SetPlayerData(_playerBase);
        if (null != _playerBase && _playerBase.Id == 1)
        {
            InitControlEvent();
        }
    }
    void Start()
    {

    }

    void OnDestroy()
    {
        if (null != _playerBase && _playerBase.Id == 1)
        {
            RemoveControlEvent();
            _playerBase = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //_playerBase.Update();通过场景刷新
#if UNITY_EDITOR
        KeyboardControl();
#endif

    }

    //初始化控制事件
    private void InitControlEvent()
    {
        TouchEvent.GetInstance().addEventListener(GameTouchEvents.JOY_MOVE, OnJoveMoveHandler);
    }

    //移除控制事件
    private void RemoveControlEvent()
    {
        TouchEvent.GetInstance().removeEventListener(GameTouchEvents.JOY_MOVE, OnJoveMoveHandler);
    }


    private void OnJoveMoveHandler(Notification data)
    {
        if (null == _playerBase) return;

        FightDefine.PLAYERDIR dir = (FightDefine.PLAYERDIR)data.param;
        _playerBase.StartMove(dir);
    }

    private void KeyboardControl()
    {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            DispatchDirEvent(FightDefine.PLAYERDIR.UP_RIGHT);
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            DispatchDirEvent(FightDefine.PLAYERDIR.UP_LEFT);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            DispatchDirEvent(FightDefine.PLAYERDIR.DOWN_LEFT);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            DispatchDirEvent(FightDefine.PLAYERDIR.DOWN_RIGHT);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            DispatchDirEvent(FightDefine.PLAYERDIR.UP);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            DispatchDirEvent(FightDefine.PLAYERDIR.DOWN);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            DispatchDirEvent(FightDefine.PLAYERDIR.LEFT);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            DispatchDirEvent(FightDefine.PLAYERDIR.RIGHT);
        }
        else if (_sendMove)
        {
            DispatchDirEvent(FightDefine.PLAYERDIR.NONE);
        }
    }

    private void DispatchDirEvent(FightDefine.PLAYERDIR moveDir)
    {
        if (moveDir == FightDefine.PLAYERDIR.NONE)
        {
            _sendMove = false;
        }
        else
        {
            _sendMove = true;
        }
        TouchEvent.GetInstance().dispatchEvent(GameTouchEvents.JOY_MOVE, new Notification(moveDir));
    }
}
