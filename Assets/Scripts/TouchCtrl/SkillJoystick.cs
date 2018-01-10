using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SkillJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

    public float outerCircleRadius = 100;

    Transform innerCircleTrans;

    Vector2 outerCircleStartWorldPos = Vector2.zero;

    public Action<Vector2> onJoystickDownEvent;     // 按下事件
    public Action onJoystickUpEvent;     // 抬起事件
    public Action<Vector2> onJoystickMoveEvent;     // 滑动事件

    private bool _isDownTouch;
    void Awake()
    {
        innerCircleTrans = transform.GetChild(0);
    }

    void Start()
    {
        outerCircleStartWorldPos = transform.position;
        _isDownTouch = false;
    }

    /// <summary>
    /// 按下
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        _isDownTouch = true;
        innerCircleTrans.position = eventData.position;
        if (onJoystickDownEvent != null)
            onJoystickDownEvent(innerCircleTrans.localPosition / outerCircleRadius);
    }

    /// <summary>
    /// 抬起
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDownTouch) return;
        _isDownTouch = false;
        //发送技能操作
        Vector2 touchPos = eventData.position - outerCircleStartWorldPos;
        if (Vector3.Distance(touchPos, Vector2.zero) > outerCircleRadius)
            touchPos = touchPos.normalized * outerCircleRadius;

        //记录操作
        int frame = SceneManager.GetInstance().SceneFrame;

        Vector3 myPos = SceneManager.GetInstance().MyPlayer.PlayerPos;
        int distance = 10;
        Vector3 targetPos = new Vector3(myPos.x + distance * touchPos.x / outerCircleRadius, 0,myPos.z + distance * touchPos.y / outerCircleRadius);
        SceneManager.GetInstance().PlayerUseSkill(1, new SkillOpera(10001, frame, new Vector3(touchPos.x, 0, touchPos.y).normalized, targetPos));

        //ui位置重置
        innerCircleTrans.localPosition = Vector3.zero;
        if (onJoystickUpEvent != null)
            onJoystickUpEvent();
    }

    /// <summary>
    /// 滑动
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDownTouch)
        {
            return;
        }
        Vector2 touchPos = eventData.position - outerCircleStartWorldPos;
        if (Vector3.Distance(touchPos, Vector2.zero) < outerCircleRadius)
            innerCircleTrans.localPosition = touchPos;
        else
            innerCircleTrans.localPosition = touchPos.normalized * outerCircleRadius;

        if (onJoystickMoveEvent != null)
            onJoystickMoveEvent(innerCircleTrans.localPosition / outerCircleRadius);
    }
}