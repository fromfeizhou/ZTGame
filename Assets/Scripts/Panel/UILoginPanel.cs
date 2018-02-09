using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.game.client.network;

public class UILoginPanel : MonoBehaviour
{
	private Button _btnLogin;

	private InputField _inputFieldAccount;

	private Widget _widget;

	private void Start()
	{
		LoginModule.GetInstance ().LoginPanel = this;
		Init ();
	}

	public void Init()
	{
		LoginModule.GetInstance().CreateRolePanel = transform.parent.Find ("UICreateRolePanel").GetComponent<UICreateRolePanel> ();
		_widget = Widget.Create(gameObject);
		_widget.BandingBtn_OnClick (OnBtnClick_Login,"ViewPort/Btn_Login");
		_widget.BandingBtn_OnClick (OnBtnClick_LoginWithOutNet,"ViewPort/Btn_LoginWithOutNet");
		_inputFieldAccount = _widget.GetComponent<InputField> ("ViewPort/Account/InputField");
	}

	private void OnBtnClick_Login()
	{
		NetWorkConst.IsOpenNetWork = true;
		GameManager.GetInstance ().Init ();

		LoginModule.GetInstance ().NetWork_Request_Login (_inputFieldAccount.text,"");
	}

	private void OnBtnClick_LoginWithOutNet()
	{
		NetWorkConst.IsOpenNetWork = false;
		GameManager.GetInstance ().Init ();

		LoginModule.GetInstance ().EnterGameScene ();
	}

}
   