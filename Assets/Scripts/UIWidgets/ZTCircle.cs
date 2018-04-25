using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

[LuaCallCSharp]
public class ZTCircle : MonoBehaviour {

	/** 计算优化，周长基数，周长：2 * PI * R */
	private const float BasePerimeter = 2.0f * Mathf.PI * 1.0025f;
	/** 计算优化，边长优化*/
	private float _lineLenBase;

	private List<RectTransform> rectLineList = new List<RectTransform> ();

	private float _lineWidth = 1.5f;

	private float _angle;

	private float _radius;

	private bool _show = false;
	public Color color;
	private RectTransform rectTransform;
	public Sprite lineSprite;

	/** 初始化 */
	public void Init(int lineCnt, bool awakeShow = false, float radius = 20)
	{
		rectTransform = transform as RectTransform;
		_show = awakeShow;
		SetLineNum (lineCnt);
		SetRadius (radius);
		SetColor (color);
	}

	/** 设置颜色 */
	public void SetColor(Color color)
	{
		Image[] imgLine = transform.GetComponentsInChildren<Image> (true);
		for (int i = 0; i < imgLine.Length; i++) {
			imgLine [i].color = color;
		}
	}

	/** 设置线宽度 */
	public void SetLineWidth(float lineWidth){
		if (_lineWidth == lineWidth)
			return;
		
		_lineWidth = lineWidth;
		for (int i = 0; i < rectLineList.Count; i++) {
			float width = _lineWidth;
			float hight = rectLineList [i].sizeDelta.y;
			rectLineList [i].sizeDelta = new Vector2 (width,hight);
		}
	}

	/** 设置线段数 */
	public void SetLineNum(int lineCnt)
	{
		if (lineCnt == rectLineList.Count)
			return;
		
		int divCnt = lineCnt - rectLineList.Count;
		bool isAdd = divCnt > 0;
		for (int i = 0; i < Mathf.Abs(divCnt); i++) {
			if (isAdd) {
				GameObject go = new GameObject ("Line_" + i.ToString());
				RectTransform lineRect = go.AddComponent<RectTransform> ();
				Image lineImg = go.AddComponent<Image> ();
				if (lineSprite != null)
					lineImg.sprite = lineSprite;
				lineImg.raycastTarget = false;
				lineRect.SetParent (transform);
				lineRect.localPosition = Vector3.zero;
				lineRect.localScale = Vector3.one;
				rectLineList.Add (lineRect);
				go.SetActive (_show);
			}
			else{
				RectTransform lineRect = rectLineList [0];
				GameObject.Destroy (lineRect.gameObject);
				rectLineList.RemoveAt (0);
			}
		}

		_angle = 360f / rectLineList.Count;
		_lineLenBase = BasePerimeter / rectLineList.Count;
	}

	/** 设置半径 */
	public void SetRadius(float radius)
	{
		if (_radius == radius)
			return;
		
		_radius = radius;

		float len = _lineLenBase * _radius;
		for (int i = 0; i < rectLineList.Count; i++) {
			Vector2 pos = Vector2.zero;
			pos.x = Mathf.Cos (i * _angle * Mathf.Deg2Rad) * _radius;
			pos.y = Mathf.Sin (i * _angle * Mathf.Deg2Rad) * _radius;
			rectLineList [i].anchoredPosition = pos;
			rectLineList [i].sizeDelta = new Vector2 (_lineWidth,len);
			if (i == 0) 
			{
				rectLineList [i].localEulerAngles = Vector3.zero;
			}
			else
			{
				rectLineList [i].localEulerAngles = Vector3.forward * i * _angle;
			}
		}
	}

	public void SetPos(Vector2 pos){
		rectTransform.anchoredPosition = pos;
	}

	public void SetCircle(Vector2 pos, float radius){
		SetPos (pos);
		SetRadius (radius);
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
		for (int i = 0; i < rectLineList.Count; i++)
			rectLineList [i].gameObject.SetActive (true);
		_show = true;
	}

	public void Hide(){
		if (!_show)
			return;
		for (int i = 0; i < rectLineList.Count; i++)
			rectLineList [i].gameObject.SetActive (false);
		_show = false;
	}
}
