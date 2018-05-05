using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PoivtTest : MonoBehaviour {
	private const int pixel = 2048;
	public RectTransform mapRect;
	public ZTScrollRect scrollRect;

	void Start () {
		scrollRect.onEndDrag = onEndDrag;
	}

	void Update () {
		
	}

	public void onValueChanged(float value){
		mapRect.sizeDelta = Vector2.one * value * pixel;
	}
	public RectTransform point;
	public RectTransform center;
	private void onEndDrag(PointerEventData eventData){
		Vector2 tmpValue = new Vector2 ();
		tmpValue.x = mapRect.anchoredPosition.x - mapRect.sizeDelta.x * mapRect.pivot.x;
		tmpValue.y = mapRect.anchoredPosition.y - mapRect.sizeDelta.y * mapRect.pivot.y;
		point.anchoredPosition = tmpValue;
		center.anchoredPosition = Vector2.one * 400 - tmpValue;
	}
}
