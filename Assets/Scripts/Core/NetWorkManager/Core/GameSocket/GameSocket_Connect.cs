using System.Net;
using System.Net.Sockets;

namespace com.game.client
{
	namespace network.gamescoket
	{
		public partial class GameSocket
		{
		    private float _curTime;

			public void Connect ()
			{
                if(IsActive())
                    return;

				if (_state != eConnectState.None) {
					Invoking_CallBack_OnError (eErrCode.ConnectStateErr,null);
					return;
				}
				try {
					_state = eConnectState.Connecting;

                    AddressFamily family = GetFamily();
				    _socket = new TcpClient(family);
				    _socket.NoDelay = true;

					ResetConnectTimeOut ();
					delegate_TimeOut (true);

					UnityEngine.Debug.Log("[" + System.DateTime.Now + "]" + "[" + this.GetType().Name + "]请求连接服务器. Ip:" + _ip + ", Port:" + _port);
				    _socket.BeginConnect(_ip, _port, ar =>
                    {
						TcpClient client = (TcpClient)ar.AsyncState;
						try {
							client.EndConnect (ar);
							_state = eConnectState.Connected;
							delegate_TimeOut (false);
						    if (_socket.Connected)
                            {
								Invoking_CallBack_OnConnect();
								delegate_OnReceiver (true);
							    delegate_CheckSendQue(true);
								delegate_CheckReceiveQue (true);
							}
						} catch (System.Exception e) {
							delegate_TimeOut (false);
							_state = eConnectState.ConnectionRefused;
							Invoking_CallBack_OnError(eErrCode.ConnectRefused, e);
							Dispose ();
						}
                    }, _socket);
				} catch (System.Exception e) {
					Invoking_CallBack_OnError(eErrCode.CreateSocket, e);
					Dispose ();
				}
			}


			private void ResetConnectTimeOut ()
			{
				_curTime = 0;
			}

            private void OnTimeOut()
			{
				if (_curTime < _connectTimeOut) {
					_curTime += UnityEngine.Time.deltaTime;
				} else {
					Invoking_CallBack_OnError (eErrCode.ConnectOutTime, null);
					Dispose ();
				}
			}

		    private AddressFamily GetFamily()
		    {
		        IPAddress[] socketAddress = Dns.GetHostAddresses(_ip);
		        AddressFamily family = AddressFamily.InterNetwork;
		        if (socketAddress[0].AddressFamily == AddressFamily.InterNetwork)
		        {
		            family = AddressFamily.InterNetwork;
		        }
		        else if (socketAddress[0].AddressFamily == AddressFamily.InterNetworkV6)
		        {
		            family = AddressFamily.InterNetworkV6;
		        }
		        return family;
		    }
		}
	}
}