using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ZTButtonLongPress : Button,IWidget
{
	public WidgetAnimation WidgetAnim {
		get {
			throw new System.NotImplementedException ();
		}
	}

	#if UNITY_EDITOR
	public void InitEditor (string paramStr)
	{
	}
	#endif

	public void Init (string paramStr)
	{
		
	}

	public UnityAction onUp;
	public UnityAction onDown;

	// 当按钮失去焦点
	public override void OnPointerClick (PointerEventData eventData)
	{
	}

	public override void OnPointerUp (PointerEventData eventData)
	{
		base.OnPointerUp (eventData);
		if (onUp != null)
			onUp.Invoke();
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown (eventData);
		if (onDown != null)
			onDown.Invoke ();
	}

	[System.Serializable]
	public class ButtonOnUp : UnityEvent
	{
	}

	[System.Serializable]
	public class ButtonOnDown : UnityEvent
	{
	}
}
