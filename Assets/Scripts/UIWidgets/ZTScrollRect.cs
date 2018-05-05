using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ZTScrollRect : ScrollRect {
	public UnityAction<PointerEventData> onEndDrag;
	public override void OnEndDrag (PointerEventData eventData)
	{
		base.OnEndDrag (eventData);
		//if (onEndDrag != null)
		//	onEndDrag (eventData);
	}

	public override void OnDrag (PointerEventData eventData)
	{
		base.OnDrag (eventData);
		if (onEndDrag != null)
			onEndDrag (eventData);
	}
}
