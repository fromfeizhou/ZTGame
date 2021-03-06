﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class JoystickBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public float outerCircleRadius = 100;
    public float activeMoveDistance = 20;
    public RectTransform innerCircleTrans;

    public Action<Vector2> onJoystickDownEvent;     // 按下事件
    public Action<Vector2> onJoystickUpEvent;     // 抬起事件
    public Action<Vector2> onJoystickMoveEvent;     // 滑动事件

    protected bool _isDownTouch;
    protected Canvas _canvas;

    public virtual void Start()
    {
		innerCircleTrans = transform.Find("InnerCircle") as RectTransform;
        _isDownTouch = false;
    }

    public virtual void OnDestroy()
    {
    }

	public void SetCanvas(Canvas canvas){
		this._canvas = canvas;
	}

    /// <summary>
    /// 按下
    /// </summary>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        _isDownTouch = true;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, _canvas.worldCamera, out pos);
        innerCircleTrans.anchoredPosition = pos;
        if(Vector2.Distance(innerCircleTrans.anchoredPosition, Vector2.zero) >= activeMoveDistance){
            if (onJoystickDownEvent != null)
                onJoystickDownEvent(innerCircleTrans.anchoredPosition / outerCircleRadius);
        }
        
    }

    /// <summary>
    /// 抬起
    /// </summary>
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDownTouch) return;
        _isDownTouch = false;
        if (onJoystickUpEvent != null)
            onJoystickUpEvent(innerCircleTrans.anchoredPosition / outerCircleRadius);

        //ui位置重置
        innerCircleTrans.anchoredPosition = Vector2.zero;
        
    }

    /// <summary>
    /// 滑动
    /// </summary>
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!_isDownTouch)
        {
            return;
        }
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, _canvas.worldCamera, out pos);
        if (Vector3.Distance(pos, Vector2.zero) < outerCircleRadius)
            innerCircleTrans.anchoredPosition = pos;
        else
            innerCircleTrans.anchoredPosition = pos.normalized * outerCircleRadius;

        if (Vector2.Distance(innerCircleTrans.anchoredPosition, Vector2.zero) >= activeMoveDistance)
        {
            if (onJoystickMoveEvent != null)
                onJoystickMoveEvent(innerCircleTrans.anchoredPosition / outerCircleRadius);
        }
        
    }
}