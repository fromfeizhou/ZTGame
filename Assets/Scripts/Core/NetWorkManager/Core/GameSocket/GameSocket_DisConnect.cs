namespace com.game.client
{
	namespace network.gamescoket
	{
		public partial class GameSocket
		{
			public void DisConnect ()
			{
			    Dispose();
				Invoking_CallBack_OnDisConnect ();
			}
		}
	}
}