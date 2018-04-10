using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;
using Vectrosity;

[LuaCallCSharp]
public class ZTCircle : MonoBehaviour {
	private VectorLine line;
	public Texture tex;
	private List<RectTransform> rectLineList = new List<RectTransform> ();

	private bool _show = false;

	/** 初始化 */
	public void Init(int lineCnt,  bool awakeShow = false, float lineWidth = 6.0f, Vector2 pos = default(Vector2), float radius = 20)
	{
		Debug.Log ("lineWidth>>>>>>>>>>>>" + lineWidth);
		gameObject.SetActive (awakeShow);
		line = new VectorLine ("Circle", new List<Vector2> (lineCnt + 1), tex,lineWidth,LineType.Continuous);
		line.texture = tex;
		line.rectTransform.SetParent (transform);
		line.rectTransform.anchorMin = Vector2.one * 0.5f;
		line.rectTransform.anchorMax = Vector2.one * 0.5f;
		line.rectTransform.anchoredPosition = Vector2.zero;
		line.rectTransform.localScale = Vector3.one;
		SetCircle (pos, radius);
	}
	public void SetColor (Color color)
	{
		line.SetColor (color);	
	}

	/** 设置线宽度 */
	public void SetLineWidth(float lineWidth){
		line.SetWidth (lineWidth);
	}

	/** 设置半径 */
	public void SetCircle(Vector2 pos, float radius)
	{
		line.MakeCircle (pos,radius);
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
