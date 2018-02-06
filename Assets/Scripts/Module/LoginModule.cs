using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.game.client.network;
using UnityEngine.SceneManagement;

public class LoginModule : Singleton<LoginModule> {

	public UILoginPanel LoginPanel;

	/** 请求登录服务器 */
	public void NetWork_Request_Login(string accName, string passWord)
	{
		LoginPanel.StartCoroutine (EnterGameScene());
		return;


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


	private IEnumerator EnterGameScene()
	{
		yield return SceneManager.LoadSceneAsync ("SkillTestScene");	
		yield return null;
		LoginPanel.gameObject.SetActive (false);
		LoginPanel.transform.parent.Find ("MainPanel").gameObject.SetActive (true);
		//GameObject.Find ("HpPanel").gameObject.SetActive (true);

		GameManager.GetInstance ().GameStart ();
	}
}
