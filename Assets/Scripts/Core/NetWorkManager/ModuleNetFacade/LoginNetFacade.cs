using gprotocol;
namespace com.game.client
{
	namespace network.facade
	{
		[NetFacadeAttribute(Module.login)]
		public class LoginNetFacade
		{
			[NetCommandAttribute(Command.login_login)]
			private void OnReceive_Login_Login(int code, login_login_s2c vo)
			{
				UnityEngine.Debug.Log ("[" + System.DateTime.Now + "]" + "[OnReceive_Login_Login]:心跳间隔:" + vo.heart_interval);
				NetWorkManager.Instace.SetHeartInterval (vo.heart_interval);
				NetWorkManager.Instace.HeartSwitch (true);
				NetWorkManager.Instace.CheckErrCode (vo.code);
				LoginModule.GetInstance ().LoginPanel.LockPanel.Hide ();

				if (vo.code == 0) {
					LoginModule.GetInstance ().OnReceive_Login (vo);
				} else {
					if (vo.code == 12) {//没有角色
						LoginModule.GetInstance ().ShowPanel_CreateRole ();
					}
				}
			}

			[NetCommandAttribute(Command.login_select_role)]
			private void OnReceive_Login_SelectRole(int code, login_select_role_s2c vo){

				NetWorkManager.Instace.CheckErrCode (vo.code);
				UnityEngine.Debug.Log ("[" + System.DateTime.Now +  "]" + "[OnReceive_Login_SelectRole]:roleID--" + vo.role[0].id);
				LoginModule.GetInstance ().OnReceive_SelectRole (vo);
			}


			[NetCommandAttribute(Command.login_wait_info)]
			private void OnReceive_Wait_Info(int code, login_wait_info_s2c vo)
			{
				//UnityEngine.Debug.Log ("[" + System.DateTime.Now +  "]" + "[OnReceive_Wait_Info]:OnReceive_Wait_Info");
			}
		}
	}
}