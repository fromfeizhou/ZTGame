using gprotocol;
namespace com.game.client
{
	namespace network.facade
	{
		[NetFacadeAttribute(Module.login)]
		public class LoginNetFacade
		{
			[NetCommandAttribute(Command.login_heart)]
			private void OnReceive_Login_Heart(int code, login_heart_s2c vo)
			{
				//UnityEngine.Debug.Log ("[" + System.DateTime.Now +  "]" + "[OnReceive_Login_Heart]:心跳");
			}

			[NetCommandAttribute(Command.login_login)]
			private void OnReceive_Login_Login(int code, login_login_s2c vo)
			{
				if (vo.code == 0) {
					string info = string.Empty;
					for (int i = 0; i < vo.login_info.Count; i++) {
						info += "Id:" + vo.login_info [i].id + ", ";
						info += "Name:" + vo.login_info [i].name + ", ";
						info += "Job:" + vo.login_info [i].job + ", ";
						info += "Lv.:" + vo.login_info [i].level + ", ";
						info += "Rank:" + vo.login_info [i].rank + ", ";
						info += "Sex:" + vo.login_info [i].sex + ", ";
						info += "LastLoginTime:" + vo.login_info [i].last_login_time + ", ";
						info += "ServerID:" + vo.login_info [i].server_id + ", ";
						info += "Platform:" + vo.login_info [i].platform + ", ";
						info += "PlatformServID:" + vo.login_info [i].platform_serv_id + '\n';
					}
					UnityEngine.Debug.Log ("[" + System.DateTime.Now + "]" + "[OnReceive_Login_Login]登陆成功：\n" + info);
				} else {
					UnityEngine.Debug.Log ("[" + System.DateTime.Now + "]" + "[OnReceive_Login_Login]登录错误. errCode:" + vo.code);
					if (vo.code == 12)//没有角色
					{
						login_create_role_c2s roleVO = new login_create_role_c2s()
						{
							name = "77731",	// 角色名字
							model = 1, 			// 职业
							sex = 1, 			// 性别 1男 2女
							serv_id = 0, 		// 服务器ID
						};	
						NetWorkManager.Instace.SendNetMsg (Module.login, Command.login_create_role, roleVO);
					}
				}
			}

			[NetCommandAttribute(Command.login_create_role)]
			private void OnReceive_Login_Create_Role(int code, login_create_role_s2c vo){
				if (vo.code == 0) {
					UnityEngine.Debug.Log ("[" + System.DateTime.Now + "]" + "[OnReceive_Login_Login]创建角色成功！。角色ID:" + vo.id);
				} else {
					UnityEngine.Debug.Log ("[" + System.DateTime.Now + "]" + "[OnReceive_Login_Login]创建角色. errCode:" + vo.code);
				}
			}

			[NetCommandAttribute(Command.login_select_role)]
			private void OnReceive_Login_SelectRole(int code, login_select_role_s2c vo){
				if (vo.code == 0) {
					string info = "ServerVer:" + vo.version +", ";
					for (int i = 0; i < vo.role.Count; i++) {
						info += "Id:" + vo.role[i].id +", ";
						info += "Name:" + vo.role[i].name +", ";
						info += "Job:" + vo.role[i].job +", ";
						info += "Lv.:" + vo.role [i].level +", ";
						info += "Rank:" + vo.role[i].rank +", ";
						info += "Sex:" + vo.role[i].sex +", ";
						info += "MapId:" + vo.role[i].map_id +", ";
						info += "Pos:" + string.Format ("MapId:{0}:{{{1},{2},{3}}}", vo.role [i].map_id, vo.role [i].x, vo.role [i].y, vo.role [i].z);
					}
					UnityEngine.Debug.Log ("[" + System.DateTime.Now + "]" + "[OnReceive_Login_SelectRole]选择角色：\n" + info);
				}
				else
					UnityEngine.Debug.Log ("[" + System.DateTime.Now + "]" + "[OnReceive_Login_SelectRole]选择角色. errCode:" + vo.code);
				
			}


			[NetCommandAttribute(Command.login_auth_key)]
			private void OnReceive_Auth_Key(int code, login_auth_key_s2c vo)
			{
				UnityEngine.Debug.Log ("[" + System.DateTime.Now +  "]" + "[OnReceive_Auth_Key]:加密参数, 开启心跳，频率：" + NetWorkConst.Heart_Time + " 秒一次");
				NetWorkManager.Instace.HeartSwitch (true);
			}

			[NetCommandAttribute(Command.login_wait_info)]
			private void OnReceive_Wait_Info(int code, login_wait_info_s2c vo)
			{
				//UnityEngine.Debug.Log ("[" + System.DateTime.Now +  "]" + "[OnReceive_Wait_Info]:排队信息");
			}
		}
	}
}