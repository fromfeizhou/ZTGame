using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidgetAnimation
{
	private WidgetAnimation()
	{
	}

	public static WidgetAnimation Create(GameObject widget)
	{
		return new WidgetAnimation ();
	}
}
