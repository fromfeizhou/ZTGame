using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

[XLua.LuaCallCSharp]
public class ZTDragItem : MonoBehaviour, IDragHandler,
    IEndDragHandler, IBeginDragHandler
{
   
     RectTransform targetRect;
     RectTransform itemRect;
    private Vector2 offset = new Vector3();
    public Action<Vector2> OnDragEvent;//返回拖拽中item对应的pos
    public Action<Vector2> OnDragEndEvent;//返回拖拽结束，鼠标点转换成target上的坐标

    void Start()
    {
    }

    public void Init(RectTransform target, RectTransform item)
    {
        targetRect = target;
        itemRect = item;
    }

    private bool isInit()
    {
        return targetRect == null || itemRect == null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isInit()) return;
        Vector2 mouseUguiPos = new Vector2();
        bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRect, eventData.position, eventData.enterEventCamera, out mouseUguiPos);
        if (isRect) 
            offset = itemRect.anchoredPosition - mouseUguiPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isInit()) return;
        Vector2 uguiPos = new Vector2();
        bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRect, eventData.position, eventData.enterEventCamera, out uguiPos);
        if (OnDragEvent != null)
            OnDragEvent(uguiPos+offset);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isInit()) return;
        offset = Vector2.zero;
        Vector2 uguiPos = new Vector2();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRect, eventData.position, eventData.enterEventCamera, out uguiPos);
        if (OnDragEndEvent != null)
            OnDragEndEvent(uguiPos);

    }

}