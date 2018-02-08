using gprotocol;
namespace com.game.client
{
	namespace network.facade
	{
		[NetFacadeAttribute(Module.activity)]
		public class ActivityNetFacade
		{
			[NetCommandAttribute(Command.activity_update_activity)]
			private void OnReceive_Activity_Update_Activity(int code, activity_update_activity_s2c vo)
			{
			}
		}
	}
}