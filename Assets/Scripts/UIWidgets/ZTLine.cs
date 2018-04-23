using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;
using Vectrosity;

[LuaCallCSharp]
public class ZTLine : MonoBehaviour {
	private VectorLine line;
	public Texture tex;
	private List<RectTransform> rectLineList = new List<RectTransform> ();

	private bool _show = false;

	/** 初始化 */
	public void Init(Texture tex, bool awakeShow = false, float lineWidth = 6.0f)
	{
		gameObject.SetActive (awakeShow);
		line = new VectorLine ("Line", new List<Vector2> (2), tex,lineWidth,LineType.Continuous);
		line.drawTransform = transform;
		line.texture = tex;
		line.rectTransform.SetParent (transform);
		line.rectTransform.anchorMin = Vector2.one * 0.5f;
		line.rectTransform.anchorMax = Vector2.one * 0.5f;
		line.rectTransform.localPosition = Vector2.zero;
		line.rectTransform.anchoredPosition = Vector2.zero;
		line.rectTransform.localScale = Vector3.one;
	}
	public void SetColor (Color color)
	{
		line.SetColor (color);	
	}

	/** 设置线宽度 */
	public void SetLineWidth(float lineWidth){
		line.SetWidth (lineWidth);
	}

	public void SetLine(Vector2 from, Vector2 to){
		line.points2 [0] = from;
		line.points2 [1] = to;
		line.Draw ();
	}

	public void Switch(bool isShow)
	{
		if (isShow)
			Show ();
		else
			Hide ();
	}

	public void Show(){
		if (_show)
			return;
		gameObject.SetActive (true);
		_show = true;
	}

	public void Hide(){
		if (!_show)
			return;
		gameObject.SetActive (false);
		_show = false;
	}
}
