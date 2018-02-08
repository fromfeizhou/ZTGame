namespace com.game.client
{
	namespace network.gamescoket
	{
		public partial class GameSocket
		{
			public void DisConnect (eErrCode errCode)
			{
			    Dispose();
				Invoking_CallBack_OnDisConnect (errCode);
			}
		}
	}
}