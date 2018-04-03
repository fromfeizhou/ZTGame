using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ZTButton : Button,IWidget,IPointerExitHandler
{
	#if UNITY_EDITOR
	public void InitEditor (string paramStr)
	{
	}
	#endif

	public void Init (string paramStr)
	{
		
	}

	private WidgetAnimation _WidgetAnim;
	public WidgetAnimation WidgetAnim {
		get {
			return _WidgetAnim;
		}
	}

	/** 移出按钮点击范围 */
	private bool _canClick = true;

	public bool CanClick{
		get{ 
			return _canClick;
		}
	}

	private bool isDown = false;

	// 当按钮失去焦点
	public override void OnPointerExit(PointerEventData eventData)
	{
		if (isDown) {
			base.OnPointerExit (eventData);
			_canClick = false;
		}
	}

	public override void OnPointerClick (PointerEventData eventData)
	{
		
	}

	public override void OnPointerUp (PointerEventData eventData)
	{
		if (isDown) {
			base.OnPointerUp (eventData);
			if (_canClick && onClick != null) {
				onClick.Invoke ();
			}
			_canClick = true;
		}
		isDown = false;
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown (eventData);
		isDown = true;
	}
}
