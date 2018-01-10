using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class JoystickCc : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public float outerCircleRadius = 50;
    public float activeMoveDistance = 15;
    Transform innerCircleTrans;

    Vector2 outerCircleStartWorldPos = Vector2.zero;
    private bool _sendMoveEvent;

    void Awake()
    {
        innerCircleTrans = transform.GetChild(0);
        _sendMoveEvent = false;
    }

    void Start()
    {
        outerCircleStartWorldPos = transform.position;
    }
  
    /// <summary>
    /// 按下
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        innerCircleTrans.position = eventData.position;
    }

    /// <summary>
    /// 抬起
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        innerCircleTrans.localPosition = Vector3.zero;
        if (_sendMoveEvent)
        {
            TouchEvent.GetInstance().dispatchEvent(GameTouchEvents.JOY_MOVE, new Notification(FightDefine.PLAYERDIR.NONE));
            _sendMoveEvent = false;
        }
    }

    /// <summary>
    /// 滑动
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 touchPos = eventData.position - outerCircleStartWorldPos;
        if (Vector3.Distance(touchPos, Vector2.zero) < outerCircleRadius)
            innerCircleTrans.localPosition = touchPos;
        else
            innerCircleTrans.localPosition = touchPos.normalized * outerCircleRadius;
        DispatchDirEvent();
    }


    void DispatchDirEvent()
    {
        Vector3 curPos = innerCircleTrans.localPosition;
        if (Vector3.Distance(outerCircleStartWorldPos, curPos) >= activeMoveDistance)
        {
            float angle = Mathf.Atan2(curPos.y, curPos.x) * Mathf.Rad2Deg;
            if (angle >= -22.5 && angle < 22.5)
            {
                TouchEvent.GetInstance().dispatchEvent(GameTouchEvents.JOY_MOVE, new Notification(FightDefine.PLAYERDIR.RIGHT));
            }
            else if (angle >= 22.5 && angle < 67.5)
            {
                TouchEvent.GetInstance().dispatchEvent(GameTouchEvents.JOY_MOVE, new Notification(FightDefine.PLAYERDIR.UP_RIGHT));
            }
            else if (angle >= 67.5 && angle < 112.5)
            {
                TouchEvent.GetInstance().dispatchEvent(GameTouchEvents.JOY_MOVE, new Notification(FightDefine.PLAYERDIR.UP));
            }
            else if (angle >= 112.5 && angle < 157.5)
            {
                TouchEvent.GetInstance().dispatchEvent(GameTouchEvents.JOY_MOVE, new Notification(FightDefine.PLAYERDIR.UP_LEFT));
            }
            else if (angle >= -157.5 && angle < -112.5)
            {
                TouchEvent.GetInstance().dispatchEvent(GameTouchEvents.JOY_MOVE, new Notification(FightDefine.PLAYERDIR.DOWN_LEFT));
            }
            else if (angle >= -112.5 && angle < -67.5)
            {
                TouchEvent.GetInstance().dispatchEvent(GameTouchEvents.JOY_MOVE, new Notification(FightDefine.PLAYERDIR.DOWN));
            }
            else if (angle >= -67.5 && angle < -22.5)
            {
                TouchEvent.GetInstance().dispatchEvent(GameTouchEvents.JOY_MOVE, new Notification(FightDefine.PLAYERDIR.DOWN_RIGHT));
            }
            else
            {
                TouchEvent.GetInstance().dispatchEvent(GameTouchEvents.JOY_MOVE, new Notification(FightDefine.PLAYERDIR.LEFT));
            }
            _sendMoveEvent = true;
        }
    }
}
