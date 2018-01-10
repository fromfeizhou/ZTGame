using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class JoystickCc : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public float outerCircleRadius = 50;
    public float activeMoveDistance = 15;
    RectTransform innerCircleTrans;

    Vector2 outerCircleStartWorldPos = Vector2.zero;
    private bool _sendMoveEvent;

    void Awake()
    {
        innerCircleTrans = transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        _sendMoveEvent = false;
    }

    void Start()
    {
        outerCircleStartWorldPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
    }
  
    /// <summary>
    /// 按下
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        innerCircleTrans.anchoredPosition = (eventData.position - outerCircleStartWorldPos);
    }

    /// <summary>
    /// 抬起
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        innerCircleTrans.anchoredPosition = Vector2.zero;
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
        if (Vector2.Distance(touchPos, Vector2.zero) < outerCircleRadius)
            innerCircleTrans.anchoredPosition = touchPos;
        else
            innerCircleTrans.anchoredPosition = touchPos.normalized * outerCircleRadius;
        DispatchDirEvent();
    }


    void DispatchDirEvent()
    {
        Vector2 curPos = innerCircleTrans.anchoredPosition;
        if (Vector2.Distance(curPos, Vector2.zero) >= activeMoveDistance)
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
