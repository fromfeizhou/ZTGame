using gprotocol;


namespace com.game.client
{
	namespace network.facade
	{
		[NetFacadeAttribute(Module.map)]
		public class MapNetFacade
		{
			[NetCommandAttribute(Command.map_switch)]
			private void OnReceive_Map_Switch(int code, map_switch_s2c vo)
			{
				UnityEngine.Debug.Log ("请求场景切换(暂时没用)");
			}

			[NetCommandAttribute(Command.map_role_leave)]
			private void OnReceive_Map_Role_Leave(int code, map_role_leave_s2c vo ){
				UnityEngine.Debug.Log ("玩家离开视野");
			}
		}
	}
}