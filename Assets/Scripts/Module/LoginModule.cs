using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.game.client.network;
using UnityEngine.SceneManagement;

public class LoginModule : Singleton<LoginModule> {

	public UILoginPanel LoginPanel;
	public UICreateRolePanel CreateRolePanel;

	/** 服务器版本号 */
	private string _serverVersion;
	private gprotocol.login_login_s2c _loginInfo;

	/** 请求登录服务器 */
	public void NetWork_Request_Login(string accName, string passWord)
	{
		if (string.IsNullOrEmpty (accName))
			return;
		gprotocol.login_login_c2s vo = new gprotocol.login_login_c2s ()
		{
			accname = accName, 		// 用户名
			key = passWord, 			// 验证key
			fcm = uint.MinValue, 		// 防沉迷信息(暂不使用)
			serv_id = uint.MinValue, 	// 服务器ID
			platform = uint.MinValue, 	// 平台，为0则为本地开发
			token = string.Empty, 		// 验证数据串
			device_id = string.Empty, 	// 设备ID
			device_desc = string.Empty, // 设备描述
			product_code = string.Empty, // quickSDK产品code
			uid = string.Empty, 		// quickSDK的uid
			nick_name = "互撸娃的哥哥", 	// 昵称
		};
		NetWorkManager.Instace.SendNetMsg(Module.login,Command.login_login,vo);
	}


	public void NetWork_Request_CreateRole(string name, uint job){
		gprotocol.login_create_role_c2s roleVO = new gprotocol.login_create_role_c2s ();
		roleVO.name = name;	// 角色名字
		roleVO.job = job;	// 职业
		roleVO.sex = 1;		// 性别 1男 2女
		roleVO.serv_id = 0;	// 服务器ID
		NetWorkManager.Instace.SendNetMsg (Module.login, Command.login_create_role, roleVO);
	}

	public void NetWork_Request_SelectRole(uint roleId){

		gprotocol.login_select_role_c2s vo = new gprotocol.login_select_role_c2s (){ 
			id = roleId
		};
		NetWorkManager.Instace.SendNetMsg (Module.login, Command.login_select_role, vo);
	}

	public void OnReceive_Login(gprotocol.login_login_s2c loginInfo)
	{
		if (loginInfo == null) {
			return;
		}
		_loginInfo = loginInfo;
		NetWork_Request_SelectRole (loginInfo.login_info[0].id);
	}

	public void OnReceive_CreateRole(gprotocol.login_create_role_s2c createRole){
		NetWork_Request_SelectRole (createRole.id);
	}

	public void OnReceive_SelectRole(gprotocol.login_select_role_s2c selectRole)
	{
		_serverVersion = selectRole.version;
		PlayerModule.GetInstance ().SetRoleInfo (selectRole.role[0]);
		HidePanel_CreateRole ();
		EnterGameScene ();
	}

	public void EnterGameScene(){
		LoginPanel.StartCoroutine (coEnterGameScene());
	}

	private IEnumerator coEnterGameScene()
	{
		AsyncOperation op = SceneManager.LoadSceneAsync ("SkillTestScene");
		yield return new WaitUntil (()=>op.isDone);
		LoginPanel.gameObject.SetActive (false);
		LoginPanel.transform.parent.Find ("MainPanel").gameObject.SetActive (true);
		GameManager.GetInstance ().GameStart ();
	}

	public void ShowPanel_CreateRole(){
		CreateRolePanel.Show ();
	}

	public void HidePanel_CreateRole(){
		CreateRolePanel.Hide ();
	}

	private void ShowErr(string err)
	{
		Debug.LogError (err);
	}
}
