using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SkillJoystick : JoystickBase
{
    RectTransform innerCircleTrans;
    RectTransform _rectTransform;

    private Image _imgBg;
    private Image _imgTouch;

    public override void Start()
    {
        _rectTransform = this.gameObject.GetComponent<RectTransform>();
        _imgBg = this.gameObject.GetComponent<Image>();

        innerCircleTrans = transform.Find("InnerCircle") as RectTransform;
        _imgTouch = innerCircleTrans.gameObject.GetComponent<Image>();

        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _isDownTouch = false;
        SetVisibleOpacity(false);
    }


    /// <summary>
    /// 按下
    /// </summary>
    public override void OnPointerDown(PointerEventData eventData)
    {
        _isDownTouch = true;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, _canvas.worldCamera, out pos);
        _rectTransform.anchoredPosition = pos;
        innerCircleTrans.anchoredPosition = Vector2.zero;
        if (onJoystickDownEvent != null)
        {
            onJoystickDownEvent(Vector2.zero);
        }
        SetVisibleOpacity(true);
    }

    /// <summary>
    /// 抬起
    /// </summary>
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDownTouch) return;
        _isDownTouch = false;
        if (onJoystickUpEvent != null)
            onJoystickUpEvent(innerCircleTrans.anchoredPosition / outerCircleRadius);
        //ui位置重置
        innerCircleTrans.anchoredPosition = Vector2.zero;
        _rectTransform.anchoredPosition = Vector2.zero;
        SetVisibleOpacity(false);
    }


    /// <summary>
    /// 滑动
    /// </summary>
    public override void OnDrag(PointerEventData eventData)
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


    private void SetVisibleOpacity(bool isShow)
    {
        if (isShow)
        {
            _imgBg.color = new Color32(255, 255, 255, 255);
            _imgTouch.color = new Color32(0, 213, 255, 255);
        }
        else
        {
            _imgBg.color = new Color32(255, 255, 255, 1);
            _imgTouch.color = new Color32(0, 213, 255, 1);
        }
    }


}
