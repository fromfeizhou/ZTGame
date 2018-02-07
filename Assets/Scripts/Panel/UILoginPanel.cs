using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoginPanel : MonoBehaviour
{
	private Button _btnLogin;

	private InputField _inputFieldAccount;

	private Widget _widget;

	private void Start()
	{
		GameManager.GetInstance ().Init ();
		LoginModule.GetInstance ().LoginPanel = this;

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
   