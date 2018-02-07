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

		if (!NetWorkConst.IsOpenNetWork) {
			LoginModule.GetInstance ().EnterGameScene ();
			return;
		}
		
		GameManager.GetInstance ().Init ();

		Init ();
	}

	public void Init()
	{
		_widget = Widget.Create(gameObject);
		_widget.BandingBtn_OnClick (OnBtnClick_Login,"ViewPort/Btn_Login");
		_inputFieldAccount = _widget.GetComponent<InputField> ("ViewPort/Account/InputField");
	}

	private void OnBtnClick_Login()
	{
		LoginModule.GetInstance ().NetWork_Request_Login (_inputFieldAccount.text,"");
	}

}
   