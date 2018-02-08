using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICreateRolePanel : MonoBehaviour {
	private Widget _widget;

	void Start(){
		Init ();
	}
	public void Init()
	{
		_widget = Widget.Create(gameObject);
		for (int i = 0; i < 4; i++) {
			int idx = i;			
			_widget.BandingBtn_OnClick (() => OnBtnClick_CreateRole (idx), "ViewPort/Item_" + idx + "/Button");
		}
	}

	private void OnBtnClick_CreateRole(int job)
	{
		string tmpName = UnityEngine.Random.Range (10000, 99999).ToString ();
		LoginModule.GetInstance ().NetWork_Request_CreateRole (tmpName,(uint)job);
	}

	public void Show()
	{
		gameObject.SetActive (true);
	}
	public void Hide()
	{
		gameObject.SetActive (false);
	}
}
