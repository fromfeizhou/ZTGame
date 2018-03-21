using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[XLua.LuaCallCSharp]
public class ZTButtonTxt : ZTButton {
	public ZTText txtBtnName;

	void Awake()
	{
		txtBtnName = transform.Find ("ZTText").GetComponent<ZTText> ();
	}

	public void SetBtnName(string btnName)
	{
		txtBtnName.text = btnName;
	}
}
