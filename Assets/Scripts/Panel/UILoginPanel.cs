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
	private Text _txtCurServerName;
	private Widget _widget;
	private Transform _serverItem_Prefab;
	private GameObject _goServerListPanel;

	IEnumerator Start()
	{
		LoginModule.GetInstance ().LoginPanel = this;
		LoginModule.GetInstance().CreateRolePanel = transform.parent.Find ("UICreateRolePanel").GetComponent<UICreateRolePanel> ();
		_widget = Widget.Create(gameObject);
		_widget.BandingBtn_OnClick (OnBtnClick_Login,"ViewPort_Login/Btn_Login");
		_widget.BandingBtn_OnClick (OnBtnClick_LoginWithOutNet,"ViewPort_Login/Btn_LoginWithOutNet");
		_widget.BandingBtn_OnClick (OnBtnClick_SelectServerList,"ViewPort_Login/Group_CurServer/Btn_SelectServer");
		_inputFieldAccount = _widget.GetComponent<InputField> ("ViewPort_Login/IF_Account/InputField");
		_serverItem_Prefab = _widget.GetComponent<Transform> ("ViewPort_ServerList/Scroll View/Viewport/Content/ServerItem");
		_txtCurServerName = _widget.GetComponent<Text> ("ViewPort_Login/Group_CurServer/Txt_CurServer");
		_goServerListPanel = _widget.GetComponent<Transform> ("ViewPort_ServerList").gameObject;
		yield return InitServerList ();
	}

	private IEnumerator InitServerList(){
		WWW www = new WWW (NetWorkConst.ServerListPath);
		yield return www;
		if (!string.IsNullOrEmpty (www.error)) {
			Debug.LogError (www.error);
			yield break;
		}
		string[] serverStr = www.text.Trim ().Split ('\n');
		for (int i = 0; i < serverStr.Length; i++) {
			AddServerItem (new UILoginPanel_ServerItem.sItemData (serverStr[i]));
		}

		if (curSelectServerItemData == null) {
			if (PlayerPrefs.HasKey (playerprefasKey)) {
				curSelectServerItemData = new UILoginPanel_ServerItem.sItemData (PlayerPrefs.GetString (playerprefasKey));
			} else {
				curSelectServerItemData = serverItemList [0].ItemData;
			}
		}
		SelectServerItem (curSelectServerItemData);
	}

	private string playerprefasKey = "PrevSelectServerData";
	private List<UILoginPanel_ServerItem> serverItemList = new List<UILoginPanel_ServerItem> ();
	private UILoginPanel_ServerItem.sItemData curSelectServerItemData;
	private void AddServerItem(UILoginPanel_ServerItem.sItemData serverItemData)
	{
		Transform tmpServerItem = Transform.Instantiate (_serverItem_Prefab,_serverItem_Prefab.parent);
		UILoginPanel_ServerItem serverItem = tmpServerItem.GetComponent<UILoginPanel_ServerItem>();
		serverItem.Init (serverItemData,SelectServerItem);
		tmpServerItem.gameObject.SetActive (true);
		tmpServerItem.name = serverItemData.ToString ();
		serverItemList.Add (serverItem);
	}

	private void SelectServerItem(UILoginPanel_ServerItem.sItemData serverItemData){
		curSelectServerItemData = serverItemData;
		for (int i = 0; i < serverItemList.Count; i++) {
			serverItemList [i].Update_ServerName (serverItemList [i].ItemData.Ip.Equals(serverItemData.Ip));
		}
		PlayerPrefs.SetString (playerprefasKey,curSelectServerItemData.ToString());
		_txtCurServerName.text = curSelectServerItemData.Name;
		HidePanel_ServerList ();
	}

	public void ShowPanel_ServerList()
	{
		_goServerListPanel.SetActive (true);	
	}

	public void HidePanel_ServerList()
	{
		_goServerListPanel.SetActive (false);	
	}

	private void OnBtnClick_SelectServerList(){
		ShowPanel_ServerList ();
	}

	private void OnBtnClick_Login()
	{
		if (string.IsNullOrEmpty (_inputFieldAccount.text))
			_inputFieldAccount.text = "Test";
		LockPanel.Show ("正在连接中");

		NetWorkConst.IsOpenNetWork = true;
		NetWorkManager.GetInstance ().SetNetWorkAddress (curSelectServerItemData.Ip,curSelectServerItemData.Port);
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
   