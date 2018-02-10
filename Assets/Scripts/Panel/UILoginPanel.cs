using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.game.client.network;

public class UILoginPanel : MonoBehaviour
{
	public UILockPanel LockPanel;
	private Button _btnLogin;

	private InputField _inputFieldAccount;

	private Widget _widget;

	void Start()
	{
		LoginModule.GetInstance ().LoginPanel = this;
		LoginModule.GetInstance().CreateRolePanel = transform.parent.Find ("UICreateRolePanel").GetComponent<UICreateRolePanel> ();
		_widget = Widget.Create(gameObject);
		_widget.BandingBtn_OnClick (OnBtnClick_Login,"ViewPort/Btn_Login");
		_widget.BandingBtn_OnClick (OnBtnClick_LoginWithOutNet,"ViewPort/Btn_LoginWithOutNet");
		_inputFieldAccount = _widget.GetComponent<InputField> ("ViewPort/Account/InputField");
	}

	private void OnBtnClick_Login()
	{
		if (string.IsNullOrEmpty (_inputFieldAccount.text))
			_inputFieldAccount.text = "Test";
		LockPanel.Show ("正在连接中");

		NetWorkConst.IsOpenNetWork = true;
		GameManager.GetInstance ().Init ();
		StartCoroutine (WaitSendLogin());
	}

	private IEnumerator WaitSendLogin(){
		LockPanel.Show ("用戶:" + _inputFieldAccount.text + " 正在登陆游戏");
		yield return new WaitForSeconds (1.0f);
		LoginModule.GetInstance ().NetWork_Request_Login (_inputFieldAccount.text,"");
	}

	private void OnBtnClick_LoginWithOutNet()
	{
		NetWorkConst.IsOpenNetWork = false;
		GameManager.GetInstance ().Init ();
		LoginModule.GetInstance ().EnterGameScene ();

	}

}
   