using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[XLua.LuaCallCSharp]
public class ZTLine : ZTImage {

	private float _width;

	public void SetLineWidth(float width){
		_width = width;
	}
	public void SetLine(Vector2 posFrom, Vector2 posTo){
		rectTransform.sizeDelta = new Vector2 (_width, Vector2.Distance (posFrom, posTo));
		rectTransform.anchoredPosition = posFrom;
		Vector2 div = posTo - posFrom;
		float angle = Mathf.Atan2 (div.y, div.x) * Mathf.Rad2Deg - 90;
		rectTransform.localEulerAngles = Vector3.forward * angle;
	}

	public void SetLine(Vector2 posFrom, Vector2 posTo, float distance){
		rectTransform.sizeDelta = new Vector2 (_width, distance);
		rectTransform.anchoredPosition = posFrom;
		Vector2 div = posTo - posFrom;
		float angle = Mathf.Atan2 (div.y, div.x) * Mathf.Rad2Deg - 90;
		rectTransform.localEulerAngles = Vector3.forward * angle;
	}
}
