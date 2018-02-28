using System.Net.Sockets;
using com.game.client.network.utility;

namespace com.game.client
{
	namespace network.gamescoket
	{
		public partial class GameSocket
		{
			private delegate void delegate_onUpdate ();

			private eConnectState _state = eConnectState.None;
			public eConnectState State {
				get{ return _state; }
			}

			private string _ip;
			private int _port;
			private int _connectTimeOut;

			private TcpClient _socket;

			private const int ReceiveBuffSize = 1024 * 8;

			public bool IsDispose ()
			{
				return _state == eConnectState.None;
			}

			public GameSocket ()
			{
				_receiveBuff = new byte[ReceiveBuffSize];
                _receiveMsgQue = new DoubleBuff<byte[]>();
			    _sendMsgQue = new DoubleBuff<byte[]>();
			}

			public bool IsActive ()
			{
                return _socket != null && _socket.Connected && _state == eConnectState.Connected;
			}

			public void Update ()
			{
				if (isCheckSendQue)
					OnCheckSendQue ();

				if (isCheckReceiveQue)
					OnCheckReceiveQue ();

				if (isOnReceiver)
					onReceive ();
				
			}

			bool isCheckSendQue = false;
			bool isCheckReceiveQue = false;
			bool isOnReceiver = false;
			public void Init (string ip, int port, int outTime)
			{
			    _ip = ip;
			    _port = port;
			    _connectTimeOut = outTime;
			}

			public void Dispose ()
			{
				_state = eConnectState.None;
				isCheckReceiveQue = false;
				isCheckSendQue = false;
				isOnReceiver = false;

			    if (_socket != null)
                {
					try {
                        if (_socket.Connected)
                            _socket.Close();
					} catch (System.Exception e) {
						Invoking_CallBack_OnError (eErrCode.DisconnectEx, e);
					}
                    _socket = null;
				}
			}
		}
	}
}